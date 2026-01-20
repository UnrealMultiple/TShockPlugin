using System.Text;
using AutoFish.Data;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     自动钓鱼插件的聊天命令处理入口与通用逻辑。
/// </summary>
public partial class Commands
{
    /// <summary>
    ///     处理 /af 相关指令的入口。
    /// </summary>
    public static void Afs(CommandArgs args)
    {
        var player = args.Player;
        var isConsole = !player.RealPlayer;

        if (!AutoFish.Config.PluginEnabled) return;

        if (isConsole)
        {
            player.SendInfoMessage(Lang.T("help.consoleUseAfa"));
            return;
        }

        var playerData = AutoFish.PlayerData.GetOrCreatePlayerData(player.Name, AutoFish.CreateDefaultPlayerData);

        //消耗模式下的剩余时间记录
        var remainingMinutes = AutoFish.Config.RewardDurationMinutes - (DateTime.Now - playerData.LogTime).TotalMinutes;

        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player);
            if (!isConsole)
            {
                if (!playerData.AutoFishEnabled)
                    args.Player.SendSuccessMessage(Lang.T("af.promptEnable"));

                //开启了消耗模式
                else if (playerData.ConsumptionEnabled)
                    args.Player.SendMessage(Lang.T("af.remaining", Math.Floor(remainingMinutes)), 243, 181,
                        145);
            }

            return;
        }

        if (!isConsole)
            if (HandlePlayerCommand(args, playerData, remainingMinutes))
                return;

        HelpCmd(args.Player);
    }

    /// <summary>
    ///     处理 /afa（管理员）指令入口。
    /// </summary>
    public static void Afa(CommandArgs args)
    {
        if (!AutoFish.Config.PluginEnabled) return;

        var caller = args.Player ?? TSPlayer.Server;
        if (!caller.HasPermission(AutoFish.AdminPermission))
        {
            caller.SendErrorMessage(Lang.T("error.noPermission.admin"));
            return;
        }

        if (args.Parameters.Count == 0)
        {
            SendAdminHelpOnly(caller);
            return;
        }

        if (HandleAdminCommand(args)) return;

        SendAdminHelpOnly(caller);
    }

    /// <summary>
    ///     展示个人状态信息。
    /// </summary>
    private static void SendStatus(TSPlayer player, AFPlayerData.ItemData playerData, double remainingMinutes)
    {
        var sb = new StringBuilder();
        var onOff = new Func<bool, string>(v => v ? Lang.T("common.enabled") : Lang.T("common.disabled"));

        sb.AppendLine(Lang.T("status.title"));
        sb.AppendLine(Lang.T("status.autofish", onOff(playerData.AutoFishEnabled)));
        sb.AppendLine(Lang.T("status.buff", onOff(playerData.BuffEnabled)));
        sb.AppendLine(Lang.T("status.multihook", onOff(playerData.MultiHookEnabled), playerData.HookMaxNum));
        sb.AppendLine(Lang.T("status.skipUnstackable", onOff(playerData.SkipNonStackableLoot)));
        sb.AppendLine(Lang.T("status.blockMonster", onOff(playerData.BlockMonsterCatch)));
        sb.AppendLine(Lang.T("status.skipAnimation", onOff(playerData.SkipFishingAnimation)));
        sb.AppendLine(Lang.T("status.protectBait", onOff(playerData.ProtectValuableBaitEnabled)));

        if (AutoFish.Config.BaitItemIds.Any() || playerData.ConsumptionEnabled)
        {
            var minutesLeft = Math.Max(0, Math.Floor(remainingMinutes));
            var consumeLine = playerData.ConsumptionEnabled
                ? Lang.T("status.consumptionEnabled", minutesLeft)
                : Lang.T("status.consumptionDisabled");
            sb.AppendLine(consumeLine);
        }

        player.SendInfoMessage(sb.ToString());
    }
}