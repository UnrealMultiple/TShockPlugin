using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace Noagent;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "Noagent";

    [LocalizedPropertyName(CultureType.Chinese, "检测服务")]
    [LocalizedPropertyName(CultureType.English, "Provider")]
    public string Provider { get; set; } = "blackbox";
}
