using LazyAPI.ConfigFiles;
using Newtonsoft.Json;

namespace ChattyBridge;

public class MessageFormat
{
    [LocalizationProperty("聊天格式","message_format")]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [LocalizationProperty("离开格式","leave_format")]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [LocalizationProperty("加入格式", "join_format")]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}