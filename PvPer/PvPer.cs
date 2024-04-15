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
        public override Version Version => new Version(1, 0, 3);
        public override string Author => "Soofa 羽学修改";
        public override string Description => "不是你死就是我活系列";
        public PvPer(Main game) : base(game)
        {

            var configPath = "决斗系统.json";
            var configuration = Configuration.Read(configPath);
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
            TShockAPI.Commands.ChatCommands.Add(new Command("pvper.admin", ClearAllDataCmd, "决斗重置", "repvp")
            {
                AllowServer = true, // 允许服务器端使用
                HelpText = "清除数据库中所有玩家的数据（仅限管理员）",
            });

        }

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

        #region Hooks

        public static void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            TSPlayer plr = TShock.Players[args.PlayerId];

            if (Utils.IsPlayerInADuel(args.PlayerId) && !Utils.IsPlayerInArena(plr))
            {
                if (PvPer.Config.KillPlayer)
                {
                    args.Player.KillPlayer();
                    plr.SendMessage("你已离开竞技场，系统已自动判定怯战 惩罚为：[c/F86565:失败并死亡]。", Color.Yellow);
                }
                else
                {
                    args.Player.DamagePlayer(int.MaxValue);
                    plr.SendMessage("你已离开竞技场，系统已自动判定怯战 惩罚为：[c/F86565:失败并扣血]。", Color.Yellow);
                }
            }
        }


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

            if (duel != null && !Utils.IsLocationInArena((int)(args.X / 16), (int)(args.Y / 16)))
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

        #region 使用指令清理数据库、设置位置方法
        private void ClearAllDataCmd(CommandArgs args)
        {
            // 权限
            if (!args.Player.HasPermission("pvper.admin"))
            {
                args.Player.SendErrorMessage("你没有权限重置决斗系统数据表。");
                TShock.Log.ConsoleInfo("玩家试图执行重置决斗系统数据指令");
                return;
            }
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
    }
}
