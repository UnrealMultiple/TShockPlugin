

using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

internal class AIStyle
{
    public static void 环绕(Projectile projectile)
    {
        var val = (projectile.Center - Main.player[projectile.owner].Center).SafeNormalize(Vector2.UnitY * 100f);
        var one = val.RotatedBy(1.03, default) * 100f;
        projectile.Center = Main.player[projectile.owner].Center + one;
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
        var target = projectile.position.FindRangeNPC(1000000f);
        if (Main.rand.Next(40) == 0 && target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY) * projectile.velocity.Length();
            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(10), 79, (int)50, 5, projectile.owner);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        }
    }
}
