using ChalleAnger;
using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger;

public class EaterofWorldsTail : CNPC
{
    public EaterofWorldsTail(NPC npc)
        : base(npc)
    {
    }

    public override void OnKilled()
    {
        if (EaterofWorldsBody.State == 0 || EaterofWorldsBody.State == 2)
        {
            var num = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 501, 13, 0f, -1, 0f, 0f, 0f);
            Main.projectile[num].timeLeft = 1;
            CProjectile.Update(num);
            for (var i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 3.0 * i, default) * 5f, 909, 14, 0f, -1, 0f, 0f, 0f);
            }
        }
    }
}