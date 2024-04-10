using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using TShockAPI;

namespace LifemaxExtra
{
    public class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "LifemaxExtra.json");
        [JsonProperty("使用生命水晶最高可提升至")]
        public int LifeCrystalMaxLife = 400;
        [JsonProperty("使用生命果最高可提升至")]
        public int LifeFruitMaxLife = 500;

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
