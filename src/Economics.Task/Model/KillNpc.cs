using Newtonsoft.Json;

namespace Economics.Task.Model;

public class KillNpc
{
    [JsonProperty("怪物ID")]
    public int ID { get; set; }

    [JsonProperty("击杀数量")]
    public int Count { get; set; }
}