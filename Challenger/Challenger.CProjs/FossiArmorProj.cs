using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class FossiArmorProj : CProjectile
    {
        private FossiArmorProj(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void ProjectileAI()
        {
            proj.Center = Main.player[proj.owner].Center + new Vector2(0f, -48f);
            TSPlayer.All.SendData((PacketTypes)27, "", proj.whoAmI, 0f, 0f, 0f, 0);
            if (Main.time % 10.0 == 0.0)
            {
                NPC val = global::Challenger.Challenger.NearestHostileNPC(proj.Center, 62500f);
                if (val != null)
                {
                    int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), proj.Center, proj.Center.DirectionTo(val.Center) * 18f, 732, 10, 8f, proj.owner);
                    Update(num);
                }
            }
        }

        public static FossiArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
        {
            int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 597, 0, 0f, owner);
            FossiArmorProj fossiArmorProj = new FossiArmorProj(Main.projectile[num], ai, lable);
            fossiArmorProj.proj.tileCollide = false;
            Collect.cprojs[num] = fossiArmorProj;
            Update(num);
            return fossiArmorProj;
        }
    }
}
