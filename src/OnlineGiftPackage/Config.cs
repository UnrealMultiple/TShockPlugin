using Newtonsoft.Json;
using TShockAPI;

namespace OnlineGiftPackage;

public class Configuration
{
    [JsonProperty("启用")]
    public bool Enabled { get; set; } = true;

    [JsonProperty("总概率(自动更新)")]
    public int TotalProbability { get; private set; } = 60; // 初始化一个默认值

    // 新增计算总概率的方法
    public int CalculateTotalProbability()
    {
        if (this.GiftPackList != null)
        {
            return this.GiftPackList.Sum(gift => gift.Probability);
        }
        else
        {
            Console.WriteLine(GetString("无法计算总概率，因为礼包列表为空。"));
            return 0;
        }
    }

    // Reload 自动设置总概率的方法
    public void UpdateTotalProbabilityOnReload()
    {
        // 重新加载配置文件
        var reloadedConfig = Read(FilePath);

        // 设置总概率
        this.TotalProbability = reloadedConfig.CalculateTotalProbability();
        this.Write(FilePath); // 将更新后的总概率写回配置文件
    }


    [JsonProperty("发放间隔/秒")]
    public int DistributionInterval { get; set; } = 1800; // 默认DistributionInterval为1800秒

    [JsonProperty("跳过生命上限")]
    public int SkipStatLifeMax { get; set; } = 500; // 默认值为2000
    [JsonProperty("每次发放礼包记录后台")]
    public bool OutputConsole { get; set; } = false; // 默认为开启控制台输出
    [JsonProperty("礼包列表")]
    public List<Gift> GiftPackList { get; set; } = new List<Gift>();
    [JsonProperty("触发序列")]
    public Dictionary<int, string> TriggerSequence { get; set; } = new Dictionary<int, string>();
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "在线礼包.json");

    // 修改Write方法，使其可以接收外部传入的Configuration实例，也可以选择默认使用CreateConfig创建的实例
    public void Write(string filePath)
    {

        // 计算礼包列表总概率
        this.TotalProbability = this.CalculateTotalProbability();

        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        using (var sw = new StreamWriter(fs))
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            sw.Write(json);
        }
    }

    // 修改Read方法，当文件不存在时，调用CreateConfig方法并返回新建的配置实例
    public static Configuration Read(string filePath)
    {
        Configuration config;

        if (!File.Exists(filePath))
        {
            config = new Configuration(); // 返回默认配置
        }
        else
        {
            // 读取配置文件并返回一个Configuration实例
            config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(filePath))!;
        }
        return config; // 确保返回一个Configuration实例
    }

    //创建配置文件方法
    internal static Configuration CreateDefaultConfig()
    {
        var config = new Configuration
        {
            GiftPackList = new List<Gift>
    {
        new Gift
        {
            ItemName = "铂金币",
            ItamID = 74,
            Probability = 1,
            ItemAmount = new int[] { 2, 5 },
        },
        new Gift
        {
            ItemName = "蠕虫罐头",
            ItamID = 4345,
            Probability = 1,
            ItemAmount = new int[] { 2, 5 },

        },
        new Gift
        {
            ItemName = "草药袋",
            ItamID = 3093,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "培根",
            ItamID = 3532,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "木匣",
            ItamID = 2334,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "铁匣",
            ItamID = 2335,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },

        },
        new Gift
        {
            ItemName = "金匣",
            ItamID = 2336,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "地牢匣",
            ItamID = 3205,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "天空匣",
            ItamID = 3206,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "冰冻匣",
            ItamID = 4405,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "绿洲匣",
            ItamID = 4407,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "黑曜石匣",
            ItamID = 4877,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "海洋匣",
            ItamID = 5002,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "暴怒药水",
            ItamID = 2347,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "怒气药水",
            ItamID = 2349,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "传送药水",
            ItamID = 2351,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "生命力药水",
            ItamID = 2345,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "耐力药水",
            ItamID = 2346,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "强效幸运药水",
            ItamID = 4479,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "黑曜石皮药水",
            ItamID = 288,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "羽落药水",
            ItamID = 295,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "洞穴探险药水",
            ItamID = 296,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "战斗药水",
            ItamID = 300,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "挖矿药水",
            ItamID = 2322,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "生命水晶",
            ItamID = 29,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "魔镜",
            ItamID = 50,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "飞虫剑",
            ItamID = 5129,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "草药袋",
            ItamID = 3093,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "血泪",
            ItamID = 4271,
            Probability = 1,
            ItemAmount = new int[] {1, 2 },
        },
        new Gift
        {
            ItemName = "幸运马掌",
            ItamID = 158,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "超亮头盔",
            ItamID = 4008,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "非负重石",
            ItamID = 5391,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "水蜡烛",
            ItamID = 148,
            Probability = 1,
            ItemAmount = new int[] {3, 5 },
        },
        new Gift
        {
            ItemName = "暗影蜡烛",
            ItamID = 5322,
            Probability = 1,
            ItemAmount = new int[] {3, 5 },
        },
        new Gift
        {
            ItemName = "肥料",
            ItamID = 602,
            Probability = 1,
            ItemAmount = new int[] {3, 5 },
        },
        new Gift
        {
            ItemName = "魔法灯笼",
            ItamID = 3043,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "挖矿衣",
            ItamID = 410,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "挖矿裤",
            ItamID = 411,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "熔线钓钩",
            ItamID = 2422,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "无底微光桶",
            ItamID = 5364,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "无底蜂蜜桶",
            ItamID = 5302,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "无底熔岩桶",
            ItamID = 4820,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "无底水桶",
            ItamID = 3031,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "湿炸弹",
            ItamID = 4824,
            Probability = 1,
            ItemAmount = new int[] {3, 5 },
        },
        new Gift
        {
            ItemName = "恶魔海螺",
            ItamID = 4819,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "魔法海螺",
            ItamID = 4263,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "鱼饵桶",
            ItamID = 4608,
            Probability = 1,
            ItemAmount = new int[] {5, 9 },
        },
        new Gift
        {
            ItemName = "花园侏儒",
            ItamID = 4609,
            Probability = 1,
            ItemAmount = new int[] {2, 5 },
        },
        new Gift
        {
            ItemName = "掘墓者铲子",
            ItamID = 4711,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "万能晶塔",
            ItamID = 4951,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "月亮领主腿",
            ItamID = 5001,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "火把神的恩宠",
            ItamID = 5043,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "工匠面包",
            ItamID = 5326,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "闭合的虚空袋",
            ItamID = 5325,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "先进战斗技术：卷二",
            ItamID = 5336,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "先进战斗技术",
            ItamID = 4382,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "闪电胡萝卜",
            ItamID = 4777,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "熔火护身符",
            ItamID = 4038,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "泰拉魔刃",
            ItamID = 4144,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
        new Gift
        {
            ItemName = "真空刃",
            ItamID = 3368,
            Probability = 1,
            ItemAmount = new int[] {1, 1 },
        },
    },
        };

        // 初始化TriggerSequence，这里只有一条记录，可直接插入无需循环
        for (var i = 1; i <= 200; i++)
        {
            config.TriggerSequence.Add(i * 1, GetString("[c/55CDFF:服主]送了你1个礼包"));
        }
        return config;

    }
}