using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;
using WorldEdit.Expressions;

namespace WorldEdit;

public static class Tools
{
    internal const int BUFFER_SIZE = 1048576;

    internal static int MAX_UNDOS;

    private static int TranslateTempCounter = 0;
    private static Random rnd = new Random();
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    public static bool Translate(string path, bool logError, string tempCopyPath = null)
    {
        // 移除无意义变量 `text`，直接使用临时路径
        string tempPath = tempCopyPath ?? Path.Combine("worldedit", $"temp-{rnd.Next()}-{Interlocked.Increment(ref TranslateTempCounter)}.dat");
        File.Copy(path, tempPath, true); // 简化参数名

        bool conversionSucceeded = true;
        try
        {
            LoadWorldDataOld(path).Write(path);
        }
        catch (Exception ex) // 修复变量名 `value` 为更通用的 `ex`
        {
            if (logError)
            {
                TShock.Log.ConsoleError($"[WorldEdit] File '{path}' could not be converted to Terraria v1.4:\n{ex}");
            }
            conversionSucceeded = false;
        }

        if (!conversionSucceeded)
        {
            File.Copy(tempPath, path, true); // 简化参数名
        }

        File.Delete(tempPath); // 直接使用临时路径变量
        return conversionSucceeded;
    }

    public static bool InMapBoundaries(int X, int Y)
    {
        return X >= 0 && Y >= 0 && X < Main.maxTilesX && Y < Main.maxTilesY;
    }

    private static void RestoreItems(Item[] destination, NetItem[] source)
    {
        for (int i = 0; i < destination.Length; i++)
        {
            destination[i] = i < source.Length ? source[i].ToItem() : new Item();
        }
    }

    public static string GetClipboardPath(int accountID)
    {
        return Path.Combine("worldedit", $"clipboard-{Main.worldID}-{accountID}.dat");
    }

    public static bool IsCorrectName(string name)
    {
        // 移除冗余的 `(char c)` 类型声明（编译器可推断）
        return name.All(c => !InvalidFileNameChars.Contains(c));
    }

    // 以下方法仅优化冗余变量，逻辑不变
    public static List<int> GetColorID(string color)
    {
        if (int.TryParse(color, out int result) && result >= 0 && result < 32) // 合并变量声明
        {
            return new List<int> { result };
        }

        List<int> list = new List<int>();
		foreach (KeyValuePair<string, int> colorEntry in WorldEdit.Colors)
		{
			if (colorEntry.Key == color)
			{
				return new List<int> { colorEntry.Value };
			}
			if (colorEntry.Key.StartsWith(color))
			{
				list.Add(colorEntry.Value);
            }
        }
        return list;
    }

    public static List<int> GetTileID(string tile)
    {
        if (int.TryParse(tile, out int result) && result >= 0 && result < TileID.Count)
        {
            return new List<int> { result };
        }

        List<int> list = new List<int>();
		foreach (KeyValuePair<string, int> tileEntry in WorldEdit.Tiles)
		{
			if (tileEntry.Key == tile)
			{
				return new List<int> { tileEntry.Value };
			}
			if (tileEntry.Key.StartsWith(tile))
			{
				list.Add(tileEntry.Value);
            }
        }
        return list;
    }

    public static List<int> GetWallID(string wall)
    {
        if (int.TryParse(wall, out int result) && result >= 0 && result < WallID.Count)
        {
            return new List<int> { result };
        }

        List<int> list = new List<int>();
		foreach (KeyValuePair<string, int> wallEntry in WorldEdit.Walls)
		{
			if (wallEntry.Key == wall)
			{
				return new List<int> { wallEntry.Value };
			}
			if (wallEntry.Key.StartsWith(wall))
			{
				list.Add(wallEntry.Value);
            }
        }
        return list;
    }

    public static int GetSlopeID(string slope)
    {
        if (int.TryParse(slope, out int result) && result >= 0 && result < 6) // 合并变量声明
        {
            return result;
        }
        return WorldEdit.Slopes.TryGetValue(slope, out int value) ? value : -1; // 移除冗余 `if` 块
    }

    public static bool HasClipboard(int accountID)
    {
        return File.Exists(GetClipboardPath(accountID));
    }

    public static Rectangle ReadSize(Stream stream)
    {
        // 移除多余的缩进，保持代码对齐
        using BinaryReader binaryReader = new BinaryReader(stream);
        return new Rectangle(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
    }

    public static Rectangle ReadSize(string path)
    {
        return ReadSize(File.Open(path, FileMode.Open));
    }

    private static T[] ReadDataArray<T>(BinaryReader reader, Func<BinaryReader, T> readItem)
    {
        int itemCount = reader.ReadInt32();
        T[] items = new T[itemCount];
        for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            items[itemIndex] = readItem(reader);
        }

        return items;
    }

