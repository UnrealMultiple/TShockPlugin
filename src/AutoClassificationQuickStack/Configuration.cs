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
            
            var 绳子 = item.Where(static x => x.netID > 0 && ItemID.Sets.SortingPriorityRopes[x.netID] > -1).Select(x => x.type).Except(new int[] { 210 }).ToArray();
            var 火把 = item.Where(x => ItemID.Sets.Torches[x.type]).Select(typeSelector).ToArray();
            var 荧光棒 = fields.Where(static x => x.Name.EndsWith("Glowstick")).Select(intValueSelector).ToArray();

            var Group_1 = new HashSet<int>();
            Group_1.UnionWith(火把);
            Group_1.UnionWith(绳子);
            Group_1.UnionWith(荧光棒);

            var 旗帜 = fields.Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Banner")).Select(intValueSelector).ToArray();
            var 宝藏袋 = item.Where(x => ItemID.Sets.BossBag[x.type]).Select(typeSelector).ToArray();
            var 环境钥匙 = item.Where(x => ItemID.Sets.ItemsThatAreProcessedAfterNormalContentSample.Contains(x.type)).Select(typeSelector).ToArray();

            var Group_2 = new HashSet<int>();
            Group_2.UnionWith(宝藏袋);
            Group_2.UnionWith(环境钥匙);

            var 炼药材料 = Enumerable.Empty<int>()
            .Concat(new[] { 5, 31, 68, 126, 150, 154, 183, 209, 275, 276 })
            .Concat(Enumerable.Range(307, 14)) // 307 到 320
            .Concat(new[] { 323, 999, 1127, 1330 })
            .Concat(Enumerable.Range(2303, 5))
            .Concat(Enumerable.Range(2310, 4))
            .Concat(new[] { 2309, 2315, 2317, 2318, 2319, 2321, 2357, 2358, 3093, 4361 })
            .Concat(Enumerable.Range(4412, 3))
            .Distinct().ToArray();

            var 任务鱼 = Enumerable.Range(2450, 2488 - 2450 + 1).ToArray();

            var 钓鱼 = item.Where(static x => x.netID > 0 && x.bait > 0).Select(typeSelector).Union(new int[]{
                1991,2290, 2297, 2298, 2299,2300, 2301, 2302, 2308, 2313, 2315, 2316,
                1991,2290, 2297, 2298, 2299,2300, 2301, 2302, 2308, 2313, 2315, 2316,
                3183, 4821, 5139, 5140, 5141, 5142, 5143, 5144, 5145, 5146,
                4373,4401,4402,4410,4345}).Except(new int[] { 2313, 2315, 2673, 4361 }).ToArray();
            var 钓鱼匣子 = item.Where(x => ItemID.Sets.IsFishingCrateHardmode[x.type] || ItemID.Sets.IsFishingCrate[x.type]).Select(typeSelector).ToArray();

            var Group_3 = new HashSet<int>();
            Group_3.UnionWith(钓鱼);
            Group_3.UnionWith(钓鱼匣子);

            var 风筝 = item.Where(x => ItemID.Sets.IsAKite[x.type]).Select(typeSelector).ToArray();

            var 食物 = item.Where(x => ItemID.Sets.IsFood[x.type]).Select(typeSelector).ToArray();
            var 增益药水 = fields.Where(static x => x.Name.EndsWith("Potion") || (x.Name.StartsWith("FlaskOf") &&
            !(x.Name.EndsWith("HealingPotion") || x.Name.EndsWith("ManaPotion") ||
            x.Name.EndsWith("RestorationPotion")))).Select(intValueSelector)
            .Union(new[] { 29, 75, 109, 1291, 1134, 2314, 4382, 5289, 5337, 5338, 5339, 5340, 5342, 5343 })
            .Except(new[] { 5320, 5321 }).ToArray();

            var Group_4 = new HashSet<int>();
            Group_4.UnionWith(食物);
            Group_4.UnionWith(增益药水);

            var 电路工具 = item.Where(static x => 
            (x.netID > 0 && ItemID.Sets.SortingPriorityWiring[x.netID] > -1) ||
            x.mech || x.Name.EndsWith("Statue") || ItemID.Sets.TrapSigned[x.type])
                .Select(typeSelector).Union(new int[] { 2799, 3619, 3624 }).ToArray();

            var 晶塔 = fields.Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.StartsWith("TeleportationPylon")).Select(intValueSelector).Distinct().ToArray();
            var 家具 = fields.Where(static x => x.IsLiteral).Where(static x =>
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
                .Select(intValueSelector).Except(电路工具).Except(绳子).Except(new int[] { 576, 889, 1785, 2375, 3043, 5042 })
                .Union(new[]
                {
                    321, 332, 345, 348, 349, 350, 351, 352, 355, 356, 363, 398,
                    995, 996, 997, 998, 1120, 1173, 1174, 1175, 1176, 1177,
                    1449, 2114, 2192, 2193, 2194, 2195, 2196, 2197, 2198,
                    2203, 2204, 2208, 3364, 3365, 3369, 3747, 3750, 4075, 4326, 5320, 5321,
                }).Distinct().ToArray(); // Distinct() 用于去除重复的物品 ID。

            var Group_5 = new HashSet<int>();
            Group_5.UnionWith(晶塔);
            Group_5.UnionWith(家具);

            var 平台 = fields.Where(static x => x.IsLiteral)
           .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Platform")).Select(intValueSelector)
           .Union(new[] { 1387, 1388, 1389, 1418 }).ToArray();

            var 方块 = fields.Where(static x => x.IsLiteral).Where(static x => (x.Name.Length > 3 &&
            x.Name.EndsWith("Block")) ||x.Name.EndsWith("Brick") ||x.Name.StartsWith("TeamBlock") ||
            (x.Name.EndsWith("GemLock") && x.Name != "DirtiestBlock")).Select(intValueSelector)
            .Except(new int[] { 1127, 3622, 3638, 3639, 3640, 3641, 3642, 5400 })
            .Union(new[] { 147, 170, 1150, 2697, 3573, 3574, 3575, 3576, 3621, 3633, 3634, 3635, 3636, 3637, 3643 }).Union(平台).Except(电路工具).ToArray();

            var 环境方块 = item.Where(x => ItemID.Sets.ItemsForStuffCannon.Contains(x.type)).Select(typeSelector).Except(方块)
                        .Except(new int[] { 276 }).ToArray();

            var Group_6 = new HashSet<int>();
            Group_6.UnionWith(平台);
            Group_6.UnionWith(方块);
            Group_6.UnionWith(环境方块);

            var 近战武器 = item.Where(static x => x != null && x.type != 0 && x.maxStack == 1 &&
            x.damage > 0 && x.ammo == 0 && x.melee && x.pick < 1 && x.hammer < 1 &&
            x.axe < 1).Select(typeSelector).ToArray();

            var 魔法武器 = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 &&
            x.damage > 0 && x.ammo == 0 && x.magic).Select(x => x.type).ToArray();

            var 远程武器 = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.ranged && !x.consumable).Select(x => x.type).ToArray();

            var 投掷物 = item.Where(x => x != null && x.type != 0 && x.maxStack == 9999 && x.damage > 0 && x.ammo == 0 && x.ranged && x.consumable)
                .Select(x => x.type).Except(new int[] { 154 }).ToArray();

            var 召唤法杖 = item.Where(x => x != null && x.type != 0 && x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.summon &&
            !ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[x.type]).Select(x => x.type).ToArray();
            var 召唤鞭子 = item.Where(x => ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[x.type]).Select(typeSelector).ToArray();

            var Group_7 = new HashSet<int>();
            Group_7.UnionWith(召唤法杖);
            Group_7.UnionWith(召唤鞭子);

            var 抓钩 = fields.Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Hook")).Select(intValueSelector).Except(new int[] { 118,4881 }).ToArray();

            var 信号枪 = item.Where(x => x != null && x.type != 0 &&
            x.maxStack == 1 && x.damage > 0 && x.ammo == 0 && x.pick == 0 && x.hammer == 0 && x.axe == 0).Select(x => x.type)
            .Except(近战武器).Except(远程武器).Except(魔法武器).Except(召唤法杖).Except(召唤鞭子).ToArray();

            var 建筑类工具 = item.Where(x => x.type != 0 && (ItemID.Sets.AlsoABuildingItem[x.type] ||
            x.pick > 0 || x.axe > 0 || x.hammer > 0)).Select(typeSelector).Except(电路工具).ToArray();
            var 更改环境工具 = item.Where(static x => x != null && (ItemID.Sets.SortingPriorityTerraforming[x.type] > -1 ||
            ItemID.Sets.GrassSeeds[x.type])).Select(typeSelector).ToArray();

            var Group_8 = new HashSet<int>();
            Group_8.UnionWith(抓钩);
            Group_8.UnionWith(信号枪);
            Group_8.UnionWith(建筑类工具);
            Group_8.UnionWith(更改环境工具);

            var 坐骑 = item.Where(x => x.mountType != -1 && !MountID.Sets.Cart[x.mountType]).Select(typeSelector).ToArray();
            var 矿车 = item.Where(x => x.mountType != -1 && MountID.Sets.Cart[x.mountType]).Select(typeSelector).
            Union(new[] { 3353, 3354, 3355, 3356 }).ToArray();

            var Group_9 = new HashSet<int>();
            Group_9.UnionWith(坐骑);
            Group_9.UnionWith(矿车);

            var 宠物 = item.Where(x => x.buffType > 0 && Main.vanityPet[x.buffType]).Select(typeSelector).ToArray();
            var 照明宠物 = item.Where(x => x.buffType > 0 && Main.lightPet[x.buffType]).Select(typeSelector).ToArray();

            var Group_10 = new HashSet<int>();
            Group_10.UnionWith(宠物);
            Group_10.UnionWith(照明宠物);

            var 奇异植物 = item.Where(x => ItemID.Sets.ExoticPlantsForDyeTrade[x.type]).Select(typeSelector).ToArray();
            var 染发剂 = item.Where(x => x.hairDye >= 0 || x.Name.EndsWith("HairDye")).Select(typeSelector).Union(new[] { 1990 }).ToArray();
            var 喷漆 = typeof(ItemID).GetFields().Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Paint")).Select(intValueSelector).ToArray();
            var 染料 = typeof(ItemID).GetFields().Where(static x => x.IsLiteral)
                .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Dye")).Select(intValueSelector).
                Union(new[] { 1107, 1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 3385, 3386, 3387, 3388 }).ToArray();

            var Group_11 = new HashSet<int>();
            Group_11.UnionWith(染料);
            Group_11.UnionWith(喷漆);
            Group_11.UnionWith(奇异植物);
            Group_11.UnionWith(染发剂);

            var 八音盒 = item.Where(x => x.Name.StartsWith("MusicBox")).Select(typeSelector).Union(new int[] { 576 }).ToArray();

            var 战斗盔甲 = item.Where(static x => x != null &&
            (x.bodySlot > 0 || x.headSlot > 0 || x.legSlot > 0) && !x.vanity && x.defense >= 1).Select(typeSelector)
            .Union(new int[] { 803, 804, 805, 1316, 1317, 1318 }).Except(new int[] { 205, 268 }).ToArray();

            var 装饰盔甲 = item.Where(static x => x != null &&
            (x.bodySlot > 0 || x.headSlot > 0 || x.legSlot > 0) && x.vanity && x.defense <= 0).Select(typeSelector)
            .Except(new int[] { 250, 264, 715, 4275 }).ToArray();

            var 服饰 = item.Where(static x => x.accessory && x.vanity).Select(typeSelector)
            .Except(八音盒).Except(new int[] { 576, 3536, 3537, 3538, 3539, 4318, 4054, 5345, 5347 }).Union(new int[] { 1522, 1523, 1524, 1525, 1526, 1527 }).ToArray();

            var Group_12 = new HashSet<int>();
            Group_12.UnionWith(装饰盔甲);
            Group_12.UnionWith(服饰);

            var 十字章护盾 = new int[] { 156, 193, 397, 885, 886, 887, 888, 889, 890, 891, 892, 893, 901, 902, 903, 904, 1612, 3781, 5354 };

            var 贝壳电话 = new int[] { 17, 18, 50, 393, 395, 709, 3036, 3037, 3084, 3095, 3096, 3099, 3102, 3118, 3119, 3120, 3121, 3122, 3123, 3124, 3199, 4263, 4819, 5358, 5359, 5360, 5361, 5437 };

            var 饰品 = item.Where(static x => x.accessory && ItemID.Sets.CanGetPrefixes[x.type] && !x.vanity).Select(typeSelector).Except(装饰盔甲)
            .Except(new int[] { 2799, 3097, 3536, 3537, 3538, 3539, 3619, 3624, 4318, 5139, 5140, 5141, 5142, 5143, 5144, 5145, 5146, 5345, 5347 })
            .Except(Enumerable.Range(4242, 14))
            .Except(十字章护盾)
            .Except(贝壳电话)
            .Union(new int[] { 268 }).ToArray();

            var 弹药 = item.Where(x => x.ammo > 0 && x.damage > 0 && !ItemID.Sets.CommonCoin[x.type]).Select(typeSelector)
            .Union(new[] { 69, 97, 1261, 1168, 1432, 1783, 1785, 1836, 3103, 3104 }).Distinct().ToArray();

            var 装备合成材料 = item.Where(x => ItemID.Sets.ItemIconPulse[x.type] && x.type != 3580 && x.type != 3581).Select(typeSelector)
            .Union(new int[]
            {
                86, 118, 210, 225, 236, 259, 324, 331, 501, 507,
                508, 522, 526, 527, 528, 531, 1328, 1329, 1332,
                1339, 1345, 1346, 1347, 1348, 1508, 1516, 1517,
                1518, 1519, 1520, 1521, 1570, 1611, 1811, 1831,
                2218, 2607, 2161, 2431, 3111, 3783, 3794, 5070,
            }).ToArray();

            var 召唤物材料 = item.Where(static x => x.netID > 0 && ItemID.Sets.SortingPriorityBossSpawns[x.netID] > -1).Select(typeSelector)
            .Union(new int[] { 23, 38, 60, 264, 362, 715, 2766, 2887, 4961 })
            .Except(new int[] { 29, 50, 75, 109, 1291, 3124, 3199, 4263, 4819, 5289, 5337, 5338, 5339, 5340, 5342, 5343, 5358, 5359, 5360, 5361, 5437 }).ToArray();

            var Group_13 = new HashSet<int>();
            Group_13.UnionWith(装备合成材料);
            Group_13.UnionWith(召唤物材料);

            var 矿石 = fields.Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Ore")).Select(intValueSelector).
            Union(new[] { 173, 174, 177, 178, 179, 180, 181, 182 }).ToArray();

            var 锭 = fields.Where(static x => x.IsLiteral)
            .Where(static x => x.Name.Length > 3 && x.Name.EndsWith("Bar")).Select(intValueSelector)
            .Union(new int[] { }).ToArray();

            var Group_14 = new HashSet<int>();
            Group_14.UnionWith(矿石);
            Group_14.UnionWith(锭);

            var 排除以上物品 = new HashSet<int>(
                方块.Concat(环境方块).Concat(炼药材料).Concat(任务鱼).Concat(钓鱼匣子).Concat(钓鱼)
                .Concat(风筝).Concat(八音盒).Concat(食物).Concat(旗帜)
               .Concat(平台).Concat(锭).Concat(矿石).Concat(晶塔).Concat(增益药水).Concat(家具)
               .Concat(装备合成材料).Concat(十字章护盾).Concat(建筑类工具)
               .Concat(火把).Concat(荧光棒).Concat(宝藏袋).Concat(环境钥匙).Concat(近战武器).Concat(召唤鞭子)
               .Concat(电路工具).Concat(更改环境工具).Concat(坐骑).Concat(矿车).Concat(宠物).Concat(照明宠物)
               .Concat(染发剂).Concat(染料).Concat(喷漆).Concat(战斗盔甲).Concat(装饰盔甲).Concat(饰品).Concat(服饰)
               .Concat(魔法武器).Concat(远程武器).Concat(投掷物).Concat(召唤法杖).Concat(信号枪).Concat(弹药)
               .Concat(召唤物材料).Concat(贝壳电话));
            var 杂物 = item.Where(static x => x.material && x.createTile == -1 && x.createWall == -1).Select(typeSelector).Except(排除以上物品).ToArray();

            config = new Configuration(new List<ClassificationInfo>
            { 
                new ClassificationInfo("火把、绳子、荧光棒",Group_1.ToArray(),Group_1.ToArray()),
                new ClassificationInfo("旗帜",旗帜,旗帜),
                new ClassificationInfo("宝藏袋、环境钥匙",Group_2.ToArray(),Group_2.ToArray()),
                new ClassificationInfo("炼药材料",炼药材料,炼药材料),
                new ClassificationInfo("任务鱼", 任务鱼, 任务鱼),
                new ClassificationInfo("钓鱼、钓鱼匣子",Group_3.ToArray(),Group_3.ToArray()),
                new ClassificationInfo("风筝", 风筝, 风筝),
                new ClassificationInfo("食物、增益药水", Group_4.ToArray(), Group_4.ToArray()),
                new ClassificationInfo("电路工具", 电路工具, 电路工具),
                new ClassificationInfo("晶塔、家具", Group_5.ToArray(), Group_5.ToArray()),
                new ClassificationInfo("平台、方块、环境方块", Group_6.ToArray(), Group_6.ToArray()),
                new ClassificationInfo("近战武器", 近战武器, 近战武器),
                new ClassificationInfo("魔法武器", 魔法武器, 魔法武器),
                new ClassificationInfo("远程武器", 远程武器, 远程武器),
                new ClassificationInfo("投掷物", 投掷物, 投掷物),
                new ClassificationInfo("召唤法杖、召唤鞭子", Group_7.ToArray(), Group_7.ToArray()),
                new ClassificationInfo("抓钩、信号枪、建筑类工具、更改环境工具", Group_8.ToArray(), Group_8.ToArray()),
                new ClassificationInfo("坐骑、矿车", Group_9.ToArray(), Group_9.ToArray()),
                new ClassificationInfo("宠物、照明宠物", Group_10.ToArray(), Group_10.ToArray()),
                new ClassificationInfo("奇异植物、染发剂、喷漆、染料", Group_11.ToArray(), Group_11.ToArray()),
                new ClassificationInfo("八音盒", 八音盒, 八音盒),
                new ClassificationInfo("战斗盔甲", 战斗盔甲, 战斗盔甲),
                new ClassificationInfo("装饰盔甲、服饰", Group_12.ToArray(), Group_12.ToArray()),
                new ClassificationInfo("十字章护盾", 十字章护盾, 十字章护盾),
                new ClassificationInfo("贝壳电话", 贝壳电话, 贝壳电话),
                new ClassificationInfo("饰品", 饰品, 饰品),
                new ClassificationInfo("弹药", 弹药, 弹药),
                new ClassificationInfo("装备合成材料、召唤物材料", Group_13.ToArray(), Group_13.ToArray()),
                new ClassificationInfo("矿石、锭", Group_14.ToArray(), Group_14.ToArray()),
                new ClassificationInfo("杂物", 杂物, 杂物),
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