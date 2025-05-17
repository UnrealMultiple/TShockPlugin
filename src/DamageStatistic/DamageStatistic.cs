using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DamageStatistic;

[ApiVersion(2, 1)]
public class DamageStatistic : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 0, 4);
    public override string Author => "Megghy";
    public override string Description => GetString("在每次 Boss 战后显示每个玩家造成的伤害。");
    public DamageStatistic(Main game) : base(game)
    {

    }
    public override void Initialize()
    {
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKill);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKill);
            this.DamageList.Clear();
        }
        base.Dispose(disposing);
    }

    private readonly Dictionary<NPC, Dictionary<string, double>> DamageList = new Dictionary<NPC, Dictionary<string, double>>();

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (npc.boss)
        {
            this.DamageList.Add(npc, new Dictionary<string, double>());
        }
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        if (this.DamageList.ContainsKey(args.Npc))
        {
            if (!this.DamageList[args.Npc].ContainsKey(args.Player.name))
            {
                this.DamageList[args.Npc].Add(args.Player.name, 0);
            }

            this.DamageList[args.Npc][args.Player.name] += args.Damage;
        }
    }

    private void OnNpcKill(NpcKilledEventArgs args)
    {
        if (this.DamageList.ContainsKey(args.npc) && this.DamageList[args.npc].Any())
        {
            var data = this.DamageList[args.npc];
            double npcLifeMax = 0;
            data.ForEach(p => npcLifeMax += data[p.Key]);
            var text = new StringBuilder();
            data.Keys.ForEach(p => text.AppendLine($"{p}: [c/74F3C9:{data[p]}] <{data[p] / npcLifeMax:0.00%}>, "));
            TShock.Utils.Broadcast(GetString($"[c/74F3C9:{data.Count}] 位玩家击败了 [c/74F3C9:{args.npc.FullName}]\n{text}"), new Color(247, 244, 150));
            this.DamageList.Remove(args.npc);
        }
    }
}