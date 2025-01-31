using Newtonsoft.Json;
using Terraria.ID;
using TShockAPI;
using TShockAPI.Hooks;

namespace VeinMiner;

public class Config
{
    public static void Load(ReloadEventArgs? args = null)
    {
        try
        {
            FileTools.CreateIfNot(Path.Combine(TShock.SavePath, "VeinMiner.json"), JsonConvert.SerializeObject(new Config
            {
                Exchange = new List<Exchange>(),
                TargetTile = new List<int>
                {
                    TileID.Copper, TileID.Iron,TileID.Silver,TileID.Gold, TileID.Tin, TileID.Lead ,TileID.Tungsten ,
                    TileID.Platinum, TileID.Meteorite,TileID.Demonite, TileID.Crimtane, TileID.Obsidian, TileID.Hellstone,
                    TileID.Cobalt, TileID.Palladium, TileID.Mythril, TileID.Adamantite, TileID.Titanium,TileID.LunarOre,
                    TileID.DesertFossil, TileID.ExposedGems, TileID.Obsidian, TileID.Sapphire, TileID.Ruby, TileID.Emerald, 
                    TileID.Topaz, TileID.Amethyst, TileID.Diamond 
                },
                IgnoreAboveTile = new List<int>()
            }, Formatting.Indented));

            VeinMiner.Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(TShock.SavePath, "VeinMiner.json")))!;
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
    
    [JsonProperty("放入背包")]
    public bool PutIntoInventory { get; set; } = true;

    [JsonProperty("矿石物块ID")]
    public List<int> TargetTile { get; set; } = new();

    [JsonProperty("忽略挖掘表面方块ID")]
    public List<int> IgnoreAboveTile { get; set; } = new();

    [JsonProperty("奖励规则")]
    public List<Exchange> Exchange { get; set; } = new();
}