using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class ClearProjectileOption : BaseRangeOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }
}
