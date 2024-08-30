using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Projectile;

public class ProjectileOption : BaseProjtileOption
{

    [JsonProperty("弹幕循环", Order = 99)]
    public ProjectileCycleOption ProjectileCycle { get; set; } = new();
}
