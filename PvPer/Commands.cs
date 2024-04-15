using Steamworks;
using TShockAPI;

namespace PvPer
{
    public class Commands
    {
        public static void Duel(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("决斗系统指令菜单：\n " +
                "[c/74D3E8:/pvp add 玩家名] - [c/7EE874:邀请玩家参加决斗] \n " +
                "[c/74D3E8:/pvp yes] - [c/7EE874:接受决斗] \n " +
                "[c/74D3E8:/pvp no] - [c/7EE874:拒绝决斗] \n " +
                "[c/74D3E8:/pvp data] - [c/7EE874:战绩查询]\n " +
                "[c/FFFE80:/pvp list] - [c/7EE874:排名]\n ");
                return;
            }

            switch (args.Parameters[0].ToLower())
            {
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
                    return;
                case "yes":
                case "接受":
                    AcceptCmd(args);
                    return;
                case "no":
                case "拒绝":
                    RejectCommand(args);
                    return;
                case "data":
                case "战绩":
                    StatsCommand(args);
                    return;
                case "l":
                case "list":
                case "排名":
                    LeaderboardCommand(args);
                    return;
                default:
                    args.Player.SendErrorMessage("决斗系统指令菜单：\n " +
                    "[c/74D3E8:/pvp add 玩家名] - [c/7EE874:邀请玩家参加决斗] \n " +
                    "[c/74D3E8:/pvp yes] - [c/7EE874:接受决斗] \n " +
                    "[c/74D3E8:/pvp no] - [c/7EE874:拒绝决斗] \n " +
                    "[c/74D3E8:/pvp data] - [c/7EE874:战绩查询]\n " +
                    "[c/FFFE80:/pvp list] - [c/7EE874:排名]\n ");
                    return;
            }
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