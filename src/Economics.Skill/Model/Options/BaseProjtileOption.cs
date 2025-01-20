using Economics.Skill.Model.Options.Range;
using Newtonsoft.Json;

namespace Economics.Skill.Model.Options;

public class BaseProjtileOption
{
    [JsonProperty("弹幕ID")]
    public int ID { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("击退")]
    public float Knockback { get; set; }

    [JsonProperty("起始角度")]
    public int StartAngle { get; set; }

    [JsonProperty("X轴起始位置")]
    public int X { get; set; }

    [JsonProperty("Y轴起始位置")]
    public int Y { get; set; }

    [JsonProperty("X轴速度")]
    public float SpeedX { get; set; } = 0f;

    [JsonProperty("Y轴速度")]
    public float SpeedY { get; set; } = 0f;

    [JsonProperty("自动方向")]
    public bool AutoDirection { get; set; } = true;

    [JsonProperty("持续时间")]
    public int TimeLeft { get; set; } = -1;

    [JsonProperty("动态伤害")]
    public bool DynamicDamage { get; set; }

    [JsonProperty("AI")]
    public float[] AI { get; set; } = new float[3];

    [JsonProperty("AI样式")]
    public AIStyleOption AISytle { get; set; } = new();

    [JsonProperty("射速")]
    public float Speed { get; set; }

    [JsonProperty("锁定怪物配置")]
    public LockNpcOption LockNpcOption { get; set; } = new();

    [JsonProperty("延迟")]
    public int Dealy { get; set; }
}