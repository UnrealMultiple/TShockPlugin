using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace VotePlus;

[ApiVersion(2, 1)]
public class VotePlus : TerrariaPlugin
{

    public override string Author => "Cai";

    public override string Description => GetString("投票插件");

    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 4);

    public VotePlus(Main game)
    : base(game)
    {
    }

    public static Dictionary<string, DateTime> VoteKicks = new Dictionary<string, DateTime>();

    //public static Dictionary<string, DateTime> VoteMutes = new Dictionary<string, DateTime>();

    public Vote? Votes = null;

    public static short Timer = 0;

    public override void Initialize()
    {
        Config.GetConfig();
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        Commands.ChatCommands.Add(new Command("vote.use", this.VoteHandler, "vote"));
        Commands.ChatCommands.Add(new Command("vote.use", this.VoteAgree, "agree", "同意"));
        Commands.ChatCommands.Add(new Command("vote.use", this.VoteDisagree, "disagree", "反对"));
        TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += this.PlayerHooks_PlayerPostLogin;

    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.VoteHandler || c.CommandDelegate == this.VoteAgree || c.CommandDelegate == this.VoteDisagree);
            TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= this.PlayerHooks_PlayerPostLogin;

        }
        base.Dispose(disposing);
    }
    private void PlayerHooks_PlayerPostLogin(TShockAPI.Hooks.PlayerPostLoginEventArgs e)
    {
        var plr = e.Player;
        if (VoteKicks.ContainsKey(plr.Account.Name))
        {
            if (VoteKicks[plr.Account.Name] > DateTime.Now)
            {
                plr.Disconnect(GetString($"你被投票踢出了服务器!解除时间剩余:{(int) (VoteKicks[plr.Account.Name] - DateTime.Now).TotalSeconds}秒"));
            }
            else
            {
                VoteKicks.Remove(plr.Account.Name);
            }
        }
    }

    private void VoteDisagree(CommandArgs args)
    {
        if (this.Votes == null)
        {
            args.Player.SendErrorMessage(GetString("[i:4344]当前没有正在进行的投票!"));
            return;
        }
        if (this.Votes.Argeement.Contains(args.Player.Account.Name) || this.Votes.Disargeement.Contains(args.Player.Account.Name))
        {
            args.Player.SendWarningMessage(GetString("[i:4344]你已经投过票了!"));
            return;
        }
        this.Votes.Disargeement.Add(args.Player.Account.Name);
        TSPlayer.All.SendInfoMessage(this.Votes.ReminderBuild((int) this.Votes.RemainTime.TotalSeconds));
    }

    private void VoteAgree(CommandArgs args)
    {
        if (this.Votes == null)
        {
            args.Player.SendErrorMessage(GetString("[i:4344]当前没有正在进行的投票!"));
            return;
        }
        if (this.Votes.Argeement.Contains(args.Player.Account.Name) || this.Votes.Disargeement.Contains(args.Player.Account.Name))
        {
            args.Player.SendWarningMessage(GetString("[i:4344]你已经投过票了!"));
            return;
        }
        this.Votes.Argeement.Add(args.Player.Account.Name);
        //发送投票结果
        TSPlayer.All.SendInfoMessage(this.Votes.ReminderBuild((int) this.Votes.RemainTime.TotalSeconds));
    }

    private void VoteHandler(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendInfoMessage(GetString("[i:4344]Vote用法:\n") +
                    GetString("/vote kick <玩家> 投票踢出玩家\n") +
                    GetString("/vote ban <玩家> 投票封禁玩家\n") +
                    GetString("/vote mute <玩家> 投票禁言玩家\n") +
                    GetString("/vote clearboss <BOSS名称> 投票清理BOSS(不掉落)\n") +
                    GetString("/vote clearevent 投票清理地图中所有事件\n") +
                    GetString("/vote day 投票将时间改为白天\n") +
                    GetString("/vote night 将时间改为黑夜\n") +
                    GetString("/vote rain 投票下雨或停止下雨\n") +
                    GetString("/vote topic <主题> 自由主题投票\n") +
                    GetString("/vote clearkick <玩家> 将玩家从踢出状态移除\n") +
                    GetString("/vote kicklist 查看处于踢出状态的玩家"));
            return;
        }
        switch (args.Parameters[0].ToLower())
        {
            case "kick":
                if (!Config.config.EnableKick)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票踢出功能已被禁用!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote kick <玩家>"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                if (TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]) == null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标账号不存在!"));
                    return;
                }

                this.Votes = new Vote(args.Player.Account, Vote.VoteType.PlayerKick, TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]));
                break;
            case "ban":
                if (!Config.config.EnableBan)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票封禁功能已被禁用!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote ban <玩家>"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                if (TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]) == null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标账号不存在!"));
                    return;
                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.PlayerBan, TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]));
                break;
            case "mute":
                if (!Config.config.EnableMute)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票禁言功能已被禁用!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote mute <玩家>"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                if (TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]) == null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标账号不存在!"));
                    return;
                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.PlayerMute, TShock.UserAccounts.GetUserAccountByName(args.Parameters[1]));
                break;
            case "clearboss":
                if (!Config.config.EnableBossClear)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票清除Boss功能已被禁用!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote clearboss <BOSS名称>"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                var bosses = TShock.Utils.GetNPCByName(args.Parameters[1]);
                if (bosses.Count == 0)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标BOSS不存在!"));
                    return;
                }
                else if (bosses.Count > 1)
                {
                    args.Player.SendMultipleMatchError(bosses.Select(b => b.FullName));
                    return;
                }
                if (!bosses[0].boss)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标不是BOSS!"));
                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.BossClear, bosses[0].netID);
                break;
            case "clearevent":
                if (!Config.config.EnableEventClear)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票清除事件功能已被禁用!"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.EventClear);
                break;
            case "day":
                if (!Config.config.EnableTimeChange)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票修改时间功能已被禁用!"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.DayRequest);
                break;
            case "night":
                if (!Config.config.EnableTimeChange)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票修改时间功能已被禁用!"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.NightRequest);
                break;
            case "rain":
                if (!Config.config.EnableWeatherChange)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]投票修改天气功能已被禁用!"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.RainRequest);
                break;
            case "topic":
                if (!Config.config.EnableFreeVote)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]自由主题投票功能已被禁用!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote topic <投票主题>"));
                    return;
                }
                if (this.Votes != null)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]当前有正在进行的投票!"));
                    return;

                }
                this.Votes = new Vote(args.Player.Account, Vote.VoteType.FreeVote)
                {
                    Project = args.Parameters[1]
                };
                break;
            case "kicklist":
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(GetString("[i:4344]当前投票踢出列表:"));
                var kickList = VoteKicks.Where(v => v.Value > DateTime.Now);
                foreach (var kick in kickList)
                {
                    stringBuilder.AppendLine(GetString($"{kick.Key}:剩余{(int) (kick.Value - DateTime.Now).TotalSeconds}秒"));
                }
                args.Player.SendSuccessMessage(stringBuilder.ToString());
                break;
            case "clearkick":
                if (!args.Player.HasPermission("vote.admin"))
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]你没有权限这么做!"));
                    return;
                }
                if (args.Parameters.Count < 2)
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]用法: /vote clearkick <玩家>"));
                    return;
                }
                if (VoteKicks.ContainsKey(args.Parameters[1]))
                {
                    VoteKicks.Remove(args.Parameters[1]);
                    args.Player.SendSuccessMessage(GetString("[i:4344]成功清除投票踢出!"));
                }
                else
                {
                    args.Player.SendErrorMessage(GetString("[i:4344]目标玩家不存在投票踢出列表中!"));
                }
                break;

            default:
                args.Player.SendInfoMessage(GetString("[i:4344]Vote用法:\n") +
                    GetString("/vote kick <玩家> 投票踢出玩家\n") +
                    GetString("/vote ban <玩家> 投票封禁玩家\n") +
                    GetString("/vote mute <玩家> 投票禁言玩家\n") +
                    GetString("/vote clearboss <BOSS名称> 投票清理BOSS(不掉落)\n") +
                    GetString("/vote clearevent 投票清理地图中所有事件\n") +
                    GetString("/vote day 投票将时间改为白天\n") +
                    GetString("/vote night 将时间改为黑夜\n") +
                    GetString("/vote rain 投票下雨或停止下雨\n") +
                    GetString("/vote topic <主题> 自由主题投票\n") +
                    GetString("/vote clearkick <玩家> 将玩家从踢出状态移除\n") +
                    GetString("/vote kicklist 查看处于踢出状态的玩家"));
                break;
        }
    }

    private void OnUpdate(EventArgs args)
    {
        if (Timer == 60)
        {
            Timer = 0;
            if (this.Votes != null)
            {
                this.Votes.CheckReminder();
                if (this.Votes.IsEnd)
                {
                    if (this.Votes.Argeement.Count + this.Votes.Disargeement.Count < Config.config.MinVote)
                    {
                        TSPlayer.All.SendErrorMessage(GetString("[i:4344]投票未通过!\n") +
                            GetString($"*投票人数少于{Config.config.MinVote}人"));
                        this.Votes = null;
                        return;
                    }
                    if (this.Votes.AgreePercent < Config.config.VotePass)
                    {
                        TSPlayer.All.SendWarningMessage(GetString("[i:4344]投票未通过!\n") +
                            GetString($"*赞成:[c/32FF82:{this.Votes.Argeement.Count}票],反对:[c/E11919:{this.Votes.Disargeement.Count}票],通过率:{this.Votes.AgreePercent}%({Config.config.VotePass}%)"));

                        this.Votes = null;
                        return;
                    }
                    if (this.Votes.AgreePercent >= Config.config.VotePass)
                    {
                        this.Votes.VotePass();
                        TSPlayer.All.SendSuccessMessage(GetString($"*赞成:[c/32FF82:{this.Votes.Argeement.Count}票],反对:[c/E11919:{this.Votes.Disargeement.Count}票],通过率:{this.Votes.AgreePercent}%({Config.config.VotePass}%)"));
                        this.Votes = null;
                        return;

                    }
                }
            }
        }
        Timer++;
    }
}
