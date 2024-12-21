using Newtonsoft.Json;

namespace CustomMonster;

public class ProjUpdateGroup
{
    [JsonProperty(PropertyName = "弹幕ID")]
    public int ProjectileID = 0;

    [JsonProperty(PropertyName = "标志")]
    public string Sign = "";

    [JsonProperty(PropertyName = "AI条件")]
    public Dictionary<string, float> AiConditions = new Dictionary<string, float>();

    [JsonProperty(PropertyName = "X轴速度")]
    public float XSpeed = 0f;

    [JsonProperty(PropertyName = "Y轴速度")]
    public float YSpeed = 0f;

    [JsonProperty(PropertyName = "角度偏移")]
    public float AngleOffset = 0f;

    [JsonProperty(PropertyName = "速度注入AI0")]
    public bool SpeedInjectsIntoAi0 = false;

    [JsonProperty(PropertyName = "速度注入AI0后X轴速度")]
    public float XSpeedAfterAi0Injection = 0f;

    [JsonProperty(PropertyName = "速度注入AI0后Y轴速度")]
    public float YSpeedAfterAi0Injection = 0f;

    [JsonProperty(PropertyName = "弹幕伤害")]
    public int Damage = 0;

    [JsonProperty(PropertyName = "弹幕击退")]
    public int Knockback = 0;

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

    [JsonProperty(PropertyName = "锁定范围")]
    public int LockRange = 0;

    [JsonProperty(PropertyName = "锁定速度")]
    public float LockSpeed = 0f;

    [JsonProperty(PropertyName = "指示物数量注入锁定速度名")]
    public string IndicatorCountInjectLockSpeedName = "";

    [JsonProperty(PropertyName = "指示物数量注入锁定速度系数")]
    public float IndicatorCountInjectLockSpeedFactor = 1f;

    [JsonProperty(PropertyName = "计入仇恨")]
    public bool IncludeInHatred = false;

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

    [JsonProperty(PropertyName = "弹点召唤怪物")]
    public int SummonMonsterAtProjectilePoint = 0;

    [JsonProperty(PropertyName = "AI赋值")]
    public Dictionary<int, float> AiValues = new Dictionary<int, float>();

    [JsonProperty(PropertyName = "指示物注入AI赋值")]
    public Dictionary<int, string> IndicatorInjectAiValues = new Dictionary<int, string>();

    [JsonProperty(PropertyName = "弹幕X轴注入指示物名")]
    public string ProjectileXIndicatorName = "";

    [JsonProperty(PropertyName = "弹幕Y轴注入指示物名")]
    public string ProjectileYIndicatorName = "";

    [JsonProperty(PropertyName = "持续时间")]
    public int Duration = -1;

    [JsonProperty(PropertyName = "清除弹幕")]
    public bool DestroyProjectile = false;

    public ProjUpdateGroup(int id, string note)
    {
        this.ProjectileID = id;
        this.Sign = note;
    }
}
