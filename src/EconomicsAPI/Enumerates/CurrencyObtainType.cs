using System.ComponentModel;

namespace EconomicsAPI.Enumerates;
public enum CurrencyObtainType
{
    [Description("不可获取")]
    None,

    [Description("击杀怪物")]
    KillNpc,

    [Description("挖掘矿物")]
    KillTiile,
}
