using Newtonsoft.Json;
using LazyAPI.ConfigFiles;
using LazyAPI;

namespace AdditionalPylons;

[Config]
internal class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "AdditionalPylons";

    [LocalizedPropertyName(CultureType.Chinese, "丛林晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "jungleTowers")]
    public int JungleTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "森林晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "surfacePurityTowers")]
    public int SurfacePurityTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "神圣晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "hallowTowers")]
    public int HallowTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "洞穴晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "undergroundTowers")]
    public int UndergroundTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "海洋晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "beachTowers")]
    public int BeachTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "沙漠晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "desertTowers")]
    public int DesertTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "雪原晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "snowTowers")]
    public int SnowTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "蘑菇晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "glowingMushroomTowers")]
    public int GlowingMushroomTowerLimit = 2;

    [LocalizedPropertyName(CultureType.Chinese, "万能晶塔数量上限")]
    [LocalizedPropertyName(CultureType.English, "victoryTowers")]
    public int VictoryTowerLimit = 2;
}