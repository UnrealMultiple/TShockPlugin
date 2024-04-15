using Microsoft.Data.Sqlite;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace PvPer
{
    [ApiVersion(2, 1)]
    public class PvPer : TerrariaPlugin
    {
        public override string Name => "决斗系统";
        public override Version Version => new Version(1, 0, 2);
        public override string Author => "Soofa 羽学修改";
        public override string Description => "不是你死就是我活系列";
        public PvPer(Main game) : base(game) { }
        public static string ConfigPath = Path.Combine(TShock.SavePath + "/决斗系统.json");
        public static Config Config = new Config();
        public static List<Pair> Invitations = new List<Pair>();
        public static List<Pair> ActiveDuels = new List<Pair>();
        public static DbManager DbManager = new DbManager(new SqliteConnection("Data Source=" + Path.Combine(TShock.SavePath, "决斗系统.sqlite")));
        public override void Initialize()
        {
            GetDataHandlers.PlayerTeam += OnPlayerChangeTeam;
            GetDataHandlers.TogglePvp += OnPlayerTogglePvP;
            GetDataHandlers.Teleport += OnPlayerTeleport;
            GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
            GetDataHandlers.KillMe += OnKill;
            ServerApi.Hooks.ServerLeave.Register(this, OnServerLeave);
            GeneralHooks.ReloadEvent += OnReload;
            TShockAPI.Commands.ChatCommands.Add(new Command("pvper.admin", ClearAllDataCmd, "决斗重置","repvp")
            {
                AllowServer = true, // 允许服务器端使用
                HelpText = "清除数据库中所有玩家的数据（仅限管理员）",
            });

            TShockAPI.Commands.ChatCommands.Add(new Command("pvper.use", Commands.Duel, "决斗", "pvp"));
            Config = Config.Read();

        }

        #region Hooks
        public static void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs args)
        {
            if (Utils.IsPlayerInADuel(args.PlayerId) && !Utils.IsPlayerInArena(args.Player))
            {
                args.Player.DamagePlayer(int.MaxValue);
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
            TSPlayer plr = TShock.Players[args.ID];
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
        private static void OnReload(ReloadEventArgs args)
        {
            args.Player.SendSuccessMessage("决斗系统已重载");
            Config = Config.Read();
        }

        //清理玩家数据库表的方法
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
