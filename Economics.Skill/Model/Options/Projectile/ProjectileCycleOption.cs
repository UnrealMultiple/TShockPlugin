using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Projectile;

public class ProjectileCycleOption
{
    [JsonProperty("延迟")]
    public int Delay { get; set; }

    [JsonProperty("同时执行")]
    public bool Concurrent { get; set; }

    [JsonProperty("配置")]
    public List<ProjectileCycle> ProjectileCycles { get; set; } = new();
}
