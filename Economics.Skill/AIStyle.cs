using Economics.Skill.Model.Options;
using EconomicsAPI.Extensions;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

public class AIStyle
{
    private static readonly Dictionary<Projectile, AIStyleOption> projectiles = new();

    public static void Set(Projectile projectile, AIStyleOption style)
    {
        if (style.Style < 0)
            return;
        projectiles[projectile] = style;
    }
    public static void AI(Projectile projectile)
    {
        if (projectiles.TryGetValue(projectile, out var style))
        {
             switch (style.Style)
            {
                case 0:
                    Revolve(projectile, style);
                    break;
                case 1:
                    Hover(projectile, style); break;
            }
        }
    }

    public static void Remove()
    {
        for (int i = 0; i < projectiles.Count; i++)
        {
            var (proj, _) = projectiles.ElementAt(i);
            if (!proj.active || proj.timeLeft <= 0)
                projectiles.Remove(proj, out var _);
        }
    }

    public static void Revolve(Projectile projectile, AIStyleOption aIStyleOption)
    {
        var val = (projectile.Center - Main.player[projectile.owner].Center).SafeNormalize(Vector2.UnitY * aIStyleOption.Range * 16);
        var one = val.RotatedBy(1.03, default) * aIStyleOption.Range * 16;
        projectile.Center = Main.player[projectile.owner].Center + one;
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
        var target = projectile.position.FindRangeNPC(1000000f);
        if (Main.time % aIStyleOption.Interval == 0.0 && target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY) * projectile.velocity.Length();
            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(aIStyleOption.Speed), aIStyleOption.ProjID, aIStyleOption.Damage, 10, projectile.owner);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        }
    }

    public static void Hover(Projectile projectile, AIStyleOption aIStyleOption)
    {
        projectile.Center = Main.player[projectile.owner].Center + new Vector2(0, 5 * 16 * -1);
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
        var target = projectile.position.FindRangeNPC(1000000f);
        if (Main.time % aIStyleOption.Interval == 0.0 && target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY) * projectile.velocity.Length();
            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Terraria.Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(aIStyleOption.Speed), aIStyleOption.ProjID, aIStyleOption.Damage, 10, projectile.owner);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        }
    }
}
