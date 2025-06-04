using Economics.Core.Enumerates;
using Newtonsoft.Json;

namespace Economics.Core.ConfigFiles;

public class CustomizeCurrency
{
    [JsonProperty("货币名称")]
    public string Name { get; set; } = "魂力";

    [JsonProperty("兑换关系")]
    public List<RedemptionRelationshipsOption> RedemptionRelationships { get; set; } = new();

    [JsonProperty("获取关系")]
    public CurrencyObtainOption CurrencyObtain { get; set; } = new();

    [JsonProperty("死亡掉落")]
    public DeathFallOption DeathFallOption { get; set; } = new();

    [JsonProperty("悬浮文本")]
    public CombatMsgOption CombatMsgOption { get; set; } = new();

    [JsonProperty("查询提示")]
    public string QueryFormat = "[c/FFA500: 当前拥有{0}{1}个]";
}

public class CombatMsgOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("提示文本")]
    public string CombatMsg { get; set; } = "+{0}$";

    public int[] Color { get; set; } = [255, 255, 255];
}

public class DeathFallOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("掉落比例")]
    public float DropRate { get; set; } = 0.1f;
}

public class CurrencyObtainOption
{
    [JsonProperty("获取方式")]
    public CurrencyObtainType CurrencyObtainType { get; set; } = default;

    [JsonProperty("给予数量")]
    public long GiveCurrency { get; set; }

    [JsonProperty("指定ID")]
    public HashSet<int> ContainsID { get; set; } = [];

    [JsonProperty("比例")]
    public float ConversionRate { get; set; } = 1f;
}

public class RedemptionRelationshipsOption
{
    [JsonProperty("数量")]
    public long Number { get; set; }

    [JsonProperty("货币类型")]
    public string CurrencyType { get; set; } = "魂力";

    public override string ToString()
    {
        return $"{this.CurrencyType}x{this.Number}";
    }
}
