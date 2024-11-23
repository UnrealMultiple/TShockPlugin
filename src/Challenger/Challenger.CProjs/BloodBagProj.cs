using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class BloodBagProj : CProjectile
{
    public float v;

    private BloodBagProj(Projectile projectile, float[] ai, int lable)
        : base(projectile, ai, lable)
    {
    }

    public override void ProjectileAI()
    {
        NPC? val = null;
        if (this.ai[3] != -1f)
        {
            val = Main.npc[(int) this.ai[3]];
        }
        if (NPC.downedAncientCultist)
        {
            this.v = 20f;
        }
        else if (NPC.downedFishron || NPC.downedEmpressOfLight)
        {
            this.v = 18f;
        }
        else if (NPC.downedGolemBoss)
        {
            this.v = 15f;
        }
        else if (NPC.downedPlantBoss)
        {
            this.v = 13f;
        }
        else if (NPC.downedMechBossAny)
        {
            this.v = 11f;
        }
        else if (Main.hardMode)
        {
            this.v = 8f;
        }
        else if (NPC.downedBoss3)
        {
            this.v = 6f;
        }
        else
        {
            this.v = NPC.downedBoss2 ? 4.5f : NPC.downedBoss1 ? 3.5f : 2f;
        }
        if (val != null && (val.type == 134 || val.type == 135 || val.type == 136))
        {
            this.v = 25f;
        }
        if ((int) this.ai[3] != -1 && val != null && val.active)
        {
            var obj = this.proj;
            obj.Center += (val.Center - this.proj.Center).SafeNormalize(Vector2.Zero) * this.v;
        }
        else
        {
            val = Challenger.NearestWeakestNPC(this.proj.position, 4000000f);
            if (val != null)
            {
                var obj2 = this.proj;
                obj2.Center += (val.Center - this.proj.Center).SafeNormalize(Vector2.Zero) * this.v;
                this.ai[3] = val.whoAmI;
            }
        }
        Vector2 val2;
        if (val != null && val.active && this.proj.active)
        {
            val2 = this.proj.position - val.Center;
            if (((Vector2) val2).LengthSquared() <= val.width * val.height / 2)
            {
                var obj3 = val;

                // 直接从配置中读取回血比例上限
                var bloodMax = Challenger.config.BloodAbsorptionRatio_Max;

                // 计算允许回血的上限值
                var maxHeal = (int) (val.lifeMax * bloodMax);
                var healAmount = (int) this.ai[4];
                if (val.life + healAmount > maxHeal)
                {
                    healAmount = maxHeal - val.life; // 调整回血量不超过允许的上限
                }
                obj3.life += healAmount;
                if (Challenger.config.EnableConsumptionMode)
                {
                    Challenger.SendPlayerText(GetString($"敌怪治疗 + {(int) this.ai[4]}"), new Color(190, 255, 0), val.Center);
                }
                else
                {
                    val.HealEffect((int) this.ai[4], true);
                }
                this.CKill();
                TSPlayer.All.SendData((PacketTypes) 23, null, val.whoAmI, 0f, 0f, 0f, 0);
                return;
            }
        }
        if (this.proj.active && this.proj.timeLeft <= this.ai[1] - 60f)
        {
            var player = Main.player;
            foreach (var val3 in player)
            {
                if (val3 == null || !val3.active)
                {
                    continue;
                }
                val2 = this.proj.Center - val3.Center;
                if (((Vector2) val2).LengthSquared() <= val3.width * val3.height / 2 && !val3.dead)
                {
                    if (Challenger.config.EnableConsumptionMode)
                    {
                        Challenger.HealPlayer(Main.player[val3.whoAmI], (int) this.ai[0], visible: false);
                        Challenger.SendPlayerText(GetString($"血包治疗 + {(int) this.ai[0]}"), new Color(0, 255, 0), val3.Center);
                    }
                    else
                    {
                        Challenger.HealPlayer(Main.player[val3.whoAmI], (int) this.ai[0]);
                    }
                    this.CKill();
                    return;
                }
            }
        }
        this.Update();
    }

    public static BloodBagProj NewCProjectile(Vector2 position, Vector2 velocity, int owner, int lable, float[] ai)
    {
        var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 125, 0, 0f, owner);
        var bloodBagProj = new BloodBagProj(Main.projectile[num], ai, lable);
        bloodBagProj.ai[1] = bloodBagProj.proj.timeLeft;
        Collect.cprojs[num] = bloodBagProj;
        Update(num);
        return bloodBagProj;
    }
}