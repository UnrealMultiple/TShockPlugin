namespace TerraNews;

public enum NewsKind
{
    None,
    SandstormStarted,
    SandstormMaxed,
    MerchantArrived
}

// 世界事件的上升沿检测。旅商是 NPC 368，她的货架在 Main.travelShop 里，
// 由 Chest.SetupTravelShop() 在刷出她之前重新掷出。
public sealed class WorldEventWatcher
{
    private bool _stormWasHappening;
    private bool _stormMaxAnnounced;
    private bool _merchantWasPresent;

    // /terranews reload 会把它清零，好让下一刻把当前状态当作新事件报出去。
    public void Reset()
    {
        _stormWasHappening = false;
        _stormMaxAnnounced = false;
        _merchantWasPresent = false;
    }

    // 喂进一个服务器刻的风暴状态，返回这一刻的事件（没有则 None）。
    public NewsKind TickSandstorm(bool happening, float severity, float maxSeverityThreshold)
    {
        NewsKind kind = NewsKind.None;

        if (happening && !_stormWasHappening)
        {
            // 风暴可能一上来就是满强度，那就别再为同一场风暴补一条"变强了"。
            _stormMaxAnnounced = severity >= maxSeverityThreshold;
            kind = NewsKind.SandstormStarted;
        }
        else if (!happening)
        {
            _stormMaxAnnounced = false;
        }
        else if (!_stormMaxAnnounced && severity >= maxSeverityThreshold)
        {
            // 强度会随时间爬升，到顶时通知一声。
            _stormMaxAnnounced = true;
            kind = NewsKind.SandstormMaxed;
        }

        _stormWasHappening = happening;
        return kind;
    }

    // 喂进一个服务器刻的旅商在否，在她出现的那一刻返回 true。
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

    // 旅商货架去重后的商品；Main.travelShop 是 40 格定长数组，0 表示空位。
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

    // 把货架渲染成只有可悬停图标、没有名称的一行。
    public static string MerchantIconRow(IReadOnlyList<int> stock, string separator = " ")
    {
        if (stock.Count == 0)
            return "（今日货架是空的）";

        return string.Join(separator, stock.Select(id => "[i:" + id + "]"));
    }

    // 把含 {items} 的行按每行 perLine 个图标展开；perLine <= 0 表示不换行。
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
