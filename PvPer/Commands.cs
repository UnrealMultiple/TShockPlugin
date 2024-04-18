using Microsoft.Xna.Framework;
using TShockAPI;

namespace PvPer
{
    public class Commands
    {
        public static void Duel(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                HelpCmd(args);
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
                case "h":
                case "help":
                case "菜单":
                    if (args.Parameters.Count < 2)
                    {
                        HelpCmd(args);
                    }
                    return; //结束
                case "0":
                case "add":
                case "邀请":
                    if (args.Parameters.Count < 2)
                    {
                        args.Player.SendErrorMessage("请指定目标玩家的名称。");
                    }
                    else
                    {
                        InviteCmd(args);
                    }
                    return; //结束
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
                case "s":
                case "set":
                case "设置":
                    {
                        int result;
                        if (args.Parameters.Count == 2 && int.TryParse(args.Parameters[1], out result) && IsValidLocationType(result))
                        {
                            int x = args.Player.TileX;
                            int y = args.Player.TileY;

                            switch (result)
                            {
                                case 1:
                                    PvPer.Config.Player1PositionX = x;
                                    PvPer.Config.Player1PositionY = y;
                                    args.Player.SendMessage($"已将你所在的位置设置为[c/F75454:邀请者]传送点，坐标为({x}, {y})", Color.CadetBlue);
                                    Console.WriteLine($"【决斗系统】邀请者传送点已设置，坐标为({x}, {y})", Color.BurlyWood);
                                    break;
                                case 2:
                                    PvPer.Config.Player2PositionX = x;
                                    PvPer.Config.Player2PositionY = y;
                                    args.Player.SendMessage($"已将你所在的位置设置为[c/49B3D6:受邀者]传送点，坐标为({x}, {y})", Color.CadetBlue);
                                    Console.WriteLine($"【决斗系统】受邀者传送点已设置，坐标为({x}, {y})", Color.BurlyWood);
                                    break;

                                case 3:
                                    PvPer.Config.ArenaPosX1 = x;
                                    PvPer.Config.ArenaPosY1 = y;
                                    args.Player.SendMessage($"已将你所在的位置设置为[c/9487D6:竞技场]左上角，坐标为({x}, {y})", Color.Yellow);
                                    Console.WriteLine($"【决斗系统】竞技场左上角已设置，坐标为({x}, {y})", Color.Yellow);
                                    break;
                                case 4:
                                    PvPer.Config.ArenaPosX2 = x;
                                    PvPer.Config.ArenaPosY2 = y;
                                    args.Player.SendMessage($"已将你所在的位置设置为[c/9487D6:竞技场]右下角，坐标为({x}, {y})", Color.Yellow);
                                    Console.WriteLine($"【决斗系统】竞技场右下角已设置，坐标为({x}, {y})", Color.Yellow);
                                    break;

                                default:
                                    args.Player.SendErrorMessage("[i:4080]指令错误! [c/CCEB60:正确指令: /pvp set [1/2/3/4]]");
                                    return;
                            }

                            PvPer.Config.Write(Configuration.FilePath);
                        }
                        else
                        {
                            args.Player.SendErrorMessage("[i:4080]指令错误! \n正确指令: /pvp set [1/2/3/4] - [c/7EE874:1/2玩家位置 3/4竞技场边界]");
                        }
                        break;
                    }
                case "r":
                case "reset":
                case "重置":
                    if (args.Parameters.Count < 2)
                    {
                        var name = args.Player.Name;
                        // 权限
                        if (!args.Player.HasPermission("pvper.admin"))
                        {
                            args.Player.SendErrorMessage("你没有重置决斗系统数据表的权限。");
                            TShock.Log.ConsoleInfo($"{name}试图执行重置决斗系统数据指令");
                            return;
                        }
                        else
                        {
                            ClearAllData(args);
                        }
                    }
                    return; //结束
                default:
                    HelpCmd(args);
                    break;
            }
        }


        private static void HelpCmd(CommandArgs args)
        {
            if (args.Player != null)
            {
                args.Player.SendMessage("【决斗系统】请参考以下指令菜单：\n " +
                 "[c/FFFE80:/pvp add 或 /pvp 邀请 玩家名] - [c/7EE874:邀请玩家参加决斗] \n " +
                 "[c/74D3E8:/pvp yes 或 /pvp 接受] - [c/7EE874:接受决斗] \n " +
                 "[c/74D3E8:/pvp no 或 /pvp 拒绝] - [c/7EE874:拒绝决斗] \n " +
                 "[c/74D3E8:/pvp data 或 /pvp 战绩] - [c/7EE874:战绩查询]\n " +
                 "[c/74D3E8:/pvp list 或 /pvp 排名] - [c/7EE874:排名]\n " +
                 "[c/FFFE80:/pvp s 或 /pvp 设置 1 2 3 4] - [c/7EE874:1/2玩家位置 3/4竞技场边界]\n " +
                 "[c/74D3E8:/pvp r 或 /pvp 重置] - [c/7EE874:重置玩家数据库]\n ", Color.GreenYellow);
            }
        }

