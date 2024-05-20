using Newtonsoft.Json;

namespace Economics.Projectile;

public class ProjConfig
{
    [JsonProperty("弹幕触发")]
    public Dictionary<int, ProjectileData> ProjectileReplace { get; set; } = new();

    [JsonProperty("物品触发 ")]
    public Dictionary<int, ItemData> ItemReplace { get; set; } = new();

}