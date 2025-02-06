using Terraria;

namespace VeinMiner;

static class Utils
{
    public static Item GetItemFromTile(int x, int y)
    {
        WorldGen.KillTile_GetItemDrops(x, y, Main.tile[x, y], out var id, out var stack, out _, out _);
        Item item = new();
        item.SetDefaults(id);
        item.stack = stack;
        return item;
    }
}