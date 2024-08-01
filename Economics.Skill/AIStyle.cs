using System.Collections.Concurrent;
using Economics.Skill.Enumerates;
using Economics.Skill.Model.Options;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

public class AIStyle
{
    private static readonly ConcurrentDictionary<string, (Projectile, AIStyleOption)> projectiles = new();

    private delegate void StyleCall(Projectile projectile, AIStyleOption option);

    private static string UUID = Guid.NewGuid().ToString();

    private static readonly Dictionary<AIStyleType, StyleCall> TypeCall = new Dictionary<AIStyleType, StyleCall>()
    {
        { AIStyleType.Revolve, Revolve },
        { AIStyleType.Hover, Hover },
        { AIStyleType.Trace, Trace }
    };

    public static void Set(Projectile projectile, AIStyleOption style, string guid)
    {
        if (style.Style < 0)
            return;
        projectiles[guid] = (projectile, style);
    }

    public static void AI(Projectile projectile)
    {
        if (projectiles.TryGetValue(projectile.miscText, out var style))
        {
            if (TypeCall.TryGetValue(style.Item2.Style, out var func))
                func(projectile, style.Item2);
        }
    }

    private static void Trace(Projectile projectile, AIStyleOption aIStyleOption)
    {
        var target = projectile.position.FindRangeNPC(aIStyleOption.AttackRange * 16f);
        if (target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY);
            projectile.velocity = speed.ToLenOf(aIStyleOption.Speed);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", projectile.whoAmI);
            if (Math.Abs(target.Distance(projectile.Center)) <= 16f)
                projectiles.Remove(projectile.miscText, out var _);
        }
    }

    public static void Remove()
    {
        foreach (var (npc, _) in projectiles.Where(x => x.Value.Item1 == null || !x.Value.Item1.active).ToList())
            projectiles.Remove(npc, out var _);
    }

    public static void Revolve(Projectile projectile, AIStyleOption aIStyleOption)
    {
        var val = (projectile.Center - Main.player[projectile.owner].Center).SafeNormalize(Vector2.UnitY * aIStyleOption.Range * 16);
        var one = val.RotatedBy(1.03, default) * aIStyleOption.Range * 16;
        projectile.Center = Main.player[projectile.owner].Center + one;
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
        var target = projectile.position.FindRangeNPC(aIStyleOption.AttackRange * 16f);
        if (Main.time % aIStyleOption.Interval == 0.0 && target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY);
            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(aIStyleOption.Speed), aIStyleOption.ProjID, aIStyleOption.Damage, 10, projectile.owner, aIStyleOption.AI[0], aIStyleOption.AI[1], aIStyleOption.AI[2], -1, UUID);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        }
    }

    public static void Hover(Projectile projectile, AIStyleOption aIStyleOption)
    {
        projectile.Center = Main.player[projectile.owner].Center + new Vector2(0, 5 * 16 * -1);
        TSPlayer.All.SendData(PacketTypes.ProjectileNew, null, projectile.whoAmI, 0f, 0f, 0f, 0);
        var target = projectile.position.FindRangeNPC(aIStyleOption.AttackRange * 16f);
        if (Main.time % aIStyleOption.Interval == 0.0 && target != null)
        {
            var speed = projectile.DirectionTo(target.Center).SafeNormalize(-Vector2.UnitY);
            int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(Projectile.GetNoneSource(), projectile.Center, speed.ToLenOf(aIStyleOption.Speed), aIStyleOption.ProjID, aIStyleOption.Damage, 10, projectile.owner, aIStyleOption.AI[0], aIStyleOption.AI[1], aIStyleOption.AI[2], -1, UUID);
            TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
        }
    }
}
