using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class RequestPacket : IRequestPacket
{
    public PacketType PacketID { get; set; }
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}

public class ResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.Response;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string> Data { get; set; } = new();
}
