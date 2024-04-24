using Challenger;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace ChalleAnger
{
    public class EaterofWorldsBody : CNPC
    {
        public static int State;

        public EaterofWorldsBody(NPC npc)
            : base(npc)
        {
        }

        public override void NPCAI()
        {
            SetState();
            NPCAimedTarget targetData = npc.GetTargetData(true);
            switch (State)
            {
                case 1:
                    if (Vector2.DistanceSquared(targetData.Center, npc.Center) <= 250000f && Main.rand.Next(1200) == 0)
                    {
                        Projectile.NewProjectile(null, npc.Center, -Vector2.UnitY * 6f, 671, 8, 0f, -1, 0f, 0f, 0f);
                    }
                    break;
                case 2:
                    if (Vector2.DistanceSquared(targetData.Center, npc.Center) <= 490000f && Main.rand.Next(800) == 0)
                    {
                        Projectile.NewProjectile(null, npc.Center, -Vector2.UnitY * 12f, 671, 14, 0f, -1, 0f, 0f, 0f);
                    }
                    break;
                case 3:
                    if (Vector2.DistanceSquared(targetData.Center, npc.Center) <= 640000f && Main.rand.Next(300) == 0)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedByRandom(Terraria.Utils.DirectionTo(npc.Center, targetData.Center), 0.1) * 12f, 671, 17, 0f, -1, 0f, 0f, 0f);
                        }
                        else
                        {
                            Projectile.NewProjectile(null, npc.Center, -Terraria.Utils.RotatedByRandom(Vector2.UnitY, 0.2) * 10f, 671, 17, 0f, -1, 0f, 0f, 0f);
                        }
                    }
                    break;
            }
        }

        public override int SetState()
        {
            int num = 0;
            NPC[] array = Main.npc;
            foreach (NPC val in array)
            {
                if ((val.type == 13 || val.type == 14 || val.type == 15) && val.active)
                {
                    num++;
                }
            }
            if (num > 66)
            {
                if (State == 0)
                {
                    State = 1;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("邪恶的蠕虫寻找新的受害者", new Color(177, 94, 255));
                    }
                }
                return State;
            }
            if (num > 40)
            {
                if (State == 1)
                {
                    State = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("有毒的鳞甲炸裂开来", new Color(177, 94, 255));
                    }
                }
                return State;
            }
            if (State == 2)
            {
                State = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("怒不可遏", new Color(177, 94, 255));
                }
            }
            return State;
        }

        public override void OnKilled()
        {
            if (State == 0 || State == 2)
            {
                int num = Projectile.NewProjectile(null, npc.Center, Vector2.Zero, 501, 13, 0f, -1, 0f, 0f, 0f);
                Main.npc[num].timeLeft = 1;
                CProjectile.Update(num);
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI / 3.0 * i, default(Vector2)) * 5f, 909, 14, 0f, -1, 0f, 0f, 0f);
                }
            }
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                int num = Main.rand.Next(1, 3);
                if (num == 1)
                {
                    global::Challenger.Challenger.SendPlayerText("刺啦", new Color(177, 94, 255), npc.Center + new Vector2(0f, -30f));
                }
                else
                {
                    global::Challenger.Challenger.SendPlayerText("小心我爆炸的鳞甲", new Color(177, 94, 255), npc.Center + new Vector2(0f, -30f));
                }
            }
        }
    }
}
