using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger
{

    public class SpiderArmorProj : CProjectile
    {
        private SpiderArmorProj(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void PreProjectileKilled()
        {
            if (proj.type == 249 && lable == 2012)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 val = Vector2.UnitY.RotatedBy((double)((float)Math.PI * 2f / 15f * i + (float)Math.PI / 15f), default);
                    int num = Collect.MyNewProjectile(proj.GetProjectileSource_FromThis(), proj.Center, val * 4f, 265, 40, 5f, proj.owner);
                    Update(num);
                }
                lable = 0;
            }
        }

        public static SpiderArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int lable, int owner, float[] ai)
        {
            int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 249, 20, 0f, owner);
            SpiderArmorProj spiderArmorProj = new SpiderArmorProj(Main.projectile[num], ai, lable);
            spiderArmorProj.lable = 2012;
            spiderArmorProj.Update();
            Collect.cprojs[num] = spiderArmorProj;
            return spiderArmorProj;
        }
    }
}
