using MonoMod.RuntimeDetour;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO.Compression;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Image = SixLabors.ImageSharp.Image;

namespace CaiBot;

internal static class MapGenerator
{
    private static readonly Hook WorldMapIndexerHook = new (typeof(WorldMap).GetMethod("get_Item"), NewWorldMapIndexer);

    internal static void Init()
    {
        WorldMapIndexerHook.Apply();
        MapHelper.Initialize();
        Main.mapEnabled = true;
        Main.Map = new WorldMap(Main.maxTilesX, Main.maxTilesY) { _tiles = new MapTile[Main.maxTilesX, Main.maxTilesY] };
        Main.ActivePlayerFileData = new PlayerFileData { Name = "CaiBot", _path = Main.GetPlayerPathFromName("CaiBot", false) };
        Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
    }

    internal static void Dispose()
    {
        WorldMapIndexerHook.Dispose();
    }

    private static MapTile NewWorldMapIndexer(Func<WorldMap, int, int, MapTile> orig, WorldMap self, int x, int y)
    {
        return self._tiles[x, y];
    }

    private static void LightWholeMap()
    {
        Main.Map = new WorldMap(Main.maxTilesX, Main.maxTilesY) { _tiles = new MapTile[Main.maxTilesX, Main.maxTilesY] };
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                Main.Map._tiles[x, y] = MapHelper.CreateMapTile(x, y, byte.MaxValue);
            }
        }
    }

    internal static Image CreateMapImg()
    {
        Image<Rgba32> image = new (Main.maxTilesX, Main.maxTilesY);
        LightWholeMap();
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = Main.Map[x, y];
                var col = MapHelper.GetMapTileXnaColor(ref tile);
                image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
            }
        }

        return image;
    }

    internal static (string, string) CreateMapFile()
    {
        LightWholeMap();
        MapHelper.SaveMap();
        var playerPath = Main.playerPathName[..^4] + Path.DirectorySeparatorChar;
        var mapFileName = !Main.ActiveWorldFileData.UseGuidAsMapName ? Main.worldID + ".map" : Main.ActiveWorldFileData.UniqueId + ".map";
        var mapFilePath = Path.Combine(playerPath, mapFileName);
        return (Utils.FileToBase64String(mapFilePath), mapFileName);
    }
}