using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerraNews.Core;

namespace TerraNews;

/// <summary>1.0/1.1 flat booleans mapped onto the Features block, for keys the new file omits.</summary>
public static class LegacyFeatureKeys
{
    public static readonly IReadOnlyDictionary<string, Feature> Map = new Dictionary<string, Feature>(StringComparer.OrdinalIgnoreCase)
    {
        ["ShowItemIcon"] = Feature.QuestFishIcon,
        ["ShowMoonPhase"] = Feature.MoonPhase,
        ["ShowDepthRange"] = Feature.FishingLocation,
        ["ShowAnglerStatus"] = Feature.AnglerStatus,
        ["AnnounceSandstorm"] = Feature.Sandstorm,
        ["AnnounceSandstormPeak"] = Feature.SandstormPeak,
        ["AnnounceMerchant"] = Feature.TravelingMerchant,
        ["LogToConsole"] = Feature.ServerLog
    };
}

/// <summary>Plain JSON POCO so the config round-trips through the server folder and hot-reloads.</summary>
public class TerraNewsConfig
{
    // ---------------------------------------------------------------- master switch

    /// <summary>Master switch: silent when false, except /terranews reload so it can be flipped back.</summary>
    [JsonProperty("Enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>Per-feature switches; all default on so a fresh config runs the full bulletin.</summary>
    [JsonProperty("Features", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public FeatureSwitches Features { get; set; } = new();

    // ---------------------------------------------------------------- timing

    /// <summary>Game time hour (0-23) of the daily broadcast. 04:30 is dawn, the moment the Angler rolls a new quest.</summary>
    [JsonProperty("BroadcastHour")]
    public int BroadcastHour { get; set; } = 4;

    /// <summary>Game time minute (0-59) of the daily broadcast.</summary>
    [JsonProperty("BroadcastMinute")]
    public int BroadcastMinute { get; set; } = 30;

    /// <summary>Trigger window in real seconds; at 60x, 30 means 04:30-05:00 in-game.</summary>
    [JsonProperty("TriggerWindowSeconds")]
    public int TriggerWindowSeconds { get; set; } = 30;

    /// <summary>How long (real seconds) to stay quiet after the plugin loads.</summary>
    [JsonProperty("StartupDelaySeconds")]
    public int StartupDelaySeconds { get; set; } = 5;

    // ---------------------------------------------------------------- thresholds

    /// <summary>Severity (0-1) at which the storm counts as "peak".</summary>
    [JsonProperty("SandstormPeakSeverity")]
    public float SandstormPeakSeverity { get; set; } = 0.95f;

    /// <summary>How many merchant icons fit on one chat line. 0 = never wrap.</summary>
    [JsonProperty("MerchantItemsPerLine")]
    public int MerchantItemsPerLine { get; set; } = 5;

    // ---------------------------------------------------------------- appearance

    /// <summary>Fish name source: zh | vanilla | both.</summary>
    [JsonProperty("NameSource")]
    public string NameSource { get; set; } = "both";

    /// <summary>Log one line per second with the state the trigger is watching.</summary>
    [JsonProperty("Diagnostics")]
    public bool Diagnostics { get; set; } = false;

    // ---------------------------------------------------------------- permissions

    /// <summary>Permission node for /terranews. Empty = everyone.</summary>
    [JsonProperty("CommandPermission")]
    public string CommandPermission { get; set; } = "";

    /// <summary>Permission node for the admin subcommands. Empty = tshock.admin.</summary>
    [JsonProperty("AdminPermission")]
    public string AdminPermission { get; set; } = "";

    /// <summary>Extra aliases for /terranews.</summary>
    [JsonProperty("CommandAliases", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public string[] CommandAliases { get; set; } = { "新闻", "泰拉新闻", "news" };

    // ---------------------------------------------------------------- templates

    /// <summary>
    /// Daily board, deliberately terse because the {icon} hover tooltip already says the rest.
    /// Placeholders: {icon} {name} {name_zh} {name_en} {name_vanilla} {biome} {depth}
    /// {yrange} {tip} {angler} {time} {id} {moon} {moon_bonus}
    /// </summary>
    [JsonProperty("DailyLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DailyLines { get; set; } = new()
    {
        "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
        "[c/FFD966:任务鱼] [c/FFFFFF:{icon}]",
        "[c/B39DDB:今日月相] [c/FFFFFF:{moon}]",
        "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
    };

    /// <summary>The 1.2.0 default board, kept only so an upgrade can recognise and swap it.</summary>
    internal static readonly string[] LegacyDailyLines =
    {
        "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
        "[c/FFD966:任务鱼] [c/FFFFFF:{icon} {name}]",
        "[c/FFD966:钓鱼地点] [c/FFFFFF:{biome} · {depth}{yrange}]",
        "[c/FFD966:提示] [c/FFFFFF:{tip}]",
        "[c/B39DDB:今日月相] [c/FFFFFF:{moon} · {moon_bonus}]",
        "[c/7FD4FF:{angler}]",
        "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
    };

    /// <summary>Sandstorm / blizzard. Placeholders: {storm} {severity} {remaining} {time} {moon}.</summary>
    [JsonProperty("SandstormLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormLines { get; set; } = new()
    {
        "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
        "[c/FFD966:{storm}]",
        "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
        "[c/888888:沙漠起黄沙，雪原飞暴雪，出行注意（游戏时间 {time}）]"
    };

    /// <summary>Sandstorm at full intensity. Same placeholders as SandstormLines.</summary>
    [JsonProperty("SandstormPeakLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormPeakLines { get; set; } = new()
    {
        "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
        "[c/FF6B6B:{storm} 已达最强]",
        "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
        "[c/888888:能见度极差，建议尽快返回城镇（游戏时间 {time}）]"
    };

    /// <summary>Travelling Merchant. Placeholders: {items} {count} {time} {moon}.</summary>
    [JsonProperty("MerchantLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> MerchantLines { get; set; } = new()
    {
        "[c/4FC3F7:========== 泰拉新闻 · 旅商到访 ==========]",
        "[c/FFD966:今日货架（悬停查看详情）]",
        "[c/FFFFFF:{items}]",
        "[c/888888:共 {count} 件 · 售完即止（游戏时间 {time}）]"
    };

    // ---------------------------------------------------------------- derived

    [JsonIgnore]
    public string ResolvedCommandPermission =>
        string.IsNullOrWhiteSpace(CommandPermission) ? null! : CommandPermission;

    [JsonIgnore]
    public string ResolvedAdminPermission =>
        string.IsNullOrWhiteSpace(AdminPermission) ? "tshock.admin" : AdminPermission;

    [JsonIgnore]
    public string ResolvedNameSource =>
        (NameSource ?? "both").Trim().ToLowerInvariant() switch
        {
            "zh" or "cn" or "chinese" => "zh",
            "vanilla" or "en" or "en_us" => "vanilla",
            _ => "both"
        };

    // ---------------------------------------------------------------- io

    public void Sanitize()
    {
        BroadcastHour = Math.Clamp(BroadcastHour, 0, 23);
        BroadcastMinute = Math.Clamp(BroadcastMinute, 0, 59);
        TriggerWindowSeconds = Math.Clamp(TriggerWindowSeconds, 1, 120);
        StartupDelaySeconds = Math.Clamp(StartupDelaySeconds, 0, 60);
        MerchantItemsPerLine = Math.Clamp(MerchantItemsPerLine, 0, 12);
        SandstormPeakSeverity = Math.Clamp(SandstormPeakSeverity, 0.5f, 1f);
        NameSource = ResolvedNameSource;

        Features ??= new FeatureSwitches();

        if (DailyLines is null || DailyLines.Count == 0)
            DailyLines = new List<string> { "[c/4FC3F7:今日渔夫任务] [c/FFFFFF:{icon} {name}]" };
        if (SandstormLines is null || SandstormLines.Count == 0)
            SandstormLines = new List<string> { "[c/E0A458:天气预警] [c/FFFFFF:{storm}]" };
        if (SandstormPeakLines is null || SandstormPeakLines.Count == 0)
            SandstormPeakLines = new List<string>(SandstormLines);
        if (MerchantLines is null || MerchantLines.Count == 0)
            MerchantLines = new List<string> { "[c/4FC3F7:旅商到访] [c/FFFFFF:{items}]" };
        if (CommandAliases is null)
            CommandAliases = Array.Empty<string>();
    }

    public static TerraNewsConfig Load(string path, out string? error)
    {
        error = null;
        try
        {
            if (!File.Exists(path))
            {
                var fresh = new TerraNewsConfig();
                fresh.Sanitize();
                Save(path, fresh);
                return fresh;
            }

            string text = File.ReadAllText(path);
            var cfg = JsonConvert.DeserializeObject<TerraNewsConfig>(text);
            if (cfg is null)
            {
                error = "配置文件内容为空";
                return new TerraNewsConfig();
            }

            MigrateLegacySwitches(cfg, text);
            MigrateDailyTemplates(cfg);
            cfg.Sanitize();

            // Write the upgraded shape back so the admin sees the new block rather than
            // editing a file whose keys we quietly stopped reading; a read-only folder must
            // not stop the plugin from running.
            try
            {
                Save(path, cfg);
            }
            catch
            {
            }

            return cfg;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return new TerraNewsConfig();
        }
    }

    /// <summary>1.0's flat booleans, folded into Features only where the new key is absent.</summary>
    private static void MigrateLegacySwitches(TerraNewsConfig cfg, string json)
    {
        JObject root;
        try
        {
            root = JObject.Parse(json);
        }
        catch
        {
            return;
        }

        JToken? features = root["Features"];
        foreach (KeyValuePair<string, Feature> entry in LegacyFeatureKeys.Map)
        {
            JToken? legacy = root[entry.Key];
            if (legacy is null || legacy.Type != JTokenType.Boolean)
                continue;

            bool value = legacy.Value<bool>();
            bool alreadyConfigured = features is JObject obj && obj[entry.Value.ToString()] is not null;
            if (alreadyConfigured)
                continue;

            cfg.Features.Set(entry.Value, value);
        }
    }

    /// <summary>
    /// Swaps the untouched 1.2.0 wording for the new default, so upgrading really changes
    /// what players see. DailyLines edited in any way are the admin's wording and left alone.
    /// </summary>
    private static void MigrateDailyTemplates(TerraNewsConfig cfg)
    {
        if (cfg.DailyLines is null || cfg.DailyLines.Count != LegacyDailyLines.Length)
            return;

        for (int i = 0; i < LegacyDailyLines.Length; i++)
        {
            if (!string.Equals(cfg.DailyLines[i], LegacyDailyLines[i], StringComparison.Ordinal))
                return;
        }

        var fresh = new TerraNewsConfig();
        cfg.DailyLines = fresh.DailyLines;
    }

    public static void Save(string path, TerraNewsConfig cfg)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir!);

        File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
    }
}
