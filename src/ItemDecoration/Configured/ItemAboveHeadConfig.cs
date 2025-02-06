using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured
{
    public class ItemAboveHeadConfig
    {
        [LocalizedPropertyName(CultureType.Chinese, "物品显示在头顶")]
        [LocalizedPropertyName(CultureType.English, "ItemAboveHead")]
        [LocalizedPropertyName(CultureType.Spanish, "ItemSobreCaveza")]
        public bool ItemAboveHead { get; set; } = true; // Valor por defecto

        // Constructor para inicializar valores predeterminados
    }
}