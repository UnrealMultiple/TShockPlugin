using Newtonsoft.Json;

namespace CriticalHit;

public class CritMessage
{
    [JsonProperty("ÏêÏ¸ÏûÏ¢ÉèÖÃ")]
    public Dictionary<string, int[]> Messages = new Dictionary<string, int[]>();
}
