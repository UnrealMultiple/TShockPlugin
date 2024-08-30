using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class PlayerTpOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("面向修正")]
    public bool Incline { get; set; }

    [JsonProperty("X轴位置")]
    public int X { get; set; }

    [JsonProperty("Y轴位置")]
    public int Y { get; set; }
}