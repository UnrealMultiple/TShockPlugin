// 来自：https://github.com/Controllerdestiny/MorMor/blob/master/MorMor/TShock/Map/

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Terraria;
using Terraria.Map;
using Color = Microsoft.Xna.Framework.Color;
using Image = SixLabors.ImageSharp.Image;
using ReLogic.OS;
using System.IO.Compression;
using Terraria.IO;
using Terraria.ID;

namespace CaiBot;

public static class MapGenerator
{
    public static Image Create()
    {
        Image<Rgba32> image = new(Main.maxTilesX, Main.maxTilesY);
        MapHelper.Initialize();
        Main.Map = new (0, 0);
        for (int x = 0; x < Main.maxTilesX; x++)
        for (int y = 0; y < Main.maxTilesY; y++)
        {
            MapTile tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
            Color col = MapHelper.GetMapTileXnaColor(ref tile);
            image[x, y] = new Rgba32(col.R, col.G, col.B, col.A);
        }

        return image;
    }
}

internal class CreateMapFile
{
    public static readonly CreateMapFile Instance = new();

    public bool Status { get; private set; } = false;

    private WorldMap WorldMap { get; set; }


    public void Init()
    {
        MapHelper.Initialize();
        Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
    }

    public MapInfo Start()
    {
        Status = true;
        WorldMap = new WorldMap(Main.tile.Width, Main.tile.Height);
        Main.Map = new (Main.tile.Width, Main.tile.Height);
        for (int x = 0; x < Main.maxTilesX; x++)
        for (int y = 0; y < Main.maxTilesY; y++)
        {
            MapTile tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
            WorldMap.SetTile(x, y, ref tile);
        }
        MapInfo res = InternalSaveMap();
        Status = false;
        return res;
    }

