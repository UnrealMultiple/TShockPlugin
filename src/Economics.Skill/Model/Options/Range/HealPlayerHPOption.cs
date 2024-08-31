using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class HealPlayerHPOption : BaseRangeOption
{
    [JsonProperty("血量")]
    public int HP { get; set; }

    [JsonProperty("魔力")]
    public int MP { get; set; }
}