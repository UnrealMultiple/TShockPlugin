using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using TShockAPI;

namespace CreateSpawn;

internal static class Utils
{
    public static void SaveOrigTile(TSPlayer plr, int startX, int startY, int endX, int endY)
    {
        // 使用CopyBuilding方法创建当前选区的原始建筑数据
        var building = CopyBuilding(startX, startY, endX, endY);

        // 将building对象压入栈中
        var stack = Map.LoadBack(plr.Name);
        stack.Push(building);
        Map.SaveBack(plr.Name, stack);
    }

    public static void RollbackBuilding(TSPlayer plr)
    {
        var stack = Map.LoadBack(plr.Name);
        if (stack.Count == 0)
        {
            plr.SendErrorMessage(GetString("没有可还原的图格"));
            return;
        }

        var building = stack.Pop();
        Map.SaveBack(plr.Name, stack);

        // 计算选区边界
        var startX = building.Origin.X;
        var startY = building.Origin.Y;
        var endX = startX + building.Width - 1;
        var endY = startY + building.Height - 1;

        if (building.Tiles == null) return;

        // 0. 还原前先销毁当前区域的互动家具实体
        KillAll(startX, endX, startY, endY);

        // 1. 还原图格数据
        for (var x = 0; x < building.Width; x++)
        {
            for (var y = 0; y < building.Height; y++)
            {
                var worldX = startX + x;
                var worldY = startY + y;

                if (worldX < 0 || worldX >= Main.maxTilesX ||
                    worldY < 0 || worldY >= Main.maxTilesY) continue;

                Main.tile[worldX, worldY].CopyFrom(building.Tiles[x, y]);
                TSPlayer.All.SendTileSquareCentered(worldX, worldY, 1);
            }
        }

        // 2. 修复实体
        FixAll(startX, endX, startY, endY);

        // 3. 恢复箱子物品（依赖已存在的箱子实体）
        if (building.ChestItems != null)
        {
            RestoreChestItems(building.ChestItems, new Point(0, 0));
        }

        // 4. 恢复标牌信息
        RestoreSignText(building, 0, 0);

        // 5. 物品框、盘子、武器架、人偶、衣帽架
        RestoreItemFrames(building.ItemFrames, new Point(0, 0));
        RestorefoodPlatter(building.FoodPlatters, new Point(0, 0));
        RestoreWeaponsRack(building.WeaponsRacks, new Point(0, 0));
        RestoreDisplayDoll(building.DisplayDolls, new Point(0, 0));
        RestoreHatRack(building.HatRacks, new Point(0, 0));
        RestoreLogicSensor(building.LogicSensors, new Point(0, 0));
    }

