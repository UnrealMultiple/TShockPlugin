
using Newtonsoft.Json;

namespace CustomMonster;

public class IndicatorModifyGroup
{
    [JsonProperty(PropertyName = "怪物ID")]
    public int NPCID = 0;

    [JsonProperty(PropertyName = "查标志")]
    public string CheckSign = "";

    [JsonProperty(PropertyName = "指示物条件")]
    public List<IndicatorGroup2> Indicator = new List<IndicatorGroup2>();

    [JsonProperty(PropertyName = "范围内")]
    public int Range = 0;

    [JsonProperty(PropertyName = "指示物修改")]
    public List<IndicatorGroup> IndicatorModify = new List<IndicatorGroup>();

    public IndicatorModifyGroup(int id, int range, int num)
    {
        this.NPCID = id;
        this.Range = range;
        this.Indicator = new List<IndicatorGroup2>();
        this.IndicatorModify = new List<IndicatorGroup>();
        this.CheckSign = "";
    }
}
