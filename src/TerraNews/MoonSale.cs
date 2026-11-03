namespace TerraNews;

using Terraria.ID;

// 出现在日报里的店铺，顺序照城镇 NPC 的解锁表排，渔夫由任务鱼另行处理，
// 骷髅商人排在最后——它是 1.4.4 才加的新 NPC，不在那张表里。
internal enum Shop
{
    Merchant,      // 商人
    DyeTrader,     // 染料商
    Zoologist,     // 动物学家
    Dryad,         // 树妖
    Painter,       // 油漆工
    Clothier,      // 服装商
    Mechanic,      // 机械师
    Steampunker,   // 蒸汽朋克人
    Cyborg,        // 机器侠
    Princess,      // 公主
    Skeleton       // 骷髅商人
}

// 一件货：物品 ID，外加一个跟在图标后面的限定说明，写成「（非墓地）」这样的样子。
internal readonly record struct Lot(int Item, string? Note = null);

// 硬编码的「随月相 / 天气轮换的货架」对照表。
//
// 数据是从 Chest.SetupShop 里逐条挑出来的：只抄判断式里带 Main.moonPhase、
// Main.eclipse、Main.bloodMoon、Main.IsItAHappyWindyDay 的分支，昼夜、血月强度、
// Boss 进度和「看你背包」这类条件一律不抄。所以这里跑一次就知道当天该播什么，
// 不必去调原版的 SetupShop。
//
// 这张表对应 SourceGameVersion 标的游戏版本，换版本后请重新核一遍。
internal static class MoonSale
{
    // 表的来源版本。启动时和 Main.netVersion 比一下，对不上就打一行警告，但不拦播报。
    public const string SourceGameVersion = "Terraria 1.4.5.8 (Protocol 326) / TShock 6.2.1.0 / OTAPI 3.3.14";
    public const string SourceVersionNumber = "1.4.5.8";

    // 世界种子在物品后面的标注，用原版的字段短码而不是种子名字。
    public const string RemixNote = "（remix）";   // Main.remixWorld，即 Don't Dig Up
    public const string TenthNote = "（tenth）";   // Main.tenthAnniversaryWorld
    public const string NoGraveyardNote = "（非墓地）";

    private static readonly string[] ShopNames =
    {
        "商人", "染料商", "动物学家", "树妖", "油漆工",
        "服装商", "机械师", "蒸汽朋克人", "机器侠", "公主", "骷髅商人"
    };

    // 下弦月的火花魔棒会被重混世界的魔法飞刀顶掉，这里记下替换关系。
    private const int SparkingWand = 3069;
    private const int MagicDagger = 517;

