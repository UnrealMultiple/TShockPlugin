using Terraria;

namespace EssentialsPlus.Extensions;

public static class ITileExtensions
{
    /// <summary>
    /// Determines whether a tile is empty.
    /// </summary>
    /// <returns></returns>
    public static bool IsEmpty(this ITile tile)
    {
        return tile == null || ((!tile.active() || !Main.tileSolid[tile.type]) && tile.liquid == 0);
    }
}