    public static WorldSectionData LoadWorldData(Stream stream)
    {
        int originX;
        int originY;
        int width;
        int height;
        using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
        {
            originX = binaryReader.ReadInt32();
            originY = binaryReader.ReadInt32();
            width = binaryReader.ReadInt32();
            height = binaryReader.ReadInt32();
        }
		using BinaryReader compressedReader = new BinaryReader(new BufferedStream(new GZipStream(stream, CompressionMode.Decompress), 1048576));
        WorldSectionData worldSectionData = new WorldSectionData(width, height)
        {
            X = originX,
            Y = originY
        };
        for (int tileX = 0; tileX < width; tileX++)
        {
            for (int tileY = 0; tileY < height; tileY++)
            {
				worldSectionData.Tiles[tileX, tileY] = (ITile)(object)compressedReader.ReadTile();
            }
        }
        try
        {
			worldSectionData.Signs = ReadDataArray(compressedReader, WorldSectionData.SignData.Read);
			worldSectionData.Chests = ReadDataArray(compressedReader, WorldSectionData.ChestData.Read);
			worldSectionData.ItemFrames = ReadDataArray(compressedReader, WorldSectionData.DisplayItemData.Read);
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
			worldSectionData.LogicSensors = ReadDataArray(compressedReader, WorldSectionData.LogicSensorData.Read);
			worldSectionData.TrainingDummies = ReadDataArray(compressedReader, WorldSectionData.PositionData.Read);
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
			worldSectionData.WeaponsRacks = ReadDataArray(compressedReader, WorldSectionData.DisplayItemData.Read);
			worldSectionData.TeleportationPylons = ReadDataArray(compressedReader, WorldSectionData.PositionData.Read);
			worldSectionData.DisplayDolls = ReadDataArray(compressedReader, WorldSectionData.DisplayItemsData.Read);
			worldSectionData.HatRacks = ReadDataArray(compressedReader, WorldSectionData.DisplayItemsData.Read);
			worldSectionData.FoodPlatters = ReadDataArray(compressedReader, WorldSectionData.DisplayItemData.Read);
			worldSectionData.DisplayJars = ReadDataArray(compressedReader, WorldSectionData.DisplayItemData.Read);
			worldSectionData.DisplayDollExtras = ReadDataArray(compressedReader, WorldSectionData.DisplayDollExtraData.Read);
        }
        catch (EndOfStreamException)
        {
        }
        return worldSectionData;
    }

    public static WorldSectionData LoadWorldData(string path)
    {
        return LoadWorldData(File.Open(path, FileMode.Open));
    }

    internal static WorldSectionData LoadWorldDataOld(Stream stream)
    {
        using BinaryReader binaryReader = new BinaryReader(new BufferedStream(new GZipStream(stream, CompressionMode.Decompress), 1048576));
        int originX = binaryReader.ReadInt32();
        int originY = binaryReader.ReadInt32();
        int width = binaryReader.ReadInt32();
        int height = binaryReader.ReadInt32();
        WorldSectionData worldSectionData = new WorldSectionData(width, height)
        {
            X = originX,
            Y = originY
        };
        for (int tileX = 0; tileX < width; tileX++)
        {
            for (int tileY = 0; tileY < height; tileY++)
            {
                worldSectionData.Tiles[tileX, tileY] = (ITile)(object)binaryReader.ReadTileOld();
            }
        }
        try
        {
            worldSectionData.Signs = ReadDataArray(binaryReader, WorldSectionData.SignData.Read);
            worldSectionData.Chests = ReadDataArray(binaryReader, WorldSectionData.ChestData.Read);
            worldSectionData.ItemFrames = ReadDataArray(binaryReader, WorldSectionData.DisplayItemData.Read);
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
            worldSectionData.LogicSensors = ReadDataArray(binaryReader, WorldSectionData.LogicSensorData.Read);
            worldSectionData.TrainingDummies = ReadDataArray(binaryReader, WorldSectionData.PositionData.Read);
        }
        catch (EndOfStreamException)
        {
        }
        return worldSectionData;
    }

    internal static WorldSectionData LoadWorldDataOld(string path)
    {
        return LoadWorldDataOld(File.Open(path, FileMode.Open));
    }

    public static Tile ReadTile(this BinaryReader reader)
    {
        Tile val = new Tile
        {
            sTileHeader = (ushort) reader.ReadInt16(),
            bTileHeader = reader.ReadByte(),
            bTileHeader2 = reader.ReadByte()
        };
        if (val.active())
        {
            val.type = reader.ReadUInt16();
            if (Main.tileFrameImportant[val.type])
            {
                val.frameX = reader.ReadInt16();
                val.frameY = reader.ReadInt16();
            }
        }
        val.wall = reader.ReadUInt16();
        val.liquid = reader.ReadByte();
        return val;
    }

