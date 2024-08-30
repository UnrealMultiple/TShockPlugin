using Newtonsoft.Json;

namespace Economics.Projectile;

public class ProjectileSpark
{
    [JsonProperty("弹幕数据")]
    public List<CustomProjectile> ProjData { get; set; } = new();

    [JsonProperty("使用物品时触发")]
    public bool useItem = true;

    [JsonProperty("来自于召唤物")]
    public bool fromMinion = false;

    [JsonProperty("本身就是召唤物")]
    public bool IsMinion = false;

    [JsonProperty("召唤物攻击间隔")]
    public int CD = 15;

    [JsonProperty("备注")]
    public string Name { get; set; } = string.Empty;
}