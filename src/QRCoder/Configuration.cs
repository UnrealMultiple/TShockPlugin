using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace QRCoder;

[Config]
internal class Config : JsonConfigBase<Config>
{
    protected override string Filename => "QRCoder";

    [LocalizedPropertyName(CultureType.Chinese, "底墙")]
    [LocalizedPropertyName(CultureType.English, "BaseWall")]
    public int BaseWall = 155;

    [LocalizedPropertyName(CultureType.Chinese, "底颜色")]
    [LocalizedPropertyName(CultureType.English, "BaseColor")]
    public int BaseColor = 26;

    [LocalizedPropertyName(CultureType.Chinese, "码墙")]
    [LocalizedPropertyName(CultureType.English, "CodeWall")]
    public int CodeWall = 155;

    [LocalizedPropertyName(CultureType.Chinese, "码颜色")]
    [LocalizedPropertyName(CultureType.English, "CodeColor")]
    public int CodeColor = 29;

    [LocalizedPropertyName(CultureType.Chinese, "是否刷夜明漆")]
    [LocalizedPropertyName(CultureType.English, "IlluminantCoating")]
    public bool isGlowPaintApplied = true;

    [LocalizedPropertyName(CultureType.Chinese, "二维码的纠错等级(1-4)")]
    [LocalizedPropertyName(CultureType.English, "QRErrorCorrectionLevel(1-4)")]
    public int QRLevel = 1;
}