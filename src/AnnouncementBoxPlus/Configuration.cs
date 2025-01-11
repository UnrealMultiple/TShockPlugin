using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AnnouncementBoxPlus;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "AnnouncementBoxPlus";


    [LocalizedPropertyName(CultureType.Chinese, "启用")]
    [LocalizedPropertyName(CultureType.English, "enable")]
    public bool Enable { get; set; } = true;

    [LocalizedPropertyName(CultureType.Chinese, "广播内容仅触发者可见")]
    [LocalizedPropertyName(CultureType.English, "selfVisible")]
    public bool justWho { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播范围")]
    [LocalizedPropertyName(CultureType.English, "range")]
    public int range { get; set; } = 0;

    [LocalizedPropertyName(CultureType.Chinese, "启用权限")]
    [LocalizedPropertyName(CultureType.English, "enablePerm")]
    public bool usePerm { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "启用插件广播盒发送格式")]
    [LocalizedPropertyName(CultureType.English, "enableFormat")]
    public bool useFormat { get; set; } = false;

    [LocalizedPropertyName(CultureType.Chinese, "广播盒发送格式")]
    [LocalizedPropertyName(CultureType.English, "format")]
    public string formation { get; set; } = "%当前时间% %玩家组名% %玩家名%:%内容% #详细可查阅文档";

    [LocalizedPropertyName(CultureType.Chinese, "启用广播盒占位符")]
    [LocalizedPropertyName(CultureType.English, "enablePlaceholder")]
    public bool usePlaceholder { get; set; } = true;
}