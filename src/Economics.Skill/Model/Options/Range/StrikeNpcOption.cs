using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class StrikeNpcOption : BaseRangeOption
{
    [JsonProperty("伤害")]
    public int Damage { get; set; }
}