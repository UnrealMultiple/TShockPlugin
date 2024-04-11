using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TShockAPI;

namespace SFactions
{
    public class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "铺桥配置.json");
        [JsonProperty("允许快速铺路方块id")]
        public int[] AllowedTileIDs { get; set; } = { 19, 380, 427, 435, 436, 437, 438, 439 };
        [JsonProperty("一次性最长铺多少格")]
        public int MaxPlaceLength { get; set; } = 256;

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
