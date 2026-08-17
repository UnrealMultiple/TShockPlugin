using Terraria;
using Terraria.ID;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private FishingLootConfig NormalizeConfig(FishingLootConfig config, List<string> warnings)
    {
        FishingLootConfig upgraded = UpgradeLegacyConfig(config);
        HashSet<int> blocked = new(upgraded.BlockedItemIds.Where(id => id > 0));

        upgraded.Notes ??= new List<string>();
        upgraded.Mode = NormalizeMode(upgraded.Mode, out string? modeWarning);
        if (modeWarning != null)
            warnings.Add(modeWarning);

        upgraded.RandomNpc ??= new RandomNpcConfig();
        upgraded.RandomNpc.ReplaceChancePercent = Math.Clamp(upgraded.RandomNpc.ReplaceChancePercent, 0, 100);
        upgraded.RandomNpc.BlockedNpcIds = upgraded.RandomNpc.BlockedNpcIds.Where(id => id > 0).Distinct().OrderBy(id => id).ToList();
        upgraded.AlwaysAvailable = NormalizeEntries(upgraded.AlwaysAvailable, blocked);
        upgraded.Stages = upgraded.Stages
            .Select(stage => NormalizeStage(stage, blocked))
            .Where(stage => stage.Items.Count > 0)
            .ToList();

        if (upgraded.Stages.Count == 0)
            return BuildDefaultConfig();

        return upgraded;
    }

    private static FishingLootConfig UpgradeLegacyConfig(FishingLootConfig config)
    {
        if (config.Stages.Count > 0 || config.AlwaysAvailable.Count > 0)
            return config;

        // 旧格式（无阶段/常驻池）配置：重建默认渔获表，但保留所有用户已配置的字段。
        FishingLootConfig upgraded = BuildDefaultConfig();
        upgraded.Enabled = config.Enabled;
        upgraded.AnnounceToPlayer = config.AnnounceToPlayer;
        if (!string.IsNullOrWhiteSpace(config.Mode))
            upgraded.Mode = config.Mode;
        upgraded.BlockedItemIds = config.BlockedItemIds ?? new List<int>();
        upgraded.RandomNpc = config.RandomNpc ?? new RandomNpcConfig();
        return upgraded;
    }

    private static FishingLootStage NormalizeStage(FishingLootStage stage, HashSet<int> blocked)
    {
        stage.Id = NormalizeKey(stage.Id, "stage");
        stage.Name = stage.Name.Trim().Length == 0 ? stage.Id : stage.Name.Trim();
        stage.Description = stage.Description.Trim();
        stage.Unlock = NormalizeConditions(stage.Unlock);
        stage.Items = NormalizeEntries(stage.Items, blocked);
        return stage;
    }

    private static UnlockConditions NormalizeConditions(UnlockConditions? conditions)
    {
        conditions ??= new UnlockConditions();
        conditions.Defeated = NormalizeKeys(conditions.Defeated);
        conditions.NotDefeated = NormalizeKeys(conditions.NotDefeated);
        conditions.GameMode = conditions.GameMode is int gameMode ? Math.Clamp(gameMode, 0, 3) : null;
        return conditions;
    }

    private static List<LootEntry> NormalizeEntries(IEnumerable<LootEntry>? entries, HashSet<int> blocked)
    {
        List<LootEntry> result = new();
        if (entries == null)
            return result;

        foreach (LootEntry entry in entries)
        {
            if (!TryNormalizeEntry(entry, blocked, out LootEntry? normalized))
                continue;

            result.Add(normalized!);
        }

        return result;
    }

    private static bool TryNormalizeEntry(LootEntry? entry, HashSet<int> blocked, out LootEntry? normalized)
    {
        normalized = null;
        if (entry == null || entry.ItemId <= 0 || blocked.Contains(entry.ItemId))
            return false;

        Item item = new();
        item.SetDefaults(entry.ItemId);
        if (item.IsAir || item.type <= 0 || IsDisallowedCombatItem(item))
            return false;

        int maxStack = Math.Max(1, item.maxStack);
        int minStack = Math.Clamp(entry.MinStack <= 0 ? 1 : entry.MinStack, 1, maxStack);
        int maxAllowed = entry.MaxStack <= 0 ? minStack : entry.MaxStack;
        int maxStackValue = Math.Clamp(maxAllowed, minStack, maxStack);

        normalized = new LootEntry
        {
            Name = NormalizeDisplayName(entry.Name, entry.ItemId),
            ItemId = entry.ItemId,
            Weight = Math.Max(1, entry.Weight),
            MinStack = minStack,
            MaxStack = maxStackValue
        };
        return true;
    }

    private static bool IsDisallowedCombatItem(Item item)
    {
        if (item.damage <= 0)
            return false;

        if (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.fishingPole > 0)
            return false;

        return true;
    }

    private static string NormalizeDisplayName(string? configured, int itemId)
    {
        string clean = configured?.Trim() ?? string.Empty;
        return clean.Length == 0 ? Lang.GetItemNameValue(itemId) : clean;
    }

    private static string NormalizeKey(string? value, string fallback)
    {
        string clean = (value ?? string.Empty).Trim().ToLowerInvariant();
        return clean.Length == 0 ? fallback : clean;
    }

    private static List<string> NormalizeKeys(IEnumerable<string>? values)
    {
        if (values == null)
            return new List<string>();

        return values
            .Select(value => NormalizeKey(value, string.Empty))
            .Where(value => value.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeMode(string? mode, out string? warning)
    {
        string value = (mode ?? string.Empty).Trim().ToLowerInvariant();
        warning = null;

        if (value.Length == 0 || value is "progression_items" or "items")
            return "progression_items";

        if (value is "random_npcs" or "npcs" or "npc")
            return "random_npcs";

        // 未知模式不再静默切换，回退到默认模式并向管理员告警。
        warning = $"配置中的 Mode \"{mode}\" 不受支持，已回退为 progression_items（可选值：progression_items / random_npcs）。";
        return "progression_items";
    }
}
