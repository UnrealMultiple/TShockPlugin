using Newtonsoft.Json;
using TShockAPI;

namespace AutoAirItem;

internal class Configuration
{
    #region 实例变量
    [JsonProperty("插件指令权限", Order = -16)]
    public string Text { get; set; } = "指令菜单：/air 或 /垃圾，权限名【AutoAir.use】，给玩家权限：/group addperm default AutoAir.use";

    [JsonProperty("使用说明", Order = -15)]
    public string Text2 { get; set; } = "玩家每次进出服都会更新【记录时间】，玩家A离线时间与玩家B登录时间相差超过【清理周期】所设定的时间，则自动清理该玩家A的数据";

    [JsonProperty("插件开关", Order = -14)]
    public bool Open { get; set; } = true;

    [JsonProperty("清理垃圾速度", Order = -2)]
    public int UpdateRate = 10;

    [JsonProperty("广告开关", Order = -1)]
    public bool Enabled { get; set; } = true;

    [JsonProperty("广告内容", Order = -1)]
    public string Advertisement { get; set; } = $"\n[i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学][i:3459]";

    [JsonProperty("是否清理数据", Order = 1)]
    public bool ClearData { get; set; } = true;

    [JsonProperty("清理数据周期/小时", Order = 2)]
    public long timer { get; set; } = 24;


    #endregion

    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "自动垃圾桶.json");

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