    private static Tile ReadTileOld(this BinaryReader reader)
    {
        Tile val = new Tile
        {
            sTileHeader = (ushort) reader.ReadInt16(),
            bTileHeader = reader.ReadByte(),
            bTileHeader2 = reader.ReadByte()
        };
        if (val.active())
        {
            val.type = reader.ReadUInt16();
            if (val.type != 49 && Main.tileFrameImportant[val.type])
            {
                val.frameX = reader.ReadInt16();
                val.frameY = reader.ReadInt16();
            }
        }
        val.wall = reader.ReadByte();
        val.liquid = reader.ReadByte();
        return val;
    }

    public static NetItem ReadNetItem(this BinaryReader reader)
    {
        return new NetItem(reader.ReadInt32(), reader.ReadInt32(), reader.ReadByte());
    }

    internal static NetItem[] ReadNetItems(this BinaryReader reader)
    {
        return ReadDataArray(reader, ReadNetItem);
    }

    public static int ClearSigns(int x, int y, int x2, int y2, bool emptyOnly)
    {
        int removedSignCount = 0;
        Rectangle area = new Rectangle(x, y, x2 - x, y2 - y);

        foreach (Sign sign in Main.sign)
        {
            if (sign != null && area.Contains(sign.x, sign.y) && (!emptyOnly || string.IsNullOrWhiteSpace(sign.text)))
            {
                removedSignCount++;
                Sign.KillSign(sign.x, sign.y);
            }
        }

        return removedSignCount;
    }

    public static int ClearChests(int x, int y, int x2, int y2, bool emptyOnly)
    {
        int removedChestCount = 0;
        Rectangle area = new Rectangle(x, y, x2 - x, y2 - y);

        foreach (Chest chest in Main.chest)
        {
            if (chest != null && area.Contains(chest.x, chest.y) &&
               (!emptyOnly || chest.item.All(i => i == null || i.IsAir)))
            {
                removedChestCount++;
                Chest.DestroyChest(chest.x, chest.y);
            }
        }

        return removedChestCount;
    }

    public static void ClearObjects(int x, int y, int x2, int y2)
    {
        ClearSigns(x, y, x2, y2, emptyOnly: false);
        ClearChests(x, y, x2, y2, emptyOnly: false);
        for (int tileX = x; tileX <= x2; tileX++)
        {
            for (int tileY = y; tileY <= y2; tileY++)
            {
                if (TEItemFrame.Find(tileX, tileY) != -1)
                {
                    TEItemFrame.Kill(tileX, tileY);
                }
                if (TELogicSensor.Find(tileX, tileY) != -1)
                {
                    TELogicSensor.Kill(tileX, tileY);
                }
                if (TETrainingDummy.Find(tileX, tileY) != -1)
                {
                    TETrainingDummy.Kill(tileX, tileY);
                }
                if (TEWeaponsRack.Find(tileX, tileY) != -1)
                {
                    TEWeaponsRack.Kill(tileX, tileY);
                }
                if (TETeleportationPylon.Find(tileX, tileY) != -1)
                {
                    TETeleportationPylon.Kill(tileX, tileY);
                }
                if (TEDisplayDoll.Find(tileX, tileY) != -1)
                {
                    TEDisplayDoll.Kill(tileX, tileY);
                }
                if (TEHatRack.Find(tileX, tileY) != -1)
                {
                    TEHatRack.Kill(tileX, tileY);
                }
                if (TEFoodPlatter.Find(tileX, tileY) != -1)
                {
                    TEFoodPlatter.Kill(tileX, tileY);
                }
				var position = new Point16(tileX, tileY);
				if (TileEntity.ByPosition.TryGetValue(position, out TileEntity entity) &&
					entity is TEDeadCellsDisplayJar)
				{
					TileEntity.ByID.Remove(entity.ID);
					TileEntity.ByPosition.Remove(position);
				}
            }
        }
    }

    public static void LoadWorldSection(string path, int? X = null, int? Y = null, bool Tiles = true)
    {
        LoadWorldSection(LoadWorldData(path), X, Y, Tiles);
    }

