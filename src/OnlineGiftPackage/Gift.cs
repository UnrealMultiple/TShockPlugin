using Newtonsoft.Json;


namespace OnlineGiftPackage;

public class Gift
{
    [JsonProperty("物品名称")]
    public string ItemName { get; set; } = "";
    [JsonProperty("物品ID")]
    public int ItamID { get; set; }
    [JsonProperty("所占概率")]
    public int Probability { get; set; }
    [JsonProperty("物品数量")]
    public int[] ItemAmount { get; set; } = new int[2];

}