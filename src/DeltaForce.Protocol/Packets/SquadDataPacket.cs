using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class SquadDataRequestPacket : IRequestPacket
{
    public PacketType PacketID => PacketType.SquadDataRequest;
    public Guid RequestId { get; set; } = Guid.NewGuid();
}

public class SquadMemberInfo
{
    public string PlayerName { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}

public class SquadInfo
{
    public int SquadId { get; set; }
    public SquadMemberInfo[] Members { get; set; } = Array.Empty<SquadMemberInfo>();
    public int MaxMembers { get; set; } = 3;
    public int CurrentMemberCount { get; set; }
}

public class SquadDataResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.SquadDataResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    
    public SquadInfo[] Squads { get; set; } = Array.Empty<SquadInfo>();
}
