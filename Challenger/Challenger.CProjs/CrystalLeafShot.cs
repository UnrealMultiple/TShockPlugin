using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class CrystalLeafShot : CProjectile
    {
        public CrystalLeafShot(Projectile projectile)
            : base(projectile)
        {
        }

        public CrystalLeafShot(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void MyEffect()
        {
            try
            {
                if (lable == 0 && proj.type == 227)
                {
                    int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), Main.player[proj.owner].Center + new Vector2(0f, -60f), proj.velocity.RotatedBy(-0.05, default), 227, 75, 5f, proj.owner);
                    Collect.cprojs[num] = new CrystalLeafShot(Main.projectile[num], new float[0], 1);
                    Update(num);
                    num = Collect.MyNewProjectile(Projectile.GetNoneSource(), Main.player[proj.owner].Center + new Vector2(0f, -60f), proj.velocity.RotatedBy(0.05, default), 227, 75, 5f, proj.owner);
                    Collect.cprojs[num] = new CrystalLeafShot(Main.projectile[num], new float[0], 1);
                    Update(num);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error("CrystalLeafShot -> MyEffect 异常：" + ex.ToString());
                Console.WriteLine("CrystalLeafShot -> MyEffect 异常：" + ex.ToString());
            }
        }
    }
}
