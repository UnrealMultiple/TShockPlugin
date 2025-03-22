using TShockAPI;
using Microsoft.Xna.Framework;
using static PacketsStop.PacketsStop;

namespace PacketsStop;

internal class Commands
{
    #region 指令
    public static void Command(CommandArgs args)
    {
        var plr = args.Player;

        //子命令数量为0时
        if (args.Parameters.Count == 0)
        {
            plr.SendInfoMessage(GetString($"数据包拦截指令菜单\n") +
                GetString($"/pksp on ——开启数据包拦截\n") +
                GetString($"/pksp off ——关闭数据包拦截\n") +
                GetString($"/pksp list ——列出拦截名单\n") +
                GetString($"/pksp add 玩家名字 ——将指定玩家添加到拦截名单\n") +
                GetString($"/pksp del 玩家名字 ——将指定玩家从拦截名单移除\n") +
                GetString($"/pksp reset ——清空拦截名单"));

            if (Config.Enabled)
            {
                plr.SendMessage(GetString($"拦截功能:已启用"), Color.Aquamarine);
            }
            else
            {
                plr.SendMessage(GetString($"拦截功能:已关闭"), Color.Aquamarine);
            }
        }

        if (args.Parameters.Count >= 1)
        {
            switch (args.Parameters[0].ToLower())
            {
                case "on":
                {
                    Config.Enabled = true;
                    Config.Write();
                    plr.SendInfoMessage(GetString("数据包拦截 已启用"));
                }
                break;

                case "off":
                {
                    Config.Enabled = false;
                    Config.Write();
                    plr.SendInfoMessage(GetString("数据包拦截 已关闭"));
                }
                break;

                case "add":
                {
                    if (args.Parameters.Count < 2)
                    {
                        plr.SendErrorMessage(GetString("请提供要添加的玩家名称。"));
                        return;
                    }

                    var name = args.Parameters[1];
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (!Config.PlayerList.Contains(name))
                        {
                            Config.PlayerList.Add(name);
                            Config.Write();
                            plr.SendMessage(GetString($"成功添加玩家 {name} 进入拦截名单"), Color.Aquamarine);
                        }
                        else
                        {
                            plr.SendMessage(GetString("该玩家已存在拦截名单中"), Color.Salmon);
                        }
                    }
                }
                break;

                case "del":
                {
                    if (args.Parameters.Count < 2)
                    {
                        plr.SendErrorMessage(GetString("请提供要移除的玩家名称。"));
                        return;
                    }

                    var name = args.Parameters[1];
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (Config.PlayerList.Contains(name))
                        {
                            Config.PlayerList.Remove(name);
                            Config.Write();
                            plr.SendMessage(GetString($"成功从拦截名单删除玩家: {name} "), Color.Aquamarine);
                        }
                        else
                        {
                            plr.SendMessage(GetString("该玩家不存在拦截名单中"), Color.Salmon);
                        }
                    }
                }
                break;

                case "list":
                {
                    if (Config.PlayerList.Any())
                    {
                        var list = string.Join(", ", Config.PlayerList);
                        plr.SendMessage(GetString($"拦截名单：{list} "), Color.Aquamarine);
                    }
                    else
                    {
                        plr.SendMessage(GetString("拦截名单：无"), Color.Aquamarine);
                    }
                }
                break;

                case "reset":
                {
                    Config.PlayerList.Clear();
                    Config.Write();
                    plr.SendMessage(GetString("成功清空拦截名单"), Color.Aquamarine);
                }
                break;
            }
        }
    }
    #endregion
}
