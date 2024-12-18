using Newtonsoft.Json;

namespace CustomMonster;

public class ItemDropGroup
{
    [JsonProperty(PropertyName = "掉落率子")]
    public int Numerator = 1;

    [JsonProperty(PropertyName = "掉落率母")]
    public int Denominator = 1;

    [JsonProperty(PropertyName = "种子条件")]
    public string[] MapSeed = new string[0];

    [JsonProperty(PropertyName = "难度条件")]
    public int[] Difficulty = new int[0];

    [JsonProperty(PropertyName = "杀数条件")]
    public int KilledStack = 0;

    [JsonProperty(PropertyName = "人数条件")]
    public int PlayerCount = 0;

    [JsonProperty(PropertyName = "杀死条件")]
    public int KilledCount = 0;

    [JsonProperty(PropertyName = "被击条件")]
    public int Strike = 0;

    [JsonProperty(PropertyName = "昼夜条件")]
    public int DayAndNight = 0;

    [JsonProperty(PropertyName = "耗时条件")]
    public int TimeConsume = 0;

    [JsonProperty(PropertyName = "肉山条件")]
    public int HardMode = 0;

    [JsonProperty(PropertyName = "巨人条件")]
    public int DownedGolemBoss = 0;

    [JsonProperty(PropertyName = "降雨条件")]
    public int Rain = 0;

    [JsonProperty(PropertyName = "血月条件")]
    public int BloodMoon = 0;

    [JsonProperty(PropertyName = "日食条件")]
    public int Eclipse = 0;

    [JsonProperty(PropertyName = "月总条件")]
    public int DownedMoonlord = 0;

    [JsonProperty(PropertyName = "开服条件")]
    public int StastServer = 0;

    [JsonProperty(PropertyName = "AI条件")]
    public Dictionary<string, float> AI = new Dictionary<string, float>();

    [JsonProperty(PropertyName = "跳出掉落")]
    public bool DontDrop = false;

    [JsonProperty(PropertyName = "怪物条件")]
    public List<MonsterConditionGroup> MonsterCondition = new List<MonsterConditionGroup>();

    [JsonProperty(PropertyName = "杀怪条件")]
    public Dictionary<int, long> KilledNPC = new Dictionary<int, long>();

    [JsonProperty(PropertyName = "指示物条件")]
    public List<IndicatorGroup2> Indicator = new List<IndicatorGroup2>();

    [JsonProperty(PropertyName = "掉落物品")]
    public List<ItemGroup> ItemDrop = new List<ItemGroup>();

    [JsonProperty(PropertyName = "喊话")]
    public string Broadcast = "";

    [JsonProperty(PropertyName = "喊话无头")]
    public bool BroadcastHeadless = false;

    [JsonProperty(PropertyName = "备注")]
    public string Notes = "";

    public ItemDropGroup(int id, int stack)
    {
        this.ItemDrop.Add(new ItemGroup(id, stack));
    }
}
