using CaiBotLite.Attributes;
using Terraria;
using Terraria.GameContent.Events;

namespace CaiBotLite.Enums;

public enum ProgressType
{

    [ProgressName("克眼", "克苏鲁之眼")]
    [ProgressMap(nameof(NPC.downedBoss1), true)]
    EyeOfCthulhu,

    [ProgressName("史莱姆王", "史莱姆之王", "史王")]
    [ProgressMap(nameof(NPC.downedSlimeKing), true)]
    SlimeKing,

    [ProgressName("克脑", "克苏鲁之脑", "世界吞噬者", "世界吞噬怪", "世吞")]
    [ProgressMap(nameof(NPC.downedBoss2), true)]
    EvilBoss,

    [ProgressName("骷髅王")]
    [ProgressMap(nameof(NPC.downedBoss3), true)]
    Skeletron,

    [ProgressName("蜂王")]
    [ProgressMap(nameof(NPC.downedQueenBee), true)]
    QueenBee,

    [ProgressName("鹿角怪", "独眼巨鹿")]
    [ProgressMap(nameof(NPC.downedDeerclops), true)]
    Deerclops,

    [ProgressName("血肉墙", "肉山", "肉后", "困难模式")]
    [ProgressMap(nameof(Main.hardMode), true)]
    WallOfFlesh,

    [ProgressName("一王后")]
    [ProgressMap(nameof(NPC.downedMechBossAny), true)]
    MechBossAny,

    [ProgressName("双子魔眼")]
    [ProgressMap(nameof(NPC.downedMechBoss2), true)]
    TheTwins,

    [ProgressName("毁灭者", "铁长直")]
    [ProgressMap(nameof(NPC.downedMechBoss1), true)]
    TheDestroyer,

    [ProgressName("机械骷髅王")]
    [ProgressMap(nameof(NPC.downedMechBoss3), true)]
    SkeletronPrime,

    [ProgressName("世纪之花", "世花")]
    [ProgressMap(nameof(NPC.downedPlantBoss), true)]
    Plantera,

    [ProgressName("石巨人")]
    [ProgressMap(nameof(NPC.downedGolemBoss), true)]
    Golem,

    [ProgressName("猪鲨", "猪龙鱼公爵")]
    [ProgressMap(nameof(NPC.downedFishron), true)]
    DukeFishron,

    [ProgressName("拜月教邪教徒", "拜月")]
    [ProgressMap(nameof(NPC.downedAncientCultist), true)]
    LunaticCultist,

    [ProgressName("月球领主", "月亮领主", "月总")]
    [ProgressMap(nameof(NPC.downedMoonlord), true)]
    Moonlord,

    [ProgressName("光之女皇", "光女")]
    [ProgressMap(nameof(NPC.downedEmpressOfLight), true)]
    EmpressOfLight,

    [ProgressName("史莱姆皇后", "史后")]
    [ProgressMap(nameof(NPC.downedQueenSlime), true)]
    QieenSlime,

    [ProgressName("哀木")]
    [ProgressMap(nameof(NPC.downedHalloweenTree), true)]
    HalloweenTree,

    [ProgressName("南瓜王")]
    [ProgressMap(nameof(NPC.downedHalloweenKing), true)]
    HalloweenKing,

    [ProgressName("长绿尖叫怪")]
    [ProgressMap(nameof(NPC.downedChristmasTree), true)]
    ChristmasTree,

    [ProgressName("冰雪女皇")]
    [ProgressMap(nameof(NPC.downedChristmasIceQueen), true)]
    ChristmasIceQueen,

    [ProgressName("圣诞坦克")]
    [ProgressMap(nameof(NPC.downedChristmasSantank), true)]
    ChristmasSantank,

    [ProgressName("火星飞碟", "飞碟")]
    [ProgressMap(nameof(NPC.downedMartians), true)]
    Martians,

