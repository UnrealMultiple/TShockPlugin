using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class SkeletronHand : CNPC
{
    private int timer = 0;

    public SkeletronHand(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.SetState();
        switch (this.state - 1)
        {
            case 0:
                if (this.npc.ai[2] == 2f || this.npc.ai[2] == 5f)
                {
                    var num2 = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 299, 10, 5f, -1, 0f, 0f, 0f);
                    Main.projectile[num2].timeLeft = 30;
                    CProjectile.Update(num2);
                }
                break;
            case 1:
                if (this.npc.ai[2] == 2f || this.npc.ai[2] == 5f)
                {
                    this.timer++;
                    if (this.timer % 5 == 0)
                    {
                        var num = Projectile.NewProjectile(null, this.npc.Center, Vector2.Zero, 299, 10, 5f, -1, 0f, 0f, 0f);
                        Main.projectile[num].timeLeft = 2400;
                        CProjectile.Update(num);
                    }
                }
                else
                {
                    this.timer = 0;
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
            }
            return this.state;
        }
        if (this.state == 1)
        {
            this.state = 2;
            if (this.npc.ai[0] == -1f && global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("你打痛我左手了！！！"), new Color(150, 143, 102));
            }
            if (this.npc.ai[0] == 1f && global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("你打痛我右手了！！！"), new Color(150, 143, 102));
            }
        }
        return this.state;
    }

    public override void OnKilled()
    {
        if (this.npc.ai[0] == -1f)
        {
            var num = Main.rand.NextDouble() * 3.0;
            for (var i = 0; i < 10; i++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI / 5.0 * i) + num, default) * 5f, 270, 20, 30f, -1, 0f, 0f, 0f);
            }
        }
        else
        {
            var num2 = Main.rand.NextDouble() * 3.0;
            for (var j = 0; j < 10; j++)
            {
                Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(Vector2.UnitY, (Math.PI / 5.0 * j) + num2, default) * 5f, 299, 20, 30f, -1, 0f, 0f, 0f);
            }
        }
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            switch (Main.rand.Next(1, 4))
            {
                case 1:
                    global::Challenger.Challenger.SendPlayerText(GetString("就这还想打倒我骷髅王爷爷"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
                case 2:
                    global::Challenger.Challenger.SendPlayerText(GetString("看我一记耳光"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
                default:
                    global::Challenger.Challenger.SendPlayerText(GetString("离地牢远点！！！"), new Color(150, 143, 102), this.npc.Center + new Vector2(0f, -30f));
                    break;
            }
        }
        e.Player.SetBuff(23, 60, false);
    }
}