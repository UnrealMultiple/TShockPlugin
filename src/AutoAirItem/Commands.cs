using Terraria;
using TShockAPI;

namespace AutoAirItem;

public class Commands
{
    #region 菜单方法
    private static void HelpCmd(TSPlayer player)
    {
        if (player == null)
        {
            return;
        }
        else
        {
            player.SendInfoMessage(GetString("【自动垃圾桶】指令菜单 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]\n") +
             GetString("/air —— 查看垃圾桶菜单\n") +
             GetString("/airreset —— 清空[c/85CFDE:所有玩家]数据\n") +
             GetString("/air on —— 开启|关闭[c/89DF85:垃圾桶]功能\n") +
             GetString("/air list —— [c/F19092:列出]自己的[c/F2F191:垃圾桶]\n") +
             GetString("/air clear —— [c/85CEDF:清理]垃圾桶\n") +
             GetString("/air yes —— 将[c/E488C1:手持物品]加入垃圾桶\n") +
             GetString("/air auto —— 监听[c/F3B691:垃圾桶位格]开关\n") +
             GetString("/air mess —— 开启|关闭[c/F2F292:清理消息]\n") +
             GetString("/air ck 数量—— 筛选出物品超过此数量的玩家\n") +
             GetString("/air add 或 del 名字 —— [c/87DF86:添加]|[c/F19092:移出]垃圾桶物品", 193, 223, 186));
        }
    }
    #endregion

