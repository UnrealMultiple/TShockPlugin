using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Projectile;

public class ProjectileOption
{
    [JsonProperty("弹幕ID")]
    public int ID { get; set; }

    [JsonProperty("优先级")]
    public int Index { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("击退")]
    public float Knockback { get; set; }

    [JsonProperty("X轴起始位置")]
    public int X { get; set; }

    [JsonProperty("Y轴起始位置")]
    public int Y { get; set; }

    [JsonProperty("射速")]
    public float Speed { get; set; }

    [JsonProperty("起始角度")]
    public int Angle { get; set; }

    [JsonProperty("弹幕循环")]
    public ProjectileCycleOption ProjectileCycle { get; set; } = new();
}
