using Newtonsoft.Json;

namespace BedSet;

internal class Config
{
    public class BedSpawn
    {

        [JsonProperty("X坐标")]
        public int X { get; set; }

        [JsonProperty("Y坐标")]
        public int Y { get; set; }
    }

    private static readonly string PATH = Path.Combine(TShockAPI.TShock.SavePath, "Bed.json");

    [JsonProperty("重生点")]
    public Dictionary<string, BedSpawn> SpawnOption { get; set; } = new();

    public static Config Read()
    {
        if (File.Exists(PATH))
        {
            return JsonConvert.DeserializeObject<Config>(File.ReadAllText(PATH)) ?? new();
        }
        else
        {
            var obj = new Config();
            obj.Write();
            return obj;
        }
    }

    public void Write()
    {
        File.WriteAllText(PATH, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}