using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;


internal class BrainofCthulhu : CNPC
{
    public BrainofCthulhu(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.SetState();
        var targetData = this.npc.GetTargetData(true);
        if (this.npc.ai[0] >= 0f || this.npc.ai[3] != 225f)
        {
            return;
        }
        switch (this.state)
        {
            case 1:
                break;
            case 2:
            {
                var num4 = Main.rand.Next(2, 5);
                for (var j = 0; j < num4; j++)
                {
                    var num5 = (float) Main.rand.NextDouble() - 0.5f;
                    var num6 = -0.25f * (float) Math.Cos(3.1415927410125732 * (double) num5);
                    Projectile.NewProjectile(null, (targetData.Center * 2f) - this.npc.Center, new Vector2(0f - num5, 0f - num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, new Vector2((targetData.Center - this.npc.Center).X * 2f, 0f) + this.npc.Center, new Vector2(0f - num5, num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, new Vector2(0f, (targetData.Center - this.npc.Center).Y * 2f) + this.npc.Center, new Vector2(num5, 0f - num6) * 17f, 811, 0, 0f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, new Vector2(num5, num6) * 17f, 811, 15, 5f, -1, 0f, 0f, 0f);
                }
                break;
            }
            case 3:
            {
                var num = Main.rand.Next(4, 7);
                for (var i = 0; i < num; i++)
                {
                    var num2 = (float) Main.rand.NextDouble() - 0.5f;
                    var num3 = -0.25f * (float) Math.Cos(3.1415927410125732 * (double) num2);
                    Projectile.NewProjectile(null, (targetData.Center * 2f) - this.npc.Center, new Vector2(0f - num2, 0f - num3) * 17f, 811, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, new Vector2((targetData.Center - this.npc.Center).X * 2f, 0f) + this.npc.Center, new Vector2(0f - num2, num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, new Vector2(0f, (targetData.Center - this.npc.Center).Y * 2f) + this.npc.Center, new Vector2(num2, 0f - num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, new Vector2(num2, num3) * 17f, 811, 18, 5f, -1, 0f, 0f, 0f);
                }
                break;
            }
        }
    }

    public override int SetState()
    {
        if (this.npc.life >= this.LifeMax * 0.98f)
        {
            if (this.state == 0)
            {
                this.state = 1;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("畸形怪脑发现了新鲜的脑子"), new Color(255, 94, 94));
                }
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.5f)
        {
            if (this.state == 1)
            {
                this.state = 2;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("你真的能分清真假血弹吗"), new Color(255, 94, 94));
                }
            }
            return this.state;
        }
        if (this.state == 2)
        {
            this.state = 3;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("虚虚实实，实实虚虚"), new Color(255, 94, 94));
            }
        }
        return this.state;
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            var num = Main.rand.Next(1, 3);
            if (num == 1)
            {
                global::Challenger.Challenger.SendPlayerText(GetString("糊你一脸"), Color.Red, this.npc.Center + new Vector2(0f, -30f));
            }
            else
            {
                global::Challenger.Challenger.SendPlayerText(GetString("哇哇嗷"), Color.Red, this.npc.Center + new Vector2(0f, -30f));
            }
        }
    }
}