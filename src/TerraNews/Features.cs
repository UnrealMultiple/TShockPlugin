using System.Text.RegularExpressions;

namespace TerraNews;

// 每一项可以单独开关的新闻。
public enum Feature
{
    DailyQuestBoard = 0,
    QuestFishIcon,
    AnglerStatus,
    MoonPhase,
    Sandstorm,
    SandstormPeak,
    TravelingMerchant,
    ServerLog
}

// 开关组，默认为全开。
public class FeatureSwitches
{
    public bool DailyQuestBoard { get; set; } = true;
    public bool QuestFishIcon { get; set; } = true;
    public bool AnglerStatus { get; set; } = true;
    public bool MoonPhase { get; set; } = true;
    public bool Sandstorm { get; set; } = true;
    public bool SandstormPeak { get; set; } = true;
    public bool TravelingMerchant { get; set; } = true;
    public bool ServerLog { get; set; } = true;

    public static readonly Feature[] All = (Feature[])Enum.GetValues(typeof(Feature));

    public bool this[Feature feature] => feature switch
    {
        Feature.DailyQuestBoard => DailyQuestBoard,
        Feature.QuestFishIcon => QuestFishIcon,
        Feature.AnglerStatus => AnglerStatus,
        Feature.MoonPhase => MoonPhase,
        Feature.Sandstorm => Sandstorm,
        Feature.SandstormPeak => SandstormPeak,
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
            case Feature.Sandstorm: Sandstorm = value; break;
            case Feature.SandstormPeak: SandstormPeak = value; break;
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
        Feature.Sandstorm => "沙尘暴播报",
        Feature.SandstormPeak => "沙尘暴最强补报",
        Feature.TravelingMerchant => "旅商播报",
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
        ["moon_bonus"] = Feature.MoonPhase
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
