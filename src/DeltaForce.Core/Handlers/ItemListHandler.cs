using DeltaForce.Protocol.Packets;
using DeltaForce.Protocol.Processing;
using LazyAPI.Utility;

namespace DeltaForce.Core.Handlers;

public class ItemListHandler : RequestHandlerBase<ItemListRequestPacket, ItemListResponsePacket>
{
    public override ItemListResponsePacket Handle(ItemListRequestPacket request)
    {
        var configItems = Config.Instance.Items;

        var items = configItems.Select(item => new ItemInfo
        {
            Name = item.Name,
            Type = item.Type,
            Weight = item.Weight,
            Value = item.Value
        }).ToArray();

        var response = CreateSuccessResponse(request, GetString("Success"));
        response.Items = items;
        response.TotalCount = items.Length;

        return response;
    }
}
