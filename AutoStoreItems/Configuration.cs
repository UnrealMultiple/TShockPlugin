using Newtonsoft.Json;
using TShockAPI;

namespace Plugin
{
    internal class Configuration
    {

        [JsonProperty("插件开关", Order = -10)]
        public bool Enable = true;

        [JsonProperty("持有物品", Order = 0)]
        public int[] HoldItems { get; set; } = new int[1] { 87 };

        [JsonProperty("储存速度", Order = 0)]
        public float time { get; set; } = 10f;

        [JsonProperty("存到存钱罐的物品表", Order = 1)]
        public List<ItemData> Items { get; set; } = new List<ItemData>();

        #region 预设参数方法
        public void Ints()
        {
            Items = new List<ItemData>
            {
                new ItemData(71, 1, 0),
                new ItemData(72, 1, 0),
                new ItemData(73, 1, 0),
                new ItemData(74, 1, 0)
            };
        }
        #endregion

        #region 数据结构
        public class ItemData
        {
            [JsonProperty("物品ID", Order = 1)]
            public int ID { get; set; }

            [JsonProperty("数量", Order = 2)]
            public int Stack { get; set; }

            [JsonProperty("前缀", Order = 3)]
            public int Prefix { get; set; }

            public ItemData( int id, int stack,int prefix)
            {
                ID = id;
                Stack = stack;
                Prefix = prefix;
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