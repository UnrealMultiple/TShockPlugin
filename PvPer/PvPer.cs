using Microsoft.Data.Sqlite;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework;

namespace PvPer
{
    [ApiVersion(2, 1)]
    public class PvPer : TerrariaPlugin
    {
        public override string Name => "决斗系统";
        public override Version Version => new Version(1, 1, 1);
        public override string Author => "Soofa 羽学修改";
        public override string Description => "不是你死就是我活系列";
        public PvPer(Main game) : base(game)
        {
        }
        public static Configuration Config = new Configuration();
        public static DbManager DbManager = new DbManager(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "决斗系统.sqlite")));
        public static List<Pair> Invitations = new List<Pair>();
        public static List<Pair> ActiveDuels = new List<Pair>();

        public override void Initialize()
        {
            LoadConfig();
            GetDataHandlers.PlayerTeam += OnPlayerChangeTeam;
            GetDataHandlers.TogglePvp += OnPlayerTogglePvP;
            GetDataHandlers.Teleport += OnPlayerTeleport;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            GetDataHandlers.KillMe += OnKill;
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            GeneralHooks.ReloadEvent += LoadConfig;
            TShockAPI.Commands.ChatCommands.Add(new Command("pvper.use", Commands.Duel, "决斗", "pvp"));
        }

        #region 创建与加载配置文件方法
        private static void LoadConfig(ReloadEventArgs args = null!)
        {
            string configPath = Configuration.FilePath;

            if (File.Exists(configPath))
            {
                Config = Configuration.Read(configPath);
                Console.WriteLine($"[决斗系统]已重载");
            }
            else
            {
                Config = new Configuration();
                Config.Write(configPath);
            }
        }
        #endregion

        #region Hooks

        public static void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.PlayerId];
            string name = plr.Name;

            if (Utils.IsPlayerInADuel(args.PlayerId) && !Utils.IsPlayerInArena(plr))
            {
                if (Config.KillPlayer)
                {
                    plr.KillPlayer();
                    TSPlayer.All.SendMessage($"{name}[c/E84B54:逃离]竞技场! 已判定为[C/13A1D1:怯战] 执行惩罚：[C/F86565:死亡]", Color.Yellow);
                    return;
                }
                else
                {
                    plr.DamagePlayer(Config.SlapPlayer);
                    plr.SendMessage($"[c/E84B54:禁止逃离竞技场!]惩罚：[c/F86565:扣{Config.SlapPlayer}血]", Color.Yellow);
                }

                if (Config.PullArena)
                {
                    float playerX = ((Entity)plr.TPlayer).Center.X;
                    float playerY = ((Entity)plr.TPlayer).Center.Y;

                    // 计算玩家到竞技场中心的向量（dx, dy）
                    float centerX = (PvPer.Config.ArenaPosX1 * 16 + PvPer.Config.ArenaPosX2 * 16) / 2f;
                    float centerY = (PvPer.Config.ArenaPosY1 * 16 + PvPer.Config.ArenaPosY2 * 16) / 2f;
                    float dx = centerX - playerX;
                    float dy = centerY - playerY;

                    // 确保拉取半径在竞技场范围内
                    float maxR = Math.Max(Math.Abs(PvPer.Config.ArenaPosX1 * 16 - centerX), Math.Abs(PvPer.Config.ArenaPosY1 * 16 - centerY)) / 2f;
                    float pullR = Math.Min(maxR, Config.PullRange);

                    // 新增配置项：拉取到竞技场中心的距离范围
                    float PullRange = Config.PullRange * 16;

                    // 计算拉取目标坐标
                    float targetX = (float)(centerX + dx * PullRange / Math.Sqrt(dx * dx + dy * dy));
                    float targetY = (float)(centerY + dy * PullRange / Math.Sqrt(dx * dx + dy * dy));

                    PullTP(plr, targetX, targetY, (int)Math.Max(pullR, 0));

                    TSPlayer.All.SendMessage($"{name}[c/E84B54:逃离]竞技场! 执行：[C/4284CD:拉回]", Color.Yellow);
                }
            }
        }

        #region 拉回竞技场方法

        //拉取玩家的方法
        public static void PullTP(TSPlayer plr, float x, float y, int r)
        {
            if (r <= 0)
            {
                plr.Teleport(x, y, 1);
                return;
            }
            float x2 = ((Entity)plr.TPlayer).Center.X;
            float y2 = ((Entity)plr.TPlayer).Center.Y;
            x2 -= x;
            y2 -= y;
            if (x2 != 0f || y2 != 0f)
            {
                double num = Math.Atan2(y2, x2) * 180.0 / Math.PI;
                x2 = (float)((double)r * Math.Cos(num * Math.PI / 180.0));
                y2 = (float)((double)r * Math.Sin(num * Math.PI / 180.0));
                x2 += x;
                y2 += y;
                plr.Teleport(x2, y2, 1);
            }
        }
        #endregion

        public void OnKill(object? sender, GetDataHandlers.KillMeEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.PlayerId];
            Pair? duel = Utils.GetDuel(plr.Index);

            if (duel != null)
            {
                int winnerIndex = duel.Player1 == plr.Index ? duel.Player2 : duel.Player1;
                duel.EndDuel(winnerIndex);
            }
        }

        public static void OnServerLeave(LeaveEventArgs args)
        {
            Pair? duel = Utils.GetDuel(args.Who);
            if (duel != null)
            {
                int winnerIndex = duel.Player1 == args.Who ? duel.Player2 : duel.Player1;
                duel.EndDuel(winnerIndex);
            }
        }

        public static void OnPlayerTogglePvP(object? sender, GetDataHandlers.TogglePvpEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.PlayerId];
            Pair? duel = Utils.GetDuel(args.PlayerId);

            if (duel != null)
            {
                args.Handled = true;
                plr.TPlayer.hostile = true;
                plr.SendData(PacketTypes.TogglePvp, number: plr.Index);
            }
        }
        public static void OnPlayerTeleport(object? sender, GetDataHandlers.TeleportEventArgs args)
        {
            Pair? duel = Utils.GetDuel(args.ID);

            if (duel != null && Config.KillPlayer && !Utils.IsLocationInArena((int)(args.X / 16), (int)(args.Y / 16)))
            {
                args.Player.KillPlayer();
            }
        }

        public static void OnPlayerChangeTeam(object? sender, GetDataHandlers.PlayerTeamEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.PlayerId];
            Pair? duel = Utils.GetDuel(args.PlayerId);

            if (duel != null)
            {
                args.Handled = true;
                plr.TPlayer.team = 0;
                plr.SendData(PacketTypes.PlayerTeam, number: plr.Index);
            }
        }
        #endregion

    }
}