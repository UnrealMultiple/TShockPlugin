using Newtonsoft.Json;
using TShockAPI;

namespace AutoFish;

internal class Configuration
{
    #region 实例变量
    [JsonProperty("插件开关", Order = -12)]
    public bool Enabled { get; set; } = true;

    [JsonProperty("多钩钓鱼", Order = -11)]
    public bool MoreHook { get; set; } = true;

    [JsonProperty("多钩上限", Order = -10)]
    public int HookMax { get; set; } = 5;

    [JsonProperty("广告开关", Order = -9)]
    public bool AdvertisementEnabled { get; set; } = true;

    [JsonProperty("广告内容", Order = -8)]
    public string Advertisement { get; set; } = GetString($"\n[i:3456][C/F2F2C7:插件开发] [C/BFDFEA:by] [c/00FFFF:羽学] | [c/7CAEDD:少司命][i:3459]");

    [JsonProperty("Buff表", Order = -6)]
    public Dictionary<int, int> BuffID { get; set; } = new Dictionary<int, int>();

    [JsonProperty("消耗模式", Order = -5)]
    public bool ConMod { get; set; } = false;

    [JsonProperty("消耗数量", Order = -4)]
    public int BaitStack { get; set; } = 10;

    [JsonProperty("自动时长", Order = -3)]
    public int timer { get; set; } = 24;

    [JsonProperty("消耗物品", Order = -2)]
    public List<int> BaitType { get; set; } = new();

    [JsonProperty("额外渔获", Order = -1)]
    public List<int> DoorItems = new();
    #endregion

    #region 预设参数方法
    public void Ints()
    {
        this.BuffID = new Dictionary<int, int>()
        {
            { 80,10 },
            { 122,240 }
        };

        this.BaitType = new List<int> 
        { 
            2002, 2675, 2676, 3191, 3194 
        };

        this.DoorItems = new List<int>
        {
            29,3093,4345
        };
    } 
    #endregion

    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "自动钓鱼.json");

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