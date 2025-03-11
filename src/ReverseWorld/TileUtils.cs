using Terraria;

namespace ReverseWorld;

internal class TileUtils
{
    public static void SetTile(int x, int y, int x2, int y2, int tileType)
    {
        for (var i = x; i <= x2; i++)
        {
            for (var j = y; j <= y2; j++)
            {
                SetTile(i, j, tileType);
            }
        }
    }

    public static void SetTile(TileSection sec, int tileType)
    {
        SetTile(sec.Left, sec.Top, sec.Right, sec.Bottom, tileType);
    }

    public static void SetTile(int i, int j, int tileType)
    {
        var tile = Main.tile[i, j];
        switch (tileType)
        {
            case -4:
            case -3:
            case -2:
            case -1:
                tile.active(false);
                tile.liquidType(tileType == -1 ? 0 : tileType + 4);
                tile.liquid = byte.MaxValue;
                tile.type = 0;
                break;
            default:
                if (tileType >= 0 && tileType < Main.tileFrameImportant.Length && Main.tileFrameImportant[tileType])
                {
                    WorldGen.PlaceTile(i, j, tileType, false, false, -1, 0);
                }
                else
                {
                    tile.active(true);
                    tile.frameX = -1;
                    tile.frameY = -1;
                    tile.liquidType(0);
                    tile.liquid = 0;
                    tile.slope(0);
                    tile.type = (ushort)tileType;
                }
                break;
        }
    }
}

