using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class PlayerGodOption
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("时长")]
    public int Time { get; set; }
}