    #region 重置数据方法
    public static void Reset(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            AutoAirItem.Data.Items.Clear();
            AutoAirItem.DB.ClearData();
            args.Player.SendSuccessMessage(GetString($"已[c/92C5EC:清空]所有玩家《自动垃圾桶》数据！"));
            return;
        }
    }
    #endregion

    #region 检查物品数量方法
    public static void CheckCmd(CommandArgs args, int num)
    {
        if (!Configuration.Instance.Enable)
        {
            return;
        }

        var flag = false;
        args.Player.SendInfoMessage(GetString($"以下垃圾物品数量超过【[c/92C5EC:{num}]】的玩家"));

        var plrs = new List<(int Index, string Name, List<(string ItemName, int Count)> ExcessItems)>();

        var index = 1;

        var db = AutoAirItem.DB.LoadData(); // 调用数据库查询方法

        foreach (var data in db)
        {
            var Items = data.DelItem.Where(pair => pair.Value >= num).Select(pair =>
                (ItemName: TShock.Utils.GetItemById(pair.Key).Name, Count: pair.Value)).ToList();

            if (Items.Any())
            {
                flag = true;
                plrs.Add((index++, data.Name, Items.ToList()));
            }
        }

        if (flag)
        {
            foreach (var plr in plrs)
            {
                args.Player.SendMessage(GetString($"[c/32CEB7:{plr.Index}.][c/F3E83B:{plr.Name}:] " +
                    $"{string.Join(", ", plr.ExcessItems.Select(item => $"{item.ItemName}([c/92C5EC:{item.Count}])"))}"), 193, 223, 186);
            }
        }
        else
        {
            args.Player.SendMessage(GetString($"没有找到垃圾数量超过[c/92C5EC:{num}]的玩家"), 193, 223, 186);
        }
    }
    #endregion

    public static void AirCmd(CommandArgs args)
    {
        var name = args.Player.Name;
        var data = AutoAirItem.Data.Items.FirstOrDefault(item => item.Name == name);

        if (!Configuration.Instance.Enable)
        {
            return;
        }

        if (data == null)
        {
            args.Player.SendMessage(GetString("请用角色[c/D95065:重进服务器]后输入：/air 指令查看菜单\n羽学声明：本插件纯属[c/7E93DE:免费]请勿上当受骗"), 217, 217, 217);
            return;
        }

        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player);
            if (!data.Enabled)
            {
                args.Player.SendSuccessMessage(GetString($"请输入该指令开启→: [c/92C5EC:/air on] "));
            }
            else
            {
                args.Player.SendSuccessMessage(GetString($"您的垃圾桶[c/F2BEC0:监听状态]为：[c/F3F292:{data.Auto}]"));
                args.Player.SendSuccessMessage(GetString($"输入指令[c/E4EFBC:切换]自动模式：[c/C086DF:/air auto]"));
            }
            return;
        }
        if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0].ToLower() == "list")
            {
                args.Player.SendInfoMessage(GetString($"[{data.Name}的垃圾桶]\n") + string.Join(", ", data.ItemType.Select(x => TShock.Utils.GetItemById(x).Name + "([c/92C5EC:{0}])".SFormat(x))));
                return;
            }

            if (args.Parameters[0].ToLower() == "on")
            {
                data.Enabled = !data.Enabled;
                args.Player.SendSuccessMessage(data.Enabled ?
                    GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:启用]自动垃圾桶功能。") :
                    GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:禁用]自动垃圾桶功能。"));
                AutoAirItem.DB.UpdateData(data); // 更新数据库
                return;
            }

            if (args.Parameters[0].ToLower() == "clear")
            {
                data.ItemType.Clear();
                data.DelItem.Clear();
                args.Player.SendSuccessMessage(GetString($"已清理[c/92C5EC: {args.Player.Name} ]的自动垃圾桶表"));
                AutoAirItem.DB.UpdateData(data); // 更新数据库
                return;
            }

            if (args.Parameters[0].ToLower() == "yes")
            {
                data.ItemType.Add(args.TPlayer.inventory[args.TPlayer.selectedItem].type);
                args.Player.SendSuccessMessage(GetString("手选物品 [c/92C5EC:{0}] 已加入自动垃圾桶中! 脱手即清!"), args.TPlayer.inventory[args.TPlayer.selectedItem].Name);
                AutoAirItem.DB.UpdateData(data); // 更新数据库
                return;
            }

            if (args.Parameters[0].ToLower() == "auto")
            {
                data.Auto = !data.Auto;
                args.Player.SendSuccessMessage(data.Auto ?
                    GetString($"玩家 [{args.Player.Name}] 的垃圾桶位格监听功能已[c/92C5EC:启用]") :
                    GetString($"玩家 [{args.Player.Name}] 的垃圾桶位格监听功能已[c/92C5EC:禁用]"));
                AutoAirItem.DB.UpdateData(data); // 更新数据库
                return;
            }

            if (args.Parameters[0].ToLower() == "mess")
            {
                data.Mess = !data.Mess;
                args.Player.SendSuccessMessage(data.Mess ?
                    GetString($"玩家 [{args.Player.Name}] 的自动清理消息已[c/92C5EC:启用]") :
                    GetString($"玩家 [{args.Player.Name}] 的自动清理消息已[c/92C5EC:禁用]"));
                AutoAirItem.DB.UpdateData(data); // 更新数据库
                return;
            }
        }

        if (args.Parameters.Count == 2)
        {
            Item item;
            var Items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (Items.Count > 1)
            {
                args.Player.SendMultipleMatchError(Items.Select(i => i.Name));
                return;
            }

            if (Items.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("不存在该物品，\"物品查询\": \"[c/92C5EC:https://terraria.wiki.gg/zh/wiki/Item_IDs]\""));
                return;
            }

            else
            {
                item = Items[0];
            }

            switch (args.Parameters[0].ToLower())
            {
                case "add":
                {
                    if (data.ItemType.Contains(item.type))
                    {
                        args.Player.SendErrorMessage(GetString("物品 [c/92C5EC:{0}] 已在垃圾桶中!"), item.Name);
                        return;
                    }
                    data.ItemType.Add(item.type);
                    args.Player.SendSuccessMessage(GetString("已成功将物品添加到垃圾桶: [c/92C5EC:{0}]!"), item.Name);
                    AutoAirItem.DB.UpdateData(data); // 更新数据库
                    break;
                }

                case "del":
                case "delete":
                case "remove":
                {
                    if (!data.ItemType.Contains(item.type))
                    {
                        args.Player.SendErrorMessage(GetString("物品 {0} 不在垃圾桶中!"), item.Name);
                        return;
                    }

                    // 返还被清理的物品（字典存在玩家自己的数据里，键=物品ID，值则是在这个ID下的物品数量）
                    var del = data.DelItem.FirstOrDefault(x => x.Key == item.type);

                    args.Player.SendSuccessMessage(GetString("已从垃圾桶移出:[c/DEFF7D:{1}]个[c/92C5EC:{0}]"), item.Name, del.Value);

                    //从找到的ID里判断:值大于堆叠数
                    if (del.Value > item.maxStack)
                    {
                        //最大堆叠数为1的物品
                        if (item.maxStack != 9999)
                        {
                            // 分批发放，每次发一个
                            for (var i = 0; i < del.Value; i += item.maxStack)
                            {
                                // 计算本次应发放的数量
                                var toGive = Math.Min(item.maxStack, del.Value - i);
                                args.Player.GiveItem(del.Key, toGive, 0);
                            }
                        }
                        // 堆叠上限为9999的物品
                        else
                        {
                            //分批发放，每次发9999个
                            for (var i = 0; i < del.Value; i += 9999)
                            {
                                // 计算本次应发放的数量
                                var toGive = Math.Min(9999, del.Value - i);
                                args.Player.GiveItem(del.Key, toGive, 0);
                            }
                        }
                    }

                    //不超上限直接给（发完堆叠上限的次数后 再轮到它就是补零头用的了）
                    else
                    {
                        args.Player.GiveItem(del.Key, del.Value, 0);
                    }

                    data.DelItem.Remove(del.Key);
                    data.ItemType.Remove(item.type);
                    AutoAirItem.DB.UpdateData(data); // 更新数据库
                    break;
                }

                case "ck":
                {
                    if (int.TryParse(args.Parameters[1], out var num))
                    {
                        CheckCmd(args, num);
                    }
                    break;
                }

                default:
                {
                    HelpCmd(args.Player);
                    break;
                }
            }
        }
    }

}
