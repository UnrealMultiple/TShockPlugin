using Newtonsoft.Json;

namespace Economics.NPC;

public class NpcOption
{
    [JsonProperty("怪物ID")]
    public int ID { get; set; }

    [JsonProperty("怪物名称")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("奖励货币")]
    public long ExtraReward { get; set; }

    [JsonProperty("按输出瓜分")]
    public bool DynamicPartition { get; set; } = true;

}