using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class Honey : CProjectile
{
    private Honey(Projectile projectile, float[] ai, int lable)
        : base(projectile, ai, lable)
    {
    }

    public override void ProjectileAI()
    {
        var any4 = Challenger.config.HivePack_4;
        var any5 = Challenger.config.HivePack_5;
        var any6 = Challenger.config.HivePack_6;
        var any8 = Challenger.config.HivePack_Time2;

        if (this.lable == 1)
        {
            if (this.proj.active && this.proj.timeLeft < this.ai[0] - 30f)
            {
                var player = Main.player;
                foreach (var val in player)
                {
                    if (val == null || val.dead)
                    {
                        continue;
                    }
                    var val2 = this.proj.Center - val.Center;
                    if (((Vector2) val2).LengthSquared() <= val.width * val.height / 2)
                    {
                        var num = Main.rand.Next(5, 12);
                        if (val.whoAmI != this.proj.owner)
                        {
                            num = Main.rand.Next(8, 16);
                        }
                        if (Challenger.config.EnableConsumptionMode)
                        {
                            Challenger.HealPlayer(Main.player[val.whoAmI], num, visible: false);
                            Challenger.SendPlayerText(GetString($"蜂糖罐治疗 + {num}"), new Color(232, 229, 74), val.Center);
                        }
                        else
                        {
                            Challenger.HealPlayer(Main.player[val.whoAmI], num);
                        }
                        TShock.Players[val.whoAmI].SetBuff(48, 300, false);
                        this.CKill();
                        break;
                    }
                }
            }
            if (this.proj.timeLeft < 120)
            {
                this.CKill();
                if (!Challenger.honey.TryAdd(this.proj.whoAmI, 0))
                {
                    Challenger.honey[this.proj.whoAmI] = 0;
                }
            }
        }
        else if (this.lable == 2 && this.proj.active && this.proj.timeLeft < this.ai[0] - 60)
        {
            if (Challenger.Timer % any8 == 0)
            {
                var num2 = Collect.MyNewProjectile(this.proj.GetProjectileSource_FromThis(), this.proj.Center, Vector2.Zero, any4, any5, any6, this.proj.owner);
                Main.projectile[num2].usesLocalNPCImmunity = true;
                Update(num2);
                this.CKill();
                if (!Challenger.honey.TryAdd(this.proj.whoAmI, 0))
                {
                    Challenger.honey[this.proj.whoAmI] = 0;
                }
            }
        }
    }

    public static Honey NewCProjectile(Vector2 position, Vector2 velocity, int lable, int owner, float[] ai)
    {
        var any = Challenger.config.HivePack_1;
        var any2 = Challenger.config.HivePack_2;
        var any3 = Challenger.config.HivePack_3;

        var num = Collect.MyNewProjectile(Projectile.GetNoneSource(), position, velocity, any, any2, any3, owner, 0f, lable - 1);
        var honey = new Honey(Main.projectile[num], ai, lable);
        honey.ai[0] = honey.proj.timeLeft;
        Collect.cprojs[num] = honey;
        Update(num);

        return honey;
    }
}