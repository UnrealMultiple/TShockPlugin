using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace AutoAirItem;

public class Commands
{
    public static void AirCmd(CommandArgs args)
    {
        var name = args.Player.Name;
        var data = AutoAirItem.data.Items.FirstOrDefault(item => item.Name == name);

        if (!AutoAirItem.Config.Open)
        {
            return;
        }

        if (data == null)
        {
            args.Player.SendInfoMessage("请用角色[c/D95065:重进服务器]后输入：/air 指令查看菜单\n羽学声明：本插件纯属[c/7E93DE:免费]请勿上当受骗", 217,217,217);
            return;
        }

        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player);
            if (!data.Enabled)
            {
                args.Player.SendSuccessMessage($"请输入该指令开启→: [c/92C5EC:/air on] ");
            }
            else
            {
                args.Player.SendSuccessMessage($"您的垃圾桶监听状态为：[c/92C5EC:{data.Auto}]");
                args.Player.SendSuccessMessage($"输入指令切换自动模式：[c/92C5EC:/air auto]");
            }
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            args.Player.SendInfoMessage($"[{data.Name}的垃圾桶]\n" + string.Join(", ", data.ItemName.Select(x => "[c/92C5EC:{0}]".SFormat(x))));
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "on")
        {
            var isEnabled = data.Enabled;
            data.Enabled = !isEnabled;
            var Mess = isEnabled ? "禁用" : "启用";
            args.Player.SendSuccessMessage($"玩家 [{args.Player.Name}] 已[c/92C5EC:{Mess}]自动垃圾桶功能。");
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "clear")
        {
            data.ItemName.Clear();
            args.Player.SendSuccessMessage($"已清理[c/92C5EC: {args.Player.Name} ]的自动垃圾桶表");
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "yes")
        {
            data.ItemName.Add(args.TPlayer.inventory[args.TPlayer.selectedItem].Name);
            args.Player.SendSuccessMessage("手选物品 [c/92C5EC:{0}] 已加入自动垃圾桶中! 脱手即清!", args.TPlayer.inventory[args.TPlayer.selectedItem].Name);
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "auto")
        {
            var isEnabled = data.Auto;
            data.Auto = !isEnabled;
            var Mess = isEnabled ? "禁用" : "启用";
            args.Player.SendSuccessMessage($"玩家 [{args.Player.Name}] 的垃圾桶位格监听功能已[c/92C5EC:{Mess}]");
            return;
        }

        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "mess")
        {
            var isEnabled = data.Mess;
            data.Mess = !isEnabled;
            var Mess = isEnabled ? "禁用" : "启用";
            args.Player.SendSuccessMessage($"玩家 [{args.Player.Name}] 的自动清理消息已[c/92C5EC:{Mess}]");
            return;
        }

        if (args.Parameters.Count == 2)
        {
            Item item;
            List<Item> Items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (Items.Count > 1)
            {
                args.Player.SendMultipleMatchError(Items.Select(i => i.Name));
                return;
            }

            if (Items.Count == 0)
            {
                args.Player.SendErrorMessage("不存在该物品，\"物品查询\": \"[c/92C5EC:https://terraria.wiki.gg/zh/wiki/Item_IDs]\"");
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
                        if (data.ItemName.Contains(item.Name))
                        {
                            args.Player.SendErrorMessage("物品 [c/92C5EC:{0}] 已在垃圾桶中!", item.Name);
                            return;
                        }
                        data.ItemName.Add(item.Name);
                        args.Player.SendSuccessMessage("已成功将物品添加到垃圾桶: [c/92C5EC:{0}]!", item.Name);
                        break;
                    }
                case "delete":
                case "del":
                case "remove":
                    {
                        if (!data.ItemName.Contains(item.Name))
                        {
                            args.Player.SendErrorMessage("物品 {0} 不在垃圾桶中!", item.Name);
                            return;
                        }
                        data.ItemName.Remove(item.Name);
                        args.Player.SendSuccessMessage("已成功从垃圾桶删除物品: [c/92C5EC:{0}]!", item.Name);
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

    #region 菜单方法
    private static void HelpCmd(TSPlayer player)
    {
        if (player == null) return;
        else
        {
            player.SendMessage("【自动垃圾桶】指令菜单\n" +
             "/air —— 查看垃圾桶菜单\n" +
             "/air on —— 开启|关闭垃圾桶功能\n" +
             "/air list —— 列出自己的垃圾桶\n" +
             "/air clear —— 清理垃圾桶\n" +
             "/air yes —— 将手持物品加入垃圾桶\n" +
             "/air auto —— 监听垃圾桶位格开关\n" +
             "/air mess —— 开启|关闭清理消息\n" +
             "/air add 或 del 物品名字 —— 添加|删除《自动垃圾桶》的物品", Color.AntiqueWhite);
        }
    }
    #endregion


}
