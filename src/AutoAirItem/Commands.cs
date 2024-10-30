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
             GetString("/air add 或 del 名字 —— [c/87DF86:添加]|[c/F19092:删除]垃圾桶物品", 193, 223, 186));
        }
    }
    #endregion

    #region 重置数据方法
    public static void Reset(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            AutoAirItem.Data.Items.Clear();
            args.Player.SendSuccessMessage(GetString($"已[c/92C5EC:清空]所有玩家《自动垃圾桶》数据！"));
            return;
        }
    }
    #endregion

    public static void AirCmd(CommandArgs args)
    {
        var name = args.Player.Name;
        var data = AutoAirItem.Data.Items.FirstOrDefault(item => item.Name == name);

        if (!AutoAirItem.Config.Open)
        {
            return;
        }

        if (data == null)
        {
            args.Player.SendInfoMessage(GetString("请用角色[c/D95065:重进服务器]后输入：/air 指令查看菜单\n羽学声明：本插件纯属[c/7E93DE:免费]请勿上当受骗"), 217, 217, 217);
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
                args.Player.SendInfoMessage(GetString($"[{data.Name}的垃圾桶]\n") + string.Join(", ", data.ItemType.Select(x => "[c/92C5EC:{0}]".SFormat(x))));
                return;
            }

            if (args.Parameters[0].ToLower() == "on")
            {
                var isEnabled = data.Enabled;
                data.Enabled = !isEnabled;
                var Mess = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{Mess}]自动垃圾桶功能。"));
                return;
            }

            if (args.Parameters[0].ToLower() == "clear")
            {
                data.ItemType.Clear();
                args.Player.SendSuccessMessage(GetString($"已清理[c/92C5EC: {args.Player.Name} ]的自动垃圾桶表"));
                return;
            }

            if (args.Parameters[0].ToLower() == "yes")
            {
                data.ItemType.Add(args.TPlayer.inventory[args.TPlayer.selectedItem].type);
                args.Player.SendSuccessMessage(GetString("手选物品 [c/92C5EC:{0}] 已加入自动垃圾桶中! 脱手即清!"), args.TPlayer.inventory[args.TPlayer.selectedItem].Name);
                return;
            }

            if (args.Parameters[0].ToLower() == "auto")
            {
                var isEnabled = data.Auto;
                data.Auto = !isEnabled;
                var Mess = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 的垃圾桶位格监听功能已[c/92C5EC:{Mess}]"));
                return;
            }

            if (args.Parameters[0].ToLower() == "mess")
            {
                var isEnabled = data.Mess;
                data.Mess = !isEnabled;
                var Mess = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 的自动清理消息已[c/92C5EC:{Mess}]"));
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
                        data.ItemType.Remove(item.type);
                        args.Player.SendSuccessMessage(GetString("已成功从垃圾桶删除物品: [c/92C5EC:{0}]!"), item.Name);
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
