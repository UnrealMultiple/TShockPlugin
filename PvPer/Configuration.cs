using Newtonsoft.Json;
using System.Text;
using TShockAPI;

namespace PvPer
{
    public class Configuration
    {
        [JsonProperty("插件权限名")]
        public string README1 = "pvper.use / pvper.admin";
        [JsonProperty("竞技场边界说明")]
        public string README2 = "/pvp set 3 4 要比玩家传送坐标高或低3格设置";
        [JsonProperty("竞技场边界说明2")]
        public string README3 = "拉取范围：会从玩家冲出竞技场方向,拉回到竞技场中心的指定反向位置（当为负数则是正向位置）,关闭杀死玩家选项后默认开启扣血";
        [JsonProperty("拉回竞技场")]
        public bool PullArena = true;
        [JsonProperty("拉取范围")]
        public int PullRange = -20;
        [JsonProperty("离开竞技场杀死玩家")]
        public bool KillPlayer = false;
        [JsonProperty("离场扣血")]
        public int SlapPlayer = 20;

        [JsonProperty("邀请者传送坐标.X")]
        public int Player1PositionX = 0;
        [JsonProperty("邀请者传送坐标.Y")]
        public int Player1PositionY = 0;
        [JsonProperty("受邀者传送坐标.X")]
        public int Player2PositionX = 0;
        [JsonProperty("受邀者传送坐标.Y")]
        public int Player2PositionY = 0;
        [JsonProperty("竞技场左上角坐标.X")]
        public int ArenaPosX1 = 0;
        [JsonProperty("竞技场左上角坐标.Y")]
        public int ArenaPosY1 = 0;
        [JsonProperty("竞技场右下角坐标.X")]
        public int ArenaPosX2 = 0;
        [JsonProperty("竞技场右下角坐标.Y")]
        public int ArenaPosY2 = 0;

        public static readonly string FilePath = Path.Combine(TShock.SavePath + "/决斗系统.json");

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
                var defaultConfig = new Configuration();
                defaultConfig.Write(path);
                return defaultConfig;
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
    }
}