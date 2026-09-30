using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TerraNews;

// 1.0/1.1 的平铺布尔键，映射到后来的 Features 块，只在新文件没有该键时生效。
public static class LegacyFeatureKeys
{
    public static readonly IReadOnlyDictionary<string, Feature> Map = new Dictionary<string, Feature>(StringComparer.OrdinalIgnoreCase)
    {
        ["ShowItemIcon"] = Feature.QuestFishIcon,
        ["ShowMoonPhase"] = Feature.MoonPhase,
        ["ShowAnglerStatus"] = Feature.AnglerStatus,
        ["AnnounceSandstorm"] = Feature.Sandstorm,
        ["AnnounceSandstormPeak"] = Feature.SandstormPeak,
        ["AnnounceMerchant"] = Feature.TravelingMerchant,
        ["LogToConsole"] = Feature.ServerLog
    };
}

// 纯 JSON POCO，配置放在服务端目录下并支持热重载。
public class TerraNewsConfig
{
    // 总开关：关闭时保持静默，但 /terranews reload 仍可用，好把它再打开而不必重启。
    [JsonProperty("Enabled")]
    public bool Enabled { get; set; } = true;

    // 每项功能单独开关，默认全开，所以新配置直接就是完整播报。
    [JsonProperty("Features", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public FeatureSwitches Features { get; set; } = new();

    // 每日播报的游戏内小时（0-23）；04:30 正是渔夫换新任务的时刻。
    [JsonProperty("BroadcastHour")]
    public int BroadcastHour { get; set; } = 4;

    // 每日播报的游戏内分钟（0-59）。
    [JsonProperty("BroadcastMinute")]
    public int BroadcastMinute { get; set; } = 30;

    // 触发窗口，单位是游戏分钟；默认 30 即覆盖 04:30-05:00。
    [JsonProperty("TriggerWindowSeconds")]
    public int TriggerWindowSeconds { get; set; } = 30;

    // 插件加载后保持静默多久（真实秒）。
    [JsonProperty("StartupDelaySeconds")]
    public int StartupDelaySeconds { get; set; } = 5;

    // 风暴强度达到多少（0-1）算作"最强"。
    [JsonProperty("SandstormPeakSeverity")]
    public float SandstormPeakSeverity { get; set; } = 0.95f;

    // 货架每行放几个图标；0 = 不换行。
    [JsonProperty("MerchantItemsPerLine")]
    public int MerchantItemsPerLine { get; set; } = 5;

    // 每秒输出一行触发器正在观察的状态。
    [JsonProperty("Diagnostics")]
    public bool Diagnostics { get; set; }

    // /terranews 的权限节点；留空 = 所有人。
    [JsonProperty("CommandPermission")]
    public string CommandPermission { get; set; } = "";

    // 管理子命令的权限节点；留空 = tshock.admin。
    [JsonProperty("AdminPermission")]
    public string AdminPermission { get; set; } = "";

    // /terranews 的额外别名。
    [JsonProperty("CommandAliases", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public string[] CommandAliases { get; set; } = { "新闻", "泰拉新闻", "news" };

    // 每日看板，刻意做得很短，因为 {icon} 的悬停提示已经包含了其余信息。
    // 占位符：{icon} {angler} {time} {id} {moon} {moon_bonus}
    [JsonProperty("DailyLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DailyLines { get; set; } = new()
    {
        "[c/4FC3F7:========== 泰拉新闻 · 今日渔夫任务 ==========]",
        "[c/FFD966:任务鱼] [c/FFFFFF:{icon}]",
        "[c/B39DDB:今日月相] [c/FFFFFF:{moon}]",
        "[c/888888:（游戏时间 {time}）输入 /terranews 可随时重新查看今日任务]"
    };

    // 1.2.0 的默认看板，仅为升级时能认出来并替换掉而保留。
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

    // 沙尘暴/暴风雪。占位符：{storm} {severity} {remaining} {time} {moon}。
    [JsonProperty("SandstormLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormLines { get; set; } = new()
    {
        "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
        "[c/FFD966:{storm}]",
        "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
        "[c/888888:沙漠起黄沙，雪原飞暴雪，出行注意（游戏时间 {time}）]"
    };

    // 风暴达到最强时的补报，占位符与 SandstormLines 相同。
    [JsonProperty("SandstormPeakLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormPeakLines { get; set; } = new()
    {
        "[c/E0A458:========== 泰拉新闻 · 天气预警 ==========]",
        "[c/FF6B6B:{storm} 已达最强]",
        "[c/FFFFFF:强度 {severity} · 预计持续 {remaining}]",
        "[c/888888:能见度极差，建议尽快返回城镇（游戏时间 {time}）]"
    };

    // 旅商到访。占位符：{items} {count} {time} {moon}。
    [JsonProperty("MerchantLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> MerchantLines { get; set; } = new()
    {
        "[c/4FC3F7:========== 泰拉新闻 · 旅商到访 ==========]",
        "[c/FFD966:今日货架（悬停查看详情）]",
        "[c/FFFFFF:{items}]",
        "[c/888888:共 {count} 件 · 售完即止（游戏时间 {time}）]"
    };

    [JsonIgnore]
    public string? ResolvedCommandPermission =>
        string.IsNullOrWhiteSpace(CommandPermission) ? null : CommandPermission;

    [JsonIgnore]
    public string ResolvedAdminPermission =>
        string.IsNullOrWhiteSpace(AdminPermission) ? "tshock.admin" : AdminPermission;


    public void Sanitize()
    {
        BroadcastHour = Math.Clamp(BroadcastHour, 0, 23);
        BroadcastMinute = Math.Clamp(BroadcastMinute, 0, 59);
        TriggerWindowSeconds = Math.Clamp(TriggerWindowSeconds, 1, 120);
        StartupDelaySeconds = Math.Clamp(StartupDelaySeconds, 0, 60);
        MerchantItemsPerLine = Math.Clamp(MerchantItemsPerLine, 0, 12);
        SandstormPeakSeverity = Math.Clamp(SandstormPeakSeverity, 0.5f, 1f);

        Features ??= new FeatureSwitches();

        if (DailyLines is null || DailyLines.Count == 0)
            DailyLines = new List<string> { "[c/4FC3F7:今日渔夫任务] [c/FFFFFF:{icon} {name}]" };
        if (SandstormLines is null || SandstormLines.Count == 0)
            SandstormLines = new List<string> { "[c/E0A458:天气预警] [c/FFFFFF:{storm}]" };
        if (SandstormPeakLines is null || SandstormPeakLines.Count == 0)
            SandstormPeakLines = new List<string>(SandstormLines);
        if (MerchantLines is null || MerchantLines.Count == 0)
            MerchantLines = new List<string> { "[c/4FC3F7:旅商到访] [c/FFFFFF:{items}]" };
        CommandAliases ??= Array.Empty<string>();
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

            // 把升级后的结构写回去，这样管理员看到的是新字段，而不是一份我们
            // 已经悄悄不再读取的旧键；只读的配置目录不该让插件跑不起来。
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

    // 1.0 的平铺布尔键，只在新的 Features 块里缺该键时才搬进去。
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

            if (features is JObject obj && obj[entry.Value.ToString()] is not null)
                continue;

            cfg.Features.Set(entry.Value, legacy.Value<bool>());
        }
    }

    // 把仍然是 1.2.0 出货文案的 DailyLines 换成新默认，让升级真的改变玩家看到的东西。
    // 任何一处被手工改过的 DailyLines 都原样保留——那是管理员自己的措辞。
    private static void MigrateDailyTemplates(TerraNewsConfig cfg)
    {
        if (cfg.DailyLines is null || cfg.DailyLines.Count != LegacyDailyLines.Length)
            return;

        for (int i = 0; i < LegacyDailyLines.Length; i++)
            if (!string.Equals(cfg.DailyLines[i], LegacyDailyLines[i], StringComparison.Ordinal))
                return;

        cfg.DailyLines = new TerraNewsConfig().DailyLines;
    }

    public static void Save(string path, TerraNewsConfig cfg)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
    }
}
