using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace Challenger;

public class Skeletron : CNPC
{
    private float rotate = 0f;

    private readonly List<int> surrandIndex = new List<int>();

    private const int MaxSurrandNum = 10;

    private int skill0 = 240;

    public Skeletron(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.skill0--;
        this.SetState();
        var targetData = this.npc.GetTargetData(true);
        switch (this.state)
        {
            case 1:
                if (this.skill0 < 0)
                {
                    var val = Terraria.Utils.DirectionTo(this.npc.Center, targetData.Center);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.5235987901687622, default), 270, 11, 5f, -1, 0f, 0f, 0f);
                    this.skill0 = 220 + Main.rand.Next(-60, 61);
                }
                break;
            case 2:
                if (this.skill0 < 0)
                {
                    for (var l = 0; l < 4; l++)
                    {
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI / 2.0 * l) + this.rotate, default) * 4f, 270, 12, 5f, -1, 0f, 0f, 0f);
                    }
                    this.skill0 = 160 + Main.rand.Next(-60, 61);
                }
                break;
            case 3:
                if (this.skill0 < 0)
                {
                    var num3 = Main.rand.Next(4, 7);
                    for (var k = 0; k < num3; k++)
                    {
                        Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI * 2.0 / 5.0 * k) + this.rotate, default) * 3f, 270, 15, 5f, -1, 0f, 0f, 0f);
                    }
                    this.skill0 = 120 + Main.rand.Next(-60, 61);
                }
                if (this.npc.ai[1] == 1f && this.npc.ai[2] % 10f == 0f)
                {
                    var num4 = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 299, 15, 5f, -1, 0f, 0f, 0f);
                    Main.projectile[num4].timeLeft = 40;
                    CProjectile.Update(num4);
                }
                break;
            case 4:
                if (this.skill0 < 0)
                {
                    var num = Main.rand.Next(5, 10);
                    if (Main.rand.Next(2) == 0)
                    {
                        for (var i = 0; i < num; i++)
                        {
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI / 3.0 * i) + this.rotate, default) * 3f, 270, 18, 30f, -1, 0f, 0f, 0f);
                        }
                    }
                    else
                    {
                        for (var j = 0; j < num; j++)
                        {
                            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI / 4.0 * j) + this.rotate, default) * 5f, 299, 18, 30f, -1, 0f, 0f, 0f);
                        }
                    }
                    this.skill0 = 100 + Main.rand.Next(-60, 31);
                }
                if (this.npc.ai[1] == 1f && this.npc.ai[2] % 5f == 0f)
                {
                    var num2 = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 299, 20, 5f, -1, 0f, 0f, 0f);
                    Main.projectile[num2].timeLeft = 180;
                    CProjectile.Update(num2);
                }
                break;
        }
        if (this.ai[0] < 10f && Main.rand.Next(180) == 0)
        {
            var num5 = NPC.NewNPC(this.npc.GetSpawnSourceForNPCFromNPCAI(), (int) this.npc.Center.X, (int) this.npc.Center.Y, 34, 0, 0f, 0f, 0f, 0f, 255);
            Main.npc[num5].lifeMax = 500;
            Main.npc[num5].life = 501;
            this.ai[0] += 1f;
            this.surrandIndex.Add(num5);
        }
        this.ai[0] -= this.surrandIndex.RemoveAll((int x) => Main.npc[x] == null || !Main.npc[x].active || Main.npc[x].netID != 34);
        this.rotate += 0.1f;
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
                    TSPlayer.All.SendMessage(GetString("被封印的骷髅帝王苏醒"), new Color(150, 143, 102));
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
                    TSPlayer.All.SendMessage(GetString("嘎吱作响"), new Color(150, 143, 102));
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
                    TSPlayer.All.SendMessage(GetString("诅咒开始应验"), new Color(150, 143, 102));
                }
            }
            return this.state;
        }
        if (this.state == 3)
        {
            this.state = 4;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("惨朽不堪"), new Color(150, 143, 102));
            }
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
                    global::Challenger.Challenger.SendPlayerText(GetString("再让我逮到一下你就玩玩"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    global::Challenger.Challenger.SendPlayerText(GetString("创死你"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    global::Challenger.Challenger.SendPlayerText(GetString("想再贴贴吗？"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
    }

    public override void OnKilled()
    {
        for (var i = 0; i < 35; i++)
        {
            Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, Math.PI * 2.0 / 35.0 * i, default) * 5f, 299, 21, 10f, -1, 0f, 0f, 0f);
        }
    }
}