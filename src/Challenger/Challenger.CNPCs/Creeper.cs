using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger
{
    public class Creeper : CNPC
    {
        public Creeper(NPC npc)
            : base(npc)
        {
        }

        public override void OnKilled()
        {
            int num = Main.rand.Next(2, 6);
            for (int i = 0; i < num; i++)
            {
                float num2 = (float)Main.rand.NextDouble() - 0.5f;
                float num3 = -0.25f * (float)Math.Cos(3.1415927410125732 * (double)num2);
                Projectile.NewProjectile(null, npc.position, new Vector2(num2, num3) * 17f, 811, 15, 5f, -1, 0f, 0f, 0f);
            }
        }
    }
}
