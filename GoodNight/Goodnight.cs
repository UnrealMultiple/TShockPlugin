using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework;
using static TShockAPI.GetDataHandlers;

namespace Goodnight
{
    [ApiVersion(2, 1)]
    public class Goodnight : TerrariaPlugin
    {
        #region 变量与插件信息
        public override string Name => "宵禁";
        public override string Author => "Jonesn 羽学";
        public override Version Version => new Version(2, 4, 0);
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
            ServerApi.Hooks.NpcKilled.Register(this, OnNPCKilled);
            ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
            TShockAPI.Commands.ChatCommands.Add(new Command("goodnight.admin", Commands.GnCmd, "gn", "宵禁"));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                NewProjectile -= NewProj!;
                GeneralHooks.ReloadEvent -= LoadConfig;
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnSpawn);
                ServerApi.Hooks.NpcTransform.Deregister(this, OnTransform);
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNPCKilled);
                ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
                TShockAPI.Commands.ChatCommands.Remove(new Command("goodnight.admin", Commands.GnCmd, "gn", "宵禁"));

            }
            base.Dispose(disposing);
        }
        #endregion


        #region 配置文件创建与重读加载方法
        internal static void LoadConfig(ReloadEventArgs args = null!)
        {
            if (!File.Exists(Configuration.FilePath))
            {
                Config = new Configuration();
                Config.Write();
            }
            else
                Config = Configuration.Read();
            if (args != null && args.Player != null)
                args.Player.SendSuccessMessage("[宵禁]重新加载配置完毕。");
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
            int PlayerCount = TShock.Utils.GetActivePlayerCount();
            bool NpcList = Config.Npcs.Contains(Main.npc[args.NpcId].netID);
            bool NpcDead = Config.NpcDead.Contains(Main.npc[args.NpcId].netID);
            bool NoPlr = PlayerCount < Config.MaxPlayers && Config.MaxPlayers > 0;
            var NpcListInfo = string.Join(", ", Config.NpcDead.Select(x => TShock.Utils.GetNPCById(x)?.FullName + $"({x})"));
            if (args.Handled || !Config.Enabled) return;

            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                if (NoPlr)
                {
                    if (NpcDead)
                    {
                        args.Handled = false;
                        Main.npc[args.NpcId].active = true;
                        TShock.Utils.Broadcast($"允许召唤的已击败怪物为：\n[c/6EABE9:{NpcListInfo}]",Color.AntiqueWhite);
                    }

                    else if (NpcList)
                    {
                        args.Handled = true;
                        Main.npc[args.NpcId].active = false;
                        TShock.Utils.Broadcast($"【宵禁】当前服务器处于维护时间\n" +
                            $"在线人数少于[c/FF3A4B:{Config.MaxPlayers}人]或该怪物[c/338AE1:不允许召唤]\n" +
                            $"禁止召唤怪物时段: " +
                            $"[c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]", Color.AntiqueWhite);
                    }
                }

                else
                {
                    if (NpcList)
                        args.Handled = false;
                    Main.npc[args.NpcId].active = true;
                }
            }
        }

        private void OnTransform(NpcTransformationEventArgs args)
        {
            int PlayerCount = TShock.Utils.GetActivePlayerCount();
            bool NpcDead = Config.NpcDead.Contains(Main.npc[args.NpcId].netID);
            bool NpcList = Config.Npcs.Contains(Main.npc[args.NpcId].netID);
            bool NoPlr = PlayerCount <= Config.MaxPlayers && Config.MaxPlayers > 0;

            if (args.Handled || !Config.Enabled) return;
            else if (DateTime.Now.TimeOfDay >= Config.Time.Start && DateTime.Now.TimeOfDay < Config.Time.Stop)
            {
                if (NoPlr)
                {
                    if (NpcDead)
                    {
                        Main.npc[args.NpcId].active = true;
                    }

                    else if (NpcList)
                    {
                        Main.npc[args.NpcId].active = false;
                    }
                }

                else
                {
                    if (NpcList)
                        Main.npc[args.NpcId].active = true;
                }
            }
        }

        private Dictionary<int, int> KillCounters = new Dictionary<int, int>();
        private void OnNPCKilled(NpcKilledEventArgs args)
        {
            if (!Config.Enabled || args.npc == null) return;

            int KillNpc = args.npc.netID;
            string npcName = TShock.Utils.GetNPCById(KillNpc)?.FullName ?? "未知NPC";

            if (Config.Npcs.Contains(KillNpc))
            {
                if (!KillCounters.ContainsKey(KillNpc))
                {
                    KillCounters[KillNpc] = 1;
                }
                else
                {
                    KillCounters[KillNpc]++;
                }

                if (!Config.NpcDead.Contains(KillNpc))
                {
                    TShock.Utils.Broadcast($"【宵禁】击败NPC【[c/FF9187:{npcName}({KillNpc})]】\n记录击败进度要求次数:[c/FF3A4B:{KillCounters[KillNpc]}]/[c/E2FA75:{Config.DeadCount}次]", Color.AntiqueWhite);
                }

                if (KillCounters[KillNpc] >= Config.DeadCount)
                {
                    if (!Config.NpcDead.Contains(KillNpc))
                    {
                        Config.NpcDead.Add(KillNpc);
                        Config.Write();
                        var NpcListInfo = string.Join(", ", Config.NpcDead.Select(x => TShock.Utils.GetNPCById(x)?.FullName + $"({x})"));
                        TShock.Utils.Broadcast($"因击杀次数达到[c/E2FA75:{Config.DeadCount}次] 将不再播报计数\n" +
                            $"已记录进宵禁时段允许召唤怪物表：\n[c/6EABE9:{NpcListInfo}]\n" +
                            $"宵禁时段：[c/DF95EC:{Config.Time.Start}] — [c/FF9187:{Config.Time.Stop}]", Color.AntiqueWhite);
                        KillCounters[KillNpc] = 0;
                    }
                }
            }

            else if (KillNpc == Config.ResetNpcDead)
            {
                Config.NpcDead.Clear();
                KillCounters.Clear();
                Config.Write();
                TShock.Utils.Broadcast($"【宵禁】玩家已击败:[c/FF9187:{npcName}({KillNpc})]，现清空宵禁时段中的允许召唤怪物表", Color.AntiqueWhite);
            }
        }
        #endregion

    }
}
