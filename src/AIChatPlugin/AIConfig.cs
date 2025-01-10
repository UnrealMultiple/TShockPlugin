using Newtonsoft.Json;
using TShockAPI;

namespace Plugin
{
    internal class AIConfig
    {
        #region 配置创建
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
        #region 配置读取
        internal static void LoadConfig()
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
        #endregion
    }
}