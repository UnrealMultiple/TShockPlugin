using Microsoft.Xna.Framework;
using System.Text;
using TShockAPI;

namespace RainbowChat;
public class Commands
{
    #region 指令方法
    public static void RainbowChatCallback(CommandArgs e)
    {
        var plr = e.Player;

        if (e.Parameters.Count == 0)
        {
            Help(e);
            return;
        }

        if (e.Parameters.Count == 1 && plr.HasPermission("rainbowchat.admin"))
        {
            if (e.Parameters[0].ToLower() == "开启" || e.Parameters[0].ToLower() == "on")
            {
                RainbowChat.Config.Enabled = true;
                RainbowChat.Config.Write();
                e.Player.SendSuccessMessage(GetString($"{plr.Name} 的彩虹聊天插件已启用."));
                return;
            }

            if (e.Parameters[0].ToLower() == "关闭" || e.Parameters[0].ToLower() == "off")
            {
                RainbowChat.Config.Enabled = false;
                RainbowChat.Config.Write();
                e.Player.SendSuccessMessage(GetString($"{plr.Name} 的彩虹聊天插件已关闭."));
                return;
            }

            if (e.Parameters[0].ToLower() == "随机开关" || e.Parameters[0].ToLower() == "rswitch")
            {
                RainbowChat.Config.Random = !RainbowChat.Config.Random;
                RainbowChat.Config.Write();
                e.Player.SendSuccessMessage(RainbowChat.Config.Random ?
                    GetString($"全局的随机色聊天已 启用.") :
                    GetString($"全局的随机色聊天已 禁用."));
                return;
            }

            if (e.Parameters[0].ToLower() == "渐变开关" || e.Parameters[0].ToLower() == "gswitch")
            {
                RainbowChat.Config.Gradient = !RainbowChat.Config.Gradient;
                RainbowChat.Config.Write();
                e.Player.SendSuccessMessage(RainbowChat.Config.Gradient ?
                    GetString($"全局的渐变色聊天已 启用.") :
                    GetString($"全局的渐变色聊天已 禁用."));
                return;
            }
        }

        switch (e.Parameters[0].ToLower())
        {
            case "随机":
            case "random":
            {
                CRainbowChat(e);
                break;
            }

            case "渐变":
            case "gradient":
            {
                if (e.Parameters.Count >= 3)
                {
                    switch (e.Parameters[1].ToLower())
                    {
                        case "开始":
                        case "begin":
                        case "start":
                            if (e.Parameters.Count >= 3)
                            {
                                var array2 = e.Parameters[2].Split(',');
                                if (array2.Length == 3 && byte.TryParse(array2[0], out var result4) && byte.TryParse(array2[1], out var result5) && byte.TryParse(array2[2], out var result6))
                                {
                                    RainbowChat.Config.GradientStartColor = new Color(result4, result5, result6);
                                    RainbowChat.Config.Write();
                                    plr.SendSuccessMessage(GetString($"渐变开始颜色已设置为 R:{result4}, G:{result5}, B:{result6}"));
                                }
                                else
                                {
                                    plr.SendErrorMessage(GetString("无效的起始颜色值。请使用格式如 'R,G,B'（逗号分隔）的颜色表示法。"));
                                }
                            }
                            else
                            {
                                plr.SendErrorMessage(GetString("起始颜色值缺失。请使用如 '/rc 渐变 开始 255,0,0' 的格式。"));
                            }
                            break;
                        case "结束":
                        case "stop":
                        case "end":
                            if (e.Parameters.Count >= 3)
                            {
                                var array = e.Parameters[2].Split(',');
                                if (array.Length == 3 && byte.TryParse(array[0], out var result) && byte.TryParse(array[1], out var result2) && byte.TryParse(array[2], out var result3))
                                {
                                    RainbowChat.Config.GradientEndColor = new Color(result, result2, result3);
                                    RainbowChat.Config.Write();
                                    plr.SendSuccessMessage(GetString($"渐变结束颜色已设置为 R:{result}, G:{result2}, B:{result3}"));
                                }
                                else
                                {
                                    plr.SendErrorMessage(GetString("无效的结束颜色值。请使用格式如 'R,G,B'（逗号分隔）的颜色表示法。"));
                                }
                            }
                            else
                            {
                                plr.SendErrorMessage(GetString("结束颜色值缺失。请使用如 '/rc 渐变 结束 0,255,0' 的格式。"));
                            }
                            break;
                        default:
                            GradientChat(e);
                            break;
                    }
                }
                else
                {
                    GradientChat(e);
                }
                break;
            }
        }
    }
    #endregion


