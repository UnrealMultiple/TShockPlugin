using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace AutoClear;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AutoClear";

    [LocalizedPropertyName(CultureType.Chinese, "清理间隔")]
    [LocalizedPropertyName(CultureType.English, "Interval")]
    public int detectionIntervalSeconds { get; set; } = 100;

    [LocalizedPropertyName(CultureType.Chinese, "排除列表")]
    [LocalizedPropertyName(CultureType.English, "Exclude")]
    public List<int> NonSweepableItemIDs { get; set; } = new List<int>();

    [LocalizedPropertyName(CultureType.Chinese, "清理阈值")]
    [LocalizedPropertyName(CultureType.English, "Threshold")]
    public int SmartSweepThreshold { get; set; } = 10;

    [LocalizedPropertyName(CultureType.Chinese, "延迟清扫")]
    [LocalizedPropertyName(CultureType.English, "Dealy")]
    public int DelayedSweepTimeoutSeconds { get; set; } = 10;

    [LocalizedPropertyName(CultureType.Chinese, "延迟清扫消息")]
    [LocalizedPropertyName(CultureType.English, "DealyMsg")]
    public string DelayedSweepCustomMessage { get; set; } = "";

    [LocalizedPropertyName(CultureType.Chinese, "清扫挥动武器")]
    [LocalizedPropertyName(CultureType.English, "SweepSwinging")]
    public bool SweepSwinging { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "清扫投掷武器")]
    [LocalizedPropertyName(CultureType.English, "SweepThrowable")]
    public bool SweepThrowable { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "清扫普通物品")]
    [LocalizedPropertyName(CultureType.English, "SweepRegaular")]
    public bool SweepRegular { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "清扫装备")]
    [LocalizedPropertyName(CultureType.English, "SweepEquipment")]
    public bool SweepEquipment { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "清扫时装")]
    [LocalizedPropertyName(CultureType.English, "SweepVanity")]
    public bool SweepVanity { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "完成清扫消息")]
    [LocalizedPropertyName(CultureType.English, "SweepMsg")]
    public string CustomMessage { get; set; } = "";

    [LocalizedPropertyName(CultureType.Chinese, "清理提示")]
    [LocalizedPropertyName(CultureType.English, "SweepTip")]
    public bool SpecificMessage { get; set; } = true;
}