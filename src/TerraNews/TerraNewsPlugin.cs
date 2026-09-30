using System.Text;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace TerraNews;

/// <summary>
/// 泰拉新闻 / TerraNews - a small TShock news desk: the 04:30 Angler quest fish and moon,
/// sandstorm / blizzard onset, and the Traveling Merchant's shelf. Driven from
/// ServerApi.Hooks.GameUpdate, which TShock raises from Main.Update - and a dedicated server
/// only calls Main.Update while a client is connected, so news only ever reaches a populated
/// server.
/// </summary>
[ApiVersion(2, 1)]
public class TerraNewsPlugin : TerrariaPlugin
{
    public const string ConfigFileName = "TerraNews.json";

    /// <summary>Terraria NPC type of the Traveling Merchant.</summary>
    public const int MerchantNpcId = 368;

    public override string Name => "TerraNews";
    public override string Author => "TerraNews";
    public override Version Version => new(1, 3, 0);
    public override string Description => GetString("泰拉新闻：每天 04:30 播报渔夫任务鱼与月相，沙尘暴预警，旅商到访货架播报。");

    private static string ConfigPath => Path.Combine(TShock.SavePath, ConfigFileName);

    public static TerraNewsConfig Config { get; private set; } = new();

    private readonly HalfDayTrigger _trigger = new();
    private readonly WorldEventWatcher _events = new();
    private readonly List<Command> _commands = new();

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
        RegisterCommand();

