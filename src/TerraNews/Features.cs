using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TerraNews;

// 每一项可以单独开关的新闻。
public enum Feature
{
    DailyQuestBoard = 0,
    QuestFishIcon,
    AnglerStatus,
    MoonPhase,
    MoonSale,
    Sandstorm,
    WindyDay,
    Eclipse,
    BloodMoon,
    Dawn,
    Dusk,
    FullMoonHerb,
    BloodMoonHerb,
    RainHerb,
    FireblossomHerb,
    TravelingMerchant,
    ServerLog
}

// 开关组，默认为全开。JSON 键名 v1.1.2 起为中文（与 JsonKeys 一一对应）。
public class FeatureSwitches
{
    [JsonProperty("每日看板")]
    public bool DailyQuestBoard { get; set; } = true;
    [JsonProperty("任务鱼图标")]
    public bool QuestFishIcon { get; set; } = true;
    [JsonProperty("渔夫状态")]
    public bool AnglerStatus { get; set; } = true;
    [JsonProperty("月相")]
    public bool MoonPhase { get; set; } = true;
    [JsonProperty("月相特售")]
    public bool MoonSale { get; set; } = true;
    [JsonProperty("沙尘暴播报")]
    public bool Sandstorm { get; set; } = true;
    [JsonProperty("大风播报")]
    public bool WindyDay { get; set; } = true;
    [JsonProperty("日食播报")]
    public bool Eclipse { get; set; } = true;
    [JsonProperty("血月播报")]
    public bool BloodMoon { get; set; } = true;
    [JsonProperty("天亮提醒")]
    public bool Dawn { get; set; } = true;
    [JsonProperty("天黑提醒")]
    public bool Dusk { get; set; } = true;
    [JsonProperty("满月草药提醒")]
    public bool FullMoonHerb { get; set; } = true;
    [JsonProperty("血月草药提醒")]
    public bool BloodMoonHerb { get; set; } = true;
    [JsonProperty("雨天草药提醒")]
    public bool RainHerb { get; set; } = true;
    [JsonProperty("火焰花提醒")]
    public bool FireblossomHerb { get; set; } = true;
    [JsonProperty("旅商货架")]
    public bool TravelingMerchant { get; set; } = true;
    [JsonProperty("控制台日志")]
    public bool ServerLog { get; set; } = true;

    public static readonly Feature[] All = (Feature[])Enum.GetValues(typeof(Feature));

    // 每个功能在配置文件里对应的中文键名，迁移逻辑按此查找。
    public static readonly IReadOnlyDictionary<Feature, string> JsonKeys = new Dictionary<Feature, string>
    {
        [Feature.DailyQuestBoard] = "每日看板",
        [Feature.QuestFishIcon] = "任务鱼图标",
        [Feature.AnglerStatus] = "渔夫状态",
        [Feature.MoonPhase] = "月相",
        [Feature.MoonSale] = "月相特售",
        [Feature.Sandstorm] = "沙尘暴播报",
        [Feature.WindyDay] = "大风播报",
        [Feature.Eclipse] = "日食播报",
        [Feature.BloodMoon] = "血月播报",
        [Feature.Dawn] = "天亮提醒",
        [Feature.Dusk] = "天黑提醒",
        [Feature.FullMoonHerb] = "满月草药提醒",
        [Feature.BloodMoonHerb] = "血月草药提醒",
        [Feature.RainHerb] = "雨天草药提醒",
        [Feature.FireblossomHerb] = "火焰花提醒",
        [Feature.TravelingMerchant] = "旅商货架",
        [Feature.ServerLog] = "控制台日志"
    };

    public bool this[Feature feature] => feature switch
    {
        Feature.DailyQuestBoard => DailyQuestBoard,
        Feature.QuestFishIcon => QuestFishIcon,
        Feature.AnglerStatus => AnglerStatus,
        Feature.MoonPhase => MoonPhase,
        Feature.MoonSale => MoonSale,
        Feature.Sandstorm => Sandstorm,
        Feature.WindyDay => WindyDay,
        Feature.Eclipse => Eclipse,
        Feature.BloodMoon => BloodMoon,
        Feature.Dawn => Dawn,
        Feature.Dusk => Dusk,
        Feature.FullMoonHerb => FullMoonHerb,
        Feature.BloodMoonHerb => BloodMoonHerb,
        Feature.RainHerb => RainHerb,
        Feature.FireblossomHerb => FireblossomHerb,
        Feature.TravelingMerchant => TravelingMerchant,
        Feature.ServerLog => ServerLog,
        _ => true
    };

