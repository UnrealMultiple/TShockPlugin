using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace CaiPacketDebug;

[Config]
public class Config : JsonConfigBase<Config>
{
    protected override string Filename => "CaiPacketDebug";

    [LocalizedPropertyName(CultureType.Chinese, "C->S")]
    [LocalizedPropertyName(CultureType.English, "ClientToServer")]
    public DebugSettings ClientToServer { get; set; } = new();

    [LocalizedPropertyName(CultureType.Chinese, "S->C")]
    [LocalizedPropertyName(CultureType.English, "ServerToClient")]
    public DebugSettings ServerToClient { get; set; } = new();

    protected override void SetDefault()
    {
        this.ClientToServer = new DebugSettings
        {
            DebugAfterInit = false,
            ExcludePackets = new int[] { 114, 514 },
            WhiteListMode = false,
            WhiteListPackets = new int[] { 1, 2, 3 }
        };

        this.ServerToClient = new DebugSettings
        {
            DebugAfterInit = false,
            ExcludePackets = new int[] { 114, 514 },
            WhiteListMode = false,
            WhiteListPackets = new int[] { 1, 2, 3 }
        };
    }
}

public class DebugSettings
{
    [LocalizedPropertyName(CultureType.Chinese, "自启动")]
    [LocalizedPropertyName(CultureType.English, "DebugAfterInit")]
    public bool DebugAfterInit { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "排除数据包")]
    [LocalizedPropertyName(CultureType.English, "ExcludePackets")]
    public int[] ExcludePackets { get; set; } = Array.Empty<int>();

    [LocalizedPropertyName(CultureType.Chinese, "白名单模式")]
    [LocalizedPropertyName(CultureType.English, "WhiteListMode")]
    public bool WhiteListMode { get; set; }

    [LocalizedPropertyName(CultureType.Chinese, "白名单模式数据包")]
    [LocalizedPropertyName(CultureType.English, "WhiteListPackets")]
    public int[] WhiteListPackets { get; set; } = Array.Empty<int>();
}