using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Terraria.ID;

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
    [JsonProperty("启用")]
    public bool Enabled { get; set; } = true;

    // 每项功能单独开关，默认全开，所以新配置直接就是完整播报。
    [JsonProperty("功能开关", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public FeatureSwitches Features { get; set; } = new();

    // 每日播报的游戏内小时（0-23）；04:30 正是渔夫换新任务的时刻。
    [JsonProperty("播报小时")]
    public int BroadcastHour { get; set; } = 4;

    // 每日播报的游戏内分钟（0-59）。
    [JsonProperty("播报分钟")]
    public int BroadcastMinute { get; set; } = 30;

    // 触发窗口，单位是游戏分钟；默认 30 即覆盖 04:30-05:00。
    [JsonProperty("触发窗口秒数")]
    public int TriggerWindowSeconds { get; set; } = 30;

    // 插件加载后保持静默多久（真实秒）。
    [JsonProperty("启动延迟秒数")]
    public int StartupDelaySeconds { get; set; } = 5;

    // 每日看板从触发到发布的延迟（真实秒）。旅商的货架已独立成 MerchantLines 到货播报，
    // 这个延迟现在只管看板本身的发布节奏。设为 0 则在触发帧上立即播报。
    [JsonProperty("看板延迟秒数")]
    public int DailyDelaySeconds { get; set; } = 2;

    // 旅商到货播报从检测到发布之间的延迟（真实秒）。她的货是刷出那一刻才掷定的，
    // 到货后再没人重掷，压住这一小会儿再读，报的就是她这一趟卖的货，客户端也收到了
    // 货架同步包。设为 0 则在到货检测帧上立即播报。
    [JsonProperty("旅商延迟秒数")]
    public int MerchantDelaySeconds { get; set; } = 2;

    // 沙尘暴与暴风雪怎么区分：auto（按地形扫描）/ sandstorm（一律沙尘暴）/ blizzard（一律暴风雪）。
    [JsonProperty("风暴判定")]
    public string StormType { get; set; } = "auto";

    // 每秒输出一行触发器正在观察的状态。
    [JsonProperty("诊断日志")]
    public bool Diagnostics { get; set; }

    // 每日看板，默认一行：月相、渔夫、月相特售。旅商货架已独立成 MerchantLines，
    // 只在她到货时播报——她的货是刷出那一刻才掷定的，日报在 04:30 抢读必然是旧的。
    // 占位符：{icon} {angler} {id} {moon} {moon_bonus} {sale} {merchant} {merchant_items} {time}
    // {merchant} 整段自带「旅商：」前缀，货架为空时整段变空；{merchant_items} 只给图标，
    // 店名由模板自己写。想把旅商放回日报就自己写回 DailyLines，功能不变。
    // 含 [i:物品ID] 的行必须用「一个颜色标签包住整行」的写法，否则图标会变成一串字符。
    [JsonProperty("每日看板模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DailyLines { get; set; } = new()
    {
        "[c/4FC3F7:泰拉日报：{moon}；渔夫：{icon}；{sale}]"
    };

    // 旅商到货播报，在她刷出（NPC 368 出现）的上升沿触发、等 MerchantDelaySeconds
    // 现实秒再读货架发布。当天她没来就不发；昨天守着没走（有玩家在她附近时原版不
    // 放人）不算新到货，不重复发。占位符同 DailyLines，常用 {merchant} 与 {merchant_items}。
    [JsonProperty("旅商到货模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> MerchantLines { get; set; } = new()
    {
        "[c/4FC3F7:旅商来了！今日货架：{merchant_items}]"
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

    // 历代默认模板（不含当前默认）。逐字一致的老配置会被迁到当前默认，
    // 不用管理员手动改 TerraNews.json。
    // v1.0.x 的单行默认，末尾挂着旅商货架。
    internal const string LegacySingleDailyLine = "[c/4FC3F7:泰拉日报：{moon}；渔夫：{icon}；{sale}；{merchant}]";

    // v1.1–1.2 的两行默认，旅商单独成行；拆去 MerchantLines 后不再用。
    internal static readonly string[] LegacyTwoLineDailyLines =
    {
        "[c/4FC3F7:泰拉日报：{moon}；渔夫：{icon}；{sale}]",
        "[c/4FC3F7:{merchant}]"
    };

    // 沙尘暴与暴风雪。原版只有一个 Sandstorm 事件，雪与沙按地形区分，
    // 所以这里拆成两套模板，播报时才说对名字。这两种天气不改变任何货架，所以不带货物。
    [JsonProperty("沙尘暴模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> SandstormLines { get; set; } = new()
    {
        "[c/E0A458:特别报道：沙尘暴]"
    };

    [JsonProperty("暴风雪模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> BlizzardLines { get; set; } = new()
    {
        "[c/E0A458:特别报道：暴风雪]"
    };

    // 大风天。占位符：{items} {count} {time} {moon}。
    [JsonProperty("大风模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> WindyLines { get; set; } = new()
    {
        "[c/9AD4C8:特别报道：大风特卖，{items}]"
    };

    // 日食与血月。两者都带货架，用同一个 {items} 占位符。
    [JsonProperty("日食模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> EclipseLines { get; set; } = new()
    {
        "[c/B39DDB:特别报道：日食特卖，{items}]"
    };

    [JsonProperty("血月模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> BloodMoonLines { get; set; } = new()
    {
        "[c/8B0000:特别报道：血月特卖，{items}]"
    };

    // 天亮（游戏内 04:30）与天黑（19:30）提醒，各自单独成条、单独开关
    // （Features 里的 Dawn / Dusk）。只在昼夜翻转的那一刻播一次，每天各最多一条；
    // 04:30 的天亮与每日看板（BroadcastHour/BroadcastMinute 同为 04:30）是两条
    // 独立消息，各走各的开关。占位符只有 {time} 和 {moon}。
    [JsonProperty("天亮模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DawnLines { get; set; } = new()
    {
        "[c/FFD966:天亮了（{time}），新的一天开始了]"
    };

    [JsonProperty("天黑模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> DuskLines { get; set; } = new()
    {
        "[c/6A5ACD:天黑了（{time}），夜里注意安全]"
    };

    // 草药提醒，与上面的事件播报各自独立成条、各自独立开关（Features 里的
    // FullMoonHerb / BloodMoonHerb / RainHerb）。死亡草在满月夜与血月夜开花、
    // 水叶草在下雨时开花，开花时才采得到种子。血月正逢满月的那一晚只发血月那条，
    // 同一晚不连发两条死亡草提醒。占位符只有 {time} 和 {moon}。
    // 含 [i:物品ID] 图标的行必须用「一个颜色标签包住整行」的写法，理由同 DailyLines。
    [JsonProperty("满月草药模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> FullMoonHerbLines { get; set; } = new()
    {
        $"[c/9CB767:草药提醒：今夜满月，[i:{ItemID.Deathweed}]死亡草开花了，记得采种]"
    };

    [JsonProperty("血月草药模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> BloodMoonHerbLines { get; set; } = new()
    {
        $"[c/8B0000:草药提醒：血月夜，[i:{ItemID.Deathweed}]死亡草开花了，记得采种]"
    };

    [JsonProperty("雨天草药模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> RainHerbLines { get; set; } = new()
    {
        $"[c/4FC3F7:草药提醒：下雨了，[i:{ItemID.Waterleaf}]水叶草开花了，记得采种]"
    };

    // 火焰花在下午 3:30–7:30（游戏内）开花，或泡在岩浆里随时可采；这里在开花后的
    // 3:45 提醒一次，每天只发这一条。占位符同上，只有 {time} 和 {moon}。
    [JsonProperty("火焰花模板", ObjectCreationHandling = ObjectCreationHandling.Replace)]
    public List<string> FireblossomHerbLines { get; set; } = new()
    {
        $"[c/E2621B:草药提醒：{{time}}，[i:{ItemID.Fireblossom}]火焰花开花了，记得采种]"
    };

    public void Sanitize()
    {
        BroadcastHour = Math.Clamp(BroadcastHour, 0, 23);
        BroadcastMinute = Math.Clamp(BroadcastMinute, 0, 59);
        TriggerWindowSeconds = Math.Clamp(TriggerWindowSeconds, 1, 120);
        StartupDelaySeconds = Math.Clamp(StartupDelaySeconds, 0, 60);
        DailyDelaySeconds = Math.Clamp(DailyDelaySeconds, 0, 60);
        MerchantDelaySeconds = Math.Clamp(MerchantDelaySeconds, 0, 60);

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

        TerraNewsConfig defaults = new();
        if (DawnLines is null || DawnLines.Count == 0)
            DawnLines = defaults.DawnLines;
        if (DuskLines is null || DuskLines.Count == 0)
            DuskLines = defaults.DuskLines;
        if (FullMoonHerbLines is null || FullMoonHerbLines.Count == 0)
            FullMoonHerbLines = defaults.FullMoonHerbLines;
        if (BloodMoonHerbLines is null || BloodMoonHerbLines.Count == 0)
            BloodMoonHerbLines = defaults.BloodMoonHerbLines;
        if (RainHerbLines is null || RainHerbLines.Count == 0)
            RainHerbLines = defaults.RainHerbLines;
        if (FireblossomHerbLines is null || FireblossomHerbLines.Count == 0)
            FireblossomHerbLines = defaults.FireblossomHerbLines;
        if (MerchantLines is null || MerchantLines.Count == 0)
            MerchantLines = defaults.MerchantLines;
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
            text = RenameEnglishKeys(text);
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

    // v1.1.2：英文键名 → 中文键名。老配置先就地改名再反序列化，所有值原样保留；
    // 已经是中文键的文件在此处不会被碰到。1.0 的平铺旧键（ShowItemIcon 等）不在此列，
    // 仍交给 MigrateLegacySwitches 处理。
    private static readonly IReadOnlyDictionary<string, string> RenamedKeys = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Enabled"] = "启用",
        ["Features"] = "功能开关",
        ["BroadcastHour"] = "播报小时",
        ["BroadcastMinute"] = "播报分钟",
        ["TriggerWindowSeconds"] = "触发窗口秒数",
        ["StartupDelaySeconds"] = "启动延迟秒数",
        ["DailyDelaySeconds"] = "看板延迟秒数",
        ["MerchantDelaySeconds"] = "旅商延迟秒数",
        ["StormType"] = "风暴判定",
        ["Diagnostics"] = "诊断日志",
        ["DailyLines"] = "每日看板模板",
        ["MerchantLines"] = "旅商到货模板",
        ["SandstormLines"] = "沙尘暴模板",
        ["BlizzardLines"] = "暴风雪模板",
        ["WindyLines"] = "大风模板",
        ["EclipseLines"] = "日食模板",
        ["BloodMoonLines"] = "血月模板",
        ["DawnLines"] = "天亮模板",
        ["DuskLines"] = "天黑模板",
        ["FullMoonHerbLines"] = "满月草药模板",
        ["BloodMoonHerbLines"] = "血月草药模板",
        ["RainHerbLines"] = "雨天草药模板",
        ["FireblossomHerbLines"] = "火焰花模板",

        // 功能开关块里的英文键名。Rename 同样作用于「功能开关」块，v1.1.2 的映射只
        // 覆盖了顶层键，这里补齐功能块的键；Dawn/Dusk 是 v1.2.0 的新开关。
        ["DailyQuestBoard"] = "每日看板",
        ["QuestFishIcon"] = "任务鱼图标",
        ["AnglerStatus"] = "渔夫状态",
        ["MoonPhase"] = "月相",
        ["MoonSale"] = "月相特售",
        ["Sandstorm"] = "沙尘暴播报",
        ["WindyDay"] = "大风播报",
        ["Eclipse"] = "日食播报",
        ["BloodMoon"] = "血月播报",
        ["Dawn"] = "天亮提醒",
        ["Dusk"] = "天黑提醒",
        ["FullMoonHerb"] = "满月草药提醒",
        ["BloodMoonHerb"] = "血月草药提醒",
        ["RainHerb"] = "雨天草药提醒",
        ["FireblossomHerb"] = "火焰花提醒",
        ["TravelingMerchant"] = "旅商货架",
        ["ServerLog"] = "控制台日志"
    };

    private static string RenameEnglishKeys(string json)
    {
        JObject root;
        try
        {
            root = JObject.Parse(json);
        }
        catch
        {
            return json;
        }

        void Rename(JObject obj)
        {
            foreach (KeyValuePair<string, string> kv in RenamedKeys)
            {
                // 注意：必须用 JObject.Remove(key) 删整个属性；
                // obj[key].Remove() 删的是 JProperty 的值节点，会抛 JProperty 异常
                if (!obj.ContainsKey(kv.Key) || obj.ContainsKey(kv.Value))
                    continue;
                JToken? token = obj[kv.Key];
                obj.Remove(kv.Key);
                obj[kv.Value] = token;
            }
        }

        Rename(root);
        if (root["功能开关"] is JObject features)
            Rename(features);
        return root.ToString(Formatting.None);
    }

    // 1.0 的平铺布尔键，只在新的功能开关块里缺该键时才搬进去。
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

        JToken? features = root["功能开关"];
        foreach (KeyValuePair<string, Feature> entry in LegacyFeatureKeys.Map)
        {
            JToken? legacy = root[entry.Key];
            if (legacy is null || legacy.Type != JTokenType.Boolean)
                continue;

            if (features is JObject obj && obj[FeatureSwitches.JsonKeys[entry.Value]] is not null)
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

        // 旧默认逐版列全：没动过模板的配置直接换成当前默认，不然升级永远落不到老服务上。
        string[][] legacyDefaults =
        {
            LegacyDailyLines,
            new[] { LegacySingleDailyLine },
            LegacyTwoLineDailyLines
        };

        foreach (string[] legacy in legacyDefaults)
        {
            if (cfg.DailyLines.Count != legacy.Length)
                continue;

            bool identical = true;
            for (int i = 0; i < legacy.Length; i++)
                if (!string.Equals(cfg.DailyLines[i], legacy[i], StringComparison.Ordinal))
                {
                    identical = false;
                    break;
                }

            if (identical)
            {
                cfg.DailyLines = new TerraNewsConfig().DailyLines;
                return;
            }
        }
    }

    public static void Save(string path, TerraNewsConfig cfg)
    {
        string? dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, JsonConvert.SerializeObject(cfg, Formatting.Indented));
    }
}
