using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

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
        var val = Challenger.NearWeakestPlayer(this.proj.Center, 640000f, Main.player[this.proj.owner]);
        if (val != null && this.proj.owner != val.whoAmI)
        {
            var obj = this.proj;
            obj.Center += (val.Center - this.proj.Center).SafeNormalize(Vector2.Zero) * 10f;
        }
        if (this.proj.active && this.proj.timeLeft <= this.ai[1] - 60f)
        {
            if (this.ai[0] > 200f)
            {
                this.ai[0] = 200f;
            }
            else if (this.ai[0] < 0f)
            {
                this.ai[0] = 0f;
            }
            try
            {
                var num = (int) this.ai[0] - config.OtherPlayerHealAmount; ;
                var players = TShock.Players;
                foreach (var val2 in players)
                {
                    if (val2 == null || !val2.Active || !val2.TPlayer.active || val2.TPlayer.dead || val2.Index == this.proj.owner)
                    {
                        continue;
                    }
                    var val3 = this.proj.Center - val2.TPlayer.Center;
                    if (((Vector2) val3).LengthSquared() <= val2.TPlayer.width * val2.TPlayer.height)
                    {
                        Challenger.HealPlayer(val2.TPlayer, num, visible: false);
                        val2.SetBuff(95, 300, false);
                        if (Challenger.config.EnableConsumptionMode)
                        {
                            Challenger.SendPlayerText(GetString($"甲虫治疗 + {num} 治疗者:{Main.player[this.proj.owner].name}"), new Color(210, 0, 255), val2.TPlayer.Center + new Vector2(Main.rand.Next(-60, 61), Main.rand.Next(61)));
                            val2.SendMessage(GetString($"你被 {Main.player[this.proj.owner].name} 治疗了 {num} 点生命值"), new Color(210, 0, 255));
                        }
                        else
                        {
                            Challenger.SendPlayerText($"{num}", new Color(0, 255, 0), val2.TPlayer.Center + new Vector2(Main.rand.Next(-60, 61), Main.rand.Next(61)));
                        }
                        this.CKill();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(GetString($"代码异常4：{ex}"));
                Console.WriteLine(GetString($"代码异常4：{ex}"));
            }
        }
        this.Update();
    }

    public static BeetleHeal NewCProjectile(Vector2 position, Vector2 velocity, int owner, float[] ai, int lable)
    {
        var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, 121, 0, 0f, owner);
        var beetleHeal = new BeetleHeal(Main.projectile[num], ai, lable);
        beetleHeal.ai[1] = beetleHeal.proj.timeLeft;
        Collect.cprojs[num] = beetleHeal;
        beetleHeal.Update();
        return beetleHeal;
    }
}