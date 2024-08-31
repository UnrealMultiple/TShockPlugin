using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Range;

public class BuffOption : BaseRangeOption
{
    [JsonProperty("Buff列表")]
    public List<Buff> Buffs { get; set; } = new();
}