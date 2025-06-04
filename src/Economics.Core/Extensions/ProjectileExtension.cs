using Terraria;

namespace Economics.Core.Extensions;

public static class ProjectileExtension
{
    public static List<Projectile> GetProjectileInRange(this Projectile proj, int range)
    {
        return proj.position.FindRangeProjectiles(range);
    }
}