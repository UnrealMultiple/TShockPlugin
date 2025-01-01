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
    internal static Image Create()
    {
        Image<Rgba32> image = new (Main.maxTilesX, Main.maxTilesY);
        MapHelper.Initialize();
        Main.Map = new WorldMap(Main.maxTilesX, Main.maxTilesY);
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

internal static class MapFileGenerator
{
    internal static (string, string) Create()
    {
            MapHelper.Initialize();
            Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
            Main.Map = new WorldMap(Main.maxTilesX, Main.maxTilesY) { _tiles = new MapTile[Main.maxTilesX, Main.maxTilesY] };
            for (var x = 0; x < Main.maxTilesX; x++)
            {
                for (var y = 0; y < Main.maxTilesY; y++)
                {
                    var tile = MapHelper.CreateMapTile(x, y, byte.MaxValue);
                    Main.Map._tiles[x, y] = tile;
                }
            }
            using var output = new MemoryStream(4000);
            using var writer = new BinaryWriter(output);
            using var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize);
            var count = 0;
            var buffer = new byte[16384];
            writer.Write(279);
            Main.MapFileMetadata.IncrementAndWrite(writer);
            writer.Write(Main.worldName);
            writer.Write(Main.worldID);
            writer.Write(Main.maxTilesY);
            writer.Write(Main.maxTilesX);
            writer.Write((short) TileID.Count);
            writer.Write((short) WallID.Count);
            writer.Write((short) 4);
            writer.Write((short) 256);
            writer.Write((short) 256);
            writer.Write((short) 256);
            byte num1 = 1;
            byte num2 = 0;
            for (var index = 0; index < TileID.Count; ++index)
            {
                if (MapHelper.tileOptionCounts[index] != 1)
                {
                    num2 |= num1;
                }

                if (num1 == 128)
                {
                    writer.Write(num2);
                    num2 = 0;
                    num1 = 1;
                }
                else
                {
                    num1 <<= 1;
                }
            }

            if (num1 != 1)
            {
                writer.Write(num2);
            }

            var index1 = 0;
            byte num3 = 1;
            byte num4 = 0;
            for (; index1 < WallID.Count; ++index1)
            {
                if (MapHelper.wallOptionCounts[index1] != 1)
                {
                    num4 |= num3;
                }

                if (num3 == 128)
                {
                    writer.Write(num4);
                    num4 = 0;
                    num3 = 1;
                }
                else
                {
                    num3 <<= 1;
                }
            }

            if (num3 != 1)
            {
                writer.Write(num4);
            }

            for (var index2 = 0; index2 < TileID.Count; ++index2)
            {
                if (MapHelper.tileOptionCounts[index2] != 1)
                {
                    writer.Write((byte) MapHelper.tileOptionCounts[index2]);
                }
            }

            for (var index3 = 0; index3 < WallID.Count; ++index3)
            {
                if (MapHelper.wallOptionCounts[index3] != 1)
                {
                    writer.Write((byte) MapHelper.wallOptionCounts[index3]);
                }
            }

            writer.Flush();
            for (var y = 0; y < Main.maxTilesY; ++y)
            {
                if (!MapHelper.noStatusText)
                {
                    var num5 = y / (float) Main.maxTilesY;
                    Main.statusText = Lang.gen[66].Value + " " + (int) ((num5 * 100.0) + 1.0) + "%";
                }

                int num6;
                for (var x1 = 0; x1 < Main.maxTilesX; x1 = num6 + 1)
                {
                    var mapTile = Main.Map._tiles[x1, y];
                    int num7;
                    var num8 = (byte) (num7 = 0);
                    var num9 = (byte) num7;
                    var num10 = (byte) num7;
                    var flag1 = true;
                    var flag2 = true;
                    var num11 = 0;
                    var num12 = 0;
                    byte num13 = 0;
                    int num14;
                    ushort num15;
                    int num16;
                    if (mapTile.Light <= 18)
                    {
                        flag2 = false;
                        flag1 = false;
                        num14 = 0;
                        num15 = 0;
                        num16 = 0;
                        var x2 = x1 + 1;
                        for (var index4 = Main.maxTilesX - x1 - 1; index4 > 0 && Main.Map._tiles[x2, y].Light <= 18; ++x2)
                        {
                            ++num16;
                            --index4;
                        }
                    }
                    else
                    {
                        num13 = mapTile.Color;
                        num15 = mapTile.Type;
                        if (num15 < MapHelper.wallPosition)
                        {
                            num14 = 1;
                            num15 -= MapHelper.tilePosition;
                        }
                        else if (num15 < MapHelper.liquidPosition)
                        {
                            num14 = 2;
                            num15 -= MapHelper.wallPosition;
                        }
                        else if (num15 < MapHelper.skyPosition)
                        {
                            var num17 = num15 - MapHelper.liquidPosition;
                            if (num17 == 3)
                            {
                                num9 |= 64;
                                num17 = 0;
                            }

                            num14 = 3 + num17;
                            flag1 = false;
                        }
                        else if (num15 < MapHelper.dirtPosition)
                        {
                            num14 = 6;
                            flag2 = false;
                            flag1 = false;
                        }
                        else if (num15 < MapHelper.hellPosition)
                        {
                            num14 = 7;
                            if (num15 < MapHelper.rockPosition)
                            {
                                num15 -= MapHelper.dirtPosition;
                            }
                            else
                            {
                                num15 -= MapHelper.rockPosition;
                            }
                        }
                        else
                        {
                            num14 = 6;
                            flag1 = false;
                        }

                        if (mapTile.Light == byte.MaxValue)
                        {
                            flag2 = false;
                        }

                        if (flag2)
                        {
                            num16 = 0;
                            var x3 = x1 + 1;
                            var num18 = Main.maxTilesX - x1 - 1;
                            num11 = x3;
                            while (num18 > 0)
                            {
                                var other = Main.Map._tiles[x3, y];
                                if (mapTile.EqualsWithoutLight(ref other))
                                {
                                    --num18;
                                    ++num16;
                                    ++x3;
                                }
                                else
                                {
                                    num12 = x3;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            num16 = 0;
                            var x4 = x1 + 1;
                            var num19 = Main.maxTilesX - x1 - 1;
                            while (num19 > 0)
                            {
                                var other = Main.Map._tiles[x4, y];
                                if (mapTile.Equals(ref other))
                                {
                                    --num19;
                                    ++num16;
                                    ++x4;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (num13 > 0)
                    {
                        num9 |= (byte) ((uint) num13 << 1);
                    }

                    if (num8 != 0)
                    {
                        num9 |= 1;
                    }

                    if (num9 != 0)
                    {
                        num10 |= 1;
                    }

                    var num20 = (byte) (num10 | (uint) (byte) (num14 << 1));
                    if (flag1 && num15 > byte.MaxValue)
                    {
                        num20 |= 16;
                    }

                    if (flag2)
                    {
                        num20 |= 32;
                    }

                    if (num16 > 0)
                    {
                        if (num16 > byte.MaxValue)
                        {
                            num20 |= 128;
                        }
                        else
                        {
                            num20 |= 64;
                        }
                    }

                    buffer[count] = num20;
                    ++count;
                    if (num9 != 0)
                    {
                        buffer[count] = num9;
                        ++count;
                    }

                    if (num8 != 0)
                    {
                        buffer[count] = num8;
                        ++count;
                    }

                    if (flag1)
                    {
                        buffer[count] = (byte) num15;
                        ++count;
                        if (num15 > byte.MaxValue)
                        {
                            buffer[count] = (byte) ((uint) num15 >> 8);
                            ++count;
                        }
                    }

                    if (flag2)
                    {
                        buffer[count] = mapTile.Light;
                        ++count;
                    }

                    if (num16 > 0)
                    {
                        buffer[count] = (byte) num16;
                        ++count;
                        if (num16 > byte.MaxValue)
                        {
                            buffer[count] = (byte) (num16 >> 8);
                            ++count;
                        }
                    }

                    for (var x5 = num11; x5 < num12; ++x5)
                    {
                        buffer[count] = Main.Map._tiles[x5, y].Light;
                        ++count;
                    }

                    num6 = x1 + num16;
                    if (count >= 4096)
                    {
                        deflateStream.Write(buffer, 0, count);
                        count = 0;
                    }
                }
            }

            if (count > 0)
            {
                deflateStream.Write(buffer, 0, count);
            }
            return (Utils.CompressBase64(Convert.ToBase64String(output.ToArray())), Path.GetFileName(!Main.ActiveWorldFileData.UseGuidAsMapName ? Main.worldID + ".map" : Main.ActiveWorldFileData.UniqueId + ".map"));
    }
}