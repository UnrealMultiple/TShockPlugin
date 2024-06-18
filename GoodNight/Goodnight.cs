using Newtonsoft.Json;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace Goodnight
{
    [ApiVersion(2, 1)]
    public class Goodnight : TerrariaPlugin
    {
        #region 变量与插件信息
        public override string Name => "宵禁";
        public override string Author => "Jonesn 羽学";
        public override Version Version => new Version(2, 2, 0);
        public override string Description => "设置服务器无法进入或禁止生成怪物的时段";
        internal static Configuration Config;
        #endregion

        #region 构造注册卸载
        public Goodnight(Main game) : base(game) { }

        public override void Initialize()
        {
            LoadConfig();
            NewProjectile += NewProj!;
            GeneralHooks.ReloadEvent += LoadConfig;
            ServerApi.Hooks.NpcSpawn.Register(this, OnSpawn);
            ServerApi.Hooks.NpcTransform.Register(this, OnTransform);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            TShockAPI.Commands.ChatCommands.Add(new Command("goodnight.admin", Commands.GnCmd, "gn","宵禁"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NewProjectile -= NewProj!;
                GeneralHooks.ReloadEvent -= LoadConfig;
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnSpawn);
                ServerApi.Hooks.NpcTransform.Deregister(this, OnTransform);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                
            }
            base.Dispose(disposing);
        }
        #endregion


        #region 配置文件创建与重读加载方法
        internal static void LoadConfig(ReloadEventArgs args = null!)
        {
            if (File.Exists(Configuration.FilePath)) 
                Config = Configuration.Read(); 
            else
            {
                Config.PlayersList =  new() { "羽学" };
                Config.Npcs = new HashSet<int>() { 4, 13, 14, 15, 35, 36, 37, 50, 113, 114, 125, 126, 127, 128, 129, 130, 131, 134, 135, 136, 222, 245, 246, 247, 248, 249, 262, 266, 370, 396, 397, 398, 400, 439, 440, 422, 493, 507, 517, 636, 657, 668 };
            }
            Config.Write();
            if (args != null && args.Player != null)
            {
                args.Player.SendSuccessMessage("[宵禁]重新加载配置完毕。");
            }
        }
        #endregion


        #region 宵禁
        private void OnJoin(JoinEventArgs args)
        {
            if (!Config.Enabled) return;
            var plr = TShock.Players[args.Who];
            if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                if (Config.DiscPlayers)
                    if (plr != null && !Config.Exempt(plr.Name))
                        plr.Disconnect($"{Config.JoinMessage} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.Stop}");
            }
        }

        private void NewProj(object sender, NewProjectileEventArgs e)
        {
            if (!Config.Enabled) return;
            if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                if (Config.DiscPlayers)
                    if (e.Player != null && !Config.Exempt(e.Player.Name) && !e.Player.HasPermission("goodnight.admin"))
                        e.Player.Disconnect($"{Config.NewProjMessage} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.Stop}");
            }
        }
        #endregion


        #region 禁止召唤怪物
        private void OnSpawn(NpcSpawnEventArgs args)
        {
            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                int PlayerCount = TShock.Utils.GetActivePlayerCount();
                if (PlayerCount < Config.MaxPlayers && Config.MaxPlayers > 0)
                {
                    if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
                    {
                        args.Handled = true;
                        Main.npc[args.NpcId].active = false;
                        TShock.Utils.Broadcast($"【宵禁】当前服务器处于维护时间\n" +
                            $"在线人数少于[c/FF3A4B:{Config.MaxPlayers}人]禁止[c/338AE1:召唤]怪物\n" +
                            $"禁止召唤怪物时间: " +
                            $"[c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]", Microsoft.Xna.Framework.Color.AntiqueWhite);
                    }
                }
            }
        }

        private void OnTransform(NpcTransformationEventArgs args)
        {
            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                int PlayerCount = TShock.Utils.GetActivePlayerCount();
                if (PlayerCount < Config.MaxPlayers && Config.MaxPlayers > 0)
                {
                    if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
                    {
                        Main.npc[args.NpcId].active = false;
                    }
                }
            }
        }
        #endregion

    }
}
