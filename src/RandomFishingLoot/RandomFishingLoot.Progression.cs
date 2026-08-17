using Terraria;
using Terraria.ID;

using TShockAPI;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private LootPool BuildCurrentPool()
    {
        FishingLootStage? activeStage = ResolveActiveStage();
        List<PendingLootChoice> entries = new();

        foreach (LootEntry entry in _config.AlwaysAvailable)
            entries.Add(ToChoice(entry, "always", "常驻"));

        if (activeStage != null)
        {
            foreach (LootEntry entry in activeStage.Items)
                entries.Add(ToChoice(entry, activeStage.Id, activeStage.Name));
        }

        return LootPool.Create(entries);
    }

    private PendingLootChoice? RollCurrentLootChoice()
    {
        return BuildCurrentPool().Next();
    }

    private FishingLootStage? ResolveActiveStage()
    {
        FishingLootStage? active = null;
        foreach (FishingLootStage stage in _config.Stages)
        {
            if (StageUnlocked(stage))
                active = stage;
        }

        return active;
    }

    private bool StageUnlocked(FishingLootStage stage)
    {
        UnlockConditions conditions = stage.Unlock;
        if (conditions.HardMode is bool hardMode && Main.hardMode != hardMode)
            return false;

        if (conditions.GameMode is int gameMode && Main.GameMode != gameMode)
            return false;

        foreach (string boss in conditions.Defeated)
        {
            if (!BossConditionMet(boss))
                return false;
        }

        foreach (string boss in conditions.NotDefeated)
        {
            if (BossConditionMet(boss))
                return false;
        }

        return true;
    }

    private static bool BossConditionMet(string boss)
    {
        return boss switch
        {
            "slimeking" or "king_slime" => NPC.downedSlimeKing,
            "eye" or "eyeofcthulhu" or "eye_of_cthulhu" => NPC.downedBoss1,
            "evilboss" or "eaterorbrain" or "eater_or_brain" => NPC.downedBoss2,
            "skeletron" => NPC.downedBoss3,
            "queenbee" or "queen_bee" => NPC.downedQueenBee,
            "wof" or "wallofflesh" or "wall_of_flesh" => Main.hardMode,
            "anymech" or "mechany" or "mech_any" => NPC.downedMechBossAny,
            "twins" => NPC.downedMechBoss2,
            "destroyer" => NPC.downedMechBoss1,
            "skeletronprime" or "skeletron_prime" => NPC.downedMechBoss3,
            "allmechs" or "mechall" or "mech_all" => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3,
            "plantera" => NPC.downedPlantBoss,
            "golem" => NPC.downedGolemBoss,
            "cultist" or "lunaticcultist" or "lunatic_cultist" => NPC.downedAncientCultist,
            "moonlord" or "moon_lord" => NPC.downedMoonlord,
            _ => false
        };
    }

    private static PendingLootChoice ToChoice(LootEntry entry, string stageId, string stageName)
    {
        return new PendingLootChoice(
            entry.ItemId,
            NormalizeDisplayName(entry.Name, entry.ItemId),
            Math.Max(1, entry.Weight),
            entry.MinStack,
            entry.MaxStack,
            GetItemMaxStack(entry.ItemId),
            stageId,
            stageName);
    }

    private static int RollStack(int minStack, int maxStack, int itemMaxStack)
    {
        int min = Math.Clamp(minStack, 1, Math.Max(1, itemMaxStack));
        int max = Math.Clamp(maxStack, min, Math.Max(1, itemMaxStack));
        return min == max ? min : Random.Shared.Next(min, max + 1);
    }

    private static int GetItemMaxStack(int itemId)
    {
        Item item = new();
        item.SetDefaults(itemId);
        return Math.Max(1, item.maxStack);
    }

    private static string BuildLoadSummary(FishingLootConfig config)
    {
        int stageItems = config.Stages.Sum(stage => stage.Items.Count);
        string mode = NormalizeMode(config.Mode) == "random_npcs" ? "随机生物" : "进度物品";
        string questFish = config.AllowQuestFish ? "，允许钓任务鱼" : "，不钓任务鱼";
        string crate = config.AllowCrates ? "，保留宝匣" : "，宝匣被替换";
        return $"随机渔获已载入：模式 {mode}；常驻 {config.AlwaysAvailable.Count} 项，阶段 {config.Stages.Count} 个，阶段物品 {stageItems} 项{questFish}{crate}。";
    }

    private static TSPlayer? PlayerByIndex(int whoAmI)
    {
        return whoAmI >= 0 && whoAmI < TShock.Players.Length ? TShock.Players[whoAmI] : null;
    }
}
