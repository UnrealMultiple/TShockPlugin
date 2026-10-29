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
            if (i == null || i.IsAir)
            {
                continue;
            }

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
        // 1.4.5 的空物品生成会返回 400（网络生成哨兵），不能作为奖励槽同步。
        if (type <= 0 || stack <= 0)
        {
            return;
        }

        var number = Item.NewItem(new EntitySource_DebugCommand(), plr.TPlayer.Center, type, stack, prefix,
            NewItemOwnership.None, null, null, true);
        if (number < 0 || number >= Main.maxItems || Main.item[number].IsAir)
        {
            throw new InvalidOperationException("Unable to create reward item.");
        }

        var item = Main.item[number];
        item.playerIndexTheItemIsReservedFor = plr.Index;
        item.timeSinceTheItemHasBeenReservedForSomeone = 0;
        item.timeToKeepReservation = WorldItem.DefaultGrabDelay;
        plr.SendData(PacketTypes.ItemDrop, number: number, number2: 1f);
        // 1.4.5 的 ItemOwner 包新增保留时长，客户端和服务端必须一致。
        plr.SendData(PacketTypes.ItemOwner, null, number, item.timeToKeepReservation);
    }
}
