using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger
{
    public class Deerclops : CNPC
    {
        private const float CooldownOfSkill0 = 350f;

        private float skill0 = 0f;

        public Deerclops(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            skill0 += 1f;
            NPCAimedTarget targetData = npc.GetTargetData(true);
            if (npc.ai[0] == 6f)
            {
                NPC? obj = npc;
                obj.life += 2;
                npc.StrikeNPC(0, 0f, 0, false, false, false, null);
            }
            SetState();
            switch (state)
            {
                case 1:
                    if (npc.ai[0] == 5f && npc.ai[1] == 59f && Main.netMode != 1)
                    {
                        Vector2 val7 = default(Vector2);
                        Vector2 val8 = default(Vector2);
                        float num5 = default(float);
                        float num6 = default(float);
                        for (int m = 0; m < 3; m++)
                        {
                            Projectile.RandomizeInsanityShadowFor((Entity)(object)Main.player[npc.target], true, out val7, out val8, out num5, out num6);
                            Projectile.NewProjectile(null, val7, val8, 965, 12, 0f, Main.myPlayer, num5, num6, 0f);
                        }
                    }
                    break;
                case 2:
                    if (npc.ai[0] == 1f && npc.ai[1] == 30f)
                    {
                        Point val4 = Terraria.Utils.ToTileCoordinates(npc.Top);
                        for (int k = 5; k < 20; k++)
                        {
                            npc.AI_123_Deerclops_ShootRubbleUp(ref targetData, ref val4, 20, 1, 200f, k);
                        }
                        if (Main.rand.Next(1) == 0 && npc.ai[1] == 79f)
                        {
                            npc.ai[0] = 5f;
                            npc.ai[1] = 0f;
                        }
                    }
                    else if (npc.ai[0] == 5f && npc.ai[1] == 59f && Main.netMode != 1)
                    {
                        Vector2 val5 = default(Vector2);
                        Vector2 val6 = default(Vector2);
                        float num3 = default(float);
                        float num4 = default(float);
                        for (int l = 0; l < 8; l++)
                        {
                            Projectile.RandomizeInsanityShadowFor((Entity)(object)Main.player[npc.target], true, out val5, out val6, out num3, out num4);
                            Projectile.NewProjectile(null, val5, val6, 965, 12, 0f, Main.myPlayer, num3, num4, 0f);
                        }
                    }
                    if (skill0 >= 7f)
                    {
                        Projectile.NewProjectile(null, targetData.Position + new Vector2(Main.rand.Next(-1536, 1536), -768f), Vector2.UnitY, 174, 5, 0f, -1, 0f, 0f, 0f);
                        skill0 = 0f;
                    }
                    break;
                case 3:
                    if (npc.ai[0] == 1f && npc.ai[1] == 30f)
                    {
                        Point val = Terraria.Utils.ToTileCoordinates(npc.Top);
                        for (int i = 5; i < 20; i++)
                        {
                            npc.AI_123_Deerclops_ShootRubbleUp(ref targetData, ref val, 20, 1, 200f, i);
                        }
                        if (Main.rand.Next(1) == 0 && npc.ai[1] == 79f)
                        {
                            npc.ai[0] = 5f;
                            npc.ai[1] = 0f;
                        }
                    }
                    else if (npc.ai[0] == 5f && npc.ai[1] == 59f && Main.netMode != 1)
                    {
                        Vector2 val2 = default(Vector2);
                        Vector2 val3 = default(Vector2);
                        float num = default(float);
                        float num2 = default(float);
                        for (int j = 0; j < 8; j++)
                        {
                            Projectile.RandomizeInsanityShadowFor((Entity)(object)Main.player[npc.target], true, out val2, out val3, out num, out num2);
                            Projectile.NewProjectile(null, val2, val3, 965, 13, 0f, Main.myPlayer, num, num2, 0f);
                        }
                    }
                    if (skill0 >= 3f)
                    {
                        Projectile.NewProjectile(null, targetData.Position + new Vector2(Main.rand.Next(-1024, 1024), -1024f), Vector2.UnitY * 3f, 174, 9, 5f, -1, 0f, 0f, 0f);
                        skill0 = 0f;
                    }
                    break;
            }
        }

        public override int SetState()
        {
            if (npc.life >= LifeMax * 0.8f)
            {
                if (state == 0)
                {
                    state = 1;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("远方的巨兽将会摧毁你所拥有的一切", new Color(111, 160, 213));
                    }
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.3f)
            {
                if (state == 1)
                {
                    state = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("冰雪从天而降", new Color(111, 160, 213));
                    }
                }
                return state;
            }
            if (state == 2)
            {
                state = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("你将受到灭顶之灾", new Color(111, 160, 213));
                }
            }
            return state;
        }

        public override void OnHurtPlayers(PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                int num = Main.rand.Next(1, 3);
                if (num == 1)
                {
                    global::Challenger.Challenger.SendPlayerText("拆掉拆掉！", new Color(111, 160, 213), npc.Center + new Vector2(0f, -30f));
                }
                else
                {
                    global::Challenger.Challenger.SendPlayerText("嗷嗷", new Color(111, 160, 213), npc.Center + new Vector2(0f, -30f));
                }
            }
        }
    }
}
