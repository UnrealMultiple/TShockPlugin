using Newtonsoft.Json;

namespace Economics.Projectile;
public class ProjectileReplace
{
    [JsonProperty("弹幕ID")]
    public int ID = 0;

    [JsonProperty("弹幕击退动态跟随")]
    public bool KnockBackFollow = false;

    [JsonProperty("弹幕伤害动态跟随")]
    public bool DamageFollow = false;

    [JsonProperty("弹幕伤害")]
    public float Damage = 0f;

    [JsonProperty("弹幕击退")]
    public float KnockBack = 0f;

    [JsonProperty("弹幕射速")]
    public float speed = 0f;

    [JsonProperty("限制等级")]
    public List<string> Limit { get; set; } = new();
}
