using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace VotePlus;


public class Vote
{
    public Vote(UserAccount sender, VoteType voteType, UserAccount target)
    {
        this.Sender = sender;
        this.Type = voteType;
        this.Target = target;
        TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
    }
    public Vote(UserAccount sender, VoteType voteType)
    {
        this.Sender = sender;
        this.Type = voteType;
        TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
    }
    public Vote(UserAccount sender, VoteType voteType, int bossid)
    {
        this.Sender = sender;
        this.Type = voteType;
        this.BossID = bossid;
        TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
    }
    public Vote(UserAccount sender, VoteType voteType, string prj)
    {
        this.Sender = sender;
        this.Type = voteType;
        this.Project = prj;
        TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
    }
    public static Config config => Config.config;
    public enum VoteType
    {
        PlayerKick, //踢出
        PlayerBan, //封禁 (感觉用不上)
        PlayerMute, //禁言
        BossClear, //清除BOSS(不掉落)
        EventClear, //关闭事件(不计为打败)
        NightRequest,//请求修改为夜晚 
        DayRequest, //请求修改为白天
        RainRequest, //请求修改为下雨
        FreeVote //自由投票
    }
    public VoteType Type { get; set; }
    public UserAccount Sender { get; set; }

    public UserAccount Target { get; set; } = null!;

    public int BossID { get; set; }

    public string Project { get; set; } = "";

    public DateTime StartTime { get; set; } = DateTime.Now;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(60);
    public DateTime EndTime => this.StartTime + this.Duration;
    public TimeSpan RemainTime => this.EndTime - DateTime.Now;
    public bool IsEnd => DateTime.Now > this.EndTime;
    public bool rain = Main.raining;
    public List<string> Argeement { get; set; } = new();
    public List<string> Disargeement { get; set; } = new();
    public int Total => TShock.Players.Where(p => p != null && p.Active).Count();
    public int AgreePercent => this.Argeement.Count * 100 / this.Total;

