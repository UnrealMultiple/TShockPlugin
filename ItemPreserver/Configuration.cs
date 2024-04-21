using Newtonsoft.Json;
using TShockAPI;

namespace ItemPreserver;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "ItemPreserverConfig.json");

    [JsonProperty("不消耗物品")]
    public HashSet<int> NoConsumItem { get; set; } = new();




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
        {
            var config = new Configuration();
            return config;
        }
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



