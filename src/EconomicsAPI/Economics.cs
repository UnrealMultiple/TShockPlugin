using EconomicsAPI.Configured;
using EconomicsAPI.DB;
using EconomicsAPI.EventArgs.PlayerEventArgs;
using EconomicsAPI.Events;
using EconomicsAPI.Extensions;
using EconomicsAPI.Model;
using EconomicsAPI.Utils;
using Microsoft.Xna.Framework;
using Rests;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace EconomicsAPI;

[ApiVersion(2, 1)]
public class Economics : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("提供经济系统API");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 5);

    public readonly static List<TSPlayer> ServerPlayers = new();

    private readonly ConcurrentDictionary<NPC, Dictionary<Player, float>> Strike = new();

    public static string SaveDirPath => Path.Combine(TShock.SavePath, "Economics");

    internal readonly string ConfigPATH = Path.Combine(SaveDirPath, "Economics.json");

    public static CurrencyManager CurrencyManager { get; private set; } = null!;

    private long TimerCount;

    private readonly Dictionary<TSPlayer, PingData> PlayerPing = new();

    public static Setting Setting { get; private set; } = new();

    public Economics(Main game) : base(game)
    {
        if (!Directory.Exists(SaveDirPath))
        {
            Directory.CreateDirectory(SaveDirPath);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnKillNpc);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.KillMe.UnRegister(this.OnKillMe);
            PlayerHandler.OnPlayerCountertop -= this.PlayerHandler_OnPlayerCountertop;
            GeneralHooks.ReloadEvent -= this.LoadConfig;
        }
        base.Dispose(disposing);
    }

    public override void Initialize()
    {
        this.LoadConfig();
        CurrencyManager = new CurrencyManager();
        Helper.InitPluginAttributes();
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnKillNpc);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.KillMe.Register(this.OnKillMe);
        PlayerHandler.OnPlayerCountertop += this.PlayerHandler_OnPlayerCountertop;
        GeneralHooks.ReloadEvent += this.LoadConfig;
    }

    private void LoadConfig(ReloadEventArgs? args = null)
    {
        if (!File.Exists(this.ConfigPATH))
        {
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
            Setting.CustomizeCurrencys.Add(new());
        }
        Setting = ConfigHelper.LoadConfig(this.ConfigPATH, Setting);
    }

    private void PlayerHandler_OnPlayerCountertop(PlayerCountertopArgs args)
    {
        args.Messages.Add(new(GetString($"当前延迟: {args.Ping.GetPing():F1}ms"), 7));
        args.Messages.Add(new(GetString($"玩家名称: {args.Player!.Name}"), 1));
        foreach (var currency in Setting.CustomizeCurrencys)
        {
            args.Messages.Add(new(GetString($"{currency.Name}数量: {CurrencyManager.GetUserCurrency(args.Player.Name, currency.Name)}"), 3));
        }
        args.Messages.Add(new(GetString($"在线人数: {TShock.Utils.GetActivePlayerCount()}/{Main.maxPlayers}"), 4));
        args.Messages.Add(new(GetString($"世界名称: {Main.worldName}"), 9));
        args.Messages.Add(new(GetString($"当前生命: {args.Player.TPlayer.statLife}/{args.Player.TPlayer.statLifeMax}"), 5));
        args.Messages.Add(new(GetString($"当前魔力: {args.Player.TPlayer.statMana}/{args.Player.TPlayer.statManaMax}"), 6));
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        foreach (var currency in Setting.CustomizeCurrencys)
        {
            if (currency.DeathFallOption.Enable)
            {
                var drop = CurrencyManager.GetUserCurrency(e.Player.Name, currency.Name).Number * currency.DeathFallOption.DropRate;
                CurrencyManager.DeductUserCurrency(e.Player.Name, Convert.ToInt64(drop), currency.Name);
                e.Player.SendErrorMessage(GetString($"你因死亡掉落{drop:F0}个{currency.Name}!"));
            }
        }
    }

    private void OnUpdate(System.EventArgs args)
    {
        this.TimerCount++;
        if (this.TimerCount % 60 == 0)
        {
            lock (ServerPlayers)
            {
                foreach (var ply in ServerPlayers)
                {
                    if (ply == null || !ply.Active)
                    {
                        continue;
                    }

                    this.Ping(ply, data =>
                    {
                        var status = new PlayerCountertopArgs()
                        {
                            Ping = data,
                            Player = data.TSPlayer
                        };
                        if (Setting.StatusText && !PlayerHandler.PlayerCountertopUpdate(status))
                        {
                            Helper.CountertopUpdate(status);
                        }
                    });
                }
            }
        }
        if (this.TimerCount % (60 * Setting.SaveTime) == 0)
        {
            CurrencyManager.UpdataAll();
            foreach (var (npc, _) in this.Strike.Where(x => x.Key == null || !x.Key.active).ToList())
            {
                this.Strike.Remove(npc, out var _);
            }
        }
    }

    private void OnStrike(NpcStrikeEventArgs args)
    {
        if (this.Strike.TryGetValue(args.Npc, out var data) && data != null)
        {
            if (data.TryGetValue(args.Player, out var damage))
            {
                this.Strike[args.Npc][args.Player] += args.Damage;
            }
            else
            {
                this.Strike[args.Npc][args.Player] = args.Damage;
            }
        }
        else
        {
            this.Strike[args.Npc] = new()
            {
                { args.Player, args.Damage }
            };
        }
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        var npc = Main.npc[args.NpcId];
        if (npc != null)
        {
            this.Strike[npc] = new();
        }
    }

    private void OnKillNpc(NpcKilledEventArgs args)
    {
        if ((args.npc.SpawnedFromStatue && Setting.IgnoreStatue) || args.npc == null)
        {
            return;
        }
        if (this.Strike.TryGetValue(args.npc, out var result) && result != null)
        {
            foreach (var (player, damage) in result)
            {
                if (!PlayerHandler.PlayerKillNpc(new PlayerKillNpcArgs(player, args.npc, damage)))
                {
                    foreach (var currency in Setting.CustomizeCurrencys)
                    {
                        if (currency.CurrencyObtain.CurrencyObtainType == Enumerates.CurrencyObtainType.KillNpc)
                        {
                            var num = Convert.ToInt64(damage * currency.CurrencyObtain.ConversionRate);
                            CurrencyManager.AddUserCurrency(player.name, num, currency.Name);
                            if (currency.CombatMsgOption.Enable)
                            {
                                player.SendCombatMsg($"+{num}$", new Color(currency.CombatMsgOption.Color[0], currency.CombatMsgOption.Color[1], currency.CombatMsgOption.Color[2]));
                            }
                        }
                    }
                }
            }
        }
        this.Strike.Remove(args.npc, out var _);
    }

    public void Ping(TSPlayer player, Action<PingData> action)
    {
        var data = new PingData()
        {
            TSPlayer = player,
            action = action
        };
        this.PlayerPing[player] = data;
        var num = -1;
        for (var i = 0; i < Main.item.Length; i++)
        {
            if (Main.item[i] != null && (!Main.item[i].active || Main.item[i].playerIndexTheItemIsReservedFor == 255))
            {
                num = i;
                break;
            }
        }
        NetMessage.TrySendData(39, player.Index, -1, null, num);
    }

    public static void RemoveAssemblyCommands(Assembly assembly)
    {
        Commands.ChatCommands.RemoveAll(cmd => cmd.GetType().Assembly == assembly);
    }

    public static void RemoveAssemblyRest(Assembly assembly)
    {
        if (typeof(Rest)
            .GetField("commands", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(TShock.RestApi) is List<RestCommand> rests)
        {
            rests.RemoveAll(cmd => cmd.GetType().GetField("callback", BindingFlags.NonPublic)?.GetType().Assembly == assembly);
        }
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

            if (this.PlayerPing.TryGetValue(TShock.Players[args.Msg.whoAmI], out var data))
            {
                data.End = DateTime.Now;
                data.action.Invoke(data);
                this.PlayerPing.Remove(TShock.Players[args.Msg.whoAmI]);
            }
        }
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var player = TShock.Players[args.Who];
        lock (ServerPlayers)
        {
            if (player != null)
            {
                ServerPlayers.Remove(player);
            }
        }
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        lock (ServerPlayers)
        {
            if (player != null)
            {
                ServerPlayers.Add(player);
            }
        }
    }
}