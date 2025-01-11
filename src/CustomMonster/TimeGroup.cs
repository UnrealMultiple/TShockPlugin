using Newtonsoft.Json;

namespace CustomMonster;

public class TimeGroup : ICloneable
{
    [JsonProperty(PropertyName = "消耗时间")]
    public float ConsumeTime = 1f;

    [JsonProperty(PropertyName = "循环执行")]
    public bool LoopExecution = true;

    [JsonProperty(PropertyName = "延迟秒数")]
    public float DelaySeconds = 0f;

    [JsonProperty(PropertyName = "可触发次")]
    public int TriggerTimes = -1;

    [JsonProperty(PropertyName = "触发率子")]
    public int TriggerNumerator = 1;

    [JsonProperty(PropertyName = "触发率母")]
    public int TriggerDenominator = 1;

    [JsonProperty(PropertyName = "种子条件")]
    public string[] MapSeed = new string[0];

    [JsonProperty(PropertyName = "难度条件")]
    public int[] Difficulty = new int[0];

    [JsonProperty(PropertyName = "杀数条件")]
    public int KilledStack = 0;

    [JsonProperty(PropertyName = "人数条件")]
    public int PlayerCount = 0;

    [JsonProperty(PropertyName = "昼夜条件")]
    public int DayAndNight = 0;

    [JsonProperty(PropertyName = "血比条件")]
    public int HealthRatioCondition = 0;

    [JsonProperty(PropertyName = "血量条件")]
    public int HealthCondition = 0;

    [JsonProperty(PropertyName = "被击条件")]
    public int HitCondition = 0;

    [JsonProperty(PropertyName = "杀死条件")]
    public int KillCondition = 0;

    [JsonProperty(PropertyName = "怪数条件")]
    public int MonsterCountCondition = 0;

    [JsonProperty(PropertyName = "耗时条件")]
    public int TimeSpentCondition = 0;

    [JsonProperty(PropertyName = "ID条件")]
    public int IDCondition = 0;

    [JsonProperty(PropertyName = "AI条件")]
    public Dictionary<string, float> AiConditions = new Dictionary<string, float>();

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

    [JsonProperty(PropertyName = "X轴条件")]
    public int XAxisCondition = 0;

    [JsonProperty(PropertyName = "Y轴条件")]
    public int YAxisCondition = 0;

    [JsonProperty(PropertyName = "面向条件")]
    public int FacingDirectionCondition = 0;

    [JsonProperty(PropertyName = "跳出事件")]
    public bool JumpOutEvent = false;

    [JsonProperty(PropertyName = "怪物条件")]
    public List<MonsterConditionGroup> MonsterCondition = new List<MonsterConditionGroup>();

    [JsonProperty(PropertyName = "杀怪条件")]
    public Dictionary<int, long> KilledNPC = new Dictionary<int, long>();

    [JsonProperty(PropertyName = "玩家条件")]
    public List<PlayerConditionGroup> PlayerCondition = new List<PlayerConditionGroup>();

    [JsonProperty(PropertyName = "弹幕条件")]
    public List<ProjectileConditionGroup> ProjectileCondition = new List<ProjectileConditionGroup>();

    [JsonProperty(PropertyName = "指示物条件")]
    public List<IndicatorGroup2> IndicatorConditions = new List<IndicatorGroup2>();

    [JsonProperty(PropertyName = "指示物修改")]
    public List<IndicatorGroup> IndicatorModifications = new List<IndicatorGroup>();

    [JsonProperty(PropertyName = "怪物指示物修改")]
    public List<IndicatorModifyGroup> MonsterIndicatorModifications = new List<IndicatorModifyGroup>();

    [JsonProperty(PropertyName = "直接撤退")]
    public bool DirectRetreat = false;

    [JsonProperty(PropertyName = "玩家复活时间")]
    public int PlayerRespawnTime = -2;

    [JsonProperty(PropertyName = "全局最大刷怪数")]
    public int DefaultMaxSpawns = -2;

    [JsonProperty(PropertyName = "全局刷怪速度")]
    public int DefaultSpawnRate = -2;

    [JsonProperty(PropertyName = "阻止传送器")]
    public int BlockTeleporter = 0;

    [JsonProperty(PropertyName = "切换智慧")]
    public int SwitchAi = -1;

    [JsonProperty(PropertyName = "怪物变形")]
    public int TransformMonster = -1;

    [JsonProperty(PropertyName = "能够穿墙")]
    public int CanPassThroughWalls = 0;

