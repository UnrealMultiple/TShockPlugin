using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Terraria.Map;
using Terraria;
using System.Reflection;

namespace CaiLib
{
    public static class CaiMap
    {
        private static Assembly LoadLib()
        {
            string resourceName = "CaiLib.SixLabors.ImageSharp.dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        public static Image CreateMapImage()
        {
            LoadLib();
            Image<Rgba32> image = new Image<Rgba32>(Main.maxTilesX, Main.maxTilesY);

            MapHelper.Initialize();
            if (Main.Map == null)
            {
                Main.Map = new WorldMap(0, 0);
            }
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
