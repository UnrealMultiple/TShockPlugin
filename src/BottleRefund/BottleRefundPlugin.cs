using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace BottleRefund
{
    /// <summary>
    /// BottleRefund —— 喝药自动返还玻璃瓶。
    ///
    /// 背景与原理（Terraria 1.4.5.8 / TShock 6.2.1）：
    /// - 泰拉瑞亚多人是"客户端权威"：喝药发生在客户端本地，服务器看不到"喝药"动作本身，
    ///   只能看到客户端随后发来的背包槽同步包（PlayerSlot，槽位堆叠绝对值）。
    /// - 但堆叠减少 ≠ 喝药：合并堆叠、快速堆放入箱、移入猪猪银行、整组丢弃、扔垃圾桶、
    ///   酿造消耗原料等都会让槽位堆叠下降。因此本插件不做"逐包即时判断"，而是：
    ///   把一段时间窗内的所有相关事件攒成流水账，窗口后做一次全面结算，再统一发放。
    ///
    /// 流水来源（三个事件，只记录"药水类物品"）：
    /// 1. PlayerSlot   —— 主背包槽（网络槽位 &lt; TrashItem，不含垃圾桶 58 与猪猪银行等 Bank 槽）。
    ///                    服务器侧维护槽位镜像，得到每次增减 Δ。
    /// 2. ItemDrop     —— 玩家把物品丢在地上（独立丢弃包，用于抵扣对应的槽位减少）。
    /// 3. ChestItemChange —— 箱子槽位变化（服务器侧维护箱子镜像，正 Δ = 存入，用于抵扣）。
    ///
    /// 全面结算（每个玩家独立、主线程泵驱动）：
    /// 条件：流水最老一笔 ≥ 结算延迟秒 且 最后事件静默 ≥ 1 秒。
    /// 按药水类型逐一配对：
    ///   a) 槽位减少 与 等量、时间靠后的槽位增加配对 → 移动/合并，不返还；
    ///   b) 与箱子存入（正 Δ）配对 → 入箱，不返还；
    ///   c) 与地面丢弃配对 → 丢弃，不返还；
    ///   d) 剩余的减少中，每笔恰好 1 的判定为"喝了一瓶"；>1 的未配对批量残留不返还。
    /// 每判定一瓶喝药，按配置概率 × 数量发放返还物品（默认 1 个玻璃瓶）。
    ///
    /// 药水集合：启动时全物品扫描 —— potion==true（治疗系）∪ buffType&gt;0 && consumable（增益系），
    /// 排除食物（IsFood）。瓶装水、原料、投掷类不在此集合，酿造消耗与吃饭不会误判。
    /// </summary>
    [ApiVersion(2, 1)]
    public class BottleRefundPlugin : TerrariaPlugin
    {
        public override string Name => "BottleRefund";
        public override string Author => "不是现在";
        public override string Description => "喝药自动返还玻璃瓶：延迟数秒全面结算背包流水后发放，批量堆叠/入箱/丢弃不误发。";
        public override Version Version => new Version(1, 0, 0);

        private static string ConfigPath => Path.Combine(TShock.SavePath, "BottleRefund.json");

        // 网络槽位：0..TrashItem-1 为主背包/钱币/弹药区；TrashItem 为垃圾桶；≥Bank1_0 为各类银行。
        // 只跟踪主背包区，垃圾桶与银行整体排除。
        private static readonly int SlotRange = PlayerItemSlotID.TrashItem;

        private volatile BottleRefundConfig? _config;
        private readonly object _lock = new();
        private readonly Dictionary<int, PlayerLedger> _ledgers = new();
        private readonly Dictionary<int, SlotState> _mirrors = new();
        private readonly Dictionary<long, (short Type, short Stack)> _chestMirror = new();
        private readonly HashSet<int> _potionSet = new();
        private readonly Stopwatch _clock = Stopwatch.StartNew();

        private double Now => _clock.Elapsed.TotalSeconds;

        private sealed class SlotState
        {
            public readonly short[] Type = new short[SlotRange];
            public readonly short[] Stack = new short[SlotRange];
        }

        private struct Entry
        {
            public double Time;
            public int Type;
            public int Amount;
            public bool Used;
        }

        private sealed class PlayerLedger
        {
            public readonly List<Entry> Decreases = new List<Entry>();      // 主背包药水减少
            public readonly List<Entry> Increases = new List<Entry>();      // 主背包药水增加
            public readonly List<Entry> ChestIncreases = new List<Entry>(); // 箱子存入（正 Δ）
            public readonly List<Entry> DropIncreases = new List<Entry>();  // 丢在地上
            public double FirstTime = double.MaxValue;
            public double LastTime;
            public int Total;
        }

        public BottleRefundPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            _config = BottleRefundConfig.Load(ConfigPath);
            BuildPotionSet(_config);

            GetDataHandlers.PlayerSlot.Register(OnPlayerSlot);
            GetDataHandlers.ItemDrop.Register(OnItemDrop);
            GetDataHandlers.ChestItemChange.Register(OnChestItemChange);
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);

            // 主线程泵。GameUpdate 在部分 TSAPI/OTAPI 组合下不触发，
            // 同时挂原版专用服务器主循环的 OnTickForThirdPartySoftwareOnly，两者共用一个泵。
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            Main.OnTickForThirdPartySoftwareOnly += OnServerTick;
            // ContentSamples 在 Main.Initialize 中段才填充，晚于插件初始化；
            // 用 GamePostInitialize 重建一次，主循环里再兜底重试直到就绪。
            ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);

            var c = _config;
            TShock.Log.ConsoleInfo(
                "[BottleRefund] v{0} 已加载：喝药自动返还，延迟 {1}s 全面结算后发放；返还 {2:P0} × {3} 个（物品 {4}）；药水 {5} 种（黑名单 {6} 项）；配置 {7}",
                Version,
                c.SettlementDelaySeconds,
                c.RefundChance,
                c.RefundCountPerDrink,
                c.RefundItemId,
                _potionSet.Count,
                c.Blacklist.Count,
                ConfigPath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GetDataHandlers.PlayerSlot.UnRegister(OnPlayerSlot);
                GetDataHandlers.ItemDrop.UnRegister(OnItemDrop);
                GetDataHandlers.ChestItemChange.UnRegister(OnChestItemChange);
                ServerApi.Hooks.ServerLeave.Deregister(this, OnServerLeave);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                Main.OnTickForThirdPartySoftwareOnly -= OnServerTick;
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
            }
            base.Dispose(disposing);
        }

        // ------------------------------------------------------------------
        // 药水集合
        // ------------------------------------------------------------------

        private void BuildPotionSet(BottleRefundConfig cfg)
        {
            _potionSet.Clear();
            foreach (var kv in ContentSamples.ItemsByType)
            {
                int id = kv.Key;
                var item = kv.Value;
                if (id <= 0 || item == null || item.stack <= 0)
                    continue;
                if (item.potion || (item.buffType > 0 && item.consumable && id < ItemID.Sets.IsFood.Length && !ItemID.Sets.IsFood[id]))
                    _potionSet.Add(id);
            }
            foreach (int id in cfg.Blacklist)
                _potionSet.Remove(id);
        }

        // ------------------------------------------------------------------
        // 流水收集
        // ------------------------------------------------------------------

        private void OnPlayerSlot(object? sender, GetDataHandlers.PlayerSlotEventArgs e)
        {
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;
            var plr = e.Player;
            if (plr == null || e.PlayerId != plr.Index || plr.IgnoreSSCPackets)
                return;
            int slot = e.Slot;
            if (slot < 0 || slot >= SlotRange)
                return;

            lock (_lock)
            {
                var st = GetMirror(plr.Index);
                int oldType = st.Type[slot];
                int oldStack = st.Stack[slot];
                st.Type[slot] = (short)e.Type;
                st.Stack[slot] = (short)e.Stack;
                double t = Now;
                var ledger = GetLedger(plr.Index, t);

                if (oldType == e.Type)
                {
                    int delta = e.Stack - oldStack;
                    if (delta == 0)
                        return;
                    if (_potionSet.Contains(e.Type))
                        AddEntry(ledger, delta < 0 ? ledger.Decreases : ledger.Increases, t, e.Type, Math.Abs(delta));
                }
                else
                {
                    // 槽位换物品：旧物品整体移出、新物品整体进入
                    if (oldType > 0 && oldStack > 0 && _potionSet.Contains(oldType))
                        AddEntry(ledger, ledger.Decreases, t, oldType, oldStack);
                    if (e.Type > 0 && e.Stack > 0 && _potionSet.Contains(e.Type))
                        AddEntry(ledger, ledger.Increases, t, e.Type, e.Stack);
                }
            }
        }

        private void OnItemDrop(object? sender, GetDataHandlers.ItemDropEventArgs e)
        {
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;
            var plr = e.Player;
            if (plr == null || e.Type <= 0 || e.Stacks <= 0 || !_potionSet.Contains(e.Type))
                return;
            lock (_lock)
                AddEntry(GetLedger(plr.Index, Now), GetLedger(plr.Index, Now).DropIncreases, Now, e.Type, e.Stacks);
        }

        private void OnChestItemChange(object? sender, GetDataHandlers.ChestItemEventArgs e)
        {
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;
            var plr = e.Player;
            if (plr == null)
                return;
            long key = ((long)e.ID << 8) | (byte)e.Slot;
            lock (_lock)
            {
                _chestMirror.TryGetValue(key, out var old);
                _chestMirror[key] = (e.Type, e.Stacks);
                var ledger = GetLedger(plr.Index, Now);
                if (old.Type == e.Type)
                {
                    int delta = e.Stacks - old.Stack;
                    if (delta > 0 && _potionSet.Contains(e.Type))
                        AddEntry(ledger, ledger.ChestIncreases, Now, e.Type, delta);
                }
                else if (e.Type > 0 && e.Stacks > 0 && _potionSet.Contains(e.Type))
                {
                    AddEntry(ledger, ledger.ChestIncreases, Now, e.Type, e.Stacks);
                }
            }
        }

        private void OnServerLeave(LeaveEventArgs e)
        {
            if (e == null || e.Who < 0)
                return;
            lock (_lock)
            {
                _ledgers.Remove(e.Who);
                _mirrors.Remove(e.Who);
            }
        }

        private SlotState GetMirror(int playerIndex)
        {
            if (!_mirrors.TryGetValue(playerIndex, out var st) || st == null)
            {
                st = new SlotState();
                _mirrors[playerIndex] = st;
            }
            return st;
        }

        private PlayerLedger GetLedger(int playerIndex, double now)
        {
            if (!_ledgers.TryGetValue(playerIndex, out var L) || L == null)
            {
                L = new PlayerLedger();
                _ledgers[playerIndex] = L;
            }
            return L;
        }

        private static void AddEntry(PlayerLedger ledger, List<Entry> list, double time, int type, int amount)
        {
            list.Add(new Entry { Time = time, Type = type, Amount = amount });
            if (time < ledger.FirstTime)
                ledger.FirstTime = time;
            ledger.LastTime = time;
            ledger.Total++;
        }

        // ------------------------------------------------------------------
        // 主线程泵：结算窗口到期后统一发放
        // ------------------------------------------------------------------

        private void OnGamePostInitialize(EventArgs args)
        {
            var cfg = _config;
            if (cfg == null)
                return;
            lock (_lock)
            {
                if (_potionSet.Count == 0)
                    BuildPotionSet(cfg);
            }
            if (_potionSet.Count > 0)
                TShock.Log.ConsoleInfo("[BottleRefund] 药水集合就绪：{0} 种", _potionSet.Count);
        }

        private void OnGameUpdate(EventArgs args)
        {
            Pump();
        }

        private void OnServerTick()
        {
            Pump();
        }

        private void Pump()
        {
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;

            if (_potionSet.Count == 0)
            {
                // ContentSamples 未就绪时的兜底重试；就绪瞬间只重建一次
                lock (_lock)
                {
                    if (_potionSet.Count == 0)
                        BuildPotionSet(cfg);
                }
                if (_potionSet.Count > 0)
                    TShock.Log.ConsoleInfo("[BottleRefund] 药水集合就绪（主循环兜底）：{0} 种", _potionSet.Count);
            }

            if (_ledgers.Count == 0)
                return;

            double now = Now;
            List<int>? ready = null;
            List<int>? dropped = null;

            lock (_lock)
            {
                foreach (var kv in _ledgers)
                {
                    var L = kv.Value;
                    if (L == null || L.Total == 0)
                        continue;
                    if (L.Total > cfg.LedgerLimit)
                    {
                        (dropped ??= new List<int>()).Add(kv.Key);
                        continue;
                    }
                    if (now - L.FirstTime >= cfg.SettlementDelaySeconds && now - L.LastTime >= 1.0)
                        (ready ??= new List<int>()).Add(kv.Key);
                }
                if (dropped != null)
                    foreach (int idx in dropped)
                        _ledgers.Remove(idx);
            }

            if (dropped != null && cfg.VerboseLog)
                TShock.Log.ConsoleInfo("[BottleRefund] {0} 名玩家流水超限，整批放弃（不返还）", dropped.Count);

            if (ready == null)
                return;
            foreach (int idx in ready)
            {
                PlayerLedger? L;
                lock (_lock)
                {
                    if (!_ledgers.TryGetValue(idx, out L) || L == null)
                        continue;
                    _ledgers.Remove(idx);
                }
                Settle(idx, L, cfg);
            }
        }

        // ------------------------------------------------------------------
        // 全面结算：配对 → 判定喝药 → 发放
        // ------------------------------------------------------------------

        private void Settle(int playerIndex, PlayerLedger L, BottleRefundConfig cfg)
        {
            var decByType = GroupByType(L.Decreases);
            if (decByType.Count == 0)
                return;

            var incByType = GroupByType(L.Increases);
            var chestByType = GroupByType(L.ChestIncreases);
            var dropByType = GroupByType(L.DropIncreases);

            int totalDrinks = 0;
            int toGive = 0;
            string detail = "";

            foreach (var kv in decByType)
            {
                int type = kv.Key;
                var incs = incByType.TryGetValue(type, out var inc) ? inc : null;
                var chest = chestByType.TryGetValue(type, out var ch) ? ch : null;
                var drops = dropByType.TryGetValue(type, out var dr) ? dr : null;

                int drinks = 0, moved = 0, chested = 0, dropped = 0, batch = 0;
                foreach (var dec in kv.Value)
                {
                    if (incs != null && TryMatchExact(incs, dec))
                    {
                        moved += dec.Amount;
                        continue;
                    }
                    if (chest != null && TryConsumeFromPool(chest, dec))
                    {
                        chested += dec.Amount;
                        continue;
                    }
                    if (drops != null && TryConsumeFromPool(drops, dec))
                    {
                        dropped += dec.Amount;
                        continue;
                    }
                    if (dec.Amount == 1)
                        drinks++;
                    else
                        batch += dec.Amount;
                }

                if (drinks > 0)
                {
                    totalDrinks += drinks;
                    for (int i = 0; i < drinks; i++)
                        if (Main.rand != null && Main.rand.NextDouble() < cfg.RefundChance)
                            toGive += Math.Max(0, cfg.RefundCountPerDrink);
                }

                if (drinks > 0 || batch > 0 || cfg.VerboseLog)
                    detail += string.Format(
                        "\n  [物品{0}] 喝药x{1} 移动x{2} 入箱x{3} 丢弃x{4} 批量忽略x{5}",
                        type, drinks, moved, chested, dropped, batch);
            }

            string playerName = "?";
            if (toGive > 0)
            {
                var tsPlayer = playerIndex >= 0 && playerIndex < TShock.Players.Length ? TShock.Players[playerIndex] : null;
                if (tsPlayer != null && tsPlayer.Active)
                {
                    playerName = tsPlayer.Name;
                    tsPlayer.GiveItem(cfg.RefundItemId, toGive);
                }
                else
                {
                    toGive = 0; // 玩家已离线，放弃发放
                }
            }

            if (cfg.VerboseLog && (totalDrinks > 0 || detail.Length > 0))
                TShock.Log.ConsoleInfo("[BottleRefund] 结算 {0}：喝药x{1}，发放x{2}{3}", playerName, totalDrinks, toGive, detail);
        }

        private static Dictionary<int, List<Entry>> GroupByType(List<Entry> list)
        {
            var map = new Dictionary<int, List<Entry>>();
            foreach (var e in list)
            {
                if (!map.TryGetValue(e.Type, out var lst) || lst == null)
                {
                    lst = new List<Entry>();
                    map[e.Type] = lst;
                }
                lst.Add(e);
            }
            // 按时间升序，保证配对从最早的一笔开始
            foreach (var lst in map.Values)
                lst.Sort((a, b) => a.Time.CompareTo(b.Time));
            return map;
        }

        /// <summary>槽位增加与槽位减少做"等量配对"（合并/移动）。</summary>
        private static bool TryMatchExact(List<Entry> incs, Entry dec)
        {
            for (int i = 0; i < incs.Count; i++)
            {
                var e = incs[i];
                if (!e.Used && e.Amount == dec.Amount && e.Time >= dec.Time - 0.5)
                {
                    e.Used = true;
                    incs[i] = e;
                    return true;
                }
            }
            return false;
        }

        /// <summary>从箱子/掉落池中消耗一笔减少量（支持部分消耗）。</summary>
        private static bool TryConsumeFromPool(List<Entry> pool, Entry dec)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                var e = pool[i];
                if (e.Used || e.Time < dec.Time - 0.5 || e.Amount < dec.Amount)
                    continue;
                e.Amount -= dec.Amount;
                e.Used = e.Amount == 0;
                pool[i] = e;
                return true;
            }
            return false;
        }
    }
}
