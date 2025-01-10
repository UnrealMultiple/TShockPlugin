using AIChatPlugin;
using Newtonsoft.Json;
using TShockAPI;

namespace Plugin;

public class Config
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "AIChat.json");
    public static Config Instance { get; private set; } = new Config();

    [JsonProperty("模型选择：1为通用，2为速度")]
    public AiType AIModelselection { get; set; } = AiType.Default;

    [JsonProperty("聊天触发词")]
    public string AIChattriggerwords { get; set; } = "AI";

    [JsonProperty("回答字限制")]
    public uint AIAnswerwordlimit { get; set; } = 666;

    [JsonProperty("回答换行字")]
    public uint AIAnswerwithlinebreaks { get; set; } = 50;

    [JsonProperty("上下文限制")]
    public uint AIContextuallimitations { get; set; } = 10;

    [JsonProperty("联网搜索")]
    public bool AIWebbasedsearch { get; set; } = true;

    [JsonProperty("AI超时时间")]
    public uint AITimeoutperiod { get; set; } = 100;

    [JsonProperty("名字")]
    public string AIname { get; set; } = "AI";

    [JsonProperty("AI设定")]
    public string AIsettings { get; set; } = "你是一个简洁高效的AI，擅长用一句话精准概括复杂问题。";
    [JsonProperty("temperature温度")]
    public double AItemperature { get; set; } = 0.5;

    [JsonProperty("top_p核采样")]
    public double AINuclearsampling { get; set; } = 0.5;

    internal static void LoadConfig()
    {
        if (!File.Exists(FilePath))
        {
            var json = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        else
        {
            try
            {
                Instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FilePath)) ?? new Config();

            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[AI聊天插件] 加载配置时发生错误，已保留原配置，使用默认值，错误信息：{ex.Message}");
            }
        }
    }
}