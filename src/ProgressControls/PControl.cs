using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace ProgressControl;

[ApiVersion(2, 1)]
public partial class PControl : TerrariaPlugin
{
    //xxx.foreach的return只会让他从=>{}的括号脱离，还是会接着循环，而foreach(var)则能从外部断开，彻底拜托外部函数
    //重置色EA00FF
    //重启色FF9000
    //NPC控制色28FFB8
    //指令色00A8FF
    //金表17，铂金表709，秒表3099，杀怪计数器3095，食人魔勋章3868，计划书903
    public override string Author => "z枳 羽学";
    public override string Description => GetString("计划书");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 12);

    public static Config config = new Config();

    /// <summary>
    /// 用来防止一开服就重置/启，用来缓冲的时间间隔，单位分钟
    /// </summary>
    private static int AvoidTime => 5;
    //新权限
    internal readonly static string p_npc = "pco.npc";
    internal readonly static string p_com = "pco.com";
    internal readonly static string p_reload = "pco.reload";
    internal readonly static string p_reset = "pco.reset";
    internal readonly static string p_admin = "pco.admin";//p_admin最大，包括上面所有

    /// <summary>
    /// 手动计划的状态类
    /// </summary>
    private class CountDown
    {
        /// <summary>
        /// 是否启用了手动计划
        /// </summary>
        public bool enable = false;
        /// <summary>
        /// 手动计划的计时器。单位秒
        /// </summary>
        public int time = 0;
    }

    /// <summary>
    /// 手动重启的类型
    /// </summary>
    private static readonly CountDown countdownRestart = new CountDown();
    /// <summary>
    /// 手动重置的类型
    /// </summary>
    private static readonly CountDown countdownReset = new CountDown();
    /// <summary>
    /// 手动指令计划的类型
    /// </summary>
    private static readonly CountDown countdownCom = new CountDown();

    private static long Timer = 0L;
    /// <summary>
    /// NPC死亡次数触发器
    /// </summary>
    private static readonly Dictionary<int, bool> NpcAndKillCountTrigger = new Dictionary<int, bool>();

    #region 线程
    /// <summary>
    /// 自动计划线程，包括自动重置，自动重启，自动执行指令
    /// </summary>
    private readonly Thread thread_auto = new Thread(thread_autoFun);
    private static void thread_autoFun(object? obj)
    {
        while (!Netplay.Disconnect)
        {
            try
            {
                Thread.Sleep(1000);//每1秒检查一次是否需要重置和重启
                Timer++;
            }
            catch { }
            //刚开服的 AvoidTime 分钟内不要自动重置和重启，手动指令存在则不用执行
            if (config.OpenAutoReset && Timer >= 60 * AvoidTime && !countdownReset.enable)
            {
                var span = config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now;
                var time_temp = span.TotalSeconds % 1;
                var time = time_temp >= 0.5 ? (int) span.TotalSeconds + 1 : (int) span.TotalSeconds;
                if (time >= 5 * 3600) //大于[5小时时，一小时一次广播
                {
                    if (time % 3600 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"世界将于{HoursToM(span.TotalHours, "EA00FF")}后重置"));
                        Console.WriteLine(GetString($"世界将于{HoursToM(span.TotalHours)}后重置"));
                    }
                }
                else if (time >= 3600)// [1h ~ 5h)，30m一次广播
                {
                    if (time % 1800 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"世界将于{HoursToM(span.TotalHours, "EA00FF")}后重置"));
                        Console.WriteLine(GetString($"世界将于{HoursToM(span.TotalHours)}后重置"));
                    }
                }
                else if (time >= 600)// [10m ~ 60m)，10m一次
                {
                    if (time % 600 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"世界将于{HoursToM(span.TotalHours, "EA00FF")}后重置"));
                        Console.WriteLine(GetString($"世界将于{HoursToM(span.TotalHours)}后重置"));
                    }
                }
                else if (time >= 60)//[60s ~ 10m), 1m一次
                {
                    if (time % 60 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"世界将于{HoursToM(span.TotalHours, "EA00FF")}后重置"));
                        Console.WriteLine(GetString($"世界将于{HoursToM(span.TotalHours)}后重置"));
                    }
                }
                else if (time >= 0)//[0 , 60)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"世界将于 [c/EA00FF:{span.Seconds}] 秒后重置"));
                    Console.WriteLine(GetString($"世界将于 {span.Seconds} 秒后重置"));
                }
                else
                {
                    ResetGame();
                    break;
                }
            }
            //刚开服的 AvoidTime 分钟内不要自动重置和重启，手动指令存在时不用执行
            if (config.AutoRestartServer && Timer >= 60 * AvoidTime && !countdownRestart.enable)
            {
                var span = config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now;
                var time_temp = span.TotalSeconds % 1;
                var time = time_temp >= 0.5 ? (int) span.TotalSeconds + 1 : (int) span.TotalSeconds;
                if (time >= 3600 * 5)//[5h, +oo)
                {
                    if (time % 3600 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"服务器将于{HoursToM(span.TotalHours, "FF9000")}后重启"));
                        Console.WriteLine(GetString($"服务器将于{HoursToM(span.TotalHours)}后重启"));
                    }
                }
                else if (time >= 3600)//[1h, 5h)
                {
                    if (time % 1800 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"服务器将于{HoursToM(span.TotalHours, "FF9000")}后重启"));
                        Console.WriteLine(GetString($"服务器将于{HoursToM(span.TotalHours)}后重启"));
                    }
                }
                else if (time >= 600)//[10m, 1h)
                {
                    if (time % 600 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"服务器将于{HoursToM(span.TotalHours, "FF9000")}后重启"));
                        Console.WriteLine(GetString($"服务器将于{HoursToM(span.TotalHours)}后重启"));
                    }
                }
                else if (time >= 60)//[1m, 10m)
                {
                    if (time % 60 == 0)
                    {
                        TSPlayer.All.SendInfoMessage(GetString($"服务器将于{HoursToM(span.TotalHours, "FF9000")}后重启"));
                        Console.WriteLine(GetString($"服务器将于{HoursToM(span.TotalHours)}后重启"));
                    }
                }
                else if (time >= 0)//[0s, 1m)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"世界将于 [c/FF9000:{span.Seconds}] 秒后重启"));
                    Console.WriteLine(GetString($"世界将于 {span.Seconds} 秒后重启"));
                }
                else
                {
                    RestartGame();
                    break;
                }
            }
            //刚开服的 1 分钟里不要自动执行指令
            if (config.OpenAutoCommand && Timer >= 60 && !countdownCom.enable)
            {
                if ((DateTime.Now - config.LasetAutoCommandDate).TotalHours >= config.HowLongTimeOfAutoCommand)
                {
                    ActiveCommands();
                }
            }
        }
    }
    /// <summary>
    /// 手动重置线程
    /// </summary>
    private Thread? thread_reset;
    private static void thread_resetFun(object? obj)
    {
        while (countdownReset.time >= 0 && !Netplay.Disconnect && countdownReset.enable)
        {
            if (countdownReset.time >= 3600 * 5)//5h ~ 无穷，每隔1h发送广播
            {
                if (countdownReset.time % 3600 == 0)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600, "EA00FF")}后重置"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600)}后重置"));
                }
            }
            else if (countdownReset.time >= 3600)//1h ~ 5h，每隔30m发送
            {
                if (countdownReset.time % 1800 == 0)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600, "EA00FF")}后重置"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600)}后重置"));
                }
            }
            else if (countdownReset.time >= 600)// 10m ~ 1h，10m一次
            {
                if (countdownReset.time % 600 == 0)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600, "EA00FF")}后重置"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600)}后重置"));
                }
            }
            else if (countdownReset.time >= 60)//1m ~ 10m，1m一次
            {
                if (countdownReset.time % 60 == 0)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600, "EA00FF")}后重置"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownReset.time * 1.0 / 3600)}秒后重置"));
                }
            }
            else if (countdownReset.time >= 20)//20s ~ 60s，5s一次
            {
                if (countdownReset.time % 5 == 0)
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在 [c/EA00FF:{countdownReset.time}] 秒后重置"));
                    Console.WriteLine(GetString($"服务器将在 {countdownReset.time} 秒后重置"));
                }
            }
            else if (countdownReset.time >= 1)//1s ~ 20s ，一秒一次
            {
                TSPlayer.All.SendInfoMessage(GetString($"服务器将在 [c/EA00FF:{countdownReset.time}] 秒后重置"));
                Console.WriteLine(GetString($"服务器将在 {countdownReset.time} 秒后重置"));
            }
            else
            {
                ResetGame();
                break;
            }
            countdownReset.time--;
            Thread.Sleep(1000);
        }
    }
    /// <summary>
    /// 手动重启线程
    /// </summary>
    private Thread? thread_reload;
    private static void thread_reloadFun(object? obj)
    {
        while (countdownRestart.time >= 0 && !Netplay.Disconnect && countdownRestart.enable)
        {
            if (countdownRestart.time >= 3600 * 5)//大于5小时时
            {
                if (countdownRestart.time % 3600 == 0)//每隔一小时发送自动化广播
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600, "FF9000")}后重启"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600)}后重启"));
                }
            }
            else if (countdownRestart.time >= 3600)//大于1h小于5h
            {
                if (countdownRestart.time % 1800 == 0)//每隔30m发送自动化广播
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600, "FF9000")}后重启"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600)}后重启"));
                }
            }
            else if (countdownRestart.time >= 600)//10m ~ 1h
            {
                if (countdownRestart.time % 600 == 0)//每隔十分钟发一次广播
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600, "FF9000")}后重启"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600)}后重启"));
                }
            }
            else if (countdownRestart.time >= 60)//1m ~ 10m
            {
                if (countdownRestart.time % 60 == 0)//每一分钟发一次广播
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600, "FF9000")}后重启"));
                    Console.WriteLine(GetString($"服务器将在{HoursToM(countdownRestart.time * 1.0 / 3600)}后重启"));
                }
            }
            else if (countdownRestart.time >= 20)//20s ~ 60s
            {
                if (countdownRestart.time % 5 == 0)//5秒发一次广播
                {
                    TSPlayer.All.SendInfoMessage(GetString($"服务器将在 [c/FF9000:{countdownRestart.time}] 秒后重启"));
                    Console.WriteLine(GetString($"服务器将在 {countdownRestart.time} 秒后重启"));
                }
            }
            else if (countdownRestart.time >= 1)//0~20秒内，每秒发一次
            {
                TSPlayer.All.SendInfoMessage(GetString($"服务器将在 [c/FF9000:{countdownRestart.time}] 秒后重启"));
                Console.WriteLine(GetString($"服务器将在 {countdownRestart.time} 秒后重启"));
            }
            else
            {
                RestartGame();
                break;
            }
            countdownRestart.time--;
            Thread.Sleep(1000);
        }
    }
    /// <summary>
    /// 手动指令线程
    /// </summary>
    private Thread? thread_com;
    private static void thread_comFun(object? obj)
    {
        while (countdownCom.time >= 0 && !Netplay.Disconnect && countdownCom.enable)
        {
            if (countdownCom.time <= 0)
            {
                ActiveCommands();
            }
            countdownCom.time--;
            Thread.Sleep(1000);
        }
    }
    #endregion

    public PControl(Main game) : base(game) { }

    public override void Initialize()
    {
        Timer = 0L;
        ServerApi.Hooks.NpcAIUpdate.Register(this, this.NPCAIUpdate);
        ServerApi.Hooks.NpcStrike.Register(this, this.NPCStrike);
        ServerApi.Hooks.GamePostInitialize.Register(this, this.PostInit);
        ServerApi.Hooks.NpcKilled.Register(this, this.OnNPCKilled);
        GeneralHooks.ReloadEvent += this.OnReload;

        Commands.ChatCommands.Add(new Command("", this.PCO, "pco")
        {
            HelpText = GetString("输入 /pco help 来获取该插件的帮助")
        });
        /*
        Commands.ChatCommands.Add(new Command("", Test, "t")
        {
            HelpText = "输入 /t"
        });
        */
    }


    /*
    private void Test(CommandArgs args)
    {
        foreach (var v in config.NpcKillCountForAutoReset)
        {
            TSPlayer.All.SendInfoMessage(Lang.GetNPCNameValue(v.Key) + "死了：" + Main.BestiaryTracker.Kills.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[v.Key]) + "次");
        }

    }
    */

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcAIUpdate.Deregister(this, this.NPCAIUpdate);
            ServerApi.Hooks.NpcStrike.Deregister(this, this.NPCStrike);
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.PostInit);
            ServerApi.Hooks.NpcKilled.Deregister(this, this.OnNPCKilled);
            GeneralHooks.ReloadEvent -= this.OnReload;
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PCO);
        }
        base.Dispose(disposing);
    }


    private void OnReload(ReloadEventArgs e)
    {
        var config = Config.LoadConfigFile();
        e.Player.SendSuccessMessage(GetString("[计划书]重新加载配置完毕。"));

        if (config.HowLongTimeOfAotuResetServer < 0)
        {
            config.HowLongTimeOfAotuResetServer = 0;
        }

        if (config.HowLongTimeOfRestartServer < 0)
        {
            config.HowLongTimeOfRestartServer = 0;
        }

        if (config.HowLongTimeOfAutoCommand < 0)
        {
            config.HowLongTimeOfAutoCommand = 0;
        }
        //对自动重置的加载判断下
        if ((config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer) - DateTime.Now).TotalHours <= AvoidTime * 1.0 / 60.0 && config.OpenAutoReset)
        {
            var temp = (DateTime.Now.AddMinutes(AvoidTime) - config.StartServerDate.AddHours(config.HowLongTimeOfAotuResetServer)).TotalHours;
            e.Player.SendInfoMessage(GetString($"重置世界倒计时过短，需Time大于 {temp:0.00} 来避免立刻重置(最短重置时间5分钟)，修改失败。若要立刻重置，请使用 /pco reset hand 指令"));
            config.HowLongTimeOfAotuResetServer = PControl.config.HowLongTimeOfAotuResetServer;
            config.OpenAutoReset = PControl.config.OpenAutoReset;
        }
        //对重启的判断下
        if ((config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer) - DateTime.Now).TotalHours <= AvoidTime * 1.0 / 60.0 && config.AutoRestartServer)
        {
            var temp = (DateTime.Now.AddMinutes(AvoidTime) - config.LasetServerRestartDate.AddHours(config.HowLongTimeOfRestartServer)).TotalHours;
            e.Player.SendInfoMessage(GetString($"重启服务器倒计时过短，需Time大于 {temp:0.00} 来避免立刻重启(最短重启时间{AvoidTime}分钟)，修改失败。若要立刻重启，请使用 /pco reload hand 指令"));
            config.HowLongTimeOfRestartServer = PControl.config.HowLongTimeOfRestartServer;
            config.AutoRestartServer = PControl.config.AutoRestartServer;
        }
        //对自动执行指令的警告下
        if ((config.LasetAutoCommandDate.AddHours(config.HowLongTimeOfAutoCommand) - DateTime.Now).TotalSeconds <= 60.0 && config.OpenAutoCommand)
        {
            e.Player.SendInfoMessage(GetString("自动执行指令倒计时过短，可能即将开始自动执行指令"));
        }
        //对自动指令的情况过滤下
        var com_temp = new HashSet<string>();
        config.AutoCommandList.ForEach(x =>
        {
            var t = CorrectCommand(x);
            if (t != x && !string.IsNullOrWhiteSpace(t))
            {
                e.Player.SendInfoMessage(GetString($"你在配置文件中提供自动执行的指令：[/{x}] 含有多余斜杠和空格，已转化为等价指令：[/{t}]"));
            }
            if (!string.IsNullOrWhiteSpace(t))
            {
                com_temp.Add(t);
            }
            else
            {
                e.Player.SendInfoMessage(GetString("你在配置文件中提供了一个空白指令，已删除"));
            }
        });
        config.AutoCommandList = com_temp;
        //对预设重置地图名字的修正
        var world_name = new HashSet<string>();
        config.ExpectedUsageWorldFileNameForAotuReset.ForEach(x =>
        {
            var tempname = CorrectFileName(x);
            world_name.Add(tempname);
            if (tempname != x)
            {
                e.Player.SendInfoMessage(GetString($"你在配置文件中提供用于重置的地图名称：[{x}] 含有一些不规则的字符或不必要的后缀，已过滤：[{tempname}]"));
            }
        });
        config.ExpectedUsageWorldFileNameForAotuReset = world_name;
        //对预设重置地图名字和这次开服的名字相同警告下
        var world_name_temp = config.ExpectedUsageWorldFileNameForAotuReset.Count > 0 ? config.ExpectedUsageWorldFileNameForAotuReset.First() : "";
        if (world_name_temp == Main.worldName && (config.OpenAutoReset || countdownReset.enable) && !config.DeleteWorldForReset)
        {
            e.Player.SendWarningMessage(GetString("警告：你当前在配置文件中提供的第一个用于重置的地图名称和当前地图名称相同，这会导致下次重置直接调用本次地图"));
        }

        //更新npc的死亡次数触发器
        NpcAndKillCountTrigger.Clear();
        foreach (var v in config.NpcKillCountForAutoReset)
        {
            NpcAndKillCountTrigger.TryAdd(v.Key, true);
        }
        //对自动指令的情况过滤下
        foreach (var npc in config.NpcKillCountForAutoReset)
        {
            for (var i = 1; i < npc.Value.Count; i++)
            {
                var t = CorrectCommand(npc.Value[i]!.ToString());
                if (t != npc.Value[i]!.ToString() && !string.IsNullOrWhiteSpace(t))
                {
                    e.Player.SendInfoMessage(GetString($"你在配置文件中提供的指令：[/{npc.Value[i]}] 含有多余斜杠和空格，已转化为等价指令：[/{t}]"));
                }
                if (!string.IsNullOrWhiteSpace(t))
                {
                    npc.Value[i] = t;
                }
                else
                {
                    e.Player.SendInfoMessage(GetString("你在配置文件中提供了一个空白指令，已删除"));
                }
            }
        }

        PControl.config = config;
        PControl.config.SaveConfigFile();
    }
}