    [ProgressName("小丑")]
    [ProgressMap(nameof(NPC.downedClown), true)]
    Clown,

    [ProgressName("日耀柱")]
    [ProgressMap(nameof(NPC.downedTowerSolar), true)]
    TowerSolar,

    [ProgressName("星旋柱")]
    [ProgressMap(nameof(NPC.downedTowerVortex), true)]
    TowerVortex,

    [ProgressName("星云柱")]
    [ProgressMap(nameof(NPC.downedTowerNebula), true)]
    TowerNebula,

    [ProgressName("星尘柱")]
    [ProgressMap(nameof(NPC.downedTowerStardust), true)]
    TowerStardust,

    [ProgressName("哥布林入侵", "哥布林")]
    [ProgressMap(nameof(NPC.downedGoblins), true)]
    Goblins,

    [ProgressName("海盗入侵", "海盗")]
    [ProgressMap(nameof(NPC.downedPirates), true)]
    Pirates,

    [ProgressName("霜月")]
    [ProgressMap(nameof(NPC.downedFrost), true)]
    Frost,

    [ProgressName("血月")]
    [ProgressMap(nameof(Main.bloodMoon), true)]
    BloodMoon,

    [ProgressName("旧日一", "黑暗法师")]
    [ProgressMap(nameof(DD2Event._downedDarkMageT1), true)]
    DrakMageT1,

    [ProgressName("旧日二", "巨魔")]
    [ProgressMap(nameof(DD2Event._downedOgreT2), true)]
    OrgeT2,

    [ProgressName("旧日三", "贝蒂斯", "双足翼龙")]
    [ProgressMap(nameof(DD2Event._spawnedBetsyT3), true)]
    BetsyT3,

    [ProgressName("雨天")]
    [ProgressMap(nameof(Main.raining), true)]
    Raining,

    [ProgressName("白天")]
    [ProgressMap(nameof(Main.dayTime), true)]
    DyaTime,

    [ProgressName("夜晚")]
    [ProgressMap(nameof(Main.dayTime), false)]
    Night,

    [ProgressName("大风天")]
    [ProgressMap(nameof(Main.IsItAHappyWindyDay), true)]
    WindyDay,

    [ProgressName("万圣节")]
    [ProgressMap(nameof(Main.halloween), true)]
    Halloween,

    [ProgressName("派对")]
    [ProgressMap(nameof(BirthdayParty.PartyIsUp), true)]
    Party,

    [ProgressName("2020", "醉酒世界")]
    [ProgressMap(nameof(Main.drunkWorld), true)]
    DrunkWorld,

    [ProgressName("2021", "十周年")]
    [ProgressMap(nameof(Main.tenthAnniversaryWorld), true)]
    tenthAnniversaryWorld,

    [ProgressName("ftw")]
    [ProgressMap(nameof(Main.getGoodWorld), true)]
    ForTheWorthy,

    [ProgressName("混乱世界", "颠倒世界")]
    [ProgressMap(nameof(Main.remixWorld), true)]
    RemixWorld,

    [ProgressName("ntb", "蜂蜜世界")]
    [ProgressMap(nameof(Main.notTheBeesWorld), true)]
    NotTheBeesWorld,

    [ProgressName("永恒领域", "饥荒")]
    [ProgressMap(nameof(Main.dontStarveWorld), true)]
    DontStarveWorld,

    [ProgressName("天顶世界", "天顶")]
    [ProgressMap(nameof(Main.zenithWorld), true)]
    zenithWorld,

    [ProgressName("危机世界")]
    [ProgressMap(nameof(Main.noTrapsWorld), true)]
    NoTrapsWorld,

    
    [ProgressName("森林")]
    [ProgressMap(nameof(Player.ShoppingZone_Forest), true)]
    ShoppingZone_Forest,

    [ProgressName("丛林")]
    [ProgressMap(nameof(Player.ZoneJungle), true)]
    ZoneJungle,

