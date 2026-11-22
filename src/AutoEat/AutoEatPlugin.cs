using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoEat;

/// <summary>
/// AutoEat 自动进食 / 续 buff。
///
/// 核心规则（仅续「激活过、且刚过期」的）：
///   1. 玩家在线期间曾激活过的食物 buff / 增益，才进入跟踪；
///   2. 跟踪中的 buff 消失（刚过期）时才尝试续上；
///   3. 玩家从未获得过的 buff 一律不补。
///
/// 两条续期通道（都真实消耗物品，按物品原时长续，不用服务器指令改时长）：
///   · 续食物：从背包真实消耗一份食物，按食物品质给出对应 buff；
///   · 续buff：从背包真实喝下能给出该 buff 的药水，按药水原时长续上。
///   两者同时启用时优先续食物（吃背包里的），再由续buff喝药；都没有则不续。
///
/// 手动取消的 buff 不续（区分自然过期/手动取消：看 buff 消失瞬间的剩余时长，
/// 快耗尽=自然过期→续；还剩很多=手动取消→不再续）。同一合并窗口内的多次续期合并为一条消息。
/// </summary>
[ApiVersion(2, 1)]
public class AutoEatPlugin : TerrariaPlugin
{
    public override string Name => "AutoEat";
    public override string Author => "不是现在";
    public override string Description => "自动进食/续buff：仅续激活过且刚过期的食物或增益";
    public override Version Version => new(1, 0, 0, 0);

    /// <summary>主背包槽位范围（0~49）；50~53 钱币、54~58 弹药不参与。</summary>
    private const int MainInventorySlots = 50;

    /// <summary>续期成功后的短暂冷却（秒），避免服务端/客户端 buff 同步延迟导致的重复触发。</summary>
    private const int RenewCooldownSeconds = 3;

    private static readonly int[] FoodBuffIds = { 26, 206, 207 };

    /// <summary>每个玩家的跟踪状态（按账号/UUID 记忆，服务器重启后清空）。</summary>
    private sealed class PlayerState
    {
        /// <summary>在线期间曾激活过的 buff。</summary>
        public readonly HashSet<int> Activated = new();

        /// <summary>buff 消失（刚过期）的时刻，用于「刚过期」判定与窗口内重试。</summary>
        public readonly Dictionary<int, DateTime> PendingSince = new();

        /// <summary>上次续期时刻，用于冷却。</summary>
        public readonly Dictionary<int, DateTime> LastRenew = new();

        /// <summary>上一次扫描时 buff 是否激活，用于识别真正的「激活→过期」转变。</summary>
        public readonly Dictionary<int, bool> WasActive = new();

        /// <summary>
        /// 续期压制截止时刻：一次续期后，授予的 buff 需要这么久才会再次自然过期。
        /// 在此之前不再续期——即使服务端 buff 数组没反射出新 buff（SetBuff 只发包给客户端），
        /// 也不会因「看着仍像过期」而每秒连吃。这是修复连吃循环的关键。
        /// </summary>
        public readonly Dictionary<int, DateTime> RenewNext = new();

        /// <summary>
        /// 预期自然过期时刻。服务端 TPlayer.buffTime 读数恒为 60 tick 且不递减（真实倒计时只在客户端），
        /// 无法用「剩余时长」区分自然过期/手动取消。改为由插件记住授予的真实时长（item.buffTime）
        /// 推算到期时刻：消失时刻接近预期=自然过期；明显早于预期=手动取消。
        /// </summary>
        public readonly Dictionary<int, DateTime> ExpectedExpiry = new();

        /// <summary>手动取消的 buff：在重新获得前不再自动续。</summary>
        public readonly HashSet<int> Cancelled = new();

        /// <summary>待发送的续期提示（合并窗口内攒批，一次一条消息）。</summary>
        public readonly List<string> NoticeBuffer = new();

        /// <summary>待写日志的续期明细。</summary>
        public readonly List<string> LogBuffer = new();

        /// <summary>合并窗口起点（第一条进入缓冲的时刻）。</summary>
        public DateTime? NoticeFirstAt;
    }

    private readonly Dictionary<string, PlayerState> _states = new();

    /// <summary>/autoeat 个人开关覆盖（未覆盖时跟随「默认开启」）。</summary>
    private readonly Dictionary<string, bool> _toggle = new();

    private int _frames;

    /// <summary>运行期异常计数，异常过多时自动暂停，避免每秒刷日志。</summary>
    private int _errorCount;

