namespace TerraNews;

// 世界事件的上升沿检测，只在事件出现的那一刻报一次，不跟进强度变化。
// 旅商的货架不进这里：它不是上升沿事件，而是每天日报里单独成行的一段固定内容，
// 直接读 Main.travelShop 就行，货由 Chest.SetupTravelShop() 在刷出她之前掷好。
public sealed class WorldEventWatcher
{
    private bool _stormWasHappening;
    private bool _windyWasOn;
    private bool _eclipseWasOn;
    private bool _bloodMoonWasOn;

    // 喂进一个服务器刻的风暴状态，在它开始的那一刻返回 true。
    public bool TickSandstorm(bool happening)
    {
        bool started = happening && !_stormWasHappening;
        _stormWasHappening = happening;
        return started;
    }

    // 大风天每天黎明掷一次，只在白天有货，所以取「白天且有风」。
    public bool TickWindy(bool windyDaytime)
    {
        bool started = windyDaytime && !_windyWasOn;
        _windyWasOn = windyDaytime;
        return started;
    }

    public bool TickEclipse(bool on)
    {
        bool started = on && !_eclipseWasOn;
        _eclipseWasOn = on;
        return started;
    }

    public bool TickBloodMoon(bool on)
    {
        bool started = on && !_bloodMoonWasOn;
        _bloodMoonWasOn = on;
        return started;
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
    public static string MerchantIconRow(IReadOnlyList<int> stock)
    {
        if (stock.Count == 0)
            return "（今日货架是空的）";

        return string.Join(string.Empty, stock.Select(id => "[i:" + id + "]"));
    }

    // 旅商那一段，默认模板里单独占一行，格式和其他店一样。货架是空的就不产出
    // 这一段：在单行模板里表现为不留悬空分号，在单独成行的默认模板里表现为
    // 整行不发，行尾不会挂一个光秃秃的「旅商：」。Main.travelShop 由
    // Chest.SetupTravelShop() 在刷出旅商之前掷好，所以人不在场时读到的是她上次的货。
    public static string MerchantRow(int[]? travelShop)
    {
        var stock = MerchantStock(travelShop);
        return stock.Count == 0 ? string.Empty : "旅商：" + MerchantIconRow(stock);
    }
}
