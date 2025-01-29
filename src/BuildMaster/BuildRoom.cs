using Microsoft.Xna.Framework;
using MiniGamesAPI;
using Newtonsoft.Json;
using System.Text;
using System.Timers;
using Terraria;
using Terraria.Utilities;
using TShockAPI;
using static MiniGamesAPI.Enum;

namespace MainPlugin;

public class BuildRoom : MiniRoom, IRoom
{
    [JsonIgnore]
    public System.Timers.Timer Waiting_Timer = new System.Timers.Timer(1000.0);

    [JsonIgnore]
    public System.Timers.Timer Gaming_Timer = new System.Timers.Timer(1000.0);

    [JsonIgnore]
    public System.Timers.Timer Scoring_Timer = new System.Timers.Timer(1000.0);

    [JsonIgnore]
    public MiniRegion GamingArea { get; set; }

    [JsonIgnore]
    public PrebuildBoard GameBoard { get; set; }

    [JsonIgnore]
    public List<MiniRegion>? PlayerAreas { get; set; }

    [JsonIgnore]
    public List<BuildPlayer> Players { get; set; }

    public Dictionary<string, int> Topics { get; set; }

    public Point WaitingPoint { get; set; }

    public Point TL { get; set; }

    public Point BR { get; set; }

    [JsonIgnore]
    public bool Loaded { get; set; }

    public int PerWidth { get; set; }

    public int PerHeight { get; set; }

    public int Gap { get; set; }

    [JsonIgnore]
    public string Topic { get; set; }

    [JsonIgnore]
    public int PlayerIndex { get; set; }

    public BuildRoom(string name, int id) : base(id, name)
    {
        this.GamingArea = null!;
        this.GameBoard = null!;
        this.Topic = null!;
        this.Topics = null!;

        this.Loaded = false;
        this.Initialize();

        this.Waiting_Timer.Elapsed += this.OnWaiting;
        this.Gaming_Timer.Elapsed += this.OnGaming;
        this.Scoring_Timer.Elapsed += this.OnScoring;
        this.PlayerAreas = new List<MiniRegion>();
        this.Players = new List<BuildPlayer>();
        this.Start();
    }

    private void OnGaming(object? sender, ElapsedEventArgs e)
    {
        if (this.Players.Count == 0)
        {
            this.Conclude();
        }
        if ((int) this.Status != 2)
        {
            return;
        }
        this.ShowRoomMemberInfo();
        if (this.GamingTime == 0)
        {
            this.PlayerIndex = 0;
            this.Gaming_Timer.Stop();
            for (var num = this.Players.Count - 1; num >= 0; num--)
            {
                var buildPlayer = this.Players[num];
                ConfigUtils.evaluatePack.RestoreCharacter((MiniPlayer) (object) buildPlayer);
                buildPlayer.Locked = true;
                buildPlayer.UnCreative();
                buildPlayer.Teleport(this.Players[this.PlayerIndex].CurrentRegion!.Center);
                buildPlayer.SendInfoMessage(GetString("已来到 ") + this.Players[this.PlayerIndex].Name + GetString(" 的作品"));
            }
            this.Scoring_Timer.Start();
            this.Status = (RoomStatus) 1;
        }
        else
        {
            if (this.GamingTime <= 5)
            {
                this.Broadcast(GetString($"还有 {this.GamingTime} 秒进入评分环节..."), Color.MediumAquamarine);
            }
            this.GamingTime--;
        }
    }

    private void OnWaiting(object? sender, ElapsedEventArgs e)
    {
        this.ShowRoomMemberInfo();
        if (this.Players.Count == 0 || this.Players.Where((BuildPlayer p) => p.IsReady).Count() < this.MinPlayer)
        {
            return;
        }
        if (this.WaitingTime == 0)
        {
            this.Waiting_Timer.Stop();
            this.SelectTopic();
            this.HandleRegions();
            for (var num = this.Players.Count - 1; num >= 0; num--)
            {
                var buildPlayer = this.Players[num];
                buildPlayer.Status = (PlayerStatus) 2;
                buildPlayer.IsReady = true;
                buildPlayer.Godmode(true);
                buildPlayer.Creative();
                ConfigUtils.defaultPack.RestoreCharacter((MiniPlayer) (object) buildPlayer);
            }
            this.Status = (RoomStatus) 2;
            this.Gaming_Timer.Start();
        }
        else
        {
            this.Broadcast(GetString($"游戏还有 {this.WaitingTime} 秒开始...."), Color.DarkTurquoise);
            this.WaitingTime--;
        }
    }

    public int GetPlayerCount()
    {
        return this.Players.Count;
    }

    public void Initialize()
    {
        this.Waiting_Timer.Elapsed += this.OnWaiting;
        this.Gaming_Timer.Elapsed += this.OnGaming;
        this.Scoring_Timer.Elapsed += this.OnScoring;
        this.PlayerAreas = new List<MiniRegion>();
        this.Players = new List<BuildPlayer>();
        this.Topics = new Dictionary<string, int>();
        this.Start();
    }

