using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger;

internal class SpectreArmorProj : CProjectile
{
    private SpectreArmorProj(Projectile projectile, float[] ai, int lable)
        : base(projectile, ai, lable)
    {
    }

    public override void ProjectileAI()
    {
        var any = Challenger.config.EnableSpectreArmorEffect_3;
        var any2 = Challenger.config.EnableSpectreArmorEffect_4;
        var any3 = Challenger.config.EnableSpectreArmorEffect_5;
        var any5 = Challenger.config.EnableSpectreArmorEffect_9;
        if (this.lable == 1)
        {
            var one = Vector2.One;
            one = (this.proj.Center - Main.player[this.proj.owner].Center).SafeNormalize(Vector2.UnitY * 100f);
            var val = one;
            Vector2 val2 = default;
            one = val.RotatedBy(1.03, val2) * 100f;
            this.proj.Center = Main.player[this.proj.owner].Center + one;
            this.Update();
            var val3 = Challenger.NearestHostileNPC(this.proj.Center, 1000000f);
            if (Main.rand.Next(40) == 0 && val3 != null && Challenger.Timer % any5 == 0)
            {
                val2 = val3.Center - this.proj.Center;
                var num = ((Vector2) val2).LengthSquared() / 1000000f;
                var damage = (int) (40f + ((1f - num) * any2));
                var val4 = (val3.Center - this.proj.Center).SafeNormalize(Vector2.Zero);
                var num2 = Collect.MyNewProjectile(null, this.proj.Center, val4 * 4f, any, damage, any3, this.proj.owner);
                Main.projectile[num2].timeLeft = 480;
                Update(num2);
            }
        }
    }

    public static SpectreArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
    {
        var any4 = Challenger.config.EnableSpectreArmorEffect_6;
        var any5 = Challenger.config.EnableSpectreArmorEffect_7;
        var any6 = Challenger.config.EnableSpectreArmorEffect_8;

        var num = Collect.MyNewProjectile(null, position, velocity, any4, any5, any6, owner);
        var spectreArmorProj = new SpectreArmorProj(Main.projectile[num], ai, lable);
        Collect.cprojs[num] = spectreArmorProj;
        Update(num);
        return spectreArmorProj;
    }
}