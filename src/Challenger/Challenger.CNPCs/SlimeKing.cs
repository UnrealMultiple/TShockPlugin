using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class SlimeKing : CNPC
{
    public readonly float CooldownOfSkill0 = 250f;

    public readonly float CooldownOfSkill1 = 250f;

    public readonly float CooldownOfSkill2 = 250f;

    public SlimeKing(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        var targetData = this.npc.GetTargetData(true);
        if (this.npc.ai[0] > -200f && this.npc.ai[0] < -120f)
        {
            var val = Terraria.Utils.ToTileCoordinates(this.npc.Bottom);
            var num = 20;
            var num2 = 1;
            val.X += this.npc.direction * 3;
            var num3 = (int) ((this.npc.ai[0] / 2f) + 101f);
            var num4 = 4;
            var num5 = num3 / num4 * num4;
            var num6 = num5 + num4;
            if (num3 % num4 != 0)
            {
                num6 = num5;
            }
            for (var i = num5; i < num6 && i < num; i++)
            {
                var num7 = i * num2;
                this.npc.AI_123_Deerclops_TryMakingSpike(ref val, this.npc.direction, num, i, num7);
                this.npc.AI_123_Deerclops_TryMakingSpike(ref val, -this.npc.direction, num, i, num7);
            }
        }
        this.SetState();
        switch (this.state)
        {
            case 1:
                this.ai[0] -= 1f;

                if (this.ai[0] < 0f && this.npc.ai[0] != -120f && this.npc.ai[0] != -200f)
                {
                    var val3 = new Vector2();
                    var directionX = (targetData.Position.X - this.npc.Center.X > 0f) ? 1f : -1f;
                    val3.X = directionX;
                    val3.Y = -2f;

                    Entity entity = this.npc;
                    entity.velocity.X += val3.X * Main.rand.Next(5, 12);
                    entity.velocity.Y += val3.Y * Main.rand.Next(1, 4);

                    this.ai[0] = this.CooldownOfSkill0 + Main.rand.Next(121);
                    this.npc.netUpdate = true;
                }
                break;
            case 2:
                this.ai[0] -= 1f;
                this.ai[1] -= 1f;

                if (this.ai[0] < 0f && this.npc.ai[0] != -120f && this.npc.ai[0] != -200f)
                {
                    var val5 = new Vector2();
                    var directionX = (targetData.Position.X - this.npc.Center.X > 0f) ? 1f : -1f;
                    val5.X = directionX;
                    val5.Y = -2f;

                    Entity entity = this.npc;
                    entity.velocity += val5 * Main.rand.Next(3, 7);

                    this.ai[0] = this.CooldownOfSkill0 + Main.rand.Next(81);
                    this.npc.netUpdate = true;
                }

                if (this.ai[1] < 0f)
                {
                    Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 464, 16, 32f, -1, 0f, 0f, 0f);
                    this.ai[1] = this.CooldownOfSkill1 + Main.rand.Next(151);
                }
                break;
            case 3:
            {
                this.ai[1] -= 1f;
                this.ai[2] -= 1f;
                var val4 = this.npc.DirectionTo(targetData.Position);
                ((Vector2) val4).Normalize();
                val4 *= 2.5f;
                if (this.ai[1] < 0f)
                {
                    Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 464, 16, 64f, -1, 0f, 0f, 0f);
                    this.ai[1] = this.CooldownOfSkill1 + Main.rand.Next(121);
                }
                if (this.ai[2] < 120f && this.ai[2] % 40f == 0f)
                {
                    Projectile.NewProjectile(null, this.npc.Center, val4, 348, 18, 64f, -1, 0f, 0f, 0f);
                }
                if (this.ai[2] < 0f)
                {
                    this.ai[2] = this.CooldownOfSkill2 + 300f;
                }
                break;
            }
            case 4:
            {
                this.ai[1] -= 1f;
                this.ai[2] -= 1f;
                if (this.ai[1] < 0f && this.ai[2] == 500f)
                {
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.One, Main.rand.NextDouble() * Math.PI, default), 464, 8, 64f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.One, Main.rand.NextDouble() * Math.PI, default), 464, 8, 64f, -1, 0f, 0f, 0f);
                    this.ai[1] = this.CooldownOfSkill1 + Main.rand.Next(151);
                }
                var val2 = this.npc.DirectionTo(targetData.Position);
                ((Vector2) val2).Normalize();
                val2 *= 2.25f;
                if (this.ai[2] < 180f && this.ai[2] % 60f == 0f)
                {
                    if (Main.rand.Next(1, 3) == 2)
                    {
                        Projectile.NewProjectile(null, this.npc.Center, val2, 348, 16, 64f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, 0.45, default), 348, 14, 64f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, -0.45, default), 348, 14, 64f, -1, 0f, 0f, 0f);
                    }
                    else
                    {
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, 0.4, default), 348, 14, 64f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val2, -0.4, default), 348, 14, 64f, -1, 0f, 0f, 0f);
                    }
                }
                if (this.ai[2] < 0f)
                {
                    this.ai[2] = this.CooldownOfSkill2 + 200f;
                }
                break;
            }
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
                    TSPlayer.All.SendMessage(GetString("史莱姆王习得冰魔法归来"), new Color(0, 146, 255));
                }
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.4f)
        {
            if (this.state == 1)
            {
                this.state = 2;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("寒风呼啸"), new Color(0, 146, 255));
                }
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.2f)
        {
            if (this.state == 2)
            {
                this.state = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("你感觉寒冷刺骨"), new Color(0, 146, 255));
                }
            }
            return this.state;
        }
        if (this.state == 3)
        {
            this.state = 4;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("史莱姆王发怒了"), new Color(0, 146, 255));
            }
        }
        return this.state;
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (this.npc is null)
        {
            return;
        }
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    global::Challenger.Challenger.SendPlayerText(GetString("走位真菜"), new Color(0, 146, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    global::Challenger.Challenger.SendPlayerText(GetString("连我都打不过，回家喝奶吧你"), new Color(0, 146, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    global::Challenger.Challenger.SendPlayerText(GetString("小辣鸡"), new Color(0, 146, 255), this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
    }
}