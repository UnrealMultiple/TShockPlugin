using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger
{

    public class QueenBee : CNPC
    {
        private int timer = 0;

        public QueenBee(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            NPCAimedTarget targetData = npc.GetTargetData(true);
            SetState();
            switch (state)
            {
                case 1:
                    if (npc.ai[0] == 0f && (npc.ai[1] == 1f || npc.ai[1] == 3f || npc.ai[1] == 5f) && npc.ai[2] == 0f && timer < 1)
                    {
                        timer++;
                        if ((npc.direction == 1 && targetData.Position.X > npc.position.X) || (npc.direction == -1 && targetData.Position.X < npc.position.X))
                        {
                            NPC? obj5 = npc;
                            obj5.velocity = obj5.velocity + npc.velocity * 0.04f;
                        }
                        else
                        {
                            NPC? obj6 = npc;
                            obj6.velocity = obj6.velocity - npc.velocity * 0.04f;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        timer = 0;
                    }
                    break;
                case 2:
                    if (npc.ai[0] == 0f && (npc.ai[1] == 1f || npc.ai[1] == 3f || npc.ai[1] == 5f) && npc.ai[2] == 0f && timer < 1)
                    {
                        timer++;
                        if ((npc.direction == 1 && targetData.Position.X > npc.position.X) || (npc.direction == -1 && targetData.Position.X < npc.position.X))
                        {
                            NPC? obj7 = npc;
                            obj7.velocity = obj7.velocity + npc.velocity * 0.07f;
                        }
                        else
                        {
                            NPC? obj8 = npc;
                            obj8.velocity = obj8.velocity - npc.velocity * 0.07f;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        timer = 0;
                    }
                    break;
                case 3:
                    if (npc.ai[0] == 0f && (npc.ai[1] == 1f || npc.ai[1] == 3f || npc.ai[1] == 5f) && npc.ai[2] == 0f && timer < 1)
                    {
                        timer++;
                        if ((npc.direction == 1 && targetData.Position.X > npc.position.X) || (npc.direction == -1 && targetData.Position.X < npc.position.X))
                        {
                            NPC? obj3 = npc;
                            obj3.velocity = obj3.velocity + npc.velocity * 0.12f;
                        }
                        else
                        {
                            NPC? obj4 = npc;
                            obj4.velocity = obj4.velocity - npc.velocity * 0.12f;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        timer = 0;
                    }
                    if (Main.rand.Next(6) == 0)
                    {
                        Projectile.NewProjectile(null, npc.Bottom, Terraria.Utils.RotateRandom(Vector2.UnitY, Math.PI / 2.0) * -8f, 719, 12, 1f, -1, 0f, 0f, 0f);
                    }
                    break;
                case 4:
                    if (npc.ai[0] == 0f && (npc.ai[1] == 1f || npc.ai[1] == 3f || npc.ai[1] == 5f) && npc.ai[2] == 0f && timer < 1)
                    {
                        timer++;
                        if ((npc.direction == 1 && targetData.Position.X > npc.position.X) || (npc.direction == -1 && targetData.Position.X < npc.position.X))
                        {
                            NPC? obj = npc;
                            obj.velocity = obj.velocity + npc.velocity * 0.12f;
                        }
                        else
                        {
                            NPC? obj2 = npc;
                            obj2.velocity = obj2.velocity - npc.velocity * 0.12f;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        timer = 0;
                    }
                    if (npc.ai[1] % 12f == 0f)
                    {
                        Projectile.NewProjectile(null, npc.position - new Vector2(Main.rand.Next(-1024, 1024), 384f), Vector2.UnitY * -3f, 719, 20, 1f, -1, 0f, 0f, 0f);
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
                        TSPlayer.All.SendMessage("谁人惊扰了我的蜂巢！", Color.Yellow);
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
                        TSPlayer.All.SendMessage("不许抢我的蜂蜜", Color.Yellow);
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
                        TSPlayer.All.SendMessage("毒刺射你一脸", Color.Yellow);
                    }
                }
                return state;
            }
            if (state == 3)
            {
                state = 4;
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
                        global::Challenger.Challenger.SendPlayerText("嗡嗡", Color.Yellow, npc.Center + new Vector2(0f, -30f));
                        break;
                    case 2:
                        global::Challenger.Challenger.SendPlayerText("嗡嗡嗡嗡", Color.Yellow, npc.Center + new Vector2(0f, -30f));
                        break;
                    default:
                        global::Challenger.Challenger.SendPlayerText("吱嗡", Color.Yellow, npc.Center + new Vector2(0f, -30f));
                        break;
                }
            }
        }
    }
}
