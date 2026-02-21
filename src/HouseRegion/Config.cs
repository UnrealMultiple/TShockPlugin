using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace HouseRegion;

[Config]
public class Config : JsonConfigBase<Config>
{
    [LocalizedPropertyName(CultureType.Chinese, "进出房屋提示")]
    [LocalizedPropertyName(CultureType.English, "JoinRegionText")]
    public bool HouseRegion = true;

    [LocalizedPropertyName(CultureType.Chinese, "房屋最大面积")]
    [LocalizedPropertyName(CultureType.English, "HouseMaxSize")]
    public int HouseMaxSize = 1000;

    [LocalizedPropertyName(CultureType.Chinese, "房屋最小宽度")]
    [LocalizedPropertyName(CultureType.English, "MinWidth")]
    public int MinWidth = 15;

    [LocalizedPropertyName(CultureType.Chinese, "房屋最小高度")]
    [LocalizedPropertyName(CultureType.English, "MinHeight")]
    public int MinHeight = 10;

    [LocalizedPropertyName(CultureType.Chinese, "房屋最大数量")]
    [LocalizedPropertyName(CultureType.English, "HouseMaxNumber")]
    public int HouseMaxNumber = 2;

    [LocalizedPropertyName(CultureType.Chinese, "禁止锁房屋")]
    [LocalizedPropertyName(CultureType.English, "ProhibitLockHouse")]
    public bool LimitLockHouse = false;

    [LocalizedPropertyName(CultureType.Chinese, "保护宝石锁")]
    [LocalizedPropertyName(CultureType.English, "ProtectiveGemstoneLock")]
    public bool ProtectiveGemstoneLock = false;

    [LocalizedPropertyName(CultureType.Chinese, "始终保护箱子")]
    [LocalizedPropertyName(CultureType.English, "ProtectiveChest")]
    public bool ProtectiveChest = true;

    [LocalizedPropertyName(CultureType.Chinese, "冻结警告破坏者")]
    [LocalizedPropertyName(CultureType.English, "WarningSpoiler")]
    public bool WarningSpoiler = true;

    [LocalizedPropertyName(CultureType.Chinese, "禁止分享所有者")]
    [LocalizedPropertyName(CultureType.English, "ProhibitSharingOwner")]
    public bool ProhibitSharingOwner = false;

    [LocalizedPropertyName(CultureType.Chinese, "禁止分享使用者")]
    [LocalizedPropertyName(CultureType.English, "ProhibitSharingUser")]
    public bool ProhibitSharingUser = false;

    [LocalizedPropertyName(CultureType.Chinese, "禁止所有者修改使用者")]
    [LocalizedPropertyName(CultureType.English, "ProhibitOwnerModifyingUser")]
    public bool ProhibitOwnerModifyingUser = true;
    
    [LocalizedPropertyName(CultureType.Chinese, "禁止TP房屋")]
    [LocalizedPropertyName(CultureType.English, "ProhibitTPHouse")]
    public bool ProhibitTPHouse = false;
}