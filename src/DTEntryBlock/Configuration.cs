using Newtonsoft.Json;
using TShockAPI;

namespace DTEntryBlock;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "阻止进入地牢或神庙.json");
    [JsonProperty("阻止玩家进入地牢总开关")]
    public bool PreventPlayersEnterDungeon { get; set; } = true;

    [JsonProperty("击杀未击败骷髅王进入地牢的玩家")]
    public bool KillPlayersEnterDungeonForUnkilledSkullKing { get; set; } = true;

    [JsonProperty("传送未击败骷髅王进入地牢的玩家")]
    public bool TeleportPlayersEnterDungeonForUnkilledSkullKing { get; set; } = true;

    [JsonProperty("阻止玩家进入神庙总开关")]
    public bool PreventPlayersEnterTemple { get; set; } = true;

    [JsonProperty("击杀未击败世纪之花进入神庙的玩家")]
    public bool KillPlayersEnterTempleForUnkilledPlantBoss { get; set; } = true;

    [JsonProperty("传送未击败世纪之花进入神庙的玩家")]
    public bool TeleportPlayersEnterTempleForUnkilledPlantBoss { get; set; } = true;


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

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            using (var sr = new StreamReader(fs))
            {
                var cf = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd())!;
                return cf;
            }
        }
    }
}