using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using TShockAPI;

namespace ServerTools
{
    internal class ClearPlayersItem
    {
        #region 清理盔甲组逻辑
        public void ClearItem(Item[] items, TSPlayer tSPlayer)
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.armor[i].IsAir && tSPlayer.TPlayer.armor[i].type == item.type)
                    {
                        tSPlayer.TPlayer.armor[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Armor0 + i);
                    }
                }
            }
        }
        #endregion

        #region 清理第七格饰品位逻辑
        public void Clear7Item(EventArgs args)
        {
            foreach (TSPlayer p in TShock.Players)
            {
                if (p == null || !p.IsLoggedIn)
                    continue;

                int flag = 0;
                if (!p.TPlayer.armor[9].IsAir)
                {
                    Item i = p.TPlayer.armor[9];
                    GiveItem(p, i.type, i.stack, i.prefix);
                    p.TPlayer.armor[9].TurnToAir();
                    p.SendData(PacketTypes.PlayerSlot, "", p.Index, Terraria.ID.PlayerItemSlotID.Armor0 + 9);
                    flag++;
                }
                if (!p.TPlayer.armor[19].IsAir)
                {
                    Item i = p.TPlayer.armor[19];
                    GiveItem(p, i.type, i.stack, i.prefix);
                    p.TPlayer.armor[19].TurnToAir();
                    p.SendData(PacketTypes.PlayerSlot, "", p.Index, Terraria.ID.PlayerItemSlotID.Armor0 + 19);
                    flag++;
                }
                if (!p.TPlayer.dye[9].IsAir)
                {
                    Item i = p.TPlayer.dye[9];
                    GiveItem(p, i.type, i.stack, i.prefix);
                    p.TPlayer.dye[9].TurnToAir();
                    p.SendData(PacketTypes.PlayerSlot, "", p.Index, Terraria.ID.PlayerItemSlotID.Dye0 + 9);
                    flag++;
                }
                if (flag != 0)
                {
                    TShock.Utils.Broadcast($"[ServerTools] 世界未开启困难模式，禁止玩家 [{p.Name}]使用恶魔心饰品栏", Color.DarkRed);
                }
            }
        } 
        #endregion

        #region 补偿玩家放错第七格饰品位的方法
        public static void GiveItem(TSPlayer p, int type, int stack, int prefix = 0)
        {
            int num = Item.NewItem(new EntitySource_DebugCommand(), (int)p.TPlayer.Center.X, (int)p.TPlayer.Center.Y, p.TPlayer.width, p.TPlayer.height, type, stack, true, prefix, true, false);
            Main.item[num].playerIndexTheItemIsReservedFor = p.Index;
            p.SendData(PacketTypes.ItemDrop, "", num, 1f, 0f, 0f, 0);
            p.SendData(PacketTypes.ItemOwner, null, num, 0f, 0f, 0f, 0);
        } 
        #endregion
    }
}
