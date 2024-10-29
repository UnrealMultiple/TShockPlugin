using Terraria;
using Terraria.ID;
using TShockAPI;

namespace AutoStoreItems;

internal class Tool
{
    #region 判断物品存到哪个空间的方法
    public static bool StoreItemInBanks(TSPlayer plr, Item inv, bool Auto, bool mess, List<string> List)
    {
        var Stored = false;
        Stored |= AutoStoreItems.Config.bank1 && AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, GetString("存钱罐"), Auto, mess, List);
        Stored |= AutoStoreItems.Config.bank2 && AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, GetString("保险箱"), Auto, mess, List);
        Stored |= AutoStoreItems.Config.bank3 && AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, GetString("护卫熔炉"), Auto, mess, List);
        Stored |= AutoStoreItems.Config.bank4 && AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, GetString("虚空袋"), Auto, mess, List);
        return Stored;
    }
    #endregion

    #region 自动储存物品方法
    public static bool AutoStoredItem(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName, bool Auto, bool mess, List<string> List)
    {
        if (!tplr.IsLoggedIn || tplr == null)
        {
            return false;
        }

        var plr = tplr.TPlayer;
        var itemName = new HashSet<string>(AutoStoreItems.Data.Items.SelectMany(x => x.ItemName));

        foreach (var bank in bankItems)
        {
            for (var i = 0; i < plr.inventory.Length; i++)
            {
                var inv = plr.inventory[i];
                var data = AutoStoreItems.Data.Items.FirstOrDefault(x => x.ItemName.Contains(inv.Name));

                if (Auto)
                {
                    if (!bank.IsAir && !bank.IsACoin && !List.Contains(bank.Name))
                    {
                        List.Add(bank.Name);

                        if (mess)
                        {
                            tplr.SendMessage(GetString($"已将 '[c/92C5EC:{bank.Name}]'添加到自动储存表|指令菜单:[c/A1D4C2:/ast]"), 255, 246, 158);
                        }
                    }
                }

                if (data != null
                    && inv.stack >= data.Stack
                    && itemName.Contains(inv.Name)
                    && inv.type == bank.type
                    && inv.type != plr.inventory[plr.selectedItem].type)
                {

                    bank.stack += inv.stack;
                    inv.TurnToAir();

                    if (bank.stack >= Item.CommonMaxStack)
                    {
                        bank.stack = Item.CommonMaxStack;
                    }

                    tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, PlayerItemSlotID.Inventory0 + i);
                    tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + Array.IndexOf(bankItems, bank));

                    if (mess)
                    {
                        tplr.SendMessage(GetString($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的{bankName} 当前数量: {bank.stack}"), 255, 246, 158);
                    }
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 自动存钱到存钱罐方法
    public static void CoinToBank(TSPlayer tplr, int coin)
    {
        var plr = tplr.TPlayer;
        var bankItem = new Item();
        var bankSolt = -1;

        for (var i2 = 0; i2 < 40; i2++)
        {
            bankItem = plr.bank.item[i2];
            if (bankItem.IsAir || bankItem.netID == coin)
            {
                bankSolt = i2;
                break;
            }
        }

        if (bankSolt != -1)
        {
            for (var i = 50; i < 54; i++)
            {
                var invItem = plr.inventory[i];
                if (invItem.netID == coin)
                {
                    invItem.netID = 0;
                    tplr.SendData(PacketTypes.PlayerSlot, "", tplr.Index, i);

                    bankItem.netID = coin;
                    bankItem.type = invItem.type;
                    bankItem.stack += invItem.stack;

                    if (bankItem.stack >= 100 && coin != 74)
                    {
                        bankItem.stack %= 100;
                        tplr.GiveItem(coin + 1, 1);
                    }

                    else if (bankItem.stack >= Item.CommonMaxStack && coin == 74)
                    {
                        bankItem.stack = Item.CommonMaxStack;
                    }

                    tplr.SendData(PacketTypes.PlayerSlot, "", tplr.Index, PlayerItemSlotID.Bank1_0 + bankSolt);
                    break;
                }
            }
        }
    }
    #endregion
}
