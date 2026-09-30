using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerraNews.Core;

/// <summary>Every independently switchable piece of news.</summary>
public enum Feature
{
    /// <summary>The 04:30 daily quest board.</summary>
    DailyQuestBoard = 0,

    /// <summary>The hoverable [i:2451] icon of the quest fish.</summary>
    QuestFishIcon,

    /// <summary>Where to fish: biome, depth band, Y range and the tip.</summary>
    FishingLocation,

    /// <summary>Whether the Angler is in town / has already been paid today.</summary>
    AnglerStatus,

    /// <summary>Tonight's moon phase and its fishing power bonus.</summary>
    MoonPhase,

    /// <summary>The broadcast when a sandstorm or blizzard starts.</summary>
    Sandstorm,

    /// <summary>A follow-up broadcast when the storm reaches full intensity.</summary>
    SandstormPeak,

    /// <summary>The Traveling Merchant's shelf, icons only.</summary>
    TravelingMerchant,

    /// <summary>Mirror every broadcast into the host's log file.</summary>
    ServerLog
}

/// <summary>
/// The three broadcast channels. Unlike a Feature, a channel is something a *player*
/// can see arrive as news, so it is the unit the server/client authority handshake talks
/// about. The display-only features (icon, location, status, moon) have no channel:
/// they shape a line inside a broadcast the channel already owns.
/// </summary>
[Flags]
public enum NewsChannel
{
    None = 0,
    Daily = 1 << 0,
    Weather = 1 << 1,
    Merchant = 1 << 2
}

/// <summary>
/// The on/off block that both hosts serialise. Every switch defaults to on, so a fresh
/// config runs the full bulletin and an admin only flips what they want off.
/// </summary>
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

    /// <summary>All features, in declaration order.</summary>
    public static readonly Feature[] All = (Feature[])Enum.GetValues(typeof(Feature));

    /// <summary>Chinese label for the startup summary and the help text.</summary>
    public static string Label(Feature feature) => feature switch
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

    /// <summary>All features that are currently enabled.</summary>
    public List<Feature> Enabled()
    {
        var result = new List<Feature>();
        foreach (Feature feature in All)
        {
            if (this[feature])
                result.Add(feature);
        }

        return result;
    }

    /// <summary>All features that are currently disabled.</summary>
    public List<Feature> Disabled()
    {
        var result = new List<Feature>();
        foreach (Feature feature in All)
        {
            if (!this[feature])
                result.Add(feature);
        }

        return result;
    }

    /// <summary>Which broadcast channels this host produces.</summary>
    public NewsChannel Channels()
    {
        NewsChannel mask = NewsChannel.None;
        if (DailyQuestBoard)
            mask |= NewsChannel.Daily;
        if (Sandstorm)
            mask |= NewsChannel.Weather;
        if (TravelingMerchant)
            mask |= NewsChannel.Merchant;

        return mask;
    }

    /// <summary>"每日任务播报=开, 月相=关, ..." for the startup line and the help text.</summary>
    public string Summary()
    {
        var parts = new List<string>(All.Length);
        foreach (Feature feature in All)
            parts.Add($"{Label(feature)}={(this[feature] ? "开" : "关")}");

        return string.Join(" ", parts);
    }

    /// <summary>Deep copy, so a config editor's clone never shares state with the live one.</summary>
    public FeatureSwitches Clone() => new()
    {
        DailyQuestBoard = DailyQuestBoard,
        QuestFishIcon = QuestFishIcon,
        FishingLocation = FishingLocation,
        AnglerStatus = AnglerStatus,
        MoonPhase = MoonPhase,
        Sandstorm = Sandstorm,
        SandstormPeak = SandstormPeak,
        TravelingMerchant = TravelingMerchant,
        ServerLog = ServerLog
    };
}

/// <summary>Links features to the template placeholders and channels they own.</summary>
public static class FeatureMap
{
    private static readonly Dictionary<string, Feature> ByPlaceholder = new(StringComparer.OrdinalIgnoreCase)
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

    private static readonly Regex PlaceholderPattern = new(@"\{(\w+)\}", RegexOptions.Compiled);

    /// <summary>Feature that owns a placeholder, or null when it is always available.</summary>
    public static Feature? Owner(string placeholder) =>
        ByPlaceholder.TryGetValue(placeholder, out Feature feature) ? feature : null;

    /// <summary>
    /// Decides whether a template line should be sent at all.
    ///
    /// A line is dropped when every placeholder it uses belongs to a disabled feature, so
    /// switching 月相 off removes the moon line instead of leaving a dangling label. A line
    /// with no placeholders is static text and is always kept; a line that mixes a disabled
    /// placeholder with a live one (a label plus the fish name) is kept and the disabled part
    /// simply renders empty.
    /// </summary>
    public static bool ShouldRender(string? template, FeatureSwitches? features)
    {
        if (string.IsNullOrEmpty(template))
            return false;

        if (features is null)
            return true;

        MatchCollection matches = PlaceholderPattern.Matches(template!);
        if (matches.Count == 0)
            return true; // static separator line

        foreach (Match match in matches)
        {
            Feature? owner = Owner(match.Groups[1].Value);
            if (owner is null || features[owner.Value])
                return true;
        }

        return false;
    }
}

/// <summary>
/// Decides who renders a news item when both sides have the mod.
///
/// The rule the brief asks for: the server owns a channel as soon as it broadcasts it, and
/// a client never re-renders a channel the server already sends. Pure logic, so the whole
/// matrix is unit tested rather than reasoned about at runtime.
/// </summary>
public sealed class AuthorityResolver
{
    /// <summary>Channels the server broadcasts, or null while the handshake is outstanding.</summary>
    public NewsChannel? ServerMask { get; private set; }

    /// <summary>Ticks since the mask last changed; used to re-request if the reply is lost.</summary>
    public int HandshakeAge { get; private set; }

    public bool HasServerMask => ServerMask.HasValue;

    /// <summary>Records the mask the server announced.</summary>
    public void ReceiveMask(NewsChannel mask)
    {
        if (ServerMask == mask)
        {
            HandshakeAge++;
            return;
        }

        ServerMask = mask;
        HandshakeAge = 0;
    }

    /// <summary>Forgets the mask, e.g. when the player leaves the world.</summary>
    public void Reset()
    {
        ServerMask = null;
        HandshakeAge = 0;
    }

    /// <summary>
    /// Server side: it renders whenever its own configuration enables the channel. The mask
    /// is about what clients are told, not about what the server does.
    /// </summary>
    public static bool ServerRenders(NewsChannel channel, FeatureSwitches features) =>
        (features.Channels() & channel) == channel;

    /// <summary>
    /// Client side: render for this player only when the server is not already sending the
    /// channel. With no mask yet the answer is "yes" - the client asks, and the server's own
    /// broadcast, if any, will suppress this through the mask before the next event.
    /// </summary>
    public static bool ClientRenders(NewsChannel channel, NewsChannel? serverMask) =>
        serverMask is null || (serverMask.Value & channel) == 0;
}
