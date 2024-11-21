using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured;
public class BaseItemOption
{
    [LocalizedPropertyName(CultureType.Chinese, "显示名称")]
    [LocalizedPropertyName(CultureType.English, "showName")]
    public bool ShowName { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "显示伤害")]
    [LocalizedPropertyName(CultureType.English, "showDamage")]
    public bool ShowDamage { get; set; }
}
