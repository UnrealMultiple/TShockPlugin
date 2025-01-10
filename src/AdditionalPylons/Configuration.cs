using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace AdditionalPylons;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AdditionalPylons";

    [LocalizedPropertyName(CultureType.Chinese, "丛林晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxJunglePylons")]
    public int MaxJunglePylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "森林晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxForestPylons")]
    public int MaxForestPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "神圣晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxHallowPylons")]
    public int MaxHallowPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "洞穴晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxCavernPylons")]
    public int MaxCavernPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "海洋晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxOceanPylons")]
    public int MaxOceanPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "沙漠晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxDesertPylons")]
    public int MaxDesertPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "雪原晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxSnowPylons")]
    public int MaxSnowPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "蘑菇晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxMushroomPylons")]
    public int MaxMushroomPylons = 2;

    [LocalizedPropertyName(CultureType.Chinese, "万能晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "MaxUniversalPylons")]
    public int MaxUniversalPylons = 2;
}