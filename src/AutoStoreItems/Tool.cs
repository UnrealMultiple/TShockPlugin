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
        Stored |= Configuration.Instance.bank1 && AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, GetString("[c/32CEB7:存钱罐]"), listen, mess, List);
        Stored |= Configuration.Instance.bank2 && AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, GetString("[c/32CEB7:保险箱]"), listen, mess, List);
        Stored |= Configuration.Instance.bank3 && AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, GetString("[c/32CEB7:护卫熔炉]"), listen, mess, List);
        Stored |= Configuration.Instance.bank4 && AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, GetString("[c/32CEB7:虚空袋]"), listen, mess, List);
        return Stored;
    }
    #endregion

    #region 自动储存物品方法
    public static bool AutoStoredItem(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName, bool listen, bool mess, List<int> List)
    {
        var plr = tplr.TPlayer;

        foreach (var bank in bankItems)
        {
            // 遍历背包58格
            for (var i = 0; i < plr.inventory.Length; i++)
            {
                // 跳过钱币栏格子
                if (i > 50 && i < 54)
                {
                    continue;
                }

                // 当前背包的格子
                var inv = plr.inventory[i];

                //把放入存钱罐的物品自动写进玩家自己的储物数据表里
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

                // 储物表里有背包物品，背包物品与存钱罐物品ID一致，且不是手上的物品则触发逻辑
                if (List.Contains(inv.type) && inv.type == bank.type && inv.type != plr.inventory[plr.selectedItem].type)
                {
                    //性能模式开关
                    if (Configuration.Instance.PM)
                    {
                        PerformanceMode(tplr, bankItems, bankSlot, bankName, mess, bank, i, inv);
                    }

                    else //做了详细的分堆处理
                    {
                        HandlePiledItems(tplr, bankItems, bankSlot, bankName, mess, bank, i, inv);
                    }

                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region 性能模式：不处理分堆，只发2次包
    private static void PerformanceMode(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName, bool mess, Item bank, int i, Item inv)
    {
        bank.stack += inv.stack;

        //移除背包的物品
        inv.TurnToAir();

        //如果数量超过最大上限，则等于最大值（不写分堆处理，避免刷物品BUG）
        if (bank.stack >= bank.maxStack)
        {
            bank.stack = bank.maxStack;
        }

        //发包给背包位格移除物品
        tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, PlayerItemSlotID.Inventory0 + i);

        //发包给储物空间添加物品，用bankItems来获取准确的储物空间，和它对应的物品（避免多个储物空间存在同样物品触发BUG）
        tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + Array.IndexOf(bankItems, bank));

        if (mess)
        {
            tplr.SendMessage(GetString($"已将'[c/92C5EC:{bank.Name}]'存入您的{bankName} 当前数量: {bank.stack}"), 255, 246, 158);
        }
    }
    #endregion

    #region 性能怪兽版：处理分堆物品，最多发4次包
    private static void HandlePiledItems(TSPlayer tplr, Item[] bankItems, int bankSlot, string bankName, bool mess, Item bank, int i, Item inv)
    {
        while (inv.stack > 0)
        {
            //避免消息刷屏 加个标识
            var Sent = false;

            // 计算存钱罐中该物品还能存储多少个，但不超过背包中该物品的数量
            var canStore = Math.Min(bank.maxStack - bank.stack, inv.stack);

            // 将计算出的物品数量加到存钱罐里
            bank.stack += canStore;

            // 从背包中减去已经存储的数量
            inv.stack -= canStore;

            if (mess && !Sent)
            {
                tplr.SendMessage(GetString($"已将 '[c/92C5EC:{inv.Name}]' 存入您的 {bankName} 当前数量: {bank.stack}"), 255, 246, 158);
                Sent = true;
            }

            // 发包给存钱罐添加物品
            tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + Array.IndexOf(bankItems, bank));

            if (inv.stack > 0)
            {
                var flag = false;

                // 尝试将剩余物品合并到其他储物空间中的相同物品
                for (var j = 0; j < bankItems.Length; j++)
                {
                    var otherBank = bankItems[j];
                    if (otherBank.type == inv.type && otherBank.stack < otherBank.maxStack)
                    {
                        // 计算可以添加到其他储物空间的物品数量
                        var AddStack = Math.Min(inv.stack, otherBank.maxStack - otherBank.stack);

                        // 更新其他储物空间物品数量
                        otherBank.stack += AddStack;

                        // 减少背包物品数量
                        inv.stack -= AddStack;

                        if (mess && !Sent)
                        {
                            tplr.SendMessage(GetString($"已将 '[c/92C5EC:{inv.Name}]' 存入您的 {bankName} 当前数量: {bank.stack}"), 255, 246, 158);
                            Sent = true;
                        }

                        // 发包更新客户端的储物空间物品状态
                        tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + j);

                        // 如果背包中的物品数量已经为0，设置标志为true并跳出循环
                        if (inv.stack <= 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                }

                if (!flag)
                {
                    // 如果背包还有剩余物品，再找空位
                    var Slot = -1;

                    // 遍历所有储物空间
                    for (var j = 0; j < bankItems.Length; j++)
                    {
                        // 找到空位
                        if (bankItems[j].IsAir)
                        {
                            // 索引更新为当前格子
                            Slot = j;
                            break;
                        }
                    }

                    // 在当前空位
                    if (Slot != -1)
                    {
                        // 创建新的物品实例
                        var newItem = new Item();
                        newItem.SetDefaults(inv.type);

                        // 每次最多存到该物品的上限，最少存完背包物品数量
                        newItem.stack = Math.Min(newItem.maxStack, inv.stack);

                        // 减少背包物品数量
                        inv.stack -= newItem.stack;

                        // 将新的物品添加到空格上
                        bankItems[Slot] = newItem;

                        if (mess && !Sent)
                        {
                            tplr.SendMessage(GetString($"已将 '[c/92C5EC:{newItem.Name}]' 存入您的 {bankName} 当前数量: {bank.stack}"), 255, 246, 158);
                            Sent = true;
                        }

                        // 发包给当前储物空间的空位添加物品
                        tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, bankSlot + Slot);

                        // 背包没东西了 返回
                        if (inv.stack <= 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        // 没有空位了 警告返回
                        if (mess && !Sent)
                        {
                            tplr.SendMessage(GetString($"您的 {bankName} 已经满了，无法继续存入物品。"), 255, 246, 158);
                            Sent = true;
                        }
                        break;
                    }
                }
            }
        }

        // 背包数量减完了 移除
        if (inv.stack <= 0)
        {
            // 移除背包的物品
            inv.TurnToAir();

            // 发包给玩家背包移除物品
            tplr.SendData(PacketTypes.PlayerSlot, null, tplr.Index, PlayerItemSlotID.Inventory0 + i);
        }
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
            if (bankItem.IsAir || bankItem.type == coin)
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
                if (invItem.type == coin)
                {
                    invItem.type = 0;
                    tplr.SendData(PacketTypes.PlayerSlot, "", tplr.Index, i);

                    bankItem.type = coin;
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
