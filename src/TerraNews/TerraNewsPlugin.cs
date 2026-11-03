using System.Text;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TerraNews;

// 泰拉新闻：一个小型 TShock 新闻台。每天 04:30 播报今日月相、渔夫任务鱼与
// 各家随月相轮换的货架，沙尘暴/暴风雪、大风、日食、血月的特别报道，以及旅商到访。
// 全部挂在 ServerApi.Hooks.GameUpdate 上，而 TShock 只在 Main.Update 内部触发它 ——
// 专用服务器仅在有客户端连接时才调用 Main.Update，所以新闻只会发往有人的服务器。
[ApiVersion(2, 1)]
public class TerraNewsPlugin : TerrariaPlugin
{
    public const string ConfigFileName = "TerraNews.json";

    public override string Name => "TerraNews";
    public override string Author => "不是现在";
    // 版本 1.0.1。
    public override Version Version => new(1, 0, 1);
    public override string Description => GetString("泰拉新闻：每天 04:30 播报月相、渔夫任务鱼与月相特售货架，大风、日食、血月、沙尘暴与暴风雪的特别报道。");

    private static string ConfigPath => Path.Combine(TShock.SavePath, ConfigFileName);

    public static TerraNewsConfig Config { get; private set; } = new();

    private readonly HalfDayTrigger _trigger = new();
    private readonly WorldEventWatcher _events = new();

    private DateTime? _pluginStart;
    private DateTime _lastDiag = DateTime.MinValue;
    private bool _ready;

    // 每日看板已触发但还压在延迟期里：到 _dailyDueUtc 后才重新读取世界状态并发布。
    private bool _dailyWaiting;
    private DateTime _dailyDueUtc;

