using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class ExecCommandOption : BaseRangeOption
{
    [JsonProperty("命令")]
    public List<string> Commands { get; set; } = new();
}
