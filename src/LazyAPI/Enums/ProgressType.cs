using LazyAPI.Attributes;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace LazyAPI.Enums;

public enum ProgressType
{
    #region 事件与进度
    [ProgressName("无限制")]
    [ProgressMap(nameof(Main.maxPlayers), typeof(Main), 255)]
    NoLimit,

    [ProgressName("克眼", "克苏鲁之眼")]
    [ProgressMapID(NPCID.EyeofCthulhu)]
    [ProgressMap(nameof(NPC.downedBoss1), typeof(NPC), true)]
    EyeOfCthulhu,

    [ProgressName("史莱姆王", "史莱姆之王", "史王")]
    [ProgressMapID(NPCID.KingSlime)]
    [ProgressMap(nameof(NPC.downedSlimeKing), typeof(NPC), true)]
    SlimeKing,

    [ProgressName("世界吞噬者", "世界吞噬怪", "世吞")]
    [ProgressMapID(NPCID.EaterofWorldsHead)]
    [ProgressMap(nameof(NPC.downedBoss2), typeof(NPC), true)]
    EvilBoss,

    [ProgressName("克脑", "克苏鲁之脑")]
    [ProgressMapID(NPCID.BrainofCthulhu, NPCID.Creeper)]
    [ProgressMap(nameof(NPC.downedBoss2), typeof(NPC), true)]
    Brainof,

    [ProgressName("骷髅王")]
    [ProgressMapID(NPCID.SkeletronHead)]
    [ProgressMap(nameof(NPC.downedBoss3), typeof(NPC), true)]
    Skeletron,

    [ProgressName("蜂王")]
    [ProgressMapID(NPCID.QueenBee)]
    [ProgressMap(nameof(NPC.downedQueenBee), typeof(NPC), true)]
    QueenBee,

    [ProgressName("鹿角怪", "独眼巨鹿")]
    [ProgressMapID(NPCID.Deerclops)]
    [ProgressMap(nameof(NPC.downedDeerclops), typeof(NPC), true)]
    Deerclops,

    [ProgressName("血肉墙", "肉山", "肉后", "困难模式")]
    [ProgressMapID(NPCID.WallofFlesh)]
    [ProgressMap(nameof(Main.hardMode), typeof(Main), true)]
    WallOfFlesh,

    [ProgressName("双子魔眼")]
    [ProgressMapID(NPCID.Retinazer, NPCID.Spazmatism)]
    [ProgressMap(nameof(NPC.downedMechBoss2), typeof(NPC), true)]
    TheTwins,

    [ProgressName("毁灭者", "铁长直")]
    [ProgressMapID(NPCID.TheDestroyer)]
    [ProgressMap(nameof(NPC.downedMechBoss1), typeof(NPC), true)]
    TheDestroyer,

    [ProgressName("机械骷髅王")]
    [ProgressMapID(NPCID.SkeletronPrime)]
    [ProgressMap(nameof(NPC.downedMechBoss3), typeof(NPC), true)]
    SkeletronPrime,

    [ProgressName("世纪之花", "世花")]
    [ProgressMapID(NPCID.Plantera)]
    [ProgressMap(nameof(NPC.downedPlantBoss), typeof(NPC), true)]
    Plantera,

    [ProgressName("石巨人")]
    [ProgressMapID(NPCID.GolemHead)]
    [ProgressMap(nameof(NPC.downedGolemBoss), typeof(NPC), true)]
    Golem,

    [ProgressName("猪鲨", "猪龙鱼公爵")]
    [ProgressMapID(NPCID.DukeFishron)]
    [ProgressMap(nameof(NPC.downedFishron), typeof(NPC), true)]
    DukeFishron,

    [ProgressName("拜月教邪教徒", "拜月")]
    [ProgressMapID(NPCID.CultistBoss)]
    [ProgressMap(nameof(NPC.downedAncientCultist), typeof(NPC), true)]
    LunaticCultist,

    [ProgressName("月球领主", "月亮领主", "月总")]
    [ProgressMapID(NPCID.MoonLordCore)]
    [ProgressMap(nameof(NPC.downedMoonlord), typeof(NPC), true)]
    Moonlord,

    [ProgressName("光之女皇", "光女")]
    [ProgressMapID(NPCID.HallowBoss)]
    [ProgressMap(nameof(NPC.downedEmpressOfLight), typeof(NPC), true)]
    EmpressOfLight,

    [ProgressName("史莱姆皇后", "史后")]
    [ProgressMapID(NPCID.QueenSlimeBoss)]
    [ProgressMap(nameof(NPC.downedQueenSlime), typeof(NPC), true)]
    QieenSlime,

    [ProgressName("哀木")]
    [ProgressMapID(NPCID.MourningWood)]
    [ProgressMap(nameof(NPC.downedHalloweenTree), typeof(NPC), true)]
    HalloweenTree,

