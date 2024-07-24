using Terraria;

namespace EconomicsAPI.Extensions;

public static class NpcExt
{
    public static List<NPC> GetNpcInRange(this NPC npc, int range)
    {
        return npc.position.FindRangeNPCs(range);
    }
}
