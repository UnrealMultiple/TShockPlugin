using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace CaiRewardChest;

public static class Utils
{
    public static void GiveItem(RewardChest chest, GetDataHandlers.ChestOpenEventArgs args)
    {
        int chestType = WorldGen.GetChestItemDrop(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type);

        foreach (Item? i in chest.Chest.item) GiveItemByDrop(args.Player, i.netID, i.stack, i.prefix);
        List<string> itemsReceived = chest.Chest.item
            .Where(i => i != null && i.netID != 0 && i.stack != 0)
            .Select(i => TShock.Utils.ItemTag(i)).ToList();
        itemsReceived.Add(TShock.Utils.ItemTag(new Item
        {
            netID = chestType,
            stack = 1
        }));
        GiveItemByDrop(args.Player, chestType, 1, 0);
        args.Player.SendSuccessMessage($"[i:{chestType}]你打开了一个奖励箱: " +
                                       $"" + string.Join(", ", itemsReceived));
        chest.HasOpenPlayer.Add(args.Player.Account.ID);
        Db.UpdateChest(chest);
    }
    private static void GiveItemByDrop(TSPlayer plr, int type, int stack, int prefix)
    {
        int number = Item.NewItem(new EntitySource_DebugCommand(), (int)plr.X, (int)plr.Y, plr.TPlayer.width,
            plr.TPlayer.height, type, stack, true, prefix, true);
        Main.item[number].playerIndexTheItemIsReservedFor = plr.Index;
        plr.SendData(PacketTypes.ItemDrop, number: number, number2: 1f);
        plr.SendData(PacketTypes.ItemOwner, null, number);
    }
}