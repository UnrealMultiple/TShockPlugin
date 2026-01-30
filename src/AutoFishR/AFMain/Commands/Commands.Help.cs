using System.Text;
using TShockAPI;

namespace AutoFish.AFMain;

/// <summary>
///     指令帮助展示逻辑。
/// </summary>
public partial class Commands
{
    private static void HelpCmd(TSPlayer player)
    {
        var isConsole = !player.RealPlayer;
        var helpMessage = new StringBuilder();
        helpMessage.Append(Lang.T("help.banner"));

        if (!isConsole)
            AppendPlayerHelp(player, helpMessage);

        player.SendMessage(helpMessage.ToString(), 193, 223, 186);
    }

    /// <summary>
    ///     仅输出管理员帮助，用于 /afa 和控制台提示。
    /// </summary>
    private static void SendAdminHelpOnly(TSPlayer player)
    {
        var sb = new StringBuilder();
        sb.Append(Lang.T("help.admin.title"));
        AppendAdminHelp(player, sb);
        player.SendMessage(sb.ToString(), 193, 223, 186);
    }
}