    public static void LoadWorldSection(WorldSectionData Data, int? X = null, int? Y = null, bool Tiles = true)
    {
        int targetX = X ?? Data.X;
        int targetY = Y ?? Data.Y;
        if (Tiles)
        {
            for (int sourceX = 0; sourceX < Data.Width; sourceX++)
            {
                for (int sourceY = 0; sourceY < Data.Height; sourceY++)
                {
                    int worldX = sourceX + targetX;
                    int worldY = sourceY + targetY;
                    if (InMapBoundaries(worldX, worldY))
                    {
                        Main.tile[worldX, worldY] = Data.Tiles[sourceX, sourceY];
                        Main.tile[worldX, worldY].skipLiquid(true);
                    }
                }
            }
        }
		ClearObjects(targetX, targetY, targetX + Data.Width - 1, targetY + Data.Height - 1);
		foreach (WorldSectionData.SignData sign in Data.Signs)
		{
			int signX = sign.X + targetX;
			int signY = sign.Y + targetY;
			if (!InMapBoundaries(signX, signY))
			{
				continue;
			}
			int signIndex = Sign.ReadSign(signX, signY, true);
			if (signIndex != -1)
            {
                Sign.TextSign(signIndex, sign.Text);
            }
        }
        foreach (WorldSectionData.DisplayItemData itemFrame in Data.ItemFrames)
        {
            int entityId = TEItemFrame.Place(itemFrame.X + targetX, itemFrame.Y + targetY);
            if (entityId != -1)
            {
                TEItemFrame placedItemFrame = (TEItemFrame)TileEntity.ByID[entityId];
                if (InMapBoundaries(placedItemFrame.Position.X, placedItemFrame.Position.Y))
                {
                    placedItemFrame.item = itemFrame.Item.ToItem();
                }
            }
        }
		foreach (WorldSectionData.ChestData chest in Data.Chests)
		{
			int chestX = chest.X + targetX;
			int chestY = chest.Y + targetY;
			if (!InMapBoundaries(chestX, chestY))
			{
				continue;
			}
			int chestIndex;
            if ((chestIndex = Chest.FindChest(chestX, chestY)) == -1 && (chestIndex = Chest.CreateChest(chestX, chestY, -1)) == -1)
            {
                continue;
            }
			RestoreItems(Main.chest[chestIndex].item, chest.Items);
        }
        foreach (WorldSectionData.LogicSensorData logicSensor in Data.LogicSensors)
        {
            int entityId = TELogicSensor.Place(logicSensor.X + targetX, logicSensor.Y + targetY);
            if (entityId != -1)
            {
                TELogicSensor placedLogicSensor = (TELogicSensor)TileEntity.ByID[entityId];
                if (InMapBoundaries(placedLogicSensor.Position.X, placedLogicSensor.Position.Y))
                {
                    placedLogicSensor.logicCheck = logicSensor.Type;
                }
            }
        }
        foreach (WorldSectionData.PositionData trainingDummy in Data.TrainingDummies)
        {
            int entityId = TETrainingDummy.Place(trainingDummy.X + targetX, trainingDummy.Y + targetY);
            if (entityId != -1)
            {
                TETrainingDummy placedTrainingDummy = (TETrainingDummy)TileEntity.ByID[entityId];
                if (InMapBoundaries(placedTrainingDummy.Position.X, placedTrainingDummy.Position.Y))
                {
                    placedTrainingDummy.npc = -1;
                }
            }
        }
        foreach (WorldSectionData.DisplayItemData weaponsRack in Data.WeaponsRacks)
        {
            int entityId = TEWeaponsRack.Place(weaponsRack.X + targetX, weaponsRack.Y + targetY);
            if (entityId != -1)
            {
                TEWeaponsRack placedWeaponRack = (TEWeaponsRack)TileEntity.ByID[entityId];
                if (InMapBoundaries(placedWeaponRack.Position.X, placedWeaponRack.Position.Y))
                {
                    placedWeaponRack.item = weaponsRack.Item.ToItem();
                }
            }
        }
        foreach (WorldSectionData.PositionData teleportationPylon in Data.TeleportationPylons)
        {
            TETeleportationPylon.Place(teleportationPylon.X + targetX, teleportationPylon.Y + targetY);
        }
        foreach (WorldSectionData.DisplayItemsData displayDollData in Data.DisplayDolls)
        {
            int entityId = TEDisplayDoll.Place(displayDollData.X + targetX, displayDollData.Y + targetY);
            if (entityId == -1)
            {
                continue;
            }
            TEDisplayDoll placedDisplayDoll = (TEDisplayDoll)TileEntity.ByID[entityId];
            if (InMapBoundaries(placedDisplayDoll.Position.X, placedDisplayDoll.Position.Y))
            {
                RestoreItems(placedDisplayDoll._equip, displayDollData.Items);
                RestoreItems(placedDisplayDoll._dyes, displayDollData.Dyes);
            }
        }
        foreach (WorldSectionData.DisplayItemsData hatRack in Data.HatRacks)
        {
            int entityId = TEHatRack.Place(hatRack.X + targetX, hatRack.Y + targetY);
            if (entityId == -1)
            {
                continue;
            }
            TEHatRack placedHatRack = (TEHatRack)TileEntity.ByID[entityId];
            if (InMapBoundaries(placedHatRack.Position.X, placedHatRack.Position.Y))
            {
                RestoreItems(placedHatRack._items, hatRack.Items);
                RestoreItems(placedHatRack._dyes, hatRack.Dyes);
            }
        }
		foreach (WorldSectionData.DisplayDollExtraData displayDollExtra in Data.DisplayDollExtras)
		{
			int entityId = TEDisplayDoll.Find(displayDollExtra.X + targetX, displayDollExtra.Y + targetY);
			if (entityId != -1 && TileEntity.ByID[entityId] is TEDisplayDoll displayDoll)
			{
				RestoreItems(displayDoll._misc, displayDollExtra.Misc);
				displayDoll._pose = displayDollExtra.Pose;
			}
		}
        foreach (WorldSectionData.DisplayItemData foodPlatter in Data.FoodPlatters)
        {
            int entityId = TEFoodPlatter.Place(foodPlatter.X + targetX, foodPlatter.Y + targetY);
            if (entityId != -1)
            {
                TEFoodPlatter placedFoodPlatter = (TEFoodPlatter)TileEntity.ByID[entityId];
                if (InMapBoundaries(placedFoodPlatter.Position.X, placedFoodPlatter.Position.Y))
                {
                    placedFoodPlatter.item = foodPlatter.Item.ToItem();
                }
            }
        }
		foreach (WorldSectionData.DisplayItemData displayJar in Data.DisplayJars)
		{
			int jarX = displayJar.X + targetX;
			int jarY = displayJar.Y + targetY;
			int entityId = TEDeadCellsDisplayJar.Hook_AfterPlacement(jarX, jarY, TileID.DeadCellsDisplayJar, 0, 0, 0);
			if (entityId != -1 && TileEntity.ByID[entityId] is TEDeadCellsDisplayJar jar)
			{
				jar.item = displayJar.Item.ToItem();
			}
		}
		ResetSection(targetX, targetY, targetX + Data.Width - 1, targetY + Data.Height - 1);
    }

