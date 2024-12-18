using Newtonsoft.Json;

namespace CustomMonster;

public class IndicatorGroup2
{
    [JsonProperty(PropertyName = "名称")]
    public string IndName = "";

    [JsonProperty(PropertyName = "数量")]
    public int IndStack = 0;

    [JsonProperty(PropertyName = "重定义判断符号")]
    public string IFMark = "";

    [JsonProperty(PropertyName = "指示物注入数量名")]
    public string InjectionStackName = "";

    [JsonProperty(PropertyName = "指示物注入数量系数")]
    public float InjectionStackRatio = 1f;

    public IndicatorGroup2(string name, int num)
    {
        this.IndName = name;
        this.IndStack = num;
    }
}
