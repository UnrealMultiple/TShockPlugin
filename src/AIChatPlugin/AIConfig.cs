using Newtonsoft.Json;
using TShockAPI;

namespace AIChatPlugin;
public class Configuration
{
    #region 创建配置
    [JsonProperty("Answer the word limit")] public int AIAnswerWordsLimit { get; set; } = 666;
    [JsonProperty("Answer the word count of word wraps")] public int AIAnswerWithLinebreaks { get; set; } = 50;
    [JsonProperty("Contextual limitations")] public int AIContextuallimitations { get; set; } = 10;
    [JsonProperty("The AI answers the timeout")] public int AITimeoutPeriod { get; set; } = 100;
    [JsonProperty("The name displayed when the AI answers")] public string AIName { get; set; } = "AI";
    [JsonProperty("Set up answer requirements")] public string AISettings { get; set; } = "You're a concise and effective multilingual AI that is adept at summarizing complex questions in one sentence in the language of the questioner";
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "AIChat.json");
    public static Configuration Config { get; private set; } = new Configuration();
    #endregion
    #region 读取配置
    public static void LoadConfig()
    {
        if (!File.Exists(FilePath) || new FileInfo(FilePath).Length == 0)
        {
            Config = new Configuration();
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        else
        {
            try
            {
                var jsonContent = File.ReadAllText(FilePath);
                var tempConfig = JsonConvert.DeserializeObject<Configuration>(jsonContent) ?? new Configuration();
                Config = tempConfig;
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"[AIChatPlugin] 加载配置时发生错误：{ex.Message}"));
            }
        }
    }
    #endregion
}