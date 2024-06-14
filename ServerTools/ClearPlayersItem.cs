using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace ServerTools
{
    internal class ClearPlayersItem
    {
        public void ClearItem(Item[] items, TSPlayer tSPlayer)
        {
            if (tSPlayer == null || !tSPlayer.IsLoggedIn)
            {
                return;
            }
            for (int i = 0; i < tSPlayer.TPlayer.inventory.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.inventory[i].IsAir && tSPlayer.TPlayer.inventory[i].type == item.type)
                    {
                        tSPlayer.TPlayer.inventory[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Inventory0 + i);
                    }
                }
            }
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
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[0].Armor.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[0].Armor[i].IsAir && tSPlayer.TPlayer.Loadouts[0].Armor[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[0].Armor[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout1_Armor_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[1].Armor.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[1].Armor[i].IsAir && tSPlayer.TPlayer.Loadouts[1].Armor[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[1].Armor[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout2_Armor_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[2].Armor.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[2].Armor[i].IsAir && tSPlayer.TPlayer.Loadouts[2].Armor[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[2].Armor[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout3_Armor_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.dye.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.dye[i].IsAir && tSPlayer.TPlayer.dye[i].type == item.type)
                    {
                        tSPlayer.TPlayer.dye[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Dye0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[0].Dye.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[0].Dye[i].IsAir && tSPlayer.TPlayer.Loadouts[0].Dye[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[0].Dye[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout1_Dye_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[1].Dye.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[1].Dye[i].IsAir && tSPlayer.TPlayer.Loadouts[1].Dye[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[1].Dye[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout2_Dye_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.Loadouts[2].Dye.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.Loadouts[2].Dye[i].IsAir && tSPlayer.TPlayer.Loadouts[2].Dye[i].type == item.type)
                    {
                        tSPlayer.TPlayer.Loadouts[2].Dye[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Loadout3_Dye_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.miscEquips.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.miscEquips[i].IsAir && tSPlayer.TPlayer.miscEquips[i].type == item.type)
                    {
                        tSPlayer.TPlayer.miscEquips[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Misc0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.miscDyes.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.miscDyes[i].IsAir && tSPlayer.TPlayer.miscDyes[i].type == item.type)
                    {
                        tSPlayer.TPlayer.miscDyes[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.MiscDye0 + i);
                    }
                }
            }
            foreach (Item item in items)
            {
                if (!tSPlayer.TPlayer.trashItem.IsAir && tSPlayer.TPlayer.trashItem.type == item.type)
                {
                    tSPlayer.TPlayer.trashItem.TurnToAir();
                    tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.TrashItem);
                }
            }
            foreach (Item item in items)
            {
                if (!tSPlayer.TPlayer.inventory[tSPlayer.TPlayer.selectedItem].IsAir && tSPlayer.TPlayer.inventory[tSPlayer.TPlayer.selectedItem].type == item.type)
                {
                    tSPlayer.TPlayer.inventory[tSPlayer.TPlayer.selectedItem].TurnToAir();
                    tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.InventoryMouseItem);
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.bank.item.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.bank.item[i].IsAir && tSPlayer.TPlayer.bank.item[i].type == item.type)
                    {
                        tSPlayer.TPlayer.bank.item[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Bank1_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.bank2.item.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.bank2.item[i].IsAir && tSPlayer.TPlayer.bank2.item[i].type == item.type)
                    {
                        tSPlayer.TPlayer.bank2.item[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Bank2_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.bank3.item.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.bank3.item[i].IsAir && tSPlayer.TPlayer.bank3.item[i].type == item.type)
                    {
                        tSPlayer.TPlayer.bank3.item[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Bank3_0 + i);
                    }
                }
            }
            for (int i = 0; i < tSPlayer.TPlayer.bank4.item.Length; i++)
            {
                foreach (Item item in items)
                {
                    if (!tSPlayer.TPlayer.bank4.item[i].IsAir && tSPlayer.TPlayer.bank4.item[i].type == item.type)
                    {
                        tSPlayer.TPlayer.bank4.item[i].TurnToAir();
                        tSPlayer.SendData(PacketTypes.PlayerSlot, "", tSPlayer.Index, Terraria.ID.PlayerItemSlotID.Bank4_0 + i);
                    }
                }
            }
            for (int i = 0; i < 22; i++)
            {
                tSPlayer.TPlayer.buffType[i] = 0;
            }
            tSPlayer.SendData(PacketTypes.PlayerBuff, "", tSPlayer.Index, 0f, 0f, 0f, 0);
        }
    }
}
