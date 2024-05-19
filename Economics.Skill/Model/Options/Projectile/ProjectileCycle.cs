using Newtonsoft.Json;

namespace Economics.Skill.Model.Options.Projectile;

public class ProjectileCycle
{
    [JsonProperty("次数")]
    public int Count { get; set; }

    [JsonProperty("X递增")]
    public int GrowX { get; set; }

    [JsonProperty("Y递增")]
    public int GrowY { get; set; }

    [JsonProperty("位置平移")]
    public int Translate { get; set; }

    [JsonProperty("角度递增")]
    public int GrowAngle { get; set; }

    [JsonProperty("延迟")]
    public int Dealy { get; set; }

    [JsonProperty("跟随玩家位置")]
    public bool FollowPlayer { get; set; }
}
