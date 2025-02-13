

using Newtonsoft.Json;

namespace Economics.Skill.Model.Loop;
public class ProjectileGenerate : BaseLoop
{
    [JsonProperty("弹幕ID")]
    public int ID { get; set; }

    [JsonProperty("角度")]
    public int Angel { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("击退")]
    public int Knockback { get; set; }

    [JsonProperty("速度")]
    public float Speed { get; set; }

    [JsonProperty("X位置")]
    public int X { get; set; }

    [JsonProperty("Y位置")]
    public int Y { get; set; }

    public float[] AI { get; set; } = new float[3];

    [JsonProperty("弹幕锁定敌怪")]
    public bool Lock { get; set; }

    [JsonProperty("锁定范围")]
    public int LockRange { get; set; }

    [JsonProperty("锁定血量最少")]
    public bool LockMinHp { get; set; } = true;

    [JsonProperty("存在目标攻击")]
    public bool TargetAttack { get; set; } = true;
}
