using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class GameStatePacket : IRequestPacket
{
    public PacketType PacketID => PacketType.GameState;

    public Guid RequestId { get; set; } = Guid.NewGuid();

    public bool IsGameActive { get; set; }

    public string GameMode { get; set; } = string.Empty;

    public double GameTime { get; set; }

    public long ServerTimestamp { get; set; }
}

public class GameStateResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.GameStateResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsGameActive { get; set; }
    public string GameMode { get; set; } = string.Empty;
    public double GameTime { get; set; }
}
