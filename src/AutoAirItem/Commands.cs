using Terraria;
using TShockAPI;
using static AutoAirItem.AutoAirItem;

namespace AutoAirItem;

public class Commands
{
    #region 主体指令
    public static void AirCmd(CommandArgs args)
    {
        var plr = args.Player;
        var data = Data.Items.FirstOrDefault(x => x.Name == plr.Name);

        if (!Configuration.Instance.Enabled)
        {
            return;
        }

        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player, data);
            return;
        }

        if (args.Parameters.Count == 1)
        {
            if (data != null)
            {
                if (args.Parameters[0].ToLower() == "on")
                {
                    if (data.Enabled)
                    {
                        plr.SendSuccessMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:关闭]自动垃圾桶功能。"));
                        data.Enabled = false;
                    }
                    else
                    {
                        plr.SendSuccessMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:开启]自动垃圾桶功能。"));
                        data.Enabled = true;
                    }
                    DB.UpdateData(data);
                    return;
                }

                if (args.Parameters[0].ToLower() == "list")
                {
                    var text = string.Join(", ", data.TrashList.Select(x =>
                        Lang.GetItemName(x.Key) + "([c/92C5EC:{0}])".SFormat(x.Value)));
                    plr.SendInfoMessage(GetString($"[{data.Name}的垃圾桶]"));
                    plr.SendInfoMessage(text);
                    return;
                }

                if (args.Parameters[0].ToLower() == "clear")
                {
                    data.TrashList.Clear();
                    plr.SendSuccessMessage(GetString($"已清理[c/92C5EC: {plr.Name} ]的自动垃圾桶表"));
                    DB.UpdateData(data); // 更新数据库
                    return;
                }

                if (args.Parameters[0].ToLower() == "mess")
                {
                    if (data.Mess)
                    {
                        plr.SendSuccessMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:关闭]垃圾桶提示功能。"));
                        data.Mess = false;
                    }
                    else
                    {
                        plr.SendSuccessMessage(GetString($"玩家 [{plr.Name}] 已[c/92C5EC:开启]垃圾桶提示功能。"));
                        data.Mess = true;
                    }
                    DB.UpdateData(data); // 更新数据库
                    return;
                }
            }

            if (args.Parameters[0].ToLower() == "reset")
            {
                Reset(args);
                return;
            }
        }

        if (args.Parameters.Count == 2)
        {
            var item = new Terraria.Item();

            if (TShock.Utils.GetItemByIdOrName(args.Parameters[1]).Count > 0)
            {
                item = TShock.Utils.GetItemByIdOrName(args.Parameters[1])[0];
            }

            switch (args.Parameters[0].ToLower())
            {
                case "del":
                case "delete":
                case "remove":
                {
                    if (data != null)
                    {
                        if (!data.TrashList.ContainsKey(item.type))
                        {
                            plr.SendErrorMessage(GetString("物品 {0} 不在垃圾桶中!"), item.Name);
                            return;
                        }

                        // 返还被清理的物品（字典存在玩家自己的数据里，键=物品ID，值则是在这个ID下的物品数量）
                        var TrashItem = data.TrashList.FirstOrDefault(x => x.Key == item.type);
                        plr.SendSuccessMessage(GetString($"已从垃圾桶移出:[c/DEFF7D:{TrashItem.Value}]个[i/s{1}:{TrashItem.Key}]"));

                        //如果找到了对应的物品 当它数量超过 最大堆叠数时
                        if (TrashItem.Value > item.maxStack)
                        {
                            //每次发放一组 一组为这个物品的最大堆叠数
                            for (var i = 0; i < TrashItem.Value; i += item.maxStack)
                            {
                                //物品ID为“自动垃圾桶物品表”里的键名 数量为:最多发到物品上限 最少发一组
                                plr.GiveItem(TrashItem.Key, Math.Min(item.maxStack, TrashItem.Value - i));
                            }
                        }
                        else //数量少于最大堆叠数 则全部发完（到了这里就是找零用的）
                        {
                            plr.GiveItem(TrashItem.Key, TrashItem.Value, 0);
                        }

                        data.TrashList.Remove(TrashItem.Key);
                        DB.UpdateData(data); // 更新数据库
                    }
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
                    HelpCmd(args.Player, data);
                    break;
                }
            }
        }
    }
    #endregion

    #region 菜单方法
    private static void HelpCmd(TSPlayer plr, MyData.PlayerData? data)
    {
        if (plr == null)
        {
            return;
        }

        if (plr.HasPermission("AutoAir.admin"))
        {
            if (plr != TSPlayer.Server)
            {
                if (data != null)
                {
                    plr.SendInfoMessage(GetString("【自动垃圾桶】指令菜单 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]"));
                    plr.SendInfoMessage(GetString("/air on —— 开启|关闭[c/89DF85:垃圾桶]功能"));
                    plr.SendInfoMessage(GetString("/air list —— [c/F19092:列出]自己的[c/F2F191:垃圾桶]"));
                    plr.SendInfoMessage(GetString("/air del 名字 —— 将物品从自动垃圾桶[c/F19092:取出]"));
                    plr.SendInfoMessage(GetString("/air clear —— [c/85CEDF:清理]垃圾桶"));
                    plr.SendInfoMessage(GetString("/air mess —— 开启|关闭[c/F2F292:清理消息]"));
                    plr.SendInfoMessage(GetString("/air ck 数量—— 筛选出物品超过此数量的玩家"));
                    plr.SendInfoMessage(GetString("/air reset —— 清空[c/85CFDE:所有玩家]数据"));
                    if (!data.Enabled)
                    {
                        plr.SendSuccessMessage(GetString($"请输入该指令开启→: [c/92C5EC:/air on] "));
                    }
                }
            }
            else
            {
                plr.SendInfoMessage(GetString("【自动垃圾桶】指令菜单"));
                plr.SendInfoMessage(GetString("/air ck 数量—— 筛选出物品超过此数量的玩家"));
                plr.SendInfoMessage(GetString("/air reset —— 清空[c/85CFDE:所有玩家]数据"));
                plr.SendSuccessMessage(GetString($"其余指令需要您进入游戏内才会显示"));
            }
        }
        else
        {
            if (data != null)
            {
                plr.SendInfoMessage(GetString("【自动垃圾桶】指令菜单 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]"));
                plr.SendInfoMessage(GetString("/air on —— 开启|关闭[c/89DF85:垃圾桶]功能"));
                plr.SendInfoMessage(GetString("/air list —— [c/F19092:列出]自己的[c/F2F191:垃圾桶]"));
                plr.SendInfoMessage(GetString("/air del 名字 —— 将物品从自动垃圾桶[c/F19092:取出]"));
                plr.SendInfoMessage(GetString("/air clear —— [c/85CEDF:清理]垃圾桶"));
                plr.SendInfoMessage(GetString("/air ck 数量 —— 筛选出物品超过此数量的玩家"));
                plr.SendInfoMessage(GetString("/air mess —— 开启|关闭[c/F2F292:清理消息]"));

                if (!data.Enabled)
                {
                    plr.SendSuccessMessage(GetString($"请输入该指令开启→: [c/92C5EC:/air on] "));
                }
            }
        }
    }
    #endregion

    #region 重置数据方法
    public static void Reset(CommandArgs args)
    {
        if (!args.Player.HasPermission("AutoAir.admin"))
        {
            args.Player.SendErrorMessage(GetString("你没有权限使用此指令！"));
            return;
        }
        else
        {
            Data.Items.Clear(); // 清空内存数据
            DB.ClearData();
            args.Player.SendSuccessMessage(GetString($"已[c/92C5EC:清空]所有玩家《自动垃圾桶》数据！"));
        }
    }
    #endregion

    #region 检查物品数量方法
    public static void CheckCmd(CommandArgs args, int num)
    {
        var flag = false;
        args.Player.SendInfoMessage(GetString($"以下垃圾物品数量超过【[c/92C5EC:{num}]】的玩家"));

        var plrs = new List<(int Index, string Name, List<(string ItemName, int Count)> ExcessItems)>();

        var index = 1;

        var db = AutoAirItem.DB.GetAllData(); // 调用数据库查询方法

        foreach (var data in db)
        {
            var Items = data.TrashList.Where(pair => pair.Value >= num).Select(pair =>
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
                args.Player.SendMessage($"[c/32CEB7:{plr.Index}.][c/F3E83B:{plr.Name}:] " +
                    $"{string.Join(", ", plr.ExcessItems.Select(item => $"{item.ItemName}([c/92C5EC:{item.Count}])"))}", 193, 223, 186);
            }
        }
        else
        {
            args.Player.SendMessage(GetString($"没有找到垃圾数量超过[c/92C5EC:{num}]的玩家"), 193, 223, 186);
        }
    }
    #endregion
}
