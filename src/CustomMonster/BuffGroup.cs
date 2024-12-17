using Newtonsoft.Json;

namespace CustomMonster;

public class BuffGroup
{
    [JsonProperty(PropertyName = "状态ID")]
    public int BuffID = 0;

    [JsonProperty(PropertyName = "状态起始范围")]
    public int BuffStartRange = 0;

    [JsonProperty(PropertyName = "状态结束范围")]
    public int BuffStopRange = 0;

    [JsonProperty(PropertyName = "头顶提示")]
    public string TopTip = "";

    public BuffGroup(int id, int start, int tail, string tip)
    {
        this.BuffID = id;
        this.BuffStartRange = start;
        this.BuffStopRange = tail;
        this.TopTip = tip;
    }
}
