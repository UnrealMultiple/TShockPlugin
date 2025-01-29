using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace ServerTools;

internal class Utils
{
    public static void Clear7Item(TSPlayer Player)
    {
        if (!Player.TPlayer.armor[8].IsAir)
        {
            var item = Player.TPlayer.armor[8];
            Player.GiveItem(item.type, item.stack, item.prefix);
            Player.TPlayer.armor[8].TurnToAir();
            Player.SendData(PacketTypes.PlayerSlot, "", Player.Index, Terraria.ID.PlayerItemSlotID.Armor0 + 8);
            TShock.Utils.Broadcast(GetString($"[ServerTools] 世界未开启困难模式，禁止玩家 [{Player.Name}]使用恶魔心饰品栏"), Color.DarkRed);
        }
    }


    #region 清理盔甲组逻辑
    public static void ClearItem(Item[] items, TSPlayer tSPlayer)
    {
        for (var i = 0; i < 10; i++)
        {
            foreach (var item in items)
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
}