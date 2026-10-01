namespace TerraNews;

// 世界事件的上升沿检测，只在事件出现的那一刻报一次，不跟进强度变化。
// 旅商是 NPC 368，她的货架在 Main.travelShop 里，由 Chest.SetupTravelShop()
// 在刷出她之前重新掷出。
public sealed class WorldEventWatcher
{
    private bool _stormWasHappening;
    private bool _merchantWasPresent;

    // 喂进一个服务器刻的风暴状态，在它开始的那一刻返回 true。
    public bool TickSandstorm(bool happening)
    {
        bool started = happening && !_stormWasHappening;
        _stormWasHappening = happening;
        return started;
    }

    // 喂进一个服务器刻的旅商在否，在她出现的那一刻返回 true。
    public bool TickMerchant(bool present)
    {
        bool arrived = present && !_merchantWasPresent;
        _merchantWasPresent = present;
        return arrived;
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
