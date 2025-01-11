using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace BedSet;

[Config]
internal class Config : JsonConfigBase<Config>
{
    public class BedSpawn
    {

        [LocalizedPropertyName(CultureType.Chinese, "X坐标")]
        [LocalizedPropertyName(CultureType.Chinese, "X")]
        public int X { get; set; }

        [LocalizedPropertyName(CultureType.Chinese, "Y坐标")]
        [LocalizedPropertyName(CultureType.Chinese, "Y")]
        public int Y { get; set; }
    }

    protected override string Filename => "Bed";

    [LocalizedPropertyName(CultureType.Chinese, "重生点")]
    [LocalizedPropertyName(CultureType.Chinese, "SpawnOption")]
    public Dictionary<string, BedSpawn> SpawnOption { get; set; } = new();
}