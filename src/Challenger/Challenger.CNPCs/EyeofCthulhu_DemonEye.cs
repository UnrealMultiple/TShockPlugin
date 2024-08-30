using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Challenger
{
    public class EyeofCthulhu_DemonEye : CNPC
    {
        public static readonly float CooldownOfSkill0 = 200f;

        public float skill0 = CooldownOfSkill0;

        public EyeofCthulhu_DemonEye(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            skill0 -= 1f;
            NPCAimedTarget targetData = npc.GetTargetData(true);
            Vector2 val = npc.DirectionTo(targetData.Position + new Vector2(Main.rand.Next(-32, 33), Main.rand.Next(-32, 33)));
            if (skill0 < 0f && val.X * npc.velocity.X > 0f && npc.HasPlayerTarget)
            {
                Projectile.NewProjectile(null, npc.Center, val * 6f, 84, 4, 5f, -1, 0f, 0f, 0f);
                skill0 += CooldownOfSkill0 + Main.rand.Next(51);
            }
        }
    }
}
