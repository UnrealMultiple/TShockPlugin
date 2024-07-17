

using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class AIStyleOption
{
    [JsonProperty("样式")]
    public int Style { get; set; } = -1;

    [JsonProperty("发射弹幕")] 
    public int ProjID { get; set; }

    [JsonProperty("伤害")] 
    public int Damage { get; set; }

    [JsonProperty("射速")]
    public int Speed { get; set; }

    [JsonProperty("环绕大小")]
    public int Range { get; set; }

    [JsonProperty("攻击间隔")]
    public int Interval { get; set; } = 10;
}
