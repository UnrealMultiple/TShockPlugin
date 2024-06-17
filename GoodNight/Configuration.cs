using Newtonsoft.Json;
using System.Text;
using TShockAPI;

public struct TimeRange
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
}

namespace Goodnight
{
    internal class Configuration
    {
        public static readonly string FilePath = Path.Combine(TShock.SavePath, "宵禁.json");

        [JsonProperty("是否关闭宵禁", Order = -10)]
        public bool Enabled { get; set; } = true;

        [JsonProperty("宵禁是否断连", Order = -9)]
        public bool DiscPlayers = false;
        [JsonProperty("玩家进服拦截消息", Order = -9)]
        public string JoinMessage = "当前为宵禁时间，无法加入游戏。";
        [JsonProperty("玩家攻击断连消息", Order = -9)]
        public string NewProjMessage = "到点了，晚安";
        [JsonProperty("断连豁免玩家", Order = -9)]
        public List<string> ExemptPlayers = new List<string>();

        [JsonProperty("禁怪少于人数(设1为关闭禁怪)", Order = -8)]
        public int MaxPlayers { get; set; } = 2;
        [JsonProperty("宵禁时间设置(禁怪/断连)", Order = -7)]
        public TimeRange Time { get; set; } = new TimeRange { Start = TimeSpan.FromHours(0), End = TimeSpan.FromHours(5) };
        [JsonProperty("禁止怪物生成表(NpcID)", Order = -6)]
        public HashSet<int> Npcs = new();

        public Configuration()
        {
            Npcs = new HashSet<int>() { 4, 13, 14, 15, 35, 36, 37, 50, 113, 114, 125, 126, 127, 128, 129, 130, 131, 134, 135, 136, 222, 245, 246, 247, 248, 249, 262, 266, 370, 396, 397, 398, 400, 439, 440, 422, 493, 507, 517, 636, 657, 668 };
        }

        #region 读取与创建配置文件方法
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                var str = JsonConvert.SerializeObject(this, Formatting.Indented);
                sw.Write(str);
            }
        }

        public static Configuration Read(string path)
        {
            if (!File.Exists(path))
            {
                var c = new Configuration();
                c.Write(path);
                return c;
            }
            else
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                {
                    var json = sr.ReadToEnd();
                    var cf = JsonConvert.DeserializeObject<Configuration>(json);
                    return cf!;
                }
            }
        }
        #endregion
    }
}