using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;

namespace GroundCraft;

[ApiVersion(2, 1)]
public sealed class GroundCraft : TerrariaPlugin
{
    private const int Anywhere = -1;
    private const string DefaultPlayerPermission = "tshock.canchat";
    private const string DefaultAdminPermission = "groundcraft.admin";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };

    private readonly List<Command> _commands = new();
    private readonly Dictionary<int, int> _stableScans = new();
    private readonly RuntimeStats _runtime = new();

    private GroundCraftConfig _config = GroundCraftConfig.Default();
    private List<DropRecipe> _recipes = new();
    private RecipeAudit _audit = new();
    private int _ticks;
    private bool _hooked;

    public override string Name => "GroundCraft";
    public override string Author => "愚蠢";
    public override string Description => "地上合成：把掉落物丢在一起，根据 JSON 配方和环境/进度条件自动合成。";
    public override Version Version => new(1, 0, 0);

    private static string DataDirectory => Path.Combine(TShock.SavePath, "GroundCraft");
    private static string ConfigPath => Path.Combine(DataDirectory, "config.json");
    private static string RecipesPath => Path.Combine(DataDirectory, "recipes.json");

    public GroundCraft(Main game) : base(game)
    {
        Order = 28;
    }

    public override void Initialize()
    {
        ReloadFiles(logToConsole: true);

        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftInfo, "groundcraft", "gc")
        {
            HelpText = "查看地上合成状态。"
        });
        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftRecipes, "gcrecipes", "gcr")
        {
            HelpText = "查看地上合成配方。用法：/gcrecipes [页码|搜索]"
        });
        Register(new Command(_config.PlayerPermissionOrDefault(), GroundCraftEnvironment, "gcenv")
        {
            HelpText = "查看当前位置可用于地上合成的环境标签。"
        });
        Register(new Command(_config.AdminPermissionOrDefault(), GroundCraftReload, "gcreload")
        {
            HelpText = "热重载 GroundCraft 的 config.json 和 recipes.json。"
        });
        Register(new Command(_config.AdminPermissionOrDefault(), GroundCraftAudit, "gcaudit")
        {
            HelpText = "查看 GroundCraft 配方审核结果。"
        });

        ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        _hooked = true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_hooked)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                _hooked = false;
            }

            foreach (Command command in _commands)
                Commands.ChatCommands.Remove(command);

            _commands.Clear();
            TShock.Log.ConsoleInfo("[GroundCraft] 插件已卸载。");
        }

        base.Dispose(disposing);
    }

    private void OnGameUpdate(EventArgs args)
    {
        try
        {
            if (!_config.Enabled)
                return;

            _ticks++;
            if (_ticks % Math.Max(1, _config.ScanIntervalTicks) != 0)
                return;

            ScanDrops();
        }
        catch (Exception ex)
        {
            _runtime.Errors++;
            if (_runtime.Errors <= 5 || _runtime.Errors % 25 == 0)
                TShock.Log.ConsoleError($"[GroundCraft] 扫描掉落物失败：{ex}");
        }
    }

    private void ScanDrops()
    {
        _runtime.Scans++;

        List<DropRef> drops = CollectStableDrops();
        if (drops.Count == 0)
            return;

        bool[] used = new bool[drops.Count];
        for (int i = 0; i < drops.Count; i++)
        {
            if (used[i])
                continue;

            List<DropRef> cluster = BuildCluster(drops, used, i);
            if (cluster.Count == 0)
                continue;

            _runtime.Clusters++;
            if (!TryCraftCluster(cluster))
                _runtime.NoMatches++;
        }
    }

    private List<DropRef> CollectStableDrops()
    {
        List<DropRef> drops = new();
        int max = Math.Min(Main.maxItems, Main.item.Length);

        for (int i = 0; i < max; i++)
        {
            WorldItem item = Main.item[i];
            if (!IsUsableDrop(item))
            {
                _stableScans.Remove(i);
                continue;
            }

            if (!IsStill(item))
            {
                _stableScans[i] = 0;
                continue;
            }

            int stable = _stableScans.TryGetValue(i, out int previous) ? previous + 1 : 1;
            _stableScans[i] = stable;
            if (stable < Math.Max(1, _config.RequiredStableScans))
                continue;

            Vector2 center = item.position + new Vector2(item.width / 2f, item.height / 2f);
            drops.Add(new DropRef(i, item, center));
        }

        return drops;
    }

    private List<DropRef> BuildCluster(IReadOnlyList<DropRef> drops, bool[] used, int seedIndex)
    {
        List<DropRef> cluster = new();
        Queue<int> queue = new();
        float radiusPixels = Math.Max(0.5f, _config.ClusterRadiusTiles) * 16f;
        float radiusSquared = radiusPixels * radiusPixels;

        used[seedIndex] = true;
        queue.Enqueue(seedIndex);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            DropRef currentDrop = drops[current];
            cluster.Add(currentDrop);

            for (int i = 0; i < drops.Count; i++)
            {
                if (used[i])
                    continue;

                if (Vector2.DistanceSquared(currentDrop.Center, drops[i].Center) > radiusSquared)
                    continue;

                used[i] = true;
                queue.Enqueue(i);
            }
        }

        return cluster;
    }

    private bool TryCraftCluster(List<DropRef> cluster)
    {
        Dictionary<int, int> available = CountItems(cluster);
        Vector2 center = AverageCenter(cluster);
        EnvironmentSnapshot snapshot = ProbeEnvironment(center);

        foreach (DropRecipe recipe in _recipes)
        {
            if (!HasIngredients(available, recipe))
                continue;

            if (!ConditionsMatch(recipe, snapshot))
            {
                _runtime.ConditionMisses++;
                continue;
            }

            if (!HasRequiredStation(recipe, center))
            {
                _runtime.StationMisses++;
                continue;
            }

            int craftCount = GetCraftCount(available, recipe);
            if (craftCount <= 0)
                continue;

            if (!ConsumeIngredients(cluster, recipe, craftCount, out Dictionary<int, int> consumedStacks))
            {
                _runtime.ConsumeFailures++;
                return false;
            }

            int outputStack = recipe.OutputStack * craftCount;
            SpawnItem(recipe.OutputType, outputStack, center);
            _runtime.CraftBatches++;
            _runtime.Crafts += craftCount;

            TShock.Log.ConsoleInfo($"[GroundCraft] {recipe.Id} 在 {center.X / 16f:0}, {center.Y / 16f:0} 合成 {ItemName(recipe.OutputType)} x{outputStack}。");
            NotifyNearby(center, recipe, outputStack, consumedStacks);
            return true;
        }

        return false;
    }

    private static Dictionary<int, int> CountItems(IEnumerable<DropRef> cluster)
    {
        Dictionary<int, int> counts = new();
        foreach (DropRef drop in cluster)
            AddCount(counts, drop.Item.type, drop.Item.stack);

        return counts;
    }

    private static bool HasIngredients(IReadOnlyDictionary<int, int> available, DropRecipe recipe)
    {
        foreach ((int type, int required) in recipe.Ingredients)
        {
            if (!available.TryGetValue(type, out int count) || count < required)
                return false;
        }

        return true;
    }

    private int GetCraftCount(IReadOnlyDictionary<int, int> available, DropRecipe recipe)
    {
        int craftCount = int.MaxValue;
        foreach ((int type, int required) in recipe.Ingredients)
        {
            if (!available.TryGetValue(type, out int count))
                return 0;

            craftCount = Math.Min(craftCount, count / required);
        }

        if (craftCount == int.MaxValue)
            return 0;

        if (recipe.OutputMaxStack > 0 && recipe.OutputStack > 0)
            craftCount = Math.Min(craftCount, Math.Max(1, recipe.OutputMaxStack / recipe.OutputStack));

        return Math.Min(craftCount, Math.Max(1, _config.MaxCraftsPerClusterPerScan));
    }

    private bool ConsumeIngredients(IEnumerable<DropRef> cluster, DropRecipe recipe, int craftCount, out Dictionary<int, int> consumedStacks)
    {
        consumedStacks = new Dictionary<int, int>();
        Dictionary<int, int> remaining = recipe.Ingredients.ToDictionary(p => p.Key, p => p.Value * craftCount);

        foreach (DropRef drop in cluster.OrderBy(d => d.Index))
        {
            WorldItem item = drop.Item;
            if (!item.active || item.type <= 0 || item.stack <= 0)
                continue;

            if (!remaining.TryGetValue(item.type, out int needed) || needed <= 0)
                continue;

            int taken = Math.Min(item.stack, needed);
            remaining[item.type] = needed - taken;
            AddCount(consumedStacks, item.type, taken);

            int itemType = item.type;
            int leftoverStack = item.stack - taken;
            Vector2 respawnCenter = drop.Center;

            item.TurnToAir(true);
            ClearConsumedItem(drop.Index);
            _stableScans.Remove(drop.Index);

            if (leftoverStack > 0)
                SpawnItem(itemType, leftoverStack, respawnCenter);
        }

        return remaining.Values.All(v => v <= 0);
    }

    private static void SpawnItem(int itemType, int stack, Vector2 center)
    {
        int index = Item.NewItem(
            new EntitySource_WorldEvent(),
            (int)center.X - 8,
            (int)center.Y - 8,
            16,
            16,
            itemType,
            stack,
            false,
            0,
            true);

        if (index >= 0)
            SyncItem(index);
    }

    private void NotifyNearby(Vector2 center, DropRecipe recipe, int outputStack, IReadOnlyDictionary<int, int> consumedStacks)
    {
        if (!_config.NotifyPlayers)
            return;

        float radiusPixels = Math.Max(1, _config.NotifyRadiusTiles) * 16f;
        float radiusSquared = radiusPixels * radiusPixels;
        string message = $"地上合成：{ItemName(recipe.OutputType)} x{outputStack}";
        string consumed = FormatConsumedStacks(consumedStacks);

        foreach (TSPlayer? player in TShock.Players)
        {
            if (player is not { Active: true } || player == TSPlayer.Server)
                continue;

            Vector2 playerCenter = PlayerCenter(player.TPlayer);
            if (Vector2.DistanceSquared(center, playerCenter) > radiusSquared)
                continue;

            player.SendSuccessMessage(message);
            if (_config.NotifyConsumedItems)
                player.SendInfoMessage($"已清除被消耗的掉落物：{consumed}。如果你还看到旧材料，那是客户端残影，实际已经不存在。");
        }
    }

    private static void SyncItem(int index)
    {
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
    }

    private void ClearConsumedItem(int index)
    {
        if (!_config.ClearClientGhostItems)
        {
            SyncItem(index);
            return;
        }

        NetMessage.SendData(MessageID.ReleaseItemOwnership, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItemDespawn, -1, -1, null, index);
        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index);
    }

    private bool HasRequiredStation(DropRecipe recipe, Vector2 center)
    {
        if (recipe.RequiredTiles.Length == 0)
            return true;

        int tileX = (int)MathF.Round(center.X / 16f);
        int tileY = (int)MathF.Round(center.Y / 16f);
        int radius = Math.Max(0, _config.StationSearchRadiusTiles);

        for (int x = tileX - radius; x <= tileX + radius; x++)
        {
            for (int y = tileY - radius; y <= tileY + radius; y++)
            {
                if (!WorldGen.InWorld(x, y, 10))
                    continue;

                ITile tile = Framing.GetTileSafely(x, y);
                if (tile.active() && TileMatchesAny(recipe.RequiredTiles, tile.type))
                    return true;
            }
        }

        return false;
    }

    private static bool TileMatchesAny(IReadOnlyCollection<int> requiredTiles, int actualTile)
    {
        foreach (int requiredTile in requiredTiles)
        {
            if (TileMatches(requiredTile, actualTile))
                return true;
        }

        return false;
    }

    private static bool TileMatches(int requiredTile, int actualTile)
    {
        if (requiredTile == actualTile)
            return true;

        List<int>[]? countsAs = Recipe.TileCountsAs;
        if (requiredTile >= 0 && requiredTile < countsAs.Length && countsAs[requiredTile]?.Contains(actualTile) == true)
            return true;

        return actualTile >= 0 && actualTile < countsAs.Length && countsAs[actualTile]?.Contains(requiredTile) == true;
    }

    private bool ConditionsMatch(DropRecipe recipe, EnvironmentSnapshot snapshot)
    {
        ConditionSet conditions = recipe.Conditions;

        if (conditions.Layers.Length > 0 && !conditions.Layers.Contains(snapshot.Layer, StringComparer.OrdinalIgnoreCase))
            return false;

        if (conditions.Biomes.Length > 0 && !conditions.Biomes.Any(b => snapshot.Biomes.Contains(b)))
            return false;

        if (conditions.Liquids.Length > 0 && !conditions.Liquids.All(l => snapshot.Liquids.Contains(l)))
            return false;

        if (conditions.AnyLiquids.Length > 0 && !conditions.AnyLiquids.Any(l => snapshot.Liquids.Contains(l)))
            return false;

        return BossProgressMatches(conditions.BossProgress);
    }

    private EnvironmentSnapshot ProbeEnvironment(Vector2 center)
    {
        int tileX = (int)MathF.Round(center.X / 16f);
        int tileY = (int)MathF.Round(center.Y / 16f);
        HashSet<string> liquids = ProbeLiquids(tileX, tileY);
        TSPlayer? nearest = FindNearestPlayer(center, Math.Max(1, _config.BiomePlayerProbeRadiusTiles) * 16f);
        string layer = ProbeLayer(tileY, nearest?.TPlayer);
        HashSet<string> biomes = ProbeBiomes(tileX, layer, nearest?.TPlayer, liquids);

        return new EnvironmentSnapshot(layer, biomes, liquids);
    }

    private HashSet<string> ProbeLiquids(int tileX, int tileY)
    {
        HashSet<string> liquids = new(StringComparer.OrdinalIgnoreCase);
        int radius = Math.Max(0, _config.EnvironmentSearchRadiusTiles);

        for (int x = tileX - radius; x <= tileX + radius; x++)
        {
            for (int y = tileY - radius; y <= tileY + radius; y++)
            {
                if (!WorldGen.InWorld(x, y, 10))
                    continue;

                ITile tile = Framing.GetTileSafely(x, y);
                if (tile.liquid <= 0)
                    continue;

                int liquidType = tile.liquidType();
                switch (liquidType)
                {
                    case LiquidID.Lava:
                        liquids.Add("Lava");
                        break;
                    case LiquidID.Honey:
                        liquids.Add("Honey");
                        break;
                    case LiquidID.Shimmer:
                        liquids.Add("Shimmer");
                        break;
                    default:
                        liquids.Add("Water");
                        break;
                }
            }
        }

        return liquids;
    }

    private static string ProbeLayer(int tileY, Player? nearbyPlayer)
    {
        if (nearbyPlayer is not null)
        {
            if (nearbyPlayer.ZoneUnderworldHeight)
                return "Underworld";
            if (nearbyPlayer.ZoneSkyHeight)
                return "Sky";
            if (nearbyPlayer.ZoneRockLayerHeight)
                return "Cavern";
            if (nearbyPlayer.ZoneDirtLayerHeight)
                return "Underground";
        }

        if (tileY >= Main.maxTilesY - 200)
            return "Underworld";
        if (tileY < Main.worldSurface * 0.35)
            return "Sky";
        if (tileY < Main.worldSurface)
            return "Surface";
        if (tileY < Main.rockLayer)
            return "Underground";

        return "Cavern";
    }

    private static HashSet<string> ProbeBiomes(int tileX, string layer, Player? nearbyPlayer, IReadOnlySet<string> liquids)
    {
        HashSet<string> biomes = new(StringComparer.OrdinalIgnoreCase);

        if (tileX <= 380 || tileX >= Main.maxTilesX - 380)
            biomes.Add("Ocean");

        if (liquids.Contains("Shimmer"))
            biomes.Add("Shimmer");

        if (nearbyPlayer is not null)
        {
            AddIf(biomes, nearbyPlayer.ZoneSnow, "Snow");
            AddIf(biomes, nearbyPlayer.ZoneDesert, "Desert");
            AddIf(biomes, nearbyPlayer.ZoneJungle, "Jungle");
            AddIf(biomes, nearbyPlayer.ZoneCorrupt, "Corruption");
            AddIf(biomes, nearbyPlayer.ZoneCrimson, "Crimson");
            AddIf(biomes, nearbyPlayer.ZoneHallow, "Hallow");
            AddIf(biomes, nearbyPlayer.ZoneGlowshroom, "Mushroom");
            AddIf(biomes, nearbyPlayer.ZoneGraveyard, "Graveyard");
            AddIf(biomes, nearbyPlayer.ZoneBeach, "Ocean");
            AddIf(biomes, nearbyPlayer.ZoneShimmer, "Shimmer");
            AddIf(biomes, nearbyPlayer.ZoneDungeon, "Dungeon");
            AddIf(biomes, nearbyPlayer.ZoneHive, "Hive");
            AddIf(biomes, nearbyPlayer.ZoneLihzhardTemple, "Temple");
        }

        if (layer.Equals("Surface", StringComparison.OrdinalIgnoreCase) && !HasMajorBiome(biomes))
            biomes.Add("Forest");

        return biomes;
    }

    private static bool HasMajorBiome(IReadOnlySet<string> biomes)
    {
        return biomes.Contains("Snow")
            || biomes.Contains("Desert")
            || biomes.Contains("Jungle")
            || biomes.Contains("Corruption")
            || biomes.Contains("Crimson")
            || biomes.Contains("Hallow")
            || biomes.Contains("Mushroom")
            || biomes.Contains("Graveyard")
            || biomes.Contains("Ocean")
            || biomes.Contains("Shimmer")
            || biomes.Contains("Dungeon")
            || biomes.Contains("Hive")
            || biomes.Contains("Temple");
    }

    private static void AddIf(HashSet<string> set, bool condition, string value)
    {
        if (condition)
            set.Add(value);
    }

    private static TSPlayer? FindNearestPlayer(Vector2 center, float maxDistancePixels)
    {
        float bestDistanceSquared = maxDistancePixels * maxDistancePixels;
        TSPlayer? best = null;

        foreach (TSPlayer? player in TShock.Players)
        {
            if (player is not { Active: true } || player == TSPlayer.Server)
                continue;

            float distanceSquared = Vector2.DistanceSquared(center, PlayerCenter(player.TPlayer));
            if (distanceSquared >= bestDistanceSquared)
                continue;

            bestDistanceSquared = distanceSquared;
            best = player;
        }

        return best;
    }

    private static Vector2 PlayerCenter(Player player)
    {
        return player.position + new Vector2(player.width / 2f, player.height / 2f);
    }

    private static bool IsUsableDrop(WorldItem item)
    {
        return item.active && item.type > 0 && item.stack > 0 && item.width > 0 && item.height > 0;
    }

    private bool IsStill(WorldItem item)
    {
        return item.velocity.LengthSquared() <= Math.Max(0.0001f, _config.StillVelocitySquared);
    }

    private void ReloadFiles(bool logToConsole)
    {
        Directory.CreateDirectory(DataDirectory);
        EnsureDefaultFiles();

        _config = LoadJson(ConfigPath, GroundCraftConfig.Default());
        _config.Sanitize();

        RecipeFile file = LoadJson(RecipesPath, RecipeFile.Default());
        BuildRecipeBook(file);

        if (logToConsole)
        {
            string summary = $"读取={_audit.Seen}，禁用={_audit.Disabled}，格式通过={_audit.ImportAccepted}，安全通过={_audit.SafetyAccepted}，启用={_audit.ActiveRecipes}";
            TShock.Log.ConsoleInfo($"[GroundCraft] 配方审核完成：{summary}。配置目录：{DataDirectory}");
            if (_audit.ImportRejects.Count > 0 || _audit.SafetyRejects.Count > 0)
                TShock.Log.ConsoleWarn($"[GroundCraft] 拒绝原因：导入={FormatReasons(_audit.ImportRejects)}；安全={FormatReasons(_audit.SafetyRejects)}");
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
            TShock.Log.ConsoleError($"[GroundCraft] 读取 JSON 失败：{path}，将使用默认值。错误：{ex.Message}");
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

    private static bool BossProgressMatches(BossProgressSet progress)
    {
        if (progress.Hardmode is not null && Main.hardMode != progress.Hardmode.Value)
            return false;

        foreach (string key in progress.AllDowned)
        {
            if (!BossProgressChecks[key]())
                return false;
        }

        if (progress.AnyDowned.Length > 0 && !progress.AnyDowned.Any(key => BossProgressChecks[key]()))
            return false;

        foreach (string key in progress.NoneDowned)
        {
            if (BossProgressChecks[key]())
                return false;
        }

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

    private static void AddCount(IDictionary<int, int> counts, int itemType, int stack)
    {
        counts[itemType] = counts.TryGetValue(itemType, out int current) ? current + stack : stack;
    }

    private static void AddReason(IDictionary<string, int> counts, string reason, int count)
    {
        counts[reason] = counts.TryGetValue(reason, out int current) ? current + count : count;
    }

    private static string ItemName(int itemType)
    {
        string name = Lang.GetItemNameValue(itemType);
        return string.IsNullOrWhiteSpace(name) ? $"物品 {itemType}" : name;
    }

    private static string FormatConsumedStacks(IReadOnlyDictionary<int, int> consumedStacks)
    {
        if (consumedStacks.Count == 0)
            return "无";

        return string.Join(", ", consumedStacks
            .OrderBy(p => ItemName(p.Key), StringComparer.OrdinalIgnoreCase)
            .Select(p => $"{ItemName(p.Key)} x{p.Value}"));
    }

    private static Vector2 AverageCenter(IReadOnlyCollection<DropRef> cluster)
    {
        Vector2 total = Vector2.Zero;
        foreach (DropRef drop in cluster)
            total += drop.Center;

        return total / Math.Max(1, cluster.Count);
    }

    private void GroundCraftInfo(CommandArgs args)
    {
        args.Player.SendInfoMessage($"地上合成：{(_config.Enabled ? "已启用" : "已关闭")}，启用配方 {_recipes.Count} 条。");
        args.Player.SendInfoMessage($"材料丢在 {_config.ClusterRadiusTiles:0.##} 格内，静止 {_config.RequiredStableScans} 次扫描后尝试合成；每堆每次最多合成 {_config.MaxCraftsPerClusterPerScan} 批。");
        args.Player.SendInfoMessage("命令：/gcrecipes [页码|搜索]、/gcenv、/gcaudit、/gcreload。");
        args.Player.SendInfoMessage($"配置：{ConfigPath}；配方：{RecipesPath}。");
    }

    private void GroundCraftRecipes(CommandArgs args)
    {
        int page = 1;
        string query = "";
        if (args.Parameters.Count > 0)
        {
            if (!int.TryParse(args.Parameters[0], out page))
                query = string.Join(" ", args.Parameters);
        }

        List<DropRecipe> matches = string.IsNullOrWhiteSpace(query)
            ? _recipes
            : _recipes.Where(recipe => MatchesRecipeQuery(recipe, query)).ToList();

        const int pageSize = 6;
        int maxPage = Math.Max(1, (int)Math.Ceiling(matches.Count / (double)pageSize));
        page = Math.Clamp(page, 1, maxPage);

        string queryPart = string.IsNullOrWhiteSpace(query) ? "" : $"，搜索“{query}”";
        args.Player.SendInfoMessage($"地上合成配方{queryPart}：第 {page}/{maxPage} 页，共 {matches.Count} 条。");

        foreach (DropRecipe recipe in matches.Skip((page - 1) * pageSize).Take(pageSize))
            args.Player.SendInfoMessage(RecipeLine(recipe));
    }

    private static bool MatchesRecipeQuery(DropRecipe recipe, string query)
    {
        if (recipe.Id.Contains(query, StringComparison.OrdinalIgnoreCase) || ItemName(recipe.OutputType).Contains(query, StringComparison.OrdinalIgnoreCase))
            return true;

        return recipe.Ingredients.Keys.Any(type => ItemName(type).Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    private void GroundCraftEnvironment(CommandArgs args)
    {
        if (args.Player == TSPlayer.Server)
        {
            args.Player.SendErrorMessage("该命令需要玩家在游戏内执行。");
            return;
        }

        EnvironmentSnapshot snapshot = ProbeEnvironment(PlayerCenter(args.Player.TPlayer));
        args.Player.SendInfoMessage($"当前层级：{snapshot.Layer}。");
        args.Player.SendInfoMessage($"当前生物群系：{FormatTags(snapshot.Biomes)}。");
        args.Player.SendInfoMessage($"附近液体：{FormatTags(snapshot.Liquids)}。");
        args.Player.SendInfoMessage($"Boss 进度示例：困难模式={(Main.hardMode ? "是" : "否")}，任意机械Boss={(NPC.downedMechBossAny ? "已击败" : "未击败")}。");
    }

    private void GroundCraftReload(CommandArgs args)
    {
        ReloadFiles(logToConsole: true);
        args.Player.SendSuccessMessage($"GroundCraft 已热重载：启用配方 {_recipes.Count} 条。无需重启服务器。");
    }

    private void GroundCraftAudit(CommandArgs args)
    {
        args.Player.SendInfoMessage($"读取={_audit.Seen}，禁用={_audit.Disabled}，格式通过={_audit.ImportAccepted}，安全通过={_audit.SafetyAccepted}，启用={_audit.ActiveRecipes}。");
        SendReasons(args.Player, "导入拒绝", _audit.ImportRejects);
        SendReasons(args.Player, "安全拒绝", _audit.SafetyRejects);
        args.Player.SendInfoMessage($"运行统计：扫描={_runtime.Scans}，材料堆={_runtime.Clusters}，合成批次={_runtime.CraftBatches}，合成次数={_runtime.Crafts}，未匹配={_runtime.NoMatches}，条件不符={_runtime.ConditionMisses}。");
    }

    private static string RecipeLine(DropRecipe recipe)
    {
        string ingredients = string.Join(" + ", recipe.Ingredients
            .OrderBy(p => ItemName(p.Key), StringComparer.OrdinalIgnoreCase)
            .Select(p => $"{ItemName(p.Key)} x{p.Value}"));

        return $"{recipe.Id}: {ingredients} => {ItemName(recipe.OutputType)} x{recipe.OutputStack} @ {StationName(recipe.RequiredTiles)} {ConditionName(recipe.Conditions)}";
    }

    private static string StationName(IReadOnlyCollection<int> tileTypes)
    {
        if (tileTypes.Count == 0)
            return "任意地点";

        return string.Join("/", tileTypes.Select(tileType => tileType switch
        {
            TileID.WorkBenches => "工作台",
            TileID.Furnaces => "熔炉",
            TileID.Hellforge => "地狱熔炉",
            TileID.Anvils => "铁砧/铅砧",
            TileID.Sawmill => "锯木机",
            TileID.Loom => "织布机",
            TileID.Kegs => "酒桶",
            TileID.Bottles => "瓶子/炼药桌",
            TileID.CookingPots => "烹饪锅",
            TileID.TinkerersWorkbench => "工匠作坊",
            TileID.Extractinator => "提炼机",
            TileID.MythrilAnvil => "秘银砧/山铜砧",
            _ => $"图格 {tileType}"
        }));
    }

    private static string ConditionName(ConditionSet conditions)
    {
        List<string> parts = new();
        if (conditions.Layers.Length > 0)
            parts.Add($"层级={string.Join("/", conditions.Layers)}");
        if (conditions.Biomes.Length > 0)
            parts.Add($"地形={string.Join("/", conditions.Biomes)}");
        if (conditions.Liquids.Length > 0)
            parts.Add($"液体={string.Join("+", conditions.Liquids)}");
        if (conditions.AnyLiquids.Length > 0)
            parts.Add($"任一液体={string.Join("/", conditions.AnyLiquids)}");
        if (!conditions.BossProgress.IsEmpty)
            parts.Add("进度条件");

        return parts.Count == 0 ? "" : $"[{string.Join("，", parts)}]";
    }

    private static string FormatTags(IReadOnlyCollection<string> tags)
    {
        return tags.Count == 0 ? "无" : string.Join(", ", tags.OrderBy(v => v, StringComparer.OrdinalIgnoreCase));
    }

    private static void SendReasons(TSPlayer player, string title, IReadOnlyDictionary<string, int> reasons)
    {
        if (reasons.Count == 0)
        {
            player.SendInfoMessage($"{title}：无。");
            return;
        }

        player.SendInfoMessage($"{title}：{FormatReasons(reasons)}");
    }

    private static string FormatReasons(IReadOnlyDictionary<string, int> reasons)
    {
        if (reasons.Count == 0)
            return "无";

        return string.Join(", ", reasons.OrderByDescending(p => p.Value).ThenBy(p => p.Key).Take(8).Select(p => $"{ReasonName(p.Key)}={p.Value}"));
    }

    private static string ReasonName(string reason)
    {
        return reason switch
        {
            "invalid_output" => "产物非法",
            "empty_ingredients" => "没有材料",
            "invalid_ingredient" => "材料非法",
            "unknown_layer" => "未知层级",
            "unknown_biome" => "未知地形",
            "unknown_liquid" => "未知液体",
            "unknown_boss_progress" => "未知Boss进度",
            "needs_two_material_types" => "材料种类不足",
            "coins" => "涉及钱币",
            "input_is_output" => "材料和产物相同",
            "output_over_max_stack" => "产物超过最大堆叠",
            "ambiguous_same_inputs" => "同材料同条件多结果歧义",
            _ => reason
        };
    }

    private void Register(Command command)
    {
        _commands.Add(command);
        Commands.ChatCommands.Add(command);
    }

    private sealed record DropRef(int Index, WorldItem Item, Vector2 Center);

    private sealed record EnvironmentSnapshot(string Layer, HashSet<string> Biomes, HashSet<string> Liquids);

    private sealed record RecipeCandidate(
        string Id,
        int OutputType,
        int OutputStack,
        int OutputMaxStack,
        int[] RequiredTiles,
        Dictionary<int, int> Ingredients,
        ConditionSet Conditions,
        string Signature,
        int ConditionScore);

    private sealed record DropRecipe(
        string Id,
        int OutputType,
        int OutputStack,
        int OutputMaxStack,
        int[] RequiredTiles,
        IReadOnlyDictionary<int, int> Ingredients,
        ConditionSet Conditions,
        int DistinctIngredientTypes,
        int TotalIngredientStack,
        int ConditionScore,
        string Signature);

    private sealed record ConditionSet(string[] Layers, string[] Biomes, string[] Liquids, string[] AnyLiquids, BossProgressSet BossProgress)
    {
        public static ConditionSet Empty { get; } = new(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), BossProgressSet.Empty);

        public int Score => Layers.Length + Biomes.Length + Liquids.Length + AnyLiquids.Length + BossProgress.Score;

        public string Signature => string.Join("|", new[]
        {
            $"L={string.Join(",", Layers)}",
            $"B={string.Join(",", Biomes)}",
            $"Q={string.Join(",", Liquids)}",
            $"AQ={string.Join(",", AnyLiquids)}",
            BossProgress.Signature
        });
    }

    private sealed record BossProgressSet(bool? Hardmode, string[] AllDowned, string[] AnyDowned, string[] NoneDowned)
    {
        public static BossProgressSet Empty { get; } = new(null, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>());

        public bool IsEmpty => Hardmode is null && AllDowned.Length == 0 && AnyDowned.Length == 0 && NoneDowned.Length == 0;

        public int Score => (Hardmode is null ? 0 : 1) + AllDowned.Length + AnyDowned.Length + NoneDowned.Length;

        public string Signature => $"H={Hardmode?.ToString() ?? "*"};All={string.Join(",", AllDowned)};Any={string.Join(",", AnyDowned)};None={string.Join(",", NoneDowned)}";
    }

    private sealed class RecipeAudit
    {
        public int Seen { get; set; }
        public int Disabled { get; set; }
        public int ImportAccepted { get; set; }
        public int SafetyAccepted { get; set; }
        public int ActiveRecipes { get; set; }
        public Dictionary<string, int> ImportRejects { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, int> SafetyRejects { get; } = new(StringComparer.OrdinalIgnoreCase);

        public void RejectImport(string reason, int count = 1)
        {
            AddReason(ImportRejects, reason, count);
        }

        public void RejectSafety(string reason, int count = 1)
        {
            AddReason(SafetyRejects, reason, count);
        }
    }

    private sealed class RuntimeStats
    {
        public long Scans { get; set; }
        public long Clusters { get; set; }
        public long Crafts { get; set; }
        public long CraftBatches { get; set; }
        public long NoMatches { get; set; }
        public long StationMisses { get; set; }
        public long ConditionMisses { get; set; }
        public long ConsumeFailures { get; set; }
        public long Errors { get; set; }
    }

    public sealed class GroundCraftConfig
    {
        public bool Enabled { get; set; } = true;
        public int ScanIntervalTicks { get; set; } = 45;
        public int RequiredStableScans { get; set; } = 2;
        public float ClusterRadiusTiles { get; set; } = 3f;
        public float StillVelocitySquared { get; set; } = 0.02f;
        public int StationSearchRadiusTiles { get; set; } = 7;
        public int EnvironmentSearchRadiusTiles { get; set; } = 8;
        public int BiomePlayerProbeRadiusTiles { get; set; } = 40;
        public int MaxCraftsPerClusterPerScan { get; set; } = 1;
        public int NotifyRadiusTiles { get; set; } = 24;
        public bool NotifyPlayers { get; set; } = true;
        public bool NotifyConsumedItems { get; set; } = true;
        public bool ClearClientGhostItems { get; set; } = true;
        public bool AllowSingleIngredientTypeRecipes { get; set; }
        public bool AllowCoinRecipes { get; set; }
        public bool AllowInputOutputSameItem { get; set; }
        public string PlayerPermission { get; set; } = DefaultPlayerPermission;
        public string AdminPermission { get; set; } = DefaultAdminPermission;

        public static GroundCraftConfig Default()
        {
            return new GroundCraftConfig();
        }

        public void Sanitize()
        {
            ScanIntervalTicks = Math.Max(1, ScanIntervalTicks);
            RequiredStableScans = Math.Max(1, RequiredStableScans);
            ClusterRadiusTiles = Math.Clamp(ClusterRadiusTiles, 0.5f, 20f);
            StillVelocitySquared = Math.Clamp(StillVelocitySquared, 0.0001f, 10f);
            StationSearchRadiusTiles = Math.Clamp(StationSearchRadiusTiles, 0, 60);
            EnvironmentSearchRadiusTiles = Math.Clamp(EnvironmentSearchRadiusTiles, 0, 80);
            BiomePlayerProbeRadiusTiles = Math.Clamp(BiomePlayerProbeRadiusTiles, 1, 200);
            MaxCraftsPerClusterPerScan = Math.Clamp(MaxCraftsPerClusterPerScan, 1, 50);
            NotifyRadiusTiles = Math.Clamp(NotifyRadiusTiles, 1, 200);
        }

        public string PlayerPermissionOrDefault()
        {
            return string.IsNullOrWhiteSpace(PlayerPermission) ? DefaultPlayerPermission : PlayerPermission.Trim();
        }

        public string AdminPermissionOrDefault()
        {
            return string.IsNullOrWhiteSpace(AdminPermission) ? DefaultAdminPermission : AdminPermission.Trim();
        }
    }

    public sealed class RecipeFile
    {
        public int Version { get; set; } = 1;
        public List<RecipeSpec> Recipes { get; set; } = new();

        public static RecipeFile Default()
        {
            return new RecipeFile
            {
                Recipes = new List<RecipeSpec>
                {
                    Recipe("torch_basic", ItemID.Torch, 6, new[] { I(ItemID.Wood, 2), I(ItemID.Gel, 1) }),
                    Recipe("rope_from_cobweb", ItemID.Rope, 12, new[] { I(ItemID.Cobweb, 2), I(ItemID.Wood, 1) }),
                    Recipe("workbench_ground", ItemID.WorkBench, 1, new[] { I(ItemID.Wood, 10), I(ItemID.StoneBlock, 2) }),
                    Recipe("campfire_ground", ItemID.Campfire, 1, new[] { I(ItemID.Wood, 10), I(ItemID.Torch, 5) }),
                    Recipe("glass_near_furnace", ItemID.Glass, 4, new[] { I(ItemID.SandBlock, 4), I(ItemID.Torch, 1) }, new[] { (int)TileID.Furnaces }),
                    Recipe("bottle_near_workbench", ItemID.Bottle, 2, new[] { I(ItemID.Glass, 2), I(ItemID.Wood, 1) }, new[] { (int)TileID.WorkBenches }),
                    Recipe("silk_near_loom", ItemID.Silk, 1, new[] { I(ItemID.Cobweb, 7), I(ItemID.Wood, 1) }, new[] { (int)TileID.Loom }),
                    Recipe("snow_brick_in_snow", ItemID.SnowBrick, 5, new[] { I(ItemID.SnowBlock, 4), I(ItemID.IceBlock, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Snow" } }),
                    Recipe("cloud_in_sky", ItemID.Cloud, 8, new[] { I(ItemID.Feather, 1), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Sky" } }),
                    Recipe("rain_cloud_at_sky_lake", ItemID.RainCloud, 8, new[] { I(ItemID.Cloud, 8), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Sky" }, Liquids = new List<string> { "Water" } }),
                    Recipe("mud_near_water", ItemID.MudBlock, 8, new[] { I(ItemID.DirtBlock, 8), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Liquids = new List<string> { "Water" } }),
                    Recipe("cactus_in_desert", ItemID.Cactus, 4, new[] { I(ItemID.SandBlock, 6), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Desert" } }),
                    Recipe("glowing_mushroom_in_mushroom_biome", ItemID.GlowingMushroom, 3, new[] { I(ItemID.MudBlock, 6), I(ItemID.Moonglow, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Mushroom" } }),
                    Recipe("enchanted_nightcrawler_surface", ItemID.EnchantedNightcrawler, 1, new[] { I(ItemID.Worm, 1), I(ItemID.FallenStar, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Surface" } }),
                    Recipe("suspicious_eye_before_eye", ItemID.SuspiciousLookingEye, 1, new[] { I(ItemID.Lens, 6), I(ItemID.FallenStar, 2) }, conditions: new ConditionsSpec { BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EyeOfCthulhu" } } }),
                    Recipe("worm_food_corruption_before_evil_boss", ItemID.WormFood, 1, new[] { I(ItemID.RottenChunk, 15), I(ItemID.VilePowder, 10) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Corruption" }, BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EaterOfWorldsOrBrain" } } }),
                    Recipe("bloody_spine_crimson_before_evil_boss", ItemID.BloodySpine, 1, new[] { I(ItemID.Vertebrae, 15), I(ItemID.ViciousPowder, 10) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Crimson" }, BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EaterOfWorldsOrBrain" } } }),
                    Recipe("hellstone_bar_underworld_hellforge", ItemID.HellstoneBar, 1, new[] { I(ItemID.Hellstone, 3), I(ItemID.Obsidian, 1) }, new[] { (int)TileID.Hellforge }, new ConditionsSpec { Layers = new List<string> { "Underworld" } }),
                    Recipe("disabled_example_after_any_mech", ItemID.CrystalShard, 2, new[] { I(ItemID.PixieDust, 4), I(ItemID.FallenStar, 1) }, conditions: new ConditionsSpec { BossProgress = new BossProgressSpec { Hardmode = true, AllDowned = new List<string> { "MechBossAny" } } }, enabled: false)
                }
            };
        }

        private static RecipeSpec Recipe(string id, int output, int outputStack, IEnumerable<ItemStackSpec> ingredients, IEnumerable<int>? requiredTiles = null, ConditionsSpec? conditions = null, bool enabled = true)
        {
            return new RecipeSpec
            {
                Id = id,
                Enabled = enabled,
                Output = I(output, outputStack),
                Ingredients = ingredients.ToList(),
                RequiredTiles = requiredTiles?.ToList() ?? new List<int>(),
                Conditions = conditions ?? new ConditionsSpec()
            };
        }

        private static ItemStackSpec I(int id, int stack)
        {
            return new ItemStackSpec { Id = id, Stack = stack };
        }
    }

    public sealed class RecipeSpec
    {
        public string Id { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public ItemStackSpec? Output { get; set; }
        public List<ItemStackSpec>? Ingredients { get; set; } = new();
        public List<int>? RequiredTiles { get; set; } = new();
        public ConditionsSpec? Conditions { get; set; } = new();
    }

    public sealed class ItemStackSpec
    {
        public int Id { get; set; }
        public int Stack { get; set; } = 1;
        public string? Name { get; set; }
    }

    public sealed class ConditionsSpec
    {
        public List<string>? Layers { get; set; } = new();
        public List<string>? Biomes { get; set; } = new();
        public List<string>? Liquids { get; set; } = new();
        public List<string>? AnyLiquids { get; set; } = new();
        public BossProgressSpec? BossProgress { get; set; } = new();
    }

    public sealed class BossProgressSpec
    {
        public bool? Hardmode { get; set; }
        public List<string>? AllDowned { get; set; } = new();
        public List<string>? AnyDowned { get; set; } = new();
        public List<string>? NoneDowned { get; set; } = new();
    }
}
