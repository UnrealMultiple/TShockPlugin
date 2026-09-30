using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerraNews.Core;

namespace TerraNews;

/// <summary>
/// Flat boolean keys used by TerraNews 1.0 and 1.1, mapped onto the Features block that
/// replaced them. Only consulted while loading, and only for keys the new file does not
/// already define, so upgrading never overwrites a deliberate choice.
/// </summary>
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

/// <summary>
/// Plugin configuration: a plain JSON POCO (Newtonsoft) so it round-trips through the
/// server config folder and can be hot-reloaded with /terranews reload.
/// </summary>
public class TerraNewsConfig
{
    // ---------------------------------------------------------------- master switch

    /// <summary>
    /// Master switch. When false the plugin loads but stays completely silent: no automatic
    /// broadcast, no /terranews output, nothing in the log. /terranews reload still works so
    /// the switch can be flipped back without restarting the server.
    /// </summary>
    [JsonProperty("Enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Per-feature switches, one per news item. Every one of them defaults to on, so a fresh
    /// config runs the full bulletin and an admin only has to flip the ones they want off.
    /// </summary>
    [JsonProperty("Features", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public FeatureSwitches Features { get; set; } = new();

    // ---------------------------------------------------------------- timing

    /// <summary>Game time hour (0-23) of the daily broadcast. 04:30 is dawn, the moment the Angler rolls a new quest.</summary>
    [JsonProperty("BroadcastHour")]
    public int BroadcastHour { get; set; } = 4;

    /// <summary>Game time minute (0-59) of the daily broadcast.</summary>
    [JsonProperty("BroadcastMinute")]
    public int BroadcastMinute { get; set; } = 30;

    /// <summary>
    /// Width of the trigger window, in real seconds. Terraria's clock runs at 60x, so one
    /// real second is one in-game minute: the default 30 covers 04:30 - 05:00, which
    /// survives a brief server hiccup while staying firmly in the early morning. Once the
    /// window closes the board for that day is not sent late - /terranews shows it instead.
    /// </summary>
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
    /// Daily board, sent once at the configured time.
    /// Placeholders: {icon} {name} {name_zh} {name_en} {name_vanilla} {biome} {depth}
    /// {yrange} {tip} {angler} {time} {id} {moon} {moon_bonus}
    ///
    /// The default is deliberately terse. The hover tooltip on {icon} already carries the
    /// fish's name, biome, depth band and how to hook it, so repeating that in the chat log
    /// only made the bulletin harder to read at a glance. The placeholders are all still
    /// available for a server that wants the long form back in its own templates.
    /// </summary>
    [JsonProperty("DailyLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DailyLines { get; set; } = new()
    {
        "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
        "[c/FFD966:任务鱼] [c/FFFFFF:{icon}]",
        "[c/B39DDB:今日月相] [c/FFFFFF:{moon}]",
        "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
    };

    /// <summary>
    /// The 1.2.0 default daily board. Kept only so an upgrade can recognise a config that
    /// still carries the long form and swap it for the terse one.
    /// </summary>
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

            // Write the upgraded shape back so the admin can see the new Features block
            // instead of editing a file whose keys the plugin has quietly stopped reading.
            try
            {
                Save(path, cfg);
            }
            catch
            {
                // A read-only config folder must not stop the plugin from running.
            }

            return cfg;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return new TerraNewsConfig();
        }
    }

    /// <summary>
    /// TerraNews 1.0 used flat booleans (ShowItemIcon, AnnounceSandstorm, ...). They are
    /// folded into the new Features block, but only where the new key is absent, so a
    /// hand-edited 1.1 config is never overwritten by a stale legacy value.
    /// </summary>
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
    /// TerraNews 1.2 shipped a long daily board: the fish's name, the location, the depth,
    /// the tip, the moon bonus and the Angler's status. The hover tooltip on the item icon
    /// already says all of that, so 1.2.1 cuts the board down to the icon and the moon.
    ///
    /// A config that still holds the *shipped* 1.2.0 wording is swapped for the new default,
    /// so upgrading actually changes what players see. A config whose DailyLines were edited
    /// in any way is left completely alone - that is the admin's wording, not ours.
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
