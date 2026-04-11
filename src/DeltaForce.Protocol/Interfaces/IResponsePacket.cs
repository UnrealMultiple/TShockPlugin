namespace DeltaForce.Protocol.Interfaces;

public interface IResponsePacket : INetPacket
{
    Guid RequestId { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
}
