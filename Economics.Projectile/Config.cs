using Newtonsoft.Json;

namespace Economics.Projectile;

public class Config
{
    [JsonProperty("弹幕触发")]
    public Dictionary<int, ProjectileSpark> ProjectileReplace { get; set; } = new();

    [JsonProperty("物品触发 ")]
    public Dictionary<int, ItemSpark> ItemReplace { get; set; } = new();

}