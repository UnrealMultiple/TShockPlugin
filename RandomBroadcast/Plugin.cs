using System.Text;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static Plugin.Configuration;
using Timer = System.Timers.Timer;

namespace Plugin
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {

        #region 插件信息
        public override string Name => "随机广播";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 0, 0);
        public override string Description => "涡轮增压不蒸鸭";

        private Timer? Timer;
        private static readonly Random random = new Random();
        #endregion

        #region 注册与释放
        public Plugin(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.GamePostInitialize.Register(this, OnWorldload);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnWorldload);
                if (Timer != null)
                {
                    Timer.Elapsed -= Message;
                    Timer.Stop();
                    Timer.Dispose();
                    Timer = null;
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private void LoadConfig()
        {
            Config = Configuration.Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[随机发送消息]重新加载配置完毕。");

            //方便修改完间隔的配置项后使用/reload能及时更新计时器
            if (Config.Enable && Timer != null)
            {
                UpdateTimer();
            }

            //如果配置关闭了插件开关，则卸载计时器
            else if (!Config.Enable && Timer != null)
            {
                Timer.Elapsed -= Message;
                Timer.Stop();
                Timer.Dispose();
                Timer = null;
            }
        }
        #endregion

        #region 加载完世界后创建计时器
        private void OnWorldload(EventArgs args)
        {
            if (Config.Enable)
            {
                Timer = new Timer(Config.DefaultTimer * 1000.0 * 60);
                Timer.Elapsed += Message;
                Timer.AutoReset = true;
                Timer.Enabled = true;
            }
        } 
        #endregion

        #region 更新计时器方法
        private void UpdateTimer()
        {
            if (Timer != null)
            {
                Timer.Stop();//停止当前计时器
                Timer.Interval = Config.DefaultTimer * 60 * 1000; //设置计时器循环
                Timer.Start(); //开启计时器
            }
        }
        #endregion

        #region 发送消息方法
        public static void Message(object? sender, ElapsedEventArgs e)
        {
            int SendCout = random.Next(1, Config.Cout + 1);
            for (int i = 0; i < SendCout; i++)
            {
                var item = SelectMessage();
                if (item != null)
                {
                    foreach (var OneMessage in item.Message)
                    {
                        string[] lines = OneMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        List<string> CommandList = new List<string>();
                        StringBuilder StringBuilder = new StringBuilder();

                        foreach (string line in lines)
                        {
                            if (line.StartsWith("/") || line.StartsWith("."))
                            {
                                CommandList.Add(line);
                            }
                            else
                            {
                                if (StringBuilder.Length > 0)
                                    StringBuilder.Append(Environment.NewLine);
                                StringBuilder.Append(line);
                            }
                        }

                        foreach (var command in CommandList)
                        {
                            Commands.HandleCommand(TSPlayer.Server, command);
                        }

                        if (StringBuilder.Length > 0)
                        {
                            string MessageContent = StringBuilder.ToString();
                            TSPlayer.All.SendMessage(MessageContent, (byte)item.ColorRGB[0], (byte)item.ColorRGB[1], (byte)item.ColorRGB[2]);
                        }
                    }
                }
            }
        }
        #endregion

        #region 随机选择消息方法
        private static ItemData? SelectMessage()
        {
            double RandomValue = random.NextDouble();
            double Sum = 0.0;

            foreach (var item in Config.MessageList)
            {
                Sum += item.Rate;
                if (RandomValue <= Sum)
                {
                    return item;
                }
            }
            return null;
        }
        #endregion
    }
}