    private void OnScoring(object? sender, ElapsedEventArgs e)
    {
        if (this.Players.Count == 0)
        {
            this.Conclude();
        }
        if ((int) this.Status != 1)
        {
            return;
        }
        this.ShowRoomMemberInfo();
        if (this.SeletingTime == 0)
        {
            this.Scoring_Timer.Stop();
            this.RoundClude();
            this.Scoring_Timer.Start();
            return;
        }
        if (this.SeletingTime <= 10)
        {
            if (this.PlayerIndex + 1 < this.Players.Count)
            {
                this.Broadcast(GetString($"还有 {this.SeletingTime} 秒进入下一个玩家的建筑区域评分,下一个玩家为{this.Players[this.PlayerIndex + 1].Name}"), Color.MediumAquamarine);
            }
            else
            {
                this.Broadcast(GetString($"还有 {this.SeletingTime} 秒结束游戏"), Color.MediumAquamarine);
            }
        }
        this.SeletingTime--;
    }

    public void Dispose()
    {
        this.Stop();
        for (var num = this.Players.Count - 1; num >= 0; num--)
        {
            var buildPlayer = this.Players[num];
            buildPlayer.Teleport(Main.spawnTileX, Main.spawnTileY);
            buildPlayer.SendInfoMessage(GetString("游戏被强制停止！"));
            buildPlayer.BackUp.RestoreCharacter((MiniPlayer) (object) buildPlayer);
            buildPlayer.BackUp = null!;
            buildPlayer.CurrentRoomID = 0;
            buildPlayer.CurrentRegion = null;
            buildPlayer.AquiredMarks = 0;
            buildPlayer.GiveMarks = 0;
            buildPlayer.SelectedTopic = "";
            buildPlayer.IsReady = false;
            buildPlayer.Locked = false;
            buildPlayer.Marked = false;
            buildPlayer.Godmode(false);
        }
        this.Restore();
        this.Players.Clear();
        this.Status = (RoomStatus) 5;
    }

    public void Conclude()
    {
        this.Status = (RoomStatus) 3;
        this.ShowVictory();
        for (var num = this.Players.Count - 1; num >= 0; num--)
        {
            var buildPlayer = this.Players[num];
            buildPlayer.Teleport(this.WaitingPoint);
            buildPlayer.SendInfoMessage(GetString("游戏结束！"));
            buildPlayer.BackUp.RestoreCharacter((MiniPlayer) (object) buildPlayer);
            buildPlayer.CurrentRegion = null;
            buildPlayer.AquiredMarks = 0;
            buildPlayer.IsReady = false;
            buildPlayer.Locked = false;
            buildPlayer.Marked = false;
            buildPlayer.SelectedTopic = "";
            buildPlayer.GiveMarks = 0;
            buildPlayer.Godmode(false);
        }
        this.Restore();
    }

    public void RoundClude()
    {
        var num = 0;
        var roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(this.ID)!;
        this.SeletingTime = roomByIDFromLocal.SeletingTime;
        this.PlayerIndex++;
        if (this.PlayerIndex < this.Players.Count)
        {
            for (var num2 = this.Players.Count - 1; num2 >= 0; num2--)
            {
                var buildPlayer = this.Players[num2];
                buildPlayer.Marked = true;
                num += buildPlayer.GiveMarks;
                buildPlayer.GiveMarks = 0;
                buildPlayer.Teleport(this.Players[this.PlayerIndex].CurrentRegion!.Center);
                buildPlayer.SendInfoMessage(GetString($"已来到 {this.Players[this.PlayerIndex].Name} 的建筑区域"));
                buildPlayer.Marked = false;
            }
        }
        this.Players[this.PlayerIndex - 1].AquiredMarks = num;
        if (this.PlayerIndex >= this.Players.Count)
        {
            this.Conclude();
        }
    }

    public void Start()
    {
        this.Waiting_Timer.Start();
        this.Status = 0;
    }

    public void Stop()
    {
        this.Waiting_Timer.Stop();
        this.Gaming_Timer.Stop();
        this.Scoring_Timer.Stop();
    }

    public void Restore()
    {
        this.Status = (RoomStatus) 4;
        var roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(this.ID)!;
        this.WaitingTime = roomByIDFromLocal.WaitingTime;
        this.GamingTime = roomByIDFromLocal.GamingTime;
        this.SeletingTime = roomByIDFromLocal.SeletingTime;
        this.Topics = roomByIDFromLocal.Topics;
        this.MaxPlayer = roomByIDFromLocal.MaxPlayer;
        this.MinPlayer = roomByIDFromLocal.MinPlayer;
        this.PerHeight = roomByIDFromLocal.PerHeight;
        this.PerWidth = roomByIDFromLocal.PerWidth;
        this.Gap = roomByIDFromLocal.Gap;
        this.PlayerAreas!.Clear();
        this.GameBoard.ReBuild(true);
        this.Status = 0;
        this.Start();
    }

