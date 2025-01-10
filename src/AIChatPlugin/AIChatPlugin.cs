using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

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
        #region 创建配置
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "AI聊天配置.json");
        public static Configuration Config { get; private set; } = new Configuration();
        public class Configuration
        {
            [JsonProperty("模型选择：1为通用，2为速度")]
            public string AIModelselection { get; set; } = "1";
            [JsonProperty("聊天触发词")]
            public string AIChattriggerwords { get; set; } = "AI";
            [JsonProperty("回答字限制")]
            public int AIAnswerwordlimit { get; set; } = 666;
            [JsonProperty("回答换行字")]
            public int AIAnswerwithlinebreaks { get; set; } = 50;
            [JsonProperty("上下文限制")]
            public int AIContextuallimitations { get; set; } = 10;
            [JsonProperty("联网搜索")]
            public bool AIWebbasedsearch { get; set; } = true;
            [JsonProperty("AI超时时间")]
            public int AITimeoutperiod { get; set; } = 100;
            [JsonProperty("名字")]
            public string AIname { get; set; } = "AI";
            [JsonProperty("AI设定")]
            public string AIsettings { get; set; } = "你是一个简洁高效的AI，擅长用一句话精准概括复杂问题。";
            [JsonProperty("temperature温度")]
            public double AItemperature { get; set; } = 0.5;
            [JsonProperty("top_p核采样")]
            public double AINuclearsampling { get; set; } = 0.5;
        }
        #endregion
        #region 读取配置
        private static void LoadConfig()
        {
            if (!File.Exists(FilePath) || new FileInfo(FilePath).Length == 0)
            {
                Config = new Configuration();
                string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            else
            {
                try
                {
                    string jsonContent = File.ReadAllText(FilePath);
                    Configuration tempConfig = JsonConvert.DeserializeObject<Configuration>(jsonContent) ?? new Configuration();
                    if (tempConfig != null)
                    {
                        if (tempConfig.AIModelselection != "1" && tempConfig.AIModelselection != "2")
                        {
                            TShock.Log.ConsoleError($"[AI聊天插件] 模式无效，已保留原配置，并使用默认值");
                            tempConfig.AIModelselection = "1";
                        }
                        tempConfig.AIChattriggerwords ??= "AI";
                        tempConfig.AIAnswerwordlimit = tempConfig.AIAnswerwordlimit > 0 ? tempConfig.AIAnswerwordlimit : 666;
                        tempConfig.AIAnswerwithlinebreaks = tempConfig.AIAnswerwithlinebreaks > 0 ? tempConfig.AIAnswerwithlinebreaks : 50;
                        tempConfig.AIContextuallimitations = tempConfig.AIContextuallimitations > 0 ? tempConfig.AIContextuallimitations : 10;
                        tempConfig.AITimeoutperiod = tempConfig.AITimeoutperiod > 0 ? tempConfig.AITimeoutperiod : 100;
                        tempConfig.AIname ??= "AI";
                        tempConfig.AIsettings ??= "你是一个简洁高效的AI，擅长用一句话精准概括复杂问题。";
                        tempConfig.AINuclearsampling = (tempConfig.AINuclearsampling < 0.0 || tempConfig.AINuclearsampling > 1.0) ? 0.5 : tempConfig.AINuclearsampling;
                        tempConfig.AItemperature = (tempConfig.AItemperature < 0.0 || tempConfig.AItemperature > 1.0) ? 0.5 : tempConfig.AItemperature;
                        Config = tempConfig;
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError($"[AI聊天插件] 加载配置时发生错误，已保留原配置，使用默认值，错误信息：{ex.Message}");
                }
            }
        }
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
        #region 问题审核
        private static readonly Dictionary<int, List<string>> playerContexts = new();
        private static readonly Dictionary<int, bool> isProcessing = new();
        private static DateTime lastCmdTime = DateTime.MinValue;
        private static readonly int cooldownDuration = 5;
        private static void ChatWithAI(TSPlayer player, string question)
        {
            int playerIndex = player.Index;
            if (isProcessing.ContainsKey(playerIndex) && isProcessing[playerIndex])
            {
                player.SendErrorMessage("[i:1344]有其他玩家在询问问题，请排队[i:1344]");
                return;
            }
            if ((DateTime.Now - lastCmdTime).TotalSeconds < cooldownDuration)
            {
                int remainingTime = cooldownDuration - (int)(DateTime.Now - lastCmdTime).TotalSeconds;
                player.SendErrorMessage($"[i:1344]请耐心等待{remainingTime}秒后再输入![i:1344]");
                return;
            }
            if (string.IsNullOrWhiteSpace(question))
            {
                player.SendErrorMessage("[i:1344]您的问题不能为空，请输入您想询问的内容！[i:1344]");
                return;
            }
            lastCmdTime = DateTime.Now;
            player.SendSuccessMessage("[i:1344]正在处理您的请求，请稍候... [i:1344]");
            isProcessing[playerIndex] = true;
            Task.Run(async () =>
            {
                try
                {
                    await ProcessAIChat(player, question);
                }
                catch (Exception ex)
                {
                    string AICLQQSFSCW1 = $"[AI聊天插件] 处理请求时发生错误！详细信息：{ex.Message}";
                    TShock.Log.ConsoleError(AICLQQSFSCW1);
                    if (player.IsLoggedIn)
                    {
                        player.SendErrorMessage(AICLQQSFSCW1);
                    }
                }
                finally
                {
                    isProcessing[playerIndex] = false;
                }
            });
        }
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
        #region 请求处理
        private static async Task ProcessAIChat(TSPlayer player, string question)
        {
            try
            {
                string cleanedQuestion = CleanMessage(question);
                List<string> context = GetContext(player.Index);
                string formattedContext = context.Count > 0
                    ? "上下文信息:\n" + string.Join("\n", context) + "\n\n"
                    : "";
                string model = Config.AIModelselection == "1" ? "glm-4-flash" : "GLM-4V-Flash";
                using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(Config.AITimeoutperiod) };
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer 742701d3fea4bed898578856989cb03c.5mKVzv5shSIqkkS7");
                var tools = new List<object>();
                if (Config.AIWebbasedsearch)
                {
                    tools.Add(new
                    {
                        type = "web_search",
                        web_search = new
                        {
                            enable = true,
                            search_query = question
                        }
                    });
                };
                var requestBody = new
                {
                    model,
                    messages = new[]
                    {
                        new { role = "user", content = formattedContext + $"（设定：{Config.AIsettings}）请您引用以上的上下文信息回答现在的问题(必须不允许复读,如复读请岔开话题,不允许继续下去):\n那," + question }
                    },
                    tools,
                    temperature = Config.AItemperature,
                    top_p = Config.AINuclearsampling,
                };
                var response = await client.PostAsync("https://open.bigmodel.cn/api/paas/v4/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AIResponse>(jsonResponse);
                    string taskId = result?.Id ?? "未提供";
                    TShock.Log.Info($"[调试信息][AI聊天插件] 任务ID: {taskId}");
                    if (result != null && result.Choices != null && result.Choices.Length > 0)
                    {
                        var firstChoice = result.Choices[0];
                        string responseMessage = firstChoice.Message.Content;
                        responseMessage = CleanMessage(responseMessage);
                        if (responseMessage.Length > Config.AIAnswerwordlimit)
                        {
                            responseMessage = TruncateMessage(responseMessage);
                        }
                        string formattedQuestion = FormatMessage(question), formattedResponse = FormatMessage(responseMessage);
                        string broadcastMessage = $"[i:267][c/FFD700:{player.Name}]\n[i:149][c/00FF00:提问: {formattedQuestion}]\n[c/A9A9A9:============================]\n[i:4805][c/FF00FF:{Config.AIname}]\n[i:149][c/FF4500:回答:] {formattedResponse}\n[c/A9A9A9:============================]";
                        TSPlayer.All.SendInfoMessage(broadcastMessage); TShock.Log.ConsoleInfo($"\n[AI聊天插件]\n{player.Name} 提问: {formattedQuestion}\n回答: {formattedResponse}");
                        AddToContext(player.Index, question, true); AddToContext(player.Index, responseMessage, false);
                    }
                    else
                    {
                        string AIWFHYXJG = "[AI聊天插件] 很抱歉，这次未获得有效的AI响应。";
                        TShock.Log.ConsoleError(AIWFHYXJG);
                        if (player.IsLoggedIn)
                        {
                            player.SendErrorMessage(AIWFHYXJG);
                        }
                    }
                }
                else
                {
                    string AIQQSB = $"[AI聊天插件] AI未能及时响应，可能是输出了违规内容，请使用 /bcz 后重新提问，状态码：{response.StatusCode}";
                    TShock.Log.ConsoleError(AIQQSB);
                    if (player.IsLoggedIn)
                    {
                        player.SendErrorMessage(AIQQSB);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                string AIQQCS = "[AI聊天插件] 请求超时！请检查网络连接和API状态，确保一切正常。";
                TShock.Log.ConsoleError(AIQQCS);
                if (player.IsLoggedIn)
                {
                    player.SendErrorMessage(AIQQCS);
                }
            }
            catch (Exception ex)
            {
                string AIQTCW = $"[AI聊天插件] 出现错误！请检查相关设置与请求！详细信息：{ex.Message}";
                TShock.Log.ConsoleError(AIQTCW);
                if (player.IsLoggedIn)
                {
                    player.SendErrorMessage(AIQTCW);
                }
            }
        }
        private class AIResponse
        {
            public Choice[] Choices { get; set; } = Array.Empty<Choice>();
            public string? Id { get; set; }
        }
        private class Choice
        {
            public Message Message { get; set; } = new Message();
        }
        private class Message
        {
            public string Content { get; set; } = string.Empty;
        }
        #endregion
        #region 历史限制
        private static void AddToContext(int playerId, string message, bool isUserMessage)
        {
            if (!playerContexts.ContainsKey(playerId))
            {
                playerContexts[playerId] = new List<string>();
            }
            string taggedMessage = isUserMessage ? $"问题：{message}" : $"回答：{message}";
            if (playerContexts[playerId].Count >= Config.AIContextuallimitations)
            {
                playerContexts[playerId].RemoveAt(0);
            }
            playerContexts[playerId].Add(taggedMessage);
        }
        private static List<string> GetContext(int playerId)
        {
            if (playerContexts.ContainsKey(playerId))
            {
                return playerContexts[playerId];
            }
            else
            {
                return new List<string>();
            }
        }
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
        public static void AIclear(CommandArgs args)
        {
            if (playerContexts.Count == 0)
            {
                args.Player.SendInfoMessage("[AI聊天插件] 当前没有任何人的上下文记录。");
            }
            else
            {
                playerContexts.Clear();
                args.Player.SendSuccessMessage("[AI聊天插件] 所有人的上下文已清除。");
            }
        }
        #endregion
        #region 回答优限
        private static string TruncateMessage(string message)
        {
            if (message.Length <= Config.AIAnswerwordlimit) return message;
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(message);
            StringBuilder truncated = new();
            int count = 0;
            while (enumerator.MoveNext())
            {
                string textElement = enumerator.GetTextElement();
                if (truncated.Length + textElement.Length > Config.AIAnswerwordlimit) break;
                truncated.Append(textElement);
                count++;
            }
            if (count == 0 || truncated.Length >= Config.AIAnswerwordlimit)
            {
                truncated.Append($"\n\n[i:1344]超出字数限制{Config.AIAnswerwordlimit}已省略！[i:1344]");
            }
            return truncated.ToString();
        }
        private static string FormatMessage(string message)
        {
            StringBuilder formattedMessage = new();
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(message);
            int currentLength = 0;
            while (enumerator.MoveNext())
            {
                string textElement = enumerator.GetTextElement();
                if (currentLength + textElement.Length > Config.AIAnswerwithlinebreaks)
                {
                    if (formattedMessage.Length > 0)
                    {
                        formattedMessage.AppendLine();
                    }
                    currentLength = 0;
                }
                formattedMessage.Append(textElement);
                currentLength += textElement.Length;
            }
            return formattedMessage.ToString();
        }
        private static string CleanMessage(string message)
        {
            if (Regex.IsMatch(message, @"[\uD800-\uDBFF][\uDC00-\uDFFF]"))
            {
                return string.Empty;
            }
            return message;
        }
        #endregion
    }
}
