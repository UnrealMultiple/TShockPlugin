using Newtonsoft.Json;

namespace CustomMonster;

public class ProjGroup
{
    [JsonProperty(PropertyName = "弹幕ID")]
    public int ProjectileID = 0;

    [JsonProperty(PropertyName = "标志")]
    public string Sign = "";

    [JsonProperty(PropertyName = "X轴偏移")]
    public float XOffset = 0f;

    [JsonProperty(PropertyName = "Y轴偏移")]
    public float YOffset = 0f;

    [JsonProperty(PropertyName = "X轴速度")]
    public float XSpeed = 0f;

    [JsonProperty(PropertyName = "Y轴速度")]
    public float YSpeed = 0f;

    [JsonProperty(PropertyName = "角度偏移")]
    public float AngleOffset = 0f;

    [JsonProperty(PropertyName = "弹幕伤害")]
    public int Damage = 0;

    [JsonProperty(PropertyName = "弹幕击退")]
    public int Knockback = 0;

    [JsonProperty(PropertyName = "指示物数量注入X轴偏移名")]
    public string IndicatorCountInjectXOffsetName = "";

    [JsonProperty(PropertyName = "指示物数量注入X轴偏移系数")]
    public float IndicatorCountInjectXOffsetFactor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入Y轴偏移名")]
    public string IndicatorCountInjectYOffsetName = "";

    [JsonProperty(PropertyName = "指示物数量注入Y轴偏移系数")]
    public float IndicatorCountInjectYOffsetFactor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入X轴速度名")]
    public string IndicatorCountInjectXSpeedName = "";

    [JsonProperty(PropertyName = "指示物数量注入X轴速度系数")]
    public float IndicatorCountInjectXSpeedFactor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入Y轴速度名")]
    public string IndicatorCountInjectYSpeedName = "";

    [JsonProperty(PropertyName = "指示物数量注入Y轴速度系数")]
    public float IndicatorCountInjectYSpeedFactor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入角度名")]
    public string IndicatorCountInjectAngleName = "";

    [JsonProperty(PropertyName = "指示物数量注入角度系数")]
    public float IndicatorCountInjectAngleFactor = 1f;

    [JsonProperty(PropertyName = "初始坐标为零")]
    public bool StartPositionZero = false;

    [JsonProperty(PropertyName = "弹幕Ai0")]
    public float Ai0 = 0f;

    [JsonProperty(PropertyName = "速度注入AI0")]
    public bool SpeedInjectsIntoAi0 = false;

    [JsonProperty(PropertyName = "速度注入AI0后X轴速度")]
    public float XSpeedAfterAi0Injection = 0f;

    [JsonProperty(PropertyName = "速度注入AI0后Y轴速度")]
    public float YSpeedAfterAi0Injection = 0f;

    [JsonProperty(PropertyName = "弹幕Ai1")]
    public float Ai1 = 0f;

    [JsonProperty(PropertyName = "弹幕Ai2")]
    public float Ai2 = 0f;

    [JsonProperty(PropertyName = "指示物数量注入Ai0名")]
    public string IndicatorCountInjectAi0Name = "";

    [JsonProperty(PropertyName = "指示物数量注入Ai0系数")]
    public float IndicatorCountInjectAi0Factor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入Ai1名")]
    public string IndicatorCountInjectAi1Name = "";

    [JsonProperty(PropertyName = "指示物数量注入Ai1系数")]
    public float IndicatorCountInjectAi1Factor = 1f;

    [JsonProperty(PropertyName = "指示物数量注入Ai2名")]
    public string IndicatorCountInjectAi2Name = "";

    [JsonProperty(PropertyName = "指示物数量注入Ai2系数")]
    public float IndicatorCountInjectAi2Factor = 1f;

    [JsonProperty(PropertyName = "怪面向X偏移修正")]
    public float MonsterFacingXOffsetCorrection = 0f;

    [JsonProperty(PropertyName = "怪面向Y偏移修正")]
    public float MonsterFacingYOffsetCorrection = 0f;

    [JsonProperty(PropertyName = "怪面向X速度修正")]
    public float MonsterFacingXSpeedCorrection = 0f;

    [JsonProperty(PropertyName = "怪面向Y速度修正")]
    public float MonsterFacingYSpeedCorrection = 0f;

    [JsonProperty(PropertyName = "始弹点怪物传送")]
    public bool TeleportMonsterFromSpawnPoint = false;

    [JsonProperty(PropertyName = "始弹点怪物传送类型")]
    public int TeleportTypeFromSpawnPoint = 0;

    [JsonProperty(PropertyName = "始弹点怪物传送信息")]
    public int TeleportInfoFromSpawnPoint = 0;

    [JsonProperty(PropertyName = "不射原始")]
    public bool DoNotShootOriginal = false;

    [JsonProperty(PropertyName = "射出始弹X轴注入指示物名")]
    public string InjectIndicatorNameForInitialProjectileX = "";

    [JsonProperty(PropertyName = "射出始弹Y轴注入指示物名")]
    public string InjectIndicatorNameForInitialProjectileY = "";

    [JsonProperty(PropertyName = "锁定玩家序号注入指示物名")]
    public string InjectIndicatorNameForLockingPlayerIndex = "";

    [JsonProperty(PropertyName = "差度位始角")]
    public int DifferentialStartAngle = 0;

    [JsonProperty(PropertyName = "差度位射数")]
    public int DifferentialShots = 0;

    [JsonProperty(PropertyName = "差度位射角")]
    public int DifferentialAngle = 0;

