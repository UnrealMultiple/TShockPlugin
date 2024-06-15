using Terraria;
using TShockAPI;

namespace ServerTools
{
    internal class ClearPlayersItem
    {
        public void ClearItem(Item[] items, TSPlayer tSPlayer)
        {
            for (int i = 0; i < tSPlayer.TPlayer.armor.Length; i++)
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
    }
}
