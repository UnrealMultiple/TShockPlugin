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
    public override string Name => "Rainbow Chat";

    public override string Author => "Professor X 熙恩 羽学";

    public override string Description => "使玩家每次说话的颜色不一样.";

    public override Version Version => new Version(1, 0, 6);

    public RainbowChat(Main game)
        : base(game)
    {
    }
    #region 注册与卸载
    private readonly Random _rand = new Random();
    private readonly bool[] _rainbowChat = new bool[255];
    private readonly bool[] _Gradient = new bool[255];
    internal static Configuration Config = null!;
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += LoadConfig;
        Commands.ChatCommands.Add(new Command("rainbowchat.use", this.RainbowChatCallback, "rainbowchat", "rc"));
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= LoadConfig;
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == this.RainbowChatCallback);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
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
            args.Player.SendSuccessMessage("[彩色聊天]重新加载配置完毕。");
        }
    }
    #endregion


    #region 加入服务器和离开服务器的自动开启方法
    //自动为加入服务器的玩家开启渐变聊天
    private void OnServerJoin(JoinEventArgs args)
    {
        if (args == null || TShock.Players[args.Who] == null)
        {
            return;
        }

        var player = TShock.Players[args.Who];

        if (!player.mute && player.HasPermission(Permissions.canchat))
        {

            this._Gradient[player.Index] = true;
            player.SendSuccessMessage("您的渐变聊天功能已自动开启.");

        }
    }

    //退出服务器的玩家关闭渐变聊天
    private void OnServerLeave(LeaveEventArgs args)
    {
        if (args == null || TShock.Players[args.Who] == null)
        {
            return;
        }

        var player = TShock.Players[args.Who];

        if (!player.mute && player.HasPermission(Permissions.canchat))
        {

            this._Gradient[player.Index] = false;
            player.SendSuccessMessage("您的渐变聊天功能已自动开启.");

        }
    }
    #endregion


    #region 监听玩家聊天并转发的方法
    private void OnChat(ServerChatEventArgs e)
    {
        var gradientStartColor = Config.GradientStartColor;
        var gradientEndColor = Config.GradientEndColor;

        if (e.Handled) { return; }
        if (e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier)) { return; }

        var player = TShock.Players[e.Who];
        if (player == null || !player.HasPermission(Permissions.canchat) || player.mute) { return; }

        if (this._rainbowChat[player.Index])
        {
            var colors = this.GetColors();
            var coloredMessage = string.Join(" ", e.Text.Split(' ').Select((string word, int index) => TShock.Utils.ColorTag(word, colors[this._rand.Next(colors.Count)])));

            // 发送给玩家带有颜色的消息
            TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, coloredMessage), player.Group.R, player.Group.G, player.Group.B);

            // 在控制台打印原始消息
            Console.Write(string.Format($"〖{player.Group.Name}〗[{e.Who}] {player.Name}：{e.Text}\n"));
        }
        else if (this._Gradient[player.Index])
        {
            var gradientMessage = Tools.TextGradient(e.Text, gradientStartColor, gradientEndColor);

            // 发送给玩家带有渐变色的消息
            TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, player.Group.Name, player.Group.Prefix, player.Name, player.Group.Suffix, gradientMessage), player.Group.R, player.Group.G, player.Group.B);

            // 在控制台打印原始消息
            Console.Write(string.Format($"〖{player.Group.Name}〗[{e.Who}] {player.Name}：{e.Text}\n"));
        }

        // 标记事件已处理
        e.Handled = true;
    }
    #endregion

    #region 指令方法
    private void RainbowChatCallback(CommandArgs e)
    {
        var player = e.Player;
        if (e.Parameters.Count == 0)
        {
            player.SendSuccessMessage(string.Format("<彩虹聊天>,可用子命令：\n/rc 随机 - 开启随机单色整句（不能和渐变同开）\n/rc 渐变 - 开启随机整句花色（与随机色相对）\n/rc 渐变 开始 R,G,B - 设置渐变聊天起始颜色\n/rc 渐变 结束 R,G,B - 设置渐变聊天结束颜色", Color.Yellow));
            return;
        }
        switch (e.Parameters[0].ToLower())
        {
            case "random":
            case "随机":
                this.CRainbowChat(e);
                break;
            case "gradient":
            case "渐变":
                if (e.Parameters.Count >= 3)
                {
                    switch (e.Parameters[1].ToLower())
                    {
                        case "开始":
                        case "begin":
                            if (e.Parameters.Count >= 3)
                            {
                                var array2 = e.Parameters[2].Split(',');
                                if (array2.Length == 3 && byte.TryParse(array2[0], out var result4) && byte.TryParse(array2[1], out var result5) && byte.TryParse(array2[2], out var result6))
                                {
                                    Config.GradientStartColor = new Color(result4, result5, result6);
                                    player.SendSuccessMessage($"渐变开始颜色已设置为 R:{result4}, G:{result5}, B:{result6}");
                                }
                                else
                                {
                                    player.SendErrorMessage("无效的起始颜色值。请使用格式如 'R,G,B'（逗号分隔）的颜色表示法。");
                                }
                            }
                            else
                            {
                                player.SendErrorMessage("起始颜色值缺失。请使用如 '/rc 渐变 开始 255,0,0' 的格式。");
                            }
                            break;
                        case "结束":
                        case "end":
                            if (e.Parameters.Count >= 3)
                            {
                                var array = e.Parameters[2].Split(',');
                                if (array.Length == 3 && byte.TryParse(array[0], out var result) && byte.TryParse(array[1], out var result2) && byte.TryParse(array[2], out var result3))
                                {
                                    Config.GradientEndColor = new Color(result, result2, result3);
                                    player.SendSuccessMessage($"渐变结束颜色已设置为 R:{result}, G:{result2}, B:{result3}");
                                }
                                else
                                {
                                    player.SendErrorMessage("无效的结束颜色值。请使用格式如 'R,G,B'（逗号分隔）的颜色表示法。");
                                }
                            }
                            else
                            {
                                player.SendErrorMessage("结束颜色值缺失。请使用如 '/rc 渐变 结束 0,255,0' 的格式。");
                            }
                            break;
                        default:
                            this.GradientChat(e);
                            break;
                    }
                }
                else
                {
                    this.GradientChat(e);
                }
                break;
            default:
                player.SendMessage("<彩虹聊天>,可用子命令：\n/rc 随机 - 开启随机单色整句（不能和渐变同开）\n/rc 渐变 - 开启随机整句花色（与随机色相对）\n/rc 渐变 开始 R,G,B - 设置渐变聊天起始颜色\n/rc 渐变 结束 R,G,B - 设置渐变聊天结束颜色", Color.Yellow);
                break;
        }
    }
    #endregion

    #region 指令调用开启的方法
    private void CRainbowChat(CommandArgs e)
    {
        var targetPlayer = this.GetTargetPlayer(e);
        if (targetPlayer != null)
        {
            this._rainbowChat[targetPlayer.Index] = !this._rainbowChat[targetPlayer.Index];
            var arg = this._rainbowChat[targetPlayer.Index] ? "启用" : "禁用";
            e.Player.SendSuccessMessage($"{targetPlayer.Name} 的彩虹聊天已 {arg}.");
        }
    }

    private void GradientChat(CommandArgs e)
    {
        var targetPlayer = this.GetTargetPlayer(e);
        if (targetPlayer != null)
        {
            this._Gradient[targetPlayer.Index] = !this._Gradient[targetPlayer.Index];
            var arg = this._Gradient[targetPlayer.Index] ? "启用" : "禁用";
            e.Player.SendSuccessMessage($"{targetPlayer.Name} 的渐变聊天已 {arg}.");
        }
    }

    private TSPlayer GetTargetPlayer(CommandArgs e)
    {
        if (e.Parameters.Count <= 1)
        {
            return e.Player;
        }
        var list = TSPlayer.FindByNameOrID(e.Parameters[1]);
        if (list.Count == 0)
        {
            e.Player.SendErrorMessage("错误的玩家!");
            return null;
        }
        if (list.Count > 1)
        {
            e.Player.SendMultipleMatchError((IEnumerable<object>) list.Select((TSPlayer p) => p.Name));
            return null;
        }
        return list[0];
    }
    #endregion

    #region 原随机色方法

    private List<Color> GetColors()
    {
        var list = new List<Color>();
        var properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.PropertyType == typeof(Color) && propertyInfo.CanRead)
            {
                list.Add((Color) propertyInfo.GetValue(null));
            }
        }
        return list;
    }
    #endregion
}