    [JsonProperty(PropertyName = "无视重力")]
    public int IgnoreGravity = 0;

    [JsonProperty(PropertyName = "修改防御")]
    public bool ModifyDefense = false;

    [JsonProperty(PropertyName = "怪物防御")]
    public int MonsterDefense = 0;

    [JsonProperty(PropertyName = "AI赋值")]
    public Dictionary<int, float> AiValues = new Dictionary<int, float>();

    [JsonProperty(PropertyName = "指示物注入AI赋值")]
    public Dictionary<int, string> IndicatorInjectAiValues = new Dictionary<int, string>();

    [JsonProperty(PropertyName = "恢复血量")]
    public int HealHealth = 0;

    [JsonProperty(PropertyName = "比例回血")]
    public int PercentageHeal = 0;

    [JsonProperty(PropertyName = "怪物无敌")]
    public int MonsterGodMode = 0;

    [JsonProperty(PropertyName = "拉取起始")]
    public int PullStart = 0;

    [JsonProperty(PropertyName = "拉取范围")]
    public int PullRange = 0;

    [JsonProperty(PropertyName = "拉取止点")]
    public int PullEnd = 0;

    [JsonProperty(PropertyName = "拉取点X轴偏移")]
    public float PullPointXOffset = 0f;

    [JsonProperty(PropertyName = "拉取点Y轴偏移")]
    public float PullPointYOffset = 0f;

    [JsonProperty(PropertyName = "指示物数量注入拉取点X轴偏移名")]
    public string IndicatorCountInjectPullPointXName = "";

    [JsonProperty(PropertyName = "指示物数量注入拉取点X轴偏移系数")]
    public float IndicatorCountInjectPullPointXFactor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入拉取点Y轴偏移名")]
    public string IndicatorCountInjectPullPointYName = "";

    [JsonProperty(PropertyName = "指示物数量注入拉取点Y轴偏移系数")]
    public float IndicatorCountInjectPullPointYFactor = 1f;

    [JsonProperty(PropertyName = "初始拉取点坐标为零")]
    public bool InitialPullPointZero = false;

    [JsonProperty(PropertyName = "击退范围")]
    public int KnockbackRange = 0;

    [JsonProperty(PropertyName = "击退力度")]
    public int KnockbackPower = 0;

    [JsonProperty(PropertyName = "杀伤范围")]
    public int DamageRange = 0;

    [JsonProperty(PropertyName = "杀伤伤害")]
    public int DamageAmount = 0;

    [JsonProperty(PropertyName = "清弹起始范围")]
    public int ClearProjectileStartRange = 0;

    [JsonProperty(PropertyName = "清弹结束范围")]
    public int ClearProjectileEndRange = 0;

    [JsonProperty(PropertyName = "反射范围")]
    public int ReflectionRange = 0;

    [JsonProperty(PropertyName = "释放弹幕")]
    public List<ProjGroup> ProjectileGroups = new List<ProjGroup>();

    [JsonProperty(PropertyName = "更新弹幕")]
    public List<ProjUpdateGroup> ProjUpdate = new List<ProjUpdateGroup>();

    [JsonProperty(PropertyName = "状态范围")]
    public int BuffRange = 0;

    [JsonProperty(PropertyName = "周围状态")]
    public List<BuffGroup> SurroundingBuff = new List<BuffGroup>();

    [JsonProperty(PropertyName = "杀伤怪物")]
    public List<MonsterStrikeGroup> StruckMonsters = new List<MonsterStrikeGroup>();

    [JsonProperty(PropertyName = "召唤怪物")]
    public Dictionary<int, int> SummonedMonsters = new Dictionary<int, int>();

    [JsonProperty(PropertyName = "播放声音")]
    public List<AudioGroup> PlaySounds = new List<AudioGroup>();

    [JsonProperty(PropertyName = "释放命令")]
    public List<string> Commands = new List<string>();

    [JsonProperty(PropertyName = "喊话")]
    public string Broadcast = "";

    [JsonProperty(PropertyName = "喊话无头")]
    public bool BroadcastHeadless = false;

    public TimeGroup(int p, Dictionary<int, int> summon, string shout, int heal, int num, List<PlayerConditionGroup> playR, bool R, int killp, List<ProjGroup> proj)
    {
        this.ConsumeTime = p;
        this.LoopExecution = R;
        this.SummonedMonsters = summon;
        this.HealHealth = heal;
        this.Broadcast = shout;
        this.TriggerTimes = num;
        this.PlayerCondition = playR;
        this.KillCondition = killp;
        this.ProjectileGroups = proj;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
