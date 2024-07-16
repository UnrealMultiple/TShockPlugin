
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

public class AISytle
{

    public static void AI(Projectile projectile, int sytle)
    {
        switch (sytle)
        {
            case 0:
                Revolve(projectile);
                break;
        }
    }

    public static void Revolve(Projectile projectile)
    {
        Task.Run(async () =>
        {
            int i = 0;
            while (i < 20 && projectile.active)
            {
                var val = (projectile.Center - Main.player[projectile.owner].Center).SafeNormalize(Vector2.UnitY * 200f);
                var one = val.RotatedBy(1.03, default) * 200f;
                projectile.Center = Main.player[projectile.owner].Center + one;
                TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
                await Task.Delay(100);
                i++;
            }
        });

      
        //var target = projectile.position.FindRangeNPC(1000000f);
        //if (Main.rand.Next(40) == 0 && target != null)
        //{
        //    var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY) * projectile.velocity.Length();
        //    int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(10), 79, 50, 5, projectile.owner);
        //    TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        //}
    }
}
