using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class CircleProjectile
{
    [JsonProperty("启用")]
    public bool Enable { get; set; }

    [JsonProperty("起始角度")]
    public float StartAngle { get; set; }

    [JsonProperty("终止角度")]
    public float EndAngle { get; set; }

    [JsonProperty("X偏移位置")]
    public int X { get; set; }

    [JsonProperty("Y偏移位置")]
    public int Y { get; set; }

    [JsonProperty("跟踪玩家位置")]
    public bool FollowPlayer { get; set; }

    [JsonProperty("弹幕ID")]
    public int ID { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("击退")]
    public float Knockback { get; set; }

    [JsonProperty("射速")]
    public float Speed { get; set; }

    [JsonProperty("持续时间")]
    public int TimeLeft { get; set; } = -1;

    [JsonProperty("AI")]
    public float[] AI { get; set; } = new float[3];

    [JsonProperty("半径")]
    public int Radius { get; set; }

    [JsonProperty("间隔")]
    public int Interval { get; set; }

    [JsonProperty("反向")]
    public bool Reverse { get; set; }

    [JsonProperty("延迟")]
    public int Dealy { get; set; }
}


