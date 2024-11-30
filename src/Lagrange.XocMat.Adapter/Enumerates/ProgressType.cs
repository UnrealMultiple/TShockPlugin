using Lagrange.XocMat.Adapter.Attributes;
using Terraria;

namespace Lagrange.XocMat.Adapter.Enumerates;

internal enum ProgressType
{
    [ProgressMatch("史莱姆王", typeof(NPC), nameof(NPC.downedSlimeKing))]
    SlimeKing,

    [ProgressMatch("克苏鲁之眼", typeof(NPC), nameof(NPC.downedBoss1))]
    EyeOfCthulhu,

    [ProgressMatch("克苏鲁之脑", typeof(NPC), nameof(NPC.downedBoss2))]
    EvilBoss,

    [ProgressMatch("世界吞噬怪", typeof(NPC), nameof(NPC.downedBoss2))]
    _EvilBoss,

    [ProgressMatch("骷髅王", typeof(NPC), nameof(NPC.downedBoss3))]
    Skeletron,

    [ProgressMatch("蜂王", typeof(NPC), nameof(NPC.downedQueenBee))]
    QueenBee,

    [ProgressMatch("鹿角怪", typeof(NPC), nameof(NPC.downedQueenBee))]
    Deerclops,

    [ProgressMatch("血肉墙", typeof(Main), nameof(Main.hardMode))]
    WallOfFlesh,

    [ProgressMatch("双子魔眼", typeof(NPC), nameof(NPC.downedMechBoss2))]
    TheTwins,

    [ProgressMatch("毁灭者", typeof(NPC), nameof(NPC.downedMechBoss1))]
    TheDestroyer,

    [ProgressMatch("机械骷髅王", typeof(NPC), nameof(NPC.downedMechBoss3))]
    SkeletronPrime,

    [ProgressMatch("世纪之花", typeof(NPC), nameof(NPC.downedPlantBoss))]
    Plantera,

    [ProgressMatch("石巨人", typeof(NPC), nameof(NPC.downedGolemBoss))]
    Golem,

    [ProgressMatch("猪龙鱼公爵", typeof(NPC), nameof(NPC.downedFishron))]
    DukeFishron,

    [ProgressMatch("拜月教邪教徒", typeof(NPC), nameof(NPC.downedAncientCultist))]
    LunaticCultist,

    [ProgressMatch("月亮领主", typeof(NPC), nameof(NPC.downedMoonlord))]
    Moonlord,

    [ProgressMatch("光之女皇", typeof(NPC), nameof(NPC.downedEmpressOfLight))]
    EmpressOfLight,

    [ProgressMatch("史莱姆皇后", typeof(NPC), nameof(NPC.downedQueenSlime))]
    QieenSlime,

    [ProgressMatch("南瓜月", typeof(NPC), nameof(NPC.downedHalloweenKing))]
    HalloweenKing,

    [ProgressMatch("火星暴乱", typeof(NPC), nameof(NPC.downedMartians))]
    Martians,

    [ProgressMatch("日耀柱", typeof(NPC), nameof(NPC.downedTowerSolar))]
    TowerSolar,

    [ProgressMatch("星旋柱", typeof(NPC), nameof(NPC.downedTowerVortex))]
    TowerVortex,

    [ProgressMatch("星云柱", typeof(NPC), nameof(NPC.downedTowerNebula))]
    TowerNebula,

    [ProgressMatch("星尘柱", typeof(NPC), nameof(NPC.downedTowerStardust))]
    TowerStardust,

    [ProgressMatch("哥布林军队", typeof(NPC), nameof(NPC.downedGoblins))]
    Goblins,

    [ProgressMatch("海盗入侵", typeof(NPC), nameof(NPC.downedPirates))]
    Pirates,

    [ProgressMatch("雪人军团", typeof(NPC), nameof(NPC.downedFrost))]
    Frost,

    [ProgressMatch("霜月", typeof(NPC), nameof(NPC.downedChristmasIceQueen))]
    ChristmasIceQueen
}
