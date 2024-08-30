using Newtonsoft.Json;
using TShockAPI;

namespace UnseenInventory;

public class Configuration
{
    public static readonly string FilePath = Path.Combine(TShock.SavePath, "允许无法获取的物品列表.json");

    [JsonProperty("允许无法获取的物品id")]
    public int[] AllowList = new int[] { 4722 };
    [JsonProperty("参考列表")]
    public Dictionary<int, string> Items = new Dictionary<int, string>
    {
    { 2772, "星旋斧" },
    { 2773, "星旋链锯" },
    { 2775, "星旋锤" },
    { 2777, "星云斧" },
    { 2778, "星云链锯" },
    { 2780, "星云锤" },
    { 2782, "耀斑斧" },
    { 2783, "耀斑链锯" },
    { 2785, "耀斑锤" },
    { 2881, "相位扭曲弹射器" },
    { 3462, "星尘斧" },
    { 3463, "星尘链锯" },
    { 3465, "星尘镐" },
    { 3847, "食人魔面具" },
    { 3848, "哥布林面具" },
    { 3849, "哥布林炸弹帽" },
    { 3850, "埃特尼亚标枪" },
    { 3851, "小妖魔雷管背包" },
    { 3861, "食人魔宝藏袋" },
    { 3862, "黑暗魔法师宝藏袋" },
    { 3978, "仅颜色染料" },
    { 4010, "苹果派切片" },
    { 4058, "骷髅头弓" },
    { 4722, "第一分型" },
    { 5013, "入睡图标" },
    };

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

        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var sr = new StreamReader(fs);
        return JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd()) ?? new();
    }
}