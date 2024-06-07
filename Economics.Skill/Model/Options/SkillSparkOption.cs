using Economics.Skill.Converter;
using Economics.Skill.Enumerates;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Economics.Skill.Model.Options;

public class SkillSparkOption
{
    [JsonProperty("触发模式")]
    [JsonConverter(typeof(SkillSparkConverter))]
    public List<SkillSparkType> SparkMethod { get; set; } = new();

    [JsonProperty("冷却")]
    public int CD { get; set; }

    [JsonProperty("血量")]
    public int HP { get; set; }

    [JsonProperty("蓝量")]
    public int MP { get; set; }

    [JsonProperty("物品条件")]
    public List<TermItem> TermItem { get; set; } = new();
}