    [ProgressName("南瓜王")]
    [ProgressMapID(NPCID.Pumpking)]
    [ProgressMap(nameof(NPC.downedHalloweenKing), typeof(NPC), true)]
    HalloweenKing,

    [ProgressName("长绿尖叫怪")]
    [ProgressMapID(NPCID.Everscream)]
    [ProgressMap(nameof(NPC.downedChristmasTree), typeof(NPC), true)]
    ChristmasTree,

    [ProgressName("冰雪女皇")]
    [ProgressMapID(NPCID.IceQueen)]
    [ProgressMap(nameof(NPC.downedChristmasIceQueen), typeof(NPC), true)]
    ChristmasIceQueen,

    [ProgressName("圣诞坦克")]
    [ProgressMapID(NPCID.SantaNK1)]
    [ProgressMap(nameof(NPC.downedChristmasSantank), typeof(NPC), true)]
    ChristmasSantank,

    [ProgressName("火星飞碟", "飞碟")]
    [ProgressMapID(NPCID.MartianSaucer)]
    [ProgressMap(nameof(NPC.downedMartians), typeof(NPC), true)]
    Martians,

    [ProgressName("小丑")]
    [ProgressMapID(NPCID.Clown)]
    [ProgressMap(nameof(NPC.downedClown), typeof(NPC), true)]
    Clown,

    [ProgressName("日耀柱")]
    [ProgressMapID(NPCID.MartianSaucer)]
    [ProgressMap(nameof(NPC.downedTowerSolar), typeof(NPC), true)]
    TowerSolar,

    [ProgressName("星旋柱")]
    [ProgressMapID(NPCID.MartianSaucer)]
    [ProgressMap(nameof(NPC.downedTowerVortex), typeof(NPC), true)]
    TowerVortex,

    [ProgressName("星云柱")]
    [ProgressMapID(NPCID.MartianSaucer)]
    [ProgressMap(nameof(NPC.downedTowerNebula), typeof(NPC), true)]
    TowerNebula,

    [ProgressName("星尘柱")]
    [ProgressMapID(NPCID.MartianSaucer)]
    [ProgressMap(nameof(NPC.downedTowerStardust), typeof(NPC), true)]
    TowerStardust,

    [ProgressName("哥布林入侵", "哥布林")]
    [ProgressMap(nameof(NPC.downedGoblins), typeof(NPC), true)]
    Goblins,

    [ProgressName("海盗入侵", "海盗")]
    [ProgressMap(nameof(NPC.downedPirates), typeof(NPC), true)]
    Pirates,

    [ProgressName("霜月")]
    [ProgressMap(nameof(NPC.downedFrost), typeof(NPC), true)]
    Frost,

    [ProgressName("血月")]
    [ProgressMap(nameof(Main.bloodMoon), typeof(Main), true)]
    BloodMoon,

    [ProgressName("旧日一", "黑暗法师")]
    [ProgressMap(nameof(DD2Event._downedDarkMageT1), typeof(DD2Event), true)]
    DrakMageT1,

    [ProgressName("旧日二", "巨魔")]
    [ProgressMap(nameof(DD2Event._downedOgreT2), typeof(DD2Event), true)]
    OrgeT2,

    [ProgressName("旧日三", "贝蒂斯", "双足翼龙")]
    [ProgressMap(nameof(DD2Event._spawnedBetsyT3), typeof(DD2Event), true)]
    BetsyT3,

    [ProgressName("雨天")]
    [ProgressMap(nameof(Main.raining), typeof(Main), true)]
    Raining,

    [ProgressName("白天")]
    [ProgressMap(nameof(Main.dayTime), typeof(Main), true)]
    DyaTime,

    [ProgressName("夜晚")]
    [ProgressMap(nameof(Main.dayTime), typeof(Main), false)]
    Night,

    [ProgressName("大风天")]
    [ProgressMap(nameof(Main.IsItAHappyWindyDay), typeof(Main), true)]
    WindyDay,

    [ProgressName("万圣节")]
    [ProgressMap(nameof(Main.halloween), typeof(Main), true)]
    Halloween,

    [ProgressName("派对")]
    [ProgressMap(nameof(BirthdayParty.PartyIsUp), typeof(BirthdayParty), true)]
    Party,
    #endregion

    #region 秘密世界
    [ProgressName("2020", "醉酒世界")]
    [ProgressMap(nameof(Main.drunkWorld), typeof(Main), true)]
    DrunkWorld,

    [ProgressName("2021", "十周年")]
    [ProgressMap(nameof(Main.tenthAnniversaryWorld), typeof(Main), true)]
    tenthAnniversaryWorld,

    [ProgressName("ftw")]
    [ProgressMap(nameof(Main.getGoodWorld), typeof(Main), true)]
    ForTheWorthy,

