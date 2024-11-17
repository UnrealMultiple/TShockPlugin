using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class Config : JsonConfigBase<Config>
{
    [CultureProperty( CultureType.EN_US, "forward_command")]
    [CultureProperty(CultureType.ZH_CN, "转发指令")]
    public bool ForwardCommamd { get; set; } = false;

    [CultureProperty(CultureType.EN_US, "rest_address")]
    [CultureProperty(CultureType.ZH_CN, "Rest地址")]
    public List<string> RestHost { get; set; } = new();

    [CultureProperty(CultureType.EN_US, "server_name")]
    [CultureProperty(CultureType.ZH_CN, "服务器名称")]
    public string ServerName { get; set; } = string.Empty;

    [CultureProperty(CultureType.EN_US, "token")]
    [CultureProperty(CultureType.ZH_CN, "验证令牌")]
    public string Verify { get; set; } = string.Empty;

    [CultureProperty(CultureType.EN_US, "message_option")]
    [CultureProperty(CultureType.ZH_CN, "消息设置")]
    public MessageFormat MessageFormat { get; set; } = new();

    protected override string Filename => "ChattyBridge";
}