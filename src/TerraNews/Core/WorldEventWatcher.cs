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
/// Rising-edge detection for the world events TerraNews reports on, over primitive inputs so
/// it runs without a server. Sandstorm state comes from Sandstorm.Happening/Severity; the
/// merchant is NPC 368 and her stock lives in Main.travelShop, re-rolled by
/// Chest.SetupTravelShop() just before she spawns.
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
            // A storm can begin already at full intensity; don't follow it with a "it got
            // worse" bulletin for the same storm.
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

    /// <summary>One tick of merchant presence; true on the tick she shows up.</summary>
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

    /// <summary>Merchant's de-duplicated stock; Main.travelShop is 40 slots where 0 means empty.</summary>
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

    /// <summary>Emits the {items} line once per row of perLine; perLine &lt;= 0 disables wrapping.</summary>
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
