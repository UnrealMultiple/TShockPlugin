using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger;

public class Deerclops : CNPC
{
    private const float CooldownOfSkill0 = 350f;

    private float skill0 = 0f;

    public Deerclops(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.skill0 += 1f;
        var targetData = this.npc.GetTargetData(true);
        if (this.npc.ai[0] == 6f)
        {
            var obj = this.npc;
            obj.life += 2;
            this.npc.StrikeNPC(0, 0f, 0, false, false, false, 255,null);
        }
        this.SetState();
        switch (this.state)
        {
            case 1:
                if (this.npc.ai[0] == 5f && this.npc.ai[1] == 59f && Main.netMode != 1)
                {
                    var val7 = default(Vector2);
                    var val8 = default(Vector2);
                    var num5 = default(float);
                    var num6 = default(float);
                    for (var m = 0; m < 3; m++)
                    {
                        Projectile.RandomizeInsanityShadowFor((Entity) (object) Main.player[this.npc.target], true, out val7, out val8, out num5, out num6);
                        Projectile.NewProjectile(null, val7, val8, 965, 12, 0f, Main.myPlayer, num5, num6, 0f);
                    }
                }
                break;
            case 2:
                if (this.npc.ai[0] == 1f && this.npc.ai[1] == 30f)
                {
                    var val4 = Terraria.Utils.ToTileCoordinates(this.npc.Top);
                    for (var k = 5; k < 20; k++)
                    {
                        this.npc.AI_123_Deerclops_ShootRubbleUp(ref targetData, ref val4, 20, 1, 200f, k);
                    }
                    if (Main.rand.Next(1) == 0 && this.npc.ai[1] == 79f)
                    {
                        this.npc.ai[0] = 5f;
                        this.npc.ai[1] = 0f;
                    }
                }
                else if (this.npc.ai[0] == 5f && this.npc.ai[1] == 59f && Main.netMode != 1)
                {
                    var val5 = default(Vector2);
                    var val6 = default(Vector2);
                    var num3 = default(float);
                    var num4 = default(float);
                    for (var l = 0; l < 8; l++)
                    {
                        Projectile.RandomizeInsanityShadowFor((Entity) (object) Main.player[this.npc.target], true, out val5, out val6, out num3, out num4);
                        Projectile.NewProjectile(null, val5, val6, 965, 12, 0f, Main.myPlayer, num3, num4, 0f);
                    }
                }
                if (this.skill0 >= 7f)
                {
                    Projectile.NewProjectile(null, targetData.Position + new Vector2(Main.rand.Next(-1536, 1536), -768f), Vector2.UnitY, 174, 5, 0f, -1, 0f, 0f, 0f);
                    this.skill0 = 0f;
                }
                break;
            case 3:
                if (this.npc.ai[0] == 1f && this.npc.ai[1] == 30f)
                {
                    var val = Terraria.Utils.ToTileCoordinates(this.npc.Top);
                    for (var i = 5; i < 20; i++)
                    {
                        this.npc.AI_123_Deerclops_ShootRubbleUp(ref targetData, ref val, 20, 1, 200f, i);
                    }
                    if (Main.rand.Next(1) == 0 && this.npc.ai[1] == 79f)
                    {
                        this.npc.ai[0] = 5f;
                        this.npc.ai[1] = 0f;
                    }
                }
                else if (this.npc.ai[0] == 5f && this.npc.ai[1] == 59f && Main.netMode != 1)
                {
                    var val2 = default(Vector2);
                    var val3 = default(Vector2);
                    var num = default(float);
                    var num2 = default(float);
                    for (var j = 0; j < 8; j++)
                    {
                        Projectile.RandomizeInsanityShadowFor((Entity) (object) Main.player[this.npc.target], true, out val2, out val3, out num, out num2);
                        Projectile.NewProjectile(null, val2, val3, 965, 13, 0f, Main.myPlayer, num, num2, 0f);
                    }
                }
                if (this.skill0 >= 3f)
                {
                    Projectile.NewProjectile(null, targetData.Position + new Vector2(Main.rand.Next(-1024, 1024), -1024f), Vector2.UnitY * 3f, 174, 9, 5f, -1, 0f, 0f, 0f);
                    this.skill0 = 0f;
                }
                break;
        }
    }

    public override int SetState()
    {
        if (this.npc.life >= this.LifeMax * 0.8f)
        {
            if (this.state == 0)
            {
                this.state = 1;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("远方的巨兽将会摧毁你所拥有的一切"), new Color(111, 160, 213));
                }
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.3f)
        {
            if (this.state == 1)
            {
                this.state = 2;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("冰雪从天而降"), new Color(111, 160, 213));
                }
            }
            return this.state;
        }
        if (this.state == 2)
        {
            this.state = 3;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("你将受到灭顶之灾"), new Color(111, 160, 213));
            }
        }
        return this.state;
    }

    public override void OnHurtPlayers(PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            var num = Main.rand.Next(1, 3);
            if (num == 1)
            {
                global::Challenger.Challenger.SendPlayerText(GetString("拆掉拆掉！"), new Color(111, 160, 213), this.npc.Center + new Vector2(0f, -30f));
            }
            else
            {
                global::Challenger.Challenger.SendPlayerText(GetString("嗷嗷"), new Color(111, 160, 213), this.npc.Center + new Vector2(0f, -30f));
            }
        }
    }
}