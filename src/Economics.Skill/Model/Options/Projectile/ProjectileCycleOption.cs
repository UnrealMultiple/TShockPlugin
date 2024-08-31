using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Projectile;

public class ProjectileCycleOption
{
    [JsonProperty("配置")]
    public List<ProjectileCycle> ProjectileCycles { get; set; } = new();
}