using Terraria;

namespace Challenger;

public class Skeletron_Surrand : CNPC
{
    private int skill0 = 240;

    public Skeletron_Surrand(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.skill0--;
        if (this.skill0 < 0)
        {
            var num = NPC.NewNPC(this.npc.GetSpawnSourceForNPCFromNPCAI(), (int) this.npc.Center.X, (int) this.npc.Center.Y, 33);
            Main.npc[num].lifeMax = 100;
            Main.npc[num].life = 101;
            this.skill0 = 180 + Main.rand.Next(-60, 61);
        }
    }

    public override void OnKilled()
    {
        var targetData = this.npc.GetTargetData();
        Projectile.NewProjectile(null, this.npc.Center, this.npc.Center.DirectionTo(targetData.Center) * 5f, 270, 6, 30f);
    }
}