    public static void PrepareUndo(int x, int y, int x2, int y2, TSPlayer plr)
    {
        if (WorldEdit.Config.DisableUndoSystemForUnrealPlayers && !plr.RealPlayer)
        {
            return;
        }
        if (WorldEdit.Database.GetSqlType() == SqlType.Mysql)
        {
            WorldEdit.Database.Query("INSERT IGNORE INTO WorldEdit VALUES (@0, -1, -1)", plr.Account.ID);
        }
        else
        {
            WorldEdit.Database.Query("INSERT OR IGNORE INTO WorldEdit VALUES (@0, 0, 0)", plr.Account.ID);
        }
        WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = -1 WHERE Account = @0", plr.Account.ID);
        WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = UndoLevel + 1 WHERE Account = @0", plr.Account.ID);
        int undoLevel = 0;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT UndoLevel FROM WorldEdit WHERE Account = @0", plr.Account.ID))
        {
            if (queryResult.Read())
            {
                undoLevel = queryResult.Get<int>("UndoLevel");
            }
        }
        string undoPath = Path.Combine("worldedit", $"undo-{Main.worldID}-{plr.Account.ID}-{undoLevel}.dat");
        SaveWorldSection(x, y, x2, y2, undoPath);
        foreach (string item in Directory.EnumerateFiles("worldedit", $"redo-{Main.worldID}-{plr.Account.ID}-*.dat"))
        {
            File.Delete(item);
        }
        File.Delete(Path.Combine("worldedit", $"undo-{Main.worldID}-{plr.Account.ID}-{undoLevel - MAX_UNDOS}.dat"));
    }

    public static bool Redo(int accountID)
    {
        if (WorldEdit.Config.DisableUndoSystemForUnrealPlayers && accountID == 0)
        {
            return false;
        }
        int redoLevel = 0;
        int undoLevel = 0;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT RedoLevel, UndoLevel FROM WorldEdit WHERE Account = @0", accountID))
        {
            if (!queryResult.Read())
            {
                return false;
            }
            redoLevel = queryResult.Get<int>("RedoLevel") - 1;
            undoLevel = queryResult.Get<int>("UndoLevel") + 1;
        }
        if (redoLevel < -1)
        {
            return false;
        }
        string redoPath = Path.Combine("worldedit", $"redo-{Main.worldID}-{accountID}-{redoLevel + 1}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = @0 WHERE Account = @1", redoLevel, accountID);
        if (!File.Exists(redoPath))
        {
            return false;
        }
        string undoPath = Path.Combine("worldedit", $"undo-{Main.worldID}-{accountID}-{undoLevel}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = @0 WHERE Account = @1", undoLevel, accountID);
        Rectangle redoArea = ReadSize(redoPath);
        SaveWorldSection(Math.Max(0, redoArea.X), Math.Max(0, redoArea.Y), Math.Min(redoArea.X + redoArea.Width - 1, Main.maxTilesX - 1), Math.Min(redoArea.Y + redoArea.Height - 1, Main.maxTilesY - 1), undoPath);
        LoadWorldSection(redoPath);
        File.Delete(redoPath);
        return true;
    }

