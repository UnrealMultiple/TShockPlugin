namespace TerraNews;

public enum NewsKind
{
    None,
    SandstormStarted,
    SandstormMaxed,
    MerchantArrived
}

/// <summary>
/// Rising-edge detection for the world events TerraNews reports on. The merchant is NPC 368
/// and her stock lives in Main.travelShop, re-rolled by Chest.SetupTravelShop() just before
/// she spawns.
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
        else if (!_stormMaxAnnounced && severity >= maxSeverityThreshold)
        {
            // Intensity ramps up over time; tell people when it peaks.
            _stormMaxAnnounced = true;
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

    public static string SeverityText(float severity)
    {
        if (severity >= 0.95f) return "狂暴";
        if (severity >= 0.70f) return "猛烈";
        if (severity >= 0.40f) return "渐强";
        return "初起";
    }

    /// <summary>Merchant's de-duplicated stock; Main.travelShop is 40 slots where 0 means empty.</summary>
    public static List<int> MerchantStock(int[]? travelShop)
    {
        var items = new List<int>();
        if (travelShop is null)
            return items;

        foreach (int id in travelShop)
            if (id > 0 && !items.Contains(id))
                items.Add(id);

        return items;
    }

    /// <summary>Renders a stock list as hoverable item icons only, no names.</summary>
    public static string MerchantIconRow(IReadOnlyList<int> stock, string separator = " ")
    {
        if (stock.Count == 0)
            return "（今日货架是空的）";

        return string.Join(separator, stock.Select(id => "[i:" + id + "]"));
    }

    /// <summary>Emits the {items} line once per row of perLine; perLine &lt;= 0 disables wrapping.</summary>
    public static List<string> ExpandItemLines(List<string>? templates, IReadOnlyList<int> stock, int perLine)
    {
        if (perLine <= 0 || templates is null)
            return templates ?? new List<string>();

        var groups = stock.Select((id, i) => new { id, i })
            .GroupBy(x => x.i / perLine)
            .Select(g => g.Select(x => x.id).ToList())
            .ToList();

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
