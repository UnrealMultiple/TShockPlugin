using System.Globalization;
using System.Text;
using Terraria;
using TShockAPI;
using Microsoft.Xna.Framework;

namespace Goodnight
{
    public class Commands
    {
        public static void GnCmd(CommandArgs args)
        {
            #region 默认指令
            if (args.Parameters.Count == 0)
            {
                HelpCmd(args.Player);
                return;
            }
            #endregion

            #region 列出禁止怪物表
            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
            {
                if (Goodnight.Config.Npcs.Count < 1)
                    args.Player.SendInfoMessage("当前禁止怪物生成表为空.");
                else
                    args.Player.SendInfoMessage("禁止怪物生成表: " + string.Join(", ", Goodnight.Config.Npcs.Select(x => TShock.Utils.GetNPCById(x)?.FullName + "({0})".SFormat(x))));
                return;
            }
            #endregion

            #region 开启或者关闭宵禁功能
            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "on")
            {
                bool isEnabled = Goodnight.Config.Enabled;
                Goodnight.Config.Enabled = !isEnabled;
                string statusStr = isEnabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage($"已{statusStr}宵禁功能。");
                Goodnight.Config.Write(Configuration.FilePath);
                return;
            }
            #endregion

            #region 开启或关闭断连功能
            if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "kick")
            {
                bool enabled = Goodnight.Config.DiscPlayers;
                Goodnight.Config.DiscPlayers = !enabled;
                string status = enabled ? "禁用" : "启用";
                args.Player.SendSuccessMessage($"已{status}在宵禁时间断开玩家连接。");
                Goodnight.Config.Write(Configuration.FilePath);
                return;
            }
            #endregion

            #region 修改禁止怪物ID + 修改解禁怪物在线人数 
            if (args.Parameters.Count == 2)
            {
                NPC npc;
                List<NPC> matchedNPCs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);

                if (matchedNPCs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(matchedNPCs.Select(i => i.FullName));
                    return;
                }
                else
                {
                    npc = matchedNPCs[0];
                }

                switch (args.Parameters[0].ToLower())
                {
                    case "add":
                        {
                            if (Goodnight.Config.Npcs.Contains(npc.netID))
                            {
                                args.Player.SendErrorMessage("NPC ID {0} 已在阻止列表中!", npc.netID);
                                return;
                            }
                            Goodnight.Config.Npcs.Add(npc.netID);
                            Goodnight.Config.Write(Configuration.FilePath);
                            args.Player.SendSuccessMessage("已成功将NPC ID添加到阻止列表: {0}!", npc.netID);
                            break;
                        }
                    case "delete":
                    case "del":
                    case "remove":
                        {
                            if (!Goodnight.Config.Npcs.Contains(npc.netID))
                            {
                                args.Player.SendErrorMessage("NPC ID {0} 不在筛选列表中!", npc.netID);
                                return;
                            }
                            Goodnight.Config.Npcs.Remove(npc.netID);
                            Goodnight.Config.Write(Configuration.FilePath);
                            args.Player.SendSuccessMessage("已成功从阻止列表中删除NPC ID: {0}!", npc.netID);
                            break;
                        }

                    case "plr":
                        {
                            if (int.TryParse(args.Parameters[1], out int num))
                            {
                                Goodnight.Config.MaxPlayers = num;
                                Goodnight.Config.Write(Configuration.FilePath);
                                args.Player.SendSuccessMessage("已成功将解禁怪物玩家人数设置为: {0} 人!", npc.netID);
                            }
                            break;
                        }

                    default:
                        {
                            HelpCmd(args.Player);
                            break;
                        }
                }
            }
            #endregion

            #region 修改豁免名单方法
            if (args.Parameters.Count >= 3 && (args.Parameters[0].ToLower() == "hm"))
            {
                switch (args.Parameters[2])
                {
                    case "update":
                        {
                            string text = args.Parameters[3];
                            if (text != null && Goodnight.Config.Add(text))
                                args.Player.SendMessage("成功添加玩家 " + text + " 进入豁免名单", Color.Aquamarine);
                            else
                                args.Player.SendMessage("该玩家已存在豁免名单中", Color.Salmon);
                            break;
                        }
                    case "clear":
                        {
                            string text = args.Parameters[3];
                            if (text != null && Goodnight.Config.Del(text))
                            {
                                TSPlayer plr = TSPlayer.FindByNameOrID(text)[0];
                                if (plr != null && plr.Active && plr.ConnectionAlive)
                                    plr.Disconnect("你已被移出服务器豁免名单");
                                args.Player.SendMessage("成功删除玩家 " + text + " ", Color.Aquamarine);
                            }
                            else
                                args.Player.SendMessage("该玩家不存在豁免名单中", Color.Salmon);
                            break;
                        }
                }
                #endregion

                #region 设置宵禁时间
                if (args.Parameters.Count == 3 && args.Parameters[0].ToLower() == "time")
                {
                    if (!TimeSpan.TryParseExact(args.Parameters[2], "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out TimeSpan SetTime))
                    {
                        args.Player.SendErrorMessage("时间格式错误，请使用HH:mm:ss格式，如02:00:00。");
                        return;
                    }

                    if (SetTime.Hours < 0 || SetTime.Hours > 23 || SetTime.Minutes < 0 || SetTime.Minutes > 59 || SetTime.Seconds < 0 || SetTime.Seconds > 59)
                    {
                        args.Player.SendErrorMessage("时间值超出范围，请确保小时在0-23之间，分钟和秒在0-59之间。");
                        return;
                    }

                    switch (args.Parameters[1].ToLower())
                    {
                        case "set":
                        case "start":
                            Goodnight.Config.Time.Start = SetTime;
                            args.Player.SendSuccessMessage($"已成功设置宵禁开始时间为：{Goodnight.Config.Time.Start}");
                            Goodnight.Config.Write(Configuration.FilePath);
                            break;
                        case "end":
                        case "stop":
                            Goodnight.Config.Time.Stop = SetTime;
                            args.Player.SendSuccessMessage($"已成功设置宵禁结束时间为：{Goodnight.Config.Time.Stop}");
                            Goodnight.Config.Write(Configuration.FilePath);
                            break;
                        default:
                            args.Player.SendInfoMessage("设置宵禁时间: /gn time start 或 stop 02:00:00"); ;
                            return;
                    }
                    Goodnight.LoadConfig();
                    return;
                }
            }
            #endregion
        }

        #region 菜单方法
        private static void HelpCmd(TSPlayer player)
        {
            if (player == null) return;
            else
            {
                player.SendInfoMessage("【宵禁指令菜单】\n" +
                 "/gn --查看宵禁指令菜单\n" +
                 "/gn on --开启或关闭宵禁功能\n" +
                 "/gn kick --开启或关闭断连功能\n" +
                 "/gn time start 或 stop 02:00:00 --设置宵禁开启结束时间\n" +
                 "/gn plr 数字 --设置宵禁时间内解禁怪物的在线人数\n" +
                 "/gn list --列出禁止怪物表\n" +
                 "/gn add NPC名字 或 ID --添加指定禁止召唤怪物\n" +
                 "/gn del NPC名字 或 ID --删除指定禁止召唤怪物\n" +
                 "/reload --重载宵禁配置文件\n" +
                 "—————以下2个指令存在问题—————\n" +
                 "/gn hm update 玩家名字 --添加指定玩家到豁免名单\n" +
                 "/gn hm clear  玩家名字 --把指定玩家从豁免名单移除\n");
            }
        }
        #endregion

    }
}
