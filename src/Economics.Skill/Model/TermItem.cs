
using Economics.Core.Model;
using Newtonsoft.Json;

namespace Economics.Skill.Model;

public class TermItem : Item
{
    [JsonProperty("背包物品")]
    public bool Inventory { get; set; } = false;

    [JsonProperty("装备")]
    public bool Armory { get; set; } = false;

    [JsonProperty("饰品")]
    public bool Misc { get; set; } = false;

    [JsonProperty("手持物品")]
    public bool HeldItem { get; set; } = false;

    [JsonProperty("是否消耗")]
    public bool Consume { get; set; } = false;
}