using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace VotePlus
{

    public class Vote
    {
        public Vote(UserAccount sender,VoteType voteType,UserAccount target)
        {
            Sender = sender;
            Type = voteType;
            Target = target;
            TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
        }
        public Vote(UserAccount sender, VoteType voteType)
        {
            Sender = sender;
            Type = voteType;
            TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
        }
        public Vote(UserAccount sender, VoteType voteType, int bossid)
        {
            Sender = sender;
            Type = voteType;
            BossID = bossid;
            TSPlayer.All.SendInfoMessage(this.VoteStartBuild());
        }
        public Vote(UserAccount sender, VoteType voteType, string prj)
        {
            Sender = sender;
            Type = voteType;
            Project = prj;
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

        public UserAccount Target { get; set; }

        public int BossID { get; set; }

        public string Project { get; set; }

        public DateTime StartTime { get; set; } = DateTime.Now;
        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(60);
        public DateTime EndTime
        {
            get
            {
                return StartTime + Duration;
            }
        }
        public TimeSpan RemainTime
        {
            get
            {
                return EndTime - DateTime.Now;
            }
        }
        public bool IsEnd
        {
            get
            {
                return DateTime.Now > EndTime;
            }
        }
        public bool rain = Main.raining;
        public List<string> Argeement { get; set; } = new();
        public List<string> Disargeement { get; set; } = new();
        public int Total => TShock.Players.Where(p => p != null && p.Active).Count();
        public int AgreePercent => (int) (Argeement.Count * 100 / Total) ;

        public short VoteReminder = 3;
        public string ReminderBuild(int seconds)
        {
            switch (Type)
            {
                case VoteType.PlayerKick:
                    return GetString($"[i:4344]踢出玩家\"{Target.Name}\"(持续{config.KickDuration})投票还有{seconds}秒结束!\n")+ VoteResultBuild();
                case VoteType.PlayerBan:
                    return GetString($"[i:4344]封禁玩家\"{Target.Name}\"投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.PlayerMute:
                    return GetString($"[i:4344]禁言玩家\"{Target.Name}\"投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.BossClear:
                    return GetString($"[i:4344]清除BOSS\"{Lang.GetNPCNameValue(BossID)}\"投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.EventClear:
                    return GetString($"[i:4344]关闭目前入侵事件投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.NightRequest:
                    return GetString($"[i:4344]请求修改为夜晚投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.DayRequest:
                    return GetString($"[i:4344]请求修改为白天投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.FreeVote:
                    return GetString($"[i:4344]赞成关于\"{Project}\"投票还有{seconds}秒结束!\n") + VoteResultBuild();
                case VoteType.RainRequest:
                    return GetString($"[i:4344]请求{(rain ? "停止" : "开始")}下雨投票还有{seconds}秒结束!\n") + VoteResultBuild();
            }
            return "";
        }

        public string VoteResultBuild()
        {
            return GetString($"*赞成:[c/32FF82:{Argeement.Count()}票],反对:[c/E11919:{Disargeement.Count()}票],通过率:{AgreePercent}%({config.VotePass}%)\n") +
                   GetString($"*使用\"[c/32FF82:/agree(/同意)]\"或\"[c/E11919:/disagree(/反对)]\"进行投票");
        }

        public void CheckReminder()
        {
            var remain = RemainTime;
            switch (VoteReminder)
            {
                case 3:
                    if (remain.TotalSeconds <= 45)
                    {
                        TSPlayer.All.SendInfoMessage(ReminderBuild(45));
                        VoteReminder--;

                    }
                    break;
                case 2:
                    if (remain.TotalSeconds <= 30)
                    {
                        TSPlayer.All.SendInfoMessage(ReminderBuild(30));
                        VoteReminder--;
                    }
                    break;
                case 1:
                    if (remain.TotalSeconds <= 15)
                    {
                        TSPlayer.All.SendInfoMessage(ReminderBuild(15));
                        VoteReminder--;
                    }
                    break;            
            }
        }
        public string VoteStartBuild()
        {

            switch (Type)
            {
                case VoteType.PlayerKick:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了踢出玩家\"{Target.Name}\"(持续{config.KickDuration})投票!\n") + VoteResultBuild();
                case VoteType.PlayerBan:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了封禁玩家\"{Target.Name}\"投票!\n") + VoteResultBuild();
                case VoteType.PlayerMute:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了禁言玩家\"{Target.Name}\"投票!\n") + VoteResultBuild();
                case VoteType.BossClear:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了清除BOSS\"{Lang.GetNPCNameValue(BossID)}\"投票!\n") + VoteResultBuild();
                case VoteType.EventClear:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了关闭目前事件投票!\n") + VoteResultBuild();
                case VoteType.NightRequest:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了请求修改为夜晚投票!\n") + VoteResultBuild();
                case VoteType.DayRequest:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了请求修改为白天投票!\n") + VoteResultBuild();
                case VoteType.FreeVote:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了赞成关于\"{Project}\"投票!\n") + VoteResultBuild();
                case VoteType.RainRequest:
                    return GetString($"[i:4344]\"{Sender.Name}\"发起了请求{(rain ? "停止" : "开始")}下雨投票!\n") + VoteResultBuild();
            }
            return "";
        }
        public void VotePass()
        {
            switch (Type)
            {
                case VoteType.PlayerKick:
                    if (VotePlus.VoteKicks.ContainsKey(Target.Name))
                    {
                        VotePlus.VoteKicks[Target.Name] = DateTime.Now + TimeSpan.FromSeconds(config.KickDuration);
                    }
                    else
                    {
                        VotePlus.VoteKicks.Add(Target.Name, DateTime.Now + TimeSpan.FromSeconds(config.KickDuration));
                    }
                    TShock.Players.Where(p => p != null && p.Active && p.Account.Name == Target.Name).ToList().ForEach(p =>
                    {
                        p.Kick(GetString($"你被投票踢出游戏,持续{config.KickDuration}秒!"),true,true,Sender.Name,true);
                    });
                    TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{Target.Name}\"被踢出游戏,持续{config.KickDuration}秒!"));
                    
                    break;
                case VoteType.PlayerBan:
                    TShock.Bans.InsertBan($"acc:{Target.Name}", GetString("你已被投票永久封禁"), Sender.Name, DateTime.UtcNow, DateTime.MaxValue);
                    TShock.Players.Where(p => p != null && p.Active && p.Account.Name == Target.Name).ToList().ForEach(p =>
                    {
                        p.Kick(GetString($"你被投票永久封禁!"), true, true, Sender.Name, true);
                    });
                    TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{Target.Name}\"已被投票永久封禁!"));
                    break;
                case VoteType.PlayerMute:
                    TShock.Players.Where(p => p != null && p.Active && p.Account.Name == Target.Name).ToList().ForEach(p =>
                    {
                       p.mute = true;
                    });

                    TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]玩家\"{Target.Name}\"已被投票禁言!"));
                    break;
                case VoteType.BossClear:
                    Main.npc.Where(n => n != null && n.active && n.type == BossID).ToList().ForEach(n =>
                    {
                        n.active = false;
                        n.life = 0;
                        NetMessage.SendData(23, -1, -1, null, n.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    });
                    TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]BOSS\"{Lang.GetNPCNameValue(BossID)}\"已被投票清除!"));
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
                     TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]关于\"{Project}\"的投票已被通过!"));
                    break;
                case VoteType.RainRequest:
                    if (rain)
                    {
                        Main.StopRain();
                    }
                    else
                    {
                        Main.StartRain();
                    }
                    TSPlayer.All.SendSuccessMessage(GetString($"[i:4344]关于{(rain ? "停止" : "开始")}下雨的投票已被通过!"));
                    break;
            }
        }
    }

   
}
