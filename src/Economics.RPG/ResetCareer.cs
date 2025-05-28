using Economics.Core.Command;
using Economics.RPG.Setting;
using Economics.Core.Extensions;
using TShockAPI;
using Microsoft.Xna.Framework;

namespace Economics.RPG;

public class ResetCareer : BaseCommand
{
    public override string[] Alias => ["resetrank", "重置职业"];

    public override List<string> Permissions => ["economics.rpg.reset"];

    public override string HelpText => GetString("重置RPG职业!");

    public override string ErrorText => GetString("语法错误，请输入/resetrank 重置职业");

    public override void Invoke(CommandArgs args)
    {
        if (!args.Player.IsLoggedIn)
        {
            args.Player.SendErrorMessage(GetString("你必须登陆才能使用此命令!"));
            return;
        }
        RPG.PlayerLevelManager.ResetPlayerLevel(args.Player.Name);
        args.Player.SendSuccessMessage(GetString("您已成功重置等级!"));
        foreach (var cmd in Config.Instance.ResetCommand)
        {
            args.Player.ExecCommand(cmd);
        }
        TShock.Utils.Broadcast(string.Format(Config.Instance.ResetBroadcast, args.Player.Name), Color.Green);
        if (Config.Instance.ResetKick)
        {
            args.Player.Disconnect(GetString("你因重置等级被踢出!"));
        }
    }
}
