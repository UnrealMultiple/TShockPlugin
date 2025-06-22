using Microsoft.Xna.Framework;
using System.IO.Compression;
using Terraria;
using Terraria.GameContent.Tile_Entities;
using TShockAPI;

namespace CreateSpawn;

public class Map
{
    internal static readonly string Paths = Path.Combine(TShock.SavePath, "CreateSpawn");

    private static GZipStream GZipWrite(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Create);
        return new GZipStream(fileStream, CompressionLevel.Optimal);
    }

    private static GZipStream GZipRead(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open);
        return new GZipStream(fileStream, CompressionMode.Decompress);
    }


    public static Stack<Building> LoadBack(string name)
    {
        var filePath = Path.Combine(Paths, $"{name}_bk.map");
        if (!File.Exists(filePath)) return new Stack<Building>();

        var stack = new Stack<Building>();
        using (var fs = GZipRead(filePath))
        using (var reader = new BinaryReader(fs))
        {
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                stack.Push(LoadBuilding(reader));
            }
        }
        return stack;
    }


    public static void SaveBack(string name, Stack<Building> stack)
    {
        Directory.CreateDirectory(Paths);
        var filePath = Path.Combine(Paths, $"{name}_bk.map");

        using (var fs = GZipWrite(filePath))
        using (var writer = new BinaryWriter(fs))
        {
            writer.Write(stack.Count);
            foreach (var building in stack)
            {
                SaveBuilding(writer, building);
            }
        }
    }

    internal static Building LoadClip(string name)
    {
        var filePath = Path.Combine(Paths, $"{name}_cp.map");
        if (!File.Exists(filePath)) return null!;

        using var fs = GZipRead(filePath);
        using var reader = new BinaryReader(fs);
        return LoadBuilding(reader);
    }

    internal static void SaveClip(string name, Building building)
    {
        Directory.CreateDirectory(Paths);
        var filePath = Path.Combine(Paths, $"{name}_cp.map");

        using (var fs = GZipWrite(filePath))
        using (var writer = new BinaryWriter(fs))
        {
            SaveBuilding(writer, building);
        }
    }

    private static void SaveBuilding(BinaryWriter writer, Building clip)
    {
        writer.Write(clip.Origin.X);
        writer.Write(clip.Origin.Y);
        writer.Write(clip.Width);
        writer.Write(clip.Height);

        if (clip.Tiles == null)
        {
            return;
        }

        for (var x = 0; x < clip.Width; x++)
        {
            for (var y = 0; y < clip.Height; y++)
            {
                var tile = clip.Tiles[x, y];
                writer.Write(tile.bTileHeader);
                writer.Write(tile.bTileHeader2);
                writer.Write(tile.bTileHeader3);
                writer.Write(tile.frameX);
                writer.Write(tile.frameY);
                writer.Write(tile.liquid);
                writer.Write(tile.sTileHeader);
                writer.Write(tile.type);
                writer.Write(tile.wall);
            }
        }

        writer.Write(clip.ChestItems?.Count ?? 0);
        if (clip.ChestItems != null)
        {
            foreach (var data in clip.ChestItems)
            {
                writer.Write(data.Position.X);
                writer.Write(data.Position.Y);
                writer.Write(data.Slot);
                writer.Write(data.Item?.type ?? 0);
                writer.Write(data.Item?.netID ?? 0);
                writer.Write(data.Item?.stack ?? 0);
                writer.Write(data.Item?.prefix ?? 0);
            }
        }

        writer.Write(clip.Signs?.Count ?? 0);
        if (clip.Signs != null)
        {
            foreach (var sign in clip.Signs)
            {
                writer.Write(sign.x - clip.Origin.X); // 相对坐标
                writer.Write(sign.y - clip.Origin.Y);
                writer.Write(sign.text ?? "");
            }
        }

        writer.Write(clip.ItemFrames?.Count ?? 0);
        if (clip.ItemFrames != null)
        {
            foreach (var data in clip.ItemFrames)
            {
                writer.Write(data.Position.X - clip.Origin.X); // 存储相对坐标
                writer.Write(data.Position.Y - clip.Origin.Y);
                writer.Write(data.Item.NetId);
                writer.Write(data.Item.Stack);
                writer.Write(data.Item.PrefixId);
            }
        }

        writer.Write(clip.WeaponsRacks?.Count ?? 0);
        if (clip.WeaponsRacks != null)
        {
            foreach (var data in clip.WeaponsRacks)
            {
                writer.Write(data.Position.X - clip.Origin.X);
                writer.Write(data.Position.Y - clip.Origin.Y);
                writer.Write(data.Item.NetId);
                writer.Write(data.Item.Stack);
                writer.Write(data.Item.PrefixId);
            }
        }

        writer.Write(clip.FoodPlatters?.Count ?? 0);
        if (clip.FoodPlatters != null)
        {
            foreach (var data in clip.FoodPlatters)
            {
                writer.Write(data.Position.X - clip.Origin.X);
                writer.Write(data.Position.Y - clip.Origin.Y);
                writer.Write(data.Item.NetId);
                writer.Write(data.Item.Stack);
                writer.Write(data.Item.PrefixId);
            }
        }

        writer.Write(clip.DisplayDolls?.Count ?? 0);
        if (clip.DisplayDolls != null)
        {
            foreach (var doll in clip.DisplayDolls)
            {
                // 保存相对坐标
                writer.Write(doll.Position.X - clip.Origin.X);
                writer.Write(doll.Position.Y - clip.Origin.Y);

                // 保存物品数据 (8个槽位)
                writer.Write(doll.Items.Length);
                foreach (var item in doll.Items)
                {
                    writer.Write(item.NetId);
                    writer.Write(item.Stack);
                    writer.Write(item.PrefixId);
                }

                // 保存染料数据 (8个槽位)
                writer.Write(doll.Dyes.Length);
                foreach (var dye in doll.Dyes)
                {
                    writer.Write(dye.NetId);
                    writer.Write(dye.Stack);
                    writer.Write(dye.PrefixId);
                }
            }
        }

        writer.Write(clip.HatRacks?.Count ?? 0);
        if (clip.HatRacks != null)
        {
            foreach (var Rack in clip.HatRacks)
            {
                // 保存相对坐标
                writer.Write(Rack.Position.X - clip.Origin.X);
                writer.Write(Rack.Position.Y - clip.Origin.Y);

                // 保存物品数据 (2个槽位)
                writer.Write(Rack.Items.Length);
                foreach (var item in Rack.Items)
                {
                    writer.Write(item.NetId);
                    writer.Write(item.Stack);
                    writer.Write(item.PrefixId);
                }

                // 保存染料数据 (2个槽位)
                writer.Write(Rack.Dyes.Length);
                foreach (var dye in Rack.Dyes)
                {
                    writer.Write(dye.NetId);
                    writer.Write(dye.Stack);
                    writer.Write(dye.PrefixId);
                }
            }
        }

        writer.Write(clip.LogicSensors?.Count ?? 0);
        if (clip.LogicSensors != null)
        {
            foreach (var data in clip.LogicSensors)
            {
                writer.Write(data.Position.X - clip.Origin.X);
                writer.Write(data.Position.Y - clip.Origin.Y);
                writer.Write((int) data.type);
            }
        }

    }

    private static Building LoadBuilding(BinaryReader reader)
    {
        var originX = reader.ReadInt32();
        var originY = reader.ReadInt32();
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();

        var tiles = new Terraria.Tile[width, height];
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var tile = new Terraria.Tile
                {
                    bTileHeader = reader.ReadByte(),
                    bTileHeader2 = reader.ReadByte(),
                    bTileHeader3 = reader.ReadByte(),
                    frameX = reader.ReadInt16(),
                    frameY = reader.ReadInt16(),
                    liquid = reader.ReadByte(),
                    sTileHeader = reader.ReadUInt16(),
                    type = reader.ReadUInt16(),
                    wall = reader.ReadUInt16()
                };
                tiles[x, y] = tile;
            }
        }

        var chestItemCount = reader.ReadInt32();
        var chestItems = new List<ChestItems>(chestItemCount);
        for (var i = 0; i < chestItemCount; i++)
        {
            var posX = reader.ReadInt32();
            var posY = reader.ReadInt32();
            var slot = reader.ReadInt32();
            var type = reader.ReadInt32();
            var netId = reader.ReadInt32();
            var stack = reader.ReadInt32();
            var prefix = reader.ReadByte();

            var item = new Item();
            item.SetDefaults(type);
            item.netID = netId;
            item.stack = stack;
            item.prefix = prefix;

            chestItems.Add(new ChestItems
            {
                Position = new Point(posX, posY),
                Slot = slot,
                Item = item
            });
        }

        var signCount = reader.ReadInt32();
        var signs = new List<Sign>(signCount);
        for (var i = 0; i < signCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();
            var text = reader.ReadString();

            signs.Add(new Sign
            {
                x = originX + relX,
                y = originY + relY,
                text = text
            });
        }

        var itemFrameCount = reader.ReadInt32();
        var itemFrames = new List<ItemFrames>(itemFrameCount);
        for (var i = 0; i < itemFrameCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();
            var netId = reader.ReadInt32();
            var stack = reader.ReadInt32();
            var prefix = reader.ReadByte();

            itemFrames.Add(new ItemFrames
            {
                Position = new Point(originX + relX, originY + relY),
                Item = new NetItem(netId, stack, prefix)
            });
        }

        var weaponRackCount = reader.ReadInt32();
        var weaponRacks = new List<WeaponRack>(weaponRackCount);
        for (var i = 0; i < weaponRackCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();
            var netId = reader.ReadInt32();
            var stack = reader.ReadInt32();
            var prefix = reader.ReadByte();

            weaponRacks.Add(new WeaponRack
            {
                Position = new Point(originX + relX, originY + relY),
                Item = new NetItem(netId, stack, prefix)
            });
        }

        var foodPlatterCount = reader.ReadInt32();
        var foodPlatters = new List<FPlatters>(foodPlatterCount);
        for (var i = 0; i < foodPlatterCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();
            var netId = reader.ReadInt32();
            var stack = reader.ReadInt32();
            var prefix = reader.ReadByte();

            foodPlatters.Add(new FPlatters
            {
                Position = new Point(originX + relX, originY + relY),
                Item = new NetItem(netId, stack, prefix)
            });
        }

        var dollCount = reader.ReadInt32();
        var displayDolls = new List<DDolls>(dollCount);
        for (var i = 0; i < dollCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();

            // 读取物品
            var itemCount = reader.ReadInt32();
            var items = new NetItem[itemCount];
            for (var j = 0; j < itemCount; j++)
            {
                items[j] = new NetItem(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadByte()
                );
            }

            // 读取染料
            var dyeCount = reader.ReadInt32();
            var dyes = new NetItem[dyeCount];
            for (var j = 0; j < dyeCount; j++)
            {
                dyes[j] = new NetItem(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadByte()
                );
            }

            displayDolls.Add(new DDolls
            {
                Position = new Point(originX + relX, originY + relY),
                Items = items,
                Dyes = dyes
            });
        }

        var RackCount = reader.ReadInt32();
        var hatRacks = new List<HatRacks>(RackCount);
        for (var i = 0; i < RackCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();

            // 读取物品
            var itemCount = reader.ReadInt32();
            var items = new NetItem[itemCount];
            for (var j = 0; j < itemCount; j++)
            {
                items[j] = new NetItem(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadByte()
                );
            }

            // 读取染料
            var dyeCount = reader.ReadInt32();
            var dyes = new NetItem[dyeCount];
            for (var j = 0; j < dyeCount; j++)
            {
                dyes[j] = new NetItem(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadByte()
                );
            }

            hatRacks.Add(new HatRacks
            {
                Position = new Point(originX + relX, originY + relY),
                Items = items,
                Dyes = dyes
            });
        }

        var LogicSensorsCount = reader.ReadInt32();
        var logicSensors = new List<LogicSensors>(LogicSensorsCount);
        for (var i = 0; i < LogicSensorsCount; i++)
        {
            var relX = reader.ReadInt32();
            var relY = reader.ReadInt32();
            var type = reader.ReadInt32();

            logicSensors.Add(new LogicSensors
            {
                Position = new Point(originX + relX, originY + relY),
                type = (TELogicSensor.LogicCheckType) reader.ReadInt32()
            });
        }

        return new Building
        {
            Origin = new Point(originX, originY),
            Width = width,
            Height = height,
            Tiles = tiles,
            ChestItems = chestItems,
            Signs = signs,
            ItemFrames = itemFrames,
            WeaponsRacks = weaponRacks,
            FoodPlatters = foodPlatters,
            DisplayDolls = displayDolls,
            HatRacks = hatRacks,
            LogicSensors = logicSensors
        };
    }

    public static List<string> GetAllClipNames()
    {
        if (!Directory.Exists(Map.Paths))
            return new List<string>();

        return Directory.GetFiles(Map.Paths, "*_cp.map")
                        .Select(f => Path.GetFileNameWithoutExtension(f).Replace("_cp", ""))
                        .ToList();
    }

    public static void BackupAndDeleteAllDataFiles()
    {
        if (!Directory.Exists(Map.Paths)) return;

        // 构建压缩包保存路径
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var backupFolder = Path.Combine(Map.Paths, $"{timestamp}");
        var zipFilePath = Path.Combine(Map.Paths, $"{timestamp}.zip");

        try
        {
            // 创建临时备份文件夹
            Directory.CreateDirectory(backupFolder);

            // 将所有 .dat 文件复制到备份文件夹
            foreach (var file in Directory.GetFiles(Map.Paths, "*_cp.map"))
            {
                var destFile = Path.Combine(backupFolder, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            // 压缩文件夹为 .zip
            ZipFile.CreateFromDirectory(backupFolder, zipFilePath, CompressionLevel.SmallestSize, false);

            // 删除临时文件夹（不再需要了）
            Directory.Delete(backupFolder, recursive: true);

            TShock.Log.ConsoleInfo(GetString($"已成功备份所有 .map 文件，压缩包保存于:\n {zipFilePath}"));

            // 删除原始 .dat 文件
            foreach (var file in Directory.GetFiles(Map.Paths, "*.map"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleInfo(GetString($"删除文件失败: {file}, 错误: {ex.Message}"));
                }
            }

            TShock.Log.ConsoleInfo(GetString($"已成功删除所有 .map 文件"));
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleInfo(GetString($"备份和删除过程中出错: {ex.Message}"));
        }
    }
}