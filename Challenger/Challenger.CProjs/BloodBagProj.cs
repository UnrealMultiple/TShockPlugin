using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class BloodBagProj : CProjectile
    {
        public float v;

        private BloodBagProj(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void ProjectileAI()
        {
            NPC val = null;
            if (ai[3] != -1f)
            {
                val = Main.npc[(int)ai[3]];
            }
            if (NPC.downedAncientCultist)
            {
                v = 20f;
            }
            else if (NPC.downedFishron || NPC.downedEmpressOfLight)
            {
                v = 18f;
            }
            else if (NPC.downedGolemBoss)
            {
                v = 15f;
            }
            else if (NPC.downedPlantBoss)
            {
                v = 13f;
            }
            else if (NPC.downedMechBossAny)
            {
                v = 11f;
            }
            else if (Main.hardMode)
            {
                v = 8f;
            }
            else if (NPC.downedBoss3)
            {
                v = 6f;
            }
            else if (NPC.downedBoss2)
            {
                v = 4.5f;
            }
            else if (NPC.downedBoss1)
            {
                v = 3.5f;
            }
            else
            {
                v = 2f;
            }
            if (val != null && (val.type == 134 || val.type == 135 || val.type == 136))
            {
                v = 25f;
            }
            if ((int)ai[3] != -1 && val != null && val.active)
            {
                Projectile? obj = proj;
                obj.Center = obj.Center + (val.Center - proj.Center).SafeNormalize(Vector2.Zero) * v;
            }
            else
            {
                val = global::Challenger.Challenger.NearestWeakestNPC(proj.position, 4000000f);
                if (val != null)
                {
                    Projectile? obj2 = proj;
                    obj2.Center = obj2.Center + (val.Center - proj.Center).SafeNormalize(Vector2.Zero) * v;
                    ai[3] = val.whoAmI;
                }
            }
            Vector2 val2;
            if (val != null && val.active && proj.active)
            {
                val2 = proj.position - val.Center;
                if (((Vector2)val2).LengthSquared() <= val.width * val.height / 2)
                {
                    NPC obj3 = val;
                    obj3.life += (int)ai[4];
                    if (val.lifeMax < val.life)
                    {
                        val.lifeMax = val.life + 1;
                    }
                    if (global::Challenger.Challenger.config.EnableConsumptionMode)
                    {
                        global::Challenger.Challenger.SendPlayerText($"敌怪治疗 + {(int)ai[4]}", new Color(190, 255, 0), val.Center);
                    }
                    else
                    {
                        val.HealEffect((int)ai[4], true);
                    }
                    CKill();
                    TSPlayer.All.SendData((PacketTypes)23, null, val.whoAmI, 0f, 0f, 0f, 0);
                    return;
                }
            }
            if (proj.active && proj.timeLeft <= ai[1] - 60f)
            {
                Player[] player = Main.player;
                foreach (Player val3 in player)
                {
                    if (val3 == null || !val3.active)
                    {
                        continue;
                    }
                    val2 = proj.Center - val3.Center;
                    if (((Vector2)val2).LengthSquared() <= val3.width * val3.height / 2 && !val3.dead)
                    {
                        if (global::Challenger.Challenger.config.EnableConsumptionMode)
                        {
                            global::Challenger.Challenger.HealPlayer(Main.player[val3.whoAmI], (int)ai[0], visible: false);
                            global::Challenger.Challenger.SendPlayerText($"血包治疗 + {(int)ai[0]}", new Color(0, 255, 0), val3.Center);
                        }
                        else
                        {
                            global::Challenger.Challenger.HealPlayer(Main.player[val3.whoAmI], (int)ai[0]);
                        }
                        CKill();
                        return;
                    }
                }
            }
            Update();
        }

        public static BloodBagProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, int lable, float[] ai)
        {
            int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 125, 0, 0f, owner);
            BloodBagProj bloodBagProj = new BloodBagProj(Main.projectile[num], ai, lable);
            bloodBagProj.ai[1] = bloodBagProj.proj.timeLeft;
            Collect.cprojs[num] = bloodBagProj;
            Update(num);
            return bloodBagProj;
        }
    }
}
