using System.Text;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TerraNews;

// 泰拉新闻：一个小型 TShock 新闻台。每天 04:30 播报渔夫任务鱼与月相，
// 沙尘暴/暴风雪登陆，以及旅商到访的货架。全部挂在 ServerApi.Hooks.GameUpdate 上，
// 而 TShock 只在 Main.Update 内部触发它 —— 专用服务器仅在有客户端连接时才调用
// Main.Update，所以新闻只会发往有人的服务器。
[ApiVersion(2, 1)]
public class TerraNewsPlugin : TerrariaPlugin
{
    public const string ConfigFileName = "TerraNews.json";

    // 旅商的 Terraria NPC 类型号。
    public const int MerchantNpcId = 368;

    public override string Name => "TerraNews";
    public override string Author => "不是现在";
    // 尚未发布，版本号固定在 1.0.0。
    public override Version Version => new(1, 0, 0);
    public override string Description => GetString("泰拉新闻：每天 04:30 播报渔夫任务鱼与月相，沙尘暴预警，旅商到访货架播报。");

    private static string ConfigPath => Path.Combine(TShock.SavePath, ConfigFileName);

    public static TerraNewsConfig Config { get; private set; } = new();

    private readonly HalfDayTrigger _trigger = new();
    private readonly WorldEventWatcher _events = new();

    private DateTime? _pluginStart;
    private DateTime _lastDiag = DateTime.MinValue;
    private bool _ready;

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

        TShock.Log.Info(GetString($"[TerraNews] 已加载。总开关=开；每日播报 {Config.BroadcastHour:00}:{Config.BroadcastMinute:00}（游戏内时间，窗口 {Config.TriggerWindowSeconds}s = 04:30–05:00）。"));
        TShock.Log.Info(GetString($"[TerraNews] 功能开关：{Config.Features.Summary()}"));
        TShock.Log.Info(GetString("[TerraNews] 任务鱼图标使用原版聊天物品标签 [i:物品ID]，玩家可悬停查看详情。"));
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
        Config = TerraNewsConfig.Load(ConfigPath, out string? error);
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
                + $"severity={Sandstorm.Severity:0.00} merchant={NPC.AnyNPCs(MerchantNpcId)}"));
        }

        _pluginStart ??= DateTime.UtcNow;
        bool warmingUp = !_ready && (DateTime.UtcNow - _pluginStart.Value).TotalSeconds < Config.StartupDelaySeconds;

        if (warmingUp || !Config.Enabled)
        {
            // 即使静默也继续推进状态机，这样中途重新开启时不会重播已经过去的边沿。
            _trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks);
            _events.TickSandstorm(Sandstorm.Happening, Sandstorm.Severity, Config.SandstormPeakSeverity);
            _events.TickMerchant(NPC.AnyNPCs(MerchantNpcId));
            return;
        }

        _ready = true;

        // 1) 每日看板 —— 严格限制在 04:30 窗口内，绝不在当天更晚的时候补播
        if (_trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks))
        {
            // 窗口无论是否播报都会被消耗掉，区别只在于要不要真的发出去。
            _trigger.MarkAnnounced();

            if (features[Feature.DailyQuestBoard])
                Broadcast(Config.DailyLines, BuildDailyContext(features), features);
        }

        // 2) 天气
        NewsKind storm = _events.TickSandstorm(Sandstorm.Happening, Sandstorm.Severity, Config.SandstormPeakSeverity);
        if (storm != NewsKind.None)
            BroadcastStorm(storm, features);

        // 3) 旅商
        if (_events.TickMerchant(NPC.AnyNPCs(MerchantNpcId)) && features[Feature.TravelingMerchant])
            BroadcastMerchant(features);
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
            ["time"] = GameClock(),
            ["id"] = netId.ToString()
        };
    }

    // 按当前这场到底是雪还是沙，挑对应的模板播报。
    private void BroadcastStorm(NewsKind kind, FeatureSwitches features)
    {
        bool blizzard = IsBlizzard();
        Feature gate = kind == NewsKind.SandstormStarted ? Feature.Sandstorm : Feature.SandstormPeak;
        if (!features[gate])
            return;

        List<string> lines = (kind, blizzard) switch
        {
            (NewsKind.SandstormStarted, false) => Config.SandstormLines,
            (NewsKind.SandstormStarted, true) => Config.BlizzardLines,
            (NewsKind.SandstormMaxed, false) => Config.SandstormPeakLines,
            _ => Config.BlizzardPeakLines
        };

        Broadcast(lines, BuildSandstormContext(blizzard), features);
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

    private static Dictionary<string, string> BuildSandstormContext(bool blizzard) => new()
    {
        ["storm"] = blizzard ? "暴风雪" : "沙尘暴",
        ["severity"] = WorldEventWatcher.SeverityText(Sandstorm.Severity),
        ["remaining"] = GameTime.FormatDuration(Sandstorm.TimeLeft),
        ["time"] = GameClock(),
        ["moon"] = MoonPhases.Name(Main.moonPhase)
    };

    private static Dictionary<string, string> BuildMerchantContext(List<int> stock) => new()
    {
        ["items"] = WorldEventWatcher.MerchantIconRow(stock),
        ["count"] = stock.Count.ToString(),
        ["time"] = GameClock(),
        ["moon"] = MoonPhases.Name(Main.moonPhase)
    };

    private void BroadcastMerchant(FeatureSwitches features)
    {
        var stock = WorldEventWatcher.MerchantStock(Main.travelShop);
        var lines = WorldEventWatcher.ExpandItemLines(Config.MerchantLines, stock, Config.MerchantItemsPerLine);
        Broadcast(lines, BuildMerchantContext(stock), features);
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
    // 再解析行首的颜色标签。
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

            result.Add(ChatLineParser.Parse(text));
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
