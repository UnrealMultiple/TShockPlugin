using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace CreateSpawn;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "CreateSpawnConfig";

    [LocalizedPropertyName(CultureType.Chinese, "中心X")]
    [LocalizedPropertyName(CultureType.English, "CentreX")]
    public int CentreX { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "计数Y")]
    [LocalizedPropertyName(CultureType.English, "CountY")]
    public int CountY { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "微调X")]
    [LocalizedPropertyName(CultureType.English, "AdjustX")]
    public int AdjustX { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "微调Y")]
    [LocalizedPropertyName(CultureType.English, "AdjustY")]
    public int AdjustY { get; set; } = 0;

    protected override void SetDefault()
    {
        this.CentreX = 0;
        this.CountY = 0;
        this.AdjustX = 0;
        this.AdjustY = 0;
    }
}