using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO.Compression;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Image = SixLabors.ImageSharp.Image;

namespace CaiBot;

public static class MapGenerator
{
    public static Image Create()
    {
        Image<Rgba32> image = new(Main.maxTilesX, Main.maxTilesY);
        MapHelper.Initialize();
        Main.Map = new(Main.maxTilesX, Main.maxTilesY);
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

internal class CreateMapFile
{
    public static readonly CreateMapFile Instance = new();

    public bool Status { get; private set; }

    //private WorldMap WorldMap { get; set; }


    public void Init()
    {
        MapHelper.Initialize();
        Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
    }

    public MapInfo Start()
    {
        this.Status = true;
        var worldMap = new WorldMap(Main.tile.Width, Main.tile.Height);
        Main.Map = new(Main.tile.Width, Main.tile.Height);
        for (var x = 0; x < Main.maxTilesX; x++)
        {
            for (var y = 0; y < Main.maxTilesY; y++)
            {
                var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                worldMap.SetTile(x, y, ref tile);
            }
        }

        var res = this.InternalSaveMap(worldMap);
        this.Status = false;
        return res;
    }

    public MapInfo InternalSaveMap(WorldMap worldMap)
    {
        var text = !Main.ActiveWorldFileData.UseGuidAsMapName
            ? Main.worldID + ".map"
            : Main.ActiveWorldFileData.UniqueId.ToString() + ".map";
        using (MemoryStream memoryStream = new(4000))
        {
            using BinaryWriter binaryWriter = new(memoryStream);
            using DeflateStream deflateStream = new(memoryStream, CompressionMode.Compress);
            var num = 0;
            var array = new byte[16384];
            binaryWriter.Write(279);
            Main.MapFileMetadata.IncrementAndWrite(binaryWriter);
            binaryWriter.Write(Main.worldName);
            binaryWriter.Write(Main.worldID);
            binaryWriter.Write(Main.maxTilesY);
            binaryWriter.Write(Main.maxTilesX);
            binaryWriter.Write((short) TileID.Count);
            binaryWriter.Write((short) WallID.Count);
            binaryWriter.Write((short) 4);
            binaryWriter.Write((short) 256);
            binaryWriter.Write((short) 256);
            binaryWriter.Write((short) 256);
            byte b = 1;
            byte b2 = 0;
            int i;
            for (i = 0; i < TileID.Count; i++)
            {
                if (MapHelper.tileOptionCounts[i] != 1)
                {
                    b2 = (byte) (b2 | b);
                }

                if (b == 128)
                {
                    binaryWriter.Write(b2);
                    b2 = 0;
                    b = 1;
                }
                else
                {
                    b = (byte) (b << 1);
                }
            }

            if (b != 1)
            {
                binaryWriter.Write(b2);
            }

            i = 0;
            b = 1;
            b2 = 0;
            for (; i < WallID.Count; i++)
            {
                if (MapHelper.wallOptionCounts[i] != 1)
                {
                    b2 = (byte) (b2 | b);
                }

                if (b == 128)
                {
                    binaryWriter.Write(b2);
                    b2 = 0;
                    b = 1;
                }
                else
                {
                    b = (byte) (b << 1);
                }
            }

            if (b != 1)
            {
                binaryWriter.Write(b2);
            }

            for (i = 0; i < TileID.Count; i++)
            {
                if (MapHelper.tileOptionCounts[i] != 1)
                {
                    binaryWriter.Write((byte) MapHelper.tileOptionCounts[i]);
                }
            }

            for (i = 0; i < WallID.Count; i++)
            {
                if (MapHelper.wallOptionCounts[i] != 1)
                {
                    binaryWriter.Write((byte) MapHelper.wallOptionCounts[i]);
                }
            }

            binaryWriter.Flush();
            for (var j = 0; j < Main.maxTilesY; j++)
            {
                if (!MapHelper.noStatusText)
                {
                    var num2 = j / (float) Main.maxTilesY;
                    //Main.statusText = Lang.gen[66].Value + " " + (int)(num2 * 100f + 1f) + "%";
                }

                int num3;
                for (num3 = 0; num3 < Main.maxTilesX; num3++)
                {
                    var mapTile = worldMap[num3, j];
                    byte b4;
                    byte b3;
                    var b5 = b4 = b3 = 0;
                    int num4;
                    var flag = true;
                    var flag2 = true;
                    var num5 = 0;
                    var num6 = 0;
                    byte b6 = 0;
                    var num7 = 0;
                    ushort num8;
                    if (mapTile.Light <= 18)
                    {
                        flag2 = false;
                        flag = false;
                        num7 = 0;
                        num8 = 0;
                        num4 = 0;
                        var num9 = num3 + 1;
                        var num10 = Main.maxTilesX - num3 - 1;
                        while (num10 > 0 && worldMap[num9, j].Light <= 18)
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
                            num8 = (ushort) (num8 - MapHelper.tilePosition);
                        }
                        else if (num8 < MapHelper.liquidPosition)
                        {
                            num7 = 2;
                            num8 = (ushort) (num8 - MapHelper.wallPosition);
                        }
                        else if (num8 < MapHelper.skyPosition)
                        {
                            var num11 = num8 - MapHelper.liquidPosition;
                            if (num11 == 3)
                            {
                                b4 = (byte) (b4 | 0x40u);
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
                                ? (ushort) (num8 - MapHelper.rockPosition)
                                : (ushort) (num8 - MapHelper.dirtPosition);
                        }
                        else
                        {
                            num7 = 6;
                            flag = false;
                        }

                        if (mapTile.Light == byte.MaxValue)
                        {
                            flag2 = false;
                        }

                        if (flag2)
                        {
                            num4 = 0;
                            var num9 = num3 + 1;
                            var num10 = Main.maxTilesX - num3 - 1;
                            num5 = num9;
                            while (num10 > 0)
                            {
                                var other = worldMap[num9, j];
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
                            var num9 = num3 + 1;
                            var num10 = Main.maxTilesX - num3 - 1;
                            while (num10 > 0)
                            {
                                var other2 = worldMap[num9, j];
                                if (!mapTile.Equals(ref other2))
                                {
                                    break;
                                }

                                num10--;
                                num4++;
                                num9++;
                            }
                        }
                    }

                    if (b6 > 0)
                    {
                        b4 = (byte) (b4 | (byte) (b6 << 1));
                    }

                    if (b3 != 0)
                    {
                        b4 = (byte) (b4 | 1u);
                    }

                    if (b4 != 0)
                    {
                        b5 = (byte) (b5 | 1u);
                    }

                    b5 = (byte) (b5 | (byte) (num7 << 1));
                    if (flag && num8 > 255)
                    {
                        b5 = (byte) (b5 | 0x10u);
                    }

                    if (flag2)
                    {
                        b5 = (byte) (b5 | 0x20u);
                    }

                    if (num4 > 0)
                    {
                        b5 = num4 <= 255 ? (byte) (b5 | 0x40u) : (byte) (b5 | 0x80u);
                    }

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
                        array[num] = (byte) num8;
                        num++;
                        if (num8 > 255)
                        {
                            array[num] = (byte) (num8 >> 8);
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
                        array[num] = (byte) num4;
                        num++;
                        if (num4 > 255)
                        {
                            array[num] = (byte) (num4 >> 8);
                            num++;
                        }
                    }

                    for (var k = num5; k < num6; k++)
                    {
                        array[num] = worldMap[k, j].Light;
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
            {
                deflateStream.Write(array, 0, num);
            }

            deflateStream.Dispose();
            return new MapInfo(text, memoryStream.ToArray());
        }
    }

}

public class MapInfo
{
    public string Name { get; set; }

    public byte[] Buffer { get; set; }

    public MapInfo(string name, byte[] buffer)
    {
        this.Name = name;
        this.Buffer = buffer;
    }
}
public class WorldMap
{
    public readonly int MaxWidth;
    public readonly int MaxHeight;
    public readonly int BlackEdgeWidth = 40;
    private readonly MapTile[,] _tiles;

    public MapTile this[int x, int y] => this._tiles[x, y];

    public WorldMap(int maxWidth, int maxHeight)
    {
        this.MaxWidth = maxWidth;
        this.MaxHeight = maxHeight;
        this._tiles = new MapTile[this.MaxWidth, this.MaxHeight];
    }

    public void SetTile(int x, int y, ref MapTile tile)
    {
        this._tiles[x, y] = tile;
    }
}