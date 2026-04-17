using Newtonsoft.Json;

namespace DeltaForce.Core.Enitys;

public struct DeltaItemEnity()
{
    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("type")]
    public int Type { get; set; }

    [JsonProperty("weight")]
    public int Weight { get; set; }

    [JsonProperty("value")]
    public long Value { get; set; }
}
