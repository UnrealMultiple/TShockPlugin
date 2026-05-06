using Economics.Core.ConfigFiles;
using Economics.Core.DB;
using Economics.Core.EventArgs.PlayerEventArgs;
using Economics.Core.Events;
using Economics.Core.Extensions;
using Economics.Core.Model;
using Economics.Core.Services;
using Economics.Core.Utility;
using Economics.Core.Utils;
using Microsoft.Xna.Framework;
using Rests;
using System.Collections.Concurrent;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Economics.Core;

[ApiVersion(2, 1)]
public class Economics : TerrariaPlugin
{
    public override string Author => "少司命, 千亦(修复 bug)";

    public override string Description => GetString("提供经济系统API");

    public override string Name => Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(3, 0, 0, 0);

    public readonly static List<TSPlayer> ServerPlayers = [];

    private readonly ConcurrentDictionary<NPC, Dictionary<Player, float>> Strike = new();

    public static string SaveDirPath => Path.Combine(TShock.SavePath, "Economics");

    #nullable disable
    public static ICurrencyService CurrencyService { get; private set; }
    public static IExchangeService ExchangeService { get; private set; }
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
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            GetDataHandlers.TileEdit.Register(this.OnTileEdit);
            GetDataHandlers.KillMe.UnRegister(this.OnKillMe);
            GetDataHandlers.NPCStrike.UnRegister(this.OnNpcStrikeData);
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
        foreach (var currency in Setting.Instance.Currencies)
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
            CurrencyService.AddCurrency(e.Player.Name, currency.Name, num);
            if (currency.CombatMsgOption.Enable)
            {
                e.Player.SendCombatMsg(currency.CombatMsgOption.CombatMsg, new Color(currency.CombatMsgOption.Color[0], currency.CombatMsgOption.Color[1], currency.CombatMsgOption.Color[2]));
            }
        }
    }

    public override void Initialize()
    {
        Setting.Load();
        var currencyManager = new CurrencyManager();
        CurrencyService = new CurrencyService(currencyManager);
        ExchangeService = new ExchangeService(CurrencyService);

        Helper.InitCommand();
        ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
        ServerApi.Hooks.NetGetData.Register(this, this.OnGetData);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnKillNpc);
        ServerApi.Hooks.NpcSpawn.Register(this, this.OnNpcSpawn);
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.KillMe.Register(this.OnKillMe);
        GetDataHandlers.NPCStrike.Register(this.OnNpcStrikeData);
        PlayerHandler.OnPlayerCountertop += this.PlayerHandler_OnPlayerCountertop;
    }

    private void PlayerHandler_OnPlayerCountertop(PlayerCountertopArgs args)
    {
        var playerCurrencys = CurrencyService.GetPlayerCurrencyRecords(args.Player!.Name);
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
        foreach (var currency in Setting.Instance.Currencies)
        {
            if (currency.DeathFallOption.Enable)
            {
                var balanceResult = CurrencyService.GetBalance(e.Player.Name, currency.Name);
                var balance = balanceResult.IsSuccess ? balanceResult.Value : 0;
                var drop = balance * currency.DeathFallOption.DropRate;
                CurrencyService.DeductCurrency(e.Player.Name, currency.Name, Convert.ToInt64(drop));
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
            // 数据保存由 CurrencyManager 内部处理，无需显式调用
            foreach (var (npc, _) in this.Strike.Where(x => x.Key == null || !x.Key.active).ToList())
            {
                this.Strike.Remove(npc, out var _);
            }
        }
    }

    private void OnNpcStrikeData(object? sender, GetDataHandlers.NPCStrikeEventArgs e)
    {
        // 改走 GetDataHandlers.NPCStrike（TShock 在处理客户端 NpcStrike 包的阶段触发），
        // 而不是 ServerApi.Hooks.NpcStrike。区别在于：
        //   1. 前者触发时 npc.life 仍是攻前的剩余血量，可以直接用作裁剪上界，不会像
        //      ServerApi.Hooks.NpcStrike 那样因为 life 已被扣到 ≤0 而把最后一击丢掉。
        //   2. 前者拿到的 Damage 是客户端上报的 raw 武器伤害，再经由 Main.CalculateDamageNPCsTake
        //      + 防御 + Ichor + 暴击换算成实际掉血，避免 raw 与 actual 不一致带来的漂移。
        // 参考 Quinci135/SEconomy 的 WorldEconomy.NetHooks_GetData + AddNPCDamage 实现。
        if (e.Handled || e.Player == null || e.Damage <= 0)
        {
            return;
        }

        if (e.ID < 0 || e.ID >= Main.npc.Length)
        {
            return;
        }

        var npc = Main.npc[e.ID];
        if (npc == null || !npc.active || npc.life <= 0)
        {
            return;
        }

        var tPlayer = e.Player.TPlayer;
        if (tPlayer == null)
        {
            return;
        }

        // Ichor 降 20 点防御；Critical 为 0 / 1，暴击翻倍
        var defense = npc.ichor ? Math.Max(0, npc.defense - 20) : npc.defense;
        var multiplier = e.Critical > 0 ? 2.0 : 1.0;
        var actual = (float) (multiplier * Main.CalculateDamageNPCsTake(e.Damage, defense));

        // 上界为攻前剩余血量——此时 npc.life 还没被本次攻击扣除
        if (actual > npc.life)
        {
            actual = npc.life;
        }
        if (actual <= 0)
        {
            return;
        }

        if (this.Strike.TryGetValue(npc, out var data) && data != null)
        {
            if (data.ContainsKey(tPlayer))
            {
                data[tPlayer] += actual;
            }
            else
            {
                data[tPlayer] = actual;
            }
        }
        else
        {
            this.Strike[npc] = new()
            {
                { tPlayer, actual }
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

        // 按 lifeMax 对累计伤害做比例校正：
        // NpcStrike hook 并不覆盖全部掉血路径——debuff DoT（着火、冰冻灼烧、中毒等）和
        // 环境伤害（岩浆、陷阱）都是直接修改 npc.life，不会触发 NpcStrike，所以 Strike
        // 累加器拿到的总伤害会小于 lifeMax。这里把每个玩家的累计伤害按比例放大到总和等于
        // lifeMax，让"按输出瓜分"的玩家在 solo 或多人瓜分时都能拿到完整的奖励池，而不会
        // 因为 DoT/环境伤害吃掉若干血量导致 rw < 1。
        var totalTracked = result.Values.Sum();
        if (totalTracked > 0)
        {
            var scale = args.npc.lifeMax / totalTracked;
            foreach (var kv in result.ToList())
            {
                result[kv.Key] = kv.Value * scale;
            }
        }

        foreach (var (player, damage) in result)
        {
            if (PlayerHandler.PlayerKillNpc(new PlayerKillNpcArgs(player, args.npc, damage)))
            {
                continue;
            }
            foreach (var currency in Setting.Instance.Currencies)
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
                CurrencyService.AddCurrency(player.name, currency.Name, num);
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