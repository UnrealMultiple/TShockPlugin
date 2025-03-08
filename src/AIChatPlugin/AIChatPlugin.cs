using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System.Reflection;
using static AIChatPlugin.Configuration;
using static AIChatPlugin.Utils;

namespace AIChatPlugin;
[ApiVersion(2, 1)]
public class AIChatPlugin : TerrariaPlugin
{
    #region 插件信息
    public override Version Version => new Version(2025, 3, 8);
    public override string Name => "AIChatPlugin";
    public override string Description => GetString("一个提供AI对话的插件");
    public override string Author => "JTL";
    #endregion
    #region 插件启动
    public override void Initialize()
    {
        LoadConfig();
        Commands.ChatCommands.Add(new Command(this.ChatWithAICommand, "ab"));
        Commands.ChatCommands.Add(new Command("aiclear", AIclear, "aiclear"));
        Commands.ChatCommands.Add(new Command(this.BotReset, "bcz"));
        Commands.ChatCommands.Add(new Command(this.BotHelp, "bbz"));
        GeneralHooks.ReloadEvent += this.GeneralHooks_ReloadEvent;
        PlayerHooks.PlayerLogout += this.OnPlayerLogout;
    }
    public AIChatPlugin(Main game) : base(game)
    {
        base.Order = 1;
    }
    #endregion
    #region 插件卸载
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(cmd => cmd.CommandDelegate.Method?.DeclaringType?.Assembly == Assembly.GetExecutingAssembly());
            GeneralHooks.ReloadEvent -= this.GeneralHooks_ReloadEvent;
            PlayerHooks.PlayerLogout -= this.OnPlayerLogout;
        }
    }
    #endregion
    #region 帮助信息
    private void BotHelp(CommandArgs args)
    {
        var helpMessage = GetString("  [i:1344]AIChatPlugin帮助信息[i:1344]\n" +
                                    "[i:1344]/ab                   - 向AI提问\n" +
                                    "[i:1344]/bcz                  - 清除您的上下文\n" +
                                    "[i:1344]/bbz                  - 显示此帮助信息\n" +
                                    "[i:1344]/aiclear              - 清除所有人的上下文");
        args.Player.SendInfoMessage(helpMessage);
    }
    #endregion
    #region 读取配置
    private void GeneralHooks_ReloadEvent(ReloadEventArgs e)
    {
        LoadConfig();
    }
    #endregion
    #region 问题审核
    private void ChatWithAICommand(CommandArgs args)
    {
        if (args.Parameters.Count == 0)
        {
            args.Player.SendErrorMessage(GetString("[i:1344]请输入您想询问的内容！[i:1344]"));
            return;
        }
        var question = string.Join(" ", args.Parameters);
        ChatWithAI(args.Player, question);
    }
    #endregion
    #region 上文重置
    private void OnPlayerLogout(PlayerLogoutEventArgs e)
    {
        var playerId = e.Player.Index;
        if (playerContexts.ContainsKey(playerId))
        {
            playerContexts.Remove(playerId);
        }
    }
    private void BotReset(CommandArgs args)
    {
        if (playerContexts.ContainsKey(args.Player.Index))
        {
            playerContexts.Remove(args.Player.Index);
            args.Player.SendSuccessMessage(GetString("[i:1344]您的上下文记录已重置！[i:1344]"));
        }
        else
        {
            args.Player.SendErrorMessage(GetString("[i:1344]您当前没有上下文记录！[i:1344]"));
        }
    }
    #endregion
}
