using Newtonsoft.Json;

namespace Economics.Skill.Model.Loop;
public class LoopEvent
{
    [JsonProperty("循环间隔")]
    public int Interval { get; set; } = 10;

    [JsonProperty("循环次数")]
    public int LoopCount { get; set; } = 1;

    [JsonProperty("弹幕循环")]
    public List<ProjectileLoop> ProjectileLoops { get; set; } = new();

    [JsonProperty("范围循环")]
    public List<RegionLoop> RegionLoops { get; set; } = new();

    [JsonProperty("玩家循环")]
    public List<PlayerLoop> PlayerLoops { get; set; } = new();

}