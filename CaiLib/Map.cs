using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Terraria;
using Terraria.Map;

namespace CaiLib
{
    public static class CaiMap
    {
        public static byte[] CreateMapBytes()
        {
            var image = CreateMapImage();
            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            return ms.ToArray();
        }
        public static Image CreateMapImage()
        {
            Image<Rgba32> image = new Image<Rgba32>(Main.maxTilesX, Main.maxTilesY);

            MapHelper.Initialize();
            Main.Map ??= new WorldMap(0, 0);
            for (var x = 0; x < Main.maxTilesX; x++)
            {
                for (var y = 0; y < Main.maxTilesY; y++)
                {
                    var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                    var col = MapHelper.GetMapTileXnaColor(ref tile);
                    image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
                }
            }

            return image;
        }
    }
}
