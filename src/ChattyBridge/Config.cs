using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace ChattyBridge;

[Config]
public class Config : JsonConfigBase<Config>
{
    [LocalizedPropertyName(CultureType.English, "forward_command")]
    [LocalizedPropertyName(CultureType.Chinese, "转发指令")]
    public bool ForwardCommand { get; set; } = false;

    [LocalizedPropertyName(CultureType.English, "rest_address")]
    [LocalizedPropertyName(CultureType.Chinese, "Rest地址")]
    public List<string> RestHost { get; set; } = new();

    [LocalizedPropertyName(CultureType.English, "server_name")]
    [LocalizedPropertyName(CultureType.Chinese, "服务器名称")]
    public string ServerName { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.English, "token")]
    [LocalizedPropertyName(CultureType.Chinese, "验证令牌")]
    public string Verify { get; set; } = string.Empty;

    [LocalizedPropertyName(CultureType.English, "message_option")]
    [LocalizedPropertyName(CultureType.Chinese, "消息设置")]
    public MessageFormat MessageFormat { get; set; } = new();

    protected override string Filename => "ChattyBridge";
}