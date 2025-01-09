using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace BP;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "Back";

    [LocalizedPropertyName(CultureType.Chinese, "Back冷却时间")]
    [LocalizedPropertyName(CultureType.English, "CD")]
    public int BackCooldown { get; set; } = 20;
}