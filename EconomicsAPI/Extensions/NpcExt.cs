using Terraria;

namespace EconomicsAPI.Extensions;

public static class NpcExt
{
    public static List<NPC> GetNpcInRange(this NPC npc, int range)
    {
        return Main.npc.Where(n =>
        {
            float dx = n.position.X - npc.position.X;
            float dy = n.position.Y - npc.position.Y;
            return (dx * dx) + (dy * dy) <= range * range * 256f;
        }).ToList();
    }
}
