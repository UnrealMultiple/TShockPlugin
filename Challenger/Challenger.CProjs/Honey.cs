using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class Honey : CProjectile
    {
        private Honey(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }

        public override void ProjectileAI()
        {
            if (lable == 1)
            {
                if (proj.active && proj.timeLeft < ai[0] - 30f)
                {
                    Player[] player = Main.player;
                    foreach (Player val in player)
                    {
                        if (val == null || val.dead)
                        {
                            continue;
                        }
                        Vector2 val2 = proj.Center - val.Center;
                        if (((Vector2)val2).LengthSquared() <= val.width * val.height / 2)
                        {
                            int num = Main.rand.Next(5, 12);
                            if (val.whoAmI != proj.owner)
                            {
                                num = Main.rand.Next(8, 16);
                            }
                            if (global::Challenger.Challenger.config.EnableConsumptionMode)
                            {
                                global::Challenger.Challenger.HealPlayer(Main.player[val.whoAmI], num, visible: false);
                                global::Challenger.Challenger.SendPlayerText($"蜂糖罐治疗 + {num}", new Color(232, 229, 74), val.Center);
                            }
                            else
                            {
                                global::Challenger.Challenger.HealPlayer(Main.player[val.whoAmI], num);
                            }
                            TShock.Players[val.whoAmI].SetBuff(48, 300, false);
                            CKill();
                            break;
                        }
                    }
                }
                if (proj.timeLeft < 120)
                {
                    CKill();
                    if (!global::Challenger.Challenger.honey.TryAdd(proj.whoAmI, 0))
                    {
                        global::Challenger.Challenger.honey[proj.whoAmI] = 0;
                    }
                }
            }
            else if (lable == 2 && proj.active && proj.timeLeft < ai[0] - 60f)
            {
                int num2 = Collect.MyNewProjectile(proj.GetProjectileSource_FromThis(), proj.Center, Vector2.Zero, 566, 30, 0f, proj.owner);
                Main.projectile[num2].usesLocalNPCImmunity = true;
                Update(num2);
                CKill();
                if (!global::Challenger.Challenger.honey.TryAdd(proj.whoAmI, 0))
                {
                    global::Challenger.Challenger.honey[proj.whoAmI] = 0;
                }
            }
        }

        public static Honey NewCProjectile(Vector2 position, Vector2 velocity, int lable, int owner, float[] ai)
        {
            int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 346, 0, 0f, owner, 0f, lable - 1);
            Honey honey = new Honey(Main.projectile[num], ai, lable);
            honey.ai[0] = honey.proj.timeLeft;
            Collect.cprojs[num] = honey;
            Update(num);
            return honey;
        }
    }
}
