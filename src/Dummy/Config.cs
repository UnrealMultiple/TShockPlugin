using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using TrProtocol.Models;

namespace Dummy;

[Config]
public class Config : JsonConfigBase<Config>
{

    [LocalizedPropertyName(CultureType.Chinese, "假人")]
    [LocalizedPropertyName(CultureType.English, "Dummys")]
    public DummyInfo[] Dummys = Array.Empty<DummyInfo>();

    protected override void SetDefault()
    {
        this.Dummys = new DummyInfo[1];
        this.Dummys[0] = new DummyInfo() { Name = "熙恩" };
    }
}

public class DummyInfo
{
    [LocalizedPropertyName(CultureType.Chinese, "登陆密码")]
    [LocalizedPropertyName(CultureType.English, "LoginPassword")]
    public string Password { get; set; } = string.Empty;

    public string UUID { get; set; } = Guid.NewGuid().ToString();

    [LocalizedPropertyName(CultureType.Chinese, "皮肤")]
    [LocalizedPropertyName(CultureType.English, "SkinVariant")]
    public byte SkinVariant { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "头发")]
    [LocalizedPropertyName(CultureType.English, "Hair")]
    public byte Hair { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "名字")]
    [LocalizedPropertyName(CultureType.English, "Name")]
    public string Name { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.Chinese, "染发")]
    [LocalizedPropertyName(CultureType.English, "HairDye")]
    public byte HairDye { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "隐藏设置")]
    [LocalizedPropertyName(CultureType.English, "HideMisc")]
    public byte HideMisc { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "头发颜色")]
    [LocalizedPropertyName(CultureType.English, "HairColor")]
    public Color HairColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "皮肤颜色")]
    [LocalizedPropertyName(CultureType.English, "SkinClolor")]
    public Color SkinColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "眼睛颜色")]
    [LocalizedPropertyName(CultureType.English, "EyeColor")]
    public Color EyeColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "上衣颜色")]
    [LocalizedPropertyName(CultureType.English, "ShirtColor")]
    public Color ShirtColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "内衣颜色")]
    [LocalizedPropertyName(CultureType.English, "UnderShirtColor")]
    public Color UnderShirtColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "裤子颜色")]
    [LocalizedPropertyName(CultureType.English, "PantColor")]
    public Color PantsColor { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "鞋子颜色")]
    [LocalizedPropertyName(CultureType.English, "ShoeColor")]
    public Color ShoeColor { get; set; }
}