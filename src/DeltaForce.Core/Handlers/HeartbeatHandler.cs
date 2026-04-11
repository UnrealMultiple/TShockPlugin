using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;

namespace DeltaForce.Core.Handlers;

public class HeartbeatHandler : RequestHandlerBase<HeartbeatPacket, HeartbeatResponsePacket>
{
    public override HeartbeatResponsePacket Handle(HeartbeatPacket request)
    {
        return new HeartbeatResponsePacket
        {
            RequestId = request.RequestId,
            Success = true,
            Message = "Heartbeat OK",
            ServerTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SequenceNumber = request.SequenceNumber
        };
    }
}
