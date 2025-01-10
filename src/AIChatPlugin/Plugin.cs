using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static Plugin.AIConfig;
using static Plugin.Invoke;

namespace Plugin
{
    [ApiVersion(2, 1)]
    public class AIChatPlugin : TerrariaPlugin
    {
        #region 插件信息
        public override Version Version => new Version(2025, 1, 9);
        public override string Name => "AIChatPlugin";
        public override string Description => "air";
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
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
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
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                playerContexts.Clear();
                isProcessing.Clear();
            }
        }
        #endregion
        #region 帮助信息
        private void BotHelp(CommandArgs args)
        {
            string helpMessage = "     [i:1344]AI聊天插件帮助信息[i:1344]\n" +
                                 "[i:1344]/ab                   - 向AI提问\n" +
                                 "[i:1344]/bcz                  - 清除您的上下文\n" +
                                 "[i:1344]/bbz                  - 显示此帮助信息\n" +
                                 "[i:1344]/aiclear              - 清除所有人的上下文\n" +
                                $"[i:1344] - 聊天栏最前面输入“{Config.AIChattriggerwords}”接问题能触发AI对话";
            args.Player.SendInfoMessage(helpMessage);
        }
        #endregion
        #region 读取配置
        private void AIreload(CommandArgs args)
        {
            LoadConfig();
            args.Player.SendSuccessMessage("[AI聊天插件] 配置已重载");
            string configInfo = $"当前配置：\n" +
                                $"模型选择：1为通用，2为速度：{Config.AIModelselection}\n" +
                                $"聊天触发词：{Config.AIChattriggerwords}\n" +
                                $"回答字限制：{Config.AIAnswerwordlimit}\n" +
                                $"回答换行字：{Config.AIAnswerwithlinebreaks}\n" +
                                $"上下文限制：{Config.AIContextuallimitations}\n" +
                                $"联网搜索：{Config.AIWebbasedsearch}\n" +
                                $"AI超时时间：{Config.AITimeoutperiod}\n" +
                                $"名字：{Config.AIname}\n" +
                                $"AI设定：{Config.AIsettings}\n" +
                                $"temperature温度：{Config.AItemperature}\n" +
                                $"top_p核采样：{Config.AINuclearsampling}";
            TShock.Log.ConsoleInfo(configInfo);
        }
        #endregion
        #region 请求拉取
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
        private void OnChat(ServerChatEventArgs args)
        {
            string message = args.Text;
            string triggerPhrase = $"{Config.AIChattriggerwords}";
            if (message.StartsWith(triggerPhrase))
            {
                int triggerLength = triggerPhrase.Length;
                string userQuestion = message[triggerLength..].Trim();
                if (!string.IsNullOrWhiteSpace(userQuestion))
                {
                    TSPlayer player = TShock.Players[args.Who];
                    if (player != null)
                    {
                        ChatWithAI(player, userQuestion);
                    }
                }
            }
        }
        #endregion
        #region 上文清除
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