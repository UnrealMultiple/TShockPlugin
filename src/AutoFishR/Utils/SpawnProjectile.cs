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
        Projectile.FindBannerToAssociateTo(spawnSource, projectile);
        if (projectile.aiStyle == 1)
            while (projectile.velocity.X >= 16f || projectile.velocity.X <= -16f || projectile.velocity.Y >= 16f ||
                   projectile.velocity.Y < -16f)
            {
                if (projectile.velocity.HasNanOrInf()) break;

                projectile.velocity.X *= 0.97f;
                projectile.velocity.Y *= 0.97f;
            }

        switch (Type)
        {
            case 206:
                projectile.ai[0] = Main.rand.Next(-100, 101) * 0.0005f;
                projectile.ai[1] = Main.rand.Next(-100, 101) * 0.0005f;
                break;
            case 335:
                projectile.ai[1] = Main.rand.Next(4);
                break;
            case 358:
                projectile.ai[1] = Main.rand.Next(10, 31) * 0.1f;
                break;
            case 406:
                projectile.ai[1] = Main.rand.Next(10, 21) * 0.1f;
                break;
            default:
                projectile.ai[0] = ai0;
                projectile.ai[1] = ai1;
                projectile.ai[2] = ai2;
                break;
        }

        if (Type == 434)
        {
            projectile.ai[0] = projectile.position.X;
            projectile.ai[1] = projectile.position.Y;
        }

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

        if (Owner == Main.myPlayer)
        {
            if (ProjectileID.Sets.IsAGolfBall[Type] && Damage <= 0)
            {
                var num3 = 0;
                var num4 = 0;
                var num5 = 99999999;
                for (var j = 999; j > 0; j--)
                    if (Main.projectile[j].active && ProjectileID.Sets.IsAGolfBall[Main.projectile[j].type] &&
                        Main.projectile[j].owner == Owner && Main.projectile[j].damage <= 0)
                    {
                        num3++;
                        if (num5 > Main.projectile[j].timeLeft)
                        {
                            num4 = j;
                            num5 = Main.projectile[j].timeLeft;
                        }
                    }

                if (num3 > 10) Main.projectile[num4].Kill();
            }

            if (Type == 28) projectile.timeLeft = 180;

            if (Type == 516) projectile.timeLeft = 180;

            if (Type == 519) projectile.timeLeft = 180;

            if (Type == 29) projectile.timeLeft = 300;

            if (Type == 470) projectile.timeLeft = 300;

            if (Type == 637) projectile.timeLeft = 300;

            if (Type == 30) projectile.timeLeft = 180;

            if (Type == 517) projectile.timeLeft = 180;

            if (Type == 37) projectile.timeLeft = 180;

            if (Type == 773) projectile.timeLeft = 180;

            if (Type == 75) projectile.timeLeft = 180;

            if (Type == 133) projectile.timeLeft = 180;

            if (Type == 136) projectile.timeLeft = 180;

            if (Type == 139) projectile.timeLeft = 180;

            if (Type == 142) projectile.timeLeft = 180;

            if (Type == 397) projectile.timeLeft = 180;

            if (Type == 419) projectile.timeLeft = 600;

            if (Type == 420) projectile.timeLeft = 600;

            if (Type == 421) projectile.timeLeft = 600;

            if (Type == 422) projectile.timeLeft = 600;

            if (Type == 588) projectile.timeLeft = 180;

            if (Type == 779) projectile.timeLeft = 60;

            if (Type == 783) projectile.timeLeft = 60;

            if (Type == 862 || Type == 863) projectile.timeLeft = 60;

            if (Type == 443) projectile.timeLeft = 300;

            if (Type == 681) projectile.timeLeft = 600;

            if (Type == 684) projectile.timeLeft = 60;

            if (Type == 706) projectile.timeLeft = 120;

            if (Type == 680 && Main.player[projectile.owner].setSquireT2) projectile.penetrate = 7;

            if (Type == 777 || Type == 781 || Type == 794 || Type == 797 || Type == 800 || Type == 785 ||
                Type == 788 || Type == 791 || Type == 903 || Type == 904 || Type == 905 || Type == 906 ||
                Type == 910 || Type == 911)
                projectile.timeLeft = 180;
        }

        if (Type == 249) projectile.frame = Main.rand.Next(5);

        if (Owner == Main.myPlayer) Main.player[Owner].TryUpdateChannel(projectile);

        if (timeLeft > 0) projectile.timeLeft = timeLeft;

        //排除单体召唤物：星尘守卫、星尘龙、沙漠虎、阿比盖尔的弹幕
        if (AFMain.AutoFish.Config.DisabledProjectileIds.Contains(Type))
        {
            timeLeft = 0;
            projectile.frame = 0;
            projectile.timeLeft = -1;
            projectile.active = false;
            return 0;
        }

        projectile.miscText = uuid;
        return num;
    }
}