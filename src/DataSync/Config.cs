using Newtonsoft.Json;
using System.Reflection;
using Terraria.ID;
using TShockAPI;

namespace DataSync;

[JsonConverter(typeof(Config.ProgressConverter))]
public enum ProgressType
{
    [Mapping(nameof(Terraria.NPC.downedSlimeKing), true)]
    [Match(NPCID.KingSlime)]
    [Alias("史莱姆王")]
    KingSlime,

    [Mapping(nameof(Terraria.NPC.downedBoss1), true)]
    [Match(NPCID.EyeofCthulhu)]
    [Alias("克苏鲁之眼")]
    EyeOfCthulhu,

    [Mapping(nameof(Terraria.NPC.downedBoss2), true)]
    [Match(NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.BrainofCthulhu)]
    [Alias("邪恶Boss", "世界吞噬者", "克苏鲁之脑")]
    PrehardmodeBoss2,

    [Mapping(nameof(Terraria.NPC.downedQueenBee), true)]
    [Match(NPCID.QueenBee)]
    [Alias("蜂后", "蜂王")]
    QueenBee,

    [Mapping(nameof(Terraria.NPC.downedBoss3), true)]
    [Match(NPCID.SkeletronHead, NPCID.SkeletronHand, NPCID.SkeletronHand, NPCID.SkeletronHand, NPCID.SkeletronHand)]
    [Alias("骷髅王")]
    Skeletron,

    [Mapping(nameof(Terraria.Main.hardMode), true)]
    [Match(NPCID.WallofFlesh, NPCID.WallofFleshEye, NPCID.TheHungry)]
    [Alias("肉山", "血肉之墙", "血肉墙")]
    WallOfFlesh,

    [Mapping(nameof(Terraria.NPC.downedMechBossAny), true)]
    [Match(NPCID.Retinazer, NPCID.Spazmatism, NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice)]
    [Alias("任一三王")]
    MechBoss,

    [Mapping(nameof(Terraria.NPC.downedMechBoss1), true)]
    [Match(NPCID.Retinazer, NPCID.Spazmatism)]
    [Alias("双子魔眼", "机械眼")]
    TheTwins,

    [Mapping(nameof(Terraria.NPC.downedMechBoss2), true)]
    [Match(NPCID.TheDestroyer, NPCID.TheDestroyerBody, NPCID.TheDestroyerTail)]
    [Alias("机械毁灭者", "毁灭者")]
    TheDestroyer,

    [Mapping(nameof(Terraria.NPC.downedMechBoss3), true)]
    [Match(NPCID.SkeletronPrime, NPCID.PrimeCannon, NPCID.PrimeLaser, NPCID.PrimeSaw, NPCID.PrimeVice)]
    [Alias("机械骷髅", "机械骷髅王")]
    SkeletronPrime,

    [Mapping(nameof(Terraria.NPC.downedPlantBoss), true)]
    [Match(NPCID.Plantera)]
    [Alias("世纪之花", "世花")]
    Plantera,

    [Mapping(nameof(Terraria.NPC.downedGolemBoss), true)]
    [Match(NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree)]
    [Alias("石巨人")]
    Golem,

    [Mapping(nameof(Terraria.NPC.downedFishron), true)]
    [Match(NPCID.DukeFishron)]
    [Alias("猪鲨公爵", "猪龙鱼公爵")]
    DukeFishron,

    [Mapping(nameof(Terraria.NPC.downedAncientCultist), true)]
    [Match(NPCID.CultistBoss)]
    [Alias("拜月教徒", "拜月教邪教徒", "教徒")]
    LunaticCultist,

    [Mapping(nameof(Terraria.NPC.downedMoonlord), true)]
    [Match(NPCID.MoonLordCore, NPCID.MoonLordHand, NPCID.MoonLordHead, NPCID.MoonLordFreeEye)]
    [Alias("月球领主", "月总")]
    MoonLord,

    [Mapping(nameof(Terraria.NPC.downedTowerSolar), true)]
    [Match(NPCID.SolarCrawltipedeHead, NPCID.SolarCrawltipedeBody, NPCID.SolarCrawltipedeTail, NPCID.SolarDrakomire, NPCID.SolarDrakomireRider, NPCID.LunarTowerSolar)]
    [Alias("日耀塔", "日耀柱")]
    SolarPillar,

    [Mapping(nameof(Terraria.NPC.downedTowerVortex), true)]
    [Match(NPCID.VortexHornetQueen, NPCID.VortexHornet, NPCID.VortexLarva, NPCID.VortexRifleman, NPCID.VortexSoldier, NPCID.LunarTowerVortex)]
    [Alias("星旋塔", "星旋柱")]
    VortexPillar,

