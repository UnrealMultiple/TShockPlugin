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

        [JsonProperty("启用状态")]
        public bool Enabled { get; set; } = true;

        [JsonProperty("是否断开连接")]
        public bool DiscPlayers { get; set; } = false;

        [JsonProperty("消息")]
        public string Message { get; set; } = "别肝了，休息一会吧！大半夜推进度这河里吗？\n当前为宵禁时间（0点至5点），禁止游戏";

        [JsonProperty("时间设置")]
        public TimeRange Time { get; set; } = new TimeRange { Start = TimeSpan.FromHours(0), End = TimeSpan.FromHours(5) };

        [JsonProperty("豁免玩家")]
        public List<string> ExemptPlayers { get; set; } = new List<string>();

        [JsonProperty("阻止怪物生成表")]
        public HashSet<int> Npcs { get; set; } = new() { 1, 2, 4, 13, 14, 15, 37, 50, 68, 73, 81, 87, 88, 89, 90, 91, 92, 98, 99, 100, 113, 114, 120, 125, 126, 127, 128, 129, 130, 131, 134, 135, 136, 180, 182, 183, 184, 186, 187, 188, 189, 190, 191, 192, 194, 200, 210, 211, 222, 223, 224, 225, 244, 245, 250, 251, 253, 262, 266, 302, 303, 304, 325, 326, 327, 328, 329, 330, 331, 332, 333, 334, 335, 336, 337, 338, 339, 340, 341, 342, 344, 345, 346, 347, 348, 349, 350, 351, 352, 370, 381, 382, 383, 384, 385, 386, 387, 388, 389, 390, 391, 392, 393, 394, 395, 396, 397, 398, 399, 422, 430, 431, 432, 433, 435, 436, 438, 439, 440, 460, 461, 462, 463, 464, 465, 466, 467, 468, 469, 473, 474, 475, 476, 477, 478, 479, 482, 489, 490, 491, 492, 493, 507, 517, 548, 549, 551, 564, 565, 571, 576, 577, 586, 587, 618, 619, 620, 621, 622, 623, 624, 636, 657, 661, 664, 668 };

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