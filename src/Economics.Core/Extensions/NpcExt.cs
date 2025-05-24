using Terraria;

namespace Economics.Core.Extensions;

public static class NpcExt
{
    public static List<NPC> GetNpcInRange(this NPC npc, int range)
    {
        return npc.position.FindRangeNPCs(range);
    }
}