    public void Set(Feature feature, bool value)
    {
        switch (feature)
        {
            case Feature.DailyQuestBoard: DailyQuestBoard = value; break;
            case Feature.QuestFishIcon: QuestFishIcon = value; break;
            case Feature.AnglerStatus: AnglerStatus = value; break;
            case Feature.MoonPhase: MoonPhase = value; break;
            case Feature.MoonSale: MoonSale = value; break;
            case Feature.Sandstorm: Sandstorm = value; break;
            case Feature.WindyDay: WindyDay = value; break;
            case Feature.Eclipse: Eclipse = value; break;
            case Feature.BloodMoon: BloodMoon = value; break;
            case Feature.Dawn: Dawn = value; break;
            case Feature.Dusk: Dusk = value; break;
            case Feature.FullMoonHerb: FullMoonHerb = value; break;
            case Feature.BloodMoonHerb: BloodMoonHerb = value; break;
            case Feature.RainHerb: RainHerb = value; break;
            case Feature.FireblossomHerb: FireblossomHerb = value; break;
            case Feature.TravelingMerchant: TravelingMerchant = value; break;
            case Feature.ServerLog: ServerLog = value; break;
        }
    }

    // 拼成 "任务鱼图标=开 月相=关, ..."，用于启动日志。
    public string Summary() => string.Join(" ", All.Select(f => $"{Label(f)}={(this[f] ? "开" : "关")}"));

    private static string Label(Feature feature) => feature switch
    {
        Feature.DailyQuestBoard => "每日任务播报",
        Feature.QuestFishIcon => "任务鱼图标",
        Feature.AnglerStatus => "渔夫状态",
        Feature.MoonPhase => "月相",
        Feature.MoonSale => "月相特售",
        Feature.Sandstorm => "风暴播报",
        Feature.WindyDay => "大风播报",
        Feature.Eclipse => "日食播报",
        Feature.BloodMoon => "血月播报",
        Feature.Dawn => "天亮提醒",
        Feature.Dusk => "天黑提醒",
        Feature.FullMoonHerb => "满月草药提醒",
        Feature.BloodMoonHerb => "血月草药提醒",
        Feature.RainHerb => "雨天草药提醒",
        Feature.FireblossomHerb => "火焰花提醒",
        Feature.TravelingMerchant => "旅商货架",
        Feature.ServerLog => "写入服务端日志",
        _ => feature.ToString()
    };
}

// 占位符与它所属功能的对应关系。
public static class FeatureMap
{
    private static readonly Dictionary<string, Feature> Owners = new(StringComparer.OrdinalIgnoreCase)
    {
        ["icon"] = Feature.QuestFishIcon,
        ["angler"] = Feature.AnglerStatus,
        ["moon"] = Feature.MoonPhase,
        ["moon_bonus"] = Feature.MoonPhase,
        ["sale"] = Feature.MoonSale,
        ["merchant"] = Feature.TravelingMerchant,
        ["merchant_items"] = Feature.TravelingMerchant
    };

    private static readonly Regex Placeholder = new(@"\{(\w+)\}", RegexOptions.Compiled);

    // 某个占位符所属的功能被关掉时整行不播，避免只剩一个空标签。纯静态行始终保留；
    // 混有关闭与开启占位符的行照播，关闭的那部分渲染成空字符串。
    public static bool ShouldRender(string? template, FeatureSwitches? features)
    {
        if (string.IsNullOrEmpty(template))
            return false;
        if (features is null)
            return true;

        bool found = false;
        foreach (Match match in Placeholder.Matches(template))
        {
            found = true;
            if (!Owners.TryGetValue(match.Groups[1].Value, out Feature owner) || features[owner])
                return true;
        }

        return !found;
    }
}
