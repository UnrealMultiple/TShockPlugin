using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class PullNpcOption : BaseRangeOption
{
    [JsonProperty("X轴调整")]
    public int X { get; set; }

    [JsonProperty("Y轴调整")]
    public int Y { get; set; }
}
