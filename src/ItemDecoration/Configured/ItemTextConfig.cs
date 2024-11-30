using LazyAPI.ConfigFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemDecoration.Configured;
public class ItemTextConfig : BaseItemOption
{
    [LocalizedPropertyName(CultureType.Chinese, "伤害文本")]
    [LocalizedPropertyName(CultureType.English, "damageText")]
    [LocalizedPropertyName(CultureType.Spanish, "textoDeDaño")]
    public string DamageText { get; set; } = string.Empty;  // Texto configurable para el daño

    [LocalizedPropertyName(CultureType.Chinese, "默认颜色")]
    [LocalizedPropertyName(CultureType.English, "defaultColor")]
    [LocalizedPropertyName(CultureType.Spanish, "colorPorDefecto")]
    public ColorConfig DefaultColor { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "稀有颜色")]
    [LocalizedPropertyName(CultureType.English, "rarityColors")]
    [LocalizedPropertyName(CultureType.Spanish, "coloresDeRaridad")]
    public Dictionary<int, ColorConfig> RarityColors { get; set; } = new();
}
