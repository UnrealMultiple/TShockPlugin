using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured;
public class ItemChatConfig : BaseItemOption
{
    [LocalizedPropertyName(CultureType.Chinese, "物品颜色")]
    [LocalizedPropertyName(CultureType.English, "itemColor")]
    [LocalizedPropertyName(CultureType.Spanish, "colorDelObjeto")]
    public ColorConfig ItemColor { get; set; } = new() { R = 255, G = 255, B = 255 };

    [LocalizedPropertyName(CultureType.Chinese, "伤害颜色")]
    [LocalizedPropertyName(CultureType.English, "damageColor")]
    [LocalizedPropertyName(CultureType.Spanish, "ColorDelDaño")]
    public ColorConfig DamageColor { get; set; } = new() { R = 0, G = 255, B = 255 };
}
