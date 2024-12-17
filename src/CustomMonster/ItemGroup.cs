using Newtonsoft.Json;

namespace CustomMonster;

public class ItemGroup
{
    [JsonProperty(PropertyName = "物品ID")]
    public int ItemID = 0;

    [JsonProperty(PropertyName = "物品数量")]
    public int ItemStack = 0;

    [JsonProperty(PropertyName = "物品前缀")]
    public int ItemPrefix = -1;

    [JsonProperty(PropertyName = "独立掉落")]
    public bool DropItemAlone = false;

    public ItemGroup(int id, int stack)
    {
        this.ItemID = id;
        this.ItemStack = stack;
    }
}
