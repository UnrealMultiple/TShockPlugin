using Newtonsoft.Json;
using TShockAPI;

namespace Plugin
{
    internal class Configuration
    {
        [JsonProperty("使用说明", Order = -3)]
        public string Text { get; set; } = "【触发概率】1为100%，【消息内容】含【/或.】的会当指令执行，【同发数量】会随机发多组内容";

        [JsonProperty("开启插件", Order = -2)]
        public bool Enable { get; set; } = true;

        [JsonProperty("是否开启触发概率", Order = -1)]
        public bool RateOpen { get; set; } = true;

        [JsonProperty("同发数量", Order = -1)]
        public int Cout { get; set; } = 1;

        [JsonProperty("默认间隔/秒", Order = 2)]
        public double DefaultTimer { get; set; } = 0.1;

        [JsonProperty("内容表", Order = 3)]
        public List<ItemData> MessageList { get; set; } = new List<ItemData>();


        #region 数据结构
        public class ItemData
        {
            [JsonProperty("触发概率", Order = 1)]
            public double Rate { get; set; }

            [JsonProperty("消息内容", Order = 2)]
            public string[] Message { get; set; }

            [JsonProperty("触发颜色", Order = 3)]
            public float[] ColorRGB { get; set; } = new float[3];


            public ItemData(double rate, float r, float g, float b, string[] ms)
            {
                if (rate < 0 || rate > 1) throw new ArgumentOutOfRangeException ("概率必须介于0和1之间");
                Rate = rate;
                ColorRGB[0] = r;
                ColorRGB[1] = g;
                ColorRGB[2] = b;
                Message = ms;
            }
        }
        #endregion

        #region 预设参数方法
        public void Ints()
        {
            MessageList = new List<ItemData>
            {
                new ItemData(0.5, 255, 234, 115 ,new[] { ".time 7:30", "我又来啦" }),
                new ItemData(0.5, 190, 233, 250, new[] { "/time 19:30", "我又走啦" }),
            };
        }
        #endregion

        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "随机广播.json");

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
                NewConfig.Ints();
                NewConfig.Write();
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