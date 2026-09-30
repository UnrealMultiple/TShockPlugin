using System.Text.RegularExpressions;

namespace TerraNews;

/// <summary>Every independently switchable piece of news.</summary>
public enum Feature
{
    DailyQuestBoard = 0,
    QuestFishIcon,
    FishingLocation,
    AnglerStatus,
    MoonPhase,
    Sandstorm,
    SandstormPeak,
    TravelingMerchant,
    ServerLog
}

/// <summary>The on/off block; every switch defaults to on.</summary>
public class FeatureSwitches
{
    public bool DailyQuestBoard { get; set; } = true;
    public bool QuestFishIcon { get; set; } = true;
    public bool FishingLocation { get; set; } = true;
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
        Feature.FishingLocation => FishingLocation,
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
            case Feature.FishingLocation: FishingLocation = value; break;
            case Feature.AnglerStatus: AnglerStatus = value; break;
            case Feature.MoonPhase: MoonPhase = value; break;
            case Feature.Sandstorm: Sandstorm = value; break;
            case Feature.SandstormPeak: SandstormPeak = value; break;
            case Feature.TravelingMerchant: TravelingMerchant = value; break;
            case Feature.ServerLog: ServerLog = value; break;
        }
    }

    /// <summary>"任务鱼图标=开 月相=关, ..." for the startup line.</summary>
    public string Summary() => string.Join(" ", All.Select(f => $"{Label(f)}={(this[f] ? "开" : "关")}"));

    private static string Label(Feature feature) => feature switch
    {
        Feature.DailyQuestBoard => "每日任务播报",
        Feature.QuestFishIcon => "任务鱼图标",
        Feature.FishingLocation => "钓鱼地点",
        Feature.AnglerStatus => "渔夫状态",
        Feature.MoonPhase => "月相",
        Feature.Sandstorm => "沙尘暴播报",
        Feature.SandstormPeak => "沙尘暴最强补报",
        Feature.TravelingMerchant => "旅商播报",
        Feature.ServerLog => "写入服务端日志",
        _ => feature.ToString()
    };
}

/// <summary>Links a template placeholder to the feature that owns it.</summary>
public static class FeatureMap
{
    private static readonly Dictionary<string, Feature> Owners = new(StringComparer.OrdinalIgnoreCase)
    {
        ["icon"] = Feature.QuestFishIcon,
        ["biome"] = Feature.FishingLocation,
        ["depth"] = Feature.FishingLocation,
        ["yrange"] = Feature.FishingLocation,
        ["tip"] = Feature.FishingLocation,
        ["angler"] = Feature.AnglerStatus,
        ["moon"] = Feature.MoonPhase,
        ["moon_bonus"] = Feature.MoonPhase
    };

    private static readonly Regex Placeholder = new(@"\{(\w+)\}", RegexOptions.Compiled);

    /// <summary>
    /// A line is dropped when every placeholder it uses belongs to a disabled feature, so
    /// switching 月相 off removes the moon line instead of leaving a dangling label. Static
    /// lines are always kept, and a line mixing a disabled placeholder with a live one stays
    /// with the disabled part rendering empty.
    /// </summary>
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
