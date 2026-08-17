namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private sealed class FishingLootConfig
    {
        public int Version { get; set; } = 2;
        public List<string> Notes { get; set; } = new();
        public bool Enabled { get; set; } = true;
        public string Mode { get; set; } = "progression_items";
        public bool AnnounceToPlayer { get; set; }
        public bool AllowQuestFish { get; set; } = true;
        public bool AllowCrates { get; set; } = true;
        public List<int> BlockedItemIds { get; set; } = new();
        public RandomNpcConfig RandomNpc { get; set; } = new();
        public List<LootEntry> AlwaysAvailable { get; set; } = new();
        public List<FishingLootStage> Stages { get; set; } = new();

        public static FishingLootConfig CreateDefault()
        {
            return BuildDefaultConfig();
        }
    }

    private sealed class RandomNpcConfig
    {
        public int ReplaceChancePercent { get; set; } = 100;
        public bool IncludeBosses { get; set; } = true;
        public bool IncludeFriendlyNPCs { get; set; } = true;
        public bool IncludeTownNPCs { get; set; }
        public List<int> BlockedNpcIds { get; set; } = new();
    }

    private sealed class FishingLootStage
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UnlockConditions Unlock { get; set; } = new();
        public List<LootEntry> Items { get; set; } = new();
    }

    private sealed class UnlockConditions
    {
        public bool? HardMode { get; set; }
        public int? GameMode { get; set; }
        public List<string> Defeated { get; set; } = new();
        public List<string> NotDefeated { get; set; } = new();

        public bool IsEmpty()
        {
            return HardMode == null
                && GameMode == null
                && Defeated.Count == 0
                && NotDefeated.Count == 0;
        }
    }

    private sealed class LootEntry
    {
        public string Name { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public int Weight { get; set; } = 1;
        public int MinStack { get; set; } = 1;
        public int MaxStack { get; set; } = 1;
    }
}
