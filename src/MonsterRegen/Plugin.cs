using Microsoft.Xna.Framework;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Timer = System.Timers.Timer;

namespace Plugin.Configuration;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "途逗 羽学";
    public override Version Version => new Version(1, 7, 4);
    public override string Description => GetString("通过击杀指定BOSS来提升回复怪物血量阶级数，可自定义回复间隔");
    internal static Configuration Config = new();
    private Timer? Timer;
    private readonly object npcLock = new object();
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => this.LoadConfig();
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        this.LoadConfig();
        if (Config.Enable)
        {
            this.Timer = new Timer(Config.DefaultTimer * 1000.0);
            this.Timer.Elapsed += this.HealMonsters;
            this.Timer.AutoReset = true;
            this.Timer.Enabled = true;
        }
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNpcKilled);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNpcKilled);
            if (this.Timer != null)
            {
                this.Timer.Elapsed -= this.HealMonsters;
                this.Timer.Stop();
                this.Timer.Dispose();
                this.Timer = null;
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
        TShock.Log.ConsoleInfo(GetString("[怪物进度回血]重新加载配置完毕。"));
        if (Config.Enable && this.Timer != null)
        {
            this.UpdateTimer();
        }
        else if (this.Timer != null)
        {
            this.Timer.Elapsed -= this.HealMonsters;
            this.Timer.Stop();
            this.Timer.Dispose();
            this.Timer = null;
        }
    }
    #endregion

    #region 更新计时器方法
    private void UpdateTimer()
    {
        if (this.Timer != null)
        {
            this.Timer.Stop();

            Config.DefaultTimer -= Config.MinusInterval;
            Config.DefaultTimer = Math.Max(Config.DefaultTimer, Config.IntervalMax);
            if (this.Level >= Config.ResetLevel)
            {
                Config.DefaultTimer = Config.RepairTimer;
            }
            this.Timer.Interval = Config.DefaultTimer * 1000.0;
            Config.Write();
            this.Timer.Start();
        }
    }
    #endregion

    #region 怪物回血方法
    public int Level { get; private set; } = 0;
    private void HealMonsters(object? sender, ElapsedEventArgs e)
    {
        lock (this.npcLock)
        {
            var npcs = Main.npc;

            foreach (var npc in npcs)
            {
                var Heal = npc.lifeMax * Config.HealRatio;
                if (!npc.active || npc.friendly || !Config.Enable || npc.life >= npc.lifeMax || Config.Excluded.Contains(npc.type))
                {
                    continue;
                }

                if (Config.Process)
                {
                    if (this.Level == 0)
                    {
                        Heal = npc.lifeMax * Config.HealRatio;
                    }

                    else if (Config.LevelList.TryGetValue(this.Level, out var levelHeal))
                    {
                        Heal = npc.lifeMax * levelHeal;
                    }
                }

                Heal = Math.Max(Heal, Config.HealMin);
                Heal = Math.Min(Heal, Config.HealMax);

                npc.life = (int) Math.Min(npc.life + Heal, npc.lifeMax);
                if (Main.netMode != 1)
                {
                    NetMessage.SendData(23, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                }
            }
        }
    }

    #endregion

    #region NPC死亡事件 计算进度阶级
    private readonly List<int> DownedBosses = new List<int>();
    private void OnNpcKilled(NpcKilledEventArgs args)
    {
        lock (this.npcLock)
        {
            if (args.npc == null || !args.npc.boss || !Config.Enable || !Config.Process)
            {
                return;
            }

            var Heal = Config.LevelList[1];

            var keyBoss = Config.BossList.FirstOrDefault(kb => kb.ID == args.npc.type);

            if (keyBoss == null || keyBoss.Equals(default(Configuration.LevelData)))
            {
                return;
            }

            if (Config.LevelList.TryGetValue(this.Level, out var level))
            {
                Heal = level;
            }

            if (!this.DownedBosses.Contains(args.npc.type))
            {
                this.DownedBosses.Add(args.npc.type);

                if (Config.BossList.Any())
                {
                    this.Level = Config.BossList.Where(kb => this.DownedBosses.Contains(kb.ID)).Max(kb => kb.Level);
                    this.UpdateTimer();
                }
                else
                {
                    this.Level = 0;
                }

                TShock.Utils.Broadcast(GetString($"\n【怪物进度回血】\n") +
                    GetString($"因击败邪恶Boss,泰拉全体怪物激发进阶回血\n") +
                    GetString($"限制最少回复：[c/3DA1FF:{Config.HealMin}]点\n") +
                    GetString($"限制最多回复：[c/F43A4C:{Config.HealMax}]点\n") +
                    GetString($"当前进度阶级：[c/DAA4EF:{this.Level}]阶\n") +
                    GetString($"阶段回血比例：[c/FFCEAB:{Heal:P1}] \n") +
                    GetString($"阶段回血间隔：[c/3DFFE3:{Config.DefaultTimer}]秒"), Color.Yellow);
            }
        }
    }
    #endregion

}