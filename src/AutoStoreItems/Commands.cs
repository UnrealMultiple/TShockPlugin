using Terraria;
using TShockAPI;

namespace AutoStoreItems;

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
            player.SendMessage(GetString("【自动储存】指令菜单 [i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]\n") +
             GetString("/ast —— 查看自动储存菜单\n") +
             GetString("/astreset —— 清空[c/85CFDE:所有玩家]数据\n") +
             GetString("/ast auto —— 开启|关闭[c/89DF85:自动识别]模式\n") +
             GetString("/ast hand —— 开启|关闭[c/F19092:手持识别]模式\n") +
             GetString("/ast armor —— 开启|关闭[c/F2F191:装备识别]模式\n") +
             GetString("/ast list —— [c/85CEDF:列出]自己的[c/85CEDF:自动储存表]\n") +
             GetString("/ast clear —— [c/E488C1:清理]自动储存表\n") +
             GetString("/ast bank —— 监听[c/F3B691:储存空间位格]开关\n") +
             GetString("/ast mess —— 开启|关闭物品[c/F2F292:自存消息]\n") +
             GetString("/ast pm —— 开启|关闭[c/85CFDE:性能模式]\n") +
             GetString("/ast add 或 del 名字 —— [c/87DF86:添加]|[c/F19092:删除]自存物品"), 193, 223, 186);
        }
    }
    #endregion

    #region 重置数据方法
    public static void Reset(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            AutoStoreItems.Data.Items.Clear();
            args.Player.SendSuccessMessage(GetString($"已[c/92C5EC:清空]所有玩家《自动储存》数据！"));
            return;
        }
    }
    #endregion

    public static void Ast(CommandArgs args)
    {
        var name = args.Player.Name;
        var data = AutoStoreItems.Data.Items.FirstOrDefault(item => item.Name == name);

        if (!AutoStoreItems.Config.open)
        {
            return;
        }

        if (data == null)
        {
            args.Player.SendInfoMessage(GetString("请用角色[c/D95065:重进服务器]后输入：/ast 指令查看菜单\n羽学声明：本插件纯属[c/7E93DE:免费]请勿上当受骗", 217, 217, 217));
            return;
        }

        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player);
            args.Player.SendSuccessMessage(GetString($"服务器[c/87DF86:性能模式]：[c/C086DF:{AutoStoreItems.Config.PM}]"));
            args.Player.SendSuccessMessage(GetString($"您的自存[c/BBE9B7:监听]为：[c/E489C0:{data.listen}]"));
            args.Player.SendSuccessMessage(GetString($"您的[c/F2BEC0:自动识别]为：[c/87DF86:{data.AutoMode}]"));
            args.Player.SendSuccessMessage(GetString($"您的[c/BCB7E9:装备识别]为：[c/F29192:{data.ArmorMode}]"));
            args.Player.SendSuccessMessage(GetString($"您的[c/E4EFBC:手持储存]为：[c/C086DF:{data.HandMode}]"));
            args.Player.SendSuccessMessage(GetString($"只有玩家[c/E4EFBC:移动|攻击]才会执行[c/87DF86:自存]"));
            return;
        }
        if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0].ToLower() == "list")
            {
                args.Player.SendInfoMessage(GetString($"[{data.Name}的自动储存表]\n" + string.Join(", ", data.ItemType.Select(x => TShock.Utils.GetItemById(x).Name + "({0})".SFormat(x)))));
                return;
            }

            if (args.Parameters[0].ToLower() == "pm" && args.Player.HasPermission("AutoStore.admin"))
            {
                var isEnabled = AutoStoreItems.Config.PM;
                AutoStoreItems.Config.PM = !isEnabled;
                AutoStoreItems.Config.Write();
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{Mess}]服务器性能模式。"));
                args.Player.SendInfoMessage(GetString($"性能模式:不为[c/C086DF:堆叠达到单格上限]物品进行分堆累积"));
                return;
            }

            if (args.Parameters[0].ToLower() == "auto")
            {
                var isEnabled = data.AutoMode;
                data.AutoMode = !isEnabled;
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{Mess}]自动识别模式。"));
                data.HandMode = false;
                data.ArmorMode = false;
                return;
            }

            if (args.Parameters[0].ToLower() == "armor")
            {
                var isEnabled = data.ArmorMode;
                data.ArmorMode = !isEnabled;
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 已[c/92C5EC:{Mess}]装备识别储存模式。"));
                data.AutoMode = false;
                data.HandMode = false;
                return;
            }

            if (args.Parameters[0].ToLower() == "hand")
            {
                var isEnabled = data.HandMode;
                data.HandMode = !isEnabled;
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 的手持识别模式已[c/92C5EC:{Mess}]"));
                data.AutoMode = false;
                data.ArmorMode = false;
                return;
            }

            if (args.Parameters[0].ToLower() == "clear")
            {
                data.ItemType.Clear();
                args.Player.SendSuccessMessage(GetString($"已清理[c/92C5EC: {args.Player.Name} ]的自动储存表"));
                return;
            }

            if (args.Parameters[0].ToLower() == "bank")
            {
                var isEnabled = data.listen;
                data.listen = !isEnabled;
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 的储物空间位格监听功能已[c/92C5EC:{Mess}]"));
                return;
            }

            if (args.Parameters[0].ToLower() == "mess")
            {
                var isEnabled = data.Mess;
                data.Mess = !isEnabled;
                var Mess = isEnabled ? GetString("禁用") : GetString("启用");
                args.Player.SendSuccessMessage(GetString($"玩家 [{args.Player.Name}] 的自动储存消息已[c/92C5EC:{Mess}]"));
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
                            args.Player.SendErrorMessage(GetString("物品 [c/92C5EC:{0}] 已在自动储存中!"), item.Name);
                            return;
                        }
                        data.ItemType.Add(item.type);
                        args.Player.SendSuccessMessage(GetString("已成功将物品添加到自动储存: [c/92C5EC:{0}]!"), item.Name);
                        break;
                    }

                case "del":
                case "delete":
                case "remove":
                    {
                        if (!data.ItemType.Contains(item.type))
                        {
                            args.Player.SendErrorMessage(GetString("物品 {0} 不在自动储存中!"), item.Name);
                            return;
                        }
                        data.ItemType.Remove(item.type);
                        args.Player.SendSuccessMessage(GetString("已成功从自动储存删除物品: [c/92C5EC:{0}]!"), item.Name);
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
