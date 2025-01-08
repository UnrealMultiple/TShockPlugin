using Newtonsoft.Json;
using TShockAPI;

namespace TimeRate;

internal class Configuration
{
    #region 实例变量
    [JsonProperty("指令加速", Order = -8)]
    public bool Enabled { get; set; } = false;

    [JsonProperty("全员睡觉加速", Order = -7)]
    public bool All { get; set; } = true;

    [JsonProperty("单人睡觉加速", Order = -7)]
    public bool One { get; set; } = false;

    [JsonProperty("视觉流畅优化", Order = -6)]
    public bool TimeSet_Packet { get; set; } = true;

    [JsonProperty("加速速率", Order = -5)]
    public int UpdateRate = 180;
    #endregion

    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "时间加速.json");

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
            new Configuration().Write();
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