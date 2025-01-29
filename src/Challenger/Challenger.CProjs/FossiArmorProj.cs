using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class FossiArmorProj : CProjectile
{
    private FossiArmorProj(Projectile projectile, float[] ai, int lable)
        : base(projectile, ai, lable)
    {
    }

    public override void ProjectileAI()
    {
        var any = Challenger.Config.FossilArmorEffect_0;
        var any2 = Challenger.Config.FossilArmorEffect_1;
        var any3 = Challenger.Config.FossilArmorDamage;
        var any4 = Challenger.Config.FossilArmorEffect_2;
        var any5 = Challenger.Config.FossilArmorTime;
        var any6 = Challenger.Config.FossilArmorEffect_Range;

        this.proj.Center = Main.player[this.proj.owner].Center + new Vector2(0f, -48f);
        TSPlayer.All.SendData((PacketTypes) 27, "", this.proj.whoAmI);
        if (Main.time % any5 == 0.0)
        {
            var val = Challenger.NearestHostileNPC(this.proj.Center, any6 * 100);
            if (val != null)
            {
                var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), this.proj.Center, this.proj.Center.DirectionTo(val.Center) * any2, any, any3, any4, this.proj.owner);
                Update(num);
            }
        }
    }

    public static FossiArmorProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
    {
        var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 597, 0, 0f, owner);
        var fossiArmorProj = new FossiArmorProj(Main.projectile[num], ai, lable);
        fossiArmorProj.proj.tileCollide = false;
        Collect.Cprojs[num] = fossiArmorProj;
        Update(num);
        return fossiArmorProj;
    }
}