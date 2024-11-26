using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured;
public class BaseItemOption
{
    [LocalizedPropertyName(CultureType.Chinese, "显示名称")]
    [LocalizedPropertyName(CultureType.English, "showName")]
    [LocalizedPropertyName(CultureType.Spanish, "mostrarNombre")]
    public bool ShowName { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "显示伤害")]
    [LocalizedPropertyName(CultureType.English, "showDamage")]
    [LocalizedPropertyName(CultureType.Spanish, "mostrarDaño")]
    public bool ShowDamage { get; set; }
}
