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

        bool flag = true;
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
            flag = false;
        }

        if (!flag)
        {
            File.Copy(tempPath, path, true); // 简化参数名
        }

        File.Delete(tempPath); // 直接使用临时路径变量
        return flag;
    }

    public static bool InMapBoundaries(int X, int Y)
    {
        return X >= 0 && Y >= 0 && X < Main.maxTilesX && Y < Main.maxTilesY;
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
        foreach (KeyValuePair<string, int> color2 in WorldEdit.Colors)
        {
            if (color2.Key == color)
            {
                return new List<int> { color2.Value };
            }
            if (color2.Key.StartsWith(color))
            {
                list.Add(color2.Value);
            }
        }
        return list;
    }

    public static List<int> GetTileID(string tile)
    {
        if (int.TryParse(tile, out int result) && result >= 0 && result < 693) // 合并变量声明
        {
            return new List<int> { result };
        }

        List<int> list = new List<int>();
        foreach (KeyValuePair<string, int> tile2 in WorldEdit.Tiles)
        {
            if (tile2.Key == tile)
            {
                return new List<int> { tile2.Value };
            }
            if (tile2.Key.StartsWith(tile))
            {
                list.Add(tile2.Value);
            }
        }
        return list;
    }

    public static List<int> GetWallID(string wall)
    {
        if (int.TryParse(wall, out int result) && result >= 0 && result < 347) // 合并变量声明
        {
            return new List<int> { result };
        }

        List<int> list = new List<int>();
        foreach (KeyValuePair<string, int> wall2 in WorldEdit.Walls)
        {
            if (wall2.Key == wall)
            {
                return new List<int> { wall2.Value };
            }
            if (wall2.Key.StartsWith(wall))
            {
                list.Add(wall2.Value);
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

    public static WorldSectionData LoadWorldData(Stream stream)
    {
        int x;
        int y;
        int num;
        int num2;
        using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
        {
            x = binaryReader.ReadInt32();
            y = binaryReader.ReadInt32();
            num = binaryReader.ReadInt32();
            num2 = binaryReader.ReadInt32();
        }
        using BinaryReader binaryReader2 = new BinaryReader(new BufferedStream(new GZipStream(stream, CompressionMode.Decompress), 1048576));
        WorldSectionData worldSectionData = new WorldSectionData(num, num2)
        {
            X = x,
            Y = y
        };
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2; j++)
            {
                worldSectionData.Tiles[i, j] = (ITile) (object) binaryReader2.ReadTile();
            }
        }
        try
        {
            int num3 = binaryReader2.ReadInt32();
            worldSectionData.Signs = new WorldSectionData.SignData[num3];
            for (int k = 0; k < num3; k++)
            {
                worldSectionData.Signs[k] = WorldSectionData.SignData.Read(binaryReader2);
            }
            int num4 = binaryReader2.ReadInt32();
            worldSectionData.Chests = new WorldSectionData.ChestData[num4];
            for (int l = 0; l < num4; l++)
            {
                worldSectionData.Chests[l] = WorldSectionData.ChestData.Read(binaryReader2);
            }
            int num5 = binaryReader2.ReadInt32();
            worldSectionData.ItemFrames = new WorldSectionData.DisplayItemData[num5];
            for (int m = 0; m < num5; m++)
            {
                worldSectionData.ItemFrames[m] = WorldSectionData.DisplayItemData.Read(binaryReader2);
            }
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
            int num6 = binaryReader2.ReadInt32();
            worldSectionData.LogicSensors = new WorldSectionData.LogicSensorData[num6];
            for (int n = 0; n < num6; n++)
            {
                worldSectionData.LogicSensors[n] = WorldSectionData.LogicSensorData.Read(binaryReader2);
            }
            int num7 = binaryReader2.ReadInt32();
            worldSectionData.TrainingDummies = new WorldSectionData.PositionData[num7];
            for (int num8 = 0; num8 < num7; num8++)
            {
                worldSectionData.TrainingDummies[num8] = WorldSectionData.PositionData.Read(binaryReader2);
            }
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
            int num9 = binaryReader2.ReadInt32();
            worldSectionData.WeaponsRacks = new WorldSectionData.DisplayItemData[num9];
            for (int num10 = 0; num10 < num9; num10++)
            {
                worldSectionData.WeaponsRacks[num10] = WorldSectionData.DisplayItemData.Read(binaryReader2);
            }
            int num11 = binaryReader2.ReadInt32();
            worldSectionData.TeleportationPylons = new WorldSectionData.PositionData[num11];
            for (int num12 = 0; num12 < num11; num12++)
            {
                worldSectionData.TeleportationPylons[num12] = WorldSectionData.PositionData.Read(binaryReader2);
            }
            int num13 = binaryReader2.ReadInt32();
            worldSectionData.DisplayDolls = new WorldSectionData.DisplayItemsData[num13];
            for (int num14 = 0; num14 < num13; num14++)
            {
                worldSectionData.DisplayDolls[num14] = WorldSectionData.DisplayItemsData.Read(binaryReader2);
            }
            int num15 = binaryReader2.ReadInt32();
            worldSectionData.HatRacks = new WorldSectionData.DisplayItemsData[num15];
            for (int num16 = 0; num16 < num15; num16++)
            {
                worldSectionData.HatRacks[num16] = WorldSectionData.DisplayItemsData.Read(binaryReader2);
            }
            int num17 = binaryReader2.ReadInt32();
            worldSectionData.FoodPlatters = new WorldSectionData.DisplayItemData[num17];
            for (int num18 = 0; num18 < num17; num18++)
            {
                worldSectionData.FoodPlatters[num18] = WorldSectionData.DisplayItemData.Read(binaryReader2);
            }
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
        int x = binaryReader.ReadInt32();
        int y = binaryReader.ReadInt32();
        int num = binaryReader.ReadInt32();
        int num2 = binaryReader.ReadInt32();
        WorldSectionData worldSectionData = new WorldSectionData(num, num2)
        {
            X = x,
            Y = y
        };
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2; j++)
            {
                worldSectionData.Tiles[i, j] = (ITile) (object) binaryReader.ReadTileOld();
            }
        }
        try
        {
            int num3 = binaryReader.ReadInt32();
            worldSectionData.Signs = new WorldSectionData.SignData[num3];
            for (int k = 0; k < num3; k++)
            {
                worldSectionData.Signs[k] = WorldSectionData.SignData.Read(binaryReader);
            }
            int num4 = binaryReader.ReadInt32();
            worldSectionData.Chests = new WorldSectionData.ChestData[num4];
            for (int l = 0; l < num4; l++)
            {
                worldSectionData.Chests[l] = WorldSectionData.ChestData.Read(binaryReader);
            }
            int num5 = binaryReader.ReadInt32();
            worldSectionData.ItemFrames = new WorldSectionData.DisplayItemData[num5];
            for (int m = 0; m < num5; m++)
            {
                worldSectionData.ItemFrames[m] = WorldSectionData.DisplayItemData.Read(binaryReader);
            }
        }
        catch (EndOfStreamException)
        {
        }
        try
        {
            int num6 = binaryReader.ReadInt32();
            worldSectionData.LogicSensors = new WorldSectionData.LogicSensorData[num6];
            for (int n = 0; n < num6; n++)
            {
                worldSectionData.LogicSensors[n] = WorldSectionData.LogicSensorData.Read(binaryReader);
            }
            int num7 = binaryReader.ReadInt32();
            worldSectionData.TrainingDummies = new WorldSectionData.PositionData[num7];
            for (int num8 = 0; num8 < num7; num8++)
            {
                worldSectionData.TrainingDummies[num8] = WorldSectionData.PositionData.Read(binaryReader);
            }
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
        int num = reader.ReadInt32();
        NetItem[] array = new NetItem[num];
        for (int i = 0; i < num; i++)
        {
            array[i] = reader.ReadNetItem();
        }
        return array;
    }

    public static int ClearSigns(int x, int y, int x2, int y2, bool emptyOnly)
    {
        int num = 0;
        Rectangle area = new Rectangle(x, y, x2 - x, y2 - y);

        foreach (Sign sign in Main.sign)
        {
            if (sign != null && area.Contains(sign.x, sign.y) && (!emptyOnly || string.IsNullOrWhiteSpace(sign.text)))
            {
                num++;
                Sign.KillSign(sign.x, sign.y);
            }
        }

        return num;
    }

    public static int ClearChests(int x, int y, int x2, int y2, bool emptyOnly)
    {
        int num = 0;
        Rectangle area = new Rectangle(x, y, x2 - x, y2 - y);

        foreach (Chest chest in Main.chest)
        {
            if (chest != null && area.Contains(chest.x, chest.y) &&
               (!emptyOnly || chest.item.All(i => i == null || i.netID == 0)))
            {
                num++;
                Chest.DestroyChest(chest.x, chest.y);
            }
        }

        return num;
    }

    public static void ClearObjects(int x, int y, int x2, int y2)
    {
        ClearSigns(x, y, x2, y2, emptyOnly: false);
        ClearChests(x, y, x2, y2, emptyOnly: false);
        for (int i = x; i <= x2; i++)
        {
            for (int j = y; j <= y2; j++)
            {
                if (TEItemFrame.Find(i, j) != -1)
                {
                    TEItemFrame.Kill(i, j);
                }
                if (TELogicSensor.Find(i, j) != -1)
                {
                    TELogicSensor.Kill(i, j);
                }
                if (TETrainingDummy.Find(i, j) != -1)
                {
                    TETrainingDummy.Kill(i, j);
                }
                if (TEWeaponsRack.Find(i, j) != -1)
                {
                    TEWeaponsRack.Kill(i, j);
                }
                if (TETeleportationPylon.Find(i, j) != -1)
                {
                    TETeleportationPylon.Kill(i, j);
                }
                if (TEDisplayDoll.Find(i, j) != -1)
                {
                    TEDisplayDoll.Kill(i, j);
                }
                if (TEHatRack.Find(i, j) != -1)
                {
                    TEHatRack.Kill(i, j);
                }
                if (TEFoodPlatter.Find(i, j) != -1)
                {
                    TEFoodPlatter.Kill(i, j);
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
        int num = X ?? Data.X;
        int num2 = Y ?? Data.Y;
        if (Tiles)
        {
            for (int i = 0; i < Data.Width; i++)
            {
                for (int j = 0; j < Data.Height; j++)
                {
                    int num3 = i + num;
                    int num4 = j + num2;
                    if (InMapBoundaries(num3, num4))
                    {
                        Main.tile[num3, num4] = Data.Tiles[i, j];
                        Main.tile[num3, num4].skipLiquid(true);
                    }
                }
            }
        }
        ClearObjects(num, num2, num + Data.Width, num2 + Data.Height);
        foreach (WorldSectionData.SignData sign in Data.Signs)
        {
            int num5 = Sign.ReadSign(sign.X + num, sign.Y + num2, true);
            if (num5 != -1 && InMapBoundaries(sign.X, sign.Y))
            {
                Sign.TextSign(num5, sign.Text);
            }
        }
        foreach (WorldSectionData.DisplayItemData itemFrame in Data.ItemFrames)
        {
            int num6 = TEItemFrame.Place(itemFrame.X + num, itemFrame.Y + num2);
            if (num6 != -1)
            {
                TEItemFrame val = (TEItemFrame) TileEntity.ByID[num6];
                if (InMapBoundaries(((TileEntity) val).Position.X, ((TileEntity) val).Position.Y))
                {
                    val.item = new Item();
                    Item item = val.item;
                    NetItem item2 = itemFrame.Item;
                    item.netDefaults(item2.NetId);
                    Item item3 = val.item;
                    item2 = itemFrame.Item;
                    item3.stack = item2.Stack;
                    Item item4 = val.item;
                    item2 = itemFrame.Item;
                    item4.prefix = item2.PrefixId;
                }
            }
        }
        foreach (WorldSectionData.ChestData chest in Data.Chests)
        {
            int num7 = chest.X + num;
            int num8 = chest.Y + num2;
            int num9;
            if ((num9 = Chest.FindChest(num7, num8)) == -1 && (num9 = Chest.CreateChest(num7, num8, -1)) == -1)
            {
                continue;
            }
            Chest val2 = Main.chest[num9];
            if (InMapBoundaries(chest.X, chest.Y))
            {
                for (int k = 0; k < chest.Items.Length; k++)
                {
                    NetItem netItem = chest.Items[k];
                    Item val3 = new Item();
                    val3.netDefaults(netItem.NetId);
                    val3.stack = netItem.Stack;
                    val3.prefix = netItem.PrefixId;
                    Main.chest[num9].item[k] = val3;
                }
            }
        }
        foreach (WorldSectionData.LogicSensorData logicSensor in Data.LogicSensors)
        {
            int num10 = TELogicSensor.Place(logicSensor.X + num, logicSensor.Y + num2);
            if (num10 != -1)
            {
                TELogicSensor val4 = (TELogicSensor) TileEntity.ByID[num10];
                if (InMapBoundaries(((TileEntity) val4).Position.X, ((TileEntity) val4).Position.Y))
                {
                    val4.logicCheck = logicSensor.Type;
                }
            }
        }
        foreach (WorldSectionData.PositionData trainingDummy in Data.TrainingDummies)
        {
            int num11 = TETrainingDummy.Place(trainingDummy.X + num, trainingDummy.Y + num2);
            if (num11 != -1)
            {
                TETrainingDummy val5 = (TETrainingDummy) TileEntity.ByID[num11];
                if (InMapBoundaries(((TileEntity) val5).Position.X, ((TileEntity) val5).Position.Y))
                {
                    val5.npc = -1;
                }
            }
        }
        foreach (WorldSectionData.DisplayItemData weaponsRack in Data.WeaponsRacks)
        {
            int num12 = TEWeaponsRack.Place(weaponsRack.X + num, weaponsRack.Y + num2);
            if (num12 != -1)
            {
                TEWeaponsRack val6 = (TEWeaponsRack) TileEntity.ByID[num12];
                if (InMapBoundaries(((TileEntity) val6).Position.X, ((TileEntity) val6).Position.Y))
                {
                    val6.item = new Item();
                    Item item5 = val6.item;
                    NetItem item2 = weaponsRack.Item;
                    item5.netDefaults(item2.NetId);
                    Item item6 = val6.item;
                    item2 = weaponsRack.Item;
                    item6.stack = item2.Stack;
                    Item item7 = val6.item;
                    item2 = weaponsRack.Item;
                    item7.prefix = item2.PrefixId;
                }
            }
        }
        foreach (WorldSectionData.PositionData teleportationPylon in Data.TeleportationPylons)
        {
            TETeleportationPylon.Place(teleportationPylon.X + num, teleportationPylon.Y + num2);
        }
        foreach (WorldSectionData.DisplayItemsData displayDoll in Data.DisplayDolls)
        {
            int num13 = TEDisplayDoll.Place(displayDoll.X + num, displayDoll.Y + num2);
            if (num13 == -1)
            {
                continue;
            }
            TEDisplayDoll val7 = (TEDisplayDoll) TileEntity.ByID[num13];
            if (InMapBoundaries(((TileEntity) val7).Position.X, ((TileEntity) val7).Position.Y))
            {
                val7._items = (Item[]) (object) new Item[displayDoll.Items.Length];
                for (int l = 0; l < displayDoll.Items.Length; l++)
                {
                    NetItem netItem2 = displayDoll.Items[l];
                    Item val8 = new Item();
                    val8.netDefaults(netItem2.NetId);
                    val8.stack = netItem2.Stack;
                    val8.prefix = netItem2.PrefixId;
                    val7._items[l] = val8;
                }
                val7._dyes = (Item[]) (object) new Item[displayDoll.Dyes.Length];
                for (int m = 0; m < displayDoll.Dyes.Length; m++)
                {
                    NetItem netItem3 = displayDoll.Dyes[m];
                    Item val9 = new Item();
                    val9.netDefaults(netItem3.NetId);
                    val9.stack = netItem3.Stack;
                    val9.prefix = netItem3.PrefixId;
                    val7._dyes[m] = val9;
                }
            }
        }
        foreach (WorldSectionData.DisplayItemsData hatRack in Data.HatRacks)
        {
            int num14 = TEHatRack.Place(hatRack.X + num, hatRack.Y + num2);
            if (num14 == -1)
            {
                continue;
            }
            TEHatRack val10 = (TEHatRack) TileEntity.ByID[num14];
            if (InMapBoundaries(((TileEntity) val10).Position.X, ((TileEntity) val10).Position.Y))
            {
                val10._items = (Item[]) (object) new Item[hatRack.Items.Length];
                for (int n = 0; n < hatRack.Items.Length; n++)
                {
                    NetItem netItem4 = hatRack.Items[n];
                    Item val11 = new Item();
                    val11.netDefaults(netItem4.NetId);
                    val11.stack = netItem4.Stack;
                    val11.prefix = netItem4.PrefixId;
                    val10._items[n] = val11;
                }
                val10._dyes = (Item[]) (object) new Item[hatRack.Dyes.Length];
                for (int num15 = 0; num15 < hatRack.Dyes.Length; num15++)
                {
                    NetItem netItem5 = hatRack.Dyes[num15];
                    Item val12 = new Item();
                    val12.netDefaults(netItem5.NetId);
                    val12.stack = netItem5.Stack;
                    val12.prefix = netItem5.PrefixId;
                    val10._dyes[num15] = val12;
                }
            }
        }
        foreach (WorldSectionData.DisplayItemData foodPlatter in Data.FoodPlatters)
        {
            int num16 = TEFoodPlatter.Place(foodPlatter.X + num, foodPlatter.Y + num2);
            if (num16 != -1)
            {
                TEFoodPlatter val13 = (TEFoodPlatter) TileEntity.ByID[num16];
                if (InMapBoundaries(((TileEntity) val13).Position.X, ((TileEntity) val13).Position.Y))
                {
                    val13.item = new Item();
                    Item item8 = val13.item;
                    NetItem item2 = foodPlatter.Item;
                    item8.netDefaults(item2.NetId);
                    Item item9 = val13.item;
                    item2 = foodPlatter.Item;
                    item9.stack = item2.Stack;
                    Item item10 = val13.item;
                    item2 = foodPlatter.Item;
                    item10.prefix = item2.PrefixId;
                }
            }
        }
        ResetSection(num, num2, num + Data.Width, num2 + Data.Height);
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
        int num = 0;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT UndoLevel FROM WorldEdit WHERE Account = @0", plr.Account.ID))
        {
            if (queryResult.Read())
            {
                num = queryResult.Get<int>("UndoLevel");
            }
        }
        string path = Path.Combine("worldedit", $"undo-{Main.worldID}-{plr.Account.ID}-{num}.dat");
        SaveWorldSection(x, y, x2, y2, path);
        foreach (string item in Directory.EnumerateFiles("worldedit", $"redo-{Main.worldID}-{plr.Account.ID}-*.dat"))
        {
            File.Delete(item);
        }
        File.Delete(Path.Combine("worldedit", $"undo-{Main.worldID}-{plr.Account.ID}-{num - MAX_UNDOS}.dat"));
    }

    public static bool Redo(int accountID)
    {
        if (WorldEdit.Config.DisableUndoSystemForUnrealPlayers && accountID == 0)
        {
            return false;
        }
        int num = 0;
        int num2 = 0;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT RedoLevel, UndoLevel FROM WorldEdit WHERE Account = @0", accountID))
        {
            if (!queryResult.Read())
            {
                return false;
            }
            num = queryResult.Get<int>("RedoLevel") - 1;
            num2 = queryResult.Get<int>("UndoLevel") + 1;
        }
        if (num < -1)
        {
            return false;
        }
        string path = Path.Combine("worldedit", $"redo-{Main.worldID}-{accountID}-{num + 1}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = @0 WHERE Account = @1", num, accountID);
        if (!File.Exists(path))
        {
            return false;
        }
        string path2 = Path.Combine("worldedit", $"undo-{Main.worldID}-{accountID}-{num2}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = @0 WHERE Account = @1", num2, accountID);
        Rectangle val = ReadSize(path);
        SaveWorldSection(Math.Max(0, val.X), Math.Max(0, val.Y), Math.Min(val.X + val.Width - 1, Main.maxTilesX - 1), Math.Min(val.Y + val.Height - 1, Main.maxTilesY - 1), path2);
        LoadWorldSection(path);
        File.Delete(path);
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
            int length2 = item.TileSections.GetLength(1);
            for (int i = sectionX; i <= sectionX2; i++)
            {
                for (int j = sectionY; j <= sectionY2; j++)
                {
                    if (i >= 0 && j >= 0 && i < length && j < length2)
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
        int num;
        int num2;
        using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT RedoLevel, UndoLevel FROM WorldEdit WHERE Account = @0", accountID))
        {
            if (!queryResult.Read())
            {
                return false;
            }
            num = queryResult.Get<int>("RedoLevel") + 1;
            num2 = queryResult.Get<int>("UndoLevel") - 1;
        }
        if (num2 < -1)
        {
            return false;
        }
        string path = Path.Combine("worldedit", $"undo-{Main.worldID}-{accountID}-{num2 + 1}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = @0 WHERE Account = @1", num2, accountID);
        if (!File.Exists(path))
        {
            return false;
        }
        string path2 = Path.Combine("worldedit", $"redo-{Main.worldID}-{accountID}-{num}.dat");
        WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = @0 WHERE Account = @1", num, accountID);
        Rectangle val = ReadSize(path);
        SaveWorldSection(Math.Max(0, val.X), Math.Max(0, val.Y), Math.Min(val.X + val.Width - 1, Main.maxTilesX - 1), Math.Min(val.Y + val.Height - 1, Main.maxTilesY - 1), path2);
        LoadWorldSection(path);
        File.Delete(path);
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
                    int num4 = j;
                    while (wEPoint.Y - num4 >= 1)
                    {
                        addPoint(list, x1, y1, x2, y2, i, num4++);
                    }
                }
                else
                {
                    int num5 = y1 + (int) radiusY - j;
                    if (num5 > 0)
                    {
                        int num6 = j;
                        while (num5-- >= 0)
                        {
                            addPoint(list, x1, y1, x2, y2, i, num6++);
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
        string[] array2 = Text.ToLower().Replace("\\n", "\n").Split('\n');
        int num = 0;
        for (int i = 0; i < array2.Length; i++)
        {
            Tuple<WEPoint[,], int> tuple = CreateStatueRow(array2[i], Width, i == 0);
            if ((num += tuple.Item1.GetLength(1) + tuple.Item2) > Height)
            {
                break;
            }
            list.Add(tuple);
        }
        int num2 = 0;
        foreach (Tuple<WEPoint[,], int> item in list)
        {
            num2 += item.Item2;
            int length = item.Item1.GetLength(0);
            int length2 = item.Item1.GetLength(1);
            for (int j = 0; j < length; j++)
            {
                for (int k = 0; k < length2 && k + num2 <= Height; k++)
                {
                    array[j, k + num2] = item.Item1[j, k];
                }
            }
            num2 += length2;
        }
        return array;
    }

    private static Tuple<WEPoint[,], int> CreateStatueRow(string Row, int Width, bool FirstRow)
    {
        Tuple<string, int, int, int> tuple = RowSettings(Row, FirstRow);
        WEPoint[,] array = new WEPoint[Width, tuple.Item4];
        List<char> list = tuple.Item1.ToCharArray().ToList();
        int num = (int) Math.Ceiling((double) (list.Count * 2 - Width) / 2.0);
        int num2 = 0;
        if (num > 0)
        {
            list.RemoveRange(list.Count - num, num);
        }
        if (tuple.Item2 == 1 && list.Count * 2 <= Width)
        {
            num2 = (Width - list.Count * 2) / 2;
        }
        else if (tuple.Item2 == 2 && list.Count * 2 <= Width)
        {
            num2 = Width - list.Count * 2;
        }
        for (int i = 0; i < list.Count; i++)
        {
            WEPoint[,] array2 = CreateStatueLetter(list[i]);
            for (int j = 0; j < 2 && j + num2 <= Width; j++)
            {
                for (int k = 0; k < tuple.Item4; k++)
                {
                    array[num2, k] = array2[j, k];
                }
                num2++;
            }
        }
        return new Tuple<WEPoint[,], int>(array, tuple.Item3);
    }

    private static Tuple<string, int, int, int> RowSettings(string Row, bool FirstRow)
    {
        int item = 0;
        int result = ((!FirstRow) ? 1 : 0);
        int item2 = 3;
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
                    item2 = 2;
                    Row = Row.Substring(2);
                    break;
                case 's':
                {
                    Row = Row.Substring(2);
                    string text = "";
                    int num = 0;
                    while (Row.Length > num + 1 && char.IsDigit(Row[num]))
                    {
                        text += Row[num++];
                    }
                    Row = Row.Substring(num);
                    if (!int.TryParse(text, out result) || result < 0)
                    {
                        result = ((!FirstRow) ? 1 : 0);
                    }
                    break;
                }
            }
        }
        return new Tuple<string, int, int, int>(Row, item, result, item2);
    }

    private static WEPoint[,] CreateStatueLetter(char Letter)
    {
        WEPoint[,] array = new WEPoint[2, 3];
        short num = 0;
        short num2;
        if (Letter > '/' && Letter < ':')
        {
            num2 = (short) ((Letter - 48) * 36);
        }
        else
        {
            if (Letter <= '`' || Letter >= '{')
            {
                return array;
            }
            num2 = (short) ((Letter - 87) * 36);
        }
        for (short num3 = num2; num3 <= num2 + 18; num3 += 18)
        {
            int num4 = 0;
            for (short num5 = 0; num5 <= 36; num5 += 18)
            {
                array[num, num4++] = new WEPoint(num3, num5);
            }
            num++;
        }
        return array;
    }
}
