using Terraria;
using Terraria.ID;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    // 原版钓鱼宝匣 ID，取自 FishingCheck_RollItemDropOld 的 crate 分支（1.4.5.6）。
    // 服务端网络路径无法预知客户端的宝匣判定，只能靠这一杆客户端上报的 itemId 识别。
    private static readonly HashSet<int> _crateItemIds =
    [
        2334, 2335, 2336, // 木匣 / 铁匣 / 金匣
        3203, 3204, 3205, 3206, 3207, 3208, 3209, 3210, 3211, // 腐化/猩红/神圣/丛林/天空/海洋/冰冻/地牢匣
        3979, 3980, 3981, 3982, 3983, 3984, 3985, 3986, 3987, // 各生物群系硬模式匣
        4405, 4406, 4407, 4408, // 冰冻匣 / 绿洲匣 变体
        4877, 4878, // 黑曜石匣 / 狱石匣
        5002, 5003, // 海洋匣 变体
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
