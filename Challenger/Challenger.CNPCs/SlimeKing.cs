using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace Challenger
{
    public class SlimeKing : CNPC
    {
        public readonly float CooldownOfSkill0 = 250f;

        public readonly float CooldownOfSkill1 = 250f;

        public readonly float CooldownOfSkill2 = 250f;

        public SlimeKing(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            NPCAimedTarget targetData = npc.GetTargetData(true);
            if (npc.ai[0] > -200f && npc.ai[0] < -120f)
            {
                Point val = Terraria.Utils.ToTileCoordinates(npc.Bottom);
                int num = 20;
                int num2 = 1;
                val.X += npc.direction * 3;
                int num3 = (int)(npc.ai[0] / 2f + 101f);
                int num4 = 4;
                int num5 = num3 / num4 * num4;
                int num6 = num5 + num4;
                if (num3 % num4 != 0)
                {
                    num6 = num5;
                }
                for (int i = num5; i < num6 && i < num; i++)
                {
                    int num7 = i * num2;
                    npc.AI_123_Deerclops_TryMakingSpike(ref val, npc.direction, num, i, num7);
                    npc.AI_123_Deerclops_TryMakingSpike(ref val, -npc.direction, num, i, num7);
                }
            }
            SetState();
            switch (state)
            {
                case 1:
                    ai[0] -= 1f;

                    if (ai[0] < 0f && npc.ai[0] != -120f && npc.ai[0] != -200f)
                    {
                        Vector2 val3 = new Vector2();
                        float directionX = (targetData.Position.X - npc.Center.X > 0f) ? 1f : -1f;
                        val3.X = directionX;
                        val3.Y = -2f;

                        Entity entity = npc;
                        entity.velocity.X += val3.X * Main.rand.Next(5, 12);
                        entity.velocity.Y += val3.Y * Main.rand.Next(1, 4);

                        ai[0] = CooldownOfSkill0 + Main.rand.Next(121);
                        npc.netUpdate = true;
                    }
                    break;
                case 2:
                    ai[0] -= 1f;
                    ai[1] -= 1f;

                    if (ai[0] < 0f && npc.ai[0] != -120f && npc.ai[0] != -200f)
                    {
                        Vector2 val5 = new Vector2();
                        float directionX = (targetData.Position.X - npc.Center.X > 0f) ? 1f : -1f;
                        val5.X = directionX;
                        val5.Y = -2f;

                        Entity entity = npc;
                        entity.velocity += val5 * Main.rand.Next(3, 7);

                        ai[0] = CooldownOfSkill0 + Main.rand.Next(81);
                        npc.netUpdate = true;
                    }

                    if (ai[1] < 0f)
                    {
                        Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 464, 16, 32f, -1, 0f, 0f, 0f);
                        ai[1] = CooldownOfSkill1 + Main.rand.Next(151);
                    }
                    break;
                case 3:
                    {
                        ai[1] -= 1f;
                        ai[2] -= 1f;
                        Vector2 val4 = npc.DirectionTo(targetData.Position);
                        ((Vector2)(val4)).Normalize();
                        val4 *= 2.5f;
                        if (ai[1] < 0f)
                        {
                            Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 464, 16, 64f, -1, 0f, 0f, 0f);
                            ai[1] = CooldownOfSkill1 + Main.rand.Next(121);
                        }
                        if (ai[2] < 120f && ai[2] % 40f == 0f)
                        {
                            Projectile.NewProjectile(null, npc.Center, val4, 348, 18, 64f, -1, 0f, 0f, 0f);
                        }
                        if (ai[2] < 0f)
                        {
                            ai[2] = CooldownOfSkill2 + 300f;
                        }
                        break;
                    }
                case 4:
                    {
                        ai[1] -= 1f;
                        ai[2] -= 1f;
                        if (ai[1] < 0f && ai[2] == 500f)
                        {
                            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.One, Main.rand.NextDouble() * Math.PI, default(Vector2)), 464, 8, 64f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.One, Main.rand.NextDouble() * Math.PI, default(Vector2)), 464, 8, 64f, -1, 0f, 0f, 0f);
                            ai[1] = CooldownOfSkill1 + Main.rand.Next(151);
                        }
                        Vector2 val2 = npc.DirectionTo(targetData.Position);
                        ((Vector2)(val2)).Normalize();
                        val2 *= 2.25f;
                        if (ai[2] < 180f && ai[2] % 60f == 0f)
                        {
                            if (Main.rand.Next(1, 3) == 2)
                            {
                                Projectile.NewProjectile(null, npc.Center, val2, 348, 16, 64f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, 0.45, default(Vector2)), 348, 14, 64f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, -0.45, default(Vector2)), 348, 14, 64f, -1, 0f, 0f, 0f);
                            }
                            else
                            {
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, 0.4, default(Vector2)), 348, 14, 64f, -1, 0f, 0f, 0f);
                                Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val2, -0.4, default(Vector2)), 348, 14, 64f, -1, 0f, 0f, 0f);
                            }
                        }
                        if (ai[2] < 0f)
                        {
                            ai[2] = CooldownOfSkill2 + 200f;
                        }
                        break;
                    }
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
                        TSPlayer.All.SendMessage("史莱姆王习得冰魔法归来", new Color(0, 146, 255));
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
                        TSPlayer.All.SendMessage("寒风呼啸", new Color(0, 146, 255));
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
                        TSPlayer.All.SendMessage("你感觉寒冷刺骨", new Color(0, 146, 255));
                    }
                }
                return state;
            }
            if (state == 3)
            {
                state = 4;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("史莱姆王发怒了", new Color(0, 146, 255));
                }
            }
            return state;
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                switch (Main.rand.Next(1, 4))
                {
                    case 1:
                        global::Challenger.Challenger.SendPlayerText("走位真菜", new Color(0, 146, 255), npc.Center + new Vector2(0f, -30f));
                        break;
                    case 2:
                        global::Challenger.Challenger.SendPlayerText("连我都打不过，回家喝奶吧你", new Color(0, 146, 255), npc.Center + new Vector2(0f, -30f));
                        break;
                    default:
                        global::Challenger.Challenger.SendPlayerText("小辣鸡", new Color(0, 146, 255), npc.Center + new Vector2(0f, -30f));
                        break;
                }
            }
        }
    }
}
