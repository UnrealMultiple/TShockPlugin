using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger
{
    public class EyeofCthulhu : CNPC
    {
        public const float CooldownOfSkill0 = 150f;

        public const float CooldownOfSkill1 = 150f;

        public const float CooldownOfSkill2 = 500f;

        public const int ProjectileSpeedOfState3 = 5;

        public float skill0 = 150f;

        public float skill1 = 150f;

        public float skill2 = 500f;

        public EyeofCthulhu(NPC npc)
            : base(npc)
        {
        }

        private void Spawn(int number)
        {
            int num = NPC.CountNPCS(5);
            if (num >= 20)
            {
                return;
            }
            if (20 - num > number)
            {
                for (int i = 0; i < number; i++)
                {
                    NPC.NewNPC(null, (int)npc.Bottom.X + Main.rand.Next(-32, 33), (int)npc.Bottom.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
                }
            }
            else
            {
                for (int j = 0; j < 20 - num; j++)
                {
                    NPC.NewNPC(null, (int)npc.Bottom.X + Main.rand.Next(-32, 33), (int)npc.Bottom.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
                }
            }
        }

        public override void NPCAI()
        {
            NPCAimedTarget targetData = npc.GetTargetData(true);
            if (npc.ai[0] == 1f && npc.ai[1] % 2f == 0f)
            {
                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.ToRotationVector2(npc.rotation) * 3f, 96, 8, 5f, -1, 0f, 0f, 0f);
            }
            SetState();
            switch (state)
            {
                case 1:
                    skill0 -= 1f;
                    if (skill0 < 0f && npc.ai[1] == 0f)
                    {
                        Vector2 val4 = npc.DirectionTo(targetData.Position);
                        if (npc.ai[2] == 40f)
                        {
                            Projectile.NewProjectile(null, npc.Bottom, val4 * 8f, 96, 4, 5f, -1, 0f, 0f, 0f);
                        }
                        else if (npc.ai[2] == 80f)
                        {
                            Projectile.NewProjectile(null, npc.Bottom, val4 * 10f, 96, 5, 5f, -1, 0f, 0f, 0f);
                        }
                        else if (npc.ai[2] == 100f)
                        {
                            Projectile.NewProjectile(null, npc.Bottom, val4 * 10f, 96, 6, 6f, -1, 0f, 0f, 0f);
                            skill0 = 150f + Main.rand.Next(100);
                        }
                    }
                    if (npc.ai[1] == 2f && (npc.ai[3] == 0f || npc.ai[3] == 1f || npc.ai[3] == 2f) && npc.ai[2] <= 2f)
                    {
                        NPC? obj3 = npc;
                        obj3.velocity = obj3.velocity + npc.velocity * 0.1f;
                        npc.netUpdate = true;
                    }
                    break;
                case 2:
                    skill0 -= 1f;
                    if (skill0 < 0f && npc.ai[1] == 0f)
                    {
                        Vector2 val2 = npc.DirectionTo(targetData.Position);
                        if (npc.ai[2] == 120f)
                        {
                            if (Main.rand.Next(1, 3) == 1)
                            {
                                Projectile.NewProjectile(null, npc.Center, val2 * 12f, 96, 8, 5f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, 0.1, default(Vector2)) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, -0.1, default(Vector2)) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                            }
                            else
                            {
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, 0.1, default(Vector2)) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, -0.1, default(Vector2)) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                            }
                            skill0 = 150f;
                            npc.ai[2] = 100f;
                            Spawn(2);
                        }
                    }
                    if (npc.ai[1] == 2f && (npc.ai[3] == 0f || npc.ai[3] == 1f || npc.ai[3] == 2f) && npc.ai[2] == 0f)
                    {
                        NPC? obj2 = npc;
                        obj2.velocity = obj2.velocity + npc.velocity;
                        Vector2 val3 = Terraria.Utils.SafeNormalize(npc.velocity, Vector2.Zero);
                        npc.netUpdate = true;
                        Projectile.NewProjectile(null, npc.Center + val3 * 2f, val3 * 18f, 96, 5, 5f, -1, 0f, 0f, 0f);
                    }
                    break;
                case 3:
                    skill1 -= 1f;
                    if (skill1 < 0f)
                    {
                        Spawn(3);
                        skill1 = 150f + Main.rand.Next(51);
                    }
                    if (npc.ai[1] == 4f && npc.ai[2] % 15f == 0f)
                    {
                        int num2 = Collect.MyNewProjectile(null, npc.Center, Terraria.Utils.RotateRandom(Vector2.One, 6.2831854820251465) * 0.5f, 96, 6, 5f);
                        Main.projectile[num2].timeLeft = 240;
                        CProjectile.Update(num2);
                    }
                    if (npc.ai[1] == 4f && (npc.ai[3] == 0f || npc.ai[3] == 1f || npc.ai[3] == 2f) && npc.ai[2] == 0f)
                    {
                        NPC? obj4 = npc;
                        obj4.velocity = obj4.velocity + npc.velocity;
                        Vector2 val5 = Terraria.Utils.SafeNormalize(npc.velocity, Vector2.Zero);
                        npc.netUpdate = true;
                        Projectile.NewProjectile(null, npc.Center + val5 * 2f, val5 * 20f, 96, 5, 5f, -1, 0f, 0f, 0f);
                    }
                    break;
                case 4:
                    skill1 -= 1f;
                    skill2 -= 1f;
                    if (skill1 < 0f)
                    {
                        Spawn(3);
                        skill1 = 250f + Main.rand.Next(150);
                    }
                    if (skill2 < 0f && npc.ai[1] == 4f && npc.ai[3] == 4f)
                    {
                        npc.ai[3] = 0f;
                        skill2 = 500f;
                    }
                    if (npc.ai[1] == 4f && npc.ai[2] % 10f == 0f)
                    {
                        int num = Collect.MyNewProjectile(null, npc.Center, Terraria.Utils.RotateRandom(Vector2.One, 6.2831854820251465) * 0.5f, 96, 9, 5f);
                        Main.projectile[num].timeLeft = 600;
                        CProjectile.Update(num);
                    }
                    if (npc.ai[1] == 4f && (npc.ai[3] == 0f || npc.ai[3] == 1f || npc.ai[3] == 2f) && npc.ai[2] == 0f)
                    {
                        NPC? obj = npc;
                        obj.velocity = obj.velocity + npc.velocity;
                        Vector2 val = Terraria.Utils.SafeNormalize(npc.velocity, Vector2.Zero);
                        npc.netUpdate = true;
                        Projectile.NewProjectile(null, npc.Center + val * 2f, val * 30f, 96, 6, 5f, -1, 0f, 0f, 0f);
                    }
                    break;
            }
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
                        TSPlayer.All.SendMessage("燃烧！无法熄灭的火焰", new Color(200, 200, 200));
                    }
                }
                return 0;
            }
            if (npc.life >= LifeMax * 0.4f)
            {
                if (state == 1)
                {
                    state = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("你找到那颗子弹了吗", new Color(200, 200, 200));
                    }
                }
                return 1;
            }
            if (npc.life >= LifeMax * 0.2f)
            {
                if (state == 2)
                {
                    state = 3;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("猪突猛进！", new Color(200, 200, 200));
                    }
                }
                return 2;
            }
            if (state == 3)
            {
                state = 4;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("疯狗狂叫！！！", new Color(200, 200, 200));
                }
            }
            return 3;
        }

        public override void OnHurtPlayers(PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                int num = Main.rand.Next(1, 3);
                if (num == 1)
                {
                    global::Challenger.Challenger.SendPlayerText("就这就这！", new Color(200, 200, 200), npc.Center + new Vector2(0f, -30f));
                }
                else
                {
                    global::Challenger.Challenger.SendPlayerText("看我创死你", new Color(200, 200, 200), npc.Center + new Vector2(0f, -30f));
                }
            }
        }
    }

}
