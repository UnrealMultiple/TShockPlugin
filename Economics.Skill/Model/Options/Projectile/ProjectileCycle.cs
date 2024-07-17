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

    [JsonProperty("角度递增")] 
    public int GrowAngle { get; set; }

    [JsonProperty("圆面半径")]
    public int Radius { get; set; }

    [JsonProperty("反向发射")]
    public bool Reverse { get; set; }

    [JsonProperty("延迟")]
    public int Dealy { get; set; }

    [JsonProperty("跟随玩家位置")]
    public bool FollowPlayer { get; set; }

    [JsonProperty("根据角度计算新的点")]
    public bool NewPos { get; set; }
}
