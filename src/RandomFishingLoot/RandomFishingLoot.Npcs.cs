using Terraria;
using Terraria.ID;

namespace RandomFishingLoot;

public sealed partial class RandomFishingLoot
{
    private static NpcPool BuildNpcPool(RandomNpcConfig config)
    {
        HashSet<int> blocked = new(config.BlockedNpcIds.Where(id => id > 0));
        List<PendingNpcChoice> choices = new();

        for (int npcId = 1; npcId < NPCID.Count; npcId++)
        {
            if (blocked.Contains(npcId))
                continue;

            if (!TryCreateNpcChoice(npcId, config, out PendingNpcChoice? choice))
                continue;

            choices.Add(choice!);
        }

        return NpcPool.Create(choices);
    }

    private static bool TryCreateNpcChoice(int npcId, RandomNpcConfig config, out PendingNpcChoice? choice)
    {
        choice = null;

        NPC npc = new();
        npc.SetDefaults(npcId);

        if (npc.type <= 0 || npc.lifeMax <= 0)
            return false;

        if (!config.IncludeTownNPCs && npc.townNPC)
            return false;

        if (!config.IncludeFriendlyNPCs && npc.friendly)
            return false;

        if (!config.IncludeBosses && npc.boss)
            return false;

        if (IsNpcObviouslyInvalid(npc))
            return false;

        choice = new PendingNpcChoice(npcId, Lang.GetNPCNameValue(npcId));
        return true;
    }

    private static bool IsNpcObviouslyInvalid(NPC npc)
    {
        if (npc.type <= 0)
            return true;

        if (npc.damage <= 0 && !npc.friendly && !npc.boss && npc.lifeMax <= 5)
            return true;

        return npc.type is NPCID.TargetDummy or NPCID.BunnyXmas or NPCID.PresentMimic;
    }
}
