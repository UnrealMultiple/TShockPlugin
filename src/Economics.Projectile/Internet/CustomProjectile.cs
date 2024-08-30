using Newtonsoft.Json;

namespace Economics.Projectile;
public class CustomProjectile
{
    [JsonProperty("弹幕ID")]
    public int ID = 0;

    [JsonProperty("弹幕击退动态跟随")]
    public bool KnockBackFollow = false;

    [JsonProperty("弹幕伤害动态跟随")]
    public bool DamageFollow = false;

    [JsonProperty("自动追踪")]
    public bool AutoFollow = false;

    [JsonProperty("弹幕伤害")]
    public float Damage = 0f;

    [JsonProperty("弹幕击退")]
    public float KnockBack = 0f;

    [JsonProperty("弹幕射速")]
    public float Speed = 0f;

    [JsonProperty("持续时间")]
    public int TimeLeft = -1;

    [JsonProperty("限制等级")]
    public List<string> Limit { get; set; } = new();

    [JsonProperty("弹幕AI")]
    public float[] AI { get; set; } = new float[3];
}