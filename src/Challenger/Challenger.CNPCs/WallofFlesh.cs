using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Challenger;

public class WallofFlesh : CNPC
{
    public static readonly float CooldownOfSkill0 = 150f;

    public static readonly float CooldownOfSkill1 = 120f;

    public float skill0 = CooldownOfSkill0;

    public float skill1 = CooldownOfSkill1;

    public WallofFlesh(NPC npc)
        : base(npc)
    {
    }

    public override void NPCAI()
    {
        this.skill0 -= 1f;
        this.skill1 -= 1f;
        var targetData = this.npc.GetTargetData(true);
        var val = this.npc.DirectionTo(targetData.Position);
        this.SetState();
        if (this.skill0 < 0f)
        {
            switch (this.state)
            {
                case 0:
                    Projectile.NewProjectile(null, this.npc.Center, val * 13f, 467, 14, 5f, -1, 0f, 0f, 0f);
                    this.skill0 = CooldownOfSkill0 + Main.rand.Next(51);
                    break;
                case 1:
                    Projectile.NewProjectile(null, this.npc.Center, val * 20f, 467, 18, 5f, -1, 0f, 0f, 0f);
                    this.skill0 = CooldownOfSkill0;
                    break;
                case 2:
                    Projectile.NewProjectile(null, this.npc.Center, val * 26f, 467, 22, 5f, -1, 0f, 0f, 0f);
                    this.skill0 = CooldownOfSkill0 - 20f;
                    break;
            }
        }
        if (this.skill1 < 0f)
        {
            switch (this.state)
            {
                case 0:
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                    this.skill1 = CooldownOfSkill0 + Main.rand.Next(51);
                    break;
                case 1:
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                    this.skill1 = CooldownOfSkill0;
                    break;
                case 2:
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    Projectile.NewProjectile(null, this.npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + (Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f)) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                    this.skill1 = CooldownOfSkill0;
                    break;
            }
        }
    }

    public override int SetState()
    {
        if (this.npc.life >= this.LifeMax * 0.6f)
        {
            if (this.state == 0)
            {
                this.state = 1;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage(GetString("罪恶血祭召唤远古守卫"), new Color(255, 77, 0));
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
                    TSPlayer.All.SendMessage(GetString("付出代价吧！"), new Color(255, 77, 0));
                }
            }
            return this.state;
        }
        if (this.state == 2)
        {
            this.state = 3;
            if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
            {
                TSPlayer.All.SendMessage(GetString("速度与激情"), new Color(255, 77, 0));
            }
        }
        return this.state;
    }

    public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
    {
        if (global::Challenger.Challenger.config.EnableConsumptionMode)
        {
            global::Challenger.Challenger.SendPlayerText(GetString("咬碎你"), new Color(0, 146, 255), this.npc.Center);
        }
    }
}