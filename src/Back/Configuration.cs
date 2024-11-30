using LazyAPI;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using TShockAPI;

namespace BP;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "Back";

    [LocalizedPropertyName(CultureType.Chinese, "Back冷却时间")]
    [LocalizedPropertyName(CultureType.English, "cd")]
    public int BackCooldown { get; set; } = 20;
}