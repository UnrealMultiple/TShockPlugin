using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TShockAPI;

namespace AutoStoreItems;

internal class Configuration
{
    [JsonProperty("插件开关", Order = -18)]
    public bool open { get; set; } = true;

    [JsonProperty("使用说明1", Order = -17)]
    public static string Text { get; set; } = GetString("[手持存储模式] 需要选中 [触发存储的物品] 其中1个才会启动存储功能(2024年10月6日已修复)，关闭则背包含有[存储道具]就会自存[储存物品表]的物品");

    [JsonProperty("使用说明2", Order = -16)]
    public static string Text2 { get; set; } = GetString("当开启3种模式任意一个时其他2个模式会默认关闭，[自动存钱] 与 [装备饰品] 不受 [储物速度]影响");

    [JsonProperty("使用说明3", Order = -15)]
    public static string Text3 { get; set; } = GetString("[物品数量] 为触发储存机制的最低数量要求 ");

    [JsonProperty("使用说明4", Order = -14)]
    public static string Text4 { get; set; } = GetString("[装备饰品存储模式] 只会检测装备3格+饰品7格，装备指定饰品(盔甲)玩家只要移动或攻击就会触发存储机制，CPU主频没3Ghz以上的别开 ");

    [JsonProperty("使用说明5", Order = -13)]
    public static string Text5 { get; set; } = GetString("[存在BUG] 收藏的物品会被取消收藏(指虚空袋的药水堆叠进箱子的风险) ,物品如果没放到【存钱罐】等储存空间内是不会触发自动存储的");


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