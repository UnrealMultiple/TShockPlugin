using System.Text;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     管理员侧指令处理。
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

        helpMessage.Append('\n').Append(Lang.T("help.admin.addloot"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.delloot"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.stack"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.monster"));
        helpMessage.Append('\n').Append(Lang.T("help.admin.anim"));
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
                case "stack":
                    AutoFish.Config.GlobalSkipNonStackableLoot = !AutoFish.Config.GlobalSkipNonStackableLoot;
                    caller.SendSuccessMessage(Lang.T("success.toggle.globalStack",
                        Lang.T(AutoFish.Config.GlobalSkipNonStackableLoot
                            ? "common.enabledVerb"
                            : "common.disabledVerb")));
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
                case "set":
                    caller.SendInfoMessage(Lang.T("info.currentConsumeCount", AutoFish.Config.BaitConsumeCount));
                    return true;
                case "time":
                    caller.SendInfoMessage(
                        Lang.T("info.currentDuration", AutoFish.Config.RewardDurationMinutes));
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
                case "add":
                    if (AutoFish.Config.BaitItemIds.Contains(item.type))
                    {
                        caller.SendErrorMessage(Lang.T("error.itemAlreadyInBait", item.Name));
                        return true;
                    }

                    AutoFish.Config.BaitItemIds.Add(item.type);
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.item.addBait", item.Name));
                    return true;
                case "del":
                    if (!AutoFish.Config.BaitItemIds.Contains(item.type))
                    {
                        caller.SendErrorMessage(Lang.T("error.itemNotInBait", item.Name));
                        return true;
                    }

                    AutoFish.Config.BaitItemIds.Remove(item.type);
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.item.removeBait", item.Name));
                    return true;
                case "addloot":
                    if (AutoFish.Config.ExtraCatchItemIds.Contains(item.type))
                    {
                        caller.SendErrorMessage(Lang.T("error.itemAlreadyInLoot", item.Name));
                        return true;
                    }

                    AutoFish.Config.ExtraCatchItemIds.Add(item.type);
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.item.addLoot", item.Name));
                    return true;
                case "delloot":
                    if (!AutoFish.Config.ExtraCatchItemIds.Contains(item.type))
                    {
                        caller.SendErrorMessage(Lang.T("error.itemNotInLoot", item.Name));
                        return true;
                    }

                    AutoFish.Config.ExtraCatchItemIds.Remove(item.type);
                    AutoFish.Config.Write();
                    caller.SendSuccessMessage(Lang.T("success.item.removeLoot", item.Name));
                    return true;
                case "set":
                    if (int.TryParse(args.Parameters[1], out var consumeNum))
                    {
                        AutoFish.Config.BaitConsumeCount = consumeNum;
                        AutoFish.Config.Write();
                        caller.SendSuccessMessage(Lang.T("success.set.consumeCount", consumeNum));
                    }

                    return true;
                case "duo":
                    if (int.TryParse(args.Parameters[1], out var maxNum))
                    {
                        AutoFish.Config.GlobalMultiHookMaxNum = maxNum;
                        AutoFish.Config.Write();
                        caller.SendSuccessMessage(Lang.T("success.set.multiMax", maxNum));
                    }

                    return true;
                case "time":
                    if (int.TryParse(args.Parameters[1], out var rewardMinutes))
                    {
                        AutoFish.Config.RewardDurationMinutes = rewardMinutes;
                        AutoFish.Config.Write();
                        caller.SendSuccessMessage(Lang.T("success.set.duration", rewardMinutes));
                    }

                    return true;
                default:
                    SendAdminHelpOnly(caller);
                    return true;
            }
        }

        return false;
    }
}