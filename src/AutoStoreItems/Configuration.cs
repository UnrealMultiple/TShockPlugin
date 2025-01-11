using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoStoreItems;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    [LocalizedPropertyName(CultureType.Chinese, "插件开关", Order = -11)]
    [LocalizedPropertyName(CultureType.English, "Enable", Order = -11)]
    public bool open { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "性能模式", Order = -10)]
    [LocalizedPropertyName(CultureType.English, "Optimize", Order = -11)]
    public bool PM { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "存钱罐", Order = -9)]
    [LocalizedPropertyName(CultureType.English, "Piggy", Order = -11)]
    public bool bank1 { get; set; } = true;
    [LocalizedPropertyName(CultureType.Chinese, "保险箱", Order = -8)]
    [LocalizedPropertyName(CultureType.English, "Safe", Order = -11)]
    public bool bank2 { get; set; } = true;
    [LocalizedPropertyName(CultureType.Chinese, "护卫熔炉", Order = -6)]
    [LocalizedPropertyName(CultureType.English, "Forge", Order = -11)]
    public bool bank3 { get; set; } = true;
    [LocalizedPropertyName(CultureType.Chinese, "虚空袋", Order = -7)]
    [LocalizedPropertyName(CultureType.English, "VoidVault", Order = -11)]
    public bool bank4 { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "触发存储的物品ID", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "TargetItems", Order = -11)]
    public int[] BankItems { get; set; } = new int[] { 87, 346, 3213, 3813, 4076, 4131, 5098, 5325 };

    [LocalizedPropertyName(CultureType.Chinese, "装备饰品的物品ID", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "LoadoutItem", Order = -11)]
    public int[] ArmorItem { get; set; } = new int[] { 88, 410, 411, 489, 490, 491, 855, 935, 1301, 2220, 2998, 3034, 3035, 3061, 3068, 4008, 4056, 4989, 5098, 5107, 5126 };

}