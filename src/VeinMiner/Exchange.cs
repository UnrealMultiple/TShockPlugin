using Newtonsoft.Json;

namespace VeinMiner;

public struct Exchange
{
    [JsonProperty("仅给予物品")]
    public bool OnlyGiveItem;

    [JsonProperty("最小尺寸")]
    public int MinSize;

    [JsonProperty("矿石物块ID")]
    public int Type;

    [JsonProperty("奖励物品")]
    public Dictionary<int, int> Item;
}