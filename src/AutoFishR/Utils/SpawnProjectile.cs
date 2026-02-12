using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace AutoFish.Utils;

public class SpawnProjectile
{
    public static int NewProjectile(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int Type,
        int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f,
        int timeLeft = -1, string uuid = "")
    {
        return NewProjectile(spawnSource, position.X, position.Y, velocity.X, velocity.Y, Type, Damage, KnockBack,
            Owner, ai0, ai1, ai2, timeLeft, uuid);
    }

    //所有浮漂：360~366 381 382 普通浮漂 760 775 特殊的 986~993 发光的
    public static int NewProjectile(IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY,
        int Type, int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0,
        int timeLeft = -1, string uuid = "")
    {
        if (Owner == -1) Owner = Main.myPlayer;

        var num = 1000;
        for (var i = 999; i > 0; i--)
            if (!Main.projectile[i].active)
            {
                num = i;
                break;
            }

        if (num == 1000) num = Projectile.FindOldestProjectile();

        var projectile = Main.projectile[num];
        projectile.SetDefaults(Type);
        projectile.position.X = X;
        projectile.position.Y = Y;
        projectile.owner = Owner;
        projectile.velocity.X = SpeedX;
        projectile.velocity.Y = SpeedY;
        projectile.damage = Damage;
        projectile.knockBack = KnockBack;
        projectile.identity = num;
        projectile.gfxOffY = 0f;
        projectile.stepSpeed = 1f;
        projectile.wet = Collision.WetCollision(projectile.position, projectile.width, projectile.height);
        if (projectile.ignoreWater) projectile.wet = false;

        projectile.honeyWet = Collision.honey;
        projectile.shimmerWet = Collision.shimmer;
        Main.projectileIdentity[Owner, num] = num;
        projectile.FindBannerToAssociateTo(spawnSource);
        if (projectile.aiStyle != 61) return 0;
        projectile.ai[0] = ai0;
        projectile.ai[1] = ai1;
        projectile.ai[2] = ai2;

        if (Type > 0 && Type < ProjectileID.Count)
        {
            if (ProjectileID.Sets.NeedsUUID[Type]) projectile.projUUID = projectile.identity;

            if (ProjectileID.Sets.StardustDragon[Type])
            {
                var num2 = Main.projectile[(int)projectile.ai[0]].projUUID;
                if (num2 >= 0) projectile.ai[0] = num2;
            }
        }

        if (Main.netMode != 0 && Owner == Main.myPlayer) NetMessage.SendData(27, -1, -1, null, num);

        if (Owner == Main.myPlayer) Main.player[Owner].TryUpdateChannel(projectile);

        if (timeLeft > 0) projectile.timeLeft = timeLeft;

        projectile.miscText = uuid;
        return num;
    }
}