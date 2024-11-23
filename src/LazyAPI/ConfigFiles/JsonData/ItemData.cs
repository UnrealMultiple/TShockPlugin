using Newtonsoft.Json;
using Terraria;

namespace LazyAPI.ConfigFiles.JsonData;

[JsonObject]
public class ItemData
{
    public int type, stack;
    public byte prefix;
    public bool favorited;
    public static implicit operator ItemData(Item item)
    {
        return new ItemData
        {
            type = item.type,
            stack = item.stack,
            prefix = item.prefix,
            favorited = item.favorited
        };
    }

    public static implicit operator Item(ItemData data)
    {
        var item = new Item();
        item.SetDefaults(data.type);
        item.stack = data.stack;
        item.prefix = data.prefix;
        item.favorited = data.favorited;
        return item;
    }
}