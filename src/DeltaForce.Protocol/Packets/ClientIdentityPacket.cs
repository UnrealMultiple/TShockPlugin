using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class ClientIdentityPacket : INetPacket
{
    public PacketType PacketID => PacketType.ClientIdentity;
    
    public Guid ClientId { get; set; } = Guid.NewGuid();
    
    public string ClientName { get; set; } = string.Empty;
}

public class ClientIdentityResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.ClientIdentityResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public Guid ClientId { get; set; }
    
    public int SessionId { get; set; }
}
