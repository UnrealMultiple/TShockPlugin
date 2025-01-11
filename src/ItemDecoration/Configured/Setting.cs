using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured;

[Config]
public class Setting : JsonConfigBase<Setting>
{
    [LocalizedPropertyName(CultureType.Chinese, "物品聊天")]
    [LocalizedPropertyName(CultureType.English, "itemChat")]
    [LocalizedPropertyName(CultureType.Spanish, "ObjetoEnChat")]
    public ItemChatConfig ItemChatConfig { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "物品切换")]
    [LocalizedPropertyName(CultureType.English, "itemText")]
    [LocalizedPropertyName(CultureType.Spanish, "textoDeObjeto")]
    public ItemTextConfig ItemTextConfig { get; set; } = new();

    protected override string Filename => "ItemDecoration";

    protected override void SetDefault()
    {
        this.ItemTextConfig.ShowName = true;
        this.ItemTextConfig.ShowDamage = true;
        this.ItemTextConfig.DamageText = "Damage"; // Valor predeterminado
        this.ItemTextConfig.DefaultColor = new ColorConfig { R = 255, G = 255, B = 255 };
        this.ItemTextConfig.RarityColors = new Dictionary<int, ColorConfig>
        {
            { -1, new ColorConfig { R = 169, G = 169, B = 169 } }, // Gris para sin rareza
            { 0, new ColorConfig { R = 255, G = 255, B = 255 } },  // Blanco
            { 1, new ColorConfig { R = 0, G = 128, B = 0 } },      // Verde
            { 2, new ColorConfig { R = 0, G = 112, B = 221 } },    // Azul
            { 3, new ColorConfig { R = 128, G = 0, B = 128 } },    // Morado
            { 4, new ColorConfig { R = 255, G = 128, B = 0 } },    // Naranja
            { 5, new ColorConfig { R = 255, G = 0, B = 0 } },      // Rojo
            { 6, new ColorConfig { R = 255, G = 215, B = 0 } },    // Amarillo (Mythical)
            { 7, new ColorConfig { R = 255, G = 105, B = 180 } },  // Rosa (Para misiones)
            { 8, new ColorConfig { R = 255, G = 215, B = 0 } },    // Dorado (Especial)
            { 9, new ColorConfig { R = 0, G = 255, B = 255 } },    // Cian (Único)
            { 10, new ColorConfig { R = 255, G = 105, B = 180 } }, // Magenta (Raro)
            { 11, new ColorConfig { R = 75, G = 0, B = 130 } },    // Índigo (Épico)
        };
    }
}

public class ColorConfig
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }

    public string ToHex()
    {
        return $"{this.R:X2}{this.G:X2}{this.B:X2}";
    }
}
