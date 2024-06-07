using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Terraria;
using Terraria.Map;
using Image = SixLabors.ImageSharp.Image;
//using System.Drawing;

namespace CaiBotPlugin
{
    public static class MapGenerator
    {
        public static Image Create()
        {
            Image<Rgba32> image = new Image<Rgba32>(Main.maxTilesX, Main.maxTilesY);

            MapHelper.Initialize();
            Main.Map = new WorldMap(0, 0);
            for (var x = 0; x < Main.maxTilesX; x++)
            {
                for (var y = 0; y < Main.maxTilesY; y++)
                {
                    //Console.WriteLine($"{x}, {y}");
                    var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                    var col = MapHelper.GetMapTileXnaColor(ref tile);
                    image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
                    //Console.WriteLine($"{x}, {y} - {col.R}, {col.G}, {col.B}, {col.A}");
                }
            }

            return image;
        }
    }
}