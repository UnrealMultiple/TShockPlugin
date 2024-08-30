using Newtonsoft.Json;

namespace CriticalHit;

public class CritMessage
{
    [JsonProperty("详细消息设置")]
    public Dictionary<string, int[]> Messages = new Dictionary<string, int[]>();
}