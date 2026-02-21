using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace CaiRewardChest;

public static class Utils
{
    public static void GiveItem(RewardChest chest, GetDataHandlers.ChestOpenEventArgs args)
    {
        var chestType = WorldGen.GetItemDrop_Chests(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type);

        foreach (var i in chest.Chest.item)
        {
            GiveItemByDrop(args.Player, i.type, i.stack, i.prefix);
        }

        var itemsReceived = chest.Chest.item
            .Where(i => i != null && i.type != 0 && i.stack != 0)
            .Select(i => TShock.Utils.ItemTag(i)).ToList();
        itemsReceived.Add(TShock.Utils.ItemTag(new Item { type = chestType, stack = 1 }));
        GiveItemByDrop(args.Player, chestType, 1, 0);
        args.Player.SendSuccessMessage(GetString($"[i:{chestType}]你打开了一个奖励箱: ") +
                                       "" + string.Join(", ", itemsReceived));
        chest.HasOpenPlayer.Add(args.Player.Account.ID);
        RewardChest.UpdateChest(chest);
    }

    private static void GiveItemByDrop(TSPlayer plr, int type, int stack, int prefix)
    {
        var number = Item.NewItem(new EntitySource_DebugCommand(), (int) plr.X, (int) plr.Y, plr.TPlayer.width,
            plr.TPlayer.height, type, stack, true, prefix, true);
        Main.item[number].playerIndexTheItemIsReservedFor = plr.Index;
        plr.SendData(PacketTypes.ItemDrop, number: number, number2: 1f);
        plr.SendData(PacketTypes.ItemOwner, null, number);
    }
}