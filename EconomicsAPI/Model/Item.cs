using Newtonsoft.Json;

namespace EconomicsAPI.Model;

public class Item
{
    [JsonProperty("物品ID")]
    public int netID { get; set; }

    [JsonProperty("数量")]
    public int Stack { get; set; }

    [JsonProperty("前缀")]
    public int Prefix { get; set; }

    public override string ToString()
    {
        return $"[i/s{Stack}:{netID}]";
    }
}
