using Newtonsoft.Json;

namespace ChattyBridge;

public class MessageFormat
{
    [JsonProperty("聊天格式")]
    public string ChatFormat { get; set; } = "[{0}]{1}: {2}";

    [JsonProperty("离开格式")]
    public string LeaveFormat { get; set; } = "[{0}]{1}离开服务器";

    [JsonProperty("加入格式")]
    public string JoinFormat { get; set; } = "[{0}]{1}加入服务器";
}