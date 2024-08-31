

using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class NpcBuffOption : BaseRangeOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("BUFF")]
    public BuffOption BuffOptions { get; set; } = new();
}