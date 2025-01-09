using Terraria;
using Terraria.ObjectData;
using TShockAPI;
using static SurfaceBlock.SurfaceBlock;

namespace SurfaceBlock;

internal class Tool
{
    #region 获取世界属性 判断世界大小
    public static int GetWorldSize()
    {
        return Main.maxTilesX == 8400 && Main.maxTilesY == 2400 ? 3 :
            Main.maxTilesX == 6400 && Main.maxTilesY == 1800 ? 2 :
            Main.maxTilesX == 4200 && Main.maxTilesY == 1200 ? 1 : 0;
    }

    //颠倒世界
    public static void RemixWorld(GetDataHandlers.NewProjectileEventArgs e, double num)
    {
        if (e.Position.Y > Main.worldSurface * num)
        {
            Remover(e);
        }
    }
    #endregion

    #region 获取图格回滚大小方法(TS里抄来的)
    public static void GetRollbackSize(int tileX, int tileY, out byte width, out byte length, out int offsetY)
    {
        CheckForTileObjectsAbove(tileY, out var topWidth, out var topLength, out offsetY);
        CheckForTileObjectsBelow(tileY, out var botWidth, out var botLength);

        width = Math.Max((byte) 1, Math.Max(topWidth, botWidth));
        length = Math.Max((byte) 1, (byte) (topLength + botLength));

        void CheckForTileObjectsAbove(int y, out byte objWidth, out byte objLength, out int yOffset)
        {
            objWidth = 0;
            objLength = 0;
            yOffset = 0;

            if (y <= 0)
            {
                return;
            }

            var above = Main.tile[tileX, y - 1];
            if (above.type < TileObjectData._data.Count && TileObjectData._data[above.type] != null)
            {
                var data = TileObjectData._data[above.type];
                objWidth = (byte) data.Width;
                objLength = (byte) data.Height;
                yOffset = -data.Height;
            }
        }

        void CheckForTileObjectsBelow(int y, out byte objWidth, out byte objLength)
        {
            objWidth = 0;
            objLength = 0;

            if (y >= Main.maxTilesY - 1)
            {
                return;
            }

            var below = Main.tile[tileX, y + 1];
            if (below.type < TileObjectData._data.Count && TileObjectData._data[below.type] != null)
            {
                var data = TileObjectData._data[below.type];
                objWidth = (byte) data.Width;
                objLength = (byte) data.Height;
            }
        }
    }
    #endregion
}
