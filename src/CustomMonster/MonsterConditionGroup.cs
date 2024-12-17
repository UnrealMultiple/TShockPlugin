using Newtonsoft.Json;

namespace CustomMonster;

public class MonsterConditionGroup
{
    [JsonProperty(PropertyName = "怪物ID")]
    public int NPCID = 0;

    [JsonProperty(PropertyName = "查标志")]
    public string CheckSign = "";

    [JsonProperty(PropertyName = "指示物")]
    public List<IndicatorGroup2> Indicator = new List<IndicatorGroup2>();

    [JsonProperty(PropertyName = "范围内")]
    public int Range = 0;

    [JsonProperty(PropertyName = "血量比")]
    public int LifeRate = 0;

    [JsonProperty(PropertyName = "符合数")]
    public int SuitNum = 0;

    public MonsterConditionGroup(int id, int range, int num)
    {
        this.NPCID = id;
        this.Range = range;
        this.SuitNum = num;
        this.Indicator = new List<IndicatorGroup2>();
        this.CheckSign = "";
    }
}
