using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace GroundCraft;

public sealed partial class GroundCraft
{
    private void ReloadFiles(bool logToConsole)
    {
        lock (_stateLock)
        {
            Directory.CreateDirectory(DataDirectory);
            EnsureDefaultFiles();

            _config = LoadJson(ConfigPath, GroundCraftConfig.Default());
            _config.Sanitize();

            RecipeFile file = LoadJson(RecipesPath, RecipeFile.Default());
            BuildRecipeBook(file);

            if (logToConsole)
            {
                string summary = GetString($"读取={_audit.Seen}，禁用={_audit.Disabled}，格式通过={_audit.ImportAccepted}，安全通过={_audit.SafetyAccepted}，启用={_audit.ActiveRecipes}");
                TShock.Log.ConsoleInfo(GetString($"[GroundCraft] 配方审核完成：{summary}。配置目录：{DataDirectory}"));
                if (_audit.ImportRejects.Count > 0 || _audit.SafetyRejects.Count > 0)
                    TShock.Log.ConsoleWarn(GetString($"[GroundCraft] 拒绝原因：导入={FormatReasons(_audit.ImportRejects)}；安全={FormatReasons(_audit.SafetyRejects)}"));
            }
        }
    }

    private static T LoadJson<T>(string path, T fallback)
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback;
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(GetString($"[GroundCraft] 读取 JSON 失败：{path}，将使用默认值。错误：{ex.Message}"));
            return fallback;
        }
    }

    private static void EnsureDefaultFiles()
    {
        if (!File.Exists(ConfigPath))
            SaveJson(ConfigPath, GroundCraftConfig.Default());

        if (!File.Exists(RecipesPath))
            SaveJson(RecipesPath, RecipeFile.Default());
    }

    private static void SaveJson<T>(string path, T value)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOptions));
    }

    private void BuildRecipeBook(RecipeFile file)
    {
        RecipeAudit audit = new();
        List<RecipeCandidate> safeCandidates = new();
        List<RecipeSpec> specs = file.Recipes ?? new List<RecipeSpec>();

        foreach (RecipeSpec spec in specs)
        {
            audit.Seen++;
            if (!spec.Enabled)
            {
                audit.Disabled++;
                continue;
            }

            if (!TryImportRecipe(spec, audit, out RecipeCandidate? candidate))
                continue;

            audit.ImportAccepted++;

            if (!PassSafetyAudit(candidate!, audit))
                continue;

            safeCandidates.Add(candidate!);
        }

        List<DropRecipe> active = new();
        foreach (IGrouping<string, RecipeCandidate> group in safeCandidates.GroupBy(c => c.Signature))
        {
            RecipeCandidate[] variants = group.ToArray();
            string[] outputs = variants
                .Select(v => $"{v.OutputType}:{v.OutputStack}")
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            if (outputs.Length > 1)
            {
                audit.RejectSafety("ambiguous_same_inputs", variants.Length);
                continue;
            }

            RecipeCandidate selected = variants[0];
            active.Add(new DropRecipe(
                selected.Id,
                selected.OutputType,
                selected.OutputStack,
                selected.OutputMaxStack,
                selected.RequiredTiles,
                selected.Ingredients,
                selected.Conditions,
                selected.Ingredients.Count,
                selected.Ingredients.Values.Sum(),
                selected.ConditionScore,
                selected.Signature));
        }

        _recipes = active
            .OrderByDescending(r => r.ConditionScore)
            .ThenByDescending(r => r.DistinctIngredientTypes)
            .ThenByDescending(r => r.TotalIngredientStack)
            .ThenByDescending(r => r.RequiredTiles.Length)
            .ThenBy(r => r.OutputType)
            .ToList();

        audit.ActiveRecipes = _recipes.Count;
        _audit = audit;
    }

    private bool TryImportRecipe(RecipeSpec spec, RecipeAudit audit, out RecipeCandidate? candidate)
    {
        candidate = null;
        string id = string.IsNullOrWhiteSpace(spec.Id) ? $"recipe_{audit.Seen}" : spec.Id.Trim();

        if (spec.Output is null || spec.Output.Id <= 0 || spec.Output.Stack <= 0 || !TryGetItemMaxStack(spec.Output.Id, out int outputMaxStack))
        {
            audit.RejectImport("invalid_output");
            return false;
        }

        if (spec.Ingredients is null || spec.Ingredients.Count == 0)
        {
            audit.RejectImport("empty_ingredients");
            return false;
        }

        Dictionary<int, int> ingredients = new();
        foreach (ItemStackSpec ingredient in spec.Ingredients)
        {
            if (ingredient.Id <= 0 || ingredient.Stack <= 0 || !TryGetItemMaxStack(ingredient.Id, out _))
            {
                audit.RejectImport("invalid_ingredient");
                return false;
            }

            AddCount(ingredients, ingredient.Id, ingredient.Stack);
        }

        int[] requiredTiles = (spec.RequiredTiles ?? new List<int>())
            .Where(tile => tile >= 0)
            .Distinct()
            .OrderBy(tile => tile)
            .ToArray();

        if (!TryBuildConditions(spec.Conditions ?? new ConditionsSpec(), audit, out ConditionSet conditions))
            return false;

        candidate = new RecipeCandidate(
            id,
            spec.Output.Id,
            spec.Output.Stack,
            outputMaxStack,
            requiredTiles,
            ingredients,
            conditions,
            BuildSignature(requiredTiles, ingredients, conditions),
            conditions.Score);

        return true;
    }

    private bool PassSafetyAudit(RecipeCandidate candidate, RecipeAudit audit)
    {
        if (!_config.AllowSingleIngredientTypeRecipes && candidate.Ingredients.Count < 2)
        {
            audit.RejectSafety("needs_two_material_types");
            return false;
        }

        if (!_config.AllowCoinRecipes && (candidate.Ingredients.Keys.Any(IsCoinItem) || IsCoinItem(candidate.OutputType)))
        {
            audit.RejectSafety("coins");
            return false;
        }

        if (!_config.AllowInputOutputSameItem && candidate.Ingredients.ContainsKey(candidate.OutputType))
        {
            audit.RejectSafety("input_is_output");
            return false;
        }

        if (candidate.OutputStack > candidate.OutputMaxStack)
        {
            audit.RejectSafety("output_over_max_stack");
            return false;
        }

        audit.SafetyAccepted++;
        return true;
    }

    private static bool TryBuildConditions(ConditionsSpec spec, RecipeAudit audit, out ConditionSet conditions)
    {
        conditions = ConditionSet.Empty;

        if (!TryNormalizeList(spec.Layers, NormalizeLayer, audit, "unknown_layer", out string[] layers))
            return false;
        if (!TryNormalizeList(spec.Biomes, NormalizeBiome, audit, "unknown_biome", out string[] biomes))
            return false;
        if (!TryNormalizeList(spec.Liquids, NormalizeLiquid, audit, "unknown_liquid", out string[] liquids))
            return false;
        if (!TryNormalizeList(spec.AnyLiquids, NormalizeLiquid, audit, "unknown_liquid", out string[] anyLiquids))
            return false;
        if (!TryNormalizeBossProgress(spec.BossProgress ?? new BossProgressSpec(), audit, out BossProgressSet bossProgress))
            return false;

        conditions = new ConditionSet(layers, biomes, liquids, anyLiquids, bossProgress);
        return true;
    }

    private static bool TryNormalizeList(
        List<string>? raw,
        Func<string, string?> normalizer,
        RecipeAudit audit,
        string reason,
        out string[] normalized)
    {
        normalized = Array.Empty<string>();
        if (raw is null || raw.Count == 0)
            return true;

        List<string> values = new();
        foreach (string value in raw)
        {
            if (IsAnyToken(value))
                continue;

            string? normalizedValue = normalizer(value);
            if (normalizedValue is null)
            {
                audit.RejectImport(reason);
                return false;
            }

            values.Add(normalizedValue);
        }

        normalized = values
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return true;
    }

    private static bool TryNormalizeBossProgress(BossProgressSpec spec, RecipeAudit audit, out BossProgressSet progress)
    {
        progress = BossProgressSet.Empty;

        if (!TryNormalizeBossList(spec.AllDowned, audit, out string[] allDowned))
            return false;
        if (!TryNormalizeBossList(spec.AnyDowned, audit, out string[] anyDowned))
            return false;
        if (!TryNormalizeBossList(spec.NoneDowned, audit, out string[] noneDowned))
            return false;

        progress = new BossProgressSet(spec.Hardmode, allDowned, anyDowned, noneDowned);
        return true;
    }

    private static bool TryNormalizeBossList(List<string>? raw, RecipeAudit audit, out string[] normalized)
    {
        normalized = Array.Empty<string>();
        if (raw is null || raw.Count == 0)
            return true;

        List<string> values = new();
        foreach (string value in raw)
        {
            if (IsAnyToken(value))
                continue;

            string? key = NormalizeBoss(value);
            if (key is null || !BossProgressChecks.ContainsKey(key))
            {
                audit.RejectImport("unknown_boss_progress");
                return false;
            }

            values.Add(key);
        }

        normalized = values
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return true;
    }


    private static bool IsAnyToken(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return true;

        string token = value.Trim();
        return token is "*" or "Any" or "any" or "任意" or "不限" or "无";
    }

    private static string? NormalizeLayer(string value)
    {
        return NormalizeToken(value) switch
        {
            "sky" or "space" or "空中" or "天空" => "Sky",
            "surface" or "overworld" or "地表" => "Surface",
            "underground" or "dirtlayer" or "地下" => "Underground",
            "cavern" or "rocklayer" or "洞穴" => "Cavern",
            "underworld" or "hell" or "地狱" => "Underworld",
            _ => null
        };
    }

    private static string? NormalizeBiome(string value)
    {
        return NormalizeToken(value) switch
        {
            "forest" or "森林" => "Forest",
            "snow" or "ice" or "雪原" or "冰雪" => "Snow",
            "desert" or "沙漠" => "Desert",
            "jungle" or "丛林" => "Jungle",
            "corruption" or "corrupt" or "腐化" or "腐化之地" => "Corruption",
            "crimson" or "猩红" or "猩红之地" => "Crimson",
            "hallow" or "hallowed" or "神圣" or "神圣之地" => "Hallow",
            "mushroom" or "glowingmushroom" or "glowshroom" or "蘑菇" or "发光蘑菇" => "Mushroom",
            "graveyard" or "墓地" => "Graveyard",
            "ocean" or "beach" or "海洋" or "海边" => "Ocean",
            "shimmer" or "微光" => "Shimmer",
            "dungeon" or "地牢" => "Dungeon",
            "hive" or "蜂巢" => "Hive",
            "temple" or "lihzahrdtemple" or "神庙" => "Temple",
            _ => null
        };
    }

    private static string? NormalizeLiquid(string value)
    {
        return NormalizeToken(value) switch
        {
            "water" or "水" => "Water",
            "lava" or "岩浆" => "Lava",
            "honey" or "蜂蜜" => "Honey",
            "shimmer" or "微光" => "Shimmer",
            _ => null
        };
    }

    private static string? NormalizeBoss(string value)
    {
        string token = NormalizeToken(value);
        return BossAliases.TryGetValue(token, out string? key) ? key : null;
    }

    private static string NormalizeToken(string value)
    {
        return value.Trim().Replace(" ", "", StringComparison.Ordinal).Replace("_", "", StringComparison.Ordinal).Replace("-", "", StringComparison.Ordinal).ToLowerInvariant();
    }

    private static readonly Dictionary<string, string> BossAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kingslime"] = "KingSlime",
        ["史莱姆王"] = "KingSlime",
        ["eyeofcthulhu"] = "EyeOfCthulhu",
        ["eoc"] = "EyeOfCthulhu",
        ["克苏鲁之眼"] = "EyeOfCthulhu",
        ["克眼"] = "EyeOfCthulhu",
        ["eaterofworldsorbrain"] = "EaterOfWorldsOrBrain",
        ["evilboss"] = "EaterOfWorldsOrBrain",
        ["世界吞噬怪或克苏鲁之脑"] = "EaterOfWorldsOrBrain",
        ["邪恶boss"] = "EaterOfWorldsOrBrain",
        ["eaterofworlds"] = "EaterOfWorldsOrBrain",
        ["brainofcthulhu"] = "EaterOfWorldsOrBrain",
        ["世界吞噬怪"] = "EaterOfWorldsOrBrain",
        ["克苏鲁之脑"] = "EaterOfWorldsOrBrain",
        ["queenbee"] = "QueenBee",
        ["蜂王"] = "QueenBee",
        ["deerclops"] = "Deerclops",
        ["独眼巨鹿"] = "Deerclops",
        ["skeletron"] = "Skeletron",
        ["骷髅王"] = "Skeletron",
        ["wallofflesh"] = "WallOfFlesh",
        ["wof"] = "WallOfFlesh",
        ["肉山"] = "WallOfFlesh",
        ["血肉墙"] = "WallOfFlesh",
        ["hardmode"] = "WallOfFlesh",
        ["困难模式"] = "WallOfFlesh",
        ["queenslime"] = "QueenSlime",
        ["史莱姆皇后"] = "QueenSlime",
        ["mechbossany"] = "MechBossAny",
        ["任意机械boss"] = "MechBossAny",
        ["mechanicalbossany"] = "MechBossAny",
        ["destroyer"] = "Destroyer",
        ["thedestroyer"] = "Destroyer",
        ["毁灭者"] = "Destroyer",
        ["twins"] = "Twins",
        ["thetwins"] = "Twins",
        ["双子魔眼"] = "Twins",
        ["skeletronprime"] = "SkeletronPrime",
        ["机械骷髅王"] = "SkeletronPrime",
        ["mechbossall"] = "MechBossAll",
        ["allmechbosses"] = "MechBossAll",
        ["全部机械boss"] = "MechBossAll",
        ["plantera"] = "Plantera",
        ["世纪之花"] = "Plantera",
        ["golem"] = "Golem",
        ["石巨人"] = "Golem",
        ["dukefishron"] = "DukeFishron",
        ["猪鲨"] = "DukeFishron",
        ["empressoflight"] = "EmpressOfLight",
        ["光之女皇"] = "EmpressOfLight",
        ["lunaticcultist"] = "LunaticCultist",
        ["cultist"] = "LunaticCultist",
        ["拜月教徒"] = "LunaticCultist",
        ["moonlord"] = "MoonLord",
        ["月亮领主"] = "MoonLord",
        ["月总"] = "MoonLord"
    };

    private static readonly Dictionary<string, Func<bool>> BossProgressChecks = new(StringComparer.OrdinalIgnoreCase)
    {
        ["KingSlime"] = () => NPC.downedSlimeKing,
        ["EyeOfCthulhu"] = () => NPC.downedBoss1,
        ["EaterOfWorldsOrBrain"] = () => NPC.downedBoss2,
        ["QueenBee"] = () => NPC.downedQueenBee,
        ["Deerclops"] = () => NPC.downedDeerclops,
        ["Skeletron"] = () => NPC.downedBoss3,
        ["WallOfFlesh"] = () => Main.hardMode,
        ["QueenSlime"] = () => NPC.downedQueenSlime,
        ["MechBossAny"] = () => NPC.downedMechBossAny,
        ["Destroyer"] = () => NPC.downedMechBoss1,
        ["Twins"] = () => NPC.downedMechBoss2,
        ["SkeletronPrime"] = () => NPC.downedMechBoss3,
        ["MechBossAll"] = () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3,
        ["Plantera"] = () => NPC.downedPlantBoss,
        ["Golem"] = () => NPC.downedGolemBoss,
        ["DukeFishron"] = () => NPC.downedFishron,
        ["EmpressOfLight"] = () => NPC.downedEmpressOfLight,
        ["LunaticCultist"] = () => NPC.downedAncientCultist,
        ["MoonLord"] = () => NPC.downedMoonlord
    };

    private static bool TryGetItemMaxStack(int itemType, out int maxStack)
    {
        maxStack = 0;
        try
        {
            Item item = new();
            item.SetDefaults(itemType);
            if (item.type <= 0 || string.IsNullOrWhiteSpace(Lang.GetItemNameValue(itemType)))
                return false;

            maxStack = Math.Max(1, item.maxStack);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsCoinItem(int itemType)
    {
        return itemType is ItemID.CopperCoin or ItemID.SilverCoin or ItemID.GoldCoin or ItemID.PlatinumCoin;
    }

    private static string BuildSignature(IReadOnlyCollection<int> requiredTiles, IReadOnlyDictionary<int, int> ingredients, ConditionSet conditions)
    {
        string tiles = requiredTiles.Count == 0 ? "*" : string.Join(",", requiredTiles.OrderBy(v => v));
        string items = string.Join(",", ingredients.OrderBy(p => p.Key).Select(p => $"{p.Key}x{p.Value}"));
        return $"{tiles}|{items}|{conditions.Signature}";
    }
}
