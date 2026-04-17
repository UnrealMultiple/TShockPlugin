using System.Collections;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;

namespace DeltaForce.Game.Modules;

public class GameUitls
{
    public static void ClearPlayerInventory(TSPlayer player, bool drop)
    {
        for (int i = 0; i < NetItem.MaxInventory; i++)
        {
            if (i < NetItem.InventoryIndex.Item2)
            {
                var item = player.TPlayer.inventory[i];
                if (drop)
                    DropItem(player, item);
                if(!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.ArmorIndex.Item2)
            {
                //59-78
                var index = i - NetItem.ArmorIndex.Item1;
                var item = player.TPlayer.armor[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.DyeIndex.Item2)
            {
                //79-88
                var index = i - NetItem.DyeIndex.Item1;
                var item = player.TPlayer.dye[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.MiscEquipIndex.Item2)
            {
                //89-93
                var index = i - NetItem.MiscEquipIndex.Item1;
                var item = player.TPlayer.miscEquips[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.MiscDyeIndex.Item2)
            {
                //93-98
                var index = i - NetItem.MiscDyeIndex.Item1;
                var item = player.TPlayer.miscDyes[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.PiggyIndex.Item2)
            {
                //98-138
                var index = i - NetItem.PiggyIndex.Item1;
                var item = player.TPlayer.bank.item[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.SafeIndex.Item2)
            {
                //138-178
                var index = i - NetItem.SafeIndex.Item1;
                var item = player.TPlayer.bank2.item[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.TrashIndex.Item2)
            {
                //179-219
                var index = i - NetItem.TrashIndex.Item1;
                var item = player.TPlayer.trashItem;
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.ForgeIndex.Item2)
            {
                //220
                var index = i - NetItem.ForgeIndex.Item1;
                var item = player.TPlayer.bank3.item[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.VoidIndex.Item2)
            {
                //260
                var index = i - NetItem.VoidIndex.Item1;
                var item = player.TPlayer.bank4.item[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout1Armor.Item2)
            {
                var index = i - NetItem.Loadout1Armor.Item1;
                var item = player.TPlayer.Loadouts[0].Armor[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout1Dye.Item2)
            {
                var index = i - NetItem.Loadout1Dye.Item1;
                var item = player.TPlayer.Loadouts[0].Dye[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout2Armor.Item2)
            {
                var index = i - NetItem.Loadout2Armor.Item1;
                var item = player.TPlayer.Loadouts[1].Armor[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout2Dye.Item2)
            {
                var index = i - NetItem.Loadout2Dye.Item1;
                var item = player.TPlayer.Loadouts[1].Dye[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout3Armor.Item2)
            {
                var index = i - NetItem.Loadout3Armor.Item1;
                var item = player.TPlayer.Loadouts[2].Armor[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
            else if (i < NetItem.Loadout3Dye.Item2)
            {
                var index = i - NetItem.Loadout3Dye.Item1;
                var item = player.TPlayer.Loadouts[2].Dye[index];
                if (drop)
                    DropItem(player, item);
                if (!item.IsAir)
                    item.TurnToAir();
            }
        }

        for (int k = 0; k < NetItem.InventorySlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Inventory0 + k);
        }
        for (int k = 0; k < NetItem.ArmorSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Armor0 + k);
        }
        for (int k = 0; k < NetItem.DyeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Dye0 + k);
        }
        for (int k = 0; k < NetItem.MiscEquipSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Misc0 + k);
        }
        for (int k = 0; k < NetItem.MiscDyeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.MiscDye0 + k);
        }
        for (int k = 0; k < NetItem.PiggySlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank1_0 + k);
        }
        for (int k = 0; k < NetItem.SafeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank2_0 + k);
        }
        NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.TrashItem);
        for (int k = 0; k < NetItem.ForgeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank3_0 + k);
        }
        for (int k = 0; k < NetItem.VoidSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank4_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout1_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout1_Dye_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout2_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout2_Dye_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout3_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, -1, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout3_Dye_0 + k);
        }

        for (int k = 0; k < NetItem.InventorySlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Inventory0 + k);
        }
        for (int k = 0; k < NetItem.ArmorSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Armor0 + k);
        }
        for (int k = 0; k < NetItem.DyeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Dye0 + k);
        }
        for (int k = 0; k < NetItem.MiscEquipSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Misc0 + k);
        }
        for (int k = 0; k < NetItem.MiscDyeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.MiscDye0 + k);
        }
        for (int k = 0; k < NetItem.PiggySlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank1_0 + k);
        }
        for (int k = 0; k < NetItem.SafeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank2_0 + k);
        }
        NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.TrashItem);
        for (int k = 0; k < NetItem.ForgeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank3_0 + k);
        }
        for (int k = 0; k < NetItem.VoidSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Bank4_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout1_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout1_Dye_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout2_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout2_Dye_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout3_Armor_0 + k);
        }
        for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
        {
            NetMessage.SendData(5, player.Index, -1, NetworkText.Empty, player.Index, PlayerItemSlotID.Loadout3_Dye_0 + k);
        }
    }

    public static void DropItem(TSPlayer player, Item item)
    {
        int num = Item.NewItem(new EntitySource_DebugCommand(), (int)player.X, (int)player.Y, player.TPlayer.width, player.TPlayer.height, item.type, item.stack, noBroadcast: true, item.prefix, noGrabDelay: true);
        TSPlayer.All.SendData(PacketTypes.ItemDrop, "", num, 1f);
    }
}
