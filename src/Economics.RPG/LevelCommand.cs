using Economics.Core.Command;
using TShockAPI;

namespace Economics.RPG;

public class LevelCommand : BaseCommand
{
    public override string[] Alias => ["level"];

    public override List<string> Permissions => ["economics.rpg.admin"];

    public override string HelpText => GetString("玩家等级管理!");

    public override string ErrorText => GetString("语法错误，请输入/level help查看正确使用方法!");

    [SubCommand("help")]
    public static void Help(CommandArgs args)
    {
        args.Player.SendInfoMessage(GetString($"level帮助"));
        args.Player.SendInfoMessage(GetString($"level set [等级]"));
        args.Player.SendInfoMessage(GetString($"level set [玩家] [等级]"));
        args.Player.SendInfoMessage(GetString($"level reset"));
    }

    [SubCommand("reset")]
    public static void ResetLevel(CommandArgs args)
    {
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "reset")
        {
            RPG.PlayerLevelManager.RemoveAll();
            args.Player.SendSuccessMessage(GetString("玩家等级信息重置成功!"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("语法错误，正确语法:"));
            args.Player.SendErrorMessage("/level reset");
        }
    }

    [SubCommand("set", 2)]
    public static void SetLevel(CommandArgs args)
    {
        var name = args.Player.Name;
        var level = "";
        if (args.Parameters.Count == 2)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendErrorMessage(GetString("为自己设置等级时，必须在游戏中使用此命令!"));
                return;
            }
            level = args.Parameters[1];
        }
        else
        {
            level = args.Parameters[2];
            name = args.Parameters[1];
        }
        RPG.PlayerLevelManager.Update(name, level);
        args.Player.SendSuccessMessage(GetString($"玩家 {name} 的等级被设置为 {level}。"));
    }
}
