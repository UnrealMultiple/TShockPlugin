using AIChatPlugin;
using Terraria;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TShockAPI;

namespace Plugin;

internal class Utils
{
    #region 问题审核
    public static readonly Dictionary<int, List<string>> playerContexts = new();

    public static readonly bool[] PlayerProcess = new bool[Main.maxPlayers];

    public static DateTime lastCmdTime = DateTime.MinValue;

    public static readonly int cooldownDuration = 5;
    public static void ChatWithAI(TSPlayer player, string question)
    {
        var index = player.Index < 0 ? 255 : player.Index;
        if (PlayerProcess[index])
        {
            player.SendErrorMessage("[i:1344]有其他玩家在询问问题，请排队[i:1344]");
            return;
        }
        if ((DateTime.Now - lastCmdTime).TotalSeconds < cooldownDuration)
        {
            var remainingTime = cooldownDuration - (int) (DateTime.Now - lastCmdTime).TotalSeconds;
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
        PlayerProcess[index] = true;
        Task.Run(async () =>
        {
            try
            {
                await ProcessAIChat(player, question);
            }
            catch (Exception ex)
            {
                var AICLQQSFSCW1 = $"[AI聊天插件] 处理请求时发生错误！详细信息：{ex.Message}";
                TShock.Log.ConsoleError(AICLQQSFSCW1);
                if (player.IsLoggedIn)
                {
                    player.SendErrorMessage(AICLQQSFSCW1);
                }
            }
            finally
            {
                PlayerProcess[index] = false;
            }
        });
    }

  
    #endregion
    #region 请求处理
    public static async Task ProcessAIChat(TSPlayer player, string question)
    {
        try
        {
            var cleanedQuestion = CleanMessage(question);
            var context = GetContext(player.Index);
            var formattedContext = context.Count > 0 ? $"上下文信息:\n{string.Join("\n", context)}\n\n" : "";
            var model = Config.Instance.AIModelselection == AiType.Default ? "glm-4-flash" : "GLM-4V-Flash";
            using HttpClient client = new() { Timeout = TimeSpan.FromSeconds(Config.Instance.AITimeoutperiod) };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer 742701d3fea4bed898578856989cb03c.5mKVzv5shSIqkkS7");
            var tools = new List<object>();
            if (Config.Instance.AIWebbasedsearch)
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
                    new 
                    { 
                        role = "user", 
                        content = formattedContext + $"（设定：{Config.Instance.AIsettings}）请您引用以上的上下文信息回答现在的问题(必须不允许复读,如复读请岔开话题,不允许继续下去):\n那," + question 
                    }
                },
                tools,
                temperature = Config.Instance.AItemperature,
                top_p = Config.Instance.AINuclearsampling,
            };
            var response = await client.PostAsync("https://open.bigmodel.cn/api/paas/v4/chat/completions",
                new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AIResponse>(jsonResponse);
                var taskId = result?.Id ?? "未提供";
                TShock.Log.Info($"[调试信息][AI聊天插件] 任务ID: {taskId}");
                if (result != null && result.Choices != null && result.Choices.Length > 0)
                {
                    var firstChoice = result.Choices[0];
                    var responseMessage = firstChoice.Message.Content;
                    responseMessage = CleanMessage(responseMessage);
                    if (responseMessage.Length > Config.Instance.AIAnswerwordlimit)
                    {
                        responseMessage = TruncateMessage(responseMessage);
                    }
                    string formattedQuestion = FormatMessage(question), formattedResponse = FormatMessage(responseMessage);
                    var broadcastMessage = $"[i:267][c/FFD700:{player.Name}]\n[i:149][c/00FF00:提问: {formattedQuestion}]\n[c/A9A9A9:============================]\n[i:4805][c/FF00FF:{Config.Instance.AIname}]\n[i:149][c/FF4500:回答:] {formattedResponse}\n[c/A9A9A9:============================]";
                    TSPlayer.All.SendInfoMessage(broadcastMessage); TShock.Log.ConsoleInfo($"\n[AI聊天插件]\n{player.Name} 提问: {formattedQuestion}\n回答: {formattedResponse}");
                    AddToContext(player.Index, question, true); 
                    AddToContext(player.Index, responseMessage, false);
                }
                else
                {
                    var AIWFHYXJG = "[AI聊天插件] 很抱歉，这次未获得有效的AI响应。";
                    TShock.Log.ConsoleError(AIWFHYXJG);
                    if (player.IsLoggedIn)
                    {
                        player.SendErrorMessage(AIWFHYXJG);
                    }
                }
            }
            else
            {
                var AIQQSB = $"[AI聊天插件] AI未能及时响应，可能是输出了违规内容，请使用 /bcz 后重新提问，状态码：{response.StatusCode}";
                TShock.Log.ConsoleError(AIQQSB);
                if (player.IsLoggedIn)
                {
                    player.SendErrorMessage(AIQQSB);
                }
            }
        }
        catch (TaskCanceledException)
        {
            var AIQQCS = "[AI聊天插件] 请求超时！请检查网络连接和API状态，确保一切正常。";
            TShock.Log.ConsoleError(AIQQCS);
            if (player.IsLoggedIn)
            {
                player.SendErrorMessage(AIQQCS);
            }
        }
        catch (Exception ex)
        {
            var AIQTCW = $"[AI聊天插件] 出现错误！请检查相关设置与请求！详细信息：{ex.Message}";
            TShock.Log.ConsoleError(AIQTCW);
            if (player.IsLoggedIn)
            {
                player.SendErrorMessage(AIQTCW);
            }
        }
    }
    public class AIResponse
    {
        public Choice[] Choices { get; set; } = Array.Empty<Choice>();
        public string? Id { get; set; }
    }
    public class Choice
    {
        public Message Message { get; set; } = new Message();
    }
    public class Message
    {
        public string Content { get; set; } = string.Empty;
    }
    #endregion
    #region 历史限制
    public static void AddToContext(int playerId, string message, bool isUserMessage)
    {
        if (!playerContexts.ContainsKey(playerId))
        {
            playerContexts[playerId] = new List<string>();
        }
        var taggedMessage = isUserMessage ? $"问题：{message}" : $"回答：{message}";
        if (playerContexts[playerId].Count >= Config.Instance.AIContextuallimitations)
        {
            playerContexts[playerId].RemoveAt(0);
        }
        playerContexts[playerId].Add(taggedMessage);
    }

    public static List<string> GetContext(int playerId)
    {
        return playerContexts.GetValueOrDefault(playerId, new List<string>());
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
    public static string TruncateMessage(string message)
    {
        if (message.Length <= Config.Instance.AIAnswerwordlimit) return message;
        var enumerator = StringInfo.GetTextElementEnumerator(message);
        StringBuilder truncated = new();
        var count = 0;
        while (enumerator.MoveNext())
        {
            var textElement = enumerator.GetTextElement();
            if (truncated.Length + textElement.Length > Config.Instance.AIAnswerwordlimit) break;
            truncated.Append(textElement);
            count++;
        }
        if (count == 0 || truncated.Length >= Config.Instance.AIAnswerwordlimit)
        {
            truncated.Append($"\n\n[i:1344]超出字数限制{Config.Instance.AIAnswerwordlimit}已省略！[i:1344]");
        }
        return truncated.ToString();
    }
    public static string FormatMessage(string message)
    {
        StringBuilder formattedMessage = new();
        var enumerator = StringInfo.GetTextElementEnumerator(message);
        var currentLength = 0;
        while (enumerator.MoveNext())
        {
            var textElement = enumerator.GetTextElement();
            if (currentLength + textElement.Length > Config.Instance.AIAnswerwithlinebreaks)
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
    public static string CleanMessage(string message)
    {
        if (Regex.IsMatch(message, @"[\uD800-\uDBFF][\uDC00-\uDFFF]"))
        {
            return string.Empty;
        }
        return message;
    }
    #endregion
}