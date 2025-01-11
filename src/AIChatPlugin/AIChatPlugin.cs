using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System.Reflection;
using static AIChatPlugin.AIConfig;
using static AIChatPlugin.Utils;

namespace AIChatPlugin
{
    [ApiVersion(2, 1)]
    public class AIChatPlugin : TerrariaPlugin
    {
        #region 插件信息
        public override Version Version => new Version(2025, 1, 11);
        public override string Name => "AIChatPlugin";
        public override string Description => GetString("一个提供AI对话的插件");
        public override string Author => "镜奇路蓝";
        #endregion
        #region 插件启动
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command("tshock.canchat", ChatWithAICommand, "ab"));
            Commands.ChatCommands.Add(new Command("tshock.cfg.reload", AIreload, "reload"));
            Commands.ChatCommands.Add(new Command("tshock.cfg.reload", AIclear, "aiclear"));
            Commands.ChatCommands.Add(new Command("tshock.canchat", BotReset, "bcz"));
            Commands.ChatCommands.Add(new Command("tshock.canchat", BotHelp, "bbz"));
            PlayerHooks.PlayerLogout += OnPlayerLogout;
        }
        public AIChatPlugin(Main game) : base(game)
        {
            base.Order = 1;
            LoadConfig();
        }
        #endregion
        #region 插件卸载
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commands.ChatCommands.RemoveAll(cmd => cmd.CommandDelegate.Method?.DeclaringType?.Assembly == Assembly.GetExecutingAssembly());
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                playerContexts.Clear();
            }
        }
        #endregion
        #region 帮助信息
        private void BotHelp(CommandArgs args)
        {
            string helpMessage = "  [i:1344]AIChatPlugin帮助信息[i:1344]\n" +
                                 "[i:1344]/ab                   - 向AI提问\n" +
                                 "[i:1344]/bcz                  - 清除您的上下文\n" +
                                 "[i:1344]/bbz                  - 显示此帮助信息\n" +
                                 "[i:1344]/aiclear              - 清除所有人的上下文";
            args.Player.SendInfoMessage(helpMessage);
        }
        #endregion
        #region 读取配置
        private void AIreload(CommandArgs args)
        {
            LoadConfig();
        }
        #endregion
        #region 问题审核
        private void ChatWithAICommand(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendErrorMessage("[i:1344]请输入您想询问的内容！[i:1344]");
                return;
            }
            string question = string.Join(" ", args.Parameters);
            ChatWithAI(args.Player, question);
        }
        #endregion
        #region 上文重置
        private void OnPlayerLogout(PlayerLogoutEventArgs e)
        {
            int playerId = e.Player.Index;
            if (playerContexts.ContainsKey(playerId))
            {
                playerContexts.Remove(playerId);
            }
            if (isProcessing.ContainsKey(playerId))
            {
                isProcessing.Remove(playerId);
            }
        }
        private void BotReset(CommandArgs args)
        {
            if (playerContexts.ContainsKey(args.Player.Index))
            {
                playerContexts.Remove(args.Player.Index);
                args.Player.SendSuccessMessage("[i:1344]您的上下文记录已重置！[i:1344]");
            }
            else
            {
                args.Player.SendErrorMessage("[i:1344]您当前没有上下文记录！[i:1344]");
            }
        }
        #endregion
    }
}