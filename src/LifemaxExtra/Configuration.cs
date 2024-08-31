using Newtonsoft.Json;
using Terraria.ID;
using TShockAPI;

namespace LifemaxExtra;

public class Configuration
{
    public class ItemRaiseInfo
    {
        [JsonProperty("最大提升至")]
        public int Max = 600;

        [JsonProperty("提升数值")]
        public int Raise = 20;
    }

    public static readonly string FilePath = Path.Combine(TShock.SavePath, "LifemaxExtra.json");

    [JsonProperty("最大生命值")]
    public short MaxHP = 1000;

    [JsonProperty("最大法力值")]
    public short MaxMP = 1000;

    [JsonProperty("提高血量物品")]
    public Dictionary<int, ItemRaiseInfo> ItemRaiseHP { get; set; } = new()
    {
        { ItemID.LifeCrystal, new() },
        { ItemID.LifeFruit, new(){ Raise = 5, Max = 700 } }
    };

    [JsonProperty("提高法力物品")]
    public Dictionary<int, ItemRaiseInfo> ItemRaiseMP { get; set; } = new()
    {
        { ItemID.ManaCrystal, new(){ Raise = 20, Max = 700 } }
    };

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