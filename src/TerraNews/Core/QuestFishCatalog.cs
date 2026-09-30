using System;
using System.Collections.Generic;

namespace TerraNews.Core;

/// <summary>Depth band used by the vanilla fishing height check (Projectile heightLevel).</summary>
public enum DepthBand
{
    /// <summary>No height restriction (heightLevel 0-4).</summary>
    Any,
    /// <summary>heightLevel 0 - Y &lt; worldSurface / 2 (sky).</summary>
    Sky,
    /// <summary>heightLevel 1 - worldSurface / 2 &lt;= Y &lt; worldSurface (surface).</summary>
    Surface,
    /// <summary>heightLevel 2 - worldSurface &lt;= Y &lt; rockLayer (underground).</summary>
    Underground,
    /// <summary>heightLevel 3 - rockLayer &lt;= Y (cavern / hell entry).</summary>
    Cavern,
    /// <summary>heightLevel 4 - bottom of the world (underworld).</summary>
    Underworld
}

/// <summary>One quest fish and where it has to be fished.</summary>
/// <remarks>
/// The biome + depth pair mirrors the vanilla conditions in
/// <c>GameContentFishDropPopulator</c> (see AddQuestFish calls), which is the code path
/// that actually decides whether the Angler's quest fish can be reeled in.
/// </remarks>
public sealed record QuestFishHint(
    int NetId,
    string NameEn,
    string NameZh,
    string Biome,
    DepthBand Depth,
    string Tip);

public static class QuestFishCatalog
{
    /// <summary>Vanilla Angler quest pool - Main.anglerQuestItemNetIDs.</summary>
    public static readonly int[] QuestPool =
    {
        2450, 2451, 2452, 2453, 2454, 2455, 2456, 2457, 2458, 2459,
        2460, 2461, 2462, 2463, 2464, 2465, 2466, 2467, 2468, 2469,
        2470, 2471, 2472, 2473, 2474, 2475, 2476, 2477, 2478, 2479,
        2480, 2481, 2482, 2483, 2484, 2485, 2486, 2487, 2488, 4393,
        4394
    };

    private static readonly Dictionary<int, QuestFishHint> Hints = new()
    {
        // --- SurfaceDrops ---
        [2450] = new(2450, "Batfish", "蝙蝠鱼", "海洋", DepthBand.Underground, "海面以下的水域，别贴着岸边浅水抛竿"),
        [2479] = new(2479, "Bunnyfish", "兔鱼", "任意水域", DepthBand.Surface, "地表附近的水，注意水量要够（≥300 格）"),
        [2456] = new(2456, "Dynamite Fish", "炸药鱼", "任意水域", DepthBand.Surface, "地表附近的水，丛林附近更容易出"),
        [2474] = new(2474, "Zombie Fish", "僵尸鱼", "任意水域", DepthBand.Surface, "地表附近的水，腐化/猩红/地下水源常见"),
        [2455] = new(2455, "Dirt Fish", "泥鱼", "任意水域", DepthBand.Underground, "地表到岩石层之间的地下水"),
        [2478] = new(2478, "Bonefish", "骨鱼", "任意水域", DepthBand.Underground, "地下的洞穴层（Cavern），水多的地方"),
        [2464] = new(2464, "Jewelfish", "宝石鱼", "海洋", DepthBand.Underground, "海洋水面以下的水域"),
        [2469] = new(2469, "Spiderfish", "蜘蛛鱼", "任意水域", DepthBand.Underground, "地下洞穴层，腐化/猩红一带概率更高"),
        [2462] = new(2462, "Hungerfish", "饥饿鱼", "任意水域", DepthBand.Cavern, "地狱 / 世界最底端"),
        [2482] = new(2482, "Demonic Hellfish", "恶魔地狱鱼", "任意水域", DepthBand.Cavern, "地狱 / 世界最底端"),
        [2472] = new(2472, "Guide to Voodoo Fish", "巫毒向导鱼", "任意水域", DepthBand.Cavern, "地狱 / 世界最底端"),
        [2460] = new(2460, "Fishotron", "吃鱼机", "任意水域", DepthBand.Cavern, "地狱 / 世界最底端"),
        [2487] = new(2487, "Slimefish", "史莱姆鱼", "任意水域", DepthBand.Any, "没有地点要求，哪儿都能钓上"),

        // --- FloatingIslandDrops ---
        [2461] = new(2461, "Harpyfish", "鹰身鱼", "浮空岛", DepthBand.Surface, "浮空岛上的水池"),
        [2453] = new(2453, "Cloudfish", "云鱼", "天空", DepthBand.Sky, "云层上方的天空"),
        [2473] = new(2473, "Wyvern Tail", "双足飞龙尾", "天空", DepthBand.Sky, "云层上方的天空"),
        [2476] = new(2476, "Angelfish", "天使鱼", "天空 / 地表", DepthBand.Sky, "天空或地表的天然水池"),
        [2458] = new(2458, "Fallen Starfish", "堕落之星海星", "浮空岛", DepthBand.Surface, "浮空岛上的水池"),
        [2459] = new(2459, "The Fish of Cthulhu", "克苏鲁之鱼", "浮空岛", DepthBand.Surface, "浮空岛上的水池"),

        // --- DesertDrops ---
        [4393] = new(4393, "Scarab Fish", "圣甲虫鱼", "沙漠", DepthBand.Any, "地下的沙漠（沙丘下的水潭）"),
        [4394] = new(4394, "Scorpio Fish", "蝎子鱼", "沙漠", DepthBand.Any, "地下的沙漠（沙丘下的水潭）"),

        // --- OceanDrops ---
        [2480] = new(2480, "Cap'n Tunabeard", "松饼船长", "海洋", DepthBand.Surface, "世界最左/最右边缘的海洋浅水区"),
        [2481] = new(2481, "Clownfish", "小丑鱼", "海洋", DepthBand.Surface, "世界最左/最右边缘的海洋浅水区"),

        // --- JungleDrops ---
        [2452] = new(2452, "Catfish", "鲶鱼", "丛林", DepthBand.Surface, "丛林地表附近的水"),
        [2483] = new(2483, "Derpfish", "呆鱼", "丛林", DepthBand.Surface, "丛林地表附近的水"),
        [2488] = new(2488, "Tropical Barracuda", "热带梭鱼", "丛林", DepthBand.Surface, "丛林地表附近的水"),
        [2486] = new(2486, "Mudfish", "淤泥鱼", "丛林", DepthBand.Underground, "丛林地下任意深度的水"),

        // --- SnowDrops ---
        [2467] = new(2467, "Pengfish", "企鹅鱼", "雪原", DepthBand.Surface, "雪原地表附近的水"),
        [2470] = new(2470, "Tundra Trout", "苔原鳟", "雪原", DepthBand.Surface, "雪原地表附近的水"),
        [2484] = new(2484, "Fishron", "鱼龙", "雪原", DepthBand.Underground, "雪原地下深处的冰水"),
        [2466] = new(2466, "Mutant Flinxfin", "变异雪怪怪鱼", "雪原", DepthBand.Underground, "雪原地下的水"),

        // --- GlowingMushroomsDrops ---
        [2475] = new(2475, "Amanita Fungifin", "毒蘑菇鳍", "发光蘑菇", DepthBand.Any, "地下的发光蘑菇生物群系"),

        // --- HallowedDrops ---
        [2465] = new(2465, "Mirage Fish", "幻海鱼", "神圣之地", DepthBand.Underground, "神圣之地的水域（神圣沙漠沙丘下的水潭）"),
        [2468] = new(2468, "Pixie Fish", "妖精鱼", "神圣之地", DepthBand.Surface, "神圣之地的地表水域"),
        [2471] = new(2471, "Unicorn Fish", "独角鱼", "神圣之地", DepthBand.Any, "神圣之地的水域"),

        // --- CrimsonDrops ---
        [2477] = new(2477, "Bloody Manowar", "血水母", "猩红之地", DepthBand.Any, "猩红之地的水域"),
        [2463] = new(2463, "Ichorfish", "灵液鱼", "猩红之地", DepthBand.Any, "猩红之地的水域"),

        // --- CorruptionDrops ---
        [2454] = new(2454, "Cursed Fish", "诅咒鱼", "腐化之地", DepthBand.Any, "腐化之地的水域"),
        [2485] = new(2485, "Infected Scabbardfish", "感染古剑鱼", "腐化之地", DepthBand.Any, "腐化之地的水域"),
        [2457] = new(2457, "Eater of Plankton", "浮游食者", "腐化之地", DepthBand.Any, "腐化之地的水域"),

        // --- HoneyDrops ---
        [2451] = new(2451, "Bumblebee Tuna", "大黄蜂金枪鱼", "蜂巢", DepthBand.Any, "蜂巢里的蜂蜜池（不是普通水塘）")
    };

