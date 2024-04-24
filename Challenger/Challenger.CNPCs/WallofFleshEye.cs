using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace Challenger
{

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
            skill0 -= 1f;
            if (skill0 < 0f)
            {
                NPCAimedTarget targetData = npc.GetTargetData(true);
                Vector2 val = npc.DirectionTo(targetData.Position);
                SetState();
                switch (state)
                {
                    case 0:
                        Projectile.NewProjectile(null, npc.Center, val * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                        skill0 += CooldownOfSkill0 + Main.rand.Next(100);
                        break;
                    case 1:
                        Projectile.NewProjectile(null, npc.Center, val * 9f, 83, 12, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default(Vector2)) * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default(Vector2)) * 8f, 83, 12, 5f, -1, 0f, 0f, 0f);
                        skill0 += CooldownOfSkill0 + Main.rand.Next(80);
                        break;
                    case 2:
                        Projectile.NewProjectile(null, npc.Center, val * 9f, 83, 14, 20f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default(Vector2)) * 10f, 83, 14, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default(Vector2)) * 10f, 83, 14, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.2, default(Vector2)) * 8f, 83, 14, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.2, default(Vector2)) * 8f, 83, 14, 5f, -1, 0f, 0f, 0f);
                        skill0 += CooldownOfSkill0 + Main.rand.Next(30);
                        break;
                    case 3:
                        Projectile.NewProjectile(null, npc.Center, val * 15f, 83, 15, 20f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.1, default(Vector2)) * 15f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.1, default(Vector2)) * 15f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.15, default(Vector2)) * 14f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.15, default(Vector2)) * 14f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.2, default(Vector2)) * 13f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.2, default(Vector2)) * 13f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, 0.25, default(Vector2)) * 12f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center, Terraria.Utils.RotatedBy(val, -0.25, default(Vector2)) * 12f, 83, 15, 5f, -1, 0f, 0f, 0f);
                        skill0 += CooldownOfSkill0;
                        break;
                }
            }
        }

        public override int SetState()
        {
            if (npc.life >= LifeMax * 0.7f)
            {
                if (state == 0)
                {
                    state = 1;
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.4f)
            {
                if (state == 1)
                {
                    state = 2;
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.2f)
            {
                if (state == 2)
                {
                    state = 3;
                }
                return state;
            }
            if (state == 3)
            {
                state = 4;
            }
            return state;
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                global::Challenger.Challenger.SendPlayerText("这么想看清我的卡姿兰大眼是吧", new Color(0, 146, 255), npc.Center + new Vector2(0f, -30f));
            }
        }
    }
}
