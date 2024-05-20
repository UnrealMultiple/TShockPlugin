using EconomicsAPI.Configured;
using EconomicsAPI.DB;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Events;
using EconomicsAPI.Extensions;
using EconomicsAPI.Model;
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

    private readonly Dictionary<NPC, Dictionary<Player, float>> Strike = new();

    public static string SaveDirPath => Path.Combine(TShock.SavePath, "Economics");

    internal readonly string ConfigPATH = Path.Combine(SaveDirPath, "Economics.json");

    public static CurrencyManager CurrencyManager;

    private long TimerCount;

    private Dictionary<TSPlayer, PingData> PlayerPing = new();

    public static Setting Setting { get; private set; } = new();

    public Economics(Main game) : base(game)
    {
        if (!Directory.Exists(SaveDirPath))
            Directory.CreateDirectory(SaveDirPath);
    }

    public override void Initialize()
    {
        LoadConfig();
        CurrencyManager = new CurrencyManager();
        Helper.InitPluginAttributes();
        ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreet);
        ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
        ServerApi.Hooks.NpcKilled.Register(this, OnKillNpc);
        ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        GetDataHandlers.KillMe.Register(OnKillMe);
        PlayerHandler.OnPlayerCountertop += PlayerHandler_OnPlayerCountertop;
        TShockAPI.Hooks.GeneralHooks.ReloadEvent += (_) => LoadConfig();
    }

    private void LoadConfig()
    {
        if (!File.Exists(ConfigPATH))
            Setting.GradientColor = new List<string>()
            {
                "[c/00ffbf:{0}]",
                "[c/1aecb8:{0}]",
                "[c/33d9b1:{0}]",
                "[c/A6D5EA:{0}]",
                "[c/A6BBEA:{0}]",
                "[c/B7A6EA:{0}]",
                "[c/A6EAB3:{0}]",
                "[c/D5F0AA:{0}]",
                "[c/F5F7AF:{0}]",
                "[c/F8ECB0:{0}]",
                "[c/F8DEB0:{0}]",
                "[c/F8D0B0:{0}]",
                "[c/F8B6B0:{0}]",
                "[c/EFA9C6:{0}]",
                "[c/00ffbf:{0}]",
                "[c/1aecb8:{0}]"
            };
        Setting = ConfigHelper.LoadConfig(ConfigPATH, Setting);
    }

    private void PlayerHandler_OnPlayerCountertop(PlayerCountertopArgs args)
    {
        args.Messages.Add(new($"当前延迟: {args.Ping.GetPing():F1}ms", 7));
        args.Messages.Add(new($"玩家名称: {args.Player.Name}", 1));
        args.Messages.Add(new($"{Setting.CurrencyName}数量: {CurrencyManager.GetUserCurrency(args.Player.Name)}", 3));
        args.Messages.Add(new($"在线人数: {TShock.Utils.GetActivePlayerCount()}/{Main.maxPlayers}", 4));
        args.Messages.Add(new($"世界名称: {Main.worldName}", 9));
        args.Messages.Add(new($"当前生命: {args.Player.TPlayer.statLife}/{args.Player.TPlayer.statLifeMax}", 5));
        args.Messages.Add(new($"当前魔力: {args.Player.TPlayer.statMana}/{args.Player.TPlayer.statManaMax}", 6));
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        if (Setting.DeathDropRate >= 0)
        {
            var drop = CurrencyManager.GetUserCurrency(e.Player.Name) * Setting.DeathDropRate;
            CurrencyManager.DelUserCurrency(e.Player.Name, Convert.ToInt64(drop));
            e.Player.SendErrorMessage($"你因死亡掉落{drop:F0}个{Setting.CurrencyName}!");
        }
    }

    private void OnUpdate(System.EventArgs args)
    {
        TimerCount++;
        if (TimerCount % 60 == 0)
        {
            lock (ServerPlayers)
            {
                foreach (var ply in ServerPlayers)
                {
                    Ping(ply, data =>
                    {
                        var status = new PlayerCountertopArgs()
                        {
                            Ping = data,
                            Player = data.TSPlayer
                        };
                        if (Setting.StatusText && !PlayerHandler.PlayerCountertopUpdate(status))
                            Helper.CountertopUpdate(status);
                    });
                }
            }
        }
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
            if (data.TryGetValue(args.Player, out float damage))
            {
                Strike[args.Npc][args.Player] += args.Damage;
            }
            else
            {
                Strike[args.Npc][args.Player] = args.Damage;
            }
        }
        else
        {
            Strike[args.Npc] = new()
            {
                { args.Player, args.Damage }
            };
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

    public void Ping(TSPlayer player, Action<PingData> action)
    {
        var data = new PingData()
        {
            TSPlayer = player,
            action = action
        };
        PlayerPing[player] = data;
        int num = -1;
        for (int i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i] != null && (!Main.item[i].active || Main.item[i].playerIndexTheItemIsReservedFor == 255))
            {
                num = i;
                break;
            }
        }
        NetMessage.TrySendData(39, player.Index, -1, null, num);
    }

    private void OnGetData(GetDataEventArgs args)
    {
        if (args.Handled || args.MsgID != PacketTypes.ItemOwner || TShock.Players[args.Msg.whoAmI] == null)
        {
            return;
        }
        using BinaryReader binaryReader = new(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length));
        int num = binaryReader.ReadInt16();
        if (binaryReader.ReadByte() == byte.MaxValue)
        {

            if (PlayerPing.TryGetValue(TShock.Players[args.Msg.whoAmI], out var data))
            {
                data.End = DateTime.Now;
                data.action.Invoke(data);
                PlayerPing.Remove(TShock.Players[args.Msg.whoAmI]);
            }
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        lock (ServerPlayers)
        { 
            if (player != null)
                ServerPlayers.Remove(player);
        }
        
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        lock (ServerPlayers)
        { 
            if (player != null)
                ServerPlayers.Add(player);
        }
    }
}
