using EconomicsAPI.Model;
using Newtonsoft.Json;

namespace Economics.Shop.Model;

public class Product
{
    [JsonProperty("商品名称")]
    public string Name { get; set; }

    [JsonProperty("商品价格")]
    public long Cost { get; set; }

    [JsonProperty("等级限制")]
    public List<string> LevelLimit { get; set; } = new();

    [JsonProperty("进度限制")]
    public List<string> ProgressLimit { get; set; } = new();

    [JsonProperty("执行命令")]
    public List<string> Commamds { get; set; } = new();

    [JsonProperty("商品内容")]
    public List<Item> Items { get; set; } = new();

    [JsonProperty("物品条件")]
    public List<ItemTerm> ItemTerm { get; set; } = new();
}
