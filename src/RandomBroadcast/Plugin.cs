using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static Plugin.Configuration;

namespace Plugin;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "羽学";
    public override Version Version => new Version(1, 0, 6);
    public override string Description => GetString("定时随机发送一条广播内容");
    private static readonly Random random = new Random();
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
        GeneralHooks.ReloadEvent += this._reloadHandler;
        ServerApi.Hooks.GameUpdate.Register(this, this.OnGameUpdate);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnGameUpdate);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private void LoadConfig()
    {
        Config = Configuration.Read();
        Config.UpdateTotalRate();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[随机发送消息]重新加载配置完毕。"));
    }
    #endregion

    #region 更新计时器方法
    public long TimerCount;
    private void OnGameUpdate(EventArgs args)
    {
        if (args == null || !Config.Enable)
        {
            return;
        }

        this.TimerCount++;
        if (this.TimerCount % (Config.DefaultTimer * 60) == 0)
        {
            Message();
            this.TimerCount = 0;
        }
    }
    #endregion

    #region 发送消息方法
    public static void Message()
    {
        for (var i = 0; i < Config.SendCount; i++)
        {
            var item = SelectMessage()!;
            if (item != null)
            {
                foreach (var OneMessage in item.Message)
                {
                    var lines = OneMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    var CmdList = new List<string>();
                    var StringBuilder = new StringBuilder();

                    foreach (var line in lines)
                    {
                        if (line.StartsWith(TShock.Config.Settings.CommandSpecifier) || line.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
                        {
                            CmdList.Add(line);
                        }
                        else
                        {
                            if (StringBuilder.Length > 0)
                            {
                                StringBuilder.Append(Environment.NewLine);
                            }

                            StringBuilder.Append(line);
                        }
                    }

                    foreach (var Cmd in CmdList)
                    {
                        Commands.HandleCommand(TSPlayer.Server, Cmd);
                    }

                    if (StringBuilder.Length > 0)
                    {
                        TSPlayer.All.SendMessage(StringBuilder.ToString(), (byte) item.ColorRGB[0], (byte) item.ColorRGB[1], (byte) item.ColorRGB[2]);
                    }
                }
            }
        }
    }
    #endregion

    #region 随机选择消息方法
    private static ItemData? SelectMessage()
    {
        var RandomValue = random.Next(Config.TotalRate);
        var Sum = 0;
        if (Config.RateOpen)
        {
            foreach (var item in Config.MessageList)
            {
                Sum += item.Rate;
                if (RandomValue < Sum)
                {
                    return item;
                }
            }
        }
        else
        {
            return Config.MessageList.OrderBy(f => Guid.NewGuid()).First();
        }
        return null;
    }
    #endregion
}