using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using TShockAPI;

namespace Challenger
{
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
            skill0 -= 1f;
            skill1 -= 1f;
            NPCAimedTarget targetData = npc.GetTargetData(true);
            Vector2 val = npc.DirectionTo(targetData.Position);
            SetState();
            if (skill0 < 0f)
            {
                switch (state)
                {
                    case 0:
                        Projectile.NewProjectile(null, npc.Center, val * 13f, 467, 14, 5f, -1, 0f, 0f, 0f);
                        skill0 = CooldownOfSkill0 + Main.rand.Next(51);
                        break;
                    case 1:
                        Projectile.NewProjectile(null, npc.Center, val * 20f, 467, 18, 5f, -1, 0f, 0f, 0f);
                        skill0 = CooldownOfSkill0;
                        break;
                    case 2:
                        Projectile.NewProjectile(null, npc.Center, val * 26f, 467, 22, 5f, -1, 0f, 0f, 0f);
                        skill0 = CooldownOfSkill0 - 20f;
                        break;
                }
            }
            if (skill1 < 0f)
            {
                switch (state)
                {
                    case 0:
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 14, 5f, -1, 0f, 0f, 0f);
                        skill1 = CooldownOfSkill0 + Main.rand.Next(51);
                        break;
                    case 1:
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 18, 5f, -1, 0f, 0f, 0f);
                        skill1 = CooldownOfSkill0;
                        break;
                    case 2:
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        Projectile.NewProjectile(null, npc.Center + new Vector2(0f, Main.rand.Next(-200, 200)), (val + Terraria.Utils.RotateRandom(Vector2.One, Math.PI) * 0.2f) * 10f, 811, 20, 5f, -1, 0f, 0f, 0f);
                        skill1 = CooldownOfSkill0;
                        break;
                }
            }
        }

        public override int SetState()
        {
            if (npc.life >= LifeMax * 0.6f)
            {
                if (state == 0)
                {
                    state = 1;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("罪恶血祭召唤远古守卫", new Color(255, 77, 0));
                    }
                }
                return state;
            }
            if (npc.life >= LifeMax * 0.3f)
            {
                if (state == 1)
                {
                    state = 2;
                    if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                    {
                        TSPlayer.All.SendMessage("付出代价吧！", new Color(255, 77, 0));
                    }
                }
                return state;
            }
            if (state == 2)
            {
                state = 3;
                if (global::Challenger.Challenger.config.EnableBroadcastConsumptionMode)
                {
                    TSPlayer.All.SendMessage("速度与激情", new Color(255, 77, 0));
                }
            }
            return state;
        }

        public override void OnHurtPlayers(GetDataHandlers.PlayerDamageEventArgs e)
        {
            if (global::Challenger.Challenger.config.EnableConsumptionMode)
            {
                global::Challenger.Challenger.SendPlayerText("咬碎你", new Color(0, 146, 255), npc.Center);
            }
        }
    }
}
