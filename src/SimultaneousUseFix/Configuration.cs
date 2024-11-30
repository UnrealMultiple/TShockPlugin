using Newtonsoft.Json;
using TShockAPI;

namespace fixbugpe;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "解决卡双锤卡星旋机枪之类的问题.json");
    private static readonly int[] DefaultExemptItemList = { 205, 206, 207, 1128 };

    [JsonProperty("免检测物品列表")]
    public int[] ExemptItemList { get; set; }

    [JsonProperty("是否杀死")]
    public bool KillPlayerOnUse { get; set; } = true;

    [JsonProperty("是否上buff")]
    public bool ApplyBuffOnUse { get; set; } = false;

    [JsonProperty("buff时长(s)")]
    public int Bufftime { get; set; } = 60;

    [JsonProperty("上什么buff")]
    public int[] BuffTypes { get; set; } = { 163, 149, 23, 164 };

    [JsonProperty("是否踢出")]
    public bool KickPlayerOnUse { get; set; } = false;

    public Configuration()
    {
        this.ExemptItemList = DefaultExemptItemList;
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