    public static void ResetSection(int x, int y, int x2, int y2)
    {
        int sectionX = Netplay.GetSectionX(x);
        int sectionX2 = Netplay.GetSectionX(x2);
        int sectionY = Netplay.GetSectionY(y);
        int sectionY2 = Netplay.GetSectionY(y2);
        foreach (RemoteClient item in Netplay.Clients.Where((RemoteClient s) => s.IsActive))
        {
            int length = item.TileSections.GetLength(0);
			int sectionRowCount = item.TileSections.GetLength(1);
            for (int i = sectionX; i <= sectionX2; i++)
            {
                for (int j = sectionY; j <= sectionY2; j++)
                {
					if (i >= 0 && j >= 0 && i < length && j < sectionRowCount)
                    {
                        item.TileSections[i, j] = false;
                    }
                }
            }
        }
    }

    public static void SaveWorldSection(int x, int y, int x2, int y2, string path)
    {
        SaveWorldSection(x, y, x2, y2).Write(path);
    }

    public static void Write(this BinaryWriter writer, ITile tile)
    {
        writer.Write(tile.sTileHeader);
        writer.Write(tile.bTileHeader);
        writer.Write(tile.bTileHeader2);
        if (tile.active())
        {
            writer.Write(tile.type);
            if (Main.tileFrameImportant[tile.type])
            {
                writer.Write(tile.frameX);
                writer.Write(tile.frameY);
            }
        }
        writer.Write(tile.wall);
        writer.Write(tile.liquid);
    }

    public static void Write(this BinaryWriter writer, NetItem item)
    {
        writer.Write(item.NetId);
        writer.Write(item.Stack);
        writer.Write(item.PrefixId);
    }

    internal static void Write(this BinaryWriter writer, NetItem[] items)
    {
        writer.Write(items.Length);
        foreach (NetItem item in items)
        {
            writer.Write(item);
        }
    }

    public static WorldSectionData SaveWorldSection(int x, int y, int x2, int y2)
    {
        int width = x2 - x + 1;
        int height = y2 - y + 1;
        WorldSectionData worldSectionData = new WorldSectionData(width, height)
        {
            X = x,
            Y = y
        };
        for (int i = x; i <= x2; i++)
        {
            for (int j = y; j <= y2; j++)
            {
                worldSectionData.ProcessTile(Main.tile[i, j], i - x, j - y);
            }
        }
        return worldSectionData;
    }

    public static bool Undo(int accountID)
    {
        if (WorldEdit.Config.DisableUndoSystemForUnrealPlayers && accountID == 0)
        {
            return false;
        }
        int redoLevel;
        int undoLevel;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT RedoLevel, UndoLevel FROM WorldEdit WHERE Account = @0", accountID))
        {
            if (!queryResult.Read())
            {
                return false;
            }
            redoLevel = queryResult.Get<int>("RedoLevel") + 1;
            undoLevel = queryResult.Get<int>("UndoLevel") - 1;
        }
        if (undoLevel < -1)
        {
            return false;
        }
        string undoPath = Path.Combine("worldedit", $"undo-{Main.worldID}-{accountID}-{undoLevel + 1}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = @0 WHERE Account = @1", undoLevel, accountID);
        if (!File.Exists(undoPath))
        {
            return false;
        }
        string redoPath = Path.Combine("worldedit", $"redo-{Main.worldID}-{accountID}-{redoLevel}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = @0 WHERE Account = @1", redoLevel, accountID);
        Rectangle undoArea = ReadSize(undoPath);
        SaveWorldSection(Math.Max(0, undoArea.X), Math.Max(0, undoArea.Y), Math.Min(undoArea.X + undoArea.Width - 1, Main.maxTilesX - 1), Math.Min(undoArea.Y + undoArea.Height - 1, Main.maxTilesY - 1), redoPath);
        LoadWorldSection(undoPath);
        File.Delete(undoPath);
        return true;
    }