    [ProgressName("沙漠")]
    [ProgressMap(nameof(Player.ZoneDesert), true)]
    ZoneDesert,

    [ProgressName("雪原")]
    [ProgressMap(nameof(Player.ZoneSnow), true)]
    ZoneSnow,

    [ProgressName("洞穴")]
    [ProgressMap(nameof(Player.ZoneUnderworldHeight), true)]
    ZoneUnderworldHeight,

    [ProgressName("海洋")]
    [ProgressMap(nameof(Player.ZoneBeach), true)]
    ZoneBeach,

    [ProgressName("神圣")]
    [ProgressMap(nameof(Player.ZoneHallow), true)]
    ZoneHallow,

    [ProgressName("蘑菇")]
    [ProgressMap(nameof(Player.ZoneGlowshroom), true)]
    ZoneGlowshroom,

    [ProgressName("微光")]
    [ProgressMap(nameof(Player.ZoneShimmer), true)]
    ZoneShimmer,

    [ProgressName("腐化之地", "腐化")]
    [ProgressMap(nameof(Player.ZoneCorrupt), true)]
    ZoneCorrupt,

    [ProgressName("猩红", "猩红之地")]
    [ProgressMap(nameof(Player.ZoneCrimson), true)]
    ZoneCrimson,

    [ProgressName("地牢")]
    [ProgressMap(nameof(Player.ZoneDungeon), true)]
    ZoneDungeon,

    [ProgressName("墓地")]
    [ProgressMap(nameof(Player.ZoneGraveyard), true)]
    ZoneGraveyard,

    [ProgressName("神庙")]
    [ProgressMap(nameof(Player.ZoneLihzhardTemple), true)]
    ZoneLihzhardTemple,

    [ProgressName("蜂巢")]
    [ProgressMap(nameof(Player.ZoneHive), true)]
    ZoneHive,

    [ProgressName("沙尘暴")]
    [ProgressMap(nameof(Player.ZoneSandstorm), true)]
    ZoneSandstorm,

    [ProgressName("天空")]
    [ProgressMap(nameof(Player.ZoneSkyHeight), true)]
    ZoneSkyHeight,

    [ProgressName("岩层")]
    [ProgressMap(nameof(Player.ZoneRockLayerHeight), true)]
    ZoneRockLayerHeight,

    [ProgressName("土层")]
    [ProgressMap(nameof(Player.ZoneDirtLayerHeight), true)]
    ZoneDirtLayerHeight,

    [ProgressName("地狱")]
    [ProgressMap(nameof(Player.inferno), true)]
    Inferno,

    [ProgressName("地下沙漠")]
    [ProgressMap(nameof(Player.ZoneUndergroundDesert), true)]
    ZoneUndergroundDesert,

    [ProgressName("满月")]
    [ProgressMap(nameof(Main.moonType), 0)]
    FullMoon,

    [ProgressName("亏凸月")]
    [ProgressMap(nameof(Main.moonType), 1)]
    WaningGibbous,

    [ProgressName("下弦月")]
    [ProgressMap(nameof(Main.moonType), 2)]
    ThirdQuarter,

    [ProgressName("残月")]
    [ProgressMap(nameof(Main.moonType), 3)]
    WaningCrescen,

    [ProgressName("新月")]
    [ProgressMap(nameof(Main.moonType), 4)]
    NewMoon,

    [ProgressName("娥眉月")]
    [ProgressMap(nameof(Main.moonType), 5)]
    WaxingCrescent,

    [ProgressName("上弦月")]
    [ProgressMap(nameof(Main.moonType), 6)]
    FirstQuarter,

    [ProgressName("盈凸月")]
    [ProgressMap(nameof(Main.moonType), 7)]
    WaxingGibbous,
    
    [ProgressName("U")]
    [ProgressMap(nameof(Main.dedServ), true)]
    Unknown,
    
}