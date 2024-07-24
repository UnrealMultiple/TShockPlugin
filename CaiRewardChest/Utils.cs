using Terraria;
using TShockAPI;

namespace CaiRewardChest;

public class Utils
{
    public static int InventorySlotAvailableCount(TSPlayer plr)
    {
        int num = 0;
        if (plr.RealPlayer)
            for (int index = 0; index < 50; ++index)
                if (plr.TPlayer.inventory[index] == null || !plr.TPlayer.inventory[index].active ||
                    plr.TPlayer.inventory[index].Name == "")
                    ++num;
        return num;
    }
    public static void GiveItem(RewardChest chest, GetDataHandlers.ChestOpenEventArgs args)
    {
        int chestType = WorldGen.GetChestItemDrop(chest.X, chest.Y, Main.tile[chest.X, chest.Y].type);
        if (InventorySlotAvailableCount(args.Player) >=
            chest.Chest.item.Count(i => i != null && i.netID != 0 && i.stack != 0) + 1)
        {
            foreach (Item? i in chest.Chest.item) args.Player.GiveItem(i.netID, i.stack, i.prefix);
            List<string> itemsReceived = chest.Chest.item
                .Where(i => i != null && i.netID != 0 && i.stack != 0)
                .Select(i => TShock.Utils.ItemTag(i)).ToList();


            itemsReceived.Add(TShock.Utils.ItemTag(new Item()
            {
                netID = chestType,
                stack = 1
            }));
            args.Player.GiveItem(chestType, 1, 0);
            args.Player.SendSuccessMessage($"[i:{chestType}]你打开了一个奖励箱: " +
                                           $"" + string.Join(", ", itemsReceived));
            chest.HasOpenPlayer.Add(args.Player.Account.ID);
            Db.UpdateChest(chest);
        }
        else
        {
            args.Player.SendWarningMessage($"[i:{chestType}]你的背包格子不够哦," +
                                           $"还需要清空{chest.Chest.item.Count(i => i != null && i.netID != 0 && i.stack != 0) + 1 - InventorySlotAvailableCount(args.Player)}个格子!");
        }
    }
}