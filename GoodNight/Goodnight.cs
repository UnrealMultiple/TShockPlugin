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
        public override string Name => "宵禁";
        public override string Author => "Jonesn 羽学";
        public override Version Version => new Version(2, 1, 0);
        public override string Description => "设置服务器无法进入或禁止生成怪物的时段";
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
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            NewProjectile += HandleEvent;
            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= LoadConfig;
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnSpawn);
                ServerApi.Hooks.NpcTransform.Deregister(this, OnTransform);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
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
        private void OnJoin(JoinEventArgs args)
        {
            if (!Config.Enabled) return;
            if (args is JoinEventArgs joinArgs)
            {
                var plr = TShock.Players[joinArgs.Who];
                if (plr != null && !IsExempt(plr.Name))
                {
                    if (Config.DiscPlayers)
                        plr.Disconnect($"{Config.JoinMessage} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.End}");
                }
            }
        }

        private void HandleEvent(object sender, EventArgs e)
        {
            if (!Config.Enabled) return;
            if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.End)
            {
                if (e is NewProjectileEventArgs)
                {
                    var Disconnect = TShock.Players.Where(p => p != null && !IsExempt(p.Name)).ToList();
                    foreach (var plr in Disconnect)
                    {
                        if (Config.DiscPlayers)
                            plr.Disconnect($"{Config.Message2} \n禁止游戏时间:{Config.Time.Start}-{Config.Time.End}");
                    }
                }

                else
                {
                    foreach (var plr in TShock.Players.Where(p => p != null))
                    {
                        if (!IsExempt(plr.Name))
                        {
                            if (Config.DiscPlayers)
                                NetMessage.SendData(2, plr.TPlayer.whoAmI, -1, NetworkText.FromLiteral(Config.Message1 + $"\n禁止游戏时间:{Config.Time.Start}-{Config.Time.End}"), 0, 0f, 0f, 0f, 0, 0, 0);
                        }
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
                int PlayerCount = TShock.Utils.GetActivePlayerCount();
                if (PlayerCount <= Config.MaxPlayers -1)
                {
                    if (Config.Npcs.Contains(Main.npc[args.NpcId].netID))
                    {
                        args.Handled = true;
                        Main.npc[args.NpcId].active = false;
                        TShock.Utils.Broadcast("[宵禁] 服务器进入维护状态\n" +
                            $"[c/FF9187:当前在线人数] 少于 [c/7AE795:{Config.MaxPlayers}人]\n" +
                            $"禁止召唤怪物时间:\n" +
                            $"[c/DF95EC:{Config.Time.Start}] —— [c/FF9187:{Config.Time.End}]", Microsoft.Xna.Framework.Color.Yellow);
                    }
                }
            }
        }

        private void OnTransform(NpcTransformationEventArgs args)
        {
            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.End)
            {
                int PlayerCount = TShock.Utils.GetActivePlayerCount();
                if (PlayerCount <= Config.MaxPlayers -1)
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