    public MapInfo InternalSaveMap()
    {
        string text = !Main.ActiveWorldFileData.UseGuidAsMapName
            ? Main.worldID + ".map"
            : Main.ActiveWorldFileData.UniqueId.ToString() + ".map";
        using (MemoryStream memoryStream = new(4000))
        {
            using BinaryWriter binaryWriter = new(memoryStream);
            using DeflateStream deflateStream = new(memoryStream, CompressionMode.Compress);
            int num = 0;
            byte[] array = new byte[16384];
            binaryWriter.Write(279);
            Main.MapFileMetadata.IncrementAndWrite(binaryWriter);
            binaryWriter.Write(Main.worldName);
            binaryWriter.Write(Main.worldID);
            binaryWriter.Write(Main.maxTilesY);
            binaryWriter.Write(Main.maxTilesX);
            binaryWriter.Write((short)TileID.Count);
            binaryWriter.Write((short)WallID.Count);
            binaryWriter.Write((short)4);
            binaryWriter.Write((short)256);
            binaryWriter.Write((short)256);
            binaryWriter.Write((short)256);
            byte b = 1;
            byte b2 = 0;
            int i;
            for (i = 0; i < TileID.Count; i++)
            {
                if (MapHelper.tileOptionCounts[i] != 1)
                    b2 = (byte)(b2 | b);

                if (b == 128)
                {
                    binaryWriter.Write(b2);
                    b2 = 0;
                    b = 1;
                }
                else
                {
                    b = (byte)(b << 1);
                }
            }

            if (b != 1)
                binaryWriter.Write(b2);

            i = 0;
            b = 1;
            b2 = 0;
            for (; i < WallID.Count; i++)
            {
                if (MapHelper.wallOptionCounts[i] != 1)
                    b2 = (byte)(b2 | b);

                if (b == 128)
                {
                    binaryWriter.Write(b2);
                    b2 = 0;
                    b = 1;
                }
                else
                {
                    b = (byte)(b << 1);
                }
            }

            if (b != 1)
                binaryWriter.Write(b2);

            for (i = 0; i < TileID.Count; i++)
                if (MapHelper.tileOptionCounts[i] != 1)
                    binaryWriter.Write((byte)MapHelper.tileOptionCounts[i]);

            for (i = 0; i < WallID.Count; i++)
                if (MapHelper.wallOptionCounts[i] != 1)
                    binaryWriter.Write((byte)MapHelper.wallOptionCounts[i]);

            binaryWriter.Flush();
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                if (!MapHelper.noStatusText)
                {
                    float num2 = j / (float)Main.maxTilesY;
                    //Main.statusText = Lang.gen[66].Value + " " + (int)(num2 * 100f + 1f) + "%";
                }

                int num3;
                for (num3 = 0; num3 < Main.maxTilesX; num3++)
                {
                    MapTile mapTile = WorldMap[num3, j];
                    byte b4;
                    byte b3;
                    byte b5 = b4 = b3 = 0;
                    int num4 = 0;
                    bool flag = true;
                    bool flag2 = true;
                    int num5 = 0;
                    int num6 = 0;
                    byte b6 = 0;
                    int num7;
                    ushort num8;
                    if (mapTile.Light <= 18)
                    {
                        flag2 = false;
                        flag = false;
                        num7 = 0;
                        num8 = 0;
                        num4 = 0;
                        int num9 = num3 + 1;
                        int num10 = Main.maxTilesX - num3 - 1;
                        while (num10 > 0 && WorldMap[num9, j].Light <= 18)
                        {
                            num4++;
                            num10--;
                            num9++;
                        }
                    }
                    else
                    {
                        b6 = mapTile.Color;
                        num8 = mapTile.Type;
                        if (num8 < MapHelper.wallPosition)
                        {
                            num7 = 1;
                            num8 = (ushort)(num8 - MapHelper.tilePosition);
                        }
                        else if (num8 < MapHelper.liquidPosition)
                        {
                            num7 = 2;
                            num8 = (ushort)(num8 - MapHelper.wallPosition);
                        }
                        else if (num8 < MapHelper.skyPosition)
                        {
                            int num11 = num8 - MapHelper.liquidPosition;
                            if (num11 == 3)
                            {
                                b4 = (byte)(b4 | 0x40u);
                                num11 = 0;
                            }

                            num7 = 3 + num11;
                            flag = false;
                        }
                        else if (num8 < MapHelper.dirtPosition)
                        {
                            num7 = 6;
                            flag2 = false;
                            flag = false;
                        }
                        else if (num8 < MapHelper.hellPosition)
                        {
                            num7 = 7;
                            num8 = num8 >= MapHelper.rockPosition
                                ? (ushort)(num8 - MapHelper.rockPosition)
                                : (ushort)(num8 - MapHelper.dirtPosition);
                        }
                        else
                        {
                            num7 = 6;
                            flag = false;
                        }

                        if (mapTile.Light == byte.MaxValue)
                            flag2 = false;

                        if (flag2)
                        {
                            num4 = 0;
                            int num9 = num3 + 1;
                            int num10 = Main.maxTilesX - num3 - 1;
                            num5 = num9;
                            while (num10 > 0)
                            {
                                MapTile other = WorldMap[num9, j];
                                if (mapTile.EqualsWithoutLight(ref other))
                                {
                                    num10--;
                                    num4++;
                                    num9++;
                                    continue;
                                }

                                num6 = num9;
                                break;
                            }
                        }
                        else
                        {
                            num4 = 0;
                            int num9 = num3 + 1;
                            int num10 = Main.maxTilesX - num3 - 1;
                            while (num10 > 0)
                            {
                                MapTile other2 = WorldMap[num9, j];
                                if (!mapTile.Equals(ref other2))
                                    break;

                                num10--;
                                num4++;
                                num9++;
                            }
                        }
                    }

                    if (b6 > 0)
                        b4 = (byte)(b4 | (byte)(b6 << 1));

                    if (b3 != 0)
                        b4 = (byte)(b4 | 1u);

                    if (b4 != 0)
                        b5 = (byte)(b5 | 1u);

                    b5 = (byte)(b5 | (byte)(num7 << 1));
                    if (flag && num8 > 255)
                        b5 = (byte)(b5 | 0x10u);

                    if (flag2)
                        b5 = (byte)(b5 | 0x20u);

                    if (num4 > 0)
                        b5 = num4 <= 255 ? (byte)(b5 | 0x40u) : (byte)(b5 | 0x80u);

                    array[num] = b5;
                    num++;
                    if (b4 != 0)
                    {
                        array[num] = b4;
                        num++;
                    }

                    if (b3 != 0)
                    {
                        array[num] = b3;
                        num++;
                    }

                    if (flag)
                    {
                        array[num] = (byte)num8;
                        num++;
                        if (num8 > 255)
                        {
                            array[num] = (byte)(num8 >> 8);
                            num++;
                        }
                    }

                    if (flag2)
                    {
                        array[num] = mapTile.Light;
                        num++;
                    }

                    if (num4 > 0)
                    {
                        array[num] = (byte)num4;
                        num++;
                        if (num4 > 255)
                        {
                            array[num] = (byte)(num4 >> 8);
                            num++;
                        }
                    }

                    for (int k = num5; k < num6; k++)
                    {
                        array[num] = WorldMap[k, j].Light;
                        num++;
                    }

                    num3 += num4;
                    if (num >= 4096)
                    {
                        deflateStream.Write(array, 0, num);
                        num = 0;
                    }
                }
            }

            if (num > 0)
                deflateStream.Write(array, 0, num);
            deflateStream.Dispose();
            return new MapInfo(text, memoryStream.ToArray());
        }
    }

    public void Dispose()
    {
        Main.netMode = 2;
        WorldGen.clearWorld();
        for (int i = 0; i < Main.tile.Width; i++)
        for (int j = 0; j < Main.tile.Height; j++)
            Main.tile[i, j] = null;
        WorldMap = null;
        for (int num3 = 0; num3 < 6000; num3++) Main.dust[num3] = null;
        for (int num4 = 0; num4 < 600; num4++) Main.gore[num4] = null;
        for (int num5 = 0; num5 < 400; num5++) Main.item[num5] = null;
        for (int num6 = 0; num6 < 200; num6++) Main.npc[num6] = null;
        for (int num7 = 0; num7 < 1000; num7++) Main.projectile[num7] = null;
        for (int num8 = 0; num8 < 8000; num8++) Main.chest[num8] = null;
        for (int num9 = 0; num9 < 1000; num9++) Main.sign[num9] = null;
        for (int num10 = 0; num10 < Liquid.maxLiquid; num10++) Main.liquid[num10] = null;
        for (int num11 = 0; num11 < 50000; num11++) Main.liquidBuffer[num11] = null;
        GC.Collect();
    }
}

