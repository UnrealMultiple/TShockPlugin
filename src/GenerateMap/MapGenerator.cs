using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO.Streams;
using Terraria;
using Terraria.IO;
using Terraria.Map;

namespace GenerateMap;

internal static class MapGenerator
{
    private const string MapsPath = @"GenerateMap";
    private const string ImagesPath = @"GenerateMap\Images";
    internal static void Init()
    {
        MapHelper.Initialize();
        Main.mapEnabled = true;
        Main.Map = CreateWorkingMap();
        Main.ActivePlayerFileData = new PlayerFileData { Name = "GenerateMap", _path = Main.GetPlayerPathFromName("GenerateMap", false) };
        Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
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
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                Main.Map._tiles[x + edge, y + edge] = tile;
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
        return File.ReadAllBytes(CreatMapFile());
    }
    
    internal static byte[] CreatMapFileBytes()
    {
        var image = CreateMapImg();
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();

    }

    internal static string SaveMapImg(string fileName)
    {
        var image = CreateMapImg();
        var path = Path.Combine(ImagesPath, fileName);
        image.SaveAsPng(path);
        return path;
    }

    private static string CreatMapFile()
    {
        LightUpWholeMap();
        MapHelper.SaveMap();
        var playerPath = Main.playerPathName[..^4] + Path.DirectorySeparatorChar;
        var mapFileName = !Main.ActiveWorldFileData.UseGuidAsMapName ? Main.worldID + ".map" : Main.ActiveWorldFileData.UniqueId + ".map";
        var mapFilePath = Path.Combine(playerPath, mapFileName);
        return mapFilePath;
    }

    internal static string SaveMapFile()
    {
        var mapPath = CreatMapFile();
        var path = Path.Combine(ImagesPath, Path.GetFileName(mapPath));
        File.Copy(mapPath, path);
        return path;
    }
}
