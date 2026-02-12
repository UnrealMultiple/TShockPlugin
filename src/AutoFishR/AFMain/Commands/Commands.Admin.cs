using System.Text;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     管理员侧指令处理�?
/// </summary>
public partial class Commands
{
    private static void AppendAdminHelp(TSPlayer player, StringBuilder helpMessage)
    {
        helpMessage.Append('\n').Append(Lang.T("help.admin.buff"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.multi"));

        if (AutoFish.Config.GlobalMultiHookFeatureEnabled)
            helpMessage.Append('\n').Append(Lang.T("help.admin.duo"));

        helpMessage.Append('\n').Append(Lang.T("help.admin.mod"));

        if (AutoFish.Config.GlobalConsumptionModeEnabled)
        {
            helpMessage.Append('\n').Append(Lang.T("help.admin.set"));
            helpMessage.Append('\n').Append(Lang.T("help.admin.time"));
            helpMessage.Append('\n').Append(Lang.T("help.admin.add"));
            helpMessage.Append('\n').Append(Lang.T("help.admin.del"));
        }

        helpMessage.Append('\n').Append(Lang.T("help.admin.monster"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.anim"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.export"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.exportstats"));
        helpMessage.Append('\n').Append("  /afa debug - 切换 DEBUG 模式");
    }

    private static bool HandleAdminCommand(CommandArgs args)
    {
        var caller = args.Player ?? TSPlayer.Server;
        if (!caller.HasPermission(AutoFish.AdminPermission)) return false;

        var sub = args.Parameters[0].ToLower();

        if (args.Parameters.Count == 1)
            switch (sub)
            {
                case "multi":
                    AutoFish.Config.GlobalMultiHookFeatureEnabled = !AutoFish.Config.GlobalMultiHookFeatureEnabled;
                    var multiToggle = Lang.T(AutoFish.Config.GlobalMultiHookFeatureEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    caller.SendSuccessMessage(Lang.T("success.toggle.globalMulti", caller.Name, multiToggle));
                    AutoFish.Config.Write();
                    return true;
                case "buff":
                    AutoFish.Config.GlobalBuffFeatureEnabled = !AutoFish.Config.GlobalBuffFeatureEnabled;
                    var buffToggleText = Lang.T(AutoFish.Config.GlobalBuffFeatureEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    caller.SendSuccessMessage(Lang.T("success.toggle.globalBuff", caller.Name, buffToggleText));
                    AutoFish.Config.Write();
                    return true;
                case "mod":
                    AutoFish.Config.GlobalConsumptionModeEnabled = !AutoFish.Config.GlobalConsumptionModeEnabled;
                    var modToggle = Lang.T(AutoFish.Config.GlobalConsumptionModeEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    caller.SendSuccessMessage(Lang.T("success.toggle.consumption", caller.Name, modToggle));
                    AutoFish.Config.Write();
                    return true;
                case "monster":
                    AutoFish.Config.GlobalBlockMonsterCatch = !AutoFish.Config.GlobalBlockMonsterCatch;
                    caller.SendSuccessMessage(Lang.T("success.toggle.globalMonster",
                        Lang.T(AutoFish.Config.GlobalBlockMonsterCatch
                            ? "common.enabledVerb"
                            : "common.disabledVerb")));
                    AutoFish.Config.Write();
                    return true;
                case "anim":
                    AutoFish.Config.GlobalSkipFishingAnimation = !AutoFish.Config.GlobalSkipFishingAnimation;
                    caller.SendSuccessMessage(Lang.T("success.toggle.globalAnim",
                        Lang.T(AutoFish.Config.GlobalSkipFishingAnimation
                            ? "common.enabledVerb"
                            : "common.disabledVerb")));
                    AutoFish.Config.Write();
                    return true;
                case "debug":
                    AutoFish.DebugMode = !AutoFish.DebugMode;
                    var debugStatus = AutoFish.DebugMode ? "[c/00FF00:已开启]" : "[c/FF0000:已关闭]";
                    caller.SendSuccessMessage($"DEBUG 模式 {debugStatus}");
                    if (AutoFish.DebugMode)
                    {
                        caller.SendInfoMessage("[c/FFFF00:警告：DEBUG 模式会输出大量调试信息到控制台]");
                        TShock.Log.ConsoleInfo($"[AutoFishR] DEBUG 模式已开启 (操作者: {caller.Name})");
                    }
                    else
                    {
                        TShock.Log.ConsoleInfo($"[AutoFishR] DEBUG 模式已关闭 (操作者: {caller.Name})");
                    }
                    return true;
                default:
                    return false;
            }

        if (args.Parameters.Count == 2)
        {
            var matchedItems = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (matchedItems.Count > 1)
            {
                caller.SendMultipleMatchError(matchedItems.Select(i => i.Name));
                return true;
            }

            if (matchedItems.Count == 0)
            {
                caller.SendErrorMessage(Lang.T("error.itemNotFound"));
                return true;
            }

            var item = matchedItems[0];

            switch (sub)
            {
                case "del":
                    if (!AutoFish.Config.BaitRewards.ContainsKey(item.type))
                    {
                        caller.SendErrorMessage(Lang.T("error.itemNotInBait", item.Name));
                        return true;
                    }

                    AutoFish.Config.BaitRewards.Remove(item.type);
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.item.removeBait", item.Name));
                    return true;
                case "duo":
                    if (int.TryParse(args.Parameters[1], out var maxNum))
                    {
                        AutoFish.Config.GlobalMultiHookMaxNum = maxNum;
                        AutoFish.Config.Write();
                        caller.SendSuccessMessage(Lang.T("success.set.multiMax", maxNum));
                    }

                    return true;
                default:
                    SendAdminHelpOnly(caller);
                    return true;
            }
        }

        // 处理 3 个参数的命令：add, set, time
        if (args.Parameters.Count == 4)
        {
            var matchedItems = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (matchedItems.Count > 1)
            {
                caller.SendMultipleMatchError(matchedItems.Select(i => i.Name));
                return true;
            }

            if (matchedItems.Count == 0)
            {
                caller.SendErrorMessage(Lang.T("error.itemNotFound"));
                return true;
            }

            var item = matchedItems[0];

            switch (sub)
            {
                case "add":
                case "set":
                    if (!int.TryParse(args.Parameters[2], out var count) || count < 1)
                    {
                        caller.SendErrorMessage("数量必须是大�?0 的整数！");
                        return true;
                    }

                    if (!int.TryParse(args.Parameters[3], out var minutes) || minutes < 1)
                    {
                        caller.SendErrorMessage("分钟数必须是大于 0 的整数！");
                        return true;
                    }

                    AutoFish.Config.BaitRewards[item.type] = new BaitReward
                    {
                        Count = count,
                        Minutes = minutes
                    };
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.set.baitReward", item.Name, count, minutes));
                    return true;
                default:
                    return false;
            }
        }

        // 处理 3 个参数的 time 命令
        if (args.Parameters.Count == 3 && sub == "time")
        {
            var matchedItems = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
            if (matchedItems.Count > 1)
            {
                caller.SendMultipleMatchError(matchedItems.Select(i => i.Name));
                return true;
            }

            if (matchedItems.Count == 0)
            {
                caller.SendErrorMessage(Lang.T("error.itemNotFound"));
                return true;
            }

            var item = matchedItems[0];

            if (!AutoFish.Config.BaitRewards.ContainsKey(item.type))
            {
                caller.SendErrorMessage(Lang.T("error.itemNotInBait", item.Name));
                return true;
            }

            if (!int.TryParse(args.Parameters[2], out var minutes) || minutes < 1)
            {
                caller.SendErrorMessage("分钟数必须是大于 0 的整数！");
                return true;
            }

            AutoFish.Config.BaitRewards[item.type].Minutes = minutes;
            AutoFish.Config.Write();
            caller.SendSuccessMessage(Lang.T("success.set.baitReward", item.Name,
                AutoFish.Config.BaitRewards[item.type].Count, minutes));
            return true;
        }

        return false;
    }

}

