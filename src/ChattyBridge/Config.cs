using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class Config : JsonConfigBase<Config>
{
    [LocalizationProperty("forward_command", LocalizationType.EN_US)]
    [LocalizationProperty("转发指令", LocalizationType.ZH_CN)]
    public bool ForwardCommamd { get; set; } = false;

    [LocalizationProperty("rest_address", LocalizationType.EN_US)]
    [LocalizationProperty("Rest地址", LocalizationType.ZH_CN)]
    public List<string> RestHost { get; set; } = new();

    [LocalizationProperty("server_name", LocalizationType.EN_US)]
    [LocalizationProperty("服务器名称", LocalizationType.ZH_CN)]
    public string ServerName { get; set; } = string.Empty;

    [LocalizationProperty("token", LocalizationType.EN_US)]
    [LocalizationProperty("验证令牌",LocalizationType.ZH_CN)]
    public string Verify { get; set; } = string.Empty;

    [LocalizationProperty("message_option", LocalizationType.EN_US)]
    [LocalizationProperty("消息设置", LocalizationType.ZH_CN)]
    public MessageFormat MessageFormat { get; set; } = new();

    protected override string Filename => "ChattyBridge";
}