using Terraria;
using Terraria.ID;
using TShockAPI;

namespace AutoStoreItems;

internal class Tool
{
    #region 判断物品存到哪个空间的方法
    public static bool StoreItemInBanks(TSPlayer plr, bool listen, bool mess, List<int> List)
    {
        var Stored = false;
        Stored |= AutoStoreItems.Config.bank1 && AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, GetString("存钱罐"), listen, mess, List);
        Stored |= AutoStoreItems.Config.bank2 && AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, GetString("保险箱"), listen, mess, List);
        Stored |= AutoStoreItems.Config.bank3 && AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, GetString("护卫熔炉"), listen, mess, List);
        Stored |= AutoStoreItems.Config.bank4 && AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, GetString("虚空袋"), listen, mess, List);
        return Stored;
    }
    #endregion

    #region 自动储存物品方法
    public static bool AutoStoredItem(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName, bool listen, bool mess, List<int> List)
    {
        var plr = tplr.TPlayer;

        foreach (var bank in bankItems)
        {
            //遍历背包58格
            for (var i = 0; i < plr.inventory.Length; i++)
            {
                //当前背包的格子
                var inv = plr.inventory[i];

                //开启监听模式，自动把放入存钱罐的物品写入到储物表里
                if (listen)
                {
                    //储物空间不为空，不是钱币，不在储物空间的物品
                    if (!bank.IsAir && !bank.IsACoin && !List.Contains(bank.type))
                    {
                        //添加到储物表
                        List.Add(bank.type);

                        if (mess)
                        {
                            tplr.SendMessage(GetString($"已将 '[c/92C5EC:{bank.Name}]'添加到自动储存表|指令菜单:[c/A1D4C2:/ast]"), 255, 246, 158);
                        }
                    }
                }

                //背包物品在储物表里，背包物品与储物空间物品ID一致，不是手上的物品则触发逻辑
                if (List.Contains(inv.type) && inv.type == bank.type && 
                    inv.type != plr.inventory[plr.selectedItem].type)
                {
                    //把背包内的物品数量 加到 储物空间里的物品数量
                    bank.stack += inv.stack;

                    //移除背包的物品
                    inv.TurnToAir();

                    //如果数量超过指令可获得的最大值，则等于最大值（不写分堆处理，避免刷物品BUG）
                    if (bank.stack >= Item.CommonMaxStack)
                    {
                        bank.stack = Item.CommonMaxStack;
                    }

                    //发包给背包位格移除物品
                    tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, PlayerItemSlotID.Inventory0 + i);

                    //发包给储物空间添加物品，用bankItems来获取准确的储物空间，和它对应的物品（避免多个储物空间存在同样物品触发BUG）
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
