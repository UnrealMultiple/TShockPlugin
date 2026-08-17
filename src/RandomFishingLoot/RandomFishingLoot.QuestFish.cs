using Terraria;
using Terraria.ID;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    // 原版钓鱼宝匣 ItemID，取自 FishingCheck_RollItemDropOld 的 crate 分支（1.4.5.6）。
    // 使用命名常量而非魔法数字，Terraria 版本更新时由编译器直接校验。
    // 服务端网络路径无法预知客户端的宝匣判定，只能靠这一杆客户端上报的 itemId 识别。
    private static readonly HashSet<int> _crateItemIds =
    [
        ItemID.WoodenCrate,
        ItemID.IronCrate,
        ItemID.GoldenCrate,
        ItemID.CorruptFishingCrate,
        ItemID.CrimsonFishingCrate,
        ItemID.DungeonFishingCrate,
        ItemID.FloatingIslandFishingCrate,
        ItemID.HallowedFishingCrate,
        ItemID.JungleFishingCrate,
        ItemID.WoodenCrateHard,
        ItemID.IronCrateHard,
        ItemID.GoldenCrateHard,
        ItemID.CorruptFishingCrateHard,
        ItemID.CrimsonFishingCrateHard,
        ItemID.DungeonFishingCrateHard,
        ItemID.FloatingIslandFishingCrateHard,
        ItemID.HallowedFishingCrateHard,
        ItemID.JungleFishingCrateHard,
        ItemID.FrozenCrate,
        ItemID.FrozenCrateHard,
        ItemID.OasisCrate,
        ItemID.OasisCrateHard,
        ItemID.LavaCrate,
        ItemID.LavaCrateHard,
        ItemID.OceanCrate,
        ItemID.OceanCrateHard
    ];

    private static bool IsCrateItem(int itemId)
    {
        return _crateItemIds.Contains(itemId);
    }

    private static bool TryGetQuestFish(int playerIndex, out int questFish)
    {
        questFish = -1;
        if (playerIndex < 0 || playerIndex >= Main.player.Length)
            return false;

        if (Main.anglerQuestFinished)
            return false;

        if (Main.anglerQuest < 0 || Main.anglerQuest >= Main.anglerQuestItemNetIDs.Length)
            return false;

        if (!NPC.AnyNPCs(NPCID.Angler))
            return false;

        int candidate = Main.anglerQuestItemNetIDs[Main.anglerQuest];
        if (Main.player[playerIndex].HasItem(candidate))
            return false;

        questFish = candidate;
        return true;
    }

    private static bool RollVanillaQuestFishChance(int fishingLevel)
    {
        int threshold = 150 * 2 / Math.Max(1, fishingLevel);
        if (threshold < 3)
            threshold = 3;

        return Random.Shared.Next(150) < threshold;
    }

    private static int GetFinalFishingLevel(int playerIndex)
    {
        try
        {
            Player? player = playerIndex >= 0 && playerIndex < Main.player.Length ? Main.player[playerIndex] : null;
            if (player?.active == true)
                return player.GetFishingConditions().FinalFishingLevel;
        }
        catch
        {
        }

        return 50;
    }

    private static PendingLootChoice MakeQuestFishChoice(int questFish)
    {
        return new PendingLootChoice(
            questFish,
            NormalizeDisplayName(null, questFish),
            1,
            1,
            1,
            GetItemMaxStack(questFish),
            "quest",
            "任务鱼");
    }
}