    [Mapping(nameof(Terraria.NPC.downedTowerNebula), true)]
    [Match(NPCID.NebulaBrain, NPCID.NebulaBeast, NPCID.NebulaHeadcrab, NPCID.NebulaSoldier, NPCID.LunarTowerNebula)]
    [Alias("星云塔", "星云柱")]
    NebulaPillar,

    [Mapping(nameof(Terraria.NPC.downedTowerStardust), true)]
    [Match(NPCID.StardustCellBig, NPCID.StardustCellSmall, NPCID.StardustJellyfishBig, NPCID.StardustJellyfishSmall, NPCID.StardustSpiderBig, NPCID.StardustSpiderSmall, NPCID.LunarTowerStardust)]
    [Alias("星尘塔", "星尘柱")]
    StardustPillar,

    [Mapping(nameof(Terraria.NPC.downedChristmasIceQueen), true)]
    [Match(NPCID.IceQueen)]
    [Alias("冰雪女王")]
    ChristmasIceQueen,

    [Mapping(nameof(Terraria.NPC.downedChristmasSantank), true)]
    [Match(NPCID.SantaNK1)]
    [Alias("圣诞坦克")]
    ChristmasSantank,

    [Mapping(nameof(Terraria.NPC.downedChristmasTree), true)]
    [Match(NPCID.Everscream)]
    [Alias("永恒尖啸", "圣诞树", "常绿尖叫怪")]
    ChristmasTree,

    [Mapping(nameof(Terraria.NPC.downedHalloweenTree), true)]
    [Match(NPCID.MourningWood)]
    [Alias("哀嚎之木", "万圣树", "哀木")]
    HalloweenTree,

    [Mapping(nameof(Terraria.NPC.downedQueenSlime), true)]
    [Match(NPCID.QueenSlimeBoss)]
    [Alias("史莱姆皇后", "史莱姆女王")]
    QueenSlime,

    [Mapping(nameof(Terraria.NPC.downedDeerclops), true)]
    [Match(NPCID.Deerclops)]
    [Alias("独眼巨鹿", "鹿角怪")]
    Deerclops,

    [Mapping(nameof(Terraria.NPC.downedEmpressOfLight), true)]
    [Match(NPCID.HallowBoss)]
    [Alias("光之女皇", "光女")]
    EmpressOfLight,

    #region Invasion
    [Mapping(nameof(Terraria.Main.bloodMoon), true)]
    [Match(NPCID.TheGroom, NPCID.TheBride, NPCID.BloodZombie, NPCID.Drippler)]
    [Alias("血月")]
    BloodMoon,

    [Match(NPCID.GoblinShark, NPCID.BloodEelBody, NPCID.BloodEelHead, NPCID.BloodEelTail, NPCID.BloodNautilus, NPCID.BloodSquid)]
    [Alias("困难血月")]
    BloodMoonHardmode,

    [Mapping(nameof(Terraria.Main.eclipse), true)]
    [Match(NPCID.Vampire, NPCID.VampireBat, NPCID.Frankenstein, NPCID.CreatureFromTheDeep, NPCID.Eyezor, NPCID.ThePossessed, NPCID.Fritz, NPCID.SwampThing)]
    [Alias("日食", "日蚀")]
    SolarEclipse,

    [Match(NPCID.Reaper)]
    [Alias("三王后日食", "三王后日蚀")]
    SolarEclipseMech,

    [Match(NPCID.Mothron, NPCID.MothronSpawn, NPCID.Butcher, NPCID.DeadlySphere, NPCID.DrManFly, NPCID.Nailhead, NPCID.Psycho)]
    [Alias("花后日食", "花后日蚀")]
    SolarEclipsePlantera,

    [Mapping(nameof(Terraria.Main.pumpkinMoon), true)]
    [Match(NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5, NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10, NPCID.Splinterling, NPCID.Hellhound, NPCID.Poltergeist, NPCID.HeadlessHorseman, NPCID.MourningWood, NPCID.Pumpking, NPCID.PumpkingBlade)]
    [Alias("南瓜月")]
    PumpkinMoon,

    [Mapping(nameof(Terraria.Main.snowMoon), true)]
    [Match(NPCID.PresentMimic, NPCID.Flocko, NPCID.GingerbreadMan, NPCID.ZombieElf, NPCID.ElfArcher, NPCID.ElfCopter, NPCID.Nutcracker, NPCID.Krampus, NPCID.Everscream, NPCID.SantaNK1, NPCID.IceQueen)]
    [Alias("霜月", "雪月")]
    FrostMoon,

    [Mapping(nameof(Terraria.Main.invasionType), 4)]
    [Match(NPCID.MartianEngineer, NPCID.MartianOfficer, NPCID.MartianSaucerCore, NPCID.MartianSaucer, NPCID.MartianTurret, NPCID.MartianWalker)]
    [Alias("火星暴乱")]
    MartianMadness,

