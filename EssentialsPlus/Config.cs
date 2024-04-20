using System.IO;
using Newtonsoft.Json;

namespace EssentialsPlus
{
    public class Config
    {
        [JsonProperty("Pvp禁用命令")]
        public string[] DisabledCommandsInPvp = new string[]
        {
            "eback"
        };

        [JsonProperty("回退位置历史记录")]
        public int BackPositionHistory = 10;

        [JsonProperty("MySql主机")]
        public string MySqlHost = "";

        [JsonProperty("MySql数据库名称")]
        public string MySqlDbName = "";

        [JsonProperty("MySql用户名")]
        public string MySqlUsername = "";

        [JsonProperty("MySql密码")]
        public string MySqlPassword = "";

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : new Config();
        }
    }
}

