using Newtonsoft.Json;

namespace CustomMonster;

public class IndicatorGroup
{
    [JsonProperty(PropertyName = "名称")]
    public string IndName = "";

    [JsonProperty(PropertyName = "数量")]
    public int IndStack = 0;

    [JsonProperty(PropertyName = "清除")]
    public bool Clear = false;

    [JsonProperty(PropertyName = "随机小")]
    public int RandomSmall = 0;

    [JsonProperty(PropertyName = "随机大")]
    public int RandomBig = 0;

    [JsonProperty(PropertyName = "指示物注入数量名")]
    public string InjectionStackName = "";

    [JsonProperty(PropertyName = "指示物注入数量系数")]
    public float InjectionStackRatio = 1f;

    [JsonProperty(PropertyName = "指示物注入数量运算符")]
    public string InjectionStackOperator = "";

    public IndicatorGroup(string name, int num)
    {
        this.IndName = name;
        this.IndStack = num;
    }
}
