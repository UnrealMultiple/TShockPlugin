using Newtonsoft.Json;

namespace Economics.Core.ConfigFiles;

public class CurrencyDefinition
{
    [JsonProperty("货币名称")]
    public string Name { get; set; } = "金币";

    [JsonProperty("货币描述")]
    public string Description { get; set; } = "";

    [JsonProperty("可兑换为")]
    public Dictionary<string, long> ExchangeRates { get; set; } = new();

    [JsonProperty("获取关系")]
    public CurrencyObtainOption CurrencyObtain { get; set; } = new();

    [JsonProperty("死亡掉落")]
    public DeathFallOption DeathFallOption { get; set; } = new();

    [JsonProperty("悬浮文本")]
    public CombatMsgOption CombatMsgOption { get; set; } = new();

    [JsonProperty("查询提示")]
    public string QueryFormat = "[c/FFA500: 当前拥有{0}{1}个]";

    [JsonProperty("是否可通过兑换获得")]
    public bool CanBeObtainedByExchange { get; set; }

    [JsonProperty("是否可兑换为其他货币")]
    public bool CanExchangeToOthers => this.ExchangeRates.Count > 0;
}

public class ExchangePath
{
    public string FromCurrency { get; set; } = "";

    public string ToCurrency { get; set; } = "";

    public long Rate { get; set; }

    public decimal ReverseRate => this.Rate > 0 ? 1m / this.Rate : 0;

    public string Description => $"1 {this.FromCurrency} = {this.Rate} {this.ToCurrency}";
}

public class ExchangeValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];

    public void AddError(string error)
    {
        this.Errors.Add(error);
    }

    public void AddWarning(string warning)
    {
        this.Warnings.Add(warning);
    }
}

public class ArbitrageCycle
{
    public List<string> CyclePath { get; set; } = [];

    public List<decimal> Rates { get; set; } = [];

    public decimal ArbitrageMultiplier { get; set; }

    public bool HasArbitrage => this.ArbitrageMultiplier > 1.01m; // 允许1%的浮点误差

    public string Description
    {
        get
        {
            var path = string.Join(" → ", this.CyclePath);
            var multiplier = this.ArbitrageMultiplier;
            return this.HasArbitrage ? GetString($"套利循环: {path} (套利倍数: {multiplier:F4}x)") : GetString($"无损循环: {path} (倍数: {multiplier:F4}x)");
        }
    }
}
