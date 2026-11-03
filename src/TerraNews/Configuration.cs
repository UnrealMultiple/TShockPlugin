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
        ["AnnounceMerchant"] = Feature.TravelingMerchant,
        ["LogToConsole"] = Feature.ServerLog
    };
}

// 纯 JSON POCO，配置放在服务端目录下，改动后需重启服务器。
public class TerraNewsConfig
{
    // 总开关：关闭时插件完全静默，得改配置再重启才会生效。
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

    // 每日看板从触发到发布的延迟（真实秒）。原版旅商的货架是 Chest.SetupTravelShop()
    // 在她刷出之前才掷好的，04:30 那一帧上读到的往往还是昨天的货；等世界把当天
    // 的新货掷出来再读，日报报的才是今天。设为 0 则在触发帧上立即播报。
    [JsonProperty("DailyDelaySeconds")]
    public int DailyDelaySeconds { get; set; } = 2;

    // 沙尘暴与暴风雪怎么区分：auto（按地形扫描）/ sandstorm（一律沙尘暴）/ blizzard（一律暴风雪）。
    [JsonProperty("StormType")]
    public string StormType { get; set; } = "auto";

    // 每秒输出一行触发器正在观察的状态。
    [JsonProperty("Diagnostics")]
    public bool Diagnostics { get; set; }

    // 每日看板，两行：日报本体（月相、渔夫、月相特售）一行，旅商货架单独一行。
    // 占位符：{icon} {angler} {id} {moon} {sale} {merchant} {merchant_items} {time}
    // {merchant} 整段自带「旅商：」前缀，货架为空时整段变空；{merchant_items} 只给图标，
    // 店名由模板自己写。旅商单独成行后，第二行货架为空时就只剩一个「旅商：」标签，
    // 会被 TidySeparators 当成残段丢掉，整行不发，所以行尾也不会挂空标签。
    // 含 [i:物品ID] 的行必须用「一个颜色标签包住整行」的写法，否则图标会变成一串字符。
    [JsonProperty("DailyLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DailyLines { get; set; } = new()
    {
        "[c/4FC3F7:泰拉日报：{moon}；渔夫：{icon}；{sale}]",
        "[c/4FC3F7:{merchant}]"
    };

    // 早期开发版那套七行看板，只为让老配置能被认出来并换成新默认而保留。
    // 它依赖已经删掉的 {name} {biome} {depth} {yrange} {tip} 占位符。
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

    // 旅商拆出去之前的单行默认模板。逐字一致的老配置会被迁到新的两行默认，
    // 不用管理员手动改 TerraNews.json。
    internal const string LegacySingleDailyLine = "[c/4FC3F7:泰拉日报：{moon}；渔夫：{icon}；{sale}；{merchant}]";

    // 沙尘暴与暴风雪。原版只有一个 Sandstorm 事件，雪与沙按地形区分，
    // 所以这里拆成两套模板，播报时才说对名字。这两种天气不改变任何货架，所以不带货物。
    [JsonProperty("SandstormLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormLines { get; set; } = new()
    {
        "[c/E0A458:特别报道：沙尘暴]"
    };

    [JsonProperty("BlizzardLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> BlizzardLines { get; set; } = new()
    {
        "[c/E0A458:特别报道：暴风雪]"
    };

    // 大风天。占位符：{items} {count} {time} {moon}。
    [JsonProperty("WindyLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> WindyLines { get; set; } = new()
    {
        "[c/9AD4C8:特别报道：大风特卖，{items}]"
    };

    // 日食与血月。两者都带货架，用同一个 {items} 占位符。
    [JsonProperty("EclipseLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> EclipseLines { get; set; } = new()
    {
        "[c/B39DDB:特别报道：日食特卖，{items}]"
    };

    [JsonProperty("BloodMoonLines", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> BloodMoonLines { get; set; } = new()
    {
        "[c/8B0000:特别报道：血月特卖，{items}]"
    };

    public void Sanitize()
    {
        BroadcastHour = Math.Clamp(BroadcastHour, 0, 23);
        BroadcastMinute = Math.Clamp(BroadcastMinute, 0, 59);
        TriggerWindowSeconds = Math.Clamp(TriggerWindowSeconds, 1, 120);
        StartupDelaySeconds = Math.Clamp(StartupDelaySeconds, 0, 60);
        DailyDelaySeconds = Math.Clamp(DailyDelaySeconds, 0, 60);

        Features ??= new FeatureSwitches();

        if (DailyLines is null || DailyLines.Count == 0)
            DailyLines = new TerraNewsConfig().DailyLines;
        if (SandstormLines is null || SandstormLines.Count == 0)
            SandstormLines = new List<string> { "[c/E0A458:特别报道：沙尘暴]" };
        if (BlizzardLines is null || BlizzardLines.Count == 0)
            BlizzardLines = new List<string> { "[c/E0A458:特别报道：暴风雪]" };
        if (WindyLines is null || WindyLines.Count == 0)
            WindyLines = new List<string> { "[c/9AD4C8:特别报道：大风特卖，{items}]" };
        if (EclipseLines is null || EclipseLines.Count == 0)
            EclipseLines = new List<string> { "[c/B39DDB:特别报道：日食特卖，{items}]" };
        if (BloodMoonLines is null || BloodMoonLines.Count == 0)
            BloodMoonLines = new List<string> { "[c/8B0000:特别报道：血月特卖，{items}]" };
    }

    // error 是"配置读不出来"，warn 是"读出来了但没能写回去"。两者分开报，
    // 因为后者的处理方式不同：读失败会退回默认值，写失败只是迁移结果留在了内存里。
    public static TerraNewsConfig Load(string path, out string? error, Action<string>? warn = null)
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
            // 已经悄悄不再读取的旧键。回写失败不该让插件跑不起来，但必须让管理员
            // 知道迁移没落盘，否则重启一次就白迁移一次。
            try
            {
                Save(path, cfg);
            }
            catch (Exception ex)
            {
                warn?.Invoke($"配置升级后的回写失败（{ex.Message}）。本次运行仍使用升级后的配置，但重启后会丢失。");
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

    // 把 DailyLines 逐字比对，凡是还等于某一版旧默认的就换成当前默认，
    // 让升级真的改变玩家看到的东西。改过一个字的都原样保留——那是管理员自己的措辞。
    private static void MigrateDailyTemplates(TerraNewsConfig cfg)
    {
        if (cfg.DailyLines is null || cfg.DailyLines.Count == 0)
            return;

        // 旅商拆出去之前的单行默认也照搬同样的规则：没动过模板的配置
        // 直接换成新的两行默认，不然这个升级永远落不到老服务上。
        if (cfg.DailyLines.Count == 1
            && string.Equals(cfg.DailyLines[0], LegacySingleDailyLine, StringComparison.Ordinal))
        {
            cfg.DailyLines = new TerraNewsConfig().DailyLines;
            return;
        }

        if (cfg.DailyLines.Count != LegacyDailyLines.Length)
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
