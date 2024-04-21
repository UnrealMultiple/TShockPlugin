using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace RainbowChat;

[ApiVersion(2, 1)]
public class RainbowChat : TerrariaPlugin
{
    public override string Name => "【五彩斑斓聊天】 Rainbow Chat";

    public override string Author => "Professor X制作,nnt升级/汉化,肝帝熙恩、羽学更新1449";

    public override string Description => "使玩家每次说话的颜色不一样.";

    public override Version Version => new Version(1, 0, 3);

    public RainbowChat(Main game): base(game)
    {
    }

    #region 注册与卸载

    private Random _rand = new Random();
    private bool[] _rainbowChat = new bool[255];
    private bool[] _Gradient = new bool[255];
    internal static Configuration Config = null!;

    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        ServerApi.Hooks.ServerChat.Register(this, OnChat);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 注销事件
            ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
        }

        // 调用基类的Dispose方法
        base.Dispose(disposing);
    }

    private void OnInitialize(EventArgs args)
    {
        Commands.ChatCommands.Add(new Command("rainbowchat.use", RainbowChatCallback, "rainbowchat", "rc"));
    }
    #endregion

    #region 配置文件创建与重读加载方法
    private static void LoadConfig(ReloadEventArgs args = null!)
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
        if (args != null && args.Player != null)
        {
            args.Player.SendSuccessMessage("[彩色聊天]重新加载配置完毕。");
        }
    }
    #endregion

    private void OnChat(ServerChatEventArgs e)
    {
        var gradientStartColor = Config.GradientStartColor;
        var gradientEndColor = Config.GradientEndColor;

        if (e.Handled ) { return; }
        if ((e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))) { return; }

        TSPlayer player = TShock.Players[e.Who];
        if (player == null || !player.HasPermission(Permissions.canchat) || player.mute) { return; }

        if (_rainbowChat[player.Index])
        {
            List<Color> colors = GetColors();
            string coloredMessage = string.Join(" ", e.Text.Split(' ').Select((word, index) => TShock.Utils.ColorTag(word, colors[_rand.Next(colors.Count)])));
            TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, coloredMessage), player.Group.R, player.Group.G, player.Group.B);
            e.Handled = true;
        }
        else if (_Gradient[player.Index])
        {
            string gradientMessage = Tools.TextGradient(e.Text, gradientStartColor, gradientEndColor);
            TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, gradientMessage), player.Group.R, player.Group.G, player.Group.B);
            e.Handled = true;
        }
    }

    private void RainbowChatCallback(CommandArgs e)
    {
        TSPlayer player = e.Player;

        if (e.Parameters.Count == 0)
        {
            player.SendSuccessMessage(string.Format("可用命令：/rc 随机 或 /rc 渐变"));
            return;
        }

        string subCommand = e.Parameters[0].ToLower();

        switch (subCommand)
        {
            case "random":
            case "随机":
                CRainbowChat(e);
                break;
            case "gradient":
            case "渐变":
                GradientChat(e);
                break;
            default:
                player.SendErrorMessage("无效的子命令,可用子命令：/rc 随机 或 /rc 渐变.");
                return;
        }
    }

    private void CRainbowChat(CommandArgs e)
    {
        TSPlayer Player = GetTargetPlayer(e);

        if (Player == null)
        {
            return;
        }

        _rainbowChat[Player.Index] = !_rainbowChat[Player.Index];
        string statusMessage = _rainbowChat[Player.Index] ? "启用" : "禁用";
        e.Player.SendSuccessMessage(string.Format("{0} 的彩虹聊天已 {1}.", Player.Name, statusMessage));
    }

    private void GradientChat(CommandArgs e)
    {
        TSPlayer Player = GetTargetPlayer(e);

        if (Player == null)
        {
            return;
        }

        _Gradient[Player.Index] = !_Gradient[Player.Index];
        string statusMessage = _Gradient[Player.Index] ? "启用" : "禁用";
        e.Player.SendSuccessMessage(string.Format("{0} 的渐变聊天已 {1}.", Player.Name, statusMessage));
    }

    private TSPlayer GetTargetPlayer(CommandArgs e)
    {
        if (e.Parameters.Count <= 1)
        {
            return e.Player;
        }

        List<TSPlayer> players = TSPlayer.FindByNameOrID(e.Parameters[1]);
        if (players.Count == 0)
        {
            e.Player.SendErrorMessage("错误的玩家!");
            return null;
        }
        if (players.Count > 1)
        {
            e.Player.SendMultipleMatchError(players.Select(p => p.Name));
            return null;
        }

        return players[0];
    }

    private List<Color> GetColors()
    {
        List<Color> colors = new List<Color>();
        PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);

        foreach (PropertyInfo property in properties)
        {
            if (property.PropertyType == typeof(Color) && property.CanRead)
            {
                colors.Add((Color)property.GetValue(null));
            }
        }
        return colors;
    }
}