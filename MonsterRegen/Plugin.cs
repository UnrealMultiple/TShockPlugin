using TerrariaApi.Server;
using Terraria;
using TShockAPI.Hooks;
using TShockAPI;
using System.Timers;
using Microsoft.Xna.Framework;
using Timer = System.Timers.Timer;

namespace Plugin.Configuration
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region 插件信息
        public override string Name => "怪物进度回血";
        public override string Author => "途逗 羽学";
        public override Version Version => new Version(1, 7, 0);
        public override string Description => "通过击杀指定BOSS来提升回复怪物血量阶级数，可自定义回复间隔";
        internal static Configuration Config = new();
        private Timer? Timer;
        private readonly object npcLock = new object();
        #endregion

        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            if (Config.Enable)
            {
                Timer = new Timer(Config.DefaultTimer * 1000.0);
                Timer.Elapsed += HealMonsters;
                Timer.AutoReset = true;
                Timer.Enabled = true;
            }
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.NpcKilled.Register(this, OnNpcKilled);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.NpcKilled.Deregister(this, OnNpcKilled);
                if (Timer != null)
                {
                    Timer.Elapsed -= HealMonsters;
                    Timer.Stop();
                    Timer.Dispose();
                    Timer = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        private void LoadConfig()
        {
            Config = Configuration.Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[怪物进度回血]重新加载配置完毕。");
            if (Config.Enable && Timer != null)
            {
                UpdateTimer();
            }
            else if (Timer != null)
            {
                Timer.Elapsed -= HealMonsters;
                Timer.Stop();
                Timer.Dispose();
                Timer = null;
            }
        }
        #endregion

        #region 更新计时器方法
        private void UpdateTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();

                Config.DefaultTimer -= Config.MinusInterval;
                Config.DefaultTimer = Math.Max(Config.DefaultTimer, Config.IntervalMax);
                if (Level >= Config.ResetLevel)
                {
                    Config.DefaultTimer = Config.RepairTimer;
                }
                Timer.Interval = Config.DefaultTimer * 1000.0;
                Config.Write();
                Timer.Start();
            }
        }
        #endregion

        #region 怪物回血方法
        public int Level { get; private set; } = 0;
        private void HealMonsters(object? sender, ElapsedEventArgs e)
        {
            lock (npcLock)
            {
                NPC[] npcs = Main.npc;

                foreach (NPC npc in npcs)
                {
                    double Heal = npc.lifeMax * Config.HealRatio;
                    if (!npc.active || npc.friendly || !Config.Enable || npc.life >= npc.lifeMax || Config.Excluded.Contains(npc.type))
                        continue;
                    if (Config.Process)
                    {
                        if (Level == 0)
                        {
                            Heal = npc.lifeMax * Config.HealRatio;
                        }

                        else if (Config.LevelList.TryGetValue(Level, out double levelHeal))
                        {
                            Heal = npc.lifeMax * levelHeal;
                        }
                    }

                    Heal = Math.Max(Heal, Config.HealMin);
                    Heal = Math.Min(Heal, Config.HealMax);

                    npc.life = (int)Math.Min(npc.life + Heal, npc.lifeMax);
                    if (Main.netMode != 1)
                    {
                        NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        #endregion

        #region NPC死亡事件 计算进度阶级
        private List<int> DownedBosses = new List<int>();
        private void OnNpcKilled(NpcKilledEventArgs args)
        {
            lock (npcLock)
            {
                if (args.npc == null || !args.npc.boss || !Config.Enable || !Config.Process) return;

                double Heal = Config.LevelList[1];

                var keyBoss = Config.BossList.FirstOrDefault(kb => kb.ID == args.npc.type);

                if (keyBoss == null || keyBoss.Equals(default(Configuration.LevelData))) return;

                if (Config.LevelList.TryGetValue(Level, out double level))
                {
                    Heal = level;
                }

                if (!DownedBosses.Contains(args.npc.type))
                {
                    DownedBosses.Add(args.npc.type);

                    if (Config.BossList.Any())
                    {
                        Level = Config.BossList.Where(kb => DownedBosses.Contains(kb.ID)).Max(kb => kb.Level);
                        UpdateTimer();
                    }
                    else
                    {
                        Level = 0;
                    }

                    TShock.Utils.Broadcast($"\n【怪物进度回血】\n" +
                        $"因击败邪恶Boss,泰拉全体怪物激发进阶回血\n" +
                        $"限制最少回复：[c/3DA1FF:{Config.HealMin}]点\n" +
                        $"限制最多回复：[c/F43A4C:{Config.HealMax}]点\n" +
                        $"当前进度阶级：[c/DAA4EF:{Level}]阶\n" +
                        $"阶段回血比例：[c/FFCEAB:{Heal:P1}] \n" +
                        $"阶段回血间隔：[c/3DFFE3:{Config.DefaultTimer}]秒", Color.Yellow);
                }
            }
        }
        #endregion

    }
}