using Microsoft.Xna.Framework;
using MiniGamesAPI;
using Newtonsoft.Json;
using System.Text;
using System.Timers;
using Terraria;
using Terraria.Utilities;
using TShockAPI;
using static MiniGamesAPI.Enum;

namespace MainPlugin
{
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
        public List<MiniRegion> PlayerAreas { get; set; }

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
            Loaded = false;
            Initialize();

            Waiting_Timer.Elapsed += OnWaiting;
            Gaming_Timer.Elapsed += OnGaming;
            Scoring_Timer.Elapsed += OnScoring;
            PlayerAreas = new List<MiniRegion>();
            Players = new List<BuildPlayer>();
            Start();
        }

        private void OnGaming(object sender, ElapsedEventArgs e)
        {
            if (Players.Count == 0)
            {
                Conclude();
            }
            if ((int)this.Status != 2)
            {
                return;
            }
            ShowRoomMemberInfo();
            if (this.GamingTime == 0)
            {
                PlayerIndex = 0;
                Gaming_Timer.Stop();
                for (int num = Players.Count - 1; num >= 0; num--)
                {
                    BuildPlayer buildPlayer = Players[num];
                    ConfigUtils.evaluatePack.RestoreCharacter((MiniPlayer)(object)buildPlayer);
                    buildPlayer.Locked = true;
                    buildPlayer.UnCreative();
                    buildPlayer.Teleport(Players[PlayerIndex].CurrentRegion.Center);
                    buildPlayer.SendInfoMessage("已来到 " + Players[PlayerIndex].Name + " 的作品");
                }
                Scoring_Timer.Start();
                this.Status = (RoomStatus)1;
            }
            else
            {
                if (this.GamingTime <= 5)
                {
                    Broadcast($"还有 {this.GamingTime} 秒进入评分环节...", Color.MediumAquamarine);
                }
                this.GamingTime = this.GamingTime - 1;
            }
        }

        private void OnWaiting(object sender, ElapsedEventArgs e)
        {
            ShowRoomMemberInfo();
            if (Players.Count == 0 || Players.Where((BuildPlayer p) => p.IsReady).Count() < this.MinPlayer)
            {
                return;
            }
            if (this.WaitingTime == 0)
            {
                Waiting_Timer.Stop();
                SelectTopic();
                HandleRegions();
                for (int num = Players.Count - 1; num >= 0; num--)
                {
                    BuildPlayer buildPlayer = Players[num];
                    buildPlayer.Status = (PlayerStatus)2;
                    buildPlayer.IsReady = true;
                    buildPlayer.Godmode(true);
                    buildPlayer.Creative();
                    ConfigUtils.defaultPack.RestoreCharacter((MiniPlayer)(object)buildPlayer);
                }
                this.Status = (RoomStatus)2;
                Gaming_Timer.Start();
            }
            else
            {
                Broadcast($"游戏还有 {this.WaitingTime} 秒开始....", Color.DarkTurquoise);
                this.WaitingTime = this.WaitingTime - 1;
            }
        }

        public int GetPlayerCount()
        {
            return Players.Count;
        }

        public void Initialize()
        {
            Waiting_Timer.Elapsed += OnWaiting;
            Gaming_Timer.Elapsed += OnGaming;
            Scoring_Timer.Elapsed += OnScoring;
            PlayerAreas = new List<MiniRegion>();
            Players = new List<BuildPlayer>();
            Topics = new Dictionary<string, int>();
            Start();
        }

        private void OnScoring(object sender, ElapsedEventArgs e)
        {
            if (Players.Count == 0)
            {
                Conclude();
            }
            if ((int)this.Status != 1)
            {
                return;
            }
            ShowRoomMemberInfo();
            if (this.SeletingTime == 0)
            {
                Scoring_Timer.Stop();
                RoundClude();
                Scoring_Timer.Start();
                return;
            }
            if (this.SeletingTime <= 10)
            {
                if (PlayerIndex + 1 < Players.Count)
                {
                    Broadcast($"还有 {this.SeletingTime} 秒进入下一个玩家的建筑区域评分,下一个玩家为{Players[PlayerIndex + 1].Name}", Color.MediumAquamarine);
                }
                else
                {
                    Broadcast($"还有 {this.SeletingTime} 秒结束游戏", Color.MediumAquamarine);
                }
            }
            this.SeletingTime = this.SeletingTime - 1;
        }

        public void Dispose()
        {
            Stop();
            for (int num = Players.Count - 1; num >= 0; num--)
            {
                BuildPlayer buildPlayer = Players[num];
                buildPlayer.Teleport(Main.spawnTileX, Main.spawnTileY);
                buildPlayer.SendInfoMessage("游戏被强制停止！");
                buildPlayer.BackUp.RestoreCharacter((MiniPlayer)(object)buildPlayer);
                buildPlayer.BackUp = null;
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
            Restore();
            Players.Clear();
            this.Status = (RoomStatus)5;
        }

        public void Conclude()
        {
            this.Status = (RoomStatus)3;
            ShowVictory();
            for (int num = Players.Count - 1; num >= 0; num--)
            {
                BuildPlayer buildPlayer = Players[num];
                buildPlayer.Teleport(WaitingPoint);
                buildPlayer.SendInfoMessage("游戏结束！");
                buildPlayer.BackUp.RestoreCharacter((MiniPlayer)(object)buildPlayer);
                buildPlayer.CurrentRegion = null;
                buildPlayer.AquiredMarks = 0;
                buildPlayer.IsReady = false;
                buildPlayer.Locked = false;
                buildPlayer.Marked = false;
                buildPlayer.SelectedTopic = "";
                buildPlayer.GiveMarks = 0;
                buildPlayer.Godmode(false);
            }
            Restore();
        }

        public void RoundClude()
        {
            int num = 0;
            BuildRoom roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(this.ID);
            this.SeletingTime = roomByIDFromLocal.SeletingTime;
            PlayerIndex++;
            if (PlayerIndex < Players.Count)
            {
                for (int num2 = Players.Count - 1; num2 >= 0; num2--)
                {
                    BuildPlayer buildPlayer = Players[num2];
                    buildPlayer.Marked = true;
                    num += buildPlayer.GiveMarks;
                    buildPlayer.GiveMarks = 0;
                    buildPlayer.Teleport(Players[PlayerIndex].CurrentRegion.Center);
                    buildPlayer.SendInfoMessage("已来到 " + Players[PlayerIndex].Name + " 的建筑区域");
                    buildPlayer.Marked = false;
                }
            }
            Players[PlayerIndex - 1].AquiredMarks = num;
            if (PlayerIndex >= Players.Count)
            {
                Conclude();
            }
        }

        public void Start()
        {
            Waiting_Timer.Start();
            this.Status = 0;
        }

        public void Stop()
        {
            Waiting_Timer.Stop();
            Gaming_Timer.Stop();
            Scoring_Timer.Stop();
        }

        public void Restore()
        {
            this.Status = (RoomStatus)4;
            BuildRoom roomByIDFromLocal = ConfigUtils.GetRoomByIDFromLocal(this.ID);
            this.WaitingTime = roomByIDFromLocal.WaitingTime;
            this.GamingTime = roomByIDFromLocal.GamingTime;
            this.SeletingTime = roomByIDFromLocal.SeletingTime;
            Topics = roomByIDFromLocal.Topics;
            this.MaxPlayer = roomByIDFromLocal.MaxPlayer;
            this.MinPlayer = roomByIDFromLocal.MinPlayer;
            PerHeight = roomByIDFromLocal.PerHeight;
            PerWidth = roomByIDFromLocal.PerWidth;
            Gap = roomByIDFromLocal.Gap;
            PlayerAreas.Clear();
            GameBoard.ReBuild(true);
            this.Status = 0;
            Start();
        }

        public void ShowRoomMemberInfo()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(MiniGamesAPI.Utils.EndLine_10);
            stringBuilder.AppendLine("————房内信息————");
            RoomStatus status = this.Status;
            switch ((int)status)
            {
                case 0:
                    {
                        int num = this.WaitingTime / 60;
                        int num2 = this.WaitingTime % 60;
                        stringBuilder.AppendLine($"倒计时[{num}:{num2}]");
                        stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(Topic) ? "无" : Topic));
                        for (int num3 = Players.Count - 1; num3 >= 0; num3--)
                        {
                            BuildPlayer buildPlayer = Players[num3];
                            stringBuilder.AppendLine("[" + buildPlayer.Name + "][" + (buildPlayer.IsReady ? "已准备" : "未准备") + "]");
                        }
                        stringBuilder.AppendLine($"当前人数:{GetPlayerCount()}/{this.MaxPlayer}");
                        stringBuilder.AppendLine("输入/bm ready 准备/未准备");
                        stringBuilder.AppendLine("输入/bm leave 离开房间");
                        stringBuilder.AppendLine("输入/bm tl 查看主题列表");
                        stringBuilder.AppendLine("输入/bm vote [主题] 进行主题投票");
                        break;
                    }
                case 2:
                    {
                        int num = this.GamingTime / 60;
                        int num2 = this.GamingTime % 60;
                        stringBuilder.AppendLine($"倒计时[{num}:{num2}]");
                        stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(Topic) ? "无" : Topic));
                        stringBuilder.AppendLine($"当前人数:{GetPlayerCount()}/{this.MaxPlayer}");
                        break;
                    }
                case 1:
                    {
                        int num = this.SeletingTime / 60;
                        int num2 = this.SeletingTime % 60;
                        stringBuilder.AppendLine($"倒计时[{num}:{num2}]");
                        stringBuilder.AppendLine("主题:" + (string.IsNullOrEmpty(Topic) ? "无" : Topic));
                        stringBuilder.AppendLine($"当前人数:{GetPlayerCount()}/{this.MaxPlayer}");
                        break;
                    }
            }
            stringBuilder.AppendLine(MiniGamesAPI.Utils.EndLine_15);
            for (int num4 = Players.Count - 1; num4 >= 0; num4--)
            {
                Players[num4].SendBoardMsg(stringBuilder.ToString());
            }
        }

        public void ShowVictory()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Players.Sort();
            int num = 1;
            foreach (BuildPlayer player in Players)
            {
                stringBuilder.AppendLine($"[{num}] {player.Name} 得分:{player.AquiredMarks}");
                num++;
            }
            Broadcast(stringBuilder.ToString(), Color.MediumAquamarine);
        }

        public void Broadcast(string msg, Color color)
        {
            for (int num = Players.Count - 1; num >= 0; num--)
            {
                Players[num].SendMessage(msg, color);
            }
        }

        public bool HandleRegions()
        {
            PlayerAreas = GamingArea.Divide(PerWidth, PerHeight, GetPlayerCount(), Gap);
            if (PlayerAreas == null)
            {
                Dispose();
                return false;
            }
            foreach (MiniRegion playerArea in PlayerAreas)
            {
                playerArea.BuildFramework(118, false);
            }
            TSPlayer.All.SendTileRect((short)GamingArea.Area.X, (short)GamingArea.Area.Y, (byte)(GamingArea.Area.Width + 3), (byte)(GamingArea.Area.Height + 3), 0);
            for (int i = 0; i < PlayerAreas.Count; i++)
            {
                MiniRegion val = PlayerAreas[i];
                BuildPlayer buildPlayer = Players[i];
                val.Owners.Add(Players[i].Name);
                buildPlayer.CurrentRegion = val;
                buildPlayer.Teleport(val.Center);
                buildPlayer.SendInfoMessage("已将你传送到建筑区域");
            }
            return true;
        }

        public void LoadRegion()
        {
            GamingArea = new MiniRegion(this.Name, this.ID, TL.X, TL.Y, BR.X, BR.Y);
            GameBoard = new PrebuildBoard(GamingArea);
            Loaded = true;
        }

        public void SelectTopic()
        {
            int max = 0;
            foreach (string key in Topics.Keys)
            {
                if (Topics[key] >= max)
                {
                    max = Topics[key];
                }
            }
            List<string> list = (from p in Topics
                                 where p.Value == max
                                 select p.Key).ToList();
            if (list.Count() == 1)
            {
                Topic = list[0];
            }
            else
            {
                UnifiedRandom val = new UnifiedRandom();
                Topic = list[val.Next(0, list.Count - 1)];
            }
            Broadcast("本次建筑的主题是 " + Topic, Color.MediumAquamarine);
        }
    }
}
