using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TShockAPI;

namespace AutoTeam
{
    public class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "AutoTeam.json");

        [JsonProperty("开启插件")]
        public bool Enabled { get; set; } = true;

        [JsonProperty("组对应的队伍")]
        public Dictionary<string, string> GroupTeamMap { get; set; } = new Dictionary<string, string>();


        public string GetTeamForGroup(string groupName)
        {
            return this.GroupTeamMap.TryGetValue(groupName, out var team) ? team : GetString("无队伍");
        }

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
                // 创建默认配置
                var defaultConfig = new Configuration
                {
                    Enabled = true,
                    GroupTeamMap = new Dictionary<string, string>
                    {
                        { "default", "red" },
                        { "owner", "红队" }
                    }
                };

                // 保存默认配置到文件
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
}