    public void ShowRoomMemberInfo()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(MiniGamesAPI.Utils.EndLine_10);
        stringBuilder.AppendLine(GetString("————房内信息————"));
        var status = this.Status;
        switch ((int) status)
        {
            case 0:
            {
                var num = this.WaitingTime / 60;
                var num2 = this.WaitingTime % 60;
                stringBuilder.AppendLine(GetString($"倒计时[{num}:{num2}]"));
                stringBuilder.AppendLine(GetString("主题:") + (string.IsNullOrEmpty(this.Topic) ? GetString("无") : this.Topic));
                for (var num3 = this.Players.Count - 1; num3 >= 0; num3--)
                {
                    var buildPlayer = this.Players[num3];
                    stringBuilder.AppendLine("[" + buildPlayer.Name + "][" + (buildPlayer.IsReady ? GetString("已准备") : GetString("未准备")) + "]");
                }
                stringBuilder.AppendLine(GetString($"当前人数:{this.GetPlayerCount()}/{this.MaxPlayer}"));
                stringBuilder.AppendLine(GetString("输入/bm ready 准备/未准备"));
                stringBuilder.AppendLine(GetString("输入/bm leave 离开房间"));
                stringBuilder.AppendLine(GetString("输入/bm tl 查看主题列表"));
                stringBuilder.AppendLine(GetString("输入/bm vote [主题] 进行主题投票"));
                break;
            }
            case 1:
            case 2:
            {
                var num = this.SeletingTime / 60;
                var num2 = this.SeletingTime % 60;
                var topic = string.IsNullOrEmpty(this.Topic)
                    ? GetString("无")
                    : this.Topic;
                stringBuilder.AppendLine(GetString($"倒计时[{num}:{num2}]"));
                stringBuilder.AppendLine(GetString($"主题:{topic}"));
                stringBuilder.AppendLine(GetString($"当前人数:{this.GetPlayerCount()}/{this.MaxPlayer}"));
                break;
            }
        }
        stringBuilder.AppendLine(MiniGamesAPI.Utils.EndLine_15);
        for (var num4 = this.Players.Count - 1; num4 >= 0; num4--)
        {
            this.Players[num4].SendBoardMsg(stringBuilder.ToString());
        }
    }

    public void ShowVictory()
    {
        var stringBuilder = new StringBuilder();
        this.Players.Sort();
        var num = 1;
        foreach (var player in this.Players)
        {
            stringBuilder.AppendLine(GetString($"[{num}] {player.Name} 得分:{player.AquiredMarks}"));
            num++;
        }
        this.Broadcast(stringBuilder.ToString(), Color.MediumAquamarine);
    }

    public void Broadcast(string msg, Color color)
    {
        for (var num = this.Players.Count - 1; num >= 0; num--)
        {
            this.Players[num].SendMessage(msg, color);
        }
    }

    public bool HandleRegions()
    {
        this.PlayerAreas = this.GamingArea.Divide(this.PerWidth, this.PerHeight, this.GetPlayerCount(), this.Gap);
        if (this.PlayerAreas == null)
        {
            this.Dispose();
            return false;
        }
        foreach (var playerArea in this.PlayerAreas)
        {
            playerArea.BuildFramework(118, false);
        }
        TSPlayer.All.SendTileRect((short) this.GamingArea.Area.X, (short) this.GamingArea.Area.Y, (byte) (this.GamingArea.Area.Width + 3), (byte) (this.GamingArea.Area.Height + 3), 0);
        for (var i = 0; i < this.PlayerAreas.Count; i++)
        {
            var val = this.PlayerAreas[i];
            var buildPlayer = this.Players[i];
            val.Owners.Add(this.Players[i].Name);
            buildPlayer.CurrentRegion = val;
            buildPlayer.Teleport(val.Center);
            buildPlayer.SendInfoMessage(GetString("已将你传送到建筑区域"));
        }
        return true;
    }

    public void LoadRegion()
    {
        this.GamingArea = new MiniRegion(this.Name, this.ID, this.TL.X, this.TL.Y, this.BR.X, this.BR.Y);
        this.GameBoard = new PrebuildBoard(this.GamingArea);
        this.Loaded = true;
    }

    public void SelectTopic()
    {
        var max = 0;
        foreach (var key in this.Topics.Keys)
        {
            if (this.Topics[key] >= max)
            {
                max = this.Topics[key];
            }
        }
        var list = (from p in this.Topics
                    where p.Value == max
                    select p.Key).ToList();
        if (list.Count == 1)
        {
            this.Topic = list[0];
        }
        else
        {
            var val = new UnifiedRandom();
            this.Topic = list[val.Next(0, list.Count - 1)];
        }
        this.Broadcast(GetString($"本次建筑的主题是 {this.Topic}"), Color.MediumAquamarine);
    }
}