    #region 菜单指令方法
    public static void Help(CommandArgs e)
    {
        var mess = new StringBuilder();

        var plr = e.Player;

        mess.AppendLine(GetString("《彩虹聊天》 菜单指令\n") +
                GetString("/rc 开启 或 关闭 - 开启|关闭插件功能\n") +
                GetString("/rc 随机开关 - 全局开启|关闭随机色功能\n") +
                GetString("/rc 渐变开关 - 全局开启|关闭渐变色功能\n") +
                GetString("/rc 随机 - 开启或关闭自己的随机单色整句\n") +
                GetString("/rc 渐变 - 开启或关闭自己的渐变聊天\n") +
                GetString("/rc 渐变 开始 R,G,B - 设置渐变聊天起始颜色\n") +
                GetString("/rc 渐变 结束 R,G,B - 设置渐变聊天结束颜色"));

        if (RainbowChat.Config.ErrorMess)
        {
            mess.Append(GetString("[i:3454][c/E83849:注意][i:3454]\n"));
            mess.Append(GetString("使用[c/B0B2D1:非正版客户端]或[c/B0B2D1:语言资源包]导致TS指令异常\n") +
                GetString("请服主安装[c/B0B2D1:Chireiden.TShock.Omni]插件修复\n") +
                GetString("或者请玩家使用 [c/B0B2D1:．help] 代替 [c/B0B2D1:/help]"));
        }

        plr.SendInfoMessage(string.Format($"{mess}", Color.Yellow));
    }
    #endregion

    #region 切换随机色聊天方法
    public static void CRainbowChat(CommandArgs e)
    {
        var plr = GetTargetPlayer(e);

        if (plr == null)
        {
            return;
        }

        if (!RainbowChat.Config.Enabled || !RainbowChat.Config.Random)
        {
            return;
        }

        RainbowChat.RandomChat[plr.Index] = !RainbowChat.RandomChat[plr.Index];
        e.Player.SendSuccessMessage(RainbowChat.RandomChat[plr.Index] ?
            GetString($"{plr.Name} 的彩虹聊天已 启用.") :
            GetString($"{plr.Name} 的彩虹聊天已 禁用."));

        if (RainbowChat.Gradient[plr.Index] && RainbowChat.Gradient[plr.Index])
        {
            RainbowChat.Gradient[plr.Index] = false;
        }
    }
    #endregion

    #region 切换渐变聊天方法
    public static void GradientChat(CommandArgs e)
    {
        var plr = GetTargetPlayer(e);

        if (plr == null)
        {
            return;
        }

        if (!RainbowChat.Config.Enabled || !RainbowChat.Config.Gradient)
        {
            return;
        }

        RainbowChat.Gradient[plr.Index] = !RainbowChat.Gradient[plr.Index];
        e.Player.SendSuccessMessage(RainbowChat.Gradient[plr.Index] ?
            GetString($"{plr.Name} 的渐变聊天已 启用.") :
            GetString($"{plr.Name} 的渐变聊天已 禁用."));

        if (RainbowChat.Gradient[plr.Index] && RainbowChat.RandomChat[plr.Index])
        {
            RainbowChat.RandomChat[plr.Index] = false;
        }
    }
    #endregion

    #region 获取转换颜色聊天的玩家目标方法（也就是自己）
    public static TSPlayer? GetTargetPlayer(CommandArgs e)
    {
        if (e.Parameters.Count <= 1)
        {
            return e.Player;
        }
        var list = TSPlayer.FindByNameOrID(e.Parameters[1]);
        if (list.Count == 0)
        {
            e.Player.SendErrorMessage(GetString("错误的玩家!"));
            return null;
        }
        if (list.Count > 1)
        {
            e.Player.SendMultipleMatchError((IEnumerable<object>) list.Select((TSPlayer p) => p.Name));
            return null;
        }
        return list[0];
    }
    #endregion
}
