using System.Text;
using AutoFish.Data;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     玩家侧指令处理
/// </summary>
public partial class Commands
{
    private static void AppendPlayerHelp(TSPlayer player, StringBuilder helpMessage)
    {
        // 个人指令（按权限和全局开关过滤）
        helpMessage.Append('\n').Append(Lang.T("help.player.af"));
        helpMessage.Append('\n').Append(Lang.T("help.player.status"));

        if (AutoFish.Config.GlobalAutoFishFeatureEnabled && AutoFish.HasFeaturePermission(player, "fish"))
            helpMessage.Append('\n').Append(Lang.T("help.player.fish"));

        if (AutoFish.Config.GlobalBuffFeatureEnabled && AutoFish.HasFeaturePermission(player, "buff"))
            helpMessage.Append('\n').Append(Lang.T("help.player.buff"));

        if (AutoFish.Config.GlobalMultiHookFeatureEnabled &&
            AutoFish.HasFeaturePermission(player, "multihook"))
        {
            helpMessage.Append('\n').Append(Lang.T("help.player.multi"));
            helpMessage.Append('\n').Append(Lang.T("help.player.hook"));
        }

        if (AutoFish.Config.GlobalBlockMonsterCatch && AutoFish.HasFeaturePermission(player, "filter.monster"))
            helpMessage.Append('\n').Append(Lang.T("help.player.monster"));

        if (AutoFish.Config.GlobalSkipFishingAnimation &&
            AutoFish.HasFeaturePermission(player, "skipanimation"))
            helpMessage.Append('\n').Append(Lang.T("help.player.anim"));

        if (AutoFish.Config.GlobalBlockQuestFish && AutoFish.HasFeaturePermission(player, "filter.quest"))
            helpMessage.Append('\n').Append(Lang.T("help.player.quest"));

        if (AutoFish.Config.GlobalConsumptionModeEnabled)
            helpMessage.Append('\n').Append(Lang.T("help.player.list"));

        if (AutoFish.Config.GlobalProtectValuableBaitEnabled &&
            AutoFish.HasFeaturePermission(player, "bait.protect"))
        {
            helpMessage.Append('\n').Append(Lang.T("help.player.bait"));
            helpMessage.Append('\n').Append(Lang.T("help.player.baitlist"));
        }
    }

