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
    private sealed record DropRef(int Index, WorldItem Item, Vector2 Center);

    private sealed record IngredientTake(DropRef Drop, int Type, int Stack);

    private sealed record AnimatedIngredient(int Index, int Type, int Stack, Vector2 StartCenter, int Width, int Height);

    private sealed record DeferredLeftover(int Type, int Stack, Vector2 Center);

    private sealed class CraftAnimation(
        DropRecipe Recipe,
        Vector2 Center,
        int OutputStack,
        int CraftCount,
        bool IsZenith,
        Dictionary<int, int> ConsumedStacks,
        List<AnimatedIngredient> Ingredients,
        List<DeferredLeftover> Leftovers)
    {
        public DropRecipe Recipe { get; } = Recipe;
        public Vector2 Center { get; } = Center;
        public int OutputStack { get; } = OutputStack;
        public int CraftCount { get; } = CraftCount;
        public bool IsZenith { get; } = IsZenith;
        public Dictionary<int, int> ConsumedStacks { get; } = ConsumedStacks;
        public List<AnimatedIngredient> Ingredients { get; } = Ingredients;
        public List<DeferredLeftover> Leftovers { get; } = Leftovers;
        public int Age { get; set; }
    }

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
        public long ExtraItemTypeRejects { get; set; }
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
        public int MaxCraftsPerClusterPerScan { get; set; } = 25;
        public bool RequireExactIngredientTypes { get; set; } = true;
        public bool AnimateConsumedItems { get; set; } = true;
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
                    Recipe("water_candle_ground", ItemID.WaterCandle, 1, new[] { I(ItemID.Torch, 3), I(ItemID.BottledWater, 1), I(ItemID.FallenStar, 1) }, new[] { (int)TileID.WorkBenches }),
                    Recipe("silk_near_loom", ItemID.Silk, 1, new[] { I(ItemID.Cobweb, 7), I(ItemID.Wood, 1) }, new[] { (int)TileID.Loom }),
                    Recipe("snow_brick_in_snow", ItemID.SnowBrick, 5, new[] { I(ItemID.SnowBlock, 4), I(ItemID.IceBlock, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Snow" } }),
                    Recipe("cloud_in_sky", ItemID.Cloud, 8, new[] { I(ItemID.Feather, 1), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Sky" } }),
                    Recipe("rain_cloud_at_sky_lake", ItemID.RainCloud, 8, new[] { I(ItemID.Cloud, 8), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Sky" }, Liquids = new List<string> { "Water" } }),
                    Recipe("mud_near_water", ItemID.MudBlock, 8, new[] { I(ItemID.DirtBlock, 8), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Liquids = new List<string> { "Water" } }),
                    Recipe("cactus_in_desert", ItemID.Cactus, 4, new[] { I(ItemID.SandBlock, 6), I(ItemID.BottledWater, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Desert" } }),
                    Recipe("glowing_mushroom_in_mushroom_biome", ItemID.GlowingMushroom, 3, new[] { I(ItemID.MudBlock, 6), I(ItemID.Moonglow, 1) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Mushroom" } }),
                    Recipe("enchanted_nightcrawler_surface", ItemID.EnchantedNightcrawler, 1, new[] { I(ItemID.Worm, 1), I(ItemID.FallenStar, 1) }, conditions: new ConditionsSpec { Layers = new List<string> { "Surface" } }),
                    Recipe("life_crystal_ground", ItemID.LifeCrystal, 1, new[] { I(ItemID.GoldBar, 8), I(ItemID.FallenStar, 5), I(ItemID.HealingPotion, 3) }, new[] { (int)TileID.Anvils }),
                    Recipe("wormhole_potion_ground", ItemID.WormholePotion, 1, new[] { I(ItemID.BottledWater, 1), I(ItemID.Blinkroot, 1), I(ItemID.FallenStar, 1), I(ItemID.RecallPotion, 1) }, new[] { (int)TileID.Bottles }),
                    Recipe("suspicious_eye_before_eye", ItemID.SuspiciousLookingEye, 1, new[] { I(ItemID.Lens, 6), I(ItemID.FallenStar, 2) }, conditions: new ConditionsSpec { BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EyeOfCthulhu" } } }),
                    Recipe("bloody_tear_graveyard", 4271, 1, new[] { I(ItemID.Deathweed, 3), I(ItemID.Lens, 2), I(ItemID.FallenStar, 1), I(ItemID.BottledWater, 1) }, new[] { (int)TileID.DemonAltar }, conditions: new ConditionsSpec { Biomes = new List<string> { "Graveyard" } }),
                    Recipe("worm_food_corruption_before_evil_boss", ItemID.WormFood, 1, new[] { I(ItemID.RottenChunk, 15), I(ItemID.VilePowder, 10) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Corruption" }, BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EaterOfWorldsOrBrain" } } }),
                    Recipe("bloody_spine_crimson_before_evil_boss", ItemID.BloodySpine, 1, new[] { I(ItemID.Vertebrae, 15), I(ItemID.ViciousPowder, 10) }, conditions: new ConditionsSpec { Biomes = new List<string> { "Crimson" }, BossProgress = new BossProgressSpec { NoneDowned = new List<string> { "EaterOfWorldsOrBrain" } } }),
                    Recipe("hellstone_bar_underworld_hellforge", ItemID.HellstoneBar, 1, new[] { I(ItemID.Hellstone, 3), I(ItemID.Obsidian, 1) }, new[] { (int)TileID.Hellforge }, new ConditionsSpec { Layers = new List<string> { "Underworld" } }),
                    Recipe("zenith_ground_mythril_anvil", ItemID.Zenith, 1, new[] { I(ItemID.CopperShortsword, 1), I(ItemID.EnchantedSword, 1), I(ItemID.Starfury, 1), I(ItemID.BeeKeeper, 1), I(ItemID.Seedler, 1), I(ItemID.TheHorsemansBlade, 1), I(ItemID.InfluxWaver, 1), I(ItemID.TerraBlade, 1), I(ItemID.Meowmere, 1), I(ItemID.StarWrath, 1) }, new[] { (int)TileID.MythrilAnvil }),
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
