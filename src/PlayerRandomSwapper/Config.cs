using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace PlayerRandomSwapper;
[Config]
internal class Config : JsonConfigBase<Config>
{
    protected override string Filename => "PlayerRandomSwapper";

    [LocalizedPropertyName(CultureType.Chinese, "总开关", Order = -3)]
    [LocalizedPropertyName(CultureType.English, "PluginEnabled", Order = -3)]
    public bool pluginEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "传送间隔秒")]
    [LocalizedPropertyName(CultureType.English, "IntervalSeconds")]
    public int IntervalSeconds { get; set; } = 10;

    [LocalizedPropertyName(CultureType.Chinese, "随机传送间隔")]
    [LocalizedPropertyName(CultureType.English, "RandomIntervalSeconds")]
    public bool RandomInterval { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "传送间隔秒最大值")]
    [LocalizedPropertyName(CultureType.English, "MaxRandomIntervalSeconds")]
    public int MaxRandomIntervalSeconds { get; set; } = 30;

    [LocalizedPropertyName(CultureType.Chinese, "传送间隔秒最小值")]
    [LocalizedPropertyName(CultureType.English, "MinRandomIntervalSeconds")]
    public int MinRandomIntervalSeconds { get; set; } = 10;

    [LocalizedPropertyName(CultureType.Chinese, "双人模式允许玩家和自己交换")]
    [LocalizedPropertyName(CultureType.English, "AllowSamePlayerSwap")]
    public bool AllowSamePlayerSwap { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "多人打乱模式")]
    [LocalizedPropertyName(CultureType.English, "Multi-PlayerMode")]
    public bool MultiPlayerMode { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播剩余传送时间")]
    [LocalizedPropertyName(CultureType.English, "BroadcastRemainingTime")]
    public bool BroadcastRemainingTimeEnabled { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "广播交换倒计时阈值")]
    [LocalizedPropertyName(CultureType.English, "BroadcastRemainingTimeThreshold")]
    public int BroadcastRemainingTimeThreshold { get; set; } = 5;

    [LocalizedPropertyName(CultureType.Chinese, "广播玩家交换位置信息")]
    [LocalizedPropertyName(CultureType.English, "BroadcastPlayerSwap")]
    public bool BroadcastPlayerSwapEnabled { get; set; } = true;

}
