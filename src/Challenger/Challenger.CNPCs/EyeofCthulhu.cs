using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger;

public class EyeofCthulhu : CNPC
{
    public const float CooldownOfSkill0 = 150f;

    public const float CooldownOfSkill1 = 150f;

    public const float CooldownOfSkill2 = 500f;

    public const int ProjectileSpeedOfState3 = 5;

    public float skill0 = 150f;

    public float skill1 = 150f;

    public float skill2 = 500f;

    public EyeofCthulhu(NPC npc)
        : base(npc)
    {
    }

    private void Spawn(int number)
    {
        var num = NPC.CountNPCS(5);
        if (num >= 20)
        {
            return;
        }
        if (20 - num > number)
        {
            for (var i = 0; i < number; i++)
            {
                NPC.NewNPC(null, (int) this.npc.Bottom.X + Main.rand.Next(-32, 33), (int) this.npc.Bottom.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
            }
        }
        else
        {
            for (var j = 0; j < 20 - num; j++)
            {
                NPC.NewNPC(null, (int) this.npc.Bottom.X + Main.rand.Next(-32, 33), (int) this.npc.Bottom.Y, 5, 0, 0f, 0f, 0f, 0f, 255);
            }
        }
    }

    public override void NPCAI()
    {
        var targetData = this.npc.GetTargetData(true);
        if (this.npc.ai[0] == 1f && this.npc.ai[1] % 2f == 0f)
        {
            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.ToRotationVector2(this.npc.rotation) * 3f, 96, 8, 5f, -1, 0f, 0f, 0f);
        }
        this.SetState();
        switch (this.state)
        {
            case 1:
                this.skill0 -= 1f;
                if (this.skill0 < 0f && this.npc.ai[1] == 0f)
                {
                    var val4 = this.npc.DirectionTo(targetData.Position);
                    if (this.npc.ai[2] == 40f)
                    {
                        Projectile.NewProjectile(null, this.npc.Bottom, val4 * 8f, 96, 4, 5f, -1, 0f, 0f, 0f);
                    }
                    else if (this.npc.ai[2] == 80f)
                    {
                        Projectile.NewProjectile(null, this.npc.Bottom, val4 * 10f, 96, 5, 5f, -1, 0f, 0f, 0f);
                    }
                    else if (this.npc.ai[2] == 100f)
                    {
                        Projectile.NewProjectile(null, this.npc.Bottom, val4 * 10f, 96, 6, 6f, -1, 0f, 0f, 0f);
                        this.skill0 = 150f + Main.rand.Next(100);
                    }
                }
                if (this.npc.ai[1] == 2f && (this.npc.ai[3] == 0f || this.npc.ai[3] == 1f || this.npc.ai[3] == 2f) && this.npc.ai[2] <= 2f)
                {
                    var obj3 = this.npc;
                    obj3.velocity += this.npc.velocity * 0.1f;
                    this.npc.netUpdate = true;
                }
                break;
            case 2:
                this.skill0 -= 1f;
                if (this.skill0 < 0f && this.npc.ai[1] == 0f)
                {
                    var val2 = this.npc.DirectionTo(targetData.Position);
                    if (this.npc.ai[2] == 120f)
                    {
                        if (Main.rand.Next(1, 3) == 1)
                        {
                            Projectile.NewProjectile(null, this.npc.Center, val2 * 12f, 96, 8, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, 0.1, default) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, -0.1, default) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                        }
                        else
                        {
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, 0.1, default) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, -0.1, default) * 11f, 96, 7, 5f, -1, 0f, 0f, 0f);
                        }
                        this.skill0 = 150f;
                        this.npc.ai[2] = 100f;
                        this.Spawn(2);
                    }
                }
                if (this.npc.ai[1] == 2f && (this.npc.ai[3] == 0f || this.npc.ai[3] == 1f || this.npc.ai[3] == 2f) && this.npc.ai[2] == 0f)
                {
                    var obj2 = this.npc;
                    obj2.velocity += this.npc.velocity;
                    var val3 = Terraria.Utils.SafeNormalize(this.npc.velocity, Vector2.Zero);
                    this.npc.netUpdate = true;
                    Projectile.NewProjectile(null, this.npc.Center + (val3 * 2f), val3 * 18f, 96, 5, 5f, -1, 0f, 0f, 0f);
                }
                break;
            case 3:
                this.skill1 -= 1f;
                if (this.skill1 < 0f)
                {
                    this.Spawn(3);
                    this.skill1 = 150f + Main.rand.Next(51);
                }
                if (this.npc.ai[1] == 4f && this.npc.ai[2] % 15f == 0f)
                {
                    var num2 = Collect.MyNewProjectile(null, this.npc.Center, Terraria.Utils.RotateRandom(Vector2.One, 6.2831854820251465) * 0.5f, 96, 6, 5f);
                    Main.projectile[num2].timeLeft = 240;
                    CProjectile.Update(num2);
                }
                if (this.npc.ai[1] == 4f && (this.npc.ai[3] == 0f || this.npc.ai[3] == 1f || this.npc.ai[3] == 2f) && this.npc.ai[2] == 0f)
                {
                    var obj4 = this.npc;
                    obj4.velocity += this.npc.velocity;
                    var val5 = Terraria.Utils.SafeNormalize(this.npc.velocity, Vector2.Zero);
                    this.npc.netUpdate = true;
                    Projectile.NewProjectile(null, this.npc.Center + (val5 * 2f), val5 * 20f, 96, 5, 5f, -1, 0f, 0f, 0f);
                }
                break;
            case 4:
                this.skill1 -= 1f;
                this.skill2 -= 1f;
                if (this.skill1 < 0f)
                {
                    this.Spawn(3);
                    this.skill1 = 250f + Main.rand.Next(150);
                }
                if (this.skill2 < 0f && this.npc.ai[1] == 4f && this.npc.ai[3] == 4f)
                {
                    this.npc.ai[3] = 0f;
                    this.skill2 = 500f;
                }
                if (this.npc.ai[1] == 4f && this.npc.ai[2] % 10f == 0f)
                {
                    var num = Collect.MyNewProjectile(null, this.npc.Center, Terraria.Utils.RotateRandom(Vector2.One, 6.2831854820251465) * 0.5f, 96, 9, 5f);
                    Main.projectile[num].timeLeft = 600;
                    CProjectile.Update(num);
                }
                if (this.npc.ai[1] == 4f && (this.npc.ai[3] == 0f || this.npc.ai[3] == 1f || this.npc.ai[3] == 2f) && this.npc.ai[2] == 0f)
                {
                    var obj = this.npc;
                    obj.velocity += this.npc.velocity;
                    var val = Terraria.Utils.SafeNormalize(this.npc.velocity, Vector2.Zero);
                    this.npc.netUpdate = true;
                    Projectile.NewProjectile(null, this.npc.Center + (val * 2f), val * 30f, 96, 6, 5f, -1, 0f, 0f, 0f);
                }
                break;
        }
    }

    public override int SetState()
    {
        if (this.npc.life >= this.LifeMax * 0.7f)
        {
            if (this.state == 0)
            {
                this.state = 1;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("燃烧！无法熄灭的火焰"), new Color(200, 200, 200));
                }
            }
            return 0;
        }
        if (this.npc.life >= this.LifeMax * 0.4f)
        {
            if (this.state == 1)
            {
                this.state = 2;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("你找到那颗子弹了吗"), new Color(200, 200, 200));
                }
            }
            return 1;
        }
        if (this.npc.life >= this.LifeMax * 0.2f)
        {
            if (this.state == 2)
            {
                this.state = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("猪突猛进！"), new Color(200, 200, 200));
                }
            }
            return 2;
        }
        if (this.state == 3)
        {
            this.state = 4;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("疯狗狂叫！！！"), new Color(200, 200, 200));
            }
        }
        return 3;
    }

    public override void OnHurtPlayers(PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            var num = Main.rand.Next(1, 3);
            if (num == 1)
            {
                global::Challenger.Challenger.SendPlayerText(GetString("就这就这！"), new Color(200, 200, 200), this.npc.Center + new Vector2(0f, -30f));
            }
            else
            {
                global::Challenger.Challenger.SendPlayerText(GetString("看我创死你"), new Color(200, 200, 200), this.npc.Center + new Vector2(0f, -30f));
            }
        }
    }
}