        if (!Config.Enabled)
        {
            TShock.Log.Warn(GetString("[TerraNews] 已加载，但配置中 Enabled=false，插件保持静默。使用 /terranews reload 可重新载入。"));
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
            UnregisterCommands();
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
            // Keep the state machines advancing even while muted, so re-enabling mid-day
            // does not replay an edge that has already gone past.
            _trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks);
            _events.TickSandstorm(Sandstorm.Happening, Sandstorm.Severity, Config.SandstormPeakSeverity);
            _events.TickMerchant(NPC.AnyNPCs(MerchantNpcId));
            return;
        }

        _ready = true;

        // 1) the daily board - strictly inside the 04:30 window, never later in the day
        if (_trigger.Tick(time, dayTime, Config.BroadcastHour, Config.BroadcastMinute, TriggerWindowTicks))
        {
            // The window is consumed either way; only the announcement is optional.
            _trigger.MarkAnnounced();

            if (features[Feature.DailyQuestBoard])
                Broadcast(Config.DailyLines, BuildDailyContext(features), features);
        }

        // 2) weather
        NewsKind storm = _events.TickSandstorm(Sandstorm.Happening, Sandstorm.Severity, Config.SandstormPeakSeverity);
        if (storm == NewsKind.SandstormStarted && features[Feature.Sandstorm])
            Broadcast(Config.SandstormLines, BuildSandstormContext(), features);
        else if (storm == NewsKind.SandstormMaxed && features[Feature.SandstormPeak])
            Broadcast(Config.SandstormPeakLines, BuildSandstormContext(), features);

        // 3) the travelling merchant
        if (_events.TickMerchant(NPC.AnyNPCs(MerchantNpcId)) && features[Feature.TravelingMerchant])
            BroadcastMerchant(features);
    }

    /// <summary>Net ID of today's quest fish (Main.anglerQuestItemNetIDs[Main.anglerQuest]).</summary>
    public static int CurrentQuestFishId
    {
        get
        {
            int[]? pool = Main.anglerQuestItemNetIDs;
            int index = Main.anglerQuest;
            return pool is null || index < 0 || index >= pool.Length ? 0 : pool[index];
        }
    }

    /// <summary>Current game clock as HH:mm.</summary>
    public static string GameClock() => GameTime.Format(Main.time, Main.dayTime);

    private static Dictionary<string, string> BuildDailyContext(FeatureSwitches features)
    {
        int netId = CurrentQuestFishId;
        QuestFish.TryGet(netId, out QuestFishHint? hint);

        string vanilla = SafeVanillaName(netId);
        string nameZh = hint?.NameZh ?? string.Empty;
        bool location = features[Feature.FishingLocation];
        bool moon = features[Feature.MoonPhase];

        return new Dictionary<string, string>
        {
            ["icon"] = features[Feature.QuestFishIcon] ? $"[i:{netId}]" : string.Empty,
            ["name"] = NameText(nameZh, vanilla),
            ["name_zh"] = nameZh,
            ["name_en"] = hint?.NameEn ?? string.Empty,
            ["name_vanilla"] = vanilla,
            ["biome"] = location ? hint?.Biome ?? "未知" : string.Empty,
            ["depth"] = location && hint is not null ? QuestFish.DepthText(hint) : string.Empty,
            ["yrange"] = location && hint is not null && Main.worldSurface > 0
                ? QuestFish.DepthRangeText(hint.Depth, Main.worldSurface, Main.rockLayer)
                : string.Empty,
            ["tip"] = location ? hint?.Tip ?? "向任意渔夫询问即可领取今日任务" : string.Empty,
            ["angler"] = features[Feature.AnglerStatus] ? AnglerStatus() : string.Empty,
            ["moon"] = moon ? MoonPhases.Name(Main.moonPhase) : string.Empty,
            ["moon_bonus"] = moon ? MoonPhases.FishingBonusText(Main.moonPhase) : string.Empty,
            ["time"] = GameClock(),
            ["id"] = netId.ToString()
        };
    }

    private static Dictionary<string, string> BuildSandstormContext() => new()
    {
        ["storm"] = "沙尘暴 / 暴风雪 已登陆",
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

    private static string NameText(string zh, string vanilla) => Config.ResolvedNameSource switch
    {
        "zh" => string.IsNullOrWhiteSpace(zh) ? vanilla : zh,
        "vanilla" => string.IsNullOrWhiteSpace(vanilla) ? zh : vanilla,
        _ => string.IsNullOrWhiteSpace(zh)
            ? vanilla
            : string.IsNullOrWhiteSpace(vanilla) || string.Equals(zh, vanilla, StringComparison.OrdinalIgnoreCase)
                ? zh
                : $"{zh}（{vanilla}）"
    };

    private static string SafeVanillaName(int netId)
    {
        try
        {
            return netId <= 0 ? string.Empty : Lang.GetItemNameValue(netId) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>Plain text only: the caller wraps it in its own tag, or a nested one shows literally.</summary>
    private static string AnglerStatus()
    {
        if (Main.anglerQuestFinished)
            return "状态：今日任务已有人交付，渔夫不再接受该鱼";
        if (!NPC.AnyNPCs(NPCID.Angler))
            return "状态：渔夫当前不在城镇，钓到后记得等他出现";
        return "状态：渔夫在岗，可前往接取 / 交付任务";
    }

    /// <summary>
    /// Substitutes every {placeholder} in a template, drops lines whose placeholders all
    /// belong to switched-off features, and resolves the leading colour tag.
    /// </summary>
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

    private static void Send(TSPlayer target, List<string> lines, Dictionary<string, string> context, FeatureSwitches features)
    {
        if (target is null || lines is null || lines.Count == 0)
            return;

        foreach (ChatLine row in BuildLines(lines, context, features))
            target.SendMessage(row.Text, row.R, row.G, row.B);
    }

    /// <summary>Mirrors a broadcast into the log, rewriting [i:ID] to [物品#ID] since no client renders it there.</summary>
    private static void LogRendered(ChatLine[] rendered, FeatureSwitches features)
    {
        if (!features.ServerLog)
            return;

        var sb = new StringBuilder("[TerraNews] ").AppendLine();
        foreach (ChatLine row in rendered)
            sb.Append("  ").AppendLine(ChatLineParser.ToLogText(row.Text));

        TShock.Log.Info(sb.ToString().TrimEnd());
    }

    // ------------------------------------------------------------------ commands

    private void RegisterCommand()
    {
        UnregisterCommands();

        var names = new List<string> { "terranews" };
        names.AddRange((Config.CommandAliases ?? Array.Empty<string>())
            .Where(a => !string.IsNullOrWhiteSpace(a)));

        var cmd = new Command(MainCommand, names.ToArray())
        {
            HelpText = GetString("泰拉新闻：查看今日渔夫任务。子命令 broadcast|storm|merchant 立即播报，reload 重载配置。")
        };

        _commands.Add(cmd);
        Commands.ChatCommands.Add(cmd);
    }

    private void UnregisterCommands()
    {
        foreach (var cmd in _commands)
            Commands.ChatCommands.Remove(cmd);
        _commands.Clear();
    }

    private void MainCommand(CommandArgs args)
    {
        string sub = args.Parameters.Count > 0 ? args.Parameters[0].ToLowerInvariant() : string.Empty;
        FeatureSwitches features = Config.Features;

        // A command is never blocked by the master switch when it is the switch itself.
        if (sub is not ("reload" or "重载" or "reloadconfig") && !Config.Enabled)
        {
            args.Player.SendErrorMessage(GetString("泰拉新闻已在配置中关闭（Enabled=false）。管理员可用 /terranews reload 重新载入。"));
            return;
        }

        if (sub is "broadcast" or "daily" or "日常")
        {
            if (Gate(args, Feature.DailyQuestBoard) != GateResult.Ok)
                return;

            var context = BuildDailyContext(features);
            Send(args.Player, Config.DailyLines, context, features);
            Broadcast(Config.DailyLines, context, features);
            return;
        }

        if (sub is "storm" or "sandstorm" or "weather" or "天气")
        {
            if (Gate(args, Feature.Sandstorm) != GateResult.Ok)
                return;

            Broadcast(Config.SandstormLines, BuildSandstormContext(), features);
            return;
        }

        if (sub is "merchant" or "shop" or "旅商")
        {
            if (Gate(args, Feature.TravelingMerchant) != GateResult.Ok)
                return;

            BroadcastMerchant(features);
            return;
        }

        if (sub is "reload" or "重载" or "reloadconfig")
        {
            if (Gate(args, null) != GateResult.Ok)
                return;

            ReloadConfig(args.Player);
            return;
        }

        // Bare /terranews: everyone who may run it gets today's board.
        if (Config.ResolvedCommandPermission is { } node && !args.Player.HasPermission(node))
        {
            args.Player.SendErrorMessage(GetString($"你没有权限使用该命令（需要 {node}）。"));
            return;
        }

        Send(args.Player, Config.DailyLines, BuildDailyContext(features), features);
    }

    private enum GateResult { Ok, Denied, Disabled }

    /// <summary>Admin check plus the per-feature switch check, in the order an admin expects.</summary>
    private static GateResult Gate(CommandArgs args, Feature? feature)
    {
        if (!args.Player.HasPermission(Config.ResolvedAdminPermission))
        {
            args.Player.SendErrorMessage(GetString($"你没有权限（需要 {Config.ResolvedAdminPermission}）。"));
            return GateResult.Denied;
        }

        if (feature.HasValue && !Config.Features[feature.Value])
        {
            args.Player.SendErrorMessage(GetString($"该功能已在配置中关闭（Features.{feature.Value}=false）。"));
            return GateResult.Disabled;
        }

        return GateResult.Ok;
    }

    private void ReloadConfig(TSPlayer player)
    {
        LoadConfig();
        _events.Reset();
        _pluginStart = DateTime.UtcNow;
        _ready = false;
        RegisterCommand();

        player.SendMessage(GetString("[TerraNews] 配置已重新载入。"), 120, 220, 255);

        if (Config.Enabled)
            Send(player, Config.DailyLines, BuildDailyContext(Config.Features), Config.Features);
    }
}
