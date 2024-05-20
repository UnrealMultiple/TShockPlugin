using Newtonsoft.Json;

namespace Economics.Projectile.Internet;

public class CustomProjectile
{
    [JsonProperty("弹幕ID")]
    public int Id { get; set; }

    [JsonProperty("伤害")]
    public int Damage { get; set; }

    [JsonProperty("击退")]
    public float Knockback { get; set; }

    [JsonProperty("射速")]
    public float Speed { get; set; }
}