    /// <summary>异常保护触发后暂停主循环（/reload 重置）。</summary>
    private bool _faulted;

    public AutoEatPlugin(Main game) : base(game)
    {
        Order = 900;
    }

    public override void Initialize()
    {
        AutoEatConfig.Load();
        ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        GeneralHooks.ReloadEvent += OnReload;
        Commands.ChatCommands.Add(new Command("autoeat.use", AutoEatCommand, "autoeat", "ae"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            GeneralHooks.ReloadEvent -= OnReload;
            var asm = Assembly.GetExecutingAssembly();
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate.Method?.DeclaringType?.Assembly == asm);
        }
        base.Dispose(disposing);
    }

    private void OnReload(ReloadEventArgs args)
    {
        AutoEatConfig.Load();
        _faulted = false;
        _errorCount = 0;
        args.Player.SendSuccessMessage("[AutoEat] 配置已重载");
    }

    // ==================== 主循环 ====================

    private void OnGameUpdate(EventArgs args)
    {
        var cfg = AutoEatConfig.Current;
        if (!cfg.GlobalEnabled || _faulted)
        {
            return;
        }

        if (++_frames < cfg.CheckIntervalSeconds * 60)
        {
            return;
        }
        _frames = 0;

        var now = DateTime.UtcNow;
        var tracked = cfg.GetTrackedBuffs();

        foreach (var plr in TShock.Players)
        {
            if (plr is not { Active: true, RealPlayer: true, IsLoggedIn: true })
            {
                continue;
            }
            if (plr.TPlayer.dead)
            {
                continue;
            }
            if (!IsEnabledFor(plr))
            {
                continue;
            }

            try
            {
            var state = GetState(plr);

            // 1) 扫描当前 buff：识别「激活→消失」转变；激活过的进入跟踪。
            //    区分「自然过期」与「手动取消」：看「消失时刻 vs 预期自然过期时刻」（服务端 buffTime 不可信，不用剩余时长）。
            foreach (var buff in tracked)
            {
                int remaining = GetBuffRemainingTicks(plr, buff);
                bool active = remaining > 0;
                bool wasActive = state.WasActive.TryGetValue(buff, out var wa) && wa;

                if (active)
                {
                    state.Activated.Add(buff);          // 曾激活：允许续期（只续激活过的）
                    state.WasActive[buff] = true;
                    if (!state.ExpectedExpiry.ContainsKey(buff))
                    {
                        // 首次外部获得（非本插件续期）：用背包中对应物品的原时长推算预期到期时刻。
                        // 本插件续期时已用 item.buffTime 精确写入 ExpectedExpiry，这里不覆盖。
                        state.ExpectedExpiry[buff] = now + TimeSpan.FromSeconds(EstimateDuration(plr, buff));
                    }
                    state.Cancelled.Remove(buff);        // 重新获得，解除「手动取消」标记
                    state.PendingSince.Remove(buff);     // 正在生效，不算过期
                }
                else
                {
                    state.WasActive[buff] = false;
                    if (state.Activated.Contains(buff))
                    {
                        if (wasActive)
                        {
                            // 观察到「激活→消失」：按「消失时刻 vs 预期自然过期时刻」判断。
                            // 服务端 buffTime 不可信（恒为 60 且不递减），故不看剩余时长；
                            // 接近预期到期=自然过期，明显早于预期=手动取消。
                            bool natural;
                            if (state.ExpectedExpiry.TryGetValue(buff, out var expect))
                            {
                                natural = now >= expect - TimeSpan.FromSeconds(cfg.ManualCancelThresholdSeconds);
                            }
                            else
                            {
                                natural = true; // 未知原时长：默认按自然过期续期（续期后即精确跟踪）
                            }
                            state.ExpectedExpiry.Remove(buff);

                            if (natural)
                            {
                                state.Cancelled.Remove(buff);
                                state.PendingSince[buff] = now;
                                TShock.Log.Info($"[AutoEat] {plr.Name} 的增益 #{buff} 自然过期，进入续期窗口");
                            }
                            else
                            {
                                state.Cancelled.Add(buff);
                                state.PendingSince.Remove(buff);
                                TShock.Log.Info($"[AutoEat] {plr.Name} 的增益 #{buff} 被手动取消（早于预期到期 {expect:HH:mm:ss}），不再自动续");
                            }
                        }
                        else if (!state.PendingSince.ContainsKey(buff)
                                 && !state.Cancelled.Contains(buff)
                                 && state.RenewNext.TryGetValue(buff, out var due)
                                 && now >= due)
                        {
                            // 上次续的 buff 理论上已到期（服务端 buff 数组未必反射 SetBuff 的新 buff，
                            // 观察不到 active→inactive 转变）：按到期时刻重新开窗
                            state.PendingSince[buff] = now;
                        }
                    }
                }
            }

            // 2) 对「激活过、且刚过期（非手动取消）」的 buff 尝试续期（每个过期窗口只续一次）
            foreach (var buff in tracked)
            {
                if (IsActive(plr, buff))
                {
                    continue;
                }
                if (!state.Activated.Contains(buff))
                {
                    continue; // 从未激活过：不续
                }
                if (state.Cancelled.Contains(buff))
                {
                    continue; // 手动取消过的：不续（重新获得后才会再跟踪）
                }
                if (!state.PendingSince.TryGetValue(buff, out var since))
                {
                    continue; // 没有打开的过期窗口：不续
                }
                if ((now - since).TotalSeconds > cfg.ExpireWindowSeconds)
                {
                    state.PendingSince.Remove(buff); // 超出「刚过期」窗口，放弃本次
                    continue;
                }
                if (state.RenewNext.TryGetValue(buff, out var suppressUntil)
                    && now < suppressUntil)
                {
                    continue; // 上次续期授予的时长还没到期，压制重复触发
                }
                if (state.LastRenew.TryGetValue(buff, out var last)
                    && (now - last).TotalSeconds < RenewCooldownSeconds)
                {
                    continue; // 续期冷却中
                }

                if (TryRenew(plr, buff, out var grantedSeconds, out var consumedName))
                {
                    state.LastRenew[buff] = now;
                    state.PendingSince.Remove(buff); // 一次窗口只续一次
                    state.RenewNext[buff] = now + TimeSpan.FromSeconds(Math.Max(grantedSeconds, 1));
                    // 记住本次授予的真实时长，用于下次区分自然过期 / 手动取消
                    state.ExpectedExpiry[buff] = now + TimeSpan.FromSeconds(Math.Max(grantedSeconds, 1));
                    state.NoticeBuffer.Add(consumedName);                          // 攒批，稍后合并成一条消息
                    state.LogBuffer.Add($"{consumedName}(#{buff}→{grantedSeconds:F0}s)");
                }
            }

            // 3) 合并提示：合并窗口内的多次续期一次性发出（多瓶药水只占一条消息）
            FlushNotices(plr, state, now);
            }
            catch (Exception ex)
            {
                _errorCount++;
                TShock.Log.Error($"[AutoEat] 处理玩家 {plr.Name} 时发生异常（第 {_errorCount} 次）: {ex}");
                if (_errorCount >= 5)
                {
                    _faulted = true;
                    TShock.Log.Error("[AutoEat] 异常次数过多，已自动暂停主循环；修复后请 /reload 重置");
                }
            }
        }
    }

