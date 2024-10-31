using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TShockAPI;

namespace AutoStoreItems;

internal class Configuration
{
    [JsonProperty("使用说明", Order = -16)]
    public static string Text2 { get; set; } = GetString("[自动] [手持] [装备] 当开启3种模式任意一个时其他2个模式会默认关闭，不论哪种模式都需要玩家移动和攻击才会触发储存");

    [JsonProperty("使用说明2", Order = -15)]
    public static string Text3 { get; set; } = GetString("[性能模式] 堆叠达到单格上限物品不进行分堆累积（人多推荐,发2次包）,关闭会为超出上限物品寻找空槽继续累积(最多发4次包)");

    [JsonProperty("使用说明3", Order = -14)]
    public static string Text5 { get; set; } = GetString("[存在BUG] 收藏的物品会被取消收藏(指虚空袋的药水堆叠进箱子的风险) ,物品如果没放到【存钱罐】等储存空间内是不会触发自动存储的");

    [JsonProperty("插件开关", Order = -11)]
    public bool open { get; set; } = true;

    [JsonProperty("性能模式", Order = -10)]
    public bool PM { get; set; } = true;

    [JsonProperty("存钱罐", Order = -9)]
    public bool bank1 { get; set; } = true;
    [JsonProperty("保险箱", Order = -8)]
    public bool bank2 { get; set; } = true;
    [JsonProperty("护卫熔炉", Order = -6)]
    public bool bank3 { get; set; } = true;
    [JsonProperty("虚空袋", Order = -7)]
    public bool bank4 { get; set; } = true;

    [JsonProperty("触发存储的物品ID", Order = 3)]
    public int[] BankItems { get; set; } = new int[] { 87, 346, 3213, 3813, 4076, 4131, 5098, 5325 };

    [JsonProperty("装备饰品的物品ID", Order = 4)]
    public int[] ArmorItem { get; set; } = new int[] { 88, 410, 411, 489, 490, 491, 855, 935, 1301, 2220, 2998, 3034, 3035, 3061, 3068, 4008, 4056, 4989, 5098, 5107, 5126 };


    #region 读取与创建配置文件方法
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "自动存储.json");

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