    public static Building CopyBuilding(int startX, int startY, int endX, int endY)
    {
        var minX = Math.Min(startX, endX);
        var maxX = Math.Max(startX, endX);
        var minY = Math.Min(startY, endY);
        var maxY = Math.Max(startY, endY);

        var width = maxX - minX + 1;
        var height = maxY - minY + 1;

        var tiles = new Tile[width, height];

        var chestItems = new List<ChestItems>();  // 定义箱子物品及其位置
        var signText = new List<Sign>(); // 用于存储标牌数据
        var itemFrames = new List<ItemFrames>(); // 用于存储物品框数据
        var weaponsRack = new List<WeaponRack>(); //武器架物品
        var foodPlatter = new List<FPlatters>(); //盘子物品
        var displayDolls = new List<DDolls>(); //人偶物品
        var hatRacks = new List<HatRacks>();  //衣帽架物品
        var logicSensor = new List<LogicSensors>(); //逻辑感应器
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                var indexX = x - minX;
                var indexY = y - minY;

                //复制图格
                tiles[indexX, indexY] = (Tile)Main.tile[x, y].Clone();

                GetChestItems(chestItems, x, y); //获取箱子物品
                GetSign(signText, x, y); //获取标牌、广播盒、墓碑上等可阅读家具的信息
                GetItemFrames(itemFrames, x, y); //获取物品框的物品
                GetWeaponsRack(weaponsRack, x, y);  //获取武器架的物品
                GetfoodPlatter(foodPlatter, x, y);  //获取盘子的物品
                GetDisplayDoll(displayDolls, x, y); //获取人偶的物品
                GetHatRack(hatRacks, x, y); //获取衣帽架物品
                GetLogicSensor(logicSensor, x, y); //获取逻辑感应器开关状态
            }
        }

        return new Building
        {
            Width = width,
            Height = height,
            Tiles = tiles,
            Origin = new Point(minX, minY),
            ChestItems = chestItems,
            Signs = signText,
            ItemFrames = itemFrames,
            WeaponsRacks = weaponsRack,
            FoodPlatters = foodPlatter,
            DisplayDolls = displayDolls,
            HatRacks = hatRacks,
            LogicSensors = logicSensor,
        };
    }

    public static void GetChestItems(List<ChestItems> chestItems, int x, int y)
    {
        // 判断是否是箱子图格
        if (!TileID.Sets.BasicChest[Main.tile[x, y].type]) return;

        // 查找对应的 Chest 对象
        var index = Chest.FindChest(x, y);
        if (index < 0) return;

        var chest = Main.chest[index];
        if (chest == null) return;

        // 只处理箱子的左上角图格
        if (x != chest.x || y != chest.y) return;

        //定义箱子内的物品格子位置
        for (var slot = 0; slot < 40; slot++)
        {
            var item = chest.item[slot];
            if (item?.active == true)
            {
                // 克隆物品并记录其原来所在箱子的位置
                chestItems.Add(new ChestItems
                {
                    Item = (Item) item.Clone(), // 克隆物品
                    Position = new Point(x, y), // 记录箱子位置
                    Slot = slot
                });
            }
        }
    }

    public static void RestoreChestItems(IEnumerable<ChestItems> Chests, Point offset)
    {
        if (Chests == null || !Chests.Any()) return;

        foreach (var data in Chests)
        {
            try
            {
                if (data.Item == null || data.Item.IsAir) continue;

                // 计算新的箱子位置
                var newX = data.Position.X + offset.X;
                var newY = data.Position.Y + offset.Y;

                // 查找箱子
                var index = Chest.FindChest(newX, newY);
                if (index == -1) continue;

                //获取箱子对象
                var chest = Main.chest[index];

                // 检查槽位是否合法
                if (data.Slot < 0 || data.Slot >= 40) continue;

                // 克隆物品并设置到箱子槽位
                chest.item[data.Slot] = (Item) data.Item.Clone();
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(GetString($"还原箱子物品出错 @ {data.Position}: {ex}"));
            }
        }
    }

    public static void GetSign(List<Sign> signs, int x, int y)
    {
        if (Main.tile[x, y]?.active() == true && Main.tileSign[Main.tile[x, y].type])
        {
            // 获取 Sign ID
            var signId = Sign.ReadSign(x, y);
            if (signId >= 0)
            {
                var origSign = Main.sign[signId];
                if (origSign != null)
                {
                    signs.Add(new Sign
                    {
                        x = x,
                        y = y,
                        text = origSign.text ?? ""
                    });
                }
            }
        }
    }

    private static int CreateNewSign(int x, int y)
    {
        // 查找是否有重复的 Sign
        for (var i = 0; i < Sign.maxSigns; i++)
        {
            if (Main.sign[i] != null && Main.sign[i].x == x && Main.sign[i].y == y)
            {
                return i;
            }
        }

        // 找一个空槽位插入新的 Sign
        for (var i = 0; i < Sign.maxSigns; i++)
        {
            if (Main.sign[i] == null)
            {
                Main.sign[i] = new Sign
                {
                    x = x,
                    y = y,
                    text = ""
                };
                return i;
            }
        }

        return -1; // 没有可用槽位
    }

    public static void RestoreSignText(Building clip, int baseX, int baseY)
    {
        if (clip.Signs != null)
        {
            foreach (var sign in clip.Signs)
            {
                var newX = sign.x + baseX;
                var newY = sign.y + baseY;

                if (newX < 0 || newY < 0 || newX >= Main.maxTilesX || newY >= Main.maxTilesY)
                    continue;

                var tile = Main.tile[newX, newY];
                if (tile == null || !tile.active() || !Main.tileSign[tile.type]) continue;

                // 创建新的 Sign 或复用已有的
                var signId = CreateNewSign(newX, newY);
                if (signId >= 0)
                {
                    Main.sign[signId].text = sign.text;
                }
            }
        }
    }

    public static void GetItemFrames(List<ItemFrames> itemFrames, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.type != TileID.ItemFrame) return;
        if (tile.frameX % 36 != 0 || tile.frameY != 0) return;

        var id = TEItemFrame.Find(x, y);
        if (id != -1)
        {
            var frame = (TEItemFrame) TileEntity.ByID[id];
            itemFrames.Add(new ItemFrames()
            {
                Item = new NetItem(frame.item.netID, frame.item.stack, frame.item.prefix),
                Position = new Point(x, y),
            });
        }
    }

    public static void RestoreItemFrames(List<ItemFrames> itemFrames, Point offset)
    {
        if (itemFrames == null || !itemFrames.Any()) return;

        foreach (var data in itemFrames)
        {
            // 计算新的物品框位置
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            // 获取图格信息
            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() || tile.type != TileID.ItemFrame) continue;

            // 查找或创建 TEItemFrame（TileEntity）
            var id = TEItemFrame.Find(newX, newY);
            if (id == -1)
            {
                id = TEItemFrame.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            // 获取物品框实体
            var frame = (TEItemFrame) TileEntity.ByID[id];

            // 创建一个实际的物品对象并复制数据
            var item = new Item();
            item.netDefaults(data.Item.NetId); // 设置物品 ID
            item.stack = data.Item.Stack;       // 设置数量
            item.prefix = data.Item.PrefixId;     // 设置前缀

            // 更新物品框内的物品
            frame.item = item;
        }
    }

    public static void GetWeaponsRack(List<WeaponRack> WRack, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.frameX % 54 != 0 || tile.frameY != 0) return;
        if (tile.type == TileID.WeaponsRack2)
        {
            var id = TEWeaponsRack.Find(x, y);
            if (id != -1)
            {
                var rack = (TEWeaponsRack) TileEntity.ByID[id];
                WRack.Add(new WeaponRack()
                {
                    Item = new NetItem(rack.item.netID, rack.item.stack, rack.item.prefix),
                    Position = new Point(x, y),
                });
            }
        }
    }

    public static void RestoreWeaponsRack(List<WeaponRack> WRack, Point offset)
    {
        if (WRack == null || !WRack.Any()) return;

        foreach (var data in WRack)
        {
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() || tile.type != TileID.WeaponsRack2) continue;

            var id = TEWeaponsRack.Find(newX, newY);
            if (id == -1)
            {
                id = TEWeaponsRack.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            var rack = (TEWeaponsRack) TileEntity.ByID[id];

            // 创建一个实际的物品对象并复制数据
            var item = new Item();
            item.netDefaults(data.Item.NetId); // 设置物品 ID
            item.stack = data.Item.Stack;       // 设置数量
            item.prefix = data.Item.PrefixId;     // 设置前缀

            rack.item = item;
        }
    }

    public static void GetfoodPlatter(List<FPlatters> FPlatter, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.type == TileID.FoodPlatter)
        {
            var id = TEFoodPlatter.Find(x, y);
            if (id != -1)
            {
                var platter = (TEFoodPlatter) TileEntity.ByID[id];
                FPlatter.Add(new FPlatters()
                {
                    Item = new NetItem(platter.item.netID, platter.item.stack, platter.item.prefix),
                    Position = new Point(x, y),
                });
            }
        }
    }

    public static void RestorefoodPlatter(List<FPlatters> FPlatter, Point offset)
    {
        if (FPlatter == null || FPlatter.Count == 0) return;

        foreach (var data in FPlatter)
        {
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() ||
                tile.type != TileID.FoodPlatter) continue;

            var id = TEFoodPlatter.Find(newX, newY);
            if (id == -1)
            {
                id = TEFoodPlatter.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            var food = (TEFoodPlatter) TileEntity.ByID[id];
            // 创建一个实际的物品对象并复制数据
            var item = new Item();
            item.netDefaults(data.Item.NetId); // 设置物品 ID
            item.stack = data.Item.Stack;       // 设置数量
            item.prefix = data.Item.PrefixId;     // 设置前缀

            food.item = item;
        }
    }

    public static void GetDisplayDoll(List<DDolls> Dolls, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.type == TileID.DisplayDoll)
        {
            var id = TEDisplayDoll.Find(x, y);
            if (id != -1)
            {
                var doll = (TEDisplayDoll) TileEntity.ByID[id];
                Dolls.Add(new DDolls()
                {
                    Items = doll._items.Select(i => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
                    Dyes = doll._dyes.Select(i => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
                    Position = new Point(x, y),
                });
            }
        }
    }

    public static void RestoreDisplayDoll(List<DDolls> Dolls, Point offset)
    {
        if (Dolls == null || !Dolls.Any()) return;

        foreach (var data in Dolls)
        {
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() ||
                tile.type != TileID.DisplayDoll) continue;

            var id = TEDisplayDoll.Find(newX, newY);
            if (id == -1)
            {
                id = TEDisplayDoll.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            var doll = (TEDisplayDoll) TileEntity.ByID[id];
            doll._items = new Item[data.Items.Length];
            for (var i = 0; i < data.Items.Length; i++)
            {
                var netItem = data.Items[i];
                var item = new Item();
                item.netDefaults(netItem.NetId);
                item.stack = netItem.Stack;
                item.prefix = netItem.PrefixId;
                doll._items[i] = item;
            }

            doll._dyes = new Item[data.Dyes.Length];
            for (var i = 0; i < data.Dyes.Length; i++)
            {
                var netItem = data.Dyes[i];
                var item = new Item();
                item.netDefaults(netItem.NetId);
                item.stack = netItem.Stack;
                item.prefix = netItem.PrefixId;
                doll._dyes[i] = item;
            }
        }
    }

    public static void GetHatRack(List<HatRacks> Racks, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.type == TileID.HatRack)
        {
            var id = TEHatRack.Find(x, y);
            if (id != -1)
            {
                var doll = (TEHatRack) TileEntity.ByID[id];
                Racks.Add(new HatRacks()
                {
                    Items = doll._items.Select(i => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
                    Dyes = doll._dyes.Select(i => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
                    Position = new Point(x, y),
                });
            }
        }
    }

    public static void RestoreHatRack(List<HatRacks> Racks, Point offset)
    {
        if (Racks == null || !Racks.Any()) return;

        foreach (var data in Racks)
        {
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() ||
                tile.type != TileID.HatRack) continue;

            var id = TEHatRack.Find(newX, newY);
            if (id == -1)
            {
                id = TEHatRack.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            var Rack = (TEHatRack) TileEntity.ByID[id];
            Rack._items = new Item[data.Items.Length];
            for (var i = 0; i < data.Items.Length; i++)
            {
                var netItem = data.Items[i];
                var item = new Item();
                item.netDefaults(netItem.NetId);
                item.stack = netItem.Stack;
                item.prefix = netItem.PrefixId;
                Rack._items[i] = item;
            }

            Rack._dyes = new Item[data.Dyes.Length];
            for (var i = 0; i < data.Dyes.Length; i++)
            {
                var netItem = data.Dyes[i];
                var item = new Item();
                item.netDefaults(netItem.NetId);
                item.stack = netItem.Stack;
                item.prefix = netItem.PrefixId;
                Rack._dyes[i] = item;
            }
        }
    }

    public static void GetLogicSensor(List<LogicSensors> sensors, int x, int y)
    {
        var tile = Main.tile[x, y];
        if (tile.type == TileID.LogicSensor)
        {
            var id = TEFoodPlatter.Find(x, y);
            if (id != -1)
            {
                var sensor = (TELogicSensor) TileEntity.ByID[id];
                sensors.Add(new LogicSensors()
                {
                    type = sensor.logicCheck,
                    Position = new Point(x, y),
                });
            }
        }
    }

    public static void RestoreLogicSensor(List<LogicSensors> Sensors, Point offset)
    {
        if (Sensors == null || !Sensors.Any()) return;

        foreach (var data in Sensors)
        {
            var newX = data.Position.X + offset.X;
            var newY = data.Position.Y + offset.Y;

            var tile = Main.tile[newX, newY];
            if (tile == null || !tile.active() ||
                tile.type != TileID.LogicSensor) continue;

            var id = TELogicSensor.Find(newX, newY);
            if (id == -1)
            {
                id = TELogicSensor.Place(newX, newY);
                if (id == -1)
                    continue; // 创建失败
            }

            var Sensor = (TELogicSensor) TileEntity.ByID[id];
            Sensor.logicCheck = data.type;
        }
    }

    public static void FixAll(int startX, int endX, int startY, int endY)
    {
        for (var x = startX; x <= endX; x++)
        {
            for (var y = startY; y <= endY; y++)
            {
                var tile = Main.tile[x, y];
                if (tile == null || !tile.active()) continue;

                //如果查找图格里是箱子
                if (TileID.Sets.BasicChest[tile.type] && Chest.FindChest(x, y) == -1)
                {
                    // 创建新的 Chest  
                    var newChest = Chest.CreateChest(x, y);
                    if (newChest == -1) continue;
                }

                // 同步箱子图格到主位置
                if (tile.type == TileID.Containers || tile.type == TileID.Containers2)
                {
                    WorldGen.SquareTileFrame(x, y, true);
                }

                //物品框
                if (tile.type == TileID.ItemFrame && tile.frameX % 36 == 0 && tile.frameY == 0)
                {
                    var ItemFrame = TEItemFrame.Place(x, y);
                    WorldGen.SquareTileFrame(x, y, true);
                    if (ItemFrame == -1) continue;
                }

                //武器架
                if ((tile.type == TileID.WeaponsRack || tile.type == TileID.WeaponsRack2) && tile.frameX % 54 == 0 && tile.frameY == 0)
                {
                    var WeaponsRack = TEWeaponsRack.Place(x, y);
                    WorldGen.SquareTileFrame(x, y, true);
                    if (WeaponsRack == -1) continue;
                }

                //标牌 墓碑  广播盒
                if ((tile.type == TileID.Signs ||
                    tile.type == TileID.Tombstones ||
                    tile.type == TileID.AnnouncementBox) &&
                    tile.frameX % 36 == 0 && tile.frameY == 0 &&
                    Sign.ReadSign(x, y, false) == -1)
                {
                    var sign = Sign.ReadSign(x, y, true);
                    if (sign == -1) continue;
                }

                //逻辑感应器
                if (tile.type == TileID.LogicSensor && TELogicSensor.Find(x, y) == -1)
                {
                    var LogicSensor = TELogicSensor.Place(x, y);
                    if (LogicSensor == -1)
                    {
                        continue;
                    } ((TELogicSensor) TileEntity.ByID[LogicSensor]).logicCheck = (TELogicSensor.LogicCheckType) ((tile.frameY / 18) + 1);
                }

                //人体模型
                if (tile.type == TileID.DisplayDoll && tile.frameX % 36 == 0 && tile.frameY == 0)
                {
                    var DisplayDoll = TEDisplayDoll.Place(x, y);
                    if (DisplayDoll == -1) continue;
                }

                //盘子
                if (tile.type == TileID.FoodPlatter)
                {
                    var FoodPlatter = TEFoodPlatter.Place(x, y);
                    WorldGen.SquareTileFrame(x, y, true);
                    if (FoodPlatter == -1) continue;
                }

                //晶塔
                if (tile.type == TileID.TeleportationPylon && tile.frameX % 54 == 0 && tile.frameY == 0)
                {
                    var TeleportationPylon = TETeleportationPylon.Place(x, y);
                    if (TeleportationPylon == -1) continue;
                }

                //训练假人（稻草人）
                if (tile.type == TileID.TargetDummy && tile.frameX % 36 == 0 && tile.frameY == 0)
                {
                    var TrainingDummy = TETrainingDummy.Place(x, y);
                    if (TrainingDummy == -1) continue;
                }

                //衣帽架
                if (tile.type == TileID.HatRack && tile.frameX == 0 && tile.frameY == 0)
                {
                    var HatRack = TEHatRack.Place(x, y);
                    WorldGen.SquareTileFrame(x, y, true);
                    if (HatRack == -1) continue;
                }
            }
        }
    }

    public static void KillAll(int startX, int endX, int startY, int endY)
    {
        for (var x = startX; x <= endX; x++)
        {
            for (var y = startY; y <= endY; y++)
            {
                var tile = Main.tile[x, y];
                if (tile == null || !tile.active()) continue;

                //如果查找图格里是箱子
                if (TileID.Sets.BasicChest[tile.type] && Chest.FindChest(x, y) == -1)
                {
                    Chest.DestroyChest(x, y);
                }

                //物品框
                if (tile.type == TileID.ItemFrame)
                {
                    TEItemFrame.Kill(x, y);
                }

                //武器架
                if (tile.type == TileID.WeaponsRack || tile.type == TileID.WeaponsRack2)
                {
                    TEWeaponsRack.Kill(x, y);
                }

                //标牌 墓碑  广播盒
                if (tile.type == TileID.Signs ||
                    tile.type == TileID.Tombstones ||
                    tile.type == TileID.AnnouncementBox)
                {
                    Sign.KillSign(x, y);
                }

                //逻辑感应器
                if (tile.type == TileID.LogicSensor)
                {
                    TELogicSensor.Kill(x, y);
                }

                //人体模型
                if (tile.type == TileID.DisplayDoll)
                {
                    TEDisplayDoll.Kill(x, y);
                }

                //盘子
                if (tile.type == TileID.FoodPlatter)
                {
                    TEFoodPlatter.Kill(x, y);
                }

                //晶塔
                if (tile.type == TileID.TeleportationPylon)
                {
                    TETeleportationPylon.Kill(x, y);
                }

                //训练假人（稻草人）
                if (tile.type == TileID.TargetDummy)
                {
                    TETrainingDummy.Kill(x, y);
                }

                //衣帽架
                if (tile.type == TileID.HatRack)
                {
                    TEHatRack.Kill(x, y);
                }
            }
        }
    }

    public static void SpawnBuilding(TSPlayer plr, int startX, int startY, Building clip)
    {
        TileHelper.StartGen();
        //缓存 方便粘贴错了还原
        Utils.SaveOrigTile(plr, startX, startY, startX + clip.Width - 1, startY + clip.Height - 1);
        var secondLast = GetUnixTimestamp;

        // 定义偏移坐标（从原始世界坐标到玩家头顶）
        var baseX = startX - clip.Origin.X;
        var baseY = startY - clip.Origin.Y;

        Task.Run(() =>
        {
            // 先销毁目标区域的互动家具实体
            Utils.KillAll(startX, startX + clip.Width - 1, startY, startY + clip.Height - 1);

            for (var x = 0; x < clip.Width; x++)
            {
                for (var y = 0; y < clip.Height; y++)
                {
                    var worldX = startX + x;
                    var worldY = startY + y;

                    // 边界检查
                    if (worldX < 0 || worldX >= Main.maxTilesX ||
                        worldY < 0 || worldY >= Main.maxTilesY) continue;

                    // 完全复制图格数据
                    Main.tile[worldX, worldY] = (Tile) clip.Tiles![x, y].Clone();
                }
            }

        }).ContinueWith(_ =>
        {
            // 修复家具实体
            Utils.FixAll(startX, startX + clip.Width - 1, startY, startY + clip.Height - 1);
            // 修复箱子内物品
            Utils.RestoreChestItems(clip.ChestItems!, new Point(baseX, baseY));
            // 修复标牌信息
            Utils.RestoreSignText(clip, baseX, baseY);
            // 修复物品框物品
            Utils.RestoreItemFrames(clip.ItemFrames, new Point(baseX, baseY));
            //修复盘子、武器架、人偶、衣帽架的物品
            Utils.RestorefoodPlatter(clip.FoodPlatters, new Point(baseX, baseY));
            Utils.RestoreWeaponsRack(clip.WeaponsRacks, new Point(baseX, baseY));
            Utils.RestoreDisplayDoll(clip.DisplayDolls, new Point(baseX, baseY));
            Utils.RestoreHatRack(clip.HatRacks, new Point(baseX, baseY));
            Utils.RestoreLogicSensor(clip.LogicSensors, new Point(baseX, baseY));

            TileHelper.GenAfter();
            var value = GetUnixTimestamp - secondLast;
            plr.SendSuccessMessage(GetString($"已粘贴区域 ({clip.Width} x {clip.Height})，用时{value}秒。"));
        });
    }

    public static int GetUnixTimestamp => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

    public static void Restore(TSPlayer ply)
    {
        var startX = ply.TempPoints[0].X;
        var startY = ply.TempPoints[0].Y;
        var endX = ply.TempPoints[1].X;
        var endY = ply.TempPoints[1].Y;
        TileHelper.StartGen();
        var secondLast = GetUnixTimestamp;
        _ = Task.Run(delegate
        {
            //还原前缓存一遍
            Utils.SaveOrigTile(ply, startX, startY, endX, endY);

            //还原方法
            Utils.RollbackBuilding(ply);

        }).ContinueWith(delegate
        {
            TileHelper.GenAfter();
            var value = GetUnixTimestamp - secondLast;
            ply.SendSuccessMessage(GetString($"已将选区还原，用时{value}秒。"));
        });
    }
}