    /// <summary>buff 是否处于激活状态（类型匹配且剩余时间大于 0）。</summary>
    private static bool IsActive(TSPlayer plr, int buff)
    {
        for (var i = 0; i < Player.maxBuffs; i++)
        {
            if (plr.TPlayer.buffType[i] == buff && plr.TPlayer.buffTime[i] > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>返回该 buff 当前剩余时长（tick）；未激活返回 0。多个同类型 buff 取最大剩余。</summary>
    private static int GetBuffRemainingTicks(TSPlayer plr, int buff)
    {
        var max = 0;
        for (var i = 0; i < Player.maxBuffs; i++)
        {
            if (plr.TPlayer.buffType[i] == buff && plr.TPlayer.buffTime[i] > max)
            {
                max = plr.TPlayer.buffTime[i];
            }
        }
        return max;
    }

    // ==================== 续期 ====================

    /// <summary>尝试续期一个刚过期的 buff：优先「续食物」，其次「续buff（真实喝药）」。返回是否成功，并给出授予时长（秒）与消耗/来源描述。</summary>
    private bool TryRenew(TSPlayer plr, int buff, out double grantedSeconds, out string consumedName)
    {
        var cfg = AutoEatConfig.Current;
        grantedSeconds = 0;
        consumedName = "";

        // 续食物：吃背包里的真实食物（按食物原时长）
        if (cfg.RenewFood.Enabled && AutoEatConfig.FoodTiers.ContainsKey(buff))
        {
            var found = FindFood(plr, buff);
            if (found.HasValue)
            {
                grantedSeconds = Eat(plr, found.Value.slot, found.Value.item, out consumedName);
                return true;
            }
        }

        // 续buff：真实喝药——消耗背包里能给出该 buff 的药水，按药水原时长续上
        if (cfg.RenewBuff.Enabled && cfg.RenewBuff.BuffList.Contains(buff))
        {
            var found = FindPotion(plr, buff);
            if (found.HasValue)
            {
                grantedSeconds = Eat(plr, found.Value.slot, found.Value.item, out consumedName);
                return true;
            }

            // 背包里没有对应药水：可选用服务器指令兜底（默认关闭，优先真实喝药）
            if (cfg.RenewBuff.FallbackSetBuff)
            {
                plr.SetBuff(buff, cfg.RenewBuff.DurationSeconds * 60, true);
                grantedSeconds = cfg.RenewBuff.DurationSeconds;
                consumedName = $"增益#{buff}(指令)";
                return true;
            }
        }

        return false;
    }

    /// <summary>在主背包中为「过期档位」挑选食物：同档 &gt; 高档 &gt; 低档（可配置关闭低档替补）。</summary>
    private static (int slot, Item item)? FindFood(TSPlayer plr, int expiredTier)
    {
        var cfg = AutoEatConfig.Current.RenewFood;
        Item? best = null;
        var bestSlot = -1;
        var bestRank = int.MaxValue;

        for (var i = 0; i < MainInventorySlots; i++)
        {
            var item = plr.TPlayer.inventory[i];
            if (item == null || item.type <= 0 || item.stack <= 0)
            {
                continue;
            }
            if (!AutoEatConfig.FoodTiers.TryGetValue(item.buffType, out var tier))
            {
                continue;
            }
            if (cfg.IgnoredItemIds.Contains(item.type))
            {
                continue;
            }
            if (cfg.WhitelistItemIds.Count > 0 && !cfg.WhitelistItemIds.Contains(item.type))
            {
                continue;
            }

            int rank;
            if (tier == expiredTier)
            {
                rank = 0; // 同档食物
            }
            else if (tier > expiredTier)
            {
                rank = 1; // 高档食物（升级替补）
            }
            else if (cfg.AllowLowerTier)
            {
                rank = 2; // 低档食物（降级保底）
            }
            else
            {
                continue;
            }

            if (rank < bestRank)
            {
                best = item;
                bestSlot = i;
                bestRank = rank;
            }
        }

        return best == null ? null : (bestSlot, best);
    }

    /// <summary>在主背包中找到能给出指定 buff 的药水/食物（buffType 精确匹配、且可消耗）。</summary>
    private static (int slot, Item item)? FindPotion(TSPlayer plr, int buff)
    {
        for (var i = 0; i < MainInventorySlots; i++)
        {
            var item = plr.TPlayer.inventory[i];
            if (item == null || item.type <= 0 || item.stack <= 0)
            {
                continue;
            }
            if (item.buffType != buff)
            {
                continue;
            }
            if (!item.consumable)
            {
                continue; // 只喝/吃可消耗物；非消耗品（饰品等）不碰
            }
            return (i, item);
        }
        return null;
    }

    /// <summary>
    /// 估算某 buff 的自然时长（秒）：取背包中能给出该 buff 的可消耗物的最大 buffTime。
    /// 用于「首次外部获得」时推算预期到期时刻；背包无对应物时返回 0（调用方据此默认按自然过期处理）。
    /// </summary>
    private static double EstimateDuration(TSPlayer plr, int buff)
    {
        double best = 0;
        for (var i = 0; i < MainInventorySlots; i++)
        {
            var item = plr.TPlayer.inventory[i];
            if (item == null || item.type <= 0 || item.stack <= 0)
            {
                continue;
            }
            if (item.buffType == buff && item.buffTime > 0)
            {
                best = Math.Max(best, item.buffTime / 60.0);
            }
        }
        return best;
    }

    /// <summary>真实进食/喝药：扣减堆叠 + 同步背包槽位 + 给予 buff（按物品原时长）+ SSC 存档。返回授予的增益时长（秒）与物品名。</summary>
    private double Eat(TSPlayer plr, int slot, Item item, out string consumedName)
    {
        // 先取值：清空槽位后 buffType/buffTime/Name 都会丢失
        var buffType = item.buffType;
        var buffTime = item.buffTime;
        var itemName = item.Name;

        // buffTime 以 tick 计（60 tick = 1 秒）；为 0 时回退到续 buff 配置时长
        var grantedSeconds = buffTime > 0 ? buffTime / 60.0 : AutoEatConfig.Current.RenewBuff.DurationSeconds;

        item.stack--;
        if (item.stack <= 0)
        {
            // 手动清空槽位。注意：不要调用 Item.TurnToAir()——
            // 不同 OTAPI 版本的 TurnToAir 签名不同（无参 / bool 参数），
            // 跨版本调用会抛 MissingMethodException
            item.stack = 0;
            item.type = 0;
            item.prefix = 0;
        }
        // 5 号包 PlayerSlot：同步该槽位（物品数据从服务端背包序列化）
        plr.SendData(PacketTypes.PlayerSlot, "", plr.Index, slot);

        plr.SetBuff(buffType, buffTime > 0 ? buffTime : AutoEatConfig.Current.RenewBuff.DurationSeconds * 60, true);

        if (TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            plr.SaveServerCharacter();
        }

        // 提示与日志由调用方攒批、合并成一条消息（见 FlushNotices）
        consumedName = itemName;
        return grantedSeconds;
    }

    /// <summary>把合并窗口内攒下的续期一次性发出（多瓶药水只占一条消息）。</summary>
    private void FlushNotices(TSPlayer plr, PlayerState state, DateTime now)
    {
        if (state.NoticeBuffer.Count == 0)
        {
            return;
        }
        state.NoticeFirstAt ??= now;
        if ((now - state.NoticeFirstAt.Value).TotalSeconds < AutoEatConfig.Current.NoticeBatchSeconds)
        {
            return; // 合并窗口未到，继续攒批
        }

        if (AutoEatConfig.Current.Notice)
        {
            plr.SendInfoMessage($"[AutoEat] 自动续上：{string.Join("、", state.NoticeBuffer)}");
        }
        TShock.Log.Info($"[AutoEat] {plr.Name} 自动续上 {string.Join("、", state.LogBuffer)}");

        state.NoticeBuffer.Clear();
        state.LogBuffer.Clear();
        state.NoticeFirstAt = null;
    }

    // ==================== 状态与开关 ====================

    private static string StateKey(TSPlayer plr) =>
        plr.Account != null ? $"acc:{plr.Account.Name}" : $"uuid:{plr.UUID}";

    private PlayerState GetState(TSPlayer plr)
    {
        var key = StateKey(plr);
        if (!_states.TryGetValue(key, out var state))
        {
            state = new PlayerState();
            _states[key] = state;
        }
        return state;
    }

    private bool IsEnabledFor(TSPlayer plr) =>
        _toggle.TryGetValue(StateKey(plr), out var v) ? v : AutoEatConfig.Current.DefaultEnabled;

    // ==================== 指令 ====================

    private void AutoEatCommand(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            var key = StateKey(args.Player);
            var next = !IsEnabledFor(args.Player);
            _toggle[key] = next;
            args.Player.SendSuccessMessage($"[AutoEat] 自动进食已{(next ? "开启" : "关闭")}");
            return;
        }

        switch (args.Parameters[0].ToLowerInvariant())
        {
            case "status":
            case "状态":
            {
                var cfg = AutoEatConfig.Current;
                var state = GetState(args.Player);
                args.Player.SendInfoMessage($"[AutoEat] 个人开关: {(IsEnabledFor(args.Player) ? "开" : "关")}（全局: {(cfg.GlobalEnabled ? "开" : "关")}）");
                args.Player.SendInfoMessage($"[AutoEat] 续食物: {(cfg.RenewFood.Enabled ? "开" : "关")}  续buff: {(cfg.RenewBuff.Enabled ? "开" : "关")}（列表: {string.Join(",", cfg.RenewBuff.BuffList)}）");
                args.Player.SendInfoMessage($"[AutoEat] 已激活过的跟踪buff: {(state.Activated.Count > 0 ? string.Join(",", state.Activated) : "无")}  等待续期: {(state.PendingSince.Count > 0 ? string.Join(",", state.PendingSince.Keys) : "无")}");
                break;
            }
            case "reload":
            case "重载":
                AutoEatConfig.Load();
                _faulted = false;
                _errorCount = 0;
                args.Player.SendSuccessMessage("[AutoEat] 配置已重载");
                break;
            default:
                args.Player.SendInfoMessage("/autoeat —— 开关个人自动进食");
                args.Player.SendInfoMessage("/autoeat status —— 查看当前状态");
                args.Player.SendInfoMessage("/autoeat reload —— 重载配置文件");
                break;
        }
    }
}