    public static bool CanSet(bool Tile, ITile tile, int type, Selection selection, Expression expression, MagicWand magicWand, int x, int y, TSPlayer player)
    {
        return (!Tile) ? (tile.wall != type && selection(x, y, player) && expression.Evaluate(tile) && magicWand.InSelection(x, y)) : (((type >= 0 && (!tile.active() || tile.type != type)) || (type == -1 && tile.active()) || (type == -2 && (tile.liquid == 0 || tile.liquidType() != 1)) || (type == -3 && (tile.liquid == 0 || tile.liquidType() != 2)) || (type == -4 && (tile.liquid == 0 || tile.liquidType() != 0))) && selection(x, y, player) && expression.Evaluate(tile) && magicWand.InSelection(x, y));
    }

    public static WEPoint[] CreateLine(int x1, int y1, int x2, int y2)
    {
        var list = new List<WEPoint> { new WEPoint((short) x1, (short) y1) };
        int dx = x2 - x1;
        int dy = y2 - y1;
        int stepX = dx > 0 ? 1 : dx < 0 ? -1 : 0;
        int stepY = dy > 0 ? 1 : dy < 0 ? -1 : 0;
        int absDx = Math.Abs(dx);
        int absDy = Math.Abs(dy);

        int xStep = stepX;
        int yStep = stepY;
        int error = absDx > absDy ? absDx : absDy;
        int delta = absDx > absDy ? absDy : absDx;
        int remainder = error / 2;

        int x = x1;
        int y = y1;

        for (int i = 0; i < error; i++)
        {
            remainder -= delta;
            if (remainder < 0)
            {
                remainder += error;
                x += stepX;
                y += stepY;
            }
            else
            {
                x += (absDx > absDy ? stepX : 0);
                y += (absDy > absDx ? stepY : 0);
            }
            list.Add(new WEPoint((short) x, (short) y));
        }

        return list.ToArray();
    }
    public static bool InEllipse(int x1, int y1, int x2, int y2, int x, int y)
    {
        Vector2 center = new Vector2((x1 + x2) / 2f, (y1 + y2) / 2f);
        float radiusX = Math.Abs((x2 - x1) / 2f);
        float radiusY = Math.Abs((y2 - y1) / 2f);

        float a = radiusX;
        float b = radiusY;

        if (radiusY > radiusX)
        {
            a = radiusY;
            b = radiusX;
        }

        return InEllipse(x1, y1, center.X, center.Y, a, b, x, y);
    }

    private static bool InEllipse(int x1, int y1, float cX, float cY, float rMax, float rMin, int x, int y)
    {
        return Math.Pow((float) x - cX - (float) x1, 2.0) / Math.Pow(rMax, 2.0) + Math.Pow((float) y - cY - (float) y1, 2.0) / Math.Pow(rMin, 2.0) <= 1.0;
    }

    public static WEPoint[] CreateEllipseOutline(int x1, int y1, int x2, int y2)
    {
        float radiusX = Math.Abs((x2 - x1) / 2f);
        float radiusY = Math.Abs((y2 - y1) / 2f);

        float a = radiusX;
        float b = radiusY;

        if (radiusY > radiusX)
        {
            a = radiusY;
            b = radiusX;
        }

        List<WEPoint> list = new List<WEPoint>();
        for (int i = x1; i <= x1 + (int) radiusX; i++)
        {
            for (int j = y1; j <= y1 + (int) radiusY; j++)
            {
                if (!InEllipse(x1, y1, radiusX, radiusY, a, b, i, j))
                    continue;

                if (list.Count > 0)
                {
                    WEPoint wEPoint = list.Last();
                    int fillY = j;
                    while (wEPoint.Y - fillY >= 1)
                    {
                        addPoint(list, x1, y1, x2, y2, i, fillY++);
                    }
                }
                else
                {
                    int fillLength = y1 + (int)radiusY - j;
                    if (fillLength > 0)
                    {
                        int fillY = j;
                        while (fillLength-- >= 0)
                        {
                            addPoint(list, x1, y1, x2, y2, i, fillY++);
                        }
                    }
                }

                addPoint(list, x1, y1, x2, y2, i, j);
                break;
            }
        }

        return list.ToArray();
    }

    private static void addPoint(List<WEPoint> points, int x1, int y1, int x2, int y2, int i, int j)
    {
        points.Add(new WEPoint((short) (x2 - i + x1), (short) j));
        points.Add(new WEPoint((short) i, (short) (y2 - j + y1)));
        points.Add(new WEPoint((short) (x2 - i + x1), (short) (y2 - j + y1)));
        points.Add(new WEPoint((short) i, (short) j));
    }

