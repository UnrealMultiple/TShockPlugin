using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger
{
    public class BeetleHeal : CProjectile
    {
        private BeetleHeal(Projectile projectile, float[] ai, int lable)
            : base(projectile, ai, lable)
        {
        }
        //用Config控制其他玩家恢复量
        internal static Config config = new Config();
        public static int OtherPlayerHealAmount => config.OtherPlayerHealAmount;

        public override void ProjectileAI()
        {
            Player val = global::Challenger.Challenger.NearWeakestPlayer(proj.Center, 640000f, Main.player[proj.owner]);
            if (val != null && proj.owner != val.whoAmI)
            {
                Projectile? obj = proj;
                obj.Center = obj.Center + (val.Center - proj.Center).SafeNormalize(Vector2.Zero) * 10f;
            }
            if (proj.active && proj.timeLeft <= ai[1] - 60f)
            {
                if (ai[0] > 200f)
                {
                    ai[0] = 200f;
                }
                else if (ai[0] < 0f)
                {
                    ai[0] = 0f;
                }
                try
                {
                    int num = (int)ai[0] - config.OtherPlayerHealAmount; ;
                    TSPlayer[] players = TShock.Players;
                    foreach (TSPlayer val2 in players)
                    {
                        if (val2 == null || !val2.Active || !val2.TPlayer.active || val2.TPlayer.dead || val2.Index == proj.owner)
                        {
                            continue;
                        }
                        Vector2 val3 = proj.Center - val2.TPlayer.Center;
                        if (((Vector2)val3).LengthSquared() <= val2.TPlayer.width * val2.TPlayer.height)
                        {
                            global::Challenger.Challenger.HealPlayer(val2.TPlayer, num, visible: false);
                            val2.SetBuff(95, 300, false);
                            if (global::Challenger.Challenger.config.EnableConsumptionMode)
                            {
                                Challenger.SendPlayerText($"甲虫治疗 + {num} 治疗者:{Main.player[proj.owner].name}", new Color(210, 0, 255), val2.TPlayer.Center + new Vector2(Main.rand.Next(-60, 61), Main.rand.Next(61)));
                                val2.SendMessage($"你被 {Main.player[proj.owner].name} 治疗了 {num} 点生命值", new Color(210, 0, 255));
                            }
                            else
                            {
                                global::Challenger.Challenger.SendPlayerText($"{num}", new Color(0, 255, 0), val2.TPlayer.Center + new Vector2(Main.rand.Next(-60, 61), Main.rand.Next(61)));
                            }
                            CKill();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.Error("代码异常4：" + ex.ToString());
                    Console.WriteLine("代码异常4：" + ex.ToString());
                }
            }
            Update();
        }

        public static BeetleHeal NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
        {
            int num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 121, 0, 0f, owner);
            BeetleHeal beetleHeal = new BeetleHeal(Main.projectile[num], ai, lable);
            beetleHeal.ai[1] = beetleHeal.proj.timeLeft;
            Collect.cprojs[num] = beetleHeal;
            beetleHeal.Update();
            return beetleHeal;
        }
    }
}
