using LazyAPI.Attributes;
using LazyAPI.ConfigFiles;
using Newtonsoft.Json;
using Terraria.ID;

namespace LifemaxExtra;

[Config]
public class Configuration : JsonConfigBase<Configuration>
{
    protected override string Filename => "LifemaxExtra";
    public class ItemRaiseInfo
    {
        [JsonProperty("最大提升至")]
        [LocalizedPropertyName(CultureType.Chinese, "最大提升至")]
        [LocalizedPropertyName(CultureType.English, "Max")]
        public int Max = 600;

        [JsonProperty("提升数值")]
        [LocalizedPropertyName(CultureType.Chinese, "提升数值")]
        [LocalizedPropertyName(CultureType.English, "Raise")]
        public int Raise = 20;
    }

    [JsonProperty("最大生命值")]
    [LocalizedPropertyName(CultureType.Chinese, "最大生命值")]
    [LocalizedPropertyName(CultureType.English, "MaxHP")]
    public short MaxHP = 1000;

    [JsonProperty("最大法力值")]
    [LocalizedPropertyName(CultureType.Chinese, "最大法力值")]
    [LocalizedPropertyName(CultureType.English, "MaxMP")]
    public short MaxMP = 400;

    [JsonProperty("提高血量物品")]
    [LocalizedPropertyName(CultureType.Chinese, "提高血量物品")]
    [LocalizedPropertyName(CultureType.English, "ItemRaiseHP")]
    public Dictionary<int, ItemRaiseInfo> ItemRaiseHP { get; set; } = new()
    {
        { ItemID.LifeCrystal, new() },
        { ItemID.LifeFruit, new(){ Raise = 5, Max = 1000 } }
    };

    [JsonProperty("提高法力物品")]
    [LocalizedPropertyName(CultureType.Chinese, "提高法力物品")]
    [LocalizedPropertyName(CultureType.English, "ItemRaiseMP")]
    public Dictionary<int, ItemRaiseInfo> ItemRaiseMP { get; set; } = new()
    {
        { ItemID.ManaCrystal, new(){ Raise = 20, Max = 400 } }
    };

    protected override void SetDefault()
    {
        this.MaxHP = 1000;
        this.MaxMP = 400;

        this.ItemRaiseHP = new Dictionary<int, ItemRaiseInfo>
        {
            { ItemID.LifeCrystal, new ItemRaiseInfo() },
            { ItemID.LifeFruit, new ItemRaiseInfo { Raise = 5, Max = 100 } }
        };

        this.ItemRaiseMP = new Dictionary<int, ItemRaiseInfo>
        {
            { ItemID.ManaCrystal, new ItemRaiseInfo { Raise = 20, Max = 400 } }
        };
    }
}