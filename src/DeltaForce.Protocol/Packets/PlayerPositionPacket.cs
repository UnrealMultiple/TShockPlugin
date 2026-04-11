using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class PlayerPositionRequestPacket : IRequestPacket
{
    public PacketType PacketID => PacketType.PlayerPositionRequest;
    public Guid RequestId { get; set; } = Guid.NewGuid();
}

public class PlayerPositionResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.PlayerPositionResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}
