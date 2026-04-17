using Newtonsoft.Json;

namespace DeltaForce.Game.Enitys;

public struct SocketOption()
{
    [JsonProperty("address")]
    public string Address { get; set; } = "127.0.0.1";

    [JsonProperty("port")]
    public int Port { get; set; } = 8888;
}
