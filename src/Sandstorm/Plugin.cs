using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Plugin;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{

    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "羽学";
    public override Version Version => new Version(1, 0, 4);
    public override string Description => GetString("使用指令切换沙尘暴");
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game)
    {
        this._reloadHandler = (_) => LoadConfig();
    }
    private readonly GeneralHooks.ReloadEventD _reloadHandler;
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += this._reloadHandler;
        TShockAPI.Commands.ChatCommands.Add(new Command("Sandstorm.admin", ToggleSandstorm, "sd", "沙尘暴"));
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= this._reloadHandler;
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == ToggleSandstorm);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
        TShock.Log.ConsoleInfo(GetString("[Sandstorm]重新加载配置完毕。"));
    }
    #endregion

    #region 切换沙尘暴方法（指令）
    private static void ToggleSandstorm(CommandArgs args)
    {
        if (!Config.CommdEnabled || args == null || args.Player == null)
        {
            return;
        }

        if (Terraria.GameContent.Events.Sandstorm.Happening)
        {
            Terraria.GameContent.Events.Sandstorm.StopSandstorm();
            Main.windSpeedTarget = Config.off;

            if (Config.SendMessage)
            {
                TSPlayer.All.SendMessage(GetString("沙尘暴: 已停止"), (byte) Config.ColorRGB[0], (byte) Config.ColorRGB[1], (byte) Config.ColorRGB[2]);
            }
        }
        else
        {
            Terraria.GameContent.Events.Sandstorm.StartSandstorm();
            Main.windSpeedTarget = Config.on;
            TSPlayer.All.SendData(PacketTypes.WorldInfo);

            if (Config.SendMessage)
            {
                TSPlayer.All.SendMessage(GetString("沙尘暴: 已开始"), (byte) Config.ColorRGB[0], (byte) Config.ColorRGB[1], (byte) Config.ColorRGB[2]);
            }
        }
    }
    #endregion

}