using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class Config : JsonConfigBase<Config>
{
    [CultureProperty( CultureType.English, "forward_command")]
    [CultureProperty(CultureType.Chinese, "转发指令")]
    public bool ForwardCommamd { get; set; } = false;

    [CultureProperty(CultureType.English, "rest_address")]
    [CultureProperty(CultureType.Chinese, "Rest地址")]
    public List<string> RestHost { get; set; } = new();

    [CultureProperty(CultureType.English, "server_name")]
    [CultureProperty(CultureType.Chinese, "服务器名称")]
    public string ServerName { get; set; } = string.Empty;

    [CultureProperty(CultureType.English, "token")]
    [CultureProperty(CultureType.Chinese, "验证令牌")]
    public string Verify { get; set; } = string.Empty;

    [CultureProperty(CultureType.English, "message_option")]
    [CultureProperty(CultureType.Chinese, "消息设置")]
    public MessageFormat MessageFormat { get; set; } = new();

    protected override string Filename => "ChattyBridge";
}