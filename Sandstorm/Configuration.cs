using Newtonsoft.Json;
using TShockAPI;

namespace Plugin
{
    internal class Configuration
    {
        [JsonProperty("使用说明", Order = -9)]
        public string Text { get; set; } = "指令：/sd 或 /沙尘暴，权限：Sandstorm.admin ";

        [JsonProperty("是否允许指令开启沙尘暴", Order = -8)]
        public bool CommdEnabled { get; set; } = true;

        [JsonProperty("是否开启广播", Order = -7)]
        public bool SendMessage { get; set; } = true;

        [JsonProperty("广播颜色", Order = -6)]
        public float[] ColorRGB { get; set; } = new float[3] { 255, 234, 115 };

        [JsonProperty("开启沙尘暴的风速目标值", Order = -5)]
        public int on { get; set; } = 35;

        [JsonProperty("关闭沙尘暴的风速目标值", Order = -4)]
        public int off { get; set; } = 20;


        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "Sandstorm.json");

        public void Write()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented); 
            File.WriteAllText(FilePath, json);
        }

        public static Configuration Read()
        {
            if (!File.Exists(FilePath))
            {
                var NewConfig = new Configuration();
                new Configuration().Write();
                return NewConfig;
            }
            else
            {
                string jsonContent = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
            }
        }
        #endregion

    }
}