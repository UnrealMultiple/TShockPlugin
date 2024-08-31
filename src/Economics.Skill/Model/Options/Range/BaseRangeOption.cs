using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class BaseRangeOption
{
    [JsonProperty("范围")]
    public int Range { get; set; }
}