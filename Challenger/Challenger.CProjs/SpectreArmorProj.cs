using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger
{
    internal class SpectreArmorProj : CProjectile
    {
        private SpectreArmorProj(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void ProjectileAI()
        {
            if (lable == 1)
            {
                Vector2 one = Vector2.One;
                one = (proj.Center - Main.player[proj.owner].Center).SafeNormalize(Vector2.UnitY * 100f);
                Vector2 val = one;
                Vector2 val2 = default;
                one = val.RotatedBy(1.03, val2) * 100f;
                proj.Center = Main.player[proj.owner].Center + one;
                Update();
                NPC val3 = Challenger.NearestHostileNPC(proj.Center, 1000000f);
                if (Main.rand.Next(40) == 0 && val3 != null)
                {
                    val2 = val3.Center - proj.Center;
                    float num = ((Vector2)val2).LengthSquared() / 1000000f;
                    int damage = (int)(40f + (1f - num) * 120f);
                    Vector2 val4 = (val3.Center - proj.Center).SafeNormalize(Vector2.Zero);
                    int num2 = Collect.MyNewProjectile(null, proj.Center, val4 * 4f, 356, damage, 0f, proj.owner);
                    Main.projectile[num2].timeLeft = 480;
                    Update(num2);
                }
            }
        }

        public static SpectreArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
        {
            int num = Collect.MyNewProjectile(null, position, velocity, 299, 0, 0f, owner);
            SpectreArmorProj spectreArmorProj = new SpectreArmorProj(Main.projectile[num], ai, lable);
            Collect.cprojs[num] = spectreArmorProj;
            Update(num);
            return spectreArmorProj;
        }
    }
}
