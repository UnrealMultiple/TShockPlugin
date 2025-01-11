using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace SurfaceBlock;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "SurfaceBlock";

    [LocalizedPropertyName(CultureType.Chinese, "开关", Order = 0)]
    [LocalizedPropertyName(CultureType.English, "Enabled")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "销毁秒数", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "DestructionSeconds")]
    public int Seconds { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "物品掉落", Order = 2)]
    [LocalizedPropertyName(CultureType.English, "ItemDrop")]
    public bool ItemDorp { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "还原图格", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "TileRestoration")]
    public bool KillTile { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "是否广播", Order = 4)]
    [LocalizedPropertyName(CultureType.English, "Broadcast")]
    public bool Mess { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "禁用弹幕", Order = 5)]
    [LocalizedPropertyName(CultureType.English, "BlockedProjectiles")]
    public HashSet<int>? ClearTable { get; set; } = new HashSet<int>();

    #region 预设参数方法
    protected override void SetDefault()
    {
        this.ClearTable = new HashSet<int>
        {
            28, 29, 37, 65, 68, 99, 108, 136, 137,
            138, 139, 142, 143, 144, 146, 147, 149,
            164, 339, 341, 354, 453, 516, 519, 637,
            716, 718, 727, 773, 780, 781, 782, 783,
            784, 785, 786, 787, 788, 789, 790, 791,
            792, 796, 797, 798, 799, 800, 801, 804,
            805, 806, 807, 809, 810, 863, 868, 869,
            904, 905, 906, 910, 911, 949, 1013, 1014
        };
    }
    #endregion
}