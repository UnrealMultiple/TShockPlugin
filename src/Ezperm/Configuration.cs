using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace Ezperm;

public class Configuration
{
    public const string Path = "tshock/ezperm.json";

    public static Configuration Instance = new();

    public void Write()
    {
        var value = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(Path, value);
    }

    public static void Read()
    {
        if (!File.Exists(Path))
        {
            Instance = new Configuration();
            Instance.Write();
        }
        else
        {
            try
            {
                var content = File.ReadAllText(Path);
                var deserializedInstance = JsonConvert.DeserializeObject<Configuration>(content) ?? throw new Exception("配置内容为空或无法解析");
                Instance = deserializedInstance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载配置文件失败: {ex.Message}");
            }
        }
    }

    public class GroupInfo
    {
        [JsonProperty("组名字")]
        public string Name { get; set; } = "";

        [JsonProperty("父组")]
        public string Parent { get; set; } = "";

        [JsonProperty("添加的权限")]
        public List<string> AddPermissions { get; set; } = new List<string>();

        [JsonProperty("删除的权限")]
        public List<string> DelPermissions { get; set; } = new List<string>();
    }

    [JsonProperty("组列表")]
    public List<GroupInfo> Groups { get; set; } = new List<GroupInfo>();

    public Configuration()
    {
        this.Groups = new List<GroupInfo>
        {
            new GroupInfo
            {
                Name = "default",
                Parent = "guest",
                AddPermissions = new List<string>
                {
                    "tshock.world.movenpc",
                    "tshock.world.worldupgrades",
                    "tshock.specialeffects.*",
                    "tshock.world.time.usemoondial",
                    "tshock.world.time.usesundial",
                    "tshock.tp.pylon",
                    "tshock.tp.demonconch",
                    "tshock.tp.magicconch",
                    "tshock.tp.tppotion",
                    "tshock.tp.rod",
                    "tshock.tp.wormhole",
                    "tshock.npc.startdd2",
                    "tshock.npc.spawnpets",
                    "tshock.npc.summonboss",
                    "tshock.npc.startinvasion",
                    "tshock.npc.hurttown"
                },
                DelPermissions = new List<string> { "tshock.admin" }
            }
        };
    }
}