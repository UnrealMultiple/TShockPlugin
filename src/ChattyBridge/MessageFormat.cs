using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class MessageFormat
{
    [LocalizedPropertyName(CultureType.English, "message_format")]
    [LocalizedPropertyName(CultureType.Chinese, "聊天格式")]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [LocalizedPropertyName(CultureType.English, "leave_format")]
    [LocalizedPropertyName(CultureType.Chinese, "离开格式")]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [LocalizedPropertyName(CultureType.English, "join_format")]
    [LocalizedPropertyName(CultureType.Chinese, "加入格式")]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}