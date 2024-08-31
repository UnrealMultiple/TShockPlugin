using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger;


public class SpiderArmorProj : CProjectile
{
    private SpiderArmorProj(Projectile projectile, float[] ai, int lable)
        : base(projectile, ai, lable)
    {
    }

    public override void PreProjectileKilled()
    {
        if (this.proj.type == 249 && this.lable == 2012)
        {
            for (var i = 0; i < 15; i++)
            {
                var val = Vector2.UnitY.RotatedBy((double) (((float) Math.PI * 2f / 15f * i) + ((float) Math.PI / 15f)), default);
                var num = Collect.MyNewProjectile(this.proj.GetProjectileSource_FromThis(), this.proj.Center, val * 4f, 265, 40, 5f, this.proj.owner);
                Update(num);
            }
            this.lable = 0;
        }
    }

    public static SpiderArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int lable, int owner, float[] ai)
    {
        var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 249, 20, 0f, owner);
        var spiderArmorProj = new SpiderArmorProj(Main.projectile[num], ai, lable)
        {
            lable = 2012
        };
        spiderArmorProj.Update();
        Collect.cprojs[num] = spiderArmorProj;
        return spiderArmorProj;
    }
}