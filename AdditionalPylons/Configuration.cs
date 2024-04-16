using Newtonsoft.Json;
using System;
using System.IO;
using TShockAPI;

namespace AdditionalPylons
{
    internal class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "无限晶塔.json");

        [JsonProperty("丛林晶塔数量上限")]
        public int JungleTowerLimit = 2;
        [JsonProperty("森林晶塔数量上限")]
        public int ForestTowerLimit = 2;
        [JsonProperty("神圣晶塔数量上限")]
        public int HolyTowerLimit = 2;
        [JsonProperty("洞穴晶塔数量上限")]
        public int CaveTowerLimit = 2;
        [JsonProperty("海洋晶塔数量上限")]
        public int OceanTowerLimit = 2;
        [JsonProperty("沙漠晶塔数量上限")]
        public int DesertTowerLimit = 2;
        [JsonProperty("雪原晶塔数量上限")]
        public int SnowTowerLimit = 2;
        [JsonProperty("蘑菇晶塔数量上限")]
        public int MushroomTowerLimit = 2;
        [JsonProperty("万能晶塔数量上限")]
        public int UniversalTowerLimit = 2;


        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                }
            }
        }

        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
                return new Configuration();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    var cf = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                    return cf;
                }
            }
        }
    }
}