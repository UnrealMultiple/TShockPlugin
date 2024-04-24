using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class SkeletronHand : CNPC
    {
        private int timer = 0;

        public SkeletronHand(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            SetState();
            switch (state - 1)
            {
                case 0:
                    if (npc.ai[2] == 2f || npc.ai[2] == 5f)
                    {
                        int num2 = Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 299, 10, 5f, -1, 0f, 0f, 0f);
                        Main.projectile[num2].timeLeft = 30;
                        CProjectile.Update(num2);
                    }
                    break;
                case 1:
                    if (npc.ai[2] == 2f || npc.ai[2] == 5f)
                    {
                        timer++;
                        if (timer % 5 == 0)
                        {
                            int num = Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 299, 10, 5f, -1, 0f, 0f, 0f);
                            Main.projectile[num].timeLeft = 2400;
                            CProjectile.Update(num);
                        }
                    }
                    else
                    {
                        timer = 0;
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
                }
                return state;
            }
            if (state == 1)
            {
                state = 2;
                if (npc.ai[0] == -1f && global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("你打痛我左手了！！！", new Color(150, 143, 102));
                }
                if (npc.ai[0] == 1f && global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("你打痛我右手了！！！", new Color(150, 143, 102));
                }
            }
            return state;
        }

        public override void OnKilled()
        {
            if (npc.ai[0] == -1f)
            {
                double num = Main.rand.NextDouble() * 3.0;
                for (int i = 0; i < 10; i++)
                {
                    Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 5.0 * i + num, default(Vector2)) * 5f, 270, 20, 30f, -1, 0f, 0f, 0f);
                }
            }
            else
            {
                double num2 = Main.rand.NextDouble() * 3.0;
                for (int j = 0; j < 10; j++)
                {
                    Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 5.0 * j + num2, default(Vector2)) * 5f, 299, 20, 30f, -1, 0f, 0f, 0f);
                }
            }
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                switch (Main.rand.Next(1, 4))
                {
                    case 1:
                        global::Challenger.Challenger.SendPlayerText("就这还想打倒我骷髅王爷爷", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                    case 2:
                        global::Challenger.Challenger.SendPlayerText("看我一记耳光", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                    default:
                        global::Challenger.Challenger.SendPlayerText("离地牢远点！！！", new Color(150, 143, 102), npc.Center + new Vector2(0f, -30f));
                        break;
                }
            }
            e.Player.SetBuff(23, 60, false);
        }
    }
}
