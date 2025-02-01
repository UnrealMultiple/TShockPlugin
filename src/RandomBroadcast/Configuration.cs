using Newtonsoft.Json;
using TShockAPI;

namespace Plugin;

internal class Configuration
{
    [JsonProperty("使用说明", Order = -2)]
    public string Text { get; set; } = "【总概率】会根据【触发概率】自动计算，【消息内容】含【/或.】的会当指令执行，【同发数量】会随机发多组内容";

    [JsonProperty("开启插件", Order = -1)]
    public bool Enable { get; set; } = true;

    [JsonProperty("同发数量", Order = 0)]
    public int SendCount { get; set; } = 1;

    [JsonProperty("默认间隔/秒", Order = 1)]
    public double DefaultTimer { get; set; } = 1.1;

    [JsonProperty("是否开启触发概率", Order = 2)]
    public bool RateOpen { get; set; } = true;

    [JsonProperty("总概率(自动更新)", Order = 3)]
    public int TotalRate { get; set; }

    [JsonProperty("内容表", Order = 4)]
    public List<ItemData> MessageList { get; set; } = new List<ItemData>();


    #region 数据结构
    public class ItemData
    {
        [JsonProperty("触发概率", Order = 1)]
        public int Rate { get; set; }

        [JsonProperty("消息内容", Order = 2)]
        public string[] Message { get; set; }

        [JsonProperty("触发颜色", Order = 3)]
        public float[] ColorRGB { get; set; } = new float[3];


        public ItemData(int rate, float r, float g, float b, string[] ms)
        {
            this.Rate = rate;
            this.ColorRGB[0] = r;
            this.ColorRGB[1] = g;
            this.ColorRGB[2] = b;
            this.Message = ms;
        }
    }
    #endregion

    #region 计算与更新总概率方法
    public int CalculateTotalRate() //计算总概率方法
    {
        if (this.MessageList != null)
        {
            return this.MessageList.Sum(item => item.Rate);
        }
        else
        {
            TShock.Log.ConsoleInfo(GetString("无法计算总概率，因消息表为空。"));
        }

        return 0;
    }

    public void UpdateTotalRate() //更新总概率方法
    {
        this.TotalRate = Read().CalculateTotalRate();
        this.Write();
    }
    #endregion

    #region 预设参数方法
    public void Ints()
    {
        this.MessageList = new List<ItemData>
        {
            new ItemData(1, 255, 234, 115 ,new[] { ".time 7:30", "我又来啦" }),
            new ItemData(1, 190, 233, 250, new[] { "/time 19:30", "我又走啦" }),
        };
    }
    #endregion

    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "随机广播.json");

    public void Write()
    {
        var json = JsonConvert.SerializeObject(this, Formatting.Indented);
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
            var jsonContent = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<Configuration>(jsonContent)!;
        }
    }
    #endregion

}