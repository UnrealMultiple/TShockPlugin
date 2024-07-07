using Newtonsoft.Json;
using TShockAPI;

namespace Plugin
{
    internal class Configuration
    {

        [JsonProperty("开服自动建监狱：/rm 重置", Order = -10)]
        public bool Enabled { get; set; } = true;

        [JsonProperty("出生点集群偏移值X", Order = -2)]
        public int spawnTileX { get; set; } = 0;

        [JsonProperty("出生点集群偏移值Y", Order = -2)]
        public int spawnTileY { get; set; } = 10;

        [JsonProperty("监狱集群宽度", Order = -2)]
        public int width { get; set; } = 36;

        [JsonProperty("监狱集群高度", Order = -1)]
        public int height { get; set; } = 80;

        [JsonProperty("图格ID", Order = 1)]
        public ushort TileID { get; set; } = 30;

        [JsonProperty("墙壁ID", Order = 2)]
        public ushort WallID { get; set; } = 4;

        [JsonProperty("平台样式", Order = 3)]
        public int PlatformStyle { get; set; } = 0;

        [JsonProperty("椅子样式", Order = 4)]
        public int ChairStyle { get; set; } = 0;

        [JsonProperty("工作台样式", Order = 5)]
        public int BenchStyle { get; set; } = 0;

        [JsonProperty("火把样式", Order = 6)]
        public int TorchStyle { get; set; } = 0;



        #region 读取与创建配置文件方法
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "出生点监狱.json");

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