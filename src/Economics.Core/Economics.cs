using Economics.Core.ConfigFiles;
using Economics.Core.DB;
using Economics.Core.EventArgs.PlayerEventArgs;
using Economics.Core.Events;
using Economics.Core.Extensions;
using Economics.Core.Model;
using Economics.Core.Utility;
using Economics.Core.Utils;
using Microsoft.Xna.Framework;
using Rests;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static System.Net.Mime.MediaTypeNames;

namespace Economics.Core;

[ApiVersion(2, 1)]
public class Economics : TerrariaPlugin
{
    public override string Author => "少司命";

    public override string Description => GetString("提供经济系统API");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(2, 0, 0, 10);

    public readonly static List<TSPlayer> ServerPlayers = [];

    private readonly ConcurrentDictionary<NPC, Dictionary<Player, float>> Strike = new();

    public static string SaveDirPath => Path.Combine(TShock.SavePath, "Economics");

    #nullable disable
    public static CurrencyManager CurrencyManager { get; private set; }
    #nullable enable

    private long TimerCount;

    private readonly Dictionary<TSPlayer, PingData> PlayerPing = [];

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
            Setting.UnLoad();
            RemoveAssemblyCommands(Assembly.GetExecutingAssembly());
            RemoveAssemblyRest(Assembly.GetExecutingAssembly());
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
            ServerApi.Hooks.NetGetData.Deregister(this, this.OnGetData);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnKillNpc);
            ServerApi.Hooks.NpcSpawn.Deregister(this, this.OnNpcSpawn);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.OnStrike);
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.TileEdit.Register(this.OnTileEdit);
            GetDataHandlers.KillMe.UnRegister(this.OnKillMe);
            PlayerHandler.OnPlayerCountertop -= this.PlayerHandler_OnPlayerCountertop;
        }
        base.Dispose(disposing);
    }

    private void OnTileEdit(object? sender, GetDataHandlers.TileEditEventArgs e)
    {
        if (e.Action != GetDataHandlers.EditAction.KillTile)
        {
            return;
        }
        foreach (var currency in Setting.Instance.CustomizeCurrencys)
        {
            if (currency.CurrencyObtain.CurrencyObtainType != Enumerates.CurrencyObtainType.KillTiile)
            {
                continue;
            }
            if (currency.CurrencyObtain.ContainsID.Count > 0 && !currency.CurrencyObtain.ContainsID.Contains(e.EditData))
            {
                continue;
            }
            var num = Convert.ToInt64(currency.CurrencyObtain.GiveCurrency * currency.CurrencyObtain.ConversionRate);
            CurrencyManager.AddUserCurrency(e.Player.Name, num, currency.Name);
            if (currency.CombatMsgOption.Enable)
            {
                e.Player.SendCombatMsg(currency.CombatMsgOption.CombatMsg, new Color(currency.CombatMsgOption.Color[0], currency.CombatMsgOption.Color[1], currency.CombatMsgOption.Color[2]));
            }
        }
    }

    public override void Initialize()
    {
        Setting.Load();
        CurrencyManager = new CurrencyManager();
        Helper.InitCommand();
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnKillNpc);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.NpcStrike.Register(this, this.OnStrike);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.KillMe.Register(this.OnKillMe);
        PlayerHandler.OnPlayerCountertop += this.PlayerHandler_OnPlayerCountertop;
    }

    private void PlayerHandler_OnPlayerCountertop(PlayerCountertopArgs args)
    {
        var playerCurrencys = CurrencyManager.GetPlayerCurrencies(args.Player!.Name);
        args.Messages.Add(new(GetString($"当前延迟: {args.Ping.GetPing():F1}ms"), 7));
        args.Messages.Add(new(GetString($"玩家名称: {args.Player!.Name}"), 1));
        args.Messages.Add(new(GetString($"货币数量: {string.Join(", ", playerCurrencys.Take(5).Select(c => c.ToString()))}{(playerCurrencys.Length > 5 ? "..." : string.Empty)}"), 3));
        args.Messages.Add(new(GetString($"在线人数: {TShock.Utils.GetActivePlayerCount()}/{Main.maxPlayers}"), 4));
        args.Messages.Add(new(GetString($"世界名称: {Main.worldName}"), 9));
        args.Messages.Add(new(GetString($"当前生命: {args.Player.TPlayer.statLife}/{args.Player.TPlayer.statLifeMax}"), 5));
        args.Messages.Add(new(GetString($"当前魔力: {args.Player.TPlayer.statMana}/{args.Player.TPlayer.statManaMax}"), 6));
    }

    private void OnKillMe(object? sender, GetDataHandlers.KillMeEventArgs e)
    {
        foreach (var currency in Setting.Instance.CustomizeCurrencys)
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
        ++TimingUtils.Timer;
        while (TimingUtils.scheduled.TryPeek(out var action, out var time))
        {
            if (time > TimingUtils.Timer)
            {
                break;
            }
            action();
            TimingUtils.scheduled.Dequeue();
        }

        this.TimerCount++;
        if (this.TimerCount % 60 == 0)
        {
            for (var i = ServerPlayers.Count - 1; i >= 0; i--)
            {
                var ply = ServerPlayers[i];
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
                    if (Setting.Instance.StatusText && !PlayerHandler.PlayerCountertopUpdate(status))
                    {
                        Helper.CountertopUpdate(status);
                    }
                });
            }
        }
        if (this.TimerCount % (60 * Setting.Instance.SaveTime) == 0)
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
            if (data.TryGetValue(args.Player, out _))
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
            this.Strike[npc] = [];
        }
    }

    private void OnKillNpc(NpcKilledEventArgs args)
    {
        if ((args.npc.SpawnedFromStatue && Setting.Instance.IgnoreStatue) || args.npc == null)
        {
            return;
        }
        if (!this.Strike.TryGetValue(args.npc, out var result) || result == null)
        {
            return;
        }
        foreach (var (player, damage) in result)
        {
            if (PlayerHandler.PlayerKillNpc(new PlayerKillNpcArgs(player, args.npc, damage)))
            {
                continue;
            }
            foreach (var currency in Setting.Instance.CustomizeCurrencys)
            {
                if (currency.CurrencyObtain.CurrencyObtainType != Enumerates.CurrencyObtainType.KillNpc)
                {
                    continue;
                }
                if (currency.CurrencyObtain.ContainsID.Count > 0 && !currency.CurrencyObtain.ContainsID.Contains(args.npc.type))
                {
                    continue;
                }
                var num = Convert.ToInt64(damage * currency.CurrencyObtain.ConversionRate);
                CurrencyManager.AddUserCurrency(player.name, num, currency.Name);
                if (currency.CombatMsgOption.Enable)
                {
                    player.SendCombatMsg(string.Format(currency.CombatMsgOption.CombatMsg, num), new Color(currency.CombatMsgOption.Color[0], currency.CombatMsgOption.Color[1], currency.CombatMsgOption.Color[2]));
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
        if (player != null)
        {
            ServerPlayers.Remove(player);
        }
    }

    private void OnGreet(GreetPlayerEventArgs args)
    {
        var player = TShock.Players[args.Who];
        if (player != null)
        {
            ServerPlayers.Add(player);
        }
    }
}