using DeltaForce.Protocol.Enums;

namespace DeltaForce.Protocol.Interfaces;

public interface INetPacket
{
    PacketType PacketID { get; }
}