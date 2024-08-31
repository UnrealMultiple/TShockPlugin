using Terraria;

namespace EconomicsAPI.Extensions;

public static class ProjectileExt
{
    public static List<Projectile> GetProjectileInRange(this Projectile proj, int range)
    {
        return proj.position.FindRangeProjectiles(range);
    }
}