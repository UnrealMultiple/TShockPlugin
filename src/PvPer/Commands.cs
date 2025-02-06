using Microsoft.Xna.Framework;
using TShockAPI;


namespace PvPer;

public class Commands
{
    public static void Duel(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendMessage(GetString("请输入 [c/42B2CE:/pvp help] [c/F25E61:共两页]"), Color.YellowGreen);
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "h":
            case "help":
            case "菜单":
                args.Parameters.RemoveAt(0);
                HelpCmd(args);
                return;

            case "0":
            case "add":
            case "邀请":
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("请指定目标玩家的名称。"));
                }
                else
                {
                    InviteCmd(args);
                }
                return;

            case "1":
            case "yes":
            case "接受":
                AcceptCmd(args);
                return;

            case "2":
            case "no":
            case "拒绝":
                RejectCommand(args);
                return;

            case "data":
            case "mark":
            case "战绩":
                StatsCommand(args);
                return;

            case "l":
            case "list":
            case "排名":
                LeaderboardCommand(args);
                return;

            case "wl":
                args.Player.SendMessage(GetString("注意：/pvp.WL 中间有个英文字符[c/F75454:“点”【 . 】]"), Color.YellowGreen);
                return;

            case "bl":
                args.Player.SendMessage(GetString("注意：/pvp.BL 中间有个英文字符[c/F75454:“点”【 . 】]"), Color.YellowGreen);
                return;

            case "bb":
                args.Player.SendMessage(GetString("注意：/pvp.BB 中间有个英文字符[c/F75454:“点”【 . 】]"), Color.YellowGreen);
                return;

            case "bw":
                args.Player.SendMessage(GetString("注意：/pvp.BW 中间有个英文[c/F75454:“点”【 . 】]"), Color.YellowGreen);
                return;

            case "s":
            case "set":
            case "设置":
            {
                if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out var result) && IsValidLocationType(result))
                {
                    var x = args.Player.TileX;
                    var y = args.Player.TileY;

                    switch (result)
                    {
                        case 1:
                            PvPer.Config.Player1PositionX = x;
                            PvPer.Config.Player1PositionY = y;
                            args.Player.SendMessage(GetString($"已将你所在的位置设置为[c/F75454:邀请者]传送点，坐标为({x}, {y})"), Color.CadetBlue);
                            Console.WriteLine(GetString($"【决斗系统】邀请者传送点已设置，坐标为({x}, {y})"), Color.BurlyWood);
                            break;
                        case 2:
                            PvPer.Config.Player2PositionX = x;
                            PvPer.Config.Player2PositionY = y;
                            args.Player.SendMessage(GetString($"已将你所在的位置设置为[c/49B3D6:受邀者]传送点，坐标为({x}, {y})"), Color.CadetBlue);
                            Console.WriteLine(GetString($"【决斗系统】受邀者传送点已设置，坐标为({x}, {y})"), Color.BurlyWood);
                            break;

                        case 3:
                            PvPer.Config.ArenaPosX1 = x;
                            PvPer.Config.ArenaPosY1 = y;
                            args.Player.SendMessage(GetString($"已将你所在的位置设置为[c/9487D6:竞技场]左上角，坐标为({x}, {y})"), Color.Yellow);
                            Console.WriteLine(GetString($"【决斗系统】竞技场左上角已设置，坐标为({x}, {y})"), Color.Yellow);
                            break;
                        case 4:
                            PvPer.Config.ArenaPosX2 = x;
                            PvPer.Config.ArenaPosY2 = y;
                            args.Player.SendMessage(GetString($"已将你所在的位置设置为[c/9487D6:竞技场]右下角，坐标为({x}, {y})"), Color.Yellow);
                            Console.WriteLine(GetString($"【决斗系统】竞技场右下角已设置，坐标为({x}, {y})"), Color.Yellow);
                            break;

                        default:
                            args.Player.SendErrorMessage(GetString("[i:4080]指令错误! [c/CCEB60:正确指令: /pvp set [1/2/3/4]]"));
                            return;
                    }

                    PvPer.Config.Write(Configuration.FilePath);
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("[i:4080]指令错误! \n正确指令: /pvp set [1/2/3/4] - [c/7EE874:1/2玩家位置 3/4竞技场边界]"));
                }
                break;
            }

            case "r":
            case "reset":
            case "重置":
                if (args.Parameters.Count < 2)
                {
                    var name = args.Player.Name;
                    if (!args.Player.HasPermission("pvper.admin"))
                    {
                        args.Player.SendErrorMessage(GetString("你没有重置决斗系统数据表的权限。"));
                        TShock.Log.ConsoleInfo(GetString($"{name}试图执行重置决斗系统数据指令"));
                        return;
                    }
                    else
                    {
                        ClearAllData(args);
                    }
                }
                return; //结束
            default:
                args.Player.SendErrorMessage(GetString("请输入/pvp help [c/F75454:共两页]"), Color.YellowGreen);
                return;
        }
    }


    private static void HelpCmd(CommandArgs args)
    {
        if (args.Player != null)
        {
            var page = 1;
            if (args.Parameters.Count > 0)
            {
                if (int.TryParse(args.Parameters[0], out page) && page >= 1)
                {
                    args.Parameters.RemoveAt(0);
                }
                else
                {
                    page = 1;
                }
            }

            string helpMessage;

            switch (page)
            {
                case 1:
                    helpMessage = "———————\n " +
                                 GetString("《决斗系统》 第1页 （1/2）：\n ") +
                                 GetString("[c/FFFE80:/pvp add 或 /pvp 邀请 玩家名] - [c/7EE874:邀请玩家参加决斗] \n ") +
                                 GetString("[c/74D3E8:/pvp yes 或 /pvp 接受] - [c/7EE874:接受决斗] \n ") +
                                 GetString("[c/74D3E8:/pvp no 或 /pvp 拒绝] - [c/7EE874:拒绝决斗] \n ") +
                                 GetString("[c/74D3E8:/pvp data 或 /pvp 战绩] - [c/7EE874:战绩查询]\n ") +
                                 GetString("[c/74D3E8:/pvp list 或 /pvp 排名] - [c/7EE874:排名]\n ") +
                                 GetString("[c/FFFE80:/pvp s 或 /pvp 设置 1 2 3 4] - [c/7EE874:1/2玩家位置 3/4竞技场边界]");
                    break;

                case 2:
                    helpMessage = "———————\n " +
                                 GetString("《决斗系统》 第2页 （2/2）：\n ") +
                                 GetString("[c/74D3E8:/pvp.WL ] - [c/7EE874:查看封禁武器表]\n ") +
                                 GetString("[c/74D3E8:/pvp.BL ] - [c/7EE874:查看封禁增益表]\n ") +
                                 GetString("[c/74D3E8:/pvp.BW add|del <武器名> ] - [c/7EE874:封禁指定武器]\n ") +
                                 GetString("[c/74D3E8:/pvp.BB add|del <增益名/ID> ] - [c/7EE874:封禁指定Buff]\n ") +
                                 GetString("[c/74D3E8:/pvp R 或 /pvp 重置] - [c/7EE874:重置玩家数据库]");
                    break;

                default:
                    HelpCmd(args);
                    return;
            }

            args.Player.SendMessage(helpMessage, Color.GreenYellow);
        }
    }

    #region 使用指令清理数据库、设置位置方法
    private static void ClearAllData(CommandArgs args)
    {
        // 尝试从数据库中删除所有玩家数据
        if (DbManager.ClearData())
        {
            args.Player.SendSuccessMessage(GetString("数据库中所有玩家的决斗数据已被成功清除。"));
            TShock.Log.ConsoleInfo(GetString("数据库中所有玩家的决斗数据已被成功清除。"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("清除所有玩家决斗数据时发生错误。"));
            TShock.Log.ConsoleInfo(GetString("清除所有玩家决斗数据时发生错误。"));
        }
    }
    #endregion

    private static bool IsValidLocationType(int locationType)
    {
        return locationType >= 1 && locationType <= 4;
    }

    private static void InviteCmd(CommandArgs args)
    {
        var plrList = TSPlayer.FindByNameOrID(string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)));


        if (plrList.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("未找到指定玩家。"));
            return;
        }

        if (Utils.IsPlayerInADuel(args.Player.Index))
        {
            args.Player.SendErrorMessage(GetString("您现在已经在决斗中了。"));
            return;
        }

        var targetPlr = plrList[0];

        if (targetPlr.Index == args.Player.Index)
        {
            args.Player.SendErrorMessage(GetString("您不能与自己决斗！"));
            return;
        }

        if (Utils.IsPlayerInADuel(targetPlr.Index))
        {
            args.Player.SendErrorMessage(GetString($"{targetPlr.Name} 正在进行一场决斗。"));
            return;
        }

        PvPer.Invitations.Add(new Pair(args.Player.Index, targetPlr.Index));
        args.Player.SendSuccessMessage(GetString($"成功邀请 {targetPlr.Name} 进行决斗。"));
        targetPlr.SendMessage(GetString($"{args.Player.Name} [c/FE7F81:已向您发送决斗邀请] \n请输入 [c/CCFFCC:/pvp yes 接受]  或 [c/FFE6CC:/pvp no拒绝] "), 255, 204, 255);
    }

    private static void AcceptCmd(CommandArgs args)
    {
        var invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

        if (invitation == null)
        {
            args.Player.SendErrorMessage(GetString("[c/FE7F81:您当前没有收到任何决斗邀请]"));
            return;
        }

        invitation.StartDuel();
    }

    private static void RejectCommand(CommandArgs args)
    {
        var invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

        if (invitation == null)
        {
            args.Player.SendErrorMessage(GetString("[c/FE7F81:您当前没有收到任何决斗邀请]"));
            return;
        }

        TShock.Players[invitation.Player1].SendErrorMessage(GetString("[c/FFCB80:对方玩家已拒绝您的决斗邀请]。"));
        PvPer.Invitations.Remove(invitation);
    }

    private static void StatsCommand(CommandArgs args)
    {
        if (args.Parameters.Count < 2)
        {
            try
            {
                var plr = PvPer.DbManager.GetDPlayer(args.Player.Account.ID);
                args.Player.SendInfoMessage(GetString("[c/FFCB80:您的战绩:]\n") +
                                            GetString($"[c/63DC5A:击杀: ]{plr.Kills}\n") +
                                            GetString($"[c/F56469:死亡:] {plr.Deaths}\n") +
                                            GetString($"[c/F56469:连胜:] {plr.WinStreak}\n") +
                                            GetString($"击杀/死亡 [c/5993DB:胜负值: ]{plr.GetKillDeathRatio()}"));
            }
            catch (NullReferenceException)
            {
                args.Player.SendErrorMessage(GetString("玩家未找到！"));
            }
        }
        else
        {
            try
            {
                var name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                var matchedAccounts = TShock.UserAccounts.GetUserAccountsByName(name);

                if (matchedAccounts.Count == 0)
                {
                    args.Player.SendErrorMessage(GetString("玩家未找到！"));
                    return;
                }

                var plr = PvPer.DbManager.GetDPlayer(matchedAccounts[0].ID);
                args.Player.SendInfoMessage(GetString("[c/FFCB80:您的战绩:]\n") +
                                            GetString($"[c/63DC5A:击杀: ]{plr.Kills}\n") +
                                            GetString($"[c/F56469:死亡:] {plr.Deaths}\n") +
                                            GetString($"[c/F56469:连胜:] {plr.WinStreak}\n") +
                                            GetString($"击杀/死亡 [c/5993DB:胜负值: ]{plr.GetKillDeathRatio()}"));
            }
            catch (NullReferenceException)
            {
                args.Player.SendErrorMessage(GetString("玩家未找到！"));
            }
        }
    }

    private static void LeaderboardCommand(CommandArgs args)
    {
        Task.Run(() =>
        {
            var message = "";
            var list = PvPer.DbManager.GetAllDPlayers();

            list.Sort((p1, p2) => p1.GetKillDeathRatio() >= p2.GetKillDeathRatio() ? -1 : 1);


            if (list.TryGetValue(0, out var p))
            {
                message += $"{1}. {TShock.UserAccounts.GetUserAccountByID(p.AccountID).Name} : {p.GetKillDeathRatio():F2}";
            }

            for (var i = 1; i < 5; i++)
            {
                if (list.TryGetValue(i, out p))
                {
                    message += $"\n{i + 1}. {TShock.UserAccounts.GetUserAccountByID(p!.AccountID).Name} : {p.GetKillDeathRatio():F2}";
                }
            }
            args.Player.SendInfoMessage(message);
        });
    }
}