    [Mapping(nameof(Terraria.GameContent.Events.DD2Event.Ongoing), true)]
    [Alias("旧日军团")]
    OldOnesArmy,

    [Mapping(nameof(Terraria.Main.invasionType), 1)]
    [Match(NPCID.GoblinArcher, NPCID.GoblinPeon, NPCID.GoblinSorcerer, NPCID.GoblinThief, NPCID.GoblinWarrior)]
    [Alias("哥布林军团", "哥布林入侵", "哥布林军队")]
    GoblinsArmy,

    [Match(NPCID.GoblinSummoner, NPCID.ShadowFlameApparition)]
    [Alias("困难哥布林军团", "困难哥布林入侵", "困难哥布林军队")]
    GoblinsArmyHardmode,

    [Mapping(nameof(Terraria.Main.invasionType), 3)]
    [Match(NPCID.PirateCorsair, NPCID.PirateCrossbower, NPCID.PirateDeadeye, NPCID.PirateDeckhand, NPCID.PirateShip, NPCID.PirateCaptain)]
    [Alias("海盗军团", "海盗入侵")]
    PiratesArmy,

    [Mapping(nameof(Terraria.Main.invasionType), 2)]
    [Match(NPCID.MisterStabby, NPCID.SnowmanGangsta, NPCID.SnowBalla)]
    [Alias("雪人军团")]
    FrostLegion,

    [Mapping(nameof(Terraria.GameContent.Events.DD2Event._downedDarkMageT1), true)]
    [Match(NPCID.DD2DarkMageT1, NPCID.DD2DarkMageT3)]
    [Alias("一阶旧日军团", "暗黑法师")]
    DD2Mage,

    [Mapping(nameof(Terraria.GameContent.Events.DD2Event._downedOgreT2), true)]
    [Match(NPCID.DD2OgreT2, NPCID.DD2OgreT3)]
    [Alias("二阶旧日军团", "巨魔")]
    DD2Orge,

    [Mapping(nameof(Terraria.GameContent.Events.DD2Event._spawnedBetsyT3), true)]
    [Match(NPCID.DD2Betsy)]
    [Alias("三阶旧日军团", "贝蒂斯")]
    DD2Betsy,
    #endregion

    [Alias("不存在的进度")]
    [Mapping(nameof(Terraria.Main.renderCount), -1)]
    Unreachable,
}

public static class Config
{
    internal static readonly Dictionary<ProgressType, string> _default = typeof(ProgressType)
        .GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .ToDictionary(f => (ProgressType) f.GetValue(null)!, f => f.GetCustomAttribute<AliasAttribute>()!.Alias[0]);
    internal static readonly Dictionary<string, ProgressType> _names = typeof(ProgressType)
        .GetFields()
        .Where(f => f.FieldType == typeof(ProgressType))
        .SelectMany(field => field.GetCustomAttribute<AliasAttribute>()!.Alias.Select(a => (field, a)))
        .ToDictionary(t => t.a, t => (ProgressType) t.field.GetValue(null)!);

    public static string GetProgressName(ProgressType type)
    {
        return _default[type];
    }

    public static ProgressType? GetProgressType(string? name)
    {
        return name is not null && _names.TryGetValue(name, out var type) ? type : null;
    }

    public static Dictionary<ProgressType, bool> ShouldSyncProgress { get; set; } = new Dictionary<ProgressType, bool>();
    internal static Dictionary<ProgressType, bool> DefaultDict => Enum.GetValues(typeof(ProgressType)).Cast<ProgressType>().ToDictionary(t => t, _ => false);

    public static void LoadConfig()
    {
        var PATH = Path.Combine(TShock.SavePath, "DataSync.json");
        try
        {
            if (!File.Exists(PATH))
            {
                FileTools.CreateIfNot(PATH, JsonConvert.SerializeObject(DefaultDict, Formatting.Indented));
            }
            ShouldSyncProgress = JsonConvert.DeserializeObject<Dictionary<ProgressType, bool>>(File.ReadAllText(PATH))!;
            File.WriteAllText(PATH, JsonConvert.SerializeObject(ShouldSyncProgress, Formatting.Indented));
        }
        catch (Exception ex)
        {
            TShock.Log.Error(ex.ToString());
            TSPlayer.Server.SendErrorMessage(GetString("[DataSync]配置文件读取错误！！！"));
        }
    }

    public class ProgressConverter : JsonConverter<ProgressType>
    {
        public override ProgressType ReadJson(JsonReader reader, Type objectType, ProgressType existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = $"{reader.Value}";
            return GetProgressType(value) is ProgressType type ? type : throw new JsonSerializationException(GetString($"无法识别的进度类型：{value}"));
        }

        public override void WriteJson(JsonWriter writer, ProgressType value, JsonSerializer serializer)
        {
            writer.WriteValue(GetProgressName(value));
        }
    }
}