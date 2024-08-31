using EconomicsAPI.Model;
using Newtonsoft.Json;

namespace Economics.Deal;

public class DealContext
{
    [JsonProperty("发布者")]
    public string Publisher { get; set; }

    [JsonProperty("价格")]
    public long Cost { get; set; }

    [JsonProperty("物品")]
    public Item Item { get; set; }
}