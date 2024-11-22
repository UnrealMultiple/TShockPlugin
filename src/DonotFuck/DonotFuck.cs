using System.Text.RegularExpressions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace DonotFuck;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    #region 插件信息
    public override string Name => "Don't Fuck";
    public override string Author => "Cai 羽学";
    public override string Description => "当玩家聊天有敏感词时用*号代替该词";
    public override Version Version => new Version(3, 1, 0);
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        if (!Directory.Exists(this.FilePath)) // 检查配置文件夹是否存在，不存在则根据FilePath路径创建。
        {
            Directory.CreateDirectory(this.FilePath); // 自动创建缺失的文件夹。
        }

        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        TShockAPI.Commands.ChatCommands.Add(new Command("DonotFuck", Commands.DFCmd, "df"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.DFCmd);
        }
        base.Dispose(disposing);
    }
    #endregion

    #region 配置重载读取与写入方法
    internal static Configuration Config = new();
    private static void ReloadConfig(ReloadEventArgs args = null!)
    {
        LoadConfig();
        args.Player.SendInfoMessage(GetString("[禁止脏话]重新加载配置完毕。"));
    }
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Config.Write();
    }
    #endregion

    #region 检查玩家聊天行为
    readonly string FilePath = Path.Combine(TShock.SavePath, "禁止脏话");
    private void OnChat(ServerChatEventArgs args)
    {
        var plr = TShock.Players[args.Who];

        if (plr == null || plr.HasPermission("DonotFuck.admin") || plr.Group.Name == "owner")
        {
            return;
        }

        var Text = args.Text;
        var Words = new List<string>(); // 用于存储玩家使用的敏感词
        var Count = 0;
        var Command = false;

        // 检查是否为命令
        if (Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
        {
            Command = true;
        }

        if (!Command)
        {
            foreach (var bad in Config.DirtyWords)
            {
                if (Text.Contains(bad, StringComparison.OrdinalIgnoreCase))
                {
                    Count++;
                    var Replace = new string('*', bad.Length); // 创建与脏话等长的星号字符串
                    Text = Regex.Replace(Text, bad, Replace, RegexOptions.IgnoreCase); // 替换脏话
                    Words.Add(bad); // 添加敏感词到列表
                }
            }

            if (Count > 0)
            {
                TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, plr.Group.Name, plr.Group.Prefix, plr.Name, plr.Group.Suffix, Text), plr.Group.R, plr.Group.G, plr.Group.B);
                Console.Write(string.Format($"{plr.Name}：{args.Text}\n"));
                Console.Write(GetString($"敏感词: {string.Join(",", Words)}\n")); // 打印所有敏感词

                if (Config.Log)
                {
                    var FilePath = Path.Combine(TShock.SavePath, "禁止脏话", "脏话纪录"); //写入日志的路径
                    Directory.CreateDirectory(FilePath); // 创建日志文件夹
                    var FileName = $"脏话纪录 {DateTime.Now.ToString("yyyy-MM-dd")}.txt"; //给日志名字加上日期
                    File.AppendAllLines(Path.Combine(FilePath, FileName), new string[] { DateTime.Now.ToString("u") + $"\n玩家【{plr.Name}】敏感词:{string.Join(",", Words)}\n" }); //写入日志
                }
            }
        }

        args.Handled = !Command;
    }
    #endregion
}