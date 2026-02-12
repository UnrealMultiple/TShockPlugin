using AutoFish.Data;
using AutoFish.Utils;
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
            // 不再直接return，允许继续执行
        }

        var playerData = AutoFish.PlayerData.GetOrCreatePlayerData(player.Name, AutoFish.CreateDefaultPlayerData);
        if (args.Parameters.Count == 0)
        {
            HelpCmd(args.Player);
            if (!isConsole)
            {
                if (!playerData.AutoFishEnabled)
                {
                    args.Player.SendSuccessMessage(Lang.T("af.promptEnable"));
                }

                //开启了消耗模式
                else if (playerData.CanConsume())
                {
                    var (minutes, seconds) = playerData.GetRemainTime();
                    args.Player.SendMessage(Lang.T("af.remaining", minutes, seconds), 243, 181, 145);
                }
            }

            return;
        }

        if (HandlePlayerCommand(args, playerData))
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
    private static void SendStatus(TSPlayer player, AFPlayerData.ItemData playerData)
    {
        var enabledFeatures = new List<string>();

        // 只添加已开启的功能
        if (playerData.AutoFishEnabled)
            enabledFeatures.Add(Lang.T("status.autofish", Lang.T("common.enabled")));

        if (playerData.BuffEnabled)
            enabledFeatures.Add(Lang.T("status.buff", Lang.T("common.enabled")));

        if (playerData.MultiHookEnabled)
            enabledFeatures.Add(Lang.T("status.multihook", Lang.T("common.enabled"), playerData.HookMaxNum));

        if (playerData.BlockMonsterCatch)
            enabledFeatures.Add(Lang.T("status.blockMonster", Lang.T("common.enabled")));

        if (playerData.SkipFishingAnimation)
            enabledFeatures.Add(Lang.T("status.skipAnimation", Lang.T("common.enabled")));

        if (playerData.BlockQuestFish)
            enabledFeatures.Add(Lang.T("status.blockQuestFish", Lang.T("common.enabled")));

        if (playerData.ProtectValuableBaitEnabled)
            enabledFeatures.Add(Lang.T("status.protectBait", Lang.T("common.enabled")));

        if (AutoFish.Config.BaitRewards.Any() && playerData.CanConsume())
        {
            var (minutes, seconds) = playerData.GetRemainTime();
            enabledFeatures.Add(Lang.T("status.consumptionEnabled", minutes, seconds));
        }

        // 发送彩色标题
        Tools.SendGradientMessage(player, Lang.T("status.title"));

        // 发送每个已开启的功能
        if (enabledFeatures.Any())
            foreach (var feature in enabledFeatures)
                Tools.SendGradientMessage(player, feature);
        else
            player.SendInfoMessage(Lang.T("status.noEnabledFeatures"));
    }
}