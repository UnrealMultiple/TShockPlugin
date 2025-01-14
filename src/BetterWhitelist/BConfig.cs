using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace BetterWhitelist;

[Config]
public class BConfig : JsonConfigBase<BConfig>
{
    [LocalizedPropertyName(CultureType.Chinese, "白名单玩家", Order = 3)]
    [LocalizedPropertyName(CultureType.English, "WhitePlayers", Order = 3)]
    public List<string> WhitePlayers = new();

    [LocalizedPropertyName(CultureType.Chinese, "插件开关", Order = 0)]
    [LocalizedPropertyName(CultureType.English, "Enable", Order = 0)]
    public bool Enable { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "连接时不在白名单提示", Order = 1)]
    [LocalizedPropertyName(CultureType.English, "NotInWhiteList", Order = 1)]
    public string NotInWhiteList { get; set; } = GetString("你不在服务器白名单中！");

    protected override string Filename => "BetterWhitelist";
}