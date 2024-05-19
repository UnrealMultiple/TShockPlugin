using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace EconomicsAPI.Utils;
public class Projectile
{
    public static int NewProjectile(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int Type, int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
    {
        return NewProjectile(spawnSource, position.X, position.Y, velocity.X, velocity.Y, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
    }

    public static int NewProjectile(IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f)
    {
        int num = 0;
        for (int i = 1000; i >= 0; i--)
        {
            if (!Main.projectile[i].active)
            {
                num = i;
                break;
            }
        }
        Terraria.Projectile projectile = Main.projectile[num];
        projectile.SetDefaults(Type);
        projectile.position.X = X - (projectile.width * 0.5f);
        projectile.position.Y = Y - (projectile.height * 0.5f);
        projectile.owner = Owner;
        projectile.velocity.X = SpeedX;
        projectile.velocity.Y = SpeedY;
        projectile.damage = Damage;
        projectile.knockBack = KnockBack;
        projectile.identity = num;
        projectile.gfxOffY = 0f;
        projectile.stepSpeed = 1f;
        projectile.ai[0] = ai0;
        projectile.ai[1] = ai1;
        projectile.ai[2] = ai2;
        projectile.wet = Collision.WetCollision(projectile.position, projectile.width, projectile.height);
        if (projectile.ignoreWater)
        {
            projectile.wet = false;
        }
        projectile.honeyWet = Collision.honey;
        projectile.shimmerWet = Collision.shimmer;
        Main.projectileIdentity[Owner, num] = num;
        Terraria.Projectile.FindBannerToAssociateTo(spawnSource, projectile);
        if (projectile.aiStyle == 1)
        {
            if (projectile.velocity.HasNanOrInf())
                projectile.velocity = Vector2.One;
            while (projectile.velocity.X >= 16f || projectile.velocity.X <= -16f || projectile.velocity.Y >= 16f || projectile.velocity.Y < -16f)
            {
                projectile.velocity.X *= 0.97f;
                projectile.velocity.Y *= 0.97f;
            }
        }

        if (Type == 434)
        {
            projectile.ai[0] = projectile.position.X;
            projectile.ai[1] = projectile.position.Y;
        }
        if (Type > 0 && Type < ProjectileID.Count)
        {
            if (ProjectileID.Sets.NeedsUUID[Type])
            {
                projectile.projUUID = projectile.identity;
            }
            if (ProjectileID.Sets.StardustDragon[Type])
            {
                int num2 = Main.projectile[(int)projectile.ai[0]].projUUID;
                if (num2 >= 0)
                {
                    projectile.ai[0] = num2;
                }
            }
        }
        if (Type == 249)
        {
            projectile.frame = Main.rand.Next(5);
        }
        if (Owner == Main.myPlayer)
        {
            Main.player[Owner].TryUpdateChannel(projectile);
        }
        return num;
    }
}
