using Newtonsoft.Json;
using TShockAPI;

namespace SpawnInfra
{
    internal class Configuration
    {
        [JsonProperty("使用说明", Order = -11)]
        public string Text { get; set; } = "注意地狱平台深度会清理上面方块，每次重置服务器前使用一遍：/rm 重置";

        [JsonProperty("开服自动基建", Order = -10)]
        public bool Enabled { get; set; } = true;

        [JsonProperty("是否建监狱", Order = -9)]
        public bool Enabled2 { get; set; } = true;
        [JsonProperty("是否建世界平台", Order = -8)]
        public bool Enabled3 { get; set; } = true;
        [JsonProperty("是否建世界轨道", Order = -7)]
        public bool Enabled4 { get; set; } = true;
        [JsonProperty("是否建左海平台", Order = -7)]
        public bool Enabled4_1 { get; set; } = true;
        [JsonProperty("是否建地狱直通车", Order = -6)]
        public bool Enabled5 { get; set; } = true;
        [JsonProperty("是否建地狱平台", Order = -5)]
        public bool Enabled6 { get; set; } = true;
        [JsonProperty("是否建地狱轨道", Order = -4)]
        public bool Enabled7 { get; set; } = true;

        [JsonProperty("监狱集群", Order = 1)]
        public List<ItemData> Prison { get; set; } = new List<ItemData>();

        [JsonProperty("世界/左海平台", Order = 2)]
        public List<ItemData1> WorldPlatform { get; set; } = new List<ItemData1>();

        [JsonProperty("地狱直通车/平台", Order = 4)]
        public List<ItemData2> HellTunnel { get; set; } = new List<ItemData2>();


        #region 监狱数据
        public class ItemData
        {
            [JsonProperty("出生点偏移X", Order = -2)]
            public int spawnTileX { get; set; }

            [JsonProperty("出生点偏移Y", Order = -2)]
            public int spawnTileY { get; set; }

            [JsonProperty("监狱集群宽度", Order = -2)]
            public int Width { get; set; }

            [JsonProperty("监狱集群高度", Order = -1)]
            public int Height { get; set; }

            [JsonProperty("图格ID", Order = 1)]
            public ushort TileID { get; set; }

            [JsonProperty("墙壁ID", Order = 2)]
            public ushort WallID { get; set; }

            [JsonProperty("平台样式", Order = 3)]
            public int PlatformStyle { get; set; }

            [JsonProperty("椅子样式", Order = 4)]
            public int ChairStyle { get; set; }

            [JsonProperty("工作台样式", Order = 5)]
            public int BenchStyle { get; set; }

            [JsonProperty("火把样式", Order = 6)]
            public int TorchStyle { get; set; }

            public ItemData(ushort tileID, ushort wallID, int tileX, int tileY, int width, int height, int platformstyle, int chairstyle, int benchstyle, int torchstyle)
            {
                TileID = tileID;
                WallID = wallID;
                spawnTileX = tileX;
                spawnTileY = tileY;
                Width = width;
                Height = height;
                PlatformStyle = platformstyle;
                ChairStyle = chairstyle;
                BenchStyle = benchstyle;
                TorchStyle = torchstyle;
            }
        }
        #endregion

        #region 世界/左海平台数据
        public class ItemData1
        {
            [JsonProperty("平台图格", Order = 2)]
            public int ID { get; set; }
            [JsonProperty("平台样式", Order = 3)]
            public int Style { get; set; }

            [JsonProperty("平台高度", Order = 5)]
            public int SpawnTileY { get; set; }
            [JsonProperty("清理高度", Order = 6)]
            public int Height { get; set; }

            [JsonProperty("左海平台长度", Order = 7)]
            public int Wide { get; set; }
            [JsonProperty("左海平台高度", Order = 8)]
            public int Height2 { get; set; }
            [JsonProperty("左海平台间隔", Order = 9)]
            public int Interval { get; set; }

            public ItemData1(int id, int style,int tileY, int height, int wide,int height2,int interval )
            {
                ID = id;
                Style = style;
                SpawnTileY = tileY;
                Height = height;
                Wide = wide;
                Height2 = height2;
                Interval = interval;
            }
        }
        #endregion

        #region 地狱直通车与平台数据
        public class ItemData2
        {
            [JsonProperty("方块图格", Order = 1)]
            public ushort ID { get; set; }
            [JsonProperty("绳子图格", Order = 2)]
            public ushort ID2 { get; set; }
            [JsonProperty("平台图格", Order = 3)]
            public ushort PlatformID { get; set; }
            [JsonProperty("平台样式", Order = 3)]
            public int PlatformStyle { get; set; }

            [JsonProperty("直通车偏移X", Order = 4)]
            public int SpawnTileX { get; set; }
            [JsonProperty("直通车偏移Y", Order = 5)]
            public int SpawnTileY { get; set; }

            [JsonProperty("地狱平台深度", Order = 6)]
            public int PlatformY{ get; set; }

            public ItemData2(ushort id, ushort id2, ushort platformID, int platformstyle, int tileX, int tileY, int platformY)
            {
                ID = id;
                ID2 = id2;
                PlatformID = platformID;
                PlatformStyle = platformstyle;
                SpawnTileX = tileX;
                SpawnTileY = tileY;
                PlatformY = platformY;
            }
        }
        #endregion

        #region 预设参数方法
        public void Ints()
        {
            Prison = new List<ItemData>
            {
                new ItemData(30,4,-4,10,36,80,0,0,0,0)
            };

            WorldPlatform = new List<ItemData1>
            {
                new ItemData1(19, 43, -150,35,270,218,30),
            };

            HellTunnel = new List<ItemData2>
            {
                new ItemData2(38, 214, 19, 43, 0, 0,40),
            };
        }
        #endregion

        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "生成基础建设.json");

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
                NewConfig.Write();
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