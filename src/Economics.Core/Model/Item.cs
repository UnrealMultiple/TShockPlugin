using Newtonsoft.Json;

namespace Economics.Core.Model;

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
        return $"[i/s{this.Stack}:{this.netID}]";
    }
}