    [ProgressName("混乱世界", "颠倒世界")]
    [ProgressMap(nameof(Main.remixWorld), typeof(Main), true)]
    RemixWorld,

    [ProgressName("ntb", "蜂蜜世界")]
    [ProgressMap(nameof(Main.notTheBeesWorld), typeof(Main), true)]
    NotTheBeesWorld,

    [ProgressName("永恒领域", "饥荒")]
    [ProgressMap(nameof(Main.dontStarveWorld), typeof(Main), true)]
    DontStarveWorld,

    [ProgressName("天顶世界", "天顶")]
    [ProgressMap(nameof(Main.zenithWorld), typeof(Main), true)]
    zenithWorld,

    [ProgressName("危机世界")]
    [ProgressMap(nameof(Main.noTrapsWorld), typeof(Main), true)]
    NoTrapsWorld,
    #endregion

    #region 环境
    [ProgressName("森林")]
    [ProgressMap(nameof(Player.ShoppingZone_Forest), typeof(Player), true)]
    ShoppingZone_Forest,

    [ProgressName("丛林")]
    [ProgressMap(nameof(Player.ZoneJungle), typeof(Player), true)]
    ZoneJungle,

    [ProgressName("沙漠")]
    [ProgressMap(nameof(Player.ZoneDesert), typeof(Player), true)]
    ZoneDesert,

    [ProgressName("雪原")]
    [ProgressMap(nameof(Player.ZoneSnow), typeof(Player), true)]
    ZoneSnow,

    [ProgressName("洞穴")]
    [ProgressMap(nameof(Player.ZoneUnderworldHeight), typeof(Player), true)]
    ZoneUnderworldHeight,

    [ProgressName("海洋")]
    [ProgressMap(nameof(Player.ZoneBeach), typeof(Player), true)]
    ZoneBeach,

    [ProgressName("神圣")]
    [ProgressMap(nameof(Player.ZoneHallow), typeof(Player), true)]
    ZoneHallow,

    [ProgressName("蘑菇")]
    [ProgressMap(nameof(Player.ZoneGlowshroom), typeof(Player), true)]
    ZoneGlowshroom,

    [ProgressName("微光")]
    [ProgressMap(nameof(Player.ZoneShimmer), typeof(Player), true)]
    ZoneShimmer,

    [ProgressName("腐化之地", "腐化")]
    [ProgressMap(nameof(Player.ZoneCorrupt), typeof(Player), true)]
    ZoneCorrupt,

    [ProgressName("猩红", "猩红之地")]
    [ProgressMap(nameof(Player.ZoneCrimson), typeof(Player), true)]
    ZoneCrimson,

    [ProgressName("地牢")]
    [ProgressMap(nameof(Player.ZoneDungeon), typeof(Player), true)]
    ZoneDungeon,

    [ProgressName("墓地")]
    [ProgressMap(nameof(Player.ZoneGraveyard), typeof(Player), true)]
    ZoneGraveyard,

    [ProgressName("神庙")]
    [ProgressMap(nameof(Player.ZoneLihzhardTemple), typeof(Player), true)]
    ZoneLihzhardTemple,

    [ProgressName("蜂巢")]
    [ProgressMap(nameof(Player.ZoneHive), typeof(Player), true)]
    ZoneHive,

    [ProgressName("沙尘暴")]
    [ProgressMap(nameof(Player.ZoneSandstorm), typeof(Player), true)]
    ZoneSandstorm,

    [ProgressName("天空")]
    [ProgressMap(nameof(Player.ZoneSkyHeight), typeof(Player), true)]
    ZoneSkyHeight,

    [ProgressName("岩层")]
    [ProgressMap(nameof(Player.ZoneRockLayerHeight), typeof(Player), true)]
    ZoneRockLayerHeight,

    [ProgressName("土层")]
    [ProgressMap(nameof(Player.ZoneDirtLayerHeight), typeof(Player), true)]
    ZoneDirtLayerHeight,

    [ProgressName("地狱")]
    [ProgressMap(nameof(Player.inferno), typeof(Player), true)]
    Inferno,

    [ProgressName("地下沙漠")]
    [ProgressMap(nameof(Player.ZoneUndergroundDesert), typeof(Player), true)]
    ZoneUndergroundDesert,
    #endregion

    #region 月相
    [ProgressName("满月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 0)]
    FullMoon,

    [ProgressName("亏凸月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 1)]
    WaningGibbous,

    [ProgressName("下弦月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 2)]
    ThirdQuarter,

    [ProgressName("残月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 3)]
    WaningCrescen,

    [ProgressName("新月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 4)]
    NewMoon,

    [ProgressName("娥眉月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 5)]
    WaxingCrescent,

    [ProgressName("上弦月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 6)]
    FirstQuarter,

    [ProgressName("盈凸月")]
    [ProgressMap(nameof(Main.moonType), typeof(Main), 7)]
    WaxingGibbous,
    #endregion
}