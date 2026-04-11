using DeltaForce.Protocol.Enums;
using DeltaForce.Protocol.Interfaces;

namespace DeltaForce.Protocol.Packets;

public class ItemListRequestPacket : IRequestPacket
{
    public PacketType PacketID => PacketType.ItemListRequest;
    public Guid RequestId { get; set; } = Guid.NewGuid();
}

public class ItemInfo
{
    public string Name { get; set; } = string.Empty;

    public int Type { get; set; }

    public int Weight { get; set; }

    public long Value { get; set; }
}

public class ItemListResponsePacket : IResponsePacket
{
    public PacketType PacketID => PacketType.ItemListResponse;
    public Guid RequestId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public ItemInfo[] Items { get; set; } = [];
    public int TotalCount { get; set; }
}