    public static WEPoint[,] CreateStatueText(string Text, int Width, int Height)
    {
        WEPoint[,] array = new WEPoint[Width, Height];
        if (string.IsNullOrWhiteSpace(Text))
        {
            return array;
        }
        List<Tuple<WEPoint[,], int>> list = new List<Tuple<WEPoint[,], int>>();
		string[] textRows = Text.ToLower().Replace("\\n", "\n").Split('\n');
		int requiredHeight = 0;
		for (int i = 0; i < textRows.Length; i++)
		{
			Tuple<WEPoint[,], int> tuple = CreateStatueRow(textRows[i], Width, i == 0);
            if ((requiredHeight += tuple.Item1.GetLength(1) + tuple.Item2) > Height)
            {
                break;
            }
            list.Add(tuple);
        }
        int rowOffset = 0;
        foreach (Tuple<WEPoint[,], int> item in list)
        {
            rowOffset += item.Item2;
            int length = item.Item1.GetLength(0);
			int rowHeight = item.Item1.GetLength(1);
            for (int j = 0; j < length; j++)
            {
				for (int k = 0; k < rowHeight && k + rowOffset <= Height; k++)
                {
                    array[j, k + rowOffset] = item.Item1[j, k];
                }
            }
			rowOffset += rowHeight;
        }
        return array;
    }

    private static Tuple<WEPoint[,], int> CreateStatueRow(string Row, int Width, bool FirstRow)
    {
        Tuple<string, int, int, int> tuple = RowSettings(Row, FirstRow);
        WEPoint[,] array = new WEPoint[Width, tuple.Item4];
        List<char> list = tuple.Item1.ToCharArray().ToList();
        int excessCharacterCount = (int)Math.Ceiling((double)(list.Count * 2 - Width) / 2.0);
        int horizontalOffset = 0;
        if (excessCharacterCount > 0)
        {
            list.RemoveRange(list.Count - excessCharacterCount, excessCharacterCount);
        }
        if (tuple.Item2 == 1 && list.Count * 2 <= Width)
        {
            horizontalOffset = (Width - list.Count * 2) / 2;
        }
        else if (tuple.Item2 == 2 && list.Count * 2 <= Width)
        {
            horizontalOffset = Width - list.Count * 2;
        }
        for (int i = 0; i < list.Count; i++)
        {
			WEPoint[,] letterPoints = CreateStatueLetter(list[i]);
            for (int j = 0; j < 2 && j + horizontalOffset <= Width; j++)
            {
                for (int k = 0; k < tuple.Item4; k++)
                {
					array[horizontalOffset, k] = letterPoints[j, k];
                }
                horizontalOffset++;
            }
        }
        return new Tuple<WEPoint[,], int>(array, tuple.Item3);
    }

    private static Tuple<string, int, int, int> RowSettings(string Row, bool FirstRow)
    {
        int item = 0;
        int result = ((!FirstRow) ? 1 : 0);
		int statueHeight = 3;
        while (Row.StartsWith("\\") && Row.Length > 1)
        {
            switch (char.ToLower(Row[1]))
            {
                case 'l':
                    item = 0;
                    Row = Row.Substring(2);
                    break;
                case 'm':
                    item = 1;
                    Row = Row.Substring(2);
                    break;
                case 'r':
                    item = 2;
                    Row = Row.Substring(2);
                    break;
                case 'c':
					statueHeight = 2;
                    Row = Row.Substring(2);
                    break;
                case 's':
                {
                    Row = Row.Substring(2);
                    string text = "";
                    int digitCount = 0;
                    while (Row.Length > digitCount + 1 && char.IsDigit(Row[digitCount]))
                    {
                        text += Row[digitCount++];
                    }
                    Row = Row.Substring(digitCount);
                    if (!int.TryParse(text, out result) || result < 0)
                    {
                        result = ((!FirstRow) ? 1 : 0);
                    }
                    break;
                }
            }
        }
		return new Tuple<string, int, int, int>(Row, item, result, statueHeight);
    }

    private static WEPoint[,] CreateStatueLetter(char Letter)
    {
        WEPoint[,] array = new WEPoint[2, 3];
        short letterColumn = 0;
        short spriteX;
        if (Letter > '/' && Letter < ':')
        {
            spriteX = (short)((Letter - 48) * 36);
        }
        else
        {
            if (Letter <= '`' || Letter >= '{')
            {
                return array;
            }
            spriteX = (short)((Letter - 87) * 36);
        }
        for (short frameX = spriteX; frameX <= spriteX + 18; frameX += 18)
        {
            int letterRow = 0;
            for (short frameY = 0; frameY <= 36; frameY += 18)
            {
                array[letterColumn, letterRow++] = new WEPoint(frameX, frameY);
            }
            letterColumn++;
        }
        return array;
    }
}
