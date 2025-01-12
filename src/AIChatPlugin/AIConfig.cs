using Newtonsoft.Json;
using TShockAPI;

namespace AIChatPlugin;
public class Configuration
{
    #region 创建配置
    [JsonProperty("回答字限制")] public int AIAnswerWordsLimit { get; set; } = 666;
    [JsonProperty("回答换行字")] public int AIAnswerWithLinebreaks { get; set; } = 50;
    [JsonProperty("上下文限制")] public int AIContextuallimitations { get; set; } = 10;
    [JsonProperty("超时时间")] public int AITimeoutPeriod { get; set; } = 100;
    [JsonProperty("名字")] public string AIName { get; set; } = "AI";
    [JsonProperty("设定")] public string AISettings { get; set; } = "你是一个简洁高效的AI，擅长用一句话精准概括复杂问题";
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
                TShock.Log.ConsoleError($"[AIChatPlugin] 加载配置时发生错误：{ex.Message}");
            }
        }
    }
    #endregion
}