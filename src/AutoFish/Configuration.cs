using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AutoFish;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    #region 实例变量
    [LocalizedPropertyName(CultureType.Chinese, "插件开关")]
    [LocalizedPropertyName(CultureType.English, "Enable")]
    public bool Enabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "多钩钓鱼", Order = -12)]
    [LocalizedPropertyName(CultureType.English, "MultipleFishFloats")]
    public bool MoreHook { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "随机物品", Order = -11)]
    [LocalizedPropertyName(CultureType.English, "RandCatches")]
    public bool Random { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "多钩上限", Order = -10)]
    [LocalizedPropertyName(CultureType.English, "MultipleFishFloatsLimit")]
    public int HookMax { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "Buff表", Order = -6)]
    [LocalizedPropertyName(CultureType.English, "SetBuffs")]
    public Dictionary<int, int> BuffID { get; set; } = new Dictionary<int, int>();

    [LocalizedPropertyName(CultureType.Chinese, "消耗模式", Order = -5)]
    [LocalizedPropertyName(CultureType.English, "ConsumeBait")]
    public bool ConMod { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "消耗数量", Order = -4)]
    [LocalizedPropertyName(CultureType.English, "ConsumeBaitNum")]
    public int BaitStack { get; set; } = 10;

    [LocalizedPropertyName(CultureType.Chinese, "自动时长", Order = -3)]
    [LocalizedPropertyName(CultureType.English, "Time")]
    public int timer { get; set; } = 24;

    [LocalizedPropertyName(CultureType.Chinese, "消耗物品", Order = -2)]
    [LocalizedPropertyName(CultureType.English, "ConsumeItem")]
    public List<int> BaitType { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "额外渔获", Order = -1)]
    [LocalizedPropertyName(CultureType.English, "AdditionalCatches")]
    public List<int> DoorItems = new();
    #endregion

    protected override string Filename => "AutoFish";

    #region 预设参数方法
    protected override void SetDefault()
    {
        this.BuffID = new Dictionary<int, int>()
        {
            { 80,10 },
            { 122,240 }
        };

        this.BaitType = new List<int>
        {
            2002, 2675, 2676, 3191, 3194
        };

        this.DoorItems = new List<int>
        {
            29,3093,4345
        };
    }
    #endregion

}