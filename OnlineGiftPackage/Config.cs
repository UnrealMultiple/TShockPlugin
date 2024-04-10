using Newtonsoft.Json;
using TShockAPI;

namespace OnlineGiftPackage
{
    public class Configuration
    {
        public bool 启用 { get; set; } = true;

        [JsonProperty("总概率(自动更新)")]
        public int 总概率 { get; private set; } = 60; // 初始化一个默认值

        // 新增计算总概率的方法
        public int CalculateTotalProbability()
        {
            if (礼包列表 != null)
            {
                return 礼包列表.Sum(gift => gift.所占概率);
            }
            else
            {
                Console.WriteLine("无法计算总概率，因为礼包列表为空。");
                return 0;
            }
        }

        // Reload 自动设置总概率的方法
        public void UpdateTotalProbabilityOnReload()
        {
            // 重新加载配置文件
            var reloadedConfig = Read(FilePath);

            // 设置总概率
            总概率 = reloadedConfig.CalculateTotalProbability();
            Write(FilePath); // 将更新后的总概率写回配置文件
        }


        [JsonProperty("发放间隔/秒")]
        public int 发放间隔 { get; set; } = 1800; // 默认发放间隔为1800秒

        [JsonProperty("跳过生命上限")]
        public int SkipStatLifeMax { get; set; } = 500; // 默认值为2000
        public bool 每次发放礼包记录后台 { get; set; } = false; // 默认为开启控制台输出
        public List<Gift> 礼包列表 { get; set; } = new List<Gift>();
        public Dictionary<int, string> 触发序列 { get; set; } = new Dictionary<int, string>();
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "在线礼包.json");

        // 修改Write方法，使其可以接收外部传入的Configuration实例，也可以选择默认使用CreateConfig创建的实例
        public void Write(string filePath)
        {

            // 计算礼包列表总概率
            总概率 = CalculateTotalProbability();

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
                config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(filePath));
            }
            return config; // 确保返回一个Configuration实例
        }

        //创建配置文件方法
        internal static Configuration CreateDefaultConfig()
        {
            Configuration config = new Configuration
            {
                礼包列表 = new List<Gift>
        {
            new Gift
            {
                物品名称 = "铂金币",
                物品ID = 74,
                所占概率 = 1,
                物品数量 = new int[] { 2, 5 },
            },
            new Gift
            {
                物品名称 = "蠕虫罐头",
                物品ID = 4345,
                所占概率 = 1,
                物品数量 = new int[] { 2, 5 },

            },
            new Gift
            {
                物品名称 = "草药袋",
                物品ID = 3093,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "培根",
                物品ID = 3532,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "木匣",
                物品ID = 2334,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "铁匣",
                物品ID = 2335,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },

            },
            new Gift
            {
                物品名称 = "金匣",
                物品ID = 2336,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "地牢匣",
                物品ID = 3205,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "天空匣",
                物品ID = 3206,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "冰冻匣",
                物品ID = 4405,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "绿洲匣",
                物品ID = 4407,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "黑曜石匣",
                物品ID = 4877,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "海洋匣",
                物品ID = 5002,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "暴怒药水",
                物品ID = 2347,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "怒气药水",
                物品ID = 2349,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "传送药水",
                物品ID = 2351,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "生命力药水",
                物品ID = 2345,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "耐力药水",
                物品ID = 2346,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "强效幸运药水",
                物品ID = 4479,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "黑曜石皮药水",
                物品ID = 288,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "羽落药水",
                物品ID = 295,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "洞穴探险药水",
                物品ID = 296,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "战斗药水",
                物品ID = 300,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "挖矿药水",
                物品ID = 2322,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "生命水晶",
                物品ID = 29,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "魔镜",
                物品ID = 50,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "飞虫剑",
                物品ID = 5129,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "草药袋",
                物品ID = 3093,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "血泪",
                物品ID = 4271,
                所占概率 = 1,
                物品数量 = new int[] {1, 2 },
            },
            new Gift
            {
                物品名称 = "幸运马掌",
                物品ID = 158,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "超亮头盔",
                物品ID = 4008,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "非负重石",
                物品ID = 5391,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "水蜡烛",
                物品ID = 148,
                所占概率 = 1,
                物品数量 = new int[] {3, 5 },
            },
            new Gift
            {
                物品名称 = "暗影蜡烛",
                物品ID = 5322,
                所占概率 = 1,
                物品数量 = new int[] {3, 5 },
            },
            new Gift
            {
                物品名称 = "肥料",
                物品ID = 602,
                所占概率 = 1,
                物品数量 = new int[] {3, 5 },
            },
            new Gift
            {
                物品名称 = "魔法灯笼",
                物品ID = 3043,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "挖矿衣",
                物品ID = 410,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "挖矿裤",
                物品ID = 411,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "熔线钓钩",
                物品ID = 2422,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "无底微光桶",
                物品ID = 5364,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "无底蜂蜜桶",
                物品ID = 5302,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "无底熔岩桶",
                物品ID = 4820,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "无底水桶",
                物品ID = 3031,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "湿炸弹",
                物品ID = 4824,
                所占概率 = 1,
                物品数量 = new int[] {3, 5 },
            },
            new Gift
            {
                物品名称 = "恶魔海螺",
                物品ID = 4819,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "魔法海螺",
                物品ID = 4263,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "鱼饵桶",
                物品ID = 4608,
                所占概率 = 1,
                物品数量 = new int[] {5, 9 },
            },
            new Gift
            {
                物品名称 = "花园侏儒",
                物品ID = 4609,
                所占概率 = 1,
                物品数量 = new int[] {2, 5 },
            },
            new Gift
            {
                物品名称 = "掘墓者铲子",
                物品ID = 4711,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "万能晶塔",
                物品ID = 4951,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "月亮领主腿",
                物品ID = 5001,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "火把神的恩宠",
                物品ID = 5043,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "工匠面包",
                物品ID = 5326,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "闭合的虚空袋",
                物品ID = 5325,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "先进战斗技术：卷二",
                物品ID = 5336,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "先进战斗技术",
                物品ID = 4382,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "闪电胡萝卜",
                物品ID = 4777,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "熔火护身符",
                物品ID = 4038,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "泰拉魔刃",
                物品ID = 4144,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
            new Gift
            {
                物品名称 = "真空刃",
                物品ID = 3368,
                所占概率 = 1,
                物品数量 = new int[] {1, 1 },
            },
        },
            };

            // 初始化触发序列，这里只有一条记录，可直接插入无需循环
            for (int i = 1; i <= 200; i++)
            {
                config.触发序列.Add(i * 1, $"[c/55CDFF:服主]送了你1个礼包");
            }
            return config;

        }
    }
}