    public TerraNewsPlugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadConfig();
        ServerApi.Hooks.GameInitialize.Register(this, OnGameInitialize);
        ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
    }

    public void OnGameInitialize(EventArgs args)
    {
        if (!Config.Enabled)
        {
            TShock.Log.Warn(GetString("[TerraNews] 已加载，但配置中 Enabled=false，插件保持静默。改完 tshock/TerraNews.json 后需重启服务器。"));
            return;
        }

        // TriggerWindowSeconds 这个键名有历史包袱，实际单位是游戏分钟（见 TriggerWindowTicks），
        // 所以窗口终点直接加它、不除 60。
        int windowEnd = (Config.BroadcastHour * 60 + Config.BroadcastMinute + Config.TriggerWindowSeconds) % 1440;
        TShock.Log.Info(GetString(
            $"[TerraNews] 已加载。总开关=开；每日播报 {Config.BroadcastHour:00}:{Config.BroadcastMinute:00}"
            + $"（游戏内时间，窗口 {Config.TriggerWindowSeconds} 游戏分钟，即到 {windowEnd / 60:00}:{windowEnd % 60:00}"
            + $",触发后等 {Config.DailyDelaySeconds} 现实秒再检测发布）。"));
        TShock.Log.Info(GetString($"[TerraNews] 功能开关：{Config.Features.Summary()}"));
        TShock.Log.Info(GetString("[TerraNews] 任务鱼图标使用原版聊天物品标签 [i:物品ID]，玩家可悬停查看详情。"));
        TShock.Log.Info(GetString($"[TerraNews] 月相特售对照表来源：{MoonSale.SourceGameVersion}"));
        if (MoonSale.VersionWarning(Main.versionNumber) is { } mismatch)
            TShock.Log.Warn(GetString(mismatch));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameInitialize.Deregister(this, OnGameInitialize);
            ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
        }

        base.Dispose(disposing);
    }

    private void LoadConfig()
    {
        Config = TerraNewsConfig.Load(
            ConfigPath,
            out string? error,
            message => TShock.Log.Warn(GetString($"[TerraNews] {message}")));

        if (!string.IsNullOrEmpty(error))
            TShock.Log.Error(GetString($"[TerraNews] 配置读取失败（{error}），已使用默认配置。"));
    }

    private double TriggerWindowTicks => Config.TriggerWindowSeconds * GameTime.TicksPerGameMinute;

    private void OnUpdate(EventArgs args)
    {
        double time = Main.time;
        bool dayTime = Main.dayTime;
        FeatureSwitches features = Config.Features;

        if (Config.Diagnostics && (DateTime.UtcNow - _lastDiag).TotalSeconds >= 1.0)
        {
            _lastDiag = DateTime.UtcNow;
            TShock.Log.Info(GetString(
                $"[TerraNews][diag] dayTime={dayTime} time={time:0.0} clock={GameClock()} "
                + $"halfDay={_trigger.HalfDayIndex} announced={_trigger.AnnouncedHalfDay} "
                + $"quest={CurrentQuestFishId} moon={Main.moonPhase} storm={Sandstorm.Happening} "
                + $"travelShop={Main.travelShop?.Count(id => id > 0) ?? -1}"));
        }

        _pluginStart ??= DateTime.UtcNow;
        bool warmingUp = !_ready && (DateTime.UtcNow - _pluginStart.Value).TotalSeconds < Config.StartupDelaySeconds;

        if (warmingUp || !Config.Enabled)
        {
            // 事件监视器要照常推进，否则静默期里发生过的风暴、大风、日食、血月会在解除
            // 静默后被当成刚发生而补播一条。每日窗口不消耗：服务器若恰好在窗口内启动，
            // 静默期结束时玩家仍应看到当天看板，把这条吞掉才是真丢信息。
            _trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks);
            TickEvents();
            return;
        }

        _ready = true;

        // 1) 每日看板 —— 严格限制在 04:30 窗口内触发，绝不在当天更晚的时候补播；
        //    触发后再等 DailyDelaySeconds 现实秒才读取世界状态并发布
        if (_trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks))
        {
            // 窗口无论是否播报都会被消耗掉，区别只在于要不要真的发出去。
            _trigger.MarkAnnounced();

            if (features[Feature.DailyQuestBoard])
            {
                // 旅商的货架是 Chest.SetupTravelShop() 在她刷出之前才掷好的，04:30
                // 触发帧上读到的往往还是昨天的货。压住这几秒，等原版把当天的
                // 新货掷出来，发布时再重新检测（BuildDailyContext 到那时才拼内容）。
                if (Config.DailyDelaySeconds > 0)
                {
                    _dailyWaiting = true;
                    _dailyDueUtc = DateTime.UtcNow.AddSeconds(Config.DailyDelaySeconds);
                }
                else
                {
                    Broadcast(Config.DailyLines, BuildDailyContext(features), features);
                }
            }
        }

        // 延迟期到点：重新检测，再发布。到点前服务器哪怕整场没再跑一个刻（玩家
        // 全下了），恢复推进后的第一个刻也会把这条发出去，不会丢。
        if (_dailyWaiting && DateTime.UtcNow >= _dailyDueUtc)
        {
            _dailyWaiting = false;
            Broadcast(Config.DailyLines, BuildDailyContext(Config.Features), Config.Features);
        }

        // 2) 各种一次性事件
        TickEvents();
    }

    // 沙尘暴/暴风雪、大风、日食、血月都只在出现的那一刻播一次。
    private void TickEvents()
    {
        FeatureSwitches features = Config.Features;

        if (_events.TickSandstorm(Sandstorm.Happening))
            BroadcastStorm(IsBlizzard(), features);

        // 大风只在白天有货，夜里报了就是假消息。
        if (_events.TickWindy(Main.dayTime && Main.IsItAHappyWindyDay) && features[Feature.WindyDay])
            BroadcastSale(Config.WindyLines, MoonSale.WindyDay(), features);

        if (_events.TickEclipse(Main.eclipse) && features[Feature.Eclipse])
            BroadcastSale(Config.EclipseLines, MoonSale.EclipseSale(), features);

        if (_events.TickBloodMoon(Main.bloodMoon) && features[Feature.BloodMoon])
            BroadcastSale(Config.BloodMoonLines, MoonSale.BloodMoonSale(WorldGen.crimson), features);
    }

    // 走 {items} 占位符的一次性播报，物品由 MoonSale 的表拼成。
    private void BroadcastSale(List<string> lines, (Shop Shop, Lot[] Lots)[] rows, FeatureSwitches features)
    {
        string items = MoonSale.Render(rows);
        Broadcast(lines, new Dictionary<string, string>
        {
            ["items"] = items,
            ["count"] = rows.Sum(r => r.Lots.Length).ToString(),
            ["time"] = GameClock(),
            ["moon"] = MoonPhases.Name(Main.moonPhase)
        }, features);
    }

    // 今日任务鱼的 Net ID（Main.anglerQuestItemNetIDs[Main.anglerQuest]）。
    public static int CurrentQuestFishId
    {
        get
        {
            int[]? pool = Main.anglerQuestItemNetIDs;
            int index = Main.anglerQuest;
            return pool is null || index < 0 || index >= pool.Length ? 0 : pool[index];
        }
    }

    // 当前游戏时刻，形如 HH:mm。
    public static string GameClock() => GameTime.Format(Main.time, Main.dayTime);

    // 每日播报的可替换变量。鱼的一切信息都在 {icon} 的悬停提示里，这里只给得出 ID。
    // {merchant} 整段自带「旅商：」前缀，货架为空时整段变空——默认模板把它单独放一行，
    // 于是空货时那一行只剩「旅商：」，会被 TidySeparators 当成残段丢掉、整行不发。
    // {merchant_items} 只给图标，店名由模板自己写，同样挂在 TravelingMerchant 开关下。
    private static Dictionary<string, string> BuildDailyContext(FeatureSwitches features)
    {
        int netId = CurrentQuestFishId;
        bool moon = features[Feature.MoonPhase];

        return new Dictionary<string, string>
        {
            ["icon"] = features[Feature.QuestFishIcon] ? $"[i:{netId}]" : string.Empty,
            ["angler"] = features[Feature.AnglerStatus] ? AnglerStatus() : string.Empty,
            ["moon"] = moon ? MoonPhases.Name(Main.moonPhase) : string.Empty,
            ["moon_bonus"] = moon ? MoonPhases.FishingBonusText(Main.moonPhase) : string.Empty,
            ["sale"] = features[Feature.MoonSale]
                ? MoonSale.Render(MoonSale.Today(Main.moonPhase, Main.remixWorld, Main.tenthAnniversaryWorld))
                : string.Empty,
            ["merchant"] = features[Feature.TravelingMerchant] ? WorldEventWatcher.MerchantRow(Main.travelShop) : string.Empty,
            ["merchant_items"] = features[Feature.TravelingMerchant]
                ? WorldEventWatcher.MerchantIconRow(WorldEventWatcher.MerchantStock(Main.travelShop))
                : string.Empty,
            ["time"] = GameClock(),
            ["id"] = netId.ToString()
        };
    }

    // 按当前这场到底是雪还是沙，挑对应的模板播报。只在风暴出现的那一刻播一次。
    private void BroadcastStorm(bool blizzard, FeatureSwitches features)
    {
        if (!features[Feature.Sandstorm])
            return;

        Broadcast(blizzard ? Config.BlizzardLines : Config.SandstormLines,
            new Dictionary<string, string>
            {
                ["storm"] = blizzard ? "暴风雪" : "沙尘暴",
                ["time"] = GameClock(),
                ["moon"] = MoonPhases.Name(Main.moonPhase)
            }, features);
    }

    // 原版的沙尘暴与暴风雪是同一个 Sandstorm 事件，客户端按 player.ZoneSnow &&
    // player.ZoneRain 决定是黄沙还是暴雪。可这两个标志在专用服务器上不可靠：
    // ZoneRain = Main.raining && Y <= worldSurface 是全局的、可靠，但 ZoneSnow 来自
    // 客户端算的 Main.SceneMetrics，服务器上读到的是过期值——"一直下雨"的种子又让
    // ZoneRain 恒为真，于是 ZoneSnow && ZoneRain 退化成只看 ZoneSnow，随便一个玩家
    // 就能把沙尘暴误判成暴风雪。
    //
    // 所以不问玩家，改看地形：抽样地表明层，数雪块和沙块谁多。这个结果只跟世界本身
    // 有关，缓存一次即可。世界里两种地形都有时无法两全，按 StormType 配置由服主定夺。
    private static bool? _worldIsSnowy;

    private static bool IsBlizzard()
    {
        return Config.StormType switch
        {
            "sandstorm" => false,
            "blizzard" => true,
            _ => _worldIsSnowy ??= ScanSurfaceForSnow()
        };
    }

    // 抽样扫描地表明层。沙漠地表是沙块(32)，雪原地表是雪块(51)，数这两种就够。
    private static bool ScanSurfaceForSnow()
    {
        int snow = 0, sand = 0;
        int top = (int)Main.worldSurface - 8;
        int bottom = (int)Main.worldSurface + 56;

        for (int x = 0; x < Main.maxTilesX; x += 4)
        {
            for (int y = top; y < bottom && y < Main.maxTilesY; y++)
            {
                if (!WorldGen.InWorld(x, y)) continue;

                switch (Main.tile[x, y].type)
                {
                    case TileID.SnowBlock: snow++; break;
                    case TileID.Sand: sand++; break;
                }
            }
        }

        TShock.Log.Info($"[TerraNews] 地形判定：雪块 {snow} 格，沙块 {sand} 格"
            + $" → 这场按{(snow > sand ? "暴风雪" : "沙尘暴")}播报。");
        return snow > sand;
    }

    // 渔夫状态。只能输出纯文本，因为外层颜色标签被剥掉后嵌套标签会变成字面文本。
    private static string AnglerStatus()
    {
        if (Main.anglerQuestFinished)
            return "状态：今日任务已有人交付，渔夫不再接受该鱼";
        if (!NPC.AnyNPCs(NPCID.Angler))
            return "状态：渔夫当前不在城镇，钓到后记得等他出现";
        return "状态：渔夫在岗，可前往接取 / 交付任务";
    }

    // 替换模板里的 {占位符}，丢掉占位符全部属于已关闭功能的那几行，
    // 清掉拼剩下的空段，再解析行首的颜色标签。
    private static ChatLine[] BuildLines(List<string> templates, Dictionary<string, string> context, FeatureSwitches? features)
    {
        var result = new List<ChatLine>(templates.Count);
        foreach (string template in templates)
        {
            if (!FeatureMap.ShouldRender(template, features))
                continue;

            string text = template ?? string.Empty;
            foreach (var pair in context)
                text = text.Replace("{" + pair.Key + "}", pair.Value);

            // 先剥颜色标签再清空段：反过来会把行首那个半截标签当成残段丢掉。
            ChatLine row = ChatLineParser.Parse(text);
            string tidy = ChatLineParser.TidySeparators(row.Text);
            if (tidy.Length > 0)
                result.Add(row with { Text = tidy });
        }

        return result.ToArray();
    }

    private void Broadcast(List<string> lines, Dictionary<string, string> context, FeatureSwitches features)
    {
        ChatLine[] rendered = BuildLines(lines, context, features);
        LogRendered(rendered, features);

        foreach (ChatLine row in rendered)
            TSPlayer.All.SendMessage(row.Text, row.R, row.G, row.B);
    }

    // 把播报镜像进日志，并把 [i:ID] 改写成 [物品#ID] —— 日志里没有客户端来渲染图标。
    private static void LogRendered(ChatLine[] rendered, FeatureSwitches features)
    {
        if (!features.ServerLog)
            return;

        var sb = new StringBuilder("[TerraNews] ").AppendLine();
        foreach (ChatLine row in rendered)
            sb.Append("  ").AppendLine(ChatLineParser.ToLogText(row.Text));

        TShock.Log.Info(sb.ToString().TrimEnd());
    }
}
