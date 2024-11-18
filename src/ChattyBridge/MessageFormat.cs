using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class MessageFormat
{
    [CultureProperty(CultureType.English, "message_format")]
    [CultureProperty(CultureType.Chinese, "聊天格式")]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [CultureProperty(CultureType.English, "leave_format")]
    [CultureProperty(CultureType.Chinese, "离开格式")]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [CultureProperty(CultureType.English, "join_format")]
    [CultureProperty(CultureType.Chinese, "加入格式")]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}