using System.Reflection;

using Newtonsoft.Json;

using Terraria;
using Terraria.ID;

namespace AutoClassificationQuickStack;

internal class Configuration
{
    public const string FileName = $"{nameof(AutoClassificationQuickStack)}.json";
    public static string ConfigPath = Path.Combine(TShockAPI.TShock.SavePath, FileName);

    public List<ClassificationInfo> ClassificationInfos;
    public Configuration()
    {
        this.ClassificationInfos = new();
    }
    public Configuration(List<ClassificationInfo> classificationInfos)
    {
        this.ClassificationInfos = classificationInfos;
    }

    public static Configuration GetConfig()
    {
        Configuration config;
        // 检查配置文件是否存在
        if (File.Exists(ConfigPath))
        {
            // 如果存在，则从文件中读取配置信息并反序列化为 Configuration 对象。
            // 使用空合并运算符 ?? 确保即使反序列化失败也能返回一个新的 Configuration 实例。
            config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath)) ?? new();
        }
        else
        {
            var item = ContentSamples.ItemsByType.Values;
            // 获取 ItemID 类中所有表示物品的静态字段（常量），这些字段定义了游戏中的物品 ID。
            var fields = typeof(ItemID).GetFields().Where(static x => x.IsLiteral).ToArray();
            // 定义辅助方法，用于将 FieldInfo 对象转换为整数值，这里用来获取物品 ID。
            static int intValueSelector(FieldInfo fieldInfo) => Convert.ToInt32(fieldInfo.GetValue(null)!);
            // 定义辅助方法，从 Item 对象中提取物品类型 ID。
            static int typeSelector(Item item) => item.type;
            // 绳
            var Rope = item.Where(static x => x.netID > 0 && ItemID.Sets.SortingPriorityRopes[x.netID] > -1).Select(x => x.type).Except(new int[] { 210 }).ToArray();
            // 火把
            var Torch = item.Where(x => ItemID.Sets.Torches[x.type]).Select(typeSelector).ToArray();
            // 闪光棒
            var Glowstick = fields.Where(static x => x.Name.EndsWith("Glowstick")).Select(intValueSelector).ToArray();

            var Group_1 = new HashSet<int>();
            Group_1.UnionWith(Torch);
            Group_1.UnionWith(Rope);
            Group_1.UnionWith(Glowstick);

            // 旗
            var flag = fields.Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Banner")).Select(intValueSelector).ToArray();
            // 宝藏袋
            var TreasureBag = item.Where(x => ItemID.Sets.BossBag[x.type]).Select(typeSelector).ToArray();
            // 环境钥匙
            var BiomeKeys = item.Where(x => ItemID.Sets.ItemsThatAreProcessedAfterNormalContentSample.Contains(x.type)).Select(typeSelector).ToArray();

            var Group_2 = new HashSet<int>();
            Group_2.UnionWith(TreasureBag);
            Group_2.UnionWith(BiomeKeys);
            // 炼药材料
            var Alchemy = Enumerable.Empty<int>()
            .Concat(new[] { 5, 31, 68, 126, 150, 154, 183, 209, 275, 276 })
            .Concat(Enumerable.Range(307, 14)) // 307 到 320
            .Concat(new[] { 323, 999, 1127, 1330 })
            .Concat(Enumerable.Range(2303, 5))
            .Concat(Enumerable.Range(2310, 4))
            .Concat(new[] { 2309, 2315, 2317, 2318, 2319, 2321, 2357, 2358, 3093, 4361 })
            .Concat(Enumerable.Range(4412, 3))
            .Distinct().ToArray();
            // 任务鱼
            var QuestsFish = Enumerable.Range(2450, 2488 - 2450 + 1).ToArray();
            // 钓鱼
            var Fishing = item.Where(static x => x.netID > 0 && x.bait > 0).Select(typeSelector).Union(new int[]{
                1991,2290, 2297, 2298, 2299,2300, 2301, 2302, 2308, 2313, 2315, 2316,
                1991,2290, 2297, 2298, 2299,2300, 2301, 2302, 2308, 2313, 2315, 2316,
                3183, 4821, 5139, 5140, 5141, 5142, 5143, 5144, 5145, 5146,
                4373,4401,4402,4410,4345}).Except(new int[] { 2313, 2315, 2673, 4361 }).ToArray();
            // 钓鱼匣
            var Crates = item.Where(x => ItemID.Sets.IsFishingCrateHardmode[x.type] || ItemID.Sets.IsFishingCrate[x.type]).Select(typeSelector).ToArray();

            var Group_3 = new HashSet<int>();
            Group_3.UnionWith(Fishing);
            Group_3.UnionWith(Crates);
            // 风筝
            var Kites = item.Where(x => ItemID.Sets.IsAKite[x.type]).Select(typeSelector).ToArray();
            // 食物
            var food = item.Where(x => ItemID.Sets.IsFood[x.type]).Select(typeSelector).ToArray();
            // 增益药水
            var Buffpotions = fields.Where(static x => x.Name.EndsWith("Potion") || (x.Name.StartsWith("FlaskOf") &&
            !(x.Name.EndsWith("HealingPotion") || x.Name.EndsWith("ManaPotion") ||
            x.Name.EndsWith("RestorationPotion")))).Select(intValueSelector)
            .Union(new[] { 29, 75, 109, 1291, 1134, 2314, 4382, 5289, 5337, 5338, 5339, 5340, 5342, 5343 })
            .Except(new[] { 5320, 5321 }).ToArray();

            var Group_4 = new HashSet<int>();
            Group_4.UnionWith(food);
            Group_4.UnionWith(Buffpotions);
            // 电路工具
            var Wire = item.Where(static x => 
            (x.netID > 0 && ItemID.Sets.SortingPriorityWiring[x.netID] > -1) ||
            x.mech || x.Name.EndsWith("Statue") || ItemID.Sets.TrapSigned[x.type])
                .Select(typeSelector).Union(new int[] { 2799, 3619, 3624 }).ToArray();
            // 晶塔
            var Pylon = fields.Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.StartsWith("TeleportationPylon")).Select(intValueSelector).Distinct().ToArray();
            var Furniture = fields.Where(static x => x.IsLiteral).Where(static x =>
            x.Name.EndsWith("Post") ||x.Name.EndsWith("Piano") ||x.Name.EndsWith("PressurePlate") ||
            x.Name.EndsWith("Anvil") ||x.Name.EndsWith("Box") ||x.Name.EndsWith("Bed") ||
            x.Name.EndsWith("Beam") ||x.Name.EndsWith("Bowl") ||x.Name.EndsWith("Bathtub") ||
            x.Name.EndsWith("Bookcase") || x.Name.EndsWith("ButterflyJar") || x.Name.EndsWith("Cup") ||
            x.Name.EndsWith("Cage") || x.Name.EndsWith("Clock") || x.Name.EndsWith("Chair") ||
            x.Name.EndsWith("Chest") ||x.Name.EndsWith("Candle") ||x.Name.EndsWith("Campfire") ||
            x.Name.EndsWith("Chandelier") ||x.Name.EndsWith("Chandelier") ||x.Name.EndsWith("Door") ||
            x.Name.EndsWith("Dresser") ||x.Name.EndsWith("Forge") ||x.Name.EndsWith("Fence") ||
            x.Name.EndsWith("Fountain") ||x.Name.EndsWith("Trap") ||x.Name.EndsWith("Table") ||
            x.Name.EndsWith("Toilet") ||x.Name.EndsWith("Trophy") ||x.Name.EndsWith("Rope") ||
            x.Name.EndsWith("Sink") ||x.Name.EndsWith("Sign") ||x.Name.EndsWith("Sofa") ||
            x.Name.StartsWith("SillyBalloon") ||x.Name.StartsWith("SillyStreamer") ||x.Name.EndsWith("StainedGlass") ||
            x.Name.EndsWith("Wall") ||x.Name.EndsWith("WorkBench") ||x.Name.EndsWith("Wallpaper") ||
            x.Name.StartsWith("WeightedPressurePlate") ||x.Name.EndsWith("Moss") ||x.Name.EndsWith("Monolith") ||
            x.Name.EndsWith("MasterTrophy") ||x.Name.EndsWith("Vase") ||x.Name.StartsWith("LogicGate") ||
            x.Name.StartsWith("LogicSensor") ||x.Name.EndsWith("Lamp") ||x.Name.EndsWith("Lantern"))
                .Select(intValueSelector).Except(Wire).Except(Rope).Except(new int[] { 576, 889, 1785, 2375, 3043, 5042 })
                .Union(new[]
                {
                    321, 332, 345, 348, 349, 350, 351, 352, 355, 356, 363, 398,
                    995, 996, 997, 998, 1120, 1173, 1174, 1175, 1176, 1177,
                    1449, 2114, 2192, 2193, 2194, 2195, 2196, 2197, 2198,
                    2203, 2204, 2208, 3364, 3365, 3369, 3747, 3750, 4075, 4326, 5320, 5321,
                }).Distinct().ToArray(); // Distinct() 用于去除重复的物品 ID。

            var Group_5 = new HashSet<int>();
            Group_5.UnionWith(Pylon);
            Group_5.UnionWith(Furniture);
            // 平台
            var Platform = fields.Where(static x => x.IsLiteral)
           .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Platform")).Select(intValueSelector)
           .Union(new[] { 1387, 1388, 1389, 1418 }).ToArray();
            // 方块
            var Tiles = fields.Where(static x => x.IsLiteral).Where(static x => (x.Name.Length > 3 &&
            x.Name.EndsWith("Block")) ||x.Name.EndsWith("Brick") ||x.Name.StartsWith("TeamBlock") ||
            (x.Name.EndsWith("GemLock") && x.Name != "DirtiestBlock")).Select(intValueSelector)
            .Except(new int[] { 1127, 3622, 3638, 3639, 3640, 3641, 3642, 5400 })
            .Union(new[] { 147, 170, 1150, 2697, 3573, 3574, 3575, 3576, 3621, 3633, 3634, 3635, 3636, 3637, 3643 }).Union(Platform).Except(Wire).ToArray();

            var BiomesTiles = item.Where(x => ItemID.Sets.ItemsForStuffCannon.Contains(x.type)).Select(typeSelector).Except(Tiles)
                        .Except(new int[] { 276 }).ToArray();

            var Group_6 = new HashSet<int>();
            Group_6.UnionWith(Platform);
            Group_6.UnionWith(Tiles);
            Group_6.UnionWith(BiomesTiles);
            // 近战武器
            var melee = item.Where(static x => x != null && x.type != 0 && x.maxStack == 1 &&
            x.damage > 0 && x.ammo == 0 && x.melee && x.pick < 1 && x.hammer < 1 &&
            x.axe < 1).Select(typeSelector).ToArray();
            // 魔法武器
            var magic = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 &&
            x.damage > 0 && x.ammo == 0 && x.magic).Select(x => x.type).ToArray();
            // 远程武器
            var ranged = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.ranged && !x.consumable).Select(x => x.type).ToArray();
            // 投掷物
            var ranged2 = item.Where(x => x != null && x.type != 0 && x.maxStack == 9999 && x.damage > 0 && x.ammo == 0 && x.ranged && x.consumable)
                .Select(x => x.type).Except(new int[] { 154 }).ToArray();
            // 召唤法杖
            var SummonStaff = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.summon &&
            !ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[x.type]).Select(x => x.type).ToArray();
            // 召唤鞭子
            var SummonWhip = item.Where(x => ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[x.type]).Select(typeSelector).ToArray();

            var Group_7 = new HashSet<int>();
            Group_7.UnionWith(SummonStaff);
            Group_7.UnionWith(SummonWhip);
            // 抓钩
            var Hooks = fields.Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Hook")).Select(intValueSelector).Except(new int[] { 118,4881 }).ToArray();
            // 信号枪
            var FlareGun = item.Where(x => x != null && x.type != 0 &&
            x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.pick == 0 && x.hammer == 0 && x.axe == 0).Select(x => x.type)
            .Except(melee).Except(ranged).Except(magic).Except(SummonStaff).Except(SummonWhip).ToArray();
            // 建筑类工具
            var BuildingTools = item.Where(x => x.type != 0 && (ItemID.Sets.AlsoABuildingItem[x.type] ||
            x.pick > 0 || x.axe > 0 || x.hammer > 0)).Select(typeSelector).Except(Wire).ToArray();
            // 更改环境工具
            var Clentaminator = item.Where(static x => x != null && (ItemID.Sets.SortingPriorityTerraforming[x.type] > -1 ||
            ItemID.Sets.GrassSeeds[x.type])).Select(typeSelector).ToArray();

            var Group_8 = new HashSet<int>();
            Group_8.UnionWith(Hooks);
            Group_8.UnionWith(FlareGun);
            Group_8.UnionWith(BuildingTools);
            Group_8.UnionWith(Clentaminator);
            // 坐骑
            var Mounts = item.Where(x => x.mountType != -1 && !MountID.Sets.Cart[x.mountType]).Select(typeSelector).ToArray();
            // 矿车
            var Minecart = item.Where(x => x.mountType != -1 && MountID.Sets.Cart[x.mountType]).Select(typeSelector).
            Union(new[] { 3353, 3354, 3355, 3356 }).ToArray();

            var Group_9 = new HashSet<int>();
            Group_9.UnionWith(Mounts);
            Group_9.UnionWith(Minecart);
            // 宠物
            var Pets = item.Where(x => x.buffType > 0 && Main.vanityPet[x.buffType]).Select(typeSelector).ToArray();
            // 照明宠物
            var LightPets = item.Where(x => x.buffType > 0 && Main.lightPet[x.buffType]).Select(typeSelector).ToArray();

            var Group_10 = new HashSet<int>();
            Group_10.UnionWith(Pets);
            Group_10.UnionWith(LightPets);
            // 奇异植物
            var StrangePlant = item.Where(x => ItemID.Sets.ExoticPlantsForDyeTrade[x.type]).Select(typeSelector).ToArray();
            // 染发剂
            var HairDye = item.Where(x => x.hairDye >= 0 || x.Name.EndsWith("HairDye")).Select(typeSelector).Union(new[] { 1990 }).ToArray();
           // 喷漆
            var Paint = typeof(ItemID).GetFields().Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Paint")).Select(intValueSelector).ToArray();
            // 染料
            var Dye = typeof(ItemID).GetFields().Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Dye")).Select(intValueSelector).
                Union(new[] { 1107, 1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 3385, 3386, 3387, 3388 }).ToArray();

            var Group_11 = new HashSet<int>();
            Group_11.UnionWith(Dye);
            Group_11.UnionWith(Paint);
            Group_11.UnionWith(StrangePlant);
            Group_11.UnionWith(HairDye);
            // 八音盒
            var MusicBox = item.Where(x => x.Name.StartsWith("MusicBox")).Select(typeSelector).Union(new int[] { 576 }).ToArray();

            // 战斗盔甲
            var CombatArmor = item.Where(static x => x != null &&
            (x.bodySlot > 0 || x.headSlot > 0 || x.legSlot > 0) && !x.vanity && x.defense >= 1).Select(typeSelector)
            .Union(new int[] { 803, 804, 805, 1316, 1317, 1318 }).Except(new int[] { 205, 268 }).ToArray();
            // 装饰盔甲
            var VanityArmor = item.Where(static x => x != null &&
            (x.bodySlot > 0 || x.headSlot > 0 || x.legSlot > 0) && x.vanity && x.defense <= 0).Select(typeSelector)
            .Except(new int[] { 250, 264, 715, 4275 }).ToArray();
            // 服饰
            var VanityAccessories = item.Where(static x => x.accessory && x.vanity).Select(typeSelector)
            .Except(MusicBox).Except(new int[] { 576, 3536, 3537, 3538, 3539, 4318, 4054, 5345, 5347 }).Union(new int[] { 1522, 1523, 1524, 1525, 1526, 1527 }).ToArray();

            var Group_12 = new HashSet<int>();
            Group_12.UnionWith(VanityArmor);
            Group_12.UnionWith(VanityAccessories);
            // 十字章护盾
            var AnkhShield = new int[] { 156, 193, 397, 885, 886, 887, 888, 889, 890, 891, 892, 893, 901, 902, 903, 904, 1612, 3781, 5354 };
            // 贝壳电话
            var Shellphone = new int[] { 17, 18, 50, 393, 395, 709, 3036, 3037, 3084, 3095, 3096, 3099, 3102, 3118, 3119, 3120, 3121, 3122, 3123, 3124, 3199, 4263, 4819, 5358, 5359, 5360, 5361, 5437 };
            // 饰品
            var Accessories = item.Where(static x => x.accessory && ItemID.Sets.CanGetPrefixes[x.type] && !x.vanity).Select(typeSelector).Except(VanityArmor)
            .Except(new int[] { 2799, 3097, 3536, 3537, 3538, 3539, 3619, 3624, 4318, 5139, 5140, 5141, 5142, 5143, 5144, 5145, 5146, 5345, 5347 })
            .Except(Enumerable.Range(4242, 14))
            .Except(AnkhShield)
            .Except(Shellphone)
            .Union(new int[] { 268 }).ToArray();
            // 弹药
            var Ammo = item.Where(x => x.ammo > 0 && x.damage > 0 && !ItemID.Sets.CommonCoin[x.type]).Select(typeSelector)
            .Union(new[] { 69, 97, 1261, 1168, 1432, 1783, 1785, 1836, 3103, 3104 }).Distinct().ToArray();
            // 装备合成材料
            var ItemCraftingIngredients = item.Where(x => ItemID.Sets.ItemIconPulse[x.type] && x.type != 3580 && x.type != 3581).Select(typeSelector)
            .Union(new int[]
            {
                86, 118, 210, 225, 236, 259, 324, 331, 501, 507,
                508, 522, 526, 527, 528, 531, 1328, 1329, 1332,
                1339, 1345, 1346, 1347, 1348, 1508, 1516, 1517,
                1518, 1519, 1520, 1521, 1570, 1611, 1811, 1831,
                2218, 2607, 2161, 2431, 3111, 3783, 3794, 5070,
            }).ToArray();
            // 召唤物材料
            var SummonMaterials = item.Where(static x => x.netID > 0 && ItemID.Sets.SortingPriorityBossSpawns[x.netID] > -1).Select(typeSelector)
            .Union(new int[] { 23, 38, 60, 264, 362, 715, 2766, 2887, 4961 })
            .Except(new int[] { 29, 50, 75, 109, 1291, 3124, 3199, 4263, 4819, 5289, 5337, 5338, 5339, 5340, 5342, 5343, 5358, 5359, 5360, 5361, 5437 }).ToArray();

            var Group_13 = new HashSet<int>();
            Group_13.UnionWith(ItemCraftingIngredients);
            Group_13.UnionWith(SummonMaterials);
            // 矿石
            var Ore = fields.Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Ore")).Select(intValueSelector).
            Union(new[] { 173, 174, 177, 178, 179, 180, 181, 182 }).ToArray();
            // 锭
            var OreBars = fields.Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Bar")).Select(intValueSelector)
            .Union(new int[] { }).ToArray();

            var Group_14 = new HashSet<int>();
            Group_14.UnionWith(Ore);
            Group_14.UnionWith(OreBars);
            // 排除以上物品
            var Other = new HashSet<int>(
                Tiles.Concat(BiomesTiles).Concat(Alchemy).Concat(QuestsFish).Concat(Crates).Concat(Fishing)
                .Concat(Kites).Concat(MusicBox).Concat(food).Concat(flag)
               .Concat(Platform).Concat(OreBars).Concat(Ore).Concat(Pylon).Concat(Buffpotions).Concat(Furniture)
               .Concat(ItemCraftingIngredients).Concat(AnkhShield).Concat(BuildingTools)
               .Concat(Torch).Concat(Glowstick).Concat(TreasureBag).Concat(BiomeKeys).Concat(melee).Concat(SummonWhip)
               .Concat(Wire).Concat(Clentaminator).Concat(Mounts).Concat(Minecart).Concat(Pets).Concat(LightPets)
               .Concat(HairDye).Concat(Dye).Concat(Paint).Concat(CombatArmor).Concat(VanityArmor).Concat(Accessories).Concat(VanityAccessories)
               .Concat(magic).Concat(ranged).Concat(ranged2).Concat(SummonStaff).Concat(FlareGun).Concat(Ammo)
               .Concat(SummonMaterials).Concat(Shellphone));
            // Misc
            var Misc = item.Where(static x => x.material && x.createTile == -1 && x.createWall == -1).Select(typeSelector).Except(Other).ToArray();

            config = new Configuration(new List<ClassificationInfo>
            { 
                new ClassificationInfo("火把、绳子、荧光棒",Group_1.ToArray(),Group_1.ToArray()),
                new ClassificationInfo("旗帜",flag,flag),
                new ClassificationInfo("宝藏袋、环境钥匙",Group_2.ToArray(),Group_2.ToArray()),
                new ClassificationInfo("炼药材料",Alchemy,Alchemy),
                new ClassificationInfo("任务鱼", QuestsFish, QuestsFish),
                new ClassificationInfo("钓鱼、钓鱼匣子",Group_3.ToArray(),Group_3.ToArray()),
                new ClassificationInfo("风筝", Kites, Kites),
                new ClassificationInfo("食物、增益药水", Group_4.ToArray(), Group_4.ToArray()),
                new ClassificationInfo("电路工具", Wire, Wire),
                new ClassificationInfo("晶塔、家具", Group_5.ToArray(), Group_5.ToArray()),
                new ClassificationInfo("平台、方块、环境方块", Group_6.ToArray(), Group_6.ToArray()),
                new ClassificationInfo("近战武器", melee, melee),
                new ClassificationInfo("魔法武器", magic, magic),
                new ClassificationInfo("远程武器", ranged, ranged),
                new ClassificationInfo("投掷物", ranged2, ranged2),
                new ClassificationInfo("召唤法杖、召唤鞭子", Group_7.ToArray(), Group_7.ToArray()),
                new ClassificationInfo("抓钩、信号枪、建筑类工具、更改环境工具", Group_8.ToArray(), Group_8.ToArray()),
                new ClassificationInfo("坐骑、矿车", Group_9.ToArray(), Group_9.ToArray()),
                new ClassificationInfo("宠物、照明宠物", Group_10.ToArray(), Group_10.ToArray()),
                new ClassificationInfo("奇异植物、染发剂、喷漆、染料", Group_11.ToArray(), Group_11.ToArray()),
                new ClassificationInfo("八音盒", MusicBox, MusicBox),
                new ClassificationInfo("战斗盔甲", CombatArmor, CombatArmor),
                new ClassificationInfo("装饰盔甲、服饰", Group_12.ToArray(), Group_12.ToArray()),
                new ClassificationInfo("十字章护盾", AnkhShield, AnkhShield),
                new ClassificationInfo("贝壳电话", Shellphone, Shellphone),
                new ClassificationInfo("饰品", Accessories, Accessories),
                new ClassificationInfo("弹药", Ammo, Ammo),
                new ClassificationInfo("装备合成材料、召唤物材料", Group_13.ToArray(), Group_13.ToArray()),
                new ClassificationInfo("矿石、锭", Group_14.ToArray(), Group_14.ToArray()),
                new ClassificationInfo("杂物", Misc, Misc),
            });

            // 将新创建的配置序列化为 JSON 格式，并写入到配置文件中。
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented));
        }

        // 确保每个 ClassificationInfo 中的 Values 数组是排序的，以保证后续处理的一致性。
        foreach (var pair in config.ClassificationInfos)
        {
            Array.Sort(pair.Values);
        }

        // 返回加载或创建的配置对象。
        return config;
    }
}

internal class ClassificationInfo
{
    public string Name { get; set; } = string.Empty;
    public int[] Keys { get; set; }
    public int[] Values { get; set; }
    public ClassificationInfo()
    {
        this.Keys = Array.Empty<int>();
        this.Values = Array.Empty<int>();
    }
    public ClassificationInfo(string name,int[] keys, int[] values)
    {
        this.Name = name;
        this.Keys = keys;
        this.Values = values;
    }
}