    private static bool HandlePlayerCommand(CommandArgs args, AFPlayerData.ItemData playerData)
    {
        var player = args.Player;
        var sub = args.Parameters[0].ToLower();

        if (args.Parameters.Count == 1)
            switch (sub)
            {
                case "fish":
                    if (!AutoFish.HasFeaturePermission(player, "fish"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.af"));
                        return true;
                    }

                    playerData.AutoFishEnabled = !playerData.AutoFishEnabled;
                    var fishVerb = Lang.T(playerData.AutoFishEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(
                        Lang.T("success.toggle.fish", args.Player.Name, fishVerb));
                    return true;
                case "buff":
                    if (!AutoFish.HasFeaturePermission(player, "buff"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.buff"));
                        return true;
                    }

                    playerData.BuffEnabled = !playerData.BuffEnabled;
                    var buffVerb = Lang.T(playerData.BuffEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(
                        Lang.T("success.toggle.buff", args.Player.Name, buffVerb));
                    return true;
                case "multi":
                    if (!AutoFish.HasFeaturePermission(player, "multihook"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.multi"));
                        return true;
                    }

                    if (!AutoFish.Config.GlobalMultiHookFeatureEnabled)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.multiDisabled"));
                        return true;
                    }

                    playerData.MultiHookEnabled = !playerData.MultiHookEnabled;
                    var multiVerb = Lang.T(playerData.MultiHookEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(Lang.T("success.toggle.multi", args.Player.Name, multiVerb));
                    return true;
                case "status":
                    SendStatus(args.Player, playerData);
                    return true;
                case "monster":
                    if (!AutoFish.Config.GlobalBlockMonsterCatch)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.monsterDisabled"));
                        return true;
                    }

                    if (!AutoFish.HasFeaturePermission(player, "filter.monster"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.monster"));
                        return true;
                    }

                    playerData.BlockMonsterCatch = !playerData.BlockMonsterCatch;
                    var monsterVerb = Lang.T(playerData.BlockMonsterCatch
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(Lang.T("success.toggle.monster", args.Player.Name, monsterVerb));
                    return true;
                case "anim":
                    if (!AutoFish.Config.GlobalSkipFishingAnimation)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.animDisabled"));
                        return true;
                    }

                    if (!AutoFish.HasFeaturePermission(player, "skipanimation"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.anim"));
                        return true;
                    }

                    playerData.SkipFishingAnimation = !playerData.SkipFishingAnimation;
                    var animVerb = Lang.T(playerData.SkipFishingAnimation
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(Lang.T("success.toggle.anim", args.Player.Name, animVerb));
                    return true;
                case "quest":
                    if (!AutoFish.Config.GlobalBlockQuestFish)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.questDisabled"));
                        return true;
                    }

                    if (!AutoFish.HasFeaturePermission(player, "filter.quest"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.quest"));
                        return true;
                    }

                    playerData.BlockQuestFish = !playerData.BlockQuestFish;
                    var questVerb = Lang.T(playerData.BlockQuestFish
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(Lang.T("success.toggle.quest", args.Player.Name, questVerb));
                    return true;
                case "bait":
                    if (!AutoFish.Config.GlobalProtectValuableBaitEnabled)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.protectDisabled"));
                        return true;
                    }

                    if (!AutoFish.HasFeaturePermission(player, "bait.protect"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.protectBait"));
                        return true;
                    }

                    playerData.ProtectValuableBaitEnabled = !playerData.ProtectValuableBaitEnabled;
                    var baitVerb = Lang.T(playerData.ProtectValuableBaitEnabled
                        ? "common.enabledVerb"
                        : "common.disabledVerb");
                    args.Player.SendSuccessMessage(Lang.T("success.toggle.protectBait", args.Player.Name, baitVerb));
                    return true;
                case "baitlist":
                    if (!AutoFish.HasFeaturePermission(player, "bait.protect"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.baitlist"));
                        return true;
                    }

                    if (!AutoFish.Config.ValuableBaitItemIds.Any())
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.noValuableBait"));
                        return true;
                    }

                    args.Player.SendInfoMessage(Lang.T("info.valuableListHeader") + string.Join(", ",
                        AutoFish.Config.ValuableBaitItemIds.Select(x =>
                            TShock.Utils.GetItemById(x).Name + "([c/92C5EC:{0}])".SFormat(x))));
                    return true;
                case "list" when AutoFish.Config.BaitRewards.Any():
                    var sb = new StringBuilder();
                    sb.Append(Lang.T("info.consumeListHeader"));
                    foreach (var kvp in AutoFish.Config.BaitRewards.OrderByDescending(x => x.Value.Minutes))
                    {
                        var itemName = TShock.Utils.GetItemById(kvp.Key).Name;
                        sb.AppendFormat(Lang.T("info.baitReward"),
                            $"[c/92C5EC:{itemName}]([c/AECDD1:{kvp.Key}])",
                            kvp.Value.Count,
                            kvp.Value.Minutes);
                    }

                    args.Player.SendInfoMessage(sb.ToString());
                    return true;
                default:
                    return false;
            }

        if (args.Parameters.Count == 2)
            switch (sub)
            {
                case "hook":
                    if (!AutoFish.HasFeaturePermission(args.Player, "multihook"))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.noPermission.hook"));
                        return true;
                    }

                    if (!AutoFish.Config.GlobalMultiHookFeatureEnabled)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.multiSetDisabled"));
                        return true;
                    }

                    if (!int.TryParse(args.Parameters[1], out var personalMax))
                    {
                        args.Player.SendErrorMessage(Lang.T("error.hookInvalidNumber"));
                        return true;
                    }

                    if (personalMax < 1)
                    {
                        args.Player.SendWarningMessage(Lang.T("warn.hookMin"));
                        return true;
                    }

                    if (personalMax > AutoFish.Config.GlobalMultiHookMaxNum)
                    {
                        args.Player.SendWarningMessage(
                            Lang.T("warn.hookLimited", AutoFish.Config.GlobalMultiHookMaxNum));
                        personalMax = AutoFish.Config.GlobalMultiHookMaxNum;
                    }

                    playerData.HookMaxNum = personalMax;
                    args.Player.SendSuccessMessage(
                        Lang.T("success.set.hookPersonal", personalMax, AutoFish.Config.GlobalMultiHookMaxNum));
                    return true;
                default:
                    return false;
            }

        return false;
    }
}