public class MapInfo
{
    public string Name { get; set; }

    public byte[] Buffer { get; set; }

    public MapInfo(string name, byte[] buffer)
    {
        Name = name;
        Buffer = buffer;
    }
}
public class WorldMap
{
    public readonly int MaxWidth;
    public readonly int MaxHeight;
    public readonly int BlackEdgeWidth = 40;
    private MapTile[,] _tiles;

    public MapTile this[int x, int y] => _tiles[x, y];

    public WorldMap(int maxWidth, int maxHeight)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        _tiles = new MapTile[MaxWidth, MaxHeight];
    }

    public void ConsumeUpdate(int x, int y) => _tiles[x, y].IsChanged = false;

    public void Update(int x, int y, byte light) => _tiles[x, y] = MapHelper.CreateMapTile(x, y, light);

    public void SetTile(int x, int y, ref MapTile tile) => _tiles[x, y] = tile;

    public bool IsRevealed(int x, int y) => _tiles[x, y].Light > 0;

    public bool UpdateLighting(int x, int y, byte light)
    {
        MapTile tile = _tiles[x, y];
        if (light == 0 && tile.Light == 0)
            return false;
        MapTile mapTile = MapHelper.CreateMapTile(x, y, Math.Max(tile.Light, light));
        if (mapTile.Equals(ref tile))
            return false;
        _tiles[x, y] = mapTile;
        return true;
    }

    public bool UpdateType(int x, int y)
    {
        MapTile mapTile = MapHelper.CreateMapTile(x, y, _tiles[x, y].Light);

        return true;
    }

    public void UnlockMapSection(int sectionX, int sectionY)
    {
    }

    public void Load()
    {

    }

    public void Save()
    { }

    public void Clear()
    {
        for (int index1 = 0; index1 < MaxWidth; ++index1)
        {
            for (int index2 = 0; index2 < MaxHeight; ++index2)
                _tiles[index1, index2].Clear();
        }
    }

    public void ClearEdges()
    {
        for (int index1 = 0; index1 < MaxWidth; ++index1)
        {
            for (int index2 = 0; index2 < BlackEdgeWidth; ++index2)
                _tiles[index1, index2].Clear();
        }
        for (int index3 = 0; index3 < MaxWidth; ++index3)
        {
            for (int index4 = MaxHeight - BlackEdgeWidth; index4 < MaxHeight; ++index4)
                _tiles[index3, index4].Clear();
        }
        for (int index = 0; index < BlackEdgeWidth; ++index)
        {
            for (int blackEdgeWidth = BlackEdgeWidth; blackEdgeWidth < MaxHeight - BlackEdgeWidth; ++blackEdgeWidth)
                _tiles[index, blackEdgeWidth].Clear();
        }
        for (int index = MaxWidth - BlackEdgeWidth; index < MaxWidth; ++index)
        {
            for (int blackEdgeWidth = BlackEdgeWidth; blackEdgeWidth < MaxHeight - BlackEdgeWidth; ++blackEdgeWidth)
                _tiles[index, blackEdgeWidth].Clear();
        }
    }
}