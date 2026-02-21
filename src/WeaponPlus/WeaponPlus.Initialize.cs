using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TerrariaApi.Server;
using TShockAPI;

namespace WeaponPlus;

public partial class WeaponPlus : TerrariaPlugin
{

    #region 问候玩家
    private void OnGreetPlayer(GreetPlayerEventArgs args)
    {
        if (args == null)
        {
            return;
        }
        wPlayers[args.Who] = new WPlayer(TShock.Players[args.Who])
        {
            hasItems = DB.ReadDBGetWItemsFromOwner(TShock.Players[args.Who].Name).ToList()
        };
        if (!config.WhetherToTurnOnAutomaticLoadWeaponsWhenEnteringTheGame)
        {
            return;
        }
        foreach (var hasItem in wPlayers[args.Who].hasItems)
        {
            ReplaceWeaponsInBackpack(Main.player[args.Who], hasItem);
        }
    }
    #endregion

    #region 离开服务器
    private void OnServerLeave(LeaveEventArgs args)
    {
        if (args == null || TShock.Players[args.Who] == null)
        {
            return;
        }
        try
        {
            if (wPlayers[args.Who] != null)
            {
                DB.WriteDB(wPlayers[args.Who].hasItems.ToArray());
            }
        }
        catch
        {
        }
    }
    #endregion

