using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class Config : JsonConfigBase<Config>
{
    [LocalizationProperty("转发指令", "forward_command")]
    public bool ForwardCommamd { get; set; } = false;

    [LocalizationProperty("Rest地址", "rest_address")]
    public List<string> RestHost { get; set; } = new();

    [LocalizationProperty("服务器名称", "server_name")]
    public string ServerName { get; set; } = string.Empty;

    [LocalizationProperty("验证令牌","token")]
    public string Verify { get; set; } = string.Empty;

    [LocalizationProperty("消息设置", "message_option")]
    public MessageFormat MessageFormat { get; set; } = new();

    protected override string Filename => "ChattyBridge";
}