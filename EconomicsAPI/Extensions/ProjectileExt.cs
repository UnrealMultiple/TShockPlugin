using Terraria;

namespace EconomicsAPI.Extensions;

public static class ProjectileExt
{
    public static List<Projectile> GetProjectileInRange(this Projectile proj, int range)
    {
        return Main.projectile.Where(p =>
        {
            float dx = p.position.X - proj.position.X;
            float dy = p.position.Y - proj.position.Y;
            return (dx * dx) + (dy * dy) <= range * range * 256f;
        }).ToList();
    }
}
