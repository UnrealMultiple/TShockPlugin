using Newtonsoft.Json;
using TShockAPI;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "EndureBoost.json");
    [JsonProperty("猪猪储钱罐")]
    public bool bank = true;
    [JsonProperty("保险箱")]
    public bool bank2 = true;
    [JsonProperty("护卫熔炉")]
    public bool bank3 = false;
    [JsonProperty("虚空宝藏袋")]
    public bool bank4 = false;
    [JsonProperty("持续时间(s)")]
    public int duration = 3600;

    public class Potion
    {
        [JsonProperty("药水id")]
        public int[] ItemID { get; set; }
        [JsonProperty("药水数量")]
        public int RequiredStack { get; set; }
    }

    public class Station
    {
        [JsonProperty("物品id")]
        public int[] Type { get; set; }
        [JsonProperty("物品数量")]
        public int RequiredStack { get; set; }
        [JsonProperty("给buff的id")]
        public int[] BuffType { get; set; }
    }
    [JsonProperty("药水")]
    public List<Potion> Potions { get; set; } = new List<Potion>();
    [JsonProperty("其他物品")]
    public List<Station> Stations { get; set; } = new List<Station>();

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
            return new Configuration();
        }

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);
        return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd()) ?? new();
    }
}