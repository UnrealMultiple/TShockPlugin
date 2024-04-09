using EconomicsAPI.Configured;
using EconomicsAPI.DB;
using EconomicsAPI.EventArgs;
using EconomicsAPI.Events;
using EconomicsAPI.Extensions;
using EconomicsAPI.Utils;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace EconomicsAPI;

[ApiVersion(2, 1)]
public class Economics : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => Assembly.GetExecutingAssembly().GetName().Name!;

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;

    public override Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

    public readonly static List<TSPlayer> ServerPlayers = new();

    private readonly Dictionary<NPC, Dictionary<Player, int>> Strike = new();

    public static string SaveDirPath => Path.Combine(TShock.SavePath, "Economics");

    internal readonly string ConfigPATH = Path.Combine(SaveDirPath, "Economics.json");

    public static CurrencyManager CurrencyManager;

    private long TimerCount;

    public static Setting Setting { get; private set; } = new();

    public Economics(Main game) : base(game)
    {
        if(!Directory.Exists(SaveDirPath))
            Directory.CreateDirectory(SaveDirPath);
    }

    public override void Initialize()
    {
        Setting = ConfigHelper.LoadConfig<Setting>(ConfigPATH);
        CurrencyManager = new CurrencyManager();
        Helper.InitPluginAttributes();
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        ServerApi.Hooks.NpcKilled.Register(this, OnKillNpc);
        ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        GetDataHandlers.KillMe.Register(OnKillMe);
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) => Setting = ConfigHelper.LoadConfig(ConfigPATH, Setting);
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        if (Setting.DeathDropRate >= 0)
        { 
        
        }
    }

    private void OnUpdate(System.EventArgs args)
    {
        TimerCount++;
        if (TimerCount % (60 * Setting.SaveTime) == 0)
        {
            CurrencyManager.UpdataAll();
            foreach (var npc in Strike.Keys.Where(npc => npc == null || !npc.active || npc.life <= 0).ToList())
            {
                Strike.Remove(npc);
            }
        }
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        if (Strike.TryGetValue(args.Npc, out var data))
        {
            if (data.TryGetValue(args.Player, out int damage))
            {
                Strike[args.Npc][args.Player] += args.Damage;
            }
            Strike[args.Npc][args.Player] = args.Damage;
        }
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (npc != null)
            Strike[npc] = new();
    }

    private void OnKillNpc(NpcKilledEventArgs args)
    {
        if (args.npc.SpawnedFromStatue && Setting.IgnoreStatue)
            return;
        if (Strike.TryGetValue(args.npc, out var result))
        {
            foreach (var (player, damage) in result)
            {
                if (!PlayerHandler.PlayerKillNpc(new PlayerKillNpcArgs(player, args.npc, damage)))
                {
                    var num = Convert.ToInt64(damage * Setting.ConversionRate);
                    CurrencyManager.AddUserCurrency(player.name, num);
                    if (Setting.ShowAboveHead)
                        player.SendCombatMsg($"+{num}$", Color.AntiqueWhite);
                }    
            }
        }
        Strike.Remove(args.npc);
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null)
            ServerPlayers.Remove(player);
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null)
            ServerPlayers.Add(player);
    }
}
