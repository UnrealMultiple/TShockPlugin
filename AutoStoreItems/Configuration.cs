using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TShockAPI;

namespace Plugin
{
    internal class Configuration
    {
        [JsonProperty("插件开关", Order = -12)]
        public bool Enable = true;
        [JsonProperty("储存速度", Order = -11)]
        public float time { get; set; } = 120f;

        [JsonProperty("使用说明1", Order = -10)]
        public static string Text = "[是否手持] 需要选中 [持有物品] 其中1个才会启动存储功能，关闭则背包含有 其中1个就会启动";

        [JsonProperty("使用说明2", Order = -10)]
        public static string Text2 = "[储存速度] 不要低于60帧(推荐120)，否则手动 [连续] 快速放入 [同样物品到存储空间格子] 会导致物品数量翻倍";

        [JsonProperty("使用说明3", Order = -10)]
        public static string Text3 = "[控超堆叠] 会在物品超过9999的时候 对该物品减-1处理 然后通知玩家整理空间";

        [JsonProperty("使用说明4", Order = -10)]
        public static string Text4 = "[物品名] 会在使用 [/Reload] 指令时根据 [物品ID] 自动写入，[物品数量] 为储存最低数量要求 ";

        [JsonProperty("使用说明5", Order = -10)]
        public static string Text5 = "[存在BUG] 收藏的物品会被取消收藏(指虚空袋药水有堆叠进箱子的风险) ";

        [JsonProperty("存钱罐", Order = -9)]
        public bool bank1 = true;
        [JsonProperty("保险箱", Order = -8)]
        public bool bank2 = true;
        [JsonProperty("护卫熔炉", Order = -6)]
        public bool bank3 = true;
        [JsonProperty("虚空袋", Order = -7)]
        public bool bank4 = true;

        [JsonProperty("控超堆叠", Order = -5)]
        public bool clear = true;
        [JsonProperty("存储提示", Order = -4)]
        public bool Mess = true;

        [JsonProperty("是否手持(↓)", Order = -1)]
        public bool Hand = false;

        [JsonProperty("持有物品", Order = 1)]
        public int[] HoldItems { get; set; } = new int[7] { 87, 346 , 3213, 3813, 4076, 4131, 5325 };

        [JsonProperty("存储物品表", Order = 3)]
        public List<ItemData> Items { get; set; } = new List<ItemData>();


        #region 预设参数方法
        public void Ints()
        {
            Items = new List<ItemData>
            {
                new ItemData("", 20,new int[] { 23, }),
                new ItemData("", 1, new int[] { 3817,71,72,73,74,75,1774,1869 })

            };
        }
        #endregion

        #region 数据结构
        public class ItemData
        {
            [JsonProperty("物品名(不用写)", Order = 0)]
            public string Name { get; set; } = "";

            [JsonProperty("物品数量", Order = 1)]
            public int Stack { get; set; }

            [JsonProperty("物品ID", Order = 2)]
            public int[] ID { get; set; }


            public ItemData(string name = "", int stack = 0 , int[] id = null!)
            {
                Name = name ?? "";
                ID = id ?? new int[] { 9 };
                Stack = Math.Max(stack, 1);
            }
        }
        #endregion

        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "自动存储.json");

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