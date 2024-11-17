using LazyAPI.ConfigFiles;

namespace ChattyBridge;

public class MessageFormat
{
    [LocalizationProperty("message_format", LocalizationType.EN_US)]
    [LocalizationProperty("聊天格式",LocalizationType.ZH_CN)]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [LocalizationProperty("leave_format", LocalizationType.EN_US)]
    [LocalizationProperty("离开格式",LocalizationType.ZH_CN)]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [LocalizationProperty("join_format", LocalizationType.EN_US)]
    [LocalizationProperty("加入格式", LocalizationType.ZH_CN)]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}