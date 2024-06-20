using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using TShockAPI;

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
                    args.Player.SendInfoMessage("当前阻止表为空.");
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
                Goodnight.Config.Write();
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
                Goodnight.Config.Write();
                return;
            }
            #endregion

            #region 修改禁止怪物ID + 修改解禁怪物在线人数等等
            if (args.Parameters.Count == 2)
            {
                NPC npc;
                List<NPC> matchedNPCs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);
                if (matchedNPCs.Count > 1)
                {
                    args.Player.SendMultipleMatchError(matchedNPCs.Select(i => i.FullName));
                    return;
                }

               if (matchedNPCs.Count == 0)
                {
                    args.Player.SendErrorMessage("不存在的NPC");
                    return;
                }

                else 
                    npc = matchedNPCs[0]; 

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
                            Goodnight.Config.Write();
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
                            Goodnight.Config.Write();
                            args.Player.SendSuccessMessage("已成功从阻止列表中删除NPC ID: {0}!", npc.netID);
                            break;
                        }

                    case "plr":
                        {
                            if (int.TryParse(args.Parameters[1], out int num))
                            {
                                Goodnight.Config.MaxPlayers = num;
                                Goodnight.Config.Write();
                                args.Player.SendSuccessMessage("已成功将解禁怪物玩家人数设置为: {0} 人!", npc.netID);
                            }
                            break;
                        }

                    case "boss":
                        {
                            if (int.TryParse(args.Parameters[1], out int num))
                            {
                                Goodnight.Config.DeadCount = num;
                                Goodnight.Config.Write();
                                args.Player.SendSuccessMessage("已成功将记录进度击杀次数设置为: {0} 次!", npc.netID);
                            }
                            break;
                        }

                    case "reset":
                        {
                            if (int.TryParse(args.Parameters[1], out int num))
                            {
                                Goodnight.Config.ResetNpcDead = num;
                                Goodnight.Config.Write();
                                args.Player.SendSuccessMessage("已设置重置允许召唤表的怪物ID为: {0} !", npc.netID);
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
            if (args.Parameters.Count == 3 && (args.Parameters[0].ToLower() == "plr"))
            {
                switch (args.Parameters[1].ToLower())
                {
                    case "add":
                        {
                            string text = args.Parameters[2];
                            if (!String.IsNullOrEmpty(text) && Goodnight.Config.Add(text))
                                args.Player.SendMessage("成功添加玩家 " + text + " 进入豁免名单", Color.Aquamarine);
                            else
                                args.Player.SendMessage("该玩家已存在豁免名单中", Color.Salmon);
                            break;
                        }
                    case "del":
                        {
                            string text = args.Parameters[2];
                            if (!String.IsNullOrEmpty(text) && Goodnight.Config.Del(text))
                            {
                                var plrs = TShock.Players.Where
                                (x => x != null && x.Active && x.ConnectionAlive && x.Name == text).ToList();
                                if (plrs.Any())
                                    if (!plrs[0].HasPermission("goodnight.admin"))
                                        plrs[0].Disconnect("【宵禁】你已被移出服务器豁免名单");
                                args.Player.SendMessage("成功删除玩家豁免名单： " + text + " ", Color.Aquamarine);
                            }
                            else
                                args.Player.SendMessage("该玩家不存在豁免名单中", Color.Salmon);
                            break;
                        }
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
                        Goodnight.Config.Write();
                        break;
                    case "end":
                    case "stop":
                        Goodnight.Config.Time.Stop = SetTime;
                        args.Player.SendSuccessMessage($"已成功设置宵禁结束时间为：{Goodnight.Config.Time.Stop}");
                        Goodnight.Config.Write();
                        break;
                    default:
                        args.Player.SendInfoMessage("设置宵禁时间: /gn time start 或 stop 02:00:00"); ;
                        return;
                }
                Goodnight.LoadConfig();
                return;
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
                 "/gn time start 或 stop 23:59:59 --设置宵禁开启结束时间\n" +
                 "/gn add NPC名字 或 ID --添加《禁止怪物生成表》的指定怪物\n" +
                 "/gn del NPC名字 或 ID --删除《禁止怪物生成表》的指定怪物\n" +
                 "/gn list --列出《禁止怪物生成表》\n" +
                 "/gn boss 次数 --设置加入《允许召唤怪物表》击杀要求次数\n" +
                 "/gn reset ID --设置重置《允许召唤怪物表》的怪物ID\n" +
                 "/gn plr 人数 --设置宵禁时段无视《禁止怪物生成表》在线人数\n" +
                 "/gn plr add 玩家名字 --添加指定玩家到断连豁免名单\n" +
                 "/gn plr del 玩家名字 --把指定玩家从豁免名单移除\n" +
                 "/reload --重载宵禁配置文件\n");
            }
        }
        #endregion

    }
}
