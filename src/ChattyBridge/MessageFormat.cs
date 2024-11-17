using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class MessageFormat
{
    [CultureProperty(CultureType.EN_US, "message_format")]
    [CultureProperty(CultureType.ZH_CN, "聊天格式")]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [CultureProperty(CultureType.EN_US, "leave_format")]
    [CultureProperty(CultureType.ZH_CN, "离开格式")]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [CultureProperty(CultureType.EN_US, "join_format")]
    [CultureProperty(CultureType.ZH_CN, "加入格式")]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}