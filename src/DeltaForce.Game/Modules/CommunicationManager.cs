using DeltaForce.Protocol.Packets;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public class CommunicationManager
{
    public static async Task<PlayerInventoryResponsePacket?> GetPlayerInventory(TSPlayer player)
    {
        var packet = new PlayerInventoryRequestPacket
        {
            PlayerName = player.Name
        };
        return await Game.Client.RequestAsync<PlayerInventoryRequestPacket, PlayerInventoryResponsePacket>(packet);
    }

    public static async Task<SquadDataResponsePacket?> GetSquadData()
    {
        var packet = new SquadDataRequestPacket();
        return await Game.Client.RequestAsync<SquadDataRequestPacket, SquadDataResponsePacket>(packet);
    }

    public static async Task<ItemListResponsePacket?> GetItemList()
    {
        var packet = new ItemListRequestPacket();
        return await Game.Client.RequestAsync<ItemListRequestPacket, ItemListResponsePacket>(packet);
    }

    public static async Task<SaveInventoryResponsePacket?> SavePlayerInventory(TSPlayer player)
    {
        return await InventoryManager.SavePlayerInventoryAsync(player);
    }
}
