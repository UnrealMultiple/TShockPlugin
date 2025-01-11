using Microsoft.Xna.Framework;
using Terraria;

namespace Challenger;

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
        this.skill0 -= 1f;
        var targetData = this.npc.GetTargetData(true);
        var val = this.npc.DirectionTo(targetData.Position + new Vector2(Main.rand.Next(-32, 33), Main.rand.Next(-32, 33)));
        if (this.skill0 < 0f && val.X * this.npc.velocity.X > 0f && this.npc.HasPlayerTarget)
        {
            Projectile.NewProjectile(null, this.npc.Center, val * 6f, 84, 4, 5f, -1, 0f, 0f, 0f);
            this.skill0 += CooldownOfSkill0 + Main.rand.Next(51);
        }
    }
}