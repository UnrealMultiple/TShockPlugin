using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;


public class WallofFleshEye : CNPC
{
    public static readonly float CooldownOfSkill0 = 140f;

    public float skill0 = CooldownOfSkill0;

    public WallofFleshEye(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.skill0 -= 1f;
        if (this.skill0 < 0f)
        {
            var targetData = this.npc.GetTargetData(true);
            var val = this.npc.DirectionTo(targetData.Position);
            this.SetState();
            switch (this.state)
            {
                case 0:
                    Projectile.NewProjectile(null, this.npc.Center, val * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                    this.skill0 += CooldownOfSkill0 + Main.rand.Next(100);
                    break;
                case 1:
                    Projectile.NewProjectile(null, this.npc.Center, val * 9f, 83, 12, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default) * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default) * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                    this.skill0 += CooldownOfSkill0 + Main.rand.Next(80);
                    break;
                case 2:
                    Projectile.NewProjectile(null, this.npc.Center, val * 9f, 83, 14, 20f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default) * 10f, 83, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default) * 10f, 83, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.2, default) * 8f, 83, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.2, default) * 8f, 83, 14, 5f, -1, 0f, 0f, 0f);
                    this.skill0 += CooldownOfSkill0 + Main.rand.Next(30);
                    break;
                case 3:
                    Projectile.NewProjectile(null, this.npc.Center, val * 15f, 83, 15, 20f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default) * 15f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default) * 15f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.15, default) * 14f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.15, default) * 14f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.2, default) * 13f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.2, default) * 13f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, 0.25, default) * 12f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center, Terraria.Utils.RotatedBy(val, -0.25, default) * 12f, 83, 15, 5f, -1, 0f, 0f, 0f);
                    this.skill0 += CooldownOfSkill0;
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
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.4f)
        {
            if (this.state == 1)
            {
                this.state = 2;
            }
            return this.state;
        }
        if (this.npc.life >= this.LifeMax * 0.2f)
        {
            if (this.state == 2)
            {
                this.state = 3;
            }
            return this.state;
        }
        if (this.state == 3)
        {
            this.state = 4;
        }
        return this.state;
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            global::Challenger.Challenger.SendPlayerText(GetString("这么想看清我的卡姿兰大眼是吧"), new Color(0, 146, 255), this.npc.Center + new Vector2(0f, -30f));
        }
    }
}