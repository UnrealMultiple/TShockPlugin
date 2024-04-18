using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace ezperm
{
    internal class GroupInfo
    {
        [JsonProperty("组名字")]
        public string Name { get; set; }
        [JsonProperty("添加的权限")]
        public List<string> AddPermissions { get; set; }
        [JsonProperty("删除的权限")]
        public List<string> DelPermissions { get; set; }
    }

    internal class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "ezperm.json");

        public List<GroupInfo> Groups { get; set; }

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
                            AddPermissions = new List<string> { "tshock.world.movenpc","tshock.tp.pylon","tshock.tp.rod","tshock.npc.startdd2","tshock.tp.wormhole","tshock.npc.summonboss","tshock.npc.startinvasion","tshock.world.time.usesundial" },
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
                    var cf = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
                    return cf;
                }
            }
        }
    }
}

