using Newtonsoft.Json;

namespace CustomMonster;

public class MonsterStrikeGroup
{
    [JsonProperty(PropertyName = "怪物ID")]
    public int NPCID = 0;

    [JsonProperty(PropertyName = "查标志")]
    public string CheckSign = "";

    [JsonProperty(PropertyName = "指示物")]
    public List<IndicatorGroup2> Indicator = new List<IndicatorGroup2>();

    [JsonProperty(PropertyName = "范围内")]
    public int Range = 0;

    [JsonProperty(PropertyName = "造成伤害")]
    public int CauseDamage = 0;

    [JsonProperty(PropertyName = "指示物数量注入造成伤害名")]
    public string NumberOfIndicatorsInjectedCausingDamageName = "";

    [JsonProperty(PropertyName = "指示物数量注入造成伤害系数")]
    public float DamageCoefficientCausedByInjectionOfIndicatorQuantity = 1f;

    [JsonProperty(PropertyName = "直接伤害")]
    public bool DirectDamage = false;

    [JsonProperty(PropertyName = "直接清除")]
    public bool DirectClear = false;

    public MonsterStrikeGroup(int id, int range, int num)
    {
        this.NPCID = id;
        this.Range = range;
        this.CauseDamage = num;
        this.Indicator = new List<IndicatorGroup2>();
        this.CheckSign = "";
    }
}

