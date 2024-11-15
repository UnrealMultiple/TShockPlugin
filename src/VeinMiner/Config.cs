using Newtonsoft.Json;
using TShockAPI;
using TShockAPI.Hooks;

namespace VeinMiner;

public class Config
{
    public static void Load(ReloadEventArgs? args = null)
    {
        try
        {
            FileTools.CreateIfNot(Path.Combine(TShock.SavePath, "VeinMiner.json"), JsonConvert.SerializeObject(new Config()
            {
                Exchange = new()
                {
                    new()
                    {
                        //OnlyGiveItem = true,
                        //MinSize = 10,
                        //Type = 169,
                        //Item = new() { { 953, 1 }, { 2425, 5 } }
                    }
                },
                Tile = new() { 7, 166, 6, 167, 9, 168, 8, 169, 37, 22, 204, 56, 58, 107, 221, 108, 222, 111, 223, 211, 408, 123, 224, 404, 178, 63, 64, 65, 66, 67, 68 },
                NotMine = new() { 21, 26, 88 }
            }, Formatting.Indented));

            VeinMiner.Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(TShock.SavePath, "VeinMiner.json")));
            if (args != null)
            {
                TShock.Log.ConsoleInfo(GetString("<VeinMiner> 配置已重新加载。"));
            }
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.Message);
            TShock.Log.ConsoleError(GetString("<VeinMiner> 读取配置文件失败。"));
        }
    }

    [JsonProperty("启用")]
    public bool Enable { get; set; } = true;

    [JsonProperty("广播")]
    public bool Broadcast { get; set; } = true;

    [JsonProperty("放入背包")]
    public bool PutInInventory { get; set; } = true;

    [JsonProperty("矿石物块ID")]
    public List<int> Tile { get; set; } = new();

    [JsonProperty("忽略挖掘表面方块ID")]
    public List<int> NotMine { get; set; } = new();

    [JsonProperty("奖励规则")]
    public List<Exchange> Exchange { get; set; } = new();
}

public struct Exchange
{
    [JsonProperty("仅给予物品")]
    public bool OnlyGiveItem;

    [JsonProperty("最小尺寸")]
    public int MinSize;

    [JsonProperty("矿石物块ID")]
    public int Type;

    [JsonProperty("奖励物品")]
    public Dictionary<int, int> Item;
}