    // 今日月相对应的轮换货。键是 Main.moonPhase，0 满月 / 1 亏凸 / 2 下弦 / 3 残月 /
    // 4 新月 / 5 娥眉 / 6 上弦 / 7 盈凸。数组内部已按店铺顺序排好，直接拼即可。
    private static readonly (Shop Shop, Lot[] Lots)[][] Daily =
    {
        // 0 满月
        new[]
        {
            (Shop.DyeTrader, new Lot[] { new(ItemID.ShadowDye), new(ItemID.NegativeDye) }),
            (Shop.Zoologist, new Lot[] { new(ItemID.DogEars), new(ItemID.DogTail) }),
            (Shop.Painter, new Lot[] { new(ItemID.FirstEncounter, NoGraveyardNote) }),
            (Shop.Clothier, new Lot[] { new(ItemID.PlumbersShirt), new(ItemID.PlumbersPants) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.WoodenBoomerang), new(ItemID.StrangeBrew),
                new(ItemID.SpelunkerGlowstick), new(ItemID.BoneArrow), new(ItemID.BlueCounterweight) })
        },
        // 1 亏凸月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.DogEars), new(ItemID.DogTail) }),
            (Shop.Painter, new Lot[] { new(ItemID.FirstEncounter, NoGraveyardNote) }),
            (Shop.Mechanic, new Lot[] { new(ItemID.MechanicsRod) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.Umbrella), new(ItemID.LesserHealingPotion),
                new(ItemID.BoneArrow), new(ItemID.RedCounterweight), new(ItemID.RollerSkatesGreenMountItem) })
        },
        // 2 下弦月 —— 火花魔棒 / 魔法飞刀按种子二选一
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.FoxEars), new(ItemID.FoxTail) }),
            (Shop.Mechanic, new Lot[] { new(ItemID.MechanicsRod) }),
            (Shop.Skeleton, new Lot[] { new(SparkingWand), new(ItemID.StrangeBrew),
                new(ItemID.PurpleCounterweight), new(ItemID.RollerSkatesGreenMountItem) })
        },
        // 3 残月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.FoxEars), new(ItemID.FoxTail) }),
            (Shop.Mechanic, new Lot[] { new(ItemID.MechanicsRod) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.PortableStool), new(ItemID.LesserHealingPotion),
                new(ItemID.GreenCounterweight), new(ItemID.RollerSkatesClassicMountItem), new(ItemID.ArtisanLoaf) })
        },
        // 4 新月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.LizardEars), new(ItemID.LizardTail) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.Aglet), new(ItemID.StrangeBrew), new(ItemID.BoneArrow),
                new(ItemID.BlueCounterweight), new(ItemID.ArtisanLoaf) })
        },
        // 5 娥眉月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.LizardEars), new(ItemID.LizardTail) }),
            (Shop.Mechanic, new Lot[] { new(ItemID.MechanicsRod) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.ClimbingClaws), new(ItemID.LesserHealingPotion),
                new(ItemID.BoneArrow), new(ItemID.RedCounterweight),
                new(ItemID.RollerSkatesClassicMountItem), new(ItemID.ArtisanLoaf) })
        },
        // 6 上弦月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.BunnyEars), new(ItemID.BunnyTail) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.CordageGuide), new(ItemID.StrangeBrew),
                new(ItemID.PurpleCounterweight), new(ItemID.RollerSkatesPartyMountItem) })
        },
        // 7 盈凸月
        new[]
        {
            (Shop.Zoologist, new Lot[] { new(ItemID.BunnyEars), new(ItemID.BunnyTail) }),
            (Shop.Mechanic, new Lot[] { new(ItemID.MechanicsRod) }),
            (Shop.Skeleton, new Lot[] { new(ItemID.Radar), new(ItemID.LesserHealingPotion),
                new(ItemID.GreenCounterweight), new(ItemID.RollerSkatesPartyMountItem) })
        }
    };

    // 公主的四张卡同样按月相两两轮换，但要十周年世界才上架，货后面标种子码。
    private static readonly (int[] Phases, int Item)[] PrincessCards =
    {
        (new[] { 0, 1 }, ItemID.PirateStaff),
        (new[] { 2, 3 }, ItemID.DiscountCard),
        (new[] { 4, 5 }, ItemID.LuckyCoin),
        (new[] { 6, 7 }, ItemID.CoinGun)
    };

    // 大风天（Main.IsItAHappyWindyDay 且在白天）只有商人多卖一把风车。
    private static readonly (Shop Shop, Lot[] Lots)[] Windy =
    {
        (Shop.Merchant, new Lot[] { new(ItemID.PinWheel) })
    };

    // 日食。机器侠三件、蒸汽朋克人一件。
    private static readonly (Shop Shop, Lot[] Lots)[] Eclipse =
    {
        (Shop.Steampunker, new Lot[] { new(ItemID.RedSolution) }),
        (Shop.Cyborg, new Lot[] { new(ItemID.RocketIII), new(774), new(ItemID.ClusterRocketII) })
    };

    // 血月。七家八件，其中树妖那件跟腐化/猩红世界有关，见 BuildBloodMoon。
    private static readonly (Shop Shop, Lot[] Lots)[] BloodMoon =
    {
        (Shop.Merchant, new Lot[] { new(ItemID.ThrowingKnife) }),
        (Shop.DyeTrader, new Lot[] { new(ItemID.BloodbathDye) }),
        (Shop.Painter, new Lot[] { new(ItemID.EvilPresence) }),
        (Shop.Clothier, new Lot[] { new(ItemID.MimeMask) }),
        (Shop.Steampunker, new Lot[] { new(ItemID.RedSolution) }),
        (Shop.Cyborg, new Lot[] { new(ItemID.RocketII), new(ItemID.ClusterRocketII) })
    };

    // 这张表是纯函数，不碰任何 Terraria 静态字段：月相与三个世界标志都由调用方传进来。
    // 这样既能在这里单测，也不会因为 Main 的静态初始化在测试环境里炸掉。
    public static (Shop Shop, Lot[] Lots)[] Today(int moonPhase, bool remix, bool tenthAnniversary)
    {
        int phase = ((moonPhase % 8) + 8) % 8;
        var rows = new List<(Shop Shop, Lot[] Lots)>(Daily[phase]);

        if (remix)
            SwapWandForDagger(rows);

        if (tenthAnniversary)
        {
            foreach ((int[] phases, int item) in PrincessCards)
                if (Array.IndexOf(phases, phase) >= 0)
                    rows.Add((Shop.Princess, new Lot[] { new(item, TenthNote) }));
        }

        return rows.ToArray();
    }

    // 重混世界把下弦月的火花魔棒换成魔法飞刀，其余不动。
    // 必须先复制一份再改：Daily 里的数组是静态共享的，直接改会把普通世界的表也污染掉。
    private static void SwapWandForDagger(List<(Shop Shop, Lot[] Lots)> rows)
    {
        for (int r = 0; r < rows.Count; r++)
        {
            if (rows[r].Shop != Shop.Skeleton)
                continue;

            var source = rows[r].Lots;
            var lots = new Lot[source.Length];
            for (int i = 0; i < source.Length; i++)
                lots[i] = source[i].Item == SparkingWand
                    ? source[i] with { Item = MagicDagger, Note = RemixNote }
                    : source[i];

            rows[r] = (rows[r].Shop, lots);
        }
    }

    public static (Shop Shop, Lot[] Lots)[] WindyDay() => Windy;

    public static (Shop Shop, Lot[] Lots)[] EclipseSale() => Eclipse;

    // 血月时树妖卖的是「毒粉」，但原版分猩红世界与腐化世界给两个不同物品：
    // 猩红世界给 2886，腐化世界给 67。两者同名，这里按世界选，不在播报里标世界。
    public static (Shop Shop, Lot[] Lots)[] BloodMoonSale(bool crimson)
    {
        var rows = new List<(Shop Shop, Lot[] Lots)>(BloodMoon);
        rows.Insert(2, (Shop.Dryad, new Lot[]
        {
            new(crimson ? ItemID.ViciousPowder : ItemID.VilePowder)
        }));
        return rows.ToArray();
    }

    public static string ShopName(Shop shop) => ShopNames[(int)shop];

    // 拼成「骷髅商人：[i:284][i:3001]；服装商：[i:245][i:246]」，图标之间不留空格。
    public static string Render(IEnumerable<(Shop Shop, Lot[] Lots)> rows)
    {
        var parts = new List<string>();
        foreach ((Shop shop, Lot[] lots) in rows)
        {
            if (lots.Length == 0)
                continue;

            var sb = new System.Text.StringBuilder(ShopName(shop)).Append('：');
            foreach (Lot lot in lots)
            {
                sb.Append("[i:").Append(lot.Item).Append(']');
                if (!string.IsNullOrEmpty(lot.Note))
                    sb.Append(lot.Note);
            }

            parts.Add(sb.ToString());
        }

        return string.Join("；", parts);
    }

    // 版本对不上时返回一句话，正常返回 null。
    public static string? VersionWarning(string currentVersion)
    {
        if (currentVersion.StartsWith("v" + SourceVersionNumber, StringComparison.Ordinal))
            return null;

        return $"[TerraNews] 当前游戏版本 {currentVersion}，货架对照表按 {SourceGameVersion} 抄的。"
            + "版本不符时播报可能与实际货架对不上。";
    }
}
