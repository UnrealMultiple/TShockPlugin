using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger;


public class QueenBee : CNPC
{
    private int timer = 0;

    public QueenBee(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        var targetData = this.npc.GetTargetData(true);
        this.SetState();
        switch (this.state)
        {
            case 1:
                if (this.npc.ai[0] == 0f && (this.npc.ai[1] == 1f || this.npc.ai[1] == 3f || this.npc.ai[1] == 5f) && this.npc.ai[2] == 0f && this.timer < 1)
                {
                    this.timer++;
                    if ((this.npc.direction == 1 && targetData.Position.X > this.npc.position.X) || (this.npc.direction == -1 && targetData.Position.X < this.npc.position.X))
                    {
                        var obj5 = this.npc;
                        obj5.velocity += this.npc.velocity * 0.04f;
                    }
                    else
                    {
                        var obj6 = this.npc;
                        obj6.velocity -= this.npc.velocity * 0.04f;
                    }
                    this.npc.netUpdate = true;
                }
                else
                {
                    this.timer = 0;
                }
                break;
            case 2:
                if (this.npc.ai[0] == 0f && (this.npc.ai[1] == 1f || this.npc.ai[1] == 3f || this.npc.ai[1] == 5f) && this.npc.ai[2] == 0f && this.timer < 1)
                {
                    this.timer++;
                    if ((this.npc.direction == 1 && targetData.Position.X > this.npc.position.X) || (this.npc.direction == -1 && targetData.Position.X < this.npc.position.X))
                    {
                        var obj7 = this.npc;
                        obj7.velocity += this.npc.velocity * 0.07f;
                    }
                    else
                    {
                        var obj8 = this.npc;
                        obj8.velocity -= this.npc.velocity * 0.07f;
                    }
                    this.npc.netUpdate = true;
                }
                else
                {
                    this.timer = 0;
                }
                break;
            case 3:
                if (this.npc.ai[0] == 0f && (this.npc.ai[1] == 1f || this.npc.ai[1] == 3f || this.npc.ai[1] == 5f) && this.npc.ai[2] == 0f && this.timer < 1)
                {
                    this.timer++;
                    if ((this.npc.direction == 1 && targetData.Position.X > this.npc.position.X) || (this.npc.direction == -1 && targetData.Position.X < this.npc.position.X))
                    {
                        var obj3 = this.npc;
                        obj3.velocity += this.npc.velocity * 0.12f;
                    }
                    else
                    {
                        var obj4 = this.npc;
                        obj4.velocity -= this.npc.velocity * 0.12f;
                    }
                    this.npc.netUpdate = true;
                }
                else
                {
                    this.timer = 0;
                }
                if (Main.rand.Next(6) == 0)
                {
                    Projectile.NewProjectile(null, this.npc.Bottom, Terraria.Utils.RotateRandom(Vector2.UnitY, Math.PI / 2.0) * -8f, 719, 12, 1f, -1, 0f, 0f, 0f);
                }
                break;
            case 4:
                if (this.npc.ai[0] == 0f && (this.npc.ai[1] == 1f || this.npc.ai[1] == 3f || this.npc.ai[1] == 5f) && this.npc.ai[2] == 0f && this.timer < 1)
                {
                    this.timer++;
                    if ((this.npc.direction == 1 && targetData.Position.X > this.npc.position.X) || (this.npc.direction == -1 && targetData.Position.X < this.npc.position.X))
                    {
                        var obj = this.npc;
                        obj.velocity += this.npc.velocity * 0.12f;
                    }
                    else
                    {
                        var obj2 = this.npc;
                        obj2.velocity -= this.npc.velocity * 0.12f;
                    }
                    this.npc.netUpdate = true;
                }
                else
                {
                    this.timer = 0;
                }
                if (this.npc.ai[1] % 12f == 0f)
                {
                    Projectile.NewProjectile(null, this.npc.position - new Vector2(Main.rand.Next(-1024, 1024), 384f), Vector2.UnitY * -3f, 719, 20, 1f, -1, 0f, 0f, 0f);
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
                    TSPlayer.All.SendMessage(GetString("谁人惊扰了我的蜂巢！"), Color.Yellow);
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
                    TSPlayer.All.SendMessage(GetString("不许抢我的蜂蜜"), Color.Yellow);
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
                    TSPlayer.All.SendMessage(GetString("毒刺射你一脸"), Color.Yellow);
                }
            }
            return this.state;
        }
        if (this.state == 3)
        {
            this.state = 4;
        }
        return this.state;
    }

    public override void OnHurtPlayers(PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    global::Challenger.Challenger.SendPlayerText(GetString("嗡嗡"), Color.Yellow, this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    global::Challenger.Challenger.SendPlayerText(GetString("嗡嗡嗡嗡"), Color.Yellow, this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    global::Challenger.Challenger.SendPlayerText(GetString("吱嗡"), Color.Yellow, this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
    }
}