using Lagrange.XocMat.Adapter.Attributes;
using TShockAPI;

namespace Lagrange.XocMat.Adapter;

public class Command
{
    #region 设置进度
    [CommandMatch("设置进度", Permission.SetProgress)]
    public void SetProgress(CommandArgs args)
    {
        if (args.Parameters.Count == 2)
        {
            bool enable;
            switch (args.Parameters[1])
            {
                case "开启":
                case "开":
                case "true":
                    enable = true;
                    break;
                case "关闭":
                case "关":
                case "false":
                    enable = false;
                    break;
                default:
                    args.Player.SendErrorMessage("请输入一个正确的状态!");
                    return;
            }
            if (Utils.SetGameProgress(args.Parameters[0], enable))
            {
                args.Player.SendSuccessMessage($"成功设置进度`{args.Parameters[0]}`状态为{enable}");
            }
            else
            {
                args.Player.SendErrorMessage($"进度{args.Parameters[0]}不存在!");
            }
        }
        else
        {
            args.Player.SendErrorMessage("语法错误，正确语法:\n!设置进度 [进度名] [true|开|开启|关|关闭|false]");
        }
    }
    #endregion
}
