using Terraria;
using Terraria.Localization;
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
        public override string Name => "宵禁 Goodnight";
        public override string Author => "Jonesn 羽学";
        public override Version Version => new Version(2, 0, 0);
        public override string Description => "宵禁插件，设置服务器无法进入的时段";
        private static Configuration Config;
        #endregion


        #region 构造注册卸载
        public Goodnight(Main game) : base(game) { }

        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += LoadConfig;
            ServerApi.Hooks.NpcSpawn.Register(this, OnSpawn);
            ServerApi.Hooks.NpcTransform.Register(this, OnTransform);
            NewProjectile += HandleEvent;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= LoadConfig;
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnSpawn);
                ServerApi.Hooks.NpcTransform.Deregister(this, OnTransform);
                NewProjectile -= HandleEvent;
            }
            base.Dispose(disposing);
        }
        #endregion


        #region 配置文件创建与重读加载方法
        private static void LoadConfig(ReloadEventArgs args = null!)
        {
            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);
            if (args != null && args.Player != null)
            {
                args.Player.SendSuccessMessage("[宵禁]重新加载配置完毕。");
            }
        }
        #endregion


        #region 宵禁
        private void HandleEvent(object sender, EventArgs e)
        {
            if (!Config.Enabled) return;
            if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.End)
            {
                if (e is JoinEventArgs joinArgs)
                {
                    var plr = TShock.Players[joinArgs.Who];
                    if (plr != null && !IsExempt(plr.Name))
                    {
                        if (Config.DiscPlayers)
                            plr.Disconnect("当前为宵禁时间，无法加入游戏。");
                    }
                }

                else if (e is NewProjectileEventArgs)
                {
                    var Disconnect = TShock.Players.Where(p => p != null && !IsExempt(p.Name)).ToList();
                    foreach (var plr in Disconnect)
                    {
                        if (Config.DiscPlayers)
                            plr.Disconnect("到点了，晚安");
                    }
                }

                else
                {
                    foreach (var plr in TShock.Players.Where(p => p != null))
                    {
                        if (Config.DiscPlayers)
                            NetMessage.SendData(2, plr.TPlayer.whoAmI, -1, NetworkText.FromLiteral(Config.Message), 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        private bool IsExempt(string playerName)
        {
            return Config.ExemptPlayers.Contains(playerName);
        }

        #endregion

        #region 禁止召唤怪物
        private void OnSpawn(NpcSpawnEventArgs args)
        {
            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.End)
            {
                if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
                {
                    args.Handled = true;
                    Main.npc[args.NpcId].active = false;
                }
            }
        }

        private void OnTransform(NpcTransformationEventArgs args)
        {
            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.End)
            {
                if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
                {
                    Main.npc[args.NpcId].active = false;
                }
            }
        }
        #endregion
    }
}