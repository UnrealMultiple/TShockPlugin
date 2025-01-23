using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace ItemDecoration.Configured
{
    public class ItemAboveHeadConfig
    {
        [LocalizedPropertyName(CultureType.Chinese, "ItemAboveHead")]
        [LocalizedPropertyName(CultureType.English, "ItemAboveHead")]
        [LocalizedPropertyName(CultureType.Spanish, "ItemSobreCaveza")]
        public bool ItemAboveHead { get; set; }

        // Constructor para inicializar valores predeterminados
        public ItemAboveHeadConfig()
        {
            ItemAboveHead = true; // Valor por defecto
        }
    }
}