    public short VoteReminder = 3;
    public string ReminderBuild(int seconds)
    {
        return this.Type switch
        {
            VoteType.PlayerKick => GetString($"[i:4344]踢出玩家\"{this.Target.Name}\"(持续{config.KickDuration})投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.PlayerBan => GetString($"[i:4344]封禁玩家\"{this.Target.Name}\"投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.PlayerMute => GetString($"[i:4344]禁言玩家\"{this.Target.Name}\"投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.BossClear => GetString($"[i:4344]清除BOSS\"{Lang.GetNPCNameValue(this.BossID)}\"投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.EventClear => GetString($"[i:4344]关闭目前入侵事件投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.NightRequest => GetString($"[i:4344]请求修改为夜晚投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.DayRequest => GetString($"[i:4344]请求修改为白天投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.FreeVote => GetString($"[i:4344]赞成关于\"{this.Project}\"投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            VoteType.RainRequest => GetString($"[i:4344]请求{(this.rain ? GetString("停止") : GetString("开始"))}下雨投票还有{seconds}秒结束!\n") + this.VoteResultBuild(),
            _ => "",
        };
    }

    public string VoteResultBuild()
    {
        return GetString($"*赞成:[c/32FF82:{this.Argeement.Count}票],反对:[c/E11919:{this.Disargeement.Count}票],通过率:{this.AgreePercent}%({config.VotePass}%)\n") +
               GetString($"*使用\"[c/32FF82:/agree(/同意)]\"或\"[c/E11919:/disagree(/反对)]\"进行投票");
    }

    public void CheckReminder()
    {
        var remain = this.RemainTime;
        switch (this.VoteReminder)
        {
            case 3:
                if (remain.TotalSeconds <= 45)
                {
                    TSPlayer.All.SendInfoMessage(this.ReminderBuild(45));
                    this.VoteReminder--;

                }
                break;
            case 2:
                if (remain.TotalSeconds <= 30)
                {
                    TSPlayer.All.SendInfoMessage(this.ReminderBuild(30));
                    this.VoteReminder--;
                }
                break;
            case 1:
                if (remain.TotalSeconds <= 15)
                {
                    TSPlayer.All.SendInfoMessage(this.ReminderBuild(15));
                    this.VoteReminder--;
                }
                break;
        }
    }
    public string VoteStartBuild()
    {

        return this.Type switch
        {
            VoteType.PlayerKick => GetString($"[i:4344]\"{this.Sender.Name}\"发起了踢出玩家\"{this.Target.Name}\"(持续{config.KickDuration})投票!\n") + this.VoteResultBuild(),
            VoteType.PlayerBan => GetString($"[i:4344]\"{this.Sender.Name}\"发起了封禁玩家\"{this.Target.Name}\"投票!\n") + this.VoteResultBuild(),
            VoteType.PlayerMute => GetString($"[i:4344]\"{this.Sender.Name}\"发起了禁言玩家\"{this.Target.Name}\"投票!\n") + this.VoteResultBuild(),
            VoteType.BossClear => GetString($"[i:4344]\"{this.Sender.Name}\"发起了清除BOSS\"{Lang.GetNPCNameValue(this.BossID)}\"投票!\n") + this.VoteResultBuild(),
            VoteType.EventClear => GetString($"[i:4344]\"{this.Sender.Name}\"发起了关闭目前事件投票!\n") + this.VoteResultBuild(),
            VoteType.NightRequest => GetString($"[i:4344]\"{this.Sender.Name}\"发起了请求修改为夜晚投票!\n") + this.VoteResultBuild(),
            VoteType.DayRequest => GetString($"[i:4344]\"{this.Sender.Name}\"发起了请求修改为白天投票!\n") + this.VoteResultBuild(),
            VoteType.FreeVote => GetString($"[i:4344]\"{this.Sender.Name}\"发起了赞成关于\"{this.Project}\"投票!\n") + this.VoteResultBuild(),
            VoteType.RainRequest => GetString($"[i:4344]\"{this.Sender.Name}\"发起了请求{(this.rain ? "停止" : "开始")}下雨投票!\n") + this.VoteResultBuild(),
            _ => "",
        };
    }
    public void VotePass()
    {
        switch (this.Type)
        {
            case VoteType.PlayerKick:
                if (VotePlus.VoteKicks.ContainsKey(this.Target.Name))
                {
                    VotePlus.VoteKicks[this.Target.Name] = DateTime.Now + TimeSpan.FromSeconds(config.KickDuration);
                }
                else
                {
                    VotePlus.VoteKicks.Add(this.Target.Name, DateTime.Now + TimeSpan.FromSeconds(config.KickDuration));
                }
                TShock.Players.Where(p => p != null && p.Active && p.Account.Name == this.Target.Name).ToList().ForEach(p => p.Kick(GetString($"你被投票踢出游戏,持续{config.KickDuration}秒!"), true, true, this.Sender.Name, true));
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{this.Target.Name}\"被踢出游戏,持续{config.KickDuration}秒!"));

                break;
            case VoteType.PlayerBan:
                TShock.Bans.InsertBan($"acc:{this.Target.Name}", GetString("你已被投票永久封禁"), this.Sender.Name, DateTime.UtcNow, DateTime.MaxValue);
                TShock.Players.Where(p => p != null && p.Active && p.Account.Name == this.Target.Name).ToList().ForEach(p => p.Kick(GetString($"你被投票永久封禁!"), true, true, this.Sender.Name, true));
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{this.Target.Name}\"已被投票永久封禁!"));
                break;
            case VoteType.PlayerMute:
                TShock.Players.Where(p => p != null && p.Active && p.Account.Name == this.Target.Name).ToList().ForEach(p => p.mute = true);

                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{this.Target.Name}\"已被投票禁言!"));
                break;
            case VoteType.BossClear:
                Main.npc.Where(n => n != null && n.active && n.type == this.BossID).ToList().ForEach(n =>
                {
                    n.active = false;
                    n.life = 0;
                    NetMessage.SendData(23, -1, -1, null, n.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                });
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]BOSS\"{Lang.GetNPCNameValue(this.BossID)}\"已被投票清除!"));
                break;
            case VoteType.EventClear:
                Main.invasionDelay = 0;
                Main.invasionSize = 0;
                Main.invasionType = 0;
                Main.pumpkinMoon = false;
                Main.snowMoon = false;
                Main.pumpkinMoon = false;
                Main.eclipse = false;
                Main.bloodMoon = false;
                Main.StopSlimeRain();
                TSPlayer.All.SendData(PacketTypes.WorldInfo, "");
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]目前事件已被投票关闭!"));
                break;
            case VoteType.NightRequest:
                TSPlayer.Server.SetTime(false, 0.0);
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]时间已被投票修改为夜晚!"));
                break;
            case VoteType.DayRequest:
                TSPlayer.Server.SetTime(true, 0.0);
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]时间已被投票修改为白天!"));
                break;
            case VoteType.FreeVote:
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]关于\"{this.Project}\"的投票已被通过!"));
                break;
            case VoteType.RainRequest:
                if (this.rain)
                {
                    Main.StopRain();
                }
                else
                {
                    Main.StartRain();
                }
                TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]关于{(this.rain ? GetString("停止") : GetString("开始"))}下雨的投票已被通过!"));
                break;
        }
    }
}


