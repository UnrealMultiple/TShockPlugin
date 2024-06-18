using Newtonsoft.Json;
using System.Text;
using TShockAPI;

public class TimeRange
{
    public TimeSpan Start { get; set; }
    public TimeSpan Stop { get; set; }
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
        [JsonProperty("踢出玩家断连消息", Order = -9)]
        public string NewProjMessage = "到点了，晚安";
        [JsonProperty("断连豁免玩家", Order = -9)]
        public List<string> PlayersList { get; set; }

        [JsonProperty("禁怪少于人数(设1为关闭禁怪)", Order = -8)]
        public int MaxPlayers { get; set; } = 2;
        [JsonProperty("宵禁时间设置(禁怪/断连)", Order = -7)]
        public TimeRange Time { get; set; } = new TimeRange()
        {
            Start = TimeSpan.FromHours(0),
            Stop = TimeSpan.FromHours(5)
        };

        [JsonProperty("禁止怪物生成表(NpcID)", Order = -6)]
        public HashSet<int> Npcs = new();

        #region 读取与创建配置文件方法
        public Configuration()
        {
            PlayersList = new List<string>();
        }

        public void Write()
        {
            using (var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.Write))
            using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
                sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Configuration Read()
        {
            if (!File.Exists(FilePath))
            {
                new Configuration().Write();
                return new Configuration();
            }
            else
            {
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs))
                    return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd())!;
            }
        }
        #endregion

        #region 豁免名单增删改查方法
        //获取断连豁免名单中的名字
        internal bool Exempt(string Name) => PlayersList.Contains(Name);

        //列出豁免名单
        public string GetList() => JsonConvert.SerializeObject(PlayersList, (Formatting)1);

        //添加豁免名单名字
        public bool Add(string name)
        {
            if (Exempt(name))
            {
                return false;
            }
            PlayersList.Add(name);
            Write();
            return true;
        }

        //移除豁免名单名字
        public bool Del(string name)
        {
            if (Exempt(name))
            {
                PlayersList.Remove(name);
                Write();
                return true;
            }
            return false;
        } 
        #endregion
    }
}