    #region 强化物品
    private void PlusItem(CommandArgs args)
    {
        var text = LangTipsGet("几乎所有的武器和弹药都能强化，但是强化结果会无效化词缀，作为补偿，前三次强化价格降低 80%") +
            "\n" + LangTipsGet("强化绑定一类武器，即同 ID 武器，而不是单独的一个物品。强化与人物绑定，不可分享，扔出即失效，只在背包，猪猪等个人私有库存内起效。") +
            "\n" + LangTipsGet("当你不小心扔出或其他原因导致强化无效，请使用指令 /plus load 来重新获取。每次重新获取都会从当前背包中查找并强制拿出来重给，请注意捡取避免丢失。") +
            "\n" + LangTipsGet("重新获取时重给的物品是单独给予，不会被其他玩家捡走，每次进入服务器时会默认强制重新获取。") +
            "\n" + LangTipsGet("第一个物品栏是强化栏，指令只对该物品栏内的物品起效，强化完即可将武器拿走换至其他栏位，功能类似于哥布林的重铸槽。");
        var text2 = LangTipsGet("输入 /plus    查看当前该武器的等级状态和升至下一级需要多少材料") +
            "\n" + LangTipsGet("输入 /plus load    将当前身上所有已升级的武器重新获取") +
            "\n" + LangTipsGet("输入 /plus [damage/da/伤害] [up/down] [num]   升级/降级当前武器的伤害等级") +
            "\n" + LangTipsGet("输入 /plus [scale/sc/大小] [up/down] [num]  升级/降级当前武器或射弹的体积等级 ±5%") +
            "\n" + LangTipsGet("输入 /plus [knockback/kn/击退] [up/down] [num]   升级/降级当前武器的击退等级 ±5%") +
            "\n" + LangTipsGet("输入 /plus [usespeed/us/用速] [up/down] [num]   升级/降级当前武器的使用速度等级") +
            "\n" + LangTipsGet("输入 /plus [shootspeed/sh/飞速] [up/down] [num]   升级/降级当前武器的射弹飞行速度等级，影响鞭类武器范围±5%") +
            "\n" + LangTipsGet("输入 /plus clear    清理当前武器的所有等级，可以回收一点消耗物") +
            "\n" + LangTipsGet("输入 /clearallplayersplus    将数据库中所有玩家的所有强化物品全部清理，管理员专属");

        if (args.Parameters.Count == 1 && args.Parameters[0].Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            if (!args.Player.Active)
            {
                args.Player.SendInfoMessage(text + "\n" + text2);
                return;
            }
            args.Player.SendMessage(text, new Color(255, 128, 255));
            args.Player.SendMessage(text2, this.getRandColor());
            return;
        }
        if (!args.Player.Active)
        {
            args.Player.SendInfoMessage(LangTipsGet("该指令必须在游戏内使用"));
            return;
        }
        if (!TShock.ServerSideCharacterConfig.Settings.Enabled)
        {
            args.Player.SendInfoMessage(LangTipsGet("SSC 未开启"));
            return;
        }
        var wPlayer = wPlayers[args.Player.Index];
        var firstItem = args.Player.TPlayer.inventory[0];
        var select = wPlayer.hasItems.Find((WItem x) => x.id == firstItem.type)!;
        select ??= new WItem(firstItem.type, args.Player.Name);
        if ((firstItem == null || firstItem.IsAir || TShock.Utils.GetItemById(firstItem.type).damage <= 0 || firstItem.accessory || firstItem.type == 0) && (args.Parameters.Count != 1 || !args.Parameters[0].Equals("load", StringComparison.OrdinalIgnoreCase)))
        {
            args.Player.SendInfoMessage(LangTipsGet("请在第一个物品栏内放入武器而不是其他什么东西或空"));
        }
        else if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage($"{LangTipsGet("当前物品：")}[i:{firstItem!.type}]   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
        }
        else if (args.Parameters.Count == 1)
        {
            if (args.Parameters[0].Equals("load", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var hasItem in wPlayer.hasItems)
                {
                    ReplaceWeaponsInBackpack(args.Player.TPlayer, hasItem);
                }
                args.Player.SendInfoMessage(LangTipsGet("您当前的升级武器已重新读取"));
            }
            else if (args.Parameters[0].Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                if (select.Level == 0)
                {
                    args.Player.SendInfoMessage(LangTipsGet("当前武器没有任何等级，不用回炉重做"));
                    return;
                }
                var text3 = cointostring((long) (select.allCost * config.ResetTheWeaponReturnMultiple), out var items2);
                foreach (var item in items2)
                {
                    var num = Item.NewItem(new EntitySource_DebugCommand(), args.Player.TPlayer.Center, new Vector2(5f, 5f), item.type, item.stack, true, 0, true);
                    Main.item[num].playerIndexTheItemIsReservedFor = args.Player.Index;
                    args.Player.SendData((PacketTypes) 21, "", num, 1f);
                    args.Player.SendData((PacketTypes) 22, null, num);
                }
                wPlayer.hasItems.RemoveAll((WItem x) => x.id == firstItem!.type);
                DB.DeleteDB(args.Player.Name, firstItem!.type);
                ReplaceWeaponsInBackpack(args.Player.TPlayer, select, 1);
                select = null!;
                args.Player.SendMessage(LangTipsGet("完全重置成功！钱币回收：") + text3, new Color(0, 255, 0));
            }
            else
            {
                args.Player.SendInfoMessage(LangTipsGet("输入 /plus help    查看 plus 系列指令帮助"));
            }
        }
        else if (args.Parameters.Count >= 2 && args.Parameters.Count <= 3)
        {
            var result = 1;
            if (args.Parameters.Count == 3)
            {
                if (!int.TryParse(args.Parameters[2], out result))
                {
                    args.Player.SendInfoMessage(LangTipsGet("输入 /plus help    查看 plus 系列指令帮助"));
                    return;
                }
                if (result <= 0)
                {
                    args.Player.SendInfoMessage(LangTipsGet("请输入正整数"));
                    return;
                }
            }
            var num2 = 0;
            if (args.Parameters[1].Equals("up", StringComparison.OrdinalIgnoreCase))
            {
                num2 = 1;
            }
            else if (args.Parameters[1].Equals("down", StringComparison.OrdinalIgnoreCase))
            {
                num2 = -1;
            }
            else
            {
                args.Player.SendInfoMessage(LangTipsGet("输入 /plus help    查看 plus 系列指令帮助"));
            }
            if (args.Parameters[0].Equals("damage", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("da", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "伤害")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player, PlusType.damage, result))
                        {
                            select.damage_level += result;
                            if (!wPlayer.hasItems.Exists((WItem x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage($"[i:{select.id}] {LangTipsGet("升级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
                        }
                        break;
                    case -1:
                    {
                        if (select.damage_level - result < 0)
                        {
                            args.Player.SendInfoMessage(LangTipsGet("等级过低"));
                            break;
                        }
                        select.damage_level -= result;
                        select.checkDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage($"[i:{select.id}]  {LangTipsGet("降级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", new Color(0, 255, 0));
                        break;
                    }
                }
            }
            else if (args.Parameters[0].Equals("scale", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("sc", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "大小")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player, PlusType.scale, result))
                        {
                            select.scale_level += result;
                            if (!wPlayer.hasItems.Exists((WItem x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage($"[i:{select.id}] {LangTipsGet("升级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
                        }
                        break;
                    case -1:
                    {
                        if (select.scale_level - result < 0)
                        {
                            args.Player.SendInfoMessage(LangTipsGet("等级过低"));
                            break;
                        }
                        select.scale_level -= result;
                        select.checkDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage($"[i:{select.id}]  {LangTipsGet("降级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", new Color(0, 255, 0));
                        break;
                    }
                }
            }
            else if (args.Parameters[0].Equals("knockBack", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("kn", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "击退")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player, PlusType.knockBack, result))
                        {
                            select.knockBack_level += result;
                            if (!wPlayer.hasItems.Exists((WItem x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage($"[i:{select.id}] {LangTipsGet("升级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
                        }
                        break;
                    case -1:
                    {
                        if (select.knockBack_level - result < 0)
                        {
                            args.Player.SendInfoMessage(LangTipsGet("等级过低"));
                            break;
                        }
                        select.knockBack_level -= result;
                        select.checkDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage($"[i:{select.id}]  {LangTipsGet("降级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", new Color(0, 255, 0));
                        break;
                    }
                }
            }
            else if (args.Parameters[0].Equals("useSpeed", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("us", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "用速")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player, PlusType.useSpeed, result))
                        {
                            select.useSpeed_level += result;
                            if (!wPlayer.hasItems.Exists((WItem x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage($"[i:{select.id}] {LangTipsGet("升级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
                        }
                        break;
                    case -1:
                    {
                        if (select.useSpeed_level - result < 0)
                        {
                            args.Player.SendInfoMessage(LangTipsGet("等级过低"));
                            break;
                        }
                        select.useSpeed_level -= result;
                        select.checkDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage($"[i:{select.id}]  {LangTipsGet("降级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", new Color(0, 255, 0));
                        break;
                    }
                }
            }
            else if (args.Parameters[0].Equals("shootSpeed", StringComparison.OrdinalIgnoreCase) || args.Parameters[0].Equals("sh", StringComparison.OrdinalIgnoreCase) || args.Parameters[0] == "飞速")
            {
                switch (num2)
                {
                    case 1:
                        if (this.Deduction(select, args.Player, PlusType.shootSpeed, result))
                        {
                            select.shootSpeed_level += result;
                            if (!wPlayer.hasItems.Exists((WItem x) => x.id == select.id))
                            {
                                wPlayer.hasItems.Add(select);
                            }
                            DB.WriteDB(select);
                            ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                            args.Player.SendMessage($"[i:{select.id}] {LangTipsGet("升级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", this.getRandColor());
                        }
                        break;
                    case -1:
                    {
                        if (select.shootSpeed_level - result < 0)
                        {
                            args.Player.SendInfoMessage(LangTipsGet("等级过低"));
                            break;
                        }
                        select.shootSpeed_level -= result;
                        select.checkDB();
                        DB.WriteDB(select);
                        ReplaceWeaponsInBackpack(args.Player.TPlayer, select);
                        args.Player.SendMessage($"[i:{select.id}]  {LangTipsGet("降级成功")}   {LangTipsGet("共计消耗：")}{cointostring(select.allCost, out var _)}\n{select.ItemMess()}", new Color(0, 255, 0));
                        break;
                    }
                }
            }
            else
            {
                args.Player.SendInfoMessage(LangTipsGet("输入 /plus help    查看 plus 系列指令帮助"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(LangTipsGet("输入 /plus help    查看 plus 系列指令帮助"));
        }
    }
    #endregion

    #region 清理强化物品
    private void ClearPlusItem(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            if (DB.DeleteDBAll())
            {
                var array = wPlayers;
                foreach (var wPlayer in array)
                {
                    if (wPlayer == null || !wPlayer.isActive)
                    {
                        continue;
                    }
                    foreach (var hasItem in wPlayer.hasItems)
                    {
                        ReplaceWeaponsInBackpack(wPlayer.me.TPlayer, hasItem, 1);
                    }
                    wPlayer.hasItems.Clear();
                }
                if (args.Player.IsLoggedIn)
                {
                    TSPlayer.All.SendSuccessMessage(LangTipsGet("所有玩家的所有强化数据全部清理成功！"));
                }
                else
                {
                    args.Player.SendInfoMessage(LangTipsGet("所有玩家的所有强化数据全部清理成功！"));
                }
            }
            else
            {
                args.Player.SendErrorMessage(LangTipsGet("强化数据清理失败！！!"));
            }
        }
        else
        {
            args.Player.SendInfoMessage(LangTipsGet("输入 /clearallplayersplus   将数据库中强化物品全部清理"));
        }
    }
    #endregion
}