        #region 使用指令清理数据库、设置位置方法
        private static void ClearAllData(CommandArgs args)
        {
            // 尝试从数据库中删除所有玩家数据
            if (DbManager.ClearData())
            {
                args.Player.SendSuccessMessage("数据库中所有玩家的决斗数据已被成功清除。");
                TShock.Log.ConsoleInfo("数据库中所有玩家的决斗数据已被成功清除。");
            }
            else
            {
                args.Player.SendErrorMessage("清除所有玩家决斗数据时发生错误。");
                TShock.Log.ConsoleInfo("清除所有玩家决斗数据时发生错误。");
            }
        }
        #endregion

        private static bool IsValidLocationType(int locationType)
        {
            return locationType >= 1 && locationType <= 4;
        }



        private static void InviteCmd(CommandArgs args)
        {
            List<TSPlayer> plrList = TSPlayer.FindByNameOrID(string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)));


            if (plrList.Count == 0)
            {
                args.Player.SendErrorMessage("未找到指定玩家。");
                return;
            }

            if (Utils.IsPlayerInADuel(args.Player.Index))
            {
                args.Player.SendErrorMessage("您现在已经在决斗中了。");
                return;
            }

            TSPlayer targetPlr = plrList[0];

            if (targetPlr.Index == args.Player.Index)
            {
                args.Player.SendErrorMessage("您不能与自己决斗！");
                return;
            }

            if (Utils.IsPlayerInADuel(targetPlr.Index))
            {
                args.Player.SendErrorMessage($"{targetPlr.Name} 正在进行一场决斗。");
                return;
            }

            PvPer.Invitations.Add(new Pair(args.Player.Index, targetPlr.Index));
            args.Player.SendSuccessMessage($"成功邀请 {targetPlr.Name} 进行决斗。");
            targetPlr.SendMessage($"{args.Player.Name} [c/FE7F81:已向您发送决斗邀请] \n请输入 [c/CCFFCC:/pvp yes 接受]  或 [c/FFE6CC:/pvp no拒绝] ", 255, 204, 255);
        }

        private static void AcceptCmd(CommandArgs args)
        {
            Pair? invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

            if (invitation == null)
            {
                args.Player.SendErrorMessage("[c/FE7F81:您当前没有收到任何决斗邀请]");
                return;
            }

            invitation.StartDuel();
        }

        private static void RejectCommand(CommandArgs args)
        {
            Pair? invitation = Utils.GetInvitationFromReceiverIndex(args.Player.Index);

            if (invitation == null)
            {
                args.Player.SendErrorMessage("[c/FE7F81:您当前没有收到任何决斗邀请]");
                return;
            }

            TShock.Players[invitation.Player1].SendErrorMessage("[c/FFCB80:对方玩家已拒绝您的决斗邀请]。");
            PvPer.Invitations.Remove(invitation);
        }

        private static void StatsCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                try
                {
                    DPlayer plr = PvPer.DbManager.GetDPlayer(args.Player.Account.ID);
                    args.Player.SendInfoMessage("[c/FFCB80:您的战绩:]\n" +
                                                $"[c/63DC5A:击杀: ]{plr.Kills}\n" +
                                                $"[c/F56469:死亡:] {plr.Deaths}\n" +
                                                $"[c/F56469:连胜:] {plr.WinStreak}\n" +
                                                $"击杀/死亡 [c/5993DB:胜负值: ]{plr.GetKillDeathRatio()}");
                }
                catch (NullReferenceException)
                {
                    args.Player.SendErrorMessage("玩家未找到！");
                }
            }
            else
            {
                try
                {
                    string name = string.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                    List<TShockAPI.DB.UserAccount> matchedAccounts = TShock.UserAccounts.GetUserAccountsByName(name);

                    if (matchedAccounts.Count == 0)
                    {
                        args.Player.SendErrorMessage("玩家未找到！");
                        return;
                    }

                    DPlayer plr = PvPer.DbManager.GetDPlayer(matchedAccounts[0].ID);
                    args.Player.SendInfoMessage("[c/FFCB80:您的战绩:]\n" +
                                                $"[c/63DC5A:击杀: ]{plr.Kills}\n" +
                                                $"[c/F56469:死亡:] {plr.Deaths}\n" +
                                                $"[c/F56469:连胜:] {plr.WinStreak}\n" +
                                                $"击杀/死亡 [c/5993DB:胜负值: ]{plr.GetKillDeathRatio()}");
                }
                catch (NullReferenceException)
                {
                    args.Player.SendErrorMessage("玩家未找到！");
                }
            }
        }

        private static void LeaderboardCommand(CommandArgs args)
        {
            Task.Run(() =>
            {
                string message = "";
                List<DPlayer> list = PvPer.DbManager.GetAllDPlayers();

                list.Sort((p1, p2) =>
                {
                    if (p1.GetKillDeathRatio() >= p2.GetKillDeathRatio())
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                });

                DPlayer p;

                if (list.TryGetValue(0, out p))
                {
                    message += $"{1}. {TShock.UserAccounts.GetUserAccountByID(p.AccountID).Name} : {p.GetKillDeathRatio():F2}";
                }

                for (int i = 1; i < 5; i++)
                {
                    if (list.TryGetValue(i, out p))
                    {
                        message += $"\n{i + 1}. {TShock.UserAccounts.GetUserAccountByID(p.AccountID).Name} : {p.GetKillDeathRatio():F2}";
                    }
                }
                args.Player.SendInfoMessage(message);
            });
        }
    }
}