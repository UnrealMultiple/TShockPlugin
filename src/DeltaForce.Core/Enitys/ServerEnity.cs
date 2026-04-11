using Newtonsoft.Json;

namespace DeltaForce.Core.Enitys;

public struct ServerEnity()
{
    [JsonProperty("address")]
    public string Address { get; set; } = "";

    [JsonProperty("port")]
    public ushort Port { get; set; } = 7778;
}
