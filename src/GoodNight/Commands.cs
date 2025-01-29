using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria;
using TShockAPI;

namespace Goodnight;

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

        #region 列出所有表
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "list")
        {
            args.Player.SendInfoMessage(GetString("—— —— —— —— —— "));
            args.Player.SendInfoMessage(GetString("《禁止怪物生成表》:\n") + string.Join(", ", Goodnight.Config.Npcs.Select(x => TShock.Utils.GetNPCById(x)?.FullName + "({0})".SFormat(x))));
            args.Player.SendInfoMessage(GetString("—— —— —— —— —— "));
            args.Player.SendInfoMessage(GetString("《允许召唤怪物表》:\n") + string.Join(", ", Goodnight.Config.NpcDead.Select(x => TShock.Utils.GetNPCById(x)?.FullName + "({0})".SFormat(x))));
            args.Player.SendInfoMessage(GetString("—— —— —— —— —— "));
            args.Player.SendInfoMessage(GetString($"《不被断连的白名单》:\n {Goodnight.Config.GetExemptPlayersAsString()}"));
            args.Player.SendInfoMessage(GetString("—— —— —— —— —— "));
            return;
        }
        #endregion

        #region 开启或者关闭宵禁功能
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "on")
        {
            Goodnight.Config.Enabled = !Goodnight.Config.Enabled;
            args.Player.SendSuccessMessage(Goodnight.Config.Enabled
                ? GetString("已启用宵禁功能。")
                : GetString("已禁用宵禁功能。"));
            Goodnight.Config.Write();
            return;
        }
        #endregion

        #region 开启或关闭断连功能
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "kick")
        {
            Goodnight.Config.DiscPlayers = !Goodnight.Config.DiscPlayers;
            args.Player.SendSuccessMessage(Goodnight.Config.DiscPlayers 
                ? GetString("已启用在宵禁时间断开玩家连接。")
                : GetString("已禁用在宵禁时间断开玩家连接。"));
            Goodnight.Config.Write();
            return;
        }
        #endregion

        #region 开启或关闭召唤区
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "pos")
        {
            Goodnight.Config.Region = !Goodnight.Config.Region;
            args.Player.SendSuccessMessage(Goodnight.Config.Region
                ? GetString("已启用召唤区，宵禁逻辑已切换。\n可用TS自带的/Region指令建个“召唤区”的领地")
                : GetString("已禁用召唤区，宵禁逻辑已切换。\n可用TS自带的/Region指令建个“召唤区”的领地"));
            Goodnight.Config.Write();
            return;
        }
        #endregion

        #region 开启或关闭所有人在召唤区
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "all")
        {
            Goodnight.Config.PlayersInRegion = !Goodnight.Config.PlayersInRegion;
            args.Player.SendSuccessMessage(Goodnight.Config.PlayersInRegion
                ? GetString("已启用召唤区人数条件，召唤区逻辑已切换。\n【启用】所有人都得在召唤区\n【禁用】有一人在召唤区，其他人可在任意位置召唤")
                : GetString("已禁用召唤区人数条件，召唤区逻辑已切换。\n【启用】所有人都得在召唤区\n【禁用】有一人在召唤区，其他人可在任意位置召唤"));
            Goodnight.Config.Write();
            return;
        }
        #endregion

        #region 清理允许召唤表
        if (args.Parameters.Count == 1 && args.Parameters[0].ToLower() == "clear")
        {
            Goodnight.Config.NpcDead.Clear();
            Goodnight.KillCounters.Clear();
            args.Player.SendSuccessMessage(GetString("已清理《允许召唤表》的怪物ID"));
            Goodnight.Config.Write();
            return;
        }
        #endregion

        #region 修改禁止怪物ID + 修改解禁怪物在线人数 
        if (args.Parameters.Count == 2)
        {
            NPC npc;
            var matchedNPCs = TShock.Utils.GetNPCByIdOrName(args.Parameters[1]);
            if (matchedNPCs.Count > 1)
            {
                args.Player.SendMultipleMatchError(matchedNPCs.Select(i => i.FullName));
                return;
            }

            if (matchedNPCs.Count == 0)
            {
                args.Player.SendErrorMessage(GetString("不存在的NPC"));
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
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 已在阻止列表中!"), npc.netID);
                        return;
                    }
                    Goodnight.Config.Npcs.Add(npc.netID);
                    Goodnight.Config.Write();
                    args.Player.SendSuccessMessage(GetString("已成功将NPC ID添加到阻止列表: {0}!"), npc.netID);
                    break;
                }
                case "delete":
                case "del":
                case "remove":
                {
                    if (!Goodnight.Config.Npcs.Contains(npc.netID))
                    {
                        args.Player.SendErrorMessage(GetString("NPC ID {0} 不在筛选列表中!"), npc.netID);
                        return;
                    }
                    Goodnight.Config.Npcs.Remove(npc.netID);
                    Goodnight.Config.Write();
                    args.Player.SendSuccessMessage(GetString("已成功从阻止列表中删除NPC ID: {0}!"), npc.netID);
                    break;
                }

                case "plr":
                {
                    if (int.TryParse(args.Parameters[1], out var num))
                    {
                        Goodnight.Config.MaxPlayers = num;
                        Goodnight.Config.Write();
                        args.Player.SendSuccessMessage(GetString("已成功将解禁怪物玩家人数设置为: {0} 人!"), npc.netID);
                    }
                    break;
                }

                case "boss":
                {
                    if (int.TryParse(args.Parameters[1], out var num))
                    {
                        Goodnight.Config.DeadCount = num;
                        Goodnight.Config.Write();
                        args.Player.SendSuccessMessage(GetString("已成功将记录进度击杀次数设置为: {0} 次!"), npc.netID);
                    }
                    break;
                }

                case "reset":
                {
                    if (int.TryParse(args.Parameters[1], out var num))
                    {
                        Goodnight.Config.ResetNpcDead = num;
                        Goodnight.Config.Write();
                        args.Player.SendSuccessMessage(GetString("已设置重置允许召唤表的怪物ID为: {0} !"), npc.netID);
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

        #region 白名单方法
        if (args.Parameters.Count == 3 && (args.Parameters[0].ToLower() == "plr"))
        {
            switch (args.Parameters[1].ToLower())
            {
                case "add":
                {
                    var text = args.Parameters[2];
                    if (!string.IsNullOrEmpty(text) && Goodnight.Config.Add(text))
                    {
                        args.Player.SendMessage(GetString($"成功添加玩家 {text} 进入白名单"), Color.Aquamarine);
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("该玩家已存在白名单中"), Color.Salmon);
                    }

                    break;
                }
                case "del":
                {
                    var text = args.Parameters[2];
                    if (!string.IsNullOrEmpty(text) && Goodnight.Config.Del(text))
                    {
                        var plrs = TShock.Players.Where
                        (x => x != null && x.Active && x.ConnectionAlive && x.Name == text).ToList();
                        if (plrs.Any())
                        {
                            if (!plrs[0].HasPermission("goodnight.admin"))
                            {
                                plrs[0].Disconnect(GetString("【宵禁】你已被移出服务器白名单"));
                            }
                        }

                        args.Player.SendMessage(GetString($"成功删除玩家白名单： {text} "), Color.Aquamarine);
                    }
                    else
                    {
                        args.Player.SendMessage(GetString("该玩家不存在白名单中"), Color.Salmon);
                    }

                    break;
                }
            }
        }
        #endregion

        #region 设置宵禁时间
        if (args.Parameters.Count == 3 && args.Parameters[0].ToLower() == "time")
        {
            if (!TimeSpan.TryParseExact(args.Parameters[2], "hh\\:mm", CultureInfo.InvariantCulture, out var SetTime))
            {
                args.Player.SendErrorMessage(GetString("时间格式错误，请使用HH:mm格式：23:59\n请确保小时在0-23之间，分钟在0-59之间"));
                return;
            }

            switch (args.Parameters[1].ToLower())
            {
                case "a":
                case "set":
                case "start":
                    Goodnight.Config.Time.Start = SetTime;
                    args.Player.SendSuccessMessage(GetString($"已成功设置宵禁开始时间为：{Goodnight.Config.Time.Start}"));
                    Goodnight.Config.Write();
                    break;
                case "b":
                case "end":
                case "stop":
                    Goodnight.Config.Time.Stop = SetTime;
                    args.Player.SendSuccessMessage(GetString($"已成功设置宵禁结束时间为：{Goodnight.Config.Time.Stop}"));
                    Goodnight.Config.Write();
                    break;
                default:
                    args.Player.SendInfoMessage(GetString("设置宵禁时间: /gn time a 或 b 23:59")); ;
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
        if (player == null)
        {
            return;
        }
        else
        {
            player.SendMessage(GetString("【宵禁指令菜单】\n") +
             GetString("/gn —— 查看宵禁指令菜单\n") +
             GetString("/gn list —— 列出所有宵禁表\n") +
             GetString("/reload —— 重载宵禁配置文件\n") +
             GetString("/gn on —— 开启|关闭宵禁功能\n") +
             GetString("/gn kick —— 开启|关闭断连功能\n") +
             GetString("/gn pos —— 开启|关闭召唤区\n") +
             GetString("/gn all —— 开启|关闭召唤区需所有人在场\n") +
             GetString("/gn clear —— 清理《允许召唤表》\n") +
             GetString("/gn boss 次数 —— 设置加入《允许召唤表》击杀要求次数\n") +
             GetString("/gn reset ID —— 设置重置《允许召唤表》的怪物ID\n") +
             GetString("/gn plr 人数 —— 设置无视《禁止怪物表》在线人数\n") +
             GetString("/gn plr add 或 del 玩家名字 —— 添加|移除指定玩家到断连白名单\n") +
             GetString("/gn add 或 del 怪物名字 —— 添加|删除《禁止怪物表》的指定怪物\n") +
             GetString("/gn time a 或 b 23:59 —— 设置宵禁开启结束时间\n") +
             GetString("/region define 召唤区 —— 使用TS自带/Region指令设置召唤区"), Color.AntiqueWhite);
        }
    }
    #endregion
}