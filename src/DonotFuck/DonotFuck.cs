using LazyAPI;
using System.Text.RegularExpressions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DonotFuck;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    #region 插件信息
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Author => "Cai 羽学";
    public override string Description => GetString("当玩家聊天有敏感词时用*号代替该词");
    public override Version Version => new Version(3, 2, 3);
    #endregion

    #region 注册与释放
    public Plugin(Main game) : base(game) { }
    public override void Initialize()
    {
        ServerApi.Hooks.ServerChat.Register(this, this.OnChat);
        TShockAPI.Commands.ChatCommands.Add(new Command("DonotFuck", Commands.DFCmd, "df"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, this.OnChat);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.DFCmd);
        }
        base.Dispose(disposing);
    }
    #endregion

    public void ReplaceSensitiveWords(ref string text, out List<string> SensitiveWords)
    {
        SensitiveWords = new List<string>();
        foreach (var bad in Configuration.Instance.DirtyWords)
        {
            if (text.Contains(bad, StringComparison.OrdinalIgnoreCase))
            {
                var Replace = new string('*', bad.Length); // 创建与脏话等长的星号字符串
                text = Regex.Replace(text, bad, Replace, RegexOptions.IgnoreCase); // 替换脏话
                SensitiveWords.Add(bad); // 添加敏感词到列表
            }
        }
    }

    #region 检查玩家聊天行为
    private void OnChat(ServerChatEventArgs args)
    {
        var plr = TShock.Players[args.Who];

        if (plr == null || plr.HasPermission("DonotFuck.admin") || plr.Group.Name == "owner")
        {
            return;
        }
        var Text = args.Text;
        if (Text.StartsWith(TShock.Config.Settings.CommandSpecifier) || Text.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
        {
            return;
        }
        this.ReplaceSensitiveWords(ref Text, out var sensitiveWords);
        if (sensitiveWords.Any())
        {
            TSPlayer.All.SendMessage(string.Format(TShock.Config.Settings.ChatFormat, plr.Group.Name, plr.Group.Prefix, plr.Name, plr.Group.Suffix, Text), plr.Group.R, plr.Group.G, plr.Group.B);
            TShock.Log.ConsoleInfo(string.Format($"{plr.Name}：{args.Text}\n"));
            TShock.Log.ConsoleInfo(GetString($"敏感词: {string.Join(",", sensitiveWords)}\n")); // 打印所有敏感词
            Configuration.Instance.Log($"[{DateTime.Now:u}] [{plr.Name}] 触发敏感词: {string.Join(",", sensitiveWords)}");
            args.Handled = true;
        }
    }
    #endregion
}