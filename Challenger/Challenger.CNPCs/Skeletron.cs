using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger
{
    public class Skeletron : CNPC
    {
        private float rotate = 0f;

        private List<int> surrandIndex = new List<int>();

        private const int MaxSurrandNum = 10;

        private int skill0 = 240;

        public Skeletron(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            skill0--;
            SetState();
            NPCAimedTarget targetData = npc.GetTargetData(true);
            switch (state)
            {
                case 1:
                    if (skill0 < 0)
                    {
                        Vector2 val = Terraria.Utils.DirectionTo(npc.Center, targetData.Center);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.5235987901687622, default(Vector2)), 270, 11, 5f, -1, 0f, 0f, 0f);
                        skill0 = 220 + Main.rand.Next(-60, 61);
                    }
                    break;
                case 2:
                    if (skill0 < 0)
                    {
                        for (int l = 0; l < 4; l++)
                        {
                            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 2.0 * l + rotate, default(Vector2)) * 4f, 270, 12, 5f, -1, 0f, 0f, 0f);
                        }
                        skill0 = 160 + Main.rand.Next(-60, 61);
                    }
                    break;
                case 3:
                    if (skill0 < 0)
                    {
                        int num3 = Main.rand.Next(4, 7);
                        for (int k = 0; k < num3; k++)
                        {
                            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI * 2.0 / 5.0 * k + rotate, default(Vector2)) * 3f, 270, 15, 5f, -1, 0f, 0f, 0f);
                        }
                        skill0 = 120 + Main.rand.Next(-60, 61);
                    }
                    if (npc.ai[1] == 1f && npc.ai[2] % 10f == 0f)
                    {
                        int num4 = Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 299, 15, 5f, -1, 0f, 0f, 0f);
                        Main.projectile[num4].timeLeft = 40;
                        CProjectile.Update(num4);
                    }
                    break;
                case 4:
                    if (skill0 < 0)
                    {
                        int num = Main.rand.Next(5, 10);
                        if (Main.rand.Next(2) == 0)
                        {
                            for (int i = 0; i < num; i++)
                            {
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 3.0 * i + rotate, default(Vector2)) * 3f, 270, 18, 30f, -1, 0f, 0f, 0f);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < num; j++)
                            {
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 4.0 * j + rotate, default(Vector2)) * 5f, 299, 18, 30f, -1, 0f, 0f, 0f);
                            }
                        }
                        skill0 = 100 + Main.rand.Next(-60, 31);
                    }
                    if (npc.ai[1] == 1f && npc.ai[2] % 5f == 0f)
                    {
                        int num2 = Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 299, 20, 5f, -1, 0f, 0f, 0f);
                        Main.projectile[num2].timeLeft = 180;
                        CProjectile.Update(num2);
                    }
                    break;
            }
            if (ai[0] < 10f && Main.rand.Next(180) == 0)
            {
                int num5 = NPC.NewNPC(npc.GetSpawnSourceForNPCFromNPCAI(), (int)npc.Center.X, (int)npc.Center.Y, 34, 0, 0f, 0f, 0f, 0f, 255);
                Main.npc[num5].lifeMax = 500;
                Main.npc[num5].life = 501;
                ai[0] += 1f;
                surrandIndex.Add(num5);
            }
            ai[0] -= surrandIndex.RemoveAll((int x) => Main.npc[x] == null || !Main.npc[x].active || Main.npc[x].netID != 34);
            rotate += 0.1f;
        }

        public override int SetState()
        {
            if (npc.life >= LifeMax * 0.7f)
            {
                if (state == 0)
                {
                    state = 1;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("被封印的骷髅帝王苏醒", new Color(150, 143, 102));
                    }
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.4f)
            {
                if (state == 1)
                {
                    state = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("嘎吱作响", new Color(150, 143, 102));
                    }
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.2f)
            {
                if (state == 2)
                {
                    state = 3;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("诅咒开始应验", new Color(150, 143, 102));
                    }
                }
                return state;
            }
            if (state == 3)
            {
                state = 4;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("惨朽不堪", new Color(150, 143, 102));
                }
            }
            return state;
        }

        public override void OnHurtPlayers(PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                switch (Main.rand.Next(1, 4))
                {
                    case 1:
                        global::Challenger.Challenger.SendPlayerText("再让我逮到一下你就玩玩", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                    case 2:
                        global::Challenger.Challenger.SendPlayerText("创死你", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                    default:
                        global::Challenger.Challenger.SendPlayerText("想再贴贴吗？", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                }
            }
        }

        public override void OnKilled()
        {
            for (int i = 0; i < 35; i++)
            {
                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI * 2.0 / 35.0 * i, default(Vector2)) * 5f, 299, 21, 10f, -1, 0f, 0f, 0f);
            }
        }
    }
}
