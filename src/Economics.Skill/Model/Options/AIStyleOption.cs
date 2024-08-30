

using Economics.Skill.Enumerates;
using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class AIStyleOption
{
    [JsonProperty("样式")]
    public AIStyleType Style { get; set; } = AIStyleType.None;

    [JsonProperty("发射弹幕")]
    public int ProjID { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("射速")]
    public int Speed { get; set; }

    [JsonProperty("攻击范围")]
    public int AttackRange { get; set; } = 50;

    [JsonProperty("环绕大小")]
    public int Range { get; set; }

    [JsonProperty("攻击间隔")]
    public int Interval { get; set; } = 10;

    [JsonProperty("AI")]
    public float[] AI { get; set; } = new float[3];
}
