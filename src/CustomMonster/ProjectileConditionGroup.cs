using Newtonsoft.Json;

namespace CustomMonster;

public class ProjectileConditionGroup
{
    [JsonProperty(PropertyName = "弹幕ID")]
    public int ProjectileID = 0;

    [JsonProperty(PropertyName = "查标志")]
    public string CheckSign = "";

    [JsonProperty(PropertyName = "范围内")]
    public int Range = 0;

    [JsonProperty(PropertyName = "符合数")]
    public int SuitNum = 0;

    [JsonProperty(PropertyName = "全局弹幕")]
    public bool FullProjectile = false;

    public ProjectileConditionGroup(int id, int range, int num)
    {
        this.ProjectileID = id;
        this.Range = range;
        this.SuitNum = num;
        this.CheckSign = "";
    }
}
