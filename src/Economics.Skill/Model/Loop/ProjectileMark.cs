using Newtonsoft.Json;

namespace Economics.Skill.Model.Loop;
public class ProjectileMark : BaseLoop
{
    [JsonProperty("角度")]
    public int Angle { get; set; }

    [JsonProperty("旋转")]
    public float Rotating { get; set; }

    [JsonProperty("X位置")]
    public int X { get; set; }

    [JsonProperty("Y位置")]
    public int Y { get; set; }

    [JsonProperty("X轴速度")]
    public float SpeedX { get; set; } = 0f;

    [JsonProperty("Y轴速度")]
    public float SpeedY { get; set; } = 0f;

    public float[] AI { get; set; } = new float[3];

    [JsonProperty("速度")]
    public float Speed { get; set; }

    [JsonProperty("跟随玩家")]
    public bool FollowPlayer { get; set; }

    [JsonProperty("锁定敌怪为中心")]
    public bool LockNpcCenter { get; set; }

    [JsonProperty("锁定后瞄准")]
    public bool FollowNpc { get; set; }

    [JsonProperty("弹幕锁定敌怪")]
    public bool Lock { get; set; }

    [JsonProperty("锁定范围")]
    public int LockRange { get; set; }

    [JsonProperty("锁定血量最少")]
    public bool LockMinHp { get; set; } = true;
}

