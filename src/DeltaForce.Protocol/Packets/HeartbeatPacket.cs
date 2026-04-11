using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class HeartbeatPacket : IRequestPacket
{
    public PacketType PacketID => PacketType.Heartbeat;
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public long Timestamp { get; set; }
    public int SequenceNumber { get; set; }
}

public class HeartbeatResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.HeartbeatResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "OK";
    public long ServerTimestamp { get; set; }
    public int SequenceNumber { get; set; }
}
