using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DamageStatistic
{
    [ApiVersion(2, 1)]
    public class DamageStatistic : TerrariaPlugin
    {
        public override string Name => "DamageStatistic-伤害统计";
        public override Version Version => new Version(1, 0, 0);
        public override string Author => "Megghy";
        public override string Description => "Show the damage caused by each player after each boss battle";
        public DamageStatistic(Main game) : base(game)
        {

        }
        public override void Initialize()
        {
            ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
            ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKill);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
                ServerApi.Hooks.NpcStrike.Deregister(this, OnStrike);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKill);
                DamageList.Clear();
            }
            base.Dispose(disposing);
        }

        private readonly Dictionary<NPC, Dictionary<string, double>> DamageList = new Dictionary<NPC, Dictionary<string, double>>();

        private void OnNpcSpawn(NpcSpawnEventArgs args)
        {
            NPC npc = Main.npc[args.NpcId];
            if (npc.boss) DamageList.Add(npc, new Dictionary<string, double>());
        }

        private void OnStrike(NpcStrikeEventArgs args)
        {
            if (DamageList.ContainsKey(args.Npc))
            {
                if (!DamageList[args.Npc].ContainsKey(args.Player.name)) DamageList[args.Npc].Add(args.Player.name, 0);
                DamageList[args.Npc][args.Player.name] += args.Damage;
            }
        }

        private void OnNpcKill(NpcKilledEventArgs args)
        {
            if (DamageList.ContainsKey(args.npc) && DamageList[args.npc].Any())
            {
                var data = DamageList[args.npc];
                double npcLifeMax = 0;
                data.ForEach(p => npcLifeMax += data[p.Key]);
                var text = new StringBuilder();
                data.Keys.ForEach(p => text.AppendLine($"{p}: [c/74F3C9:{data[p]}] <{data[p] / npcLifeMax:0.00%}>, "));
                TShock.Utils.Broadcast($"[c/74F3C9:{data.Count}] 位玩家击败了 [c/74F3C9:{args.npc.FullName}]\n{text}", new Color(247, 244, 150));
                DamageList.Remove(args.npc);
            }
        }
    }
}
