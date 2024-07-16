using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class LockNpcOption : BaseRangeOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("弹幕锁定敌怪")]
    public bool Lock { get; set; }

    [JsonProperty("以锁定敌怪为中心")]
    public bool LockCenter { get; set; }

    [JsonProperty("锁定血量最少")]
    public bool LockMinHp { get; set; } = true;
}
