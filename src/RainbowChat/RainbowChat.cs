using Microsoft.Xna.Framework;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace RainbowChat;

[ApiVersion(2, 1)]
public class RainbowChat : TerrariaPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "Professor X 熙恩 羽学";
    public override string Description => GetString("使玩家每次说话的颜色不一样.");
    public override Version Version => new Version(1, 0, 10);
    #endregion

    #region 注册与卸载
    public RainbowChat(Main game) : base(game) { }
    public static Random Random = new Random();
    public static bool[] RandomChat = new bool[255];
    public static bool[] Gradient = new bool[255];
    internal static Configuration Config = new();
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        TShockAPI.Commands.ChatCommands.Add(new Command("rainbowchat.use", Commands.RainbowChatCallback, "rainbowchat", "rc"));
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        ServerApi.Hooks.ServerJoin.Register(this, this.OnServerJoin);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            TShockAPI.Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == Commands.RainbowChatCallback);
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnServerJoin);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置文件创建与重读加载方法
    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendSuccessMessage(GetString("[彩色聊天]重新加载配置完毕。"));
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write();
    }
    #endregion

    #region 自动为加入服务器的玩家开启渐变聊天
    private void OnServerJoin(JoinEventArgs args)
    {
        if (args == null || TShock.Players[args.Who] == null || !Config.OpenGradientForJoinServer || !Config.Enabled)
        {
            return;
        }

        var plr = TShock.Players[args.Who];

        if (plr != null && !plr.mute && plr.HasPermission(Permissions.canchat))
        {
            Config.Gradient = true;
            Gradient[plr.Index] = true;
            Config.Write();
            plr.SendSuccessMessage(GetString("您的渐变聊天功能已自动开启."));
        }
    }
    #endregion

    #region 监听玩家聊天并转发的方法
    private void OnChat(ServerChatEventArgs e)
    {
        if (e == null || e.Handled || !Config.Enabled)
        {
            return;
        }

        var plr = TShock.Players[e.Who];
        var ErrorMess = new StringBuilder();

        //如果玩家为空 或 没有聊天权限 或 被禁言 返回
        if (plr == null || !plr.HasPermission(Permissions.canchat) || plr.mute || (!RandomChat[plr.Index] && !Gradient[plr.Index]))
        {
            return;
        }

        //检查命令用的标识
        var Command = false;

        // 检查是否为命令
        if (e.Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || e.Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
        {
            Command = true;
        }

        //检查是否使用其他语言包导致指令为空
        if (string.IsNullOrWhiteSpace(e.Text))
        {
            Command = true;
            if (RainbowChat.Config.ErrorMess)
            {
                ErrorMess.Append(GetString("[i:3454][c/E83849:注意][i:3454]\n"));
                ErrorMess.Append(GetString("使用[c/B0B2D1:非正版客户端]或[c/B0B2D1:语言资源包]导致TS指令异常\n") +
                    GetString("请服主安装[c/B0B2D1:Chireiden.TShock.Omni]插件修复\n") +
                    GetString("或者请玩家使用 [c/B0B2D1:.help] 代替 [c/B0B2D1:/help]"));
                plr.SendInfoMessage(string.Format($"{ErrorMess}", Color.Yellow));
            }
        }

        //不是命令 则继续
        if (!Command)
        {
            //如果开启了随机颜色聊天
            if (RandomChat[plr.Index] && Config.Random)
            {
                //从GetColors方法获取随机颜色
                var colors = Tools.GetColors();

                var Mess = string.Join(" ", e.Text.Split(' ').Select((string word, int index) =>
                TShock.Utils.ColorTag(word, colors[Random.Next(colors.Count)])));

                // 发送给玩家带有颜色的消息
                TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, plr.Group.Name, plr.Group.Prefix, plr.Name, plr.Group.Suffix, Mess), plr.Group.R, plr.Group.G, plr.Group.B);

                // 在控制台打印原始消息
                Console.Write(string.Format($"〖{plr.Group.Name}〗[{e.Who}] {plr.Name}：{e.Text}\n"));
            }

            //如果开启了渐变色聊天
            if (Gradient[plr.Index] && Config.Gradient)
            {
                //渐变开始颜色
                var StartColor = Config.GradientStartColor;
                //渐变结束颜色
                var EndColor = Config.GradientEndColor;

                var Mess = Tools.TextGradient(e.Text, StartColor, EndColor);

                // 发送给玩家带有渐变色的消息
                TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, plr.Group.Name, plr.Group.Prefix, plr.Name, plr.Group.Suffix, Mess), plr.Group.R, plr.Group.G, plr.Group.B);

                // 在控制台打印原始消息
                Console.Write(string.Format($"〖{plr.Group.Name}〗[{e.Who}] {plr.Name}：{e.Text}\n"));
            }
        }

        // 标记事件已处理，除非它是命令
        e.Handled = !Command;
    }
    #endregion

}