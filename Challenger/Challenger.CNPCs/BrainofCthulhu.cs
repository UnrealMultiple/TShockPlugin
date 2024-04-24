using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace Challenger
{

    internal class BrainofCthulhu : CNPC
    {
        public BrainofCthulhu(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            SetState();
            NPCAimedTarget targetData = npc.GetTargetData(true);
            if (npc.ai[0] >= 0f || npc.ai[3] != 225f)
            {
                return;
            }
            switch (state)
            {
                case 1:
                    break;
                case 2:
                    {
                        int num4 = Main.rand.Next(2, 5);
                        for (int j = 0; j < num4; j++)
                        {
                            float num5 = (float)Main.rand.NextDouble() - 0.5f;
                            float num6 = -0.25f * (float)Math.Cos(3.1415927410125732 * (double)num5);
                            Projectile.NewProjectile(null, targetData.Center * 2f - npc.Center, new Vector2(0f - num5, 0f - num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, new Vector2((targetData.Center - npc.Center).X * 2f, 0f) + npc.Center, new Vector2(0f - num5, num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, new Vector2(0f, (targetData.Center - npc.Center).Y * 2f) + npc.Center, new Vector2(num5, 0f - num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, npc.Center, new Vector2(num5, num6) * 17f, 811, 15, 5f, -1, 0f, 0f, 0f);
                        }
                        break;
                    }
                case 3:
                    {
                        int num = Main.rand.Next(4, 7);
                        for (int i = 0; i < num; i++)
                        {
                            float num2 = (float)Main.rand.NextDouble() - 0.5f;
                            float num3 = -0.25f * (float)Math.Cos(3.1415927410125732 * (double)num2);
                            Projectile.NewProjectile(null, targetData.Center * 2f - npc.Center, new Vector2(0f - num2, 0f - num3) * 17f, 811, 14, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, new Vector2((targetData.Center - npc.Center).X * 2f, 0f) + npc.Center, new Vector2(0f - num2, num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, new Vector2(0f, (targetData.Center - npc.Center).Y * 2f) + npc.Center, new Vector2(num2, 0f - num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, npc.Center, new Vector2(num2, num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                        }
                        break;
                    }
            }
        }

        public override int SetState()
        {
            if (npc.life >= LifeMax * 0.98f)
            {
                if (state == 0)
                {
                    state = 1;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("畸形怪脑发现了新鲜的脑子", new Color(255, 94, 94));
                    }
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.5f)
            {
                if (state == 1)
                {
                    state = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("你真的能分清真假血弹吗", new Color(255, 94, 94));
                    }
                }
                return state;
            }
            if (state == 2)
            {
                state = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("虚虚实实，实实虚虚", new Color(255, 94, 94));
                }
            }
            return state;
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                int num = Main.rand.Next(1, 3);
                if (num == 1)
                {
                    global::Challenger.Challenger.SendPlayerText("糊你一脸", Color.Red, npc.Center + new Vector2(0f, -30f));
                }
                else
                {
                    global::Challenger.Challenger.SendPlayerText("哇哇嗷", Color.Red, npc.Center + new Vector2(0f, -30f));
                }
            }
        }
    }
}
