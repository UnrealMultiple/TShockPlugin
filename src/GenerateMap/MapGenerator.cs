using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO.Streams;
using Terraria;
using Terraria.IO;
using Terraria.Map;

namespace GenerateMap;

internal static class MapGenerator
{
    private const string BasePath = "GenerateMap";
    private static readonly string MapsPath = Path.Combine(BasePath, "Maps");
    private static readonly string ImagesPath = Path.Combine(BasePath, "Images");
    internal static void Init()
    {
        MapHelper.Initialize();
        Main.mapEnabled = true;
        Main.Map = CreateWorkingMap();
        Main.ActivePlayerFileData = new PlayerFileData { Name = "GenerateMap", _path = Main.GetPlayerPathFromName("GenerateMap", false) };
        Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
        Utils.TryCreatingDirectory(BasePath);
        Utils.TryCreatingDirectory(MapsPath);
        Utils.TryCreatingDirectory(ImagesPath);
    }

    internal static void Dispose()
    {
    }

    private static WorldMap CreateWorkingMap()
    {
        var edge = WorldMap.BlackEdgeWidth;
        return new WorldMap(Main.maxTilesX, Main.maxTilesY)
        {
            _tiles = new MapTile[Main.maxTilesX + edge * 2, Main.maxTilesY + edge * 2]
        };
    }

    private static void LightUpWholeMap()
    {
        Main.Map = CreateWorkingMap();
        var edge = WorldMap.BlackEdgeWidth;
        var width = Main.Map._tiles.GetLength(0);
        var height = Main.Map._tiles.GetLength(1);
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);

                // 1.4.5+ on different runtimes may read either raw or edge-offset coordinates during save.
                if ((uint) x < (uint) width && (uint) y < (uint) height)
                {
                    Main.Map._tiles[x, y] = tile;
                }

                var rawX = x + edge;
                var rawY = y + edge;
                if ((uint) rawX < (uint) width && (uint) rawY < (uint) height)
                {
                    Main.Map._tiles[rawX, rawY] = tile;
                }
            }
        }
    }

    private static Image CreateMapImg()
    {
        Image<Rgba32> image = new (Main.maxTilesX, Main.maxTilesY);
        LightUpWholeMap();
        var edge = WorldMap.BlackEdgeWidth;
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = Main.Map._tiles[x + edge, y + edge];
                var col = MapHelper.GetMapTileXnaColor(tile);
                image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
            }
        }

        return image;
    }

    internal static byte[] CreatMapImgBytes()
    {
        var image = CreateMapImg();
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }
    
    internal static byte[] CreatMapFileBytes()
    {
        return File.ReadAllBytes(GetWorldFilePath());
    }

    internal static string SaveMapImg(string fileName)
    {
        var image = CreateMapImg();
        var path = Path.Combine(ImagesPath, fileName);
        image.SaveAsPng(path);
        return path;
    }

    private static string GetWorldFilePath()
    {
        var worldFilePath = Main.worldPathName;
        if (string.IsNullOrWhiteSpace(worldFilePath) || !File.Exists(worldFilePath))
        {
            throw new FileNotFoundException("World file not found.", worldFilePath);
        }

        return worldFilePath;
    }

    internal static string SaveMapFile()
    {
        var worldPath = GetWorldFilePath();
        var worldName = Path.GetFileNameWithoutExtension(worldPath);
        var ext = Path.GetExtension(worldPath);
        if (string.IsNullOrWhiteSpace(ext))
        {
            ext = ".wld";
        }

        var fileName = $"{worldName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}{ext}";
        var path = Path.Combine(MapsPath, fileName);
        File.Copy(worldPath, path, true);
        return path;
    }
}