    [JsonProperty(PropertyName = "差度位半径")]
    public int DifferentialRadius = 0;

    [JsonProperty(PropertyName = "不射差度位")]
    public bool DoNotShootDifferential = false;

    [JsonProperty(PropertyName = "指示物数量注入差度位始角名")]
    public string InjectIndicatorNameForDifferentialStartAngle = "";

    [JsonProperty(PropertyName = "指示物数量注入差度位始角系数")]
    public float InjectIndicatorFactorForDifferentialStartAngle = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差度位射数名")]
    public string InjectIndicatorNameForDifferentialShots = "";

    [JsonProperty(PropertyName = "指示物数量注入差度位射数系数")]
    public float InjectIndicatorFactorForDifferentialShots = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差度位射角名")]
    public string InjectIndicatorNameForDifferentialAngle = "";

    [JsonProperty(PropertyName = "指示物数量注入差度位射角系数")]
    public float InjectIndicatorFactorForDifferentialAngle = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差度位半径名")]
    public string InjectIndicatorNameForDifferentialRadius = "";

    [JsonProperty(PropertyName = "指示物数量注入差度位半径系数")]
    public float InjectIndicatorFactorForDifferentialRadius = 1f;

    [JsonProperty(PropertyName = "差度射数")]
    public int DifferentialShotCount = 0;

    [JsonProperty(PropertyName = "差度射角")]
    public float DifferentialShotAngle = 0f;

    [JsonProperty(PropertyName = "指示物数量注入差度射数名")]
    public string InjectIndicatorNameForDifferentialShotCount = "";

    [JsonProperty(PropertyName = "指示物数量注入差度射数系数")]
    public float InjectIndicatorFactorForDifferentialShotCount = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差度射角名")]
    public string InjectIndicatorNameForDifferentialShotAngle = "";

    [JsonProperty(PropertyName = "指示物数量注入差度射角系数")]
    public float InjectIndicatorFactorForDifferentialShotAngle = 1f;

    [JsonProperty(PropertyName = "差位射数")]
    public int DifferentiatedShotCount = 0;

    [JsonProperty(PropertyName = "差位偏移X")]
    public float DifferentiatedOffsetX = 0f;

    [JsonProperty(PropertyName = "差位偏移Y")]
    public float DifferentiatedOffsetY = 0f;

    [JsonProperty(PropertyName = "指示物数量注入差位射数名")]
    public string InjectIndicatorNameForDifferentiatedShotCount = "";

    [JsonProperty(PropertyName = "指示物数量注入差位射数系数")]
    public float InjectIndicatorFactorForDifferentiatedShotCount = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差位偏移X名")]
    public string InjectIndicatorNameForDifferentiatedOffsetX = "";

    [JsonProperty(PropertyName = "指示物数量注入差位偏移X系数")]
    public float InjectIndicatorFactorForDifferentiatedOffsetX = 1f;

    [JsonProperty(PropertyName = "指示物数量注入差位偏移Y名")]
    public string InjectIndicatorNameForDifferentiatedOffsetY = "";

    [JsonProperty(PropertyName = "指示物数量注入差位偏移Y系数")]
    public float InjectIndicatorFactorForDifferentiatedOffsetY = 1f;

    [JsonProperty(PropertyName = "锁定范围")]
    public int LockRange = 0;

    [JsonProperty(PropertyName = "锁定速度")]
    public float LockSpeed = 0f;

    [JsonProperty(PropertyName = "指示物数量注入锁定速度名")]
    public string InjectIndicatorNameForLockSpeed = "";

    [JsonProperty(PropertyName = "指示物数量注入锁定速度系数")]
    public float InjectIndicatorFactorForLockSpeed = 1f;

    [JsonProperty(PropertyName = "以弹为位")]
    public bool UseProjectileAsPosition = false;

    [JsonProperty(PropertyName = "持续时间")]
    public int Duration = -1;

    [JsonProperty(PropertyName = "计入仇恨")]
    public bool IncludeHatred = false;

    [JsonProperty(PropertyName = "锁定血少")]
    public bool LockLowLife = false;

    [JsonProperty(PropertyName = "锁定低防")]
    public bool LockLowDefense = false;

    [JsonProperty(PropertyName = "仅攻击对象")]
    public bool AttackTargetOnly = false;

    [JsonProperty(PropertyName = "逆仇恨锁定")]
    public bool ReverseHatredLock = false;

    [JsonProperty(PropertyName = "逆血量锁定")]
    public bool ReverseLifeLock = false;

    [JsonProperty(PropertyName = "逆防御锁定")]
    public bool ReverseDefenseLock = false;

    [JsonProperty(PropertyName = "仅扇形锁定")]
    public bool FanShapedLock = false;

    [JsonProperty(PropertyName = "扇形半偏角")]
    public int FanShapedHalfAngle = 60;

    [JsonProperty(PropertyName = "以锁定为点")]
    public bool UseLockAsPoint = false;

    [JsonProperty(PropertyName = "最大锁定数")]
    public int MaxLocks = 1;

    [JsonProperty(PropertyName = "弹点召唤怪物")]
    public int SummonMonsterAtProjectilePoint = 0;

    [JsonProperty(PropertyName = "弹点召唤怪物无弹")]
    public bool SummonMonsterAtProjectilePointWithoutProjectile = false;

    public ProjGroup(int id, int sx, int sy, int dm)
    {
        this.ProjectileID = id;
        this.XSpeed = sx;
        this.YSpeed = sy;
        this.Damage = dm;
    }
}