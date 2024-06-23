using System.Drawing;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using TShockAPI;

namespace WorldModify
{
    internal class PaintTool
    {
        public static void Manage(TSPlayer op)
        {
            string text = Path.Combine(Utils.SaveDir, "1.png");
            if (!File.Exists(text))
            {
                op.SendErrorMessage(text + " 文件不存在！");
                return;
            }
            Bitmap bitmap = new Bitmap(text);
            if (bitmap.Width > Main.maxTilesX)
            {
                op.SendErrorMessage($"图片宽度太大了，不应超过{Main.maxTilesX}px");
                bitmap.Dispose();
                return;
            }
            if (bitmap.Height > Main.maxTilesY)
            {
                op.SendErrorMessage($"图片的高度太大了，不应超过{Main.maxTilesY}px");
                bitmap.Dispose();
                return;
            }
            int num = op.TileX - bitmap.Width / 2;
            int num2 = op.TileY - bitmap.Height;
            if (num < 0)
            {
                op.SendErrorMessage("位置太靠左了，请往右移动一些");
                bitmap.Dispose();
                return;
            }
            if (num2 < 0)
            {
                op.SendErrorMessage("位置太靠上了，请往下移动一些");
                bitmap.Dispose();
                return;
            }
            if (num + bitmap.Width / 2 > Main.maxTilesX)
            {
                op.SendErrorMessage("位置太靠右了，请往左移动一些");
                bitmap.Dispose();
                return;
            }
            if (num2 + bitmap.Height > Main.maxTilesY)
            {
                op.SendErrorMessage("位置太靠下了，请往上移动一些");
                bitmap.Dispose();
                return;
            }
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    int num3 = num + j;
                    int num4 = num2 + i;
                    Main.tile[num3, num4] = (ITile)(object)TileFromColor(ConvertToXnaColor(bitmap.GetPixel(j, i)));
                    NetMessage.SendTileSquare(-1, num3, num4, (TileChangeType)0);
                }
            }
            bitmap.Dispose();
            op.SendSuccessMessage("像素打印完成！");
        }

        private static Tile TileFromColor(Microsoft.Xna.Framework.Color color)
        {
            Tile val = new Tile();
            int brickFromColor = GetBrickFromColor(color);
            if (brickFromColor != -1)
            {
                val.type = (ushort)brickFromColor;
                val.active(true);
            }
            return val;
        }

        private static Microsoft.Xna.Framework.Color ConvertToXnaColor(System.Drawing.Color color)
        {
            return new Microsoft.Xna.Framework.Color(
                color.R,
                color.G,
                color.B,
                color.A
            );
        }

        public static int GetBrickFromColor(Microsoft.Xna.Framework.Color color)
        {
            for (int i = 0; i < MapHelper.colorLookup.Length; i++)
            {
                Microsoft.Xna.Framework.Color val = MapHelper.colorLookup[i];
                if (color.R == val.R && color.G == val.G && color.B == val.B)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
