using Newtonsoft.Json;

namespace CustomMonster;

public class MonsterGroup
{
    [JsonProperty(PropertyName = "标志")]
    public string Sign = "";

    [JsonProperty(PropertyName = "怪物ID")]
    public int NPCID = 0;

    [JsonProperty(PropertyName = "再匹配")]
    public int[] Rematching = new int[0];

    [JsonProperty(PropertyName = "再匹配例外")]
    public int[] AdditionalMatching = new int[0];

    [JsonProperty(PropertyName = "初始属性玩家系数")]
    public int InitialPlayerCoefficient = 0;

    [JsonProperty(PropertyName = "初始属性强化系数")]
    public float InitialEnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "初始属性对怪物伤害修正")]
    public float InitialDamageCorrectionForMonsters = 1f;

    [JsonProperty(PropertyName = "怪物血量")]
    public int NPCLife = 0;

    [JsonProperty(PropertyName = "玩家系数")]
    public int PlayerCoefficient = 0;

    [JsonProperty(PropertyName = "开服系数")]
    public int ServiceOpenCoefficient = 0;

    [JsonProperty(PropertyName = "杀数系数")]
    public int KillCountCoefficient = 0;

    [JsonProperty(PropertyName = "开服时间型")]
    public int StartServerTimeType = 0;

    [JsonProperty(PropertyName = "覆盖原血量")]
    public bool OverrideOriginalLife = true;

    [JsonProperty(PropertyName = "不低于正常")]
    public bool NotLowerThanNormal = true;

    [JsonProperty(PropertyName = "强化系数")]
    public float EnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "不小于正常")]
    public bool NotLessThanNormal = true;

    [JsonProperty(PropertyName = "玩家强化系数")]
    public float PlayerEnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "开服强化系数")]
    public float ServiceOpenEnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "杀数强化系数")]
    public float KillCountEnhancementCoefficient = 0f;

    [JsonProperty(PropertyName = "覆盖原强化")]
    public bool OverrideOriginalEnhancement = true;

    [JsonProperty(PropertyName = "玩家复活时间")]
    public int RespawnSeconds = -1;

    [JsonProperty(PropertyName = "全局最大刷怪数")]
    public int DefaultMaxSpawns = -1;

    [JsonProperty(PropertyName = "全局刷怪速度")]
    public int DefaultSpawnRate = -1;

    [JsonProperty(PropertyName = "阻止传送器")]
    public int BlockTeleporter = 0;

    [JsonProperty(PropertyName = "出没秒数")]
    public int AppearanceSeconds = 0;

    [JsonProperty(PropertyName = "人秒系数")]
    public int PersonSecondCoefficient = 0;

    [JsonProperty(PropertyName = "开服系秒")]
    public int ServiceOpenSecond = 0;

    [JsonProperty(PropertyName = "杀数系秒")]
    public int KillCountSecond = 0;

    [JsonProperty(PropertyName = "出没率子")]
    public int AppearanceRateNumerator = 1;

    [JsonProperty(PropertyName = "出没率母")]
    public int AppearanceRateDenominator = 1;

    [JsonProperty(PropertyName = "种子条件")]
    public string[] MapSeed = new string[0];

    [JsonProperty(PropertyName = "难度条件")]
    public int[] Difficulty = new int[0];

    [JsonProperty(PropertyName = "杀数条件")]
    public int KilledStack = 0;

    [JsonProperty(PropertyName = "数量条件")]
    public int QuantityCondition = 0;

    [JsonProperty(PropertyName = "人数条件")]
    public int PlayerCount = 0;

    [JsonProperty(PropertyName = "昼夜条件")]
    public int DayAndNight = 0;

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

    [JsonProperty(PropertyName = "怪物条件")]
    public List<MonsterConditionGroup> MonsterCondition = new List<MonsterConditionGroup>();

    [JsonProperty(PropertyName = "杀怪条件")]
    public Dictionary<int, long> KilledNPC = new Dictionary<int, long>();

    [JsonProperty(PropertyName = "智慧机制")]
    public int AIMechanism = -1;

    [JsonProperty(PropertyName = "免疫熔岩")]
    public int ImmuneToLava = 0;

    [JsonProperty(PropertyName = "免疫陷阱")]
    public int ImmuneToTraps = 0;

    [JsonProperty(PropertyName = "能够穿墙")]
    public int CanPassThroughWalls = 0;

    [JsonProperty(PropertyName = "无视重力")]
    public int IgnoreGravity = 0;

    [JsonProperty(PropertyName = "设为老怪")]
    public int SetAsBoss = 0;

    [JsonProperty(PropertyName = "修改防御")]
    public bool ModifyDefense = false;

    [JsonProperty(PropertyName = "怪物防御")]
    public int MonsterDefense = 0;

    [JsonProperty(PropertyName = "怪物无敌")]
    public int MonsterGodMode = 0;

    [JsonProperty(PropertyName = "自定缀称")]
    public string CustomPrefix = "";

    [JsonProperty(PropertyName = "出场喊话")]
    public string EntryBroadcast = "";

    [JsonProperty(PropertyName = "出场喊话无头")]
    public bool EntryBroadcastHeadless = false;

    [JsonProperty(PropertyName = "死亡喊话")]
    public string DeathBroadcast = "";

    [JsonProperty(PropertyName = "死亡喊话无头")]
    public bool DeathBroadcastHeadless = false;

    [JsonProperty(PropertyName = "不宣读信息")]
    public bool DoNotAnnounceInfo = false;

    [JsonProperty(PropertyName = "状态范围")]
    public int BuffRange = 0;

    [JsonProperty(PropertyName = "周围状态")]
    public List<BuffGroup> SurroundingBuff = new List<BuffGroup>();

    [JsonProperty(PropertyName = "死状范围")]
    public int DeathBuffRange = 0;

    [JsonProperty(PropertyName = "死亡状态")]
    public Dictionary<int, int> DeathBuff = new Dictionary<int, int>();

    [JsonProperty(PropertyName = "出场弹幕")]
    public List<ProjGroup> EntryProjectile = new List<ProjGroup>();

    [JsonProperty(PropertyName = "死亡弹幕")]
    public List<ProjGroup> DeathProjectile = new List<ProjGroup>();

    [JsonProperty(PropertyName = "出场放音")]
    public List<AudioGroup> EntryAudio = new List<AudioGroup>();

    [JsonProperty(PropertyName = "死亡放音")]
    public List<AudioGroup> DeathAudio = new List<AudioGroup>();

    [JsonProperty(PropertyName = "出场伤怪")]
    public List<MonsterStrikeGroup> EntryMonsterStrike = new List<MonsterStrikeGroup>();

    [JsonProperty(PropertyName = "死亡伤怪")]
    public List<MonsterStrikeGroup> DeathMonsterStrike = new List<MonsterStrikeGroup>();

    [JsonProperty(PropertyName = "出场命令")]
    public List<string> EntryCommands = new List<string>();

    [JsonProperty(PropertyName = "死亡命令")]
    public List<string> DeathCommands = new List<string>();

    [JsonProperty(PropertyName = "出场怪物指示物修改")]
    public List<IndicatorModifyGroup> EntryMonsterIndicatorModifications = new List<IndicatorModifyGroup>();

    [JsonProperty(PropertyName = "死亡怪物指示物修改")]
    public List<IndicatorModifyGroup> DeathMonsterIndicatorModifications = new List<IndicatorModifyGroup>();

    [JsonProperty(PropertyName = "血事件限")]
    public int LifeEventLimit = 1;

    [JsonProperty(PropertyName = "血量事件")]
    public List<RatioGroup> LifeEvent = new List<RatioGroup>();

    [JsonProperty(PropertyName = "时事件限")]
    public int TimeEventLimit = 3;

    [JsonProperty(PropertyName = "时间事件")]
    public List<TimeGroup> TimeEvent = new List<TimeGroup>();

    [JsonProperty(PropertyName = "事件权重")]
    public int EventWeight = 0;

    [JsonProperty(PropertyName = "随从怪物")]
    public Dictionary<int, int> FollowerMonsters = new Dictionary<int, int>();

    [JsonProperty(PropertyName = "遗言怪物")]
    public Dictionary<int, int> DyingWordsMonsters = new Dictionary<int, int>();

    [JsonProperty(PropertyName = "掉落组限")]
    public int DropGroupLimit = 1;

    [JsonProperty(PropertyName = "额外掉落")]
    public List<ItemDropGroup> ExtraItemDrops = new List<ItemDropGroup>();

    [JsonProperty(PropertyName = "备注")]
    public string Notes = "";
}
