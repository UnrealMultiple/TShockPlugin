using Newtonsoft.Json;

namespace OnlineGiftPackage
{
    [JsonConverter(typeof(GiftConverter))]
    public class Gift
    {
        [JsonProperty("物品名称")]
        public string 物品名称 { get; set; }
        [JsonProperty("物品ID")]
        public int 物品ID { get; set; }
        [JsonProperty("所占概率")]
        public int 所占概率 { get; set; }
        [JsonProperty("物品数量")]
        public int[] 物品数量 { get; set; }

    }
}