    public static bool TryGet(int netId, out QuestFishHint hint) => Hints.TryGetValue(netId, out hint!);

    /// <summary>
    /// Builds the display name for a quest fish.
    ///
    /// The vanilla name is passed in rather than looked up, because reading it needs the
    /// game's localisation tables - which differ between the two hosts. The joining rule
    /// lives here so both hosts spell the result identically.
    /// </summary>
    /// <param name="nameSource">"zh", "en" or "both".</param>
    /// <param name="vanillaName">The name the game itself would show, in its own language.</param>
    public static string NameText(QuestFishHint hint, string nameSource, string vanillaName)
    {
        // The catalog carries the English name it was written from, which is also what the
        // vanilla tables return, so prefer the game's answer when it is a real lookup.
        string en = string.IsNullOrWhiteSpace(vanillaName) ? hint.NameEn : vanillaName;

        if (string.Equals(nameSource, "zh", StringComparison.OrdinalIgnoreCase))
            return hint.NameZh;

        if (string.Equals(nameSource, "en", StringComparison.OrdinalIgnoreCase))
            return en;

        return string.Equals(hint.NameZh, en, StringComparison.Ordinal)
            ? hint.NameZh
            : $"{hint.NameZh}（{en}）";
    }

    public static string BiomeText(QuestFishHint hint) => hint.Biome;

    public static string DepthText(QuestFishHint hint) => hint.Depth switch
    {
        DepthBand.Sky => "天空",
        DepthBand.Surface => "地表",
        DepthBand.Underground => "地下",
        DepthBand.Cavern => "地狱 / 世界底部",
        _ => "任意深度"
    };

    /// <summary>Human readable Y range for a depth band, using the live world values.</summary>
    public static string DepthRangeText(DepthBand band, double worldSurface, double rockLayer, int maxTilesY)
    {
        double skyBottom = worldSurface * 0.5;
        double hellTop = maxTilesY - 300;

        return band switch
        {
            DepthBand.Sky => $"（Y < {F(skyBottom)}）",
            DepthBand.Surface => $"（Y {F(skyBottom)} ~ {F(worldSurface)}）",
            DepthBand.Underground => $"（Y {F(worldSurface)} ~ {F(rockLayer)}）",
            DepthBand.Cavern => $"（Y >= {F(rockLayer)}）",
            _ => string.Empty
        };
    }

    private static string F(double v) => ((int)Math.Round(v)).ToString();
}
