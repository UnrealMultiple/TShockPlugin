using Newtonsoft.Json;
using TShockAPI;

namespace Ezperm;

internal class GroupInfo
{
    [JsonProperty("组名字")]
    public string Name { get; set; } = "";
    [JsonProperty("添加的权限")]
    public List<string> AddPermissions { get; set; } = new();
    [JsonProperty("删除的权限")]
    public List<string> DelPermissions { get; set; } = new();
}

internal class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "ezperm.json");

    public List<GroupInfo> Groups { get; set; } = new();

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
            // If the file doesn't exist, create a default configuration
            var defaultConfig = new Configuration
            {
                Groups = new List<GroupInfo>
                {
                    new GroupInfo
                    {
                        Name = "default",
                        AddPermissions = new List<string> { "tshock.world.movenpc","tshock.world.time.usesundial", "tshock.tp.pylon", "tshock.tp.demonconch", "tshock.tp.magicconch", "tshock.tp.tppotion", "tshock.tp.rod","tshock.tp.wormhole","tshock.npc.startdd2", "tshock.npc.spawnpets", "tshock.npc.summonboss","tshock.npc.startinvasion","tshock.npc.hurttown" },
                        DelPermissions = new List<string> { "tshock.admin" }
                    }
                }
            };

            defaultConfig.Write(path);
            return defaultConfig;
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