using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace HerbBloom
{
    /// <summary>
    /// HerbBloom —— 全草药常开（无视条件开花，开花后不回退），生长速度可调。
    ///
    /// 原理（Terraria 1.4.5.8 服务端，依据反编译 WorldGen/Main）：
    /// - 草药统一使用三个 tile：82 幼年(ImmatureHerbs)、83 成熟(MatureHerbs)、84 开花(BloomingHerbs)。
    ///   品种(style) = frameX / 18：0太阳花 1月光草 2闪光根 3死亡草 4水叶草 5火焰花 6霜花(Shiverthorn, frameX=108)。
    ///   三种状态的帧编码完全相同（frameX = 18*style, frameY = 0），状态切换只改 type。
    /// - 原版服务端 GrowAlch：82→83 随机成熟；闪光根(style 2) 83↔84 随机开/落（1/5 概率回退）；
    ///   霜花(style 6) 83→84（1/30 概率，永不回退）。其余品种（太阳花/月光草/死亡草/水叶草/火焰花）
    ///   的"开花"只是客户端按世界条件（昼夜/月相/下雨/时段）把 83 渲染成开花，条件结束即"收拢"，
    ///   且采集时只有窗口内才算"开花掉种子"（IsHarvestableHerbWithSeed）。
    /// - 因此插件的做法是"参考霜花"：把成熟草药(83)直接改成开花 tile(84)——
    ///   84 在客户端永远渲染为开花、采集永远掉种子、且没有条件判断；
    ///   再用周期巡检把任何被原版随机回退（闪光根 1/5、熔岩火焰花低水量）或客户端同步导致的
    ///   84→83 改回 84，实现"开花后不再取消开花"。
    /// - 生长速度：幼年草(82)在玩家活动区内由插件按"基础概率 × 倍率"催熟（倍率 0 = 纯原版速度）。
    /// </summary>
    [ApiVersion(2, 1)]
    public class HerbBloomPlugin : TerrariaPlugin
    {
        public override string Name => "HerbBloom";
        public override string Author => "不是现在";
        public override string Description => "全草药无视条件常开（参考霜花逻辑），开花后不回退，生长速度可配置。";
        public override Version Version => new Version(1, 0, 0);

        // 1.4.5 草药 tile 类型与品种上限（style = frameX/18，0..6）。
        private const ushort TileImmature = 82;
        private const ushort TileMature = 83;
        private const ushort TileBlooming = 84;
        private const int MaxHerbStyle = 6;

        private static string ConfigPath => Path.Combine(TShock.SavePath, "HerbBloom.json");

        private volatile HerbBloomConfig? _config;
        private Thread? _scanThread;
        private volatile bool _stop;

        // 全世界推进扫描的光标（列号）。
        private int _sweepX = 10;

        // 扫描线程只收集坐标，主线程 GameUpdate 校验后改 tile 并广播。
        private readonly ConcurrentQueue<PendingChange> _pending = new ConcurrentQueue<PendingChange>();

        private struct PendingChange
        {
            public short X;
            public short Y;
            public ushort ExpectedType;
        }

        // 统计（供 /herbbloom status 展示）。
        private long _statConverted;
        private long _statSkippedStale;
        private long _statBadFrame;
        private long _applierCalls;

        // 记录世界尺寸，检测世界重载/切换（此时丢弃旧队列坐标）。
        private int _lastMaxTilesX = -1;
        private int _lastMaxTilesY = -1;

        public HerbBloomPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            _config = HerbBloomConfig.Load(ConfigPath);

            // 主线程每刻驱动。GameUpdate 在部分 TSAPI/OTAPI 组合下不触发，
            // 因此同时挂原版专用服务器主循环里的 OnTickForThirdPartySoftwareOnly
            // （两者共用一个泵，ConcurrentQueue 原子出队，重复驱动无害）。
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            Main.OnTickForThirdPartySoftwareOnly += OnServerTick;

            _stop = false;
            _sweepX = 10;
            _scanThread = new Thread(ScannerLoop)
            {
                IsBackground = true,
                Name = "HerbBloom-Scanner",
            };
            _scanThread.Start();

            var c = _config;
            TShock.Log.ConsoleInfo(
                "[HerbBloom] v{0} 已加载：无视条件开花={1}，开花不回退={2}，幼草直接开花={3}；生长倍率={4}（0=原版速度，当前每轮成熟率 {5:P0}）；扫描间隔={6}s；配置 {7}",
                Version,
                c.Enabled && c.BloomIgnoreConditions ? "开" : "关",
                c.Enabled && c.PreventUnbloom ? "开" : "关",
                c.BloomImmatureHerbs ? "开" : "关",
                c.GrowthSpeedMultiplier,
                c.EffectiveGrowthChancePerScan(),
                c.ScanIntervalSeconds,
                ConfigPath);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stop = true;
                try
                {
                    _scanThread?.Join(2500);
                }
                catch
                {
                }
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                Main.OnTickForThirdPartySoftwareOnly -= OnServerTick;
            }
            base.Dispose(disposing);
        }

        // ------------------------------------------------------------------
        // 扫描线程：只读 tile，收集待处理坐标。
        // ------------------------------------------------------------------

        private void ScannerLoop()
        {
            while (!_stop)
            {
                double remaining = Math.Max(0.5, _config?.ScanIntervalSeconds ?? 10.0);
                while (remaining > 0 && !_stop)
                {
                    Thread.Sleep(100);
                    remaining -= 0.1;
                }
                if (_stop)
                    break;
                try
                {
                    ScanCycle();
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError("[HerbBloom] 扫描出错：{0}", ex.Message);
                }
            }
        }

        private void ScanCycle()
        {
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;
            if (Main.tile == null || Main.maxTilesX <= 20 || Main.maxTilesY <= 20)
                return;

            long found = 0;

            // 1) 玩家活动区：幼草催熟 + 成熟草强制开花。
            int radius = Math.Max(16, Math.Min(1024, cfg.PlayerScanRadiusTiles));
            for (int i = 0; i < TShock.Players.Length; i++)
            {
                var ts = TShock.Players[i];
                if (ts == null || !ts.Active)
                    continue;
                var plr = Main.player[i];
                if (plr == null || !plr.active)
                    continue;
                int cx = ts.TileX;
                int cy = ts.TileY;
                found += ScanRect(cx - radius, cy - radius, cx + radius, cy + radius, scanImmature: true);
            }

            // 2) 全世界推进扫描：只做开花强制（成熟→开花），不催熟幼草。
            int cols = Math.Max(10, Math.Min(100000, cfg.WorldSweepColumnsPerCycle));
            int x0 = _sweepX;
            int x1 = Math.Min(Main.maxTilesX - 10, x0 + cols);
            if (x1 > x0)
                found += ScanRect(x0, 10, x1, Main.maxTilesY - 10, scanImmature: false);
            _sweepX = x1 >= Main.maxTilesX - 10 ? 10 : x1;

            if (cfg.VerboseLog)
                TShock.Log.ConsoleInfo(
                    "[HerbBloom] 本轮入队 {0} 株，队列 {1}，累计转换 {2}，跳过(过期) {3}，异常帧 {4}",
                    found, _pending.Count,
                    Interlocked.Read(ref _statConverted),
                    Interlocked.Read(ref _statSkippedStale),
                    Interlocked.Read(ref _statBadFrame));
        }

        /// <summary>扫描矩形区域，把需要处理的草药坐标入队（不动 tile，只读）。</summary>
        private long ScanRect(int x0, int y0, int x1, int y1, bool scanImmature)
        {
            long found = 0;
            int maxX = Main.maxTilesX - 10;
            int maxY = Main.maxTilesY - 10;
            x0 = Math.Max(10, x0);
            y0 = Math.Max(10, y0);
            x1 = Math.Min(maxX, x1);
            y1 = Math.Min(maxY, y1);

            for (int x = x0; x < x1; x++)
            {
                for (int y = y0; y < y1; y++)
                {
                    var tile = Main.tile[x, y];
                    if (tile == null || !tile.active())
                        continue;
                    ushort type = tile.type;
                    if (type == TileMature || (scanImmature && type == TileImmature))
                    {
                        _pending.Enqueue(new PendingChange { X = (short)x, Y = (short)y, ExpectedType = type });
                        found++;
                    }
                    // type == 84（已开花）：无需处理；若被原版随机回退成 83，
                    // 下轮扫描会以 83 入队再改回 84，等效"开花后不回退"。
                }
            }
            return found;
        }

        // ------------------------------------------------------------------
        // 主线程：每刻把队列里的改动落到 tile 并按原版方式广播。
        // ------------------------------------------------------------------

        private void OnGameUpdate(EventArgs args)
        {
            Pump();
        }

        private void OnServerTick()
        {
            Pump();
        }

        /// <summary>主线程泵：把扫描线程收集的改动落到 tile 并按原版方式广播。</summary>
        private void Pump()
        {
            long calls = Interlocked.Increment(ref _applierCalls);
            var cfg = _config;
            if (cfg == null || !cfg.Enabled)
                return;
            if (cfg.VerboseLog && calls % 600 == 0)
                TShock.Log.ConsoleInfo("[HerbBloom] 主线程泵活跃：calls={0} queue={1}", calls, _pending.Count);
            if (_pending.IsEmpty)
                return;

            // 世界重载/切换：丢弃旧世界的坐标，避免误改新世界。
            if (Main.maxTilesX != _lastMaxTilesX || Main.maxTilesY != _lastMaxTilesY)
            {
                if (_lastMaxTilesX != -1)
                {
                    while (_pending.TryDequeue(out _))
                    {
                    }
                }
                _lastMaxTilesX = Main.maxTilesX;
                _lastMaxTilesY = Main.maxTilesY;
            }
            if (Main.tile == null || Main.maxTilesX <= 20)
                return;

            int budget = Math.Max(1, cfg.MaxTileSquaresPerTick);
            long converted = 0;

            while (budget-- > 0 && _pending.TryDequeue(out var ch))
            {
                if (ch.X < 0 || ch.Y < 0 || ch.X >= Main.maxTilesX || ch.Y >= Main.maxTilesY)
                    continue;

                var tile = Main.tile[ch.X, ch.Y];
                if (tile == null || !tile.active())
                    continue;

                ushort type = tile.type;
                if (type != ch.ExpectedType)
                {
                    Interlocked.Increment(ref _statSkippedStale);
                    continue;
                }
                if (type != TileImmature && type != TileMature)
                    continue;
                if (Main.tileAlch == null || type >= Main.tileAlch.Length || !Main.tileAlch[type])
                    continue;

                int style = tile.frameX / 18;
                if (style < 0 || style > MaxHerbStyle || tile.frameX % 18 != 0)
                {
                    Interlocked.Increment(ref _statBadFrame);
                    continue;
                }

                ushort target;
                if (type == TileImmature)
                {
                    if (cfg.BloomImmatureHerbs)
                    {
                        target = TileBlooming;
                    }
                    else
                    {
                        double chance = cfg.EffectiveGrowthChancePerScan();
                        if (chance <= 0.0 || Main.rand.NextDouble() >= chance)
                            continue; // 本轮不成熟，下轮扫描再掷骰
                        target = TileMature;
                    }
                }
                else
                {
                    if (!cfg.BloomIgnoreConditions)
                        continue;
                    target = TileBlooming;
                }

                if (target == type)
                    continue;

                tile.type = target;
                tile.frameX = (short)(18 * style);
                tile.frameY = 0;
                converted++;

                // 与原版 GrowAlch 服务端广播完全一致：1x1 tile square（内部含 RangeFrame）。
                NetMessage.SendTileSquare(-1, ch.X, ch.Y, 1, TileChangeType.None);
            }

            if (converted > 0)
            {
                Interlocked.Add(ref _statConverted, converted);
                if (cfg.VerboseLog)
                    TShock.Log.ConsoleInfo("[HerbBloom] 本刻转换 {0} 株", converted);
            }
        }

    }
}
