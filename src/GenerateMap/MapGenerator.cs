using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO.Streams;
using Terraria;
using Terraria.IO;
using Terraria.Map;

namespace GenerateMap;

public static class MapGenerator
{
    private const string BasePath = "GenerateMap";
    private static readonly string MapsPath = Path.Combine(BasePath, "Maps");
    private static readonly string ImagesPath = Path.Combine(BasePath, "Images");
    private const int Edge = WorldMap.BlackEdgeWidth;

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

    private static WorldMap CreateWorkingMap()
    {
        return new WorldMap(Main.maxTilesX, Main.maxTilesY) { _tiles = new MapTile[Main.maxTilesX + (Edge * 2), Main.maxTilesY + (Edge * 2)] };
    }

    private static void LightUpWholeMap()
    {
        Main.Map = CreateWorkingMap();
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

                var rawX = x + Edge;
                var rawY = y + Edge;
                if ((uint) rawX < (uint) width && (uint) rawY < (uint) height)
                {
                    Main.Map._tiles[rawX, rawY] = tile;
                }
            }
        }
    }

    private static Image<Rgba32> CreateMapImg()
    {
        Image<Rgba32> image = new (Main.maxTilesX, Main.maxTilesY);
        LightUpWholeMap();
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = Main.Map._tiles[x + Edge, y + Edge];
                var col = MapHelper.GetMapTileXnaColor(tile);
                image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
            }
        }

        return image;
    }

    public static byte[] CreatMapImgBytes()
    {
        var image = CreateMapImg();
        using var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return stream.ToArray();
    }
    
    public class MapFile(byte[] file, string name)
    {
        public readonly byte[] File = file;
        public readonly string Name = name;
    }

    public static MapFile CreatMapFile()
    {
        var mapFilePath = CreateMapFile();
        var mapFile = new MapFile(File.ReadAllBytes(mapFilePath), Path.GetFileName(mapFilePath));
        return mapFile;
    }

    internal static string SaveMapImg(string fileName)
    {
        var image = CreateMapImg();
        var path = Path.Combine(ImagesPath, fileName);
        image.SaveAsPng(path);
        return path;
    }

    private static string CreateMapFile()
    {
        LightUpWholeMap();
        MapHelper.SaveMap();

        var playerPath = Main.playerPathName[..^4] + Path.DirectorySeparatorChar;
        var mapFileName = !Main.ActiveWorldFileData.UseGuidAsMapName
            ? Main.worldID + ".map"
            : Main.ActiveWorldFileData.UniqueId + ".map";
        var mapFilePath = Path.Combine(playerPath, mapFileName);
        return !File.Exists(mapFilePath) ? throw new FileNotFoundException("Map file not found.", mapFilePath) : mapFilePath;
    }

    internal static string SaveMapFile()
    {
        var mapPath = CreateMapFile();
        var mapName = Path.GetFileNameWithoutExtension(mapPath);
        var fileName = $"{mapName}.map";
        var mapPathWithTime = Path.Combine(MapsPath, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}");
        Utils.TryCreatingDirectory(mapPathWithTime);
        var path = Path.Combine(mapPathWithTime, fileName);
        File.Copy(mapPath, path, true);
        return path;
    }
}