using Terraria;
using Terraria.DataStructures;

namespace Challenger
{
    public class Skeletron_Surrand : CNPC
    {
        private int skill0 = 240;

        public Skeletron_Surrand(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            skill0--;
            if (skill0 < 0)
            {
                int num = NPC.NewNPC(npc.GetSpawnSourceForNPCFromNPCAI(), (int)npc.Center.X, (int)npc.Center.Y, 33, 0, 0f, 0f, 0f, 0f, 255);
                Main.npc[num].lifeMax = 100;
                Main.npc[num].life = 101;
                skill0 = 180 + Main.rand.Next(-60, 61);
            }
        }

        public override void OnKilled()
        {
            NPCAimedTarget targetData = npc.GetTargetData(true);
            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.DirectionTo(npc.Center, targetData.Center) * 5f, 270, 6, 30f, -1, 0f, 0f, 0f);
        }
    }
}
