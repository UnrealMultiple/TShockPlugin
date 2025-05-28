using Economics.Core.ConfigFiles;
using Economics.Core.Model;
using Newtonsoft.Json;

namespace Economics.Deal;

public class DealContext
{
    [JsonProperty("发布者")]
    public string Publisher { get; set; } = string.Empty;

    [JsonProperty("货币")]
    public RedemptionRelationshipsOption RedemptionRelationships { get; set; } = new();

    [JsonProperty("物品")]
    public Item Item { get; set; } = new();
}