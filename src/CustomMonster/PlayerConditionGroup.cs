using Newtonsoft.Json;

namespace CustomMonster;

public class PlayerConditionGroup
{
    [JsonProperty(PropertyName = "范围起")]
    public int StartRange = 0;

    [JsonProperty(PropertyName = "范围内")]
    public int InRange = 0;

    [JsonProperty(PropertyName = "生命值")]
    public int Life = 0;

    [JsonProperty(PropertyName = "符合数")]
    public int SuitNum = 0;

    public PlayerConditionGroup(int rangeB, int rangeE, int num)
    {
        this.StartRange = rangeB;
        this.InRange = rangeE;
        this.SuitNum = num;
    }
}
