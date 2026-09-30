using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace TerraNews.Core;

/// <summary>A one-shot world event the plugin wants to announce.</summary>
public enum NewsKind
{
    None,
    SandstormStarted,
    SandstormMaxed,
    MerchantArrived
}

/// <summary>
/// Watches the world-event flags TerraNews reports on. All logic is pure and takes
/// primitive inputs so it can be exercised without a running server.
///
/// Sources:
///   * Sandstorm : Terraria.GameContent.Events.Sandstorm.Happening / Severity / TimeLeft
///   * Merchant  : NPC.AnyNPCs(NPCID 368); the stock lives in Main.travelShop, which the
///                 server re-rolls in Chest.SetupTravelShop() right before spawning her
///                 (WorldGen.cs, then NetMessage.SendTravelShop(-1)).
/// </summary>
public sealed class WorldEventWatcher
{
    private bool _stormWasHappening;
    private bool _stormMaxAnnounced;
    private bool _merchantWasPresent;

    /// <summary>Set false by /terranews reload so the next tick reports current state as new.</summary>
    public void Reset()
    {
        _stormWasHappening = false;
        _stormMaxAnnounced = false;
        _merchantWasPresent = false;
    }

    /// <summary>Feeds one tick of storm state. Returns the storm event, if any.</summary>
    public NewsKind TickSandstorm(bool happening, float severity, float maxSeverityThreshold)
    {
        NewsKind kind = NewsKind.None;

        // Rising edge: the storm has just started.
        if (happening && !_stormWasHappening)
        {
            // A storm can begin already at full intensity; do not follow it up with a
            // "it got worse" bulletin for the same storm.
            _stormMaxAnnounced = severity >= maxSeverityThreshold;
            kind = NewsKind.SandstormStarted;
        }
        else if (!happening)
        {
            _stormMaxAnnounced = false;
        }
        // The intensity ramps up over time; tell people when it peaks.
        else if (happening && !_stormMaxAnnounced && severity >= maxSeverityThreshold)
        {
            _stormMaxAnnounced = true;
            if (kind == NewsKind.None)
                kind = NewsKind.SandstormMaxed;
        }

        _stormWasHappening = happening;
        return kind;
    }

    /// <summary>
    /// Feeds one tick of merchant presence. Returns whether she just showed up.
    /// A reload resets the detector so a merchant that is already in town still gets news.
    /// </summary>
    public bool TickMerchant(bool present)
    {
        bool arrived = present && !_merchantWasPresent;
        _merchantWasPresent = present;
        return arrived;
    }

    /// <summary>Human readable intensity for a 0..1 severity value.</summary>
    public static string SeverityText(float severity)
    {
        if (severity >= 0.95f) return "狂暴";
        if (severity >= 0.70f) return "猛烈";
        if (severity >= 0.40f) return "渐强";
        return "初起";
    }

    /// <summary>
    /// Collects the merchant's non-empty stock. Main.travelShop is a fixed 40-slot int[]
    /// where 0 means "empty slot", so the leading zeros are simply skipped.
    /// </summary>
    public static List<int> MerchantStock(int[]? travelShop, string iconFormat = "[i:{0}]")
    {
        var items = new List<int>();
        if (travelShop is null)
            return items;

        for (int i = 0; i < travelShop.Length; i++)
        {
            int id = travelShop[i];
            if (id > 0 && !items.Contains(id))
                items.Add(id);
        }

        return items;
    }

    /// <summary>Renders a stock list as hoverable item icons only, no names.</summary>
    public static string MerchantIconRow(IReadOnlyList<int> stock, string separator = " ")
    {
        if (stock.Count == 0)
            return "（今日货架是空的）";

        var parts = new List<string>(stock.Count);
        foreach (int id in stock)
            parts.Add("[i:" + id.ToString(CultureInfo.InvariantCulture) + "]");

        return string.Join(separator, parts);
    }

    /// <summary>
    /// Rewrites the merchant templates so a long shelf wraps instead of running off the
    /// side of the screen. Every line that contains {items} is emitted once per row; the
    /// header and footer lines around it are emitted once. perLine &lt;= 0 disables wrapping.
    /// </summary>
    public static List<string> ExpandItemLines(List<string>? templates, IReadOnlyList<int> stock, int perLine)
    {
        if (perLine <= 0 || templates is null)
            return templates ?? new List<string>();

        var groups = new List<List<int>>();
        for (int i = 0; i < stock.Count; i += perLine)
            groups.Add(stock.Skip(i).Take(perLine).ToList());

        if (groups.Count <= 1)
            return templates;

        var expanded = new List<string>(templates.Count + groups.Count);
        foreach (string? template in templates)
        {
            if (template is null || !template.Contains("{items}", StringComparison.Ordinal))
            {
                expanded.Add(template ?? string.Empty);
                continue;
            }

            foreach (var group in groups)
                expanded.Add(template.Replace("{items}", MerchantIconRow(group)));
        }

        return expanded;
    }
}
