using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using static CustomMonster.TestPlugin;

namespace CustomMonster;

public class Sundry
{
    #region 发射弹幕方法
    public static void LaunchProjectile(List<ProjGroup> Projectiles, NPC npc, LNPC lnpc)
    {
        foreach (var proj in Projectiles)
        {
            if (proj.ProjectileID <= 0)
            {
                continue;
            }
            var num = 0f;
            var num2 = 0f;
            if (!proj.StartPositionZero)
            {
                num = npc.Center.X;
                num2 = npc.Center.Y;
            }
            if (proj.LockRange > 0 || proj.LockRange == -1)
            {
                var list = new List<int>();
                if (proj.LockRange == -1)
                {
                    list.Add(-1);
                }
                else
                {
                    for (var i = 0; i < proj.MaxLocks && i <= TShock.Utils.GetActivePlayerCount(); i++)
                    {
                        var num3 = -1;
                        var num4 = -1f;
                        int? num5 = null;
                        int? num6 = null;
                        int? num7 = null;
                        for (var j = 0; j < 255; j++)
                        {
                            if (list.Contains(j) || Main.player[j] == null || !Main.player[j].active || Main.player[j].dead || (proj.AttackTargetOnly && j != npc.target))
                            {
                                continue;
                            }
                            if (proj.FanShapedLock)
                            {
                                if (proj.FanShapedHalfAngle > 180)
                                {
                                    proj.FanShapedHalfAngle = 180;
                                }
                                if (proj.FanShapedHalfAngle < 1)
                                {
                                    proj.FanShapedHalfAngle = 1;
                                }
                                var num8 = Main.player[j].Center.X - num;
                                var num9 = Main.player[j].Center.Y - num2;
                                if ((num8 != 0f || num9 != 0f) && (npc.direction != 0 || npc.directionY != 0))
                                {
                                    var num10 = Math.Atan2(num9, num8) * 180.0 / Math.PI;
                                    var num11 = Math.Atan2(npc.directionY, npc.direction) * 180.0 / Math.PI;
                                    var num12 = num11 + proj.FanShapedHalfAngle;
                                    var num13 = num11 - proj.FanShapedHalfAngle;
                                    if (num12 > 360.0)
                                    {
                                        num12 -= 360.0;
                                    }
                                    if (num13 < 0.0)
                                    {
                                        num13 += 360.0;
                                    }
                                    if (num10 > num12 && num10 < num13)
                                    {
                                        continue;
                                    }
                                }
                            }
                            var num14 = Math.Abs(Main.player[j].Center.X - num + Math.Abs(Main.player[j].Center.Y - num2));
                            if ((num4 == -1f || num14 < num4) && (!proj.IncludeHatred || !num6.HasValue || (proj.ReverseHatredLock ? (Main.player[j].aggro < num6) : (Main.player[j].aggro > num6))) && (!proj.LockLowLife || !num5.HasValue || (proj.ReverseLifeLock ? (Main.player[j].statLife > num5) : (Main.player[j].statLife < num5))) && (!proj.LockLowDefense || !num7.HasValue || (proj.ReverseDefenseLock ? (Main.player[j].statDefense > num7) : (Main.player[j].statDefense < num7))))
                            {
                                if (proj.IncludeHatred)
                                {
                                    num6 = Main.player[j].aggro;
                                }
                                if (proj.LockLowLife)
                                {
                                    num5 = Main.player[j].statLife;
                                }
                                if (proj.LockLowDefense)
                                {
                                    num7 = Main.player[j].statDefense;
                                }
                                num4 = num14;
                                num3 = j;
                            }
                        }
                        if (num3 != -1)
                        {
                            list.Add(num3);
                        }
                    }
                }
                foreach (var item in list)
                {
                    float num15;
                    float num16;
                    if (item == -1)
                    {
                        num15 = num;
                        num16 = num2;
                    }
                    else
                    {
                        var val = Main.player[item];
                        if (val == null || val.dead || val.statLife < 1)
                        {
                            continue;
                        }
                        if (proj.LockRange > 350)
                        {
                            proj.LockRange = 350;
                        }
                        if (!WithinRange(num, num2, val.Center, proj.LockRange << 4))
                        {
                            continue;
                        }
                        num15 = val.Center.X;
                        num16 = val.Center.Y;
                    }
                    var num17 = proj.MonsterFacingXOffsetCorrection * npc.direction;
                    var num18 = proj.MonsterFacingYOffsetCorrection * npc.directionY;
                    var ai = proj.Ai0 + (lnpc.getMarkers(proj.IndicatorCountInjectAi0Name) * proj.IndicatorCountInjectAi0Factor);
                    var ai2 = proj.Ai1 + (lnpc.getMarkers(proj.IndicatorCountInjectAi1Name) * proj.IndicatorCountInjectAi1Factor);
                    var ai3 = proj.Ai2 + (lnpc.getMarkers(proj.IndicatorCountInjectAi2Name) * proj.IndicatorCountInjectAi2Factor);
                    var num19 = proj.LockSpeed + (lnpc.getMarkers(proj.InjectIndicatorNameForLockSpeed) * proj.InjectIndicatorFactorForLockSpeed);
                    float num20;
                    float num21;
                    double num25;
                    float num22;
                    float num23;
                    if (proj.UseLockAsPoint)
                    {
                        num20 = num15;
                        num21 = num16;
                        num22 = proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
                        num23 = proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
                        var num24 = (float) Math.Sqrt(Math.Pow(num22, 2.0) + Math.Pow(num23, 2.0));
                        num25 = Math.Atan2(num23, num22) * 180.0 / Math.PI;
                        var num26 = proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor);
                        if (num26 != 0f)
                        {
                            num25 += (double) num26;
                            num22 = (float) ((double) num24 * Math.Cos(num25 * Math.PI / 180.0));
                            num23 = (float) ((double) num24 * Math.Sin(num25 * Math.PI / 180.0));
                        }
                    }
                    else
                    {
                        num20 = num;
                        num21 = num2;
                        num22 = num15 - (num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor));
                        num23 = num16 - (num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor));
                        if (num22 == 0f && num23 == 0f)
                        {
                            num22 = 1f;
                        }
                        num25 = Math.Atan2(num23, num22) * 180.0 / Math.PI;
                        num25 += (double) (proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor));
                        num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                        num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                        num22 += proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
                        num23 += proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
                    }
                    if (proj.UseProjectileAsPosition)
                    {
                        var num27 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                        var num28 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                        num22 = num15 - (num27 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor));
                        num23 = num16 - (num28 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor));
                        if (num22 == 0f && num23 == 0f)
                        {
                            num22 = 1f;
                        }
                        num25 = Math.Atan2(num23, num22) * 180.0 / Math.PI;
                        num25 += (double) (proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor));
                        num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                        num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                        num22 += proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
                        num23 += proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
                    }
                    if (proj.SpeedInjectsIntoAi0)
                    {
                        ai = (float) Math.Atan2(num23, num22);
                        num22 = proj.XSpeedAfterAi0Injection;
                        num23 = proj.YSpeedAfterAi0Injection;
                    }
                    var num29 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                    var num30 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                    if (list.IndexOf(item) == 0)
                    {
                        if (proj.InjectIndicatorNameForInitialProjectileX != "")
                        {
                            lnpc.setMarkers(proj.InjectIndicatorNameForInitialProjectileX, (int) num29, reset: false);
                        }
                        if (proj.InjectIndicatorNameForInitialProjectileY != "")
                        {
                            lnpc.setMarkers(proj.InjectIndicatorNameForInitialProjectileY, (int) num30, reset: false);
                        }
                        if (proj.InjectIndicatorNameForLockingPlayerIndex != "")
                        {
                            lnpc.setMarkers(proj.InjectIndicatorNameForLockingPlayerIndex, item, reset: false);
                        }
                    }
                    if (!proj.DoNotShootOriginal)
                    {
                        if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                        {
                            NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                        }
                        else if (proj.SummonMonsterAtProjectilePoint != 0)
                        {
                            LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                        }
                    }
                    if (proj.TeleportMonsterFromSpawnPoint)
                    {
                        npc.Teleport(new Vector2(num29, num30), proj.TeleportTypeFromSpawnPoint, proj.TeleportInfoFromSpawnPoint);
                    }
                    var num31 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShots) * proj.InjectIndicatorFactorForDifferentialShots);
                    var num32 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialAngle) * proj.InjectIndicatorFactorForDifferentialAngle);
                    var num33 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialRadius) * proj.InjectIndicatorFactorForDifferentialRadius);
                    var num34 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShotCount) * proj.InjectIndicatorFactorForDifferentialShotCount);
                    var num35 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShotAngle) * proj.InjectIndicatorFactorForDifferentialShotAngle);
                    var num36 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedShotCount) * proj.InjectIndicatorFactorForDifferentiatedShotCount);
                    var num37 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedOffsetX) * proj.InjectIndicatorFactorForDifferentiatedOffsetX);
                    var num38 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedOffsetY) * proj.InjectIndicatorFactorForDifferentiatedOffsetY);
                    if (proj.DifferentialShots + num31 > 0 && proj.DifferentialAngle + num32 != 0 && proj.DifferentialRadius + num33 > 0)
                    {
                        var num39 = proj.DifferentialStartAngle + (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialStartAngle) * proj.InjectIndicatorFactorForDifferentialStartAngle);
                        for (var k = 0; k < proj.DifferentialShots + num31; k++)
                        {
                            num39 += proj.DifferentialAngle + num32;
                            var num40 = (float) ((proj.DifferentialRadius + num33) * Math.Cos(num39 * Math.PI / 180.0));
                            var num41 = (float) ((proj.DifferentialRadius + num33) * Math.Sin(num39 * Math.PI / 180.0));
                            if (proj.UseProjectileAsPosition)
                            {
                                var num42 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num40 + num17;
                                var num43 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num41 + num18;
                                num22 = num15 - (num42 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor));
                                num23 = num16 - (num43 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor));
                                if (num22 == 0f && num23 == 0f)
                                {
                                    num22 = 1f;
                                }
                                num25 = Math.Atan2(num23, num22) * 180.0 / Math.PI;
                                num25 += (double) (proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor));
                                num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                                num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                                num22 += proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
                                num23 += proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
                            }
                            if (proj.SpeedInjectsIntoAi0)
                            {
                                ai = (float) Math.Atan2(num23, num22);
                                num22 = proj.XSpeedAfterAi0Injection;
                                num23 = proj.YSpeedAfterAi0Injection;
                            }
                            num29 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num40 + num17;
                            num30 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num41 + num18;
                            if (!proj.DoNotShootDifferential)
                            {
                                if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                {
                                    NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                                }
                                else if (proj.SummonMonsterAtProjectilePoint != 0)
                                {
                                    LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                                }
                            }
                            if (proj.DifferentialShotCount + num34 > 0 && proj.DifferentialShotAngle + num35 != 0f)
                            {
                                for (var l = 0; l < proj.DifferentialShotCount + num34; l++)
                                {
                                    num25 += (double) (proj.DifferentialShotAngle + num35);
                                    num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                                    num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                                    if (proj.SpeedInjectsIntoAi0)
                                    {
                                        ai = (float) Math.Atan2(num23, num22);
                                        num22 = proj.XSpeedAfterAi0Injection;
                                        num23 = proj.YSpeedAfterAi0Injection;
                                    }
                                    num29 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                                    num30 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                                    if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                    {
                                        NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                                    }
                                    else if (proj.SummonMonsterAtProjectilePoint != 0)
                                    {
                                        LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                                    }
                                    if (proj.DifferentiatedShotCount + num36 <= 0 || (proj.DifferentiatedOffsetX + num37 == 0f && proj.DifferentiatedOffsetY + num38 == 0f))
                                    {
                                        continue;
                                    }
                                    var num44 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                                    var num45 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                                    for (var m = 0; m < proj.DifferentiatedShotCount + num36; m++)
                                    {
                                        num44 += proj.DifferentiatedOffsetX + num37;
                                        num45 += proj.DifferentiatedOffsetY + num38;
                                        num29 = num44;
                                        num30 = num45;
                                        if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                        {
                                            NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                                        }
                                        else if (proj.SummonMonsterAtProjectilePoint != 0)
                                        {
                                            LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (proj.DifferentiatedShotCount + num36 <= 0 || (proj.DifferentiatedOffsetX + num37 == 0f && proj.DifferentiatedOffsetY + num38 == 0f))
                                {
                                    continue;
                                }
                                var num46 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                                var num47 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                                for (var n = 0; n < proj.DifferentiatedShotCount; n++)
                                {
                                    num46 += proj.DifferentiatedOffsetX + num37;
                                    num47 += proj.DifferentiatedOffsetY + num38;
                                    if (proj.UseProjectileAsPosition)
                                    {
                                        num22 = num15 - (num46 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor));
                                        num23 = num16 - (num47 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor));
                                        if (num22 == 0f && num23 == 0f)
                                        {
                                            num22 = 1f;
                                        }
                                        num25 = Math.Atan2(num23, num22) * 180.0 / Math.PI;
                                        num25 += (double) (proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor));
                                        num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                                        num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                                        num22 += proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
                                        num23 += proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
                                        if (proj.SpeedInjectsIntoAi0)
                                        {
                                            ai = (float) Math.Atan2(num23, num22);
                                            num22 = proj.XSpeedAfterAi0Injection;
                                            num23 = proj.YSpeedAfterAi0Injection;
                                        }
                                    }
                                    num29 = num46;
                                    num30 = num47;
                                    if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                    {
                                        NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                                    }
                                    else if (proj.SummonMonsterAtProjectilePoint != 0)
                                    {
                                        LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                                    }
                                }
                            }
                        }
                    }
                    else if (proj.DifferentialShotCount + num34 > 0 && proj.DifferentialShotAngle + num35 != 0f)
                    {
                        for (var num48 = 0; num48 < proj.DifferentialShotCount + num34; num48++)
                        {
                            num25 += (double) (proj.DifferentialShotAngle + num35);
                            num22 = (float) ((double) num19 * Math.Cos(num25 * Math.PI / 180.0));
                            num23 = (float) ((double) num19 * Math.Sin(num25 * Math.PI / 180.0));
                            if (proj.SpeedInjectsIntoAi0)
                            {
                                ai = (float) Math.Atan2(num23, num22);
                                num22 = proj.XSpeedAfterAi0Injection;
                                num23 = proj.YSpeedAfterAi0Injection;
                            }
                            num29 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                            num30 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                            if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                            {
                                NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                            }
                            else if (proj.SummonMonsterAtProjectilePoint != 0)
                            {
                                LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                            }
                            if (proj.DifferentiatedShotCount + num36 <= 0 || (proj.DifferentiatedOffsetX + num37 == 0f && proj.DifferentiatedOffsetY + num38 == 0f))
                            {
                                continue;
                            }
                            var num49 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                            var num50 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                            for (var num51 = 0; num51 < proj.DifferentiatedShotCount + num36; num51++)
                            {
                                num49 += proj.DifferentiatedOffsetX + num37;
                                num50 += proj.DifferentiatedOffsetY + num38;
                                num29 = num49;
                                num30 = num50;
                                if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                {
                                    NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                                }
                                else if (proj.SummonMonsterAtProjectilePoint != 0)
                                {
                                    LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (proj.DifferentiatedShotCount + num36 <= 0 || (proj.DifferentiatedOffsetX + num37 == 0f && proj.DifferentiatedOffsetY + num38 == 0f))
                        {
                            continue;
                        }
                        var num52 = num20 + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num17;
                        var num53 = num21 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num18;
                        for (var num54 = 0; num54 < proj.DifferentiatedShotCount + num36; num54++)
                        {
                            num52 += proj.DifferentiatedOffsetX + num37;
                            num53 += proj.DifferentiatedOffsetY + num38;
                            num29 = num52;
                            num30 = num53;
                            if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                            {
                                NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num29, num30, num22, num23, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai, ai2, ai3, proj.Duration);
                            }
                            else if (proj.SummonMonsterAtProjectilePoint != 0)
                            {
                                LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num29, num30);
                            }
                        }
                    }
                }
                continue;
            }
            var num55 = proj.XSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectXSpeedName) * proj.IndicatorCountInjectXSpeedFactor) + (proj.MonsterFacingXSpeedCorrection * npc.direction);
            var num56 = proj.YSpeed + (lnpc.getMarkers(proj.IndicatorCountInjectYSpeedName) * proj.IndicatorCountInjectYSpeedFactor) + (proj.MonsterFacingYSpeedCorrection * npc.directionY);
            var num57 = (float) Math.Sqrt(Math.Pow(num55, 2.0) + Math.Pow(num56, 2.0));
            var num58 = Math.Atan2(num56, num55) * 180.0 / Math.PI;
            var num59 = proj.MonsterFacingXOffsetCorrection * npc.direction;
            var num60 = proj.MonsterFacingYOffsetCorrection * npc.directionY;
            var ai4 = proj.Ai0 + (lnpc.getMarkers(proj.IndicatorCountInjectAi0Name) * proj.IndicatorCountInjectAi0Factor);
            var ai5 = proj.Ai1 + (lnpc.getMarkers(proj.IndicatorCountInjectAi1Name) * proj.IndicatorCountInjectAi1Factor);
            var ai6 = proj.Ai2 + (lnpc.getMarkers(proj.IndicatorCountInjectAi2Name) * proj.IndicatorCountInjectAi2Factor);
            var num61 = proj.AngleOffset + (lnpc.getMarkers(proj.IndicatorCountInjectAngleName) * proj.IndicatorCountInjectAngleFactor);
            if (num61 != 0f)
            {
                num58 += (double) num61;
                num55 = (float) ((double) num57 * Math.Cos(num58 * Math.PI / 180.0));
                num56 = (float) ((double) num57 * Math.Sin(num58 * Math.PI / 180.0));
            }
            if (proj.SpeedInjectsIntoAi0)
            {
                ai4 = (float) Math.Atan2(num56, num55);
                num55 = proj.XSpeedAfterAi0Injection;
                num56 = proj.YSpeedAfterAi0Injection;
            }
            var num62 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
            var num63 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
            if (proj.InjectIndicatorNameForInitialProjectileX != "")
            {
                lnpc.setMarkers(proj.InjectIndicatorNameForInitialProjectileX, (int) num62, reset: false);
            }
            if (proj.InjectIndicatorNameForInitialProjectileY != "")
            {
                lnpc.setMarkers(proj.InjectIndicatorNameForInitialProjectileY, (int) num63, reset: false);
            }
            if (!proj.DoNotShootOriginal)
            {
                if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                {
                    NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                }
                else if (proj.SummonMonsterAtProjectilePoint != 0)
                {
                    LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                }
            }
            if (proj.TeleportMonsterFromSpawnPoint)
            {
                npc.Teleport(new Vector2(num62, num63), proj.TeleportTypeFromSpawnPoint, proj.TeleportInfoFromSpawnPoint);
            }
            var num64 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShots) * proj.InjectIndicatorFactorForDifferentialShots);
            var num65 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialAngle) * proj.InjectIndicatorFactorForDifferentialAngle);
            var num66 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialRadius) * proj.InjectIndicatorFactorForDifferentialRadius);
            var num67 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShotCount) * proj.InjectIndicatorFactorForDifferentialShotCount);
            var num68 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialShotAngle) * proj.InjectIndicatorFactorForDifferentialShotAngle);
            var num69 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedShotCount) * proj.InjectIndicatorFactorForDifferentiatedShotCount);
            var num70 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedOffsetX) * proj.InjectIndicatorFactorForDifferentiatedOffsetX);
            var num71 = (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentiatedOffsetY) * proj.InjectIndicatorFactorForDifferentiatedOffsetY);
            if (proj.DifferentialShots + num64 > 0 && proj.DifferentialAngle + num65 != 0 && proj.DifferentialRadius + num66 > 0)
            {
                var num72 = proj.DifferentialStartAngle + (int) (lnpc.getMarkers(proj.InjectIndicatorNameForDifferentialStartAngle) * proj.InjectIndicatorFactorForDifferentialStartAngle);
                for (var num73 = 0; num73 < proj.DifferentialShots + num64; num73++)
                {
                    num72 += proj.DifferentialAngle + num65;
                    var num74 = (float) ((proj.DifferentialRadius + num66) * Math.Cos(num72 * Math.PI / 180.0));
                    var num75 = (float) ((proj.DifferentialRadius + num66) * Math.Sin(num72 * Math.PI / 180.0));
                    num62 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num74 + num59;
                    num63 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num75 + num60;
                    if (!proj.DoNotShootDifferential)
                    {
                        if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                        {
                            NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                        }
                        else if (proj.SummonMonsterAtProjectilePoint != 0)
                        {
                            LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                        }
                    }
                    if (proj.DifferentialShotCount + num67 > 0 && proj.DifferentialShotAngle + num68 != 0f)
                    {
                        for (var num76 = 0; num76 < proj.DifferentialShotCount + num67; num76++)
                        {
                            num58 += (double) (proj.DifferentialShotAngle + num68);
                            num55 = (float) ((double) num57 * Math.Cos(num58 * Math.PI / 180.0));
                            num56 = (float) ((double) num57 * Math.Sin(num58 * Math.PI / 180.0));
                            if (proj.SpeedInjectsIntoAi0)
                            {
                                ai4 = (float) Math.Atan2(num56, num55);
                                num55 = proj.XSpeedAfterAi0Injection;
                                num56 = proj.YSpeedAfterAi0Injection;
                            }
                            num62 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                            num63 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                            if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                            {
                                NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                            }
                            else if (proj.SummonMonsterAtProjectilePoint != 0)
                            {
                                LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                            }
                            if (proj.DifferentiatedShotCount + num69 <= 0 || (proj.DifferentiatedOffsetX + num70 == 0f && proj.DifferentiatedOffsetY + num71 == 0f))
                            {
                                continue;
                            }
                            var num77 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                            var num78 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                            for (var num79 = 0; num79 < proj.DifferentiatedShotCount + num69; num79++)
                            {
                                num77 += proj.DifferentiatedOffsetX + num70;
                                num78 += proj.DifferentiatedOffsetY + num71;
                                num62 = num77;
                                num63 = num78;
                                if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                                {
                                    NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                                }
                                else if (proj.SummonMonsterAtProjectilePoint != 0)
                                {
                                    LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (proj.DifferentiatedShotCount + num69 <= 0 || (proj.DifferentiatedOffsetX + num70 == 0f && proj.DifferentiatedOffsetY + num71 == 0f))
                        {
                            continue;
                        }
                        var num80 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                        var num81 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                        for (var num82 = 0; num82 < proj.DifferentiatedShotCount + num69; num82++)
                        {
                            num80 += proj.DifferentiatedOffsetX + num70;
                            num81 += proj.DifferentiatedOffsetY + num71;
                            num62 = num80;
                            num63 = num81;
                            if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                            {
                                NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                            }
                            else if (proj.SummonMonsterAtProjectilePoint != 0)
                            {
                                LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                            }
                        }
                    }
                }
            }
            else if (proj.DifferentialShotCount + num67 > 0 && proj.DifferentialShotAngle + num68 != 0f)
            {
                for (var num83 = 0; num83 < proj.DifferentialShotCount + num67; num83++)
                {
                    num58 += (double) (proj.DifferentialShotAngle + num68);
                    num55 = (float) ((double) num57 * Math.Cos(num58 * Math.PI / 180.0));
                    num56 = (float) ((double) num57 * Math.Sin(num58 * Math.PI / 180.0));
                    if (proj.SpeedInjectsIntoAi0)
                    {
                        ai4 = (float) Math.Atan2(num56, num55);
                        num55 = proj.XSpeedAfterAi0Injection;
                        num56 = proj.YSpeedAfterAi0Injection;
                    }
                    num62 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                    num63 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                    if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                    {
                        NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                    }
                    else if (proj.SummonMonsterAtProjectilePoint != 0)
                    {
                        LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                    }
                    if (proj.DifferentiatedShotCount + num69 <= 0 || (proj.DifferentiatedOffsetX + num70 == 0f && proj.DifferentiatedOffsetY + num71 == 0f))
                    {
                        continue;
                    }
                    var num84 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                    var num85 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                    for (var num86 = 0; num86 < proj.DifferentiatedShotCount + num69; num86++)
                    {
                        num84 += proj.DifferentiatedOffsetX + num70;
                        num85 += proj.DifferentiatedOffsetY + num71;
                        num62 = num84;
                        num63 = num85;
                        if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                        {
                            NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                        }
                        else if (proj.SummonMonsterAtProjectilePoint != 0)
                        {
                            LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                        }
                    }
                }
            }
            else
            {
                if (proj.DifferentiatedShotCount + num69 <= 0 || (proj.DifferentiatedOffsetX + num70 == 0f && proj.DifferentiatedOffsetY + num71 == 0f))
                {
                    continue;
                }
                var num87 = num + proj.XOffset + (lnpc.getMarkers(proj.IndicatorCountInjectXOffsetName) * proj.IndicatorCountInjectXOffsetFactor) + num59;
                var num88 = num2 + proj.YOffset + (lnpc.getMarkers(proj.IndicatorCountInjectYOffsetName) * proj.IndicatorCountInjectYOffsetFactor) + num60;
                for (var num89 = 0; num89 < proj.DifferentiatedShotCount + num69; num89++)
                {
                    num87 += proj.DifferentiatedOffsetX + num70;
                    num88 += proj.DifferentiatedOffsetY + num71;
                    num62 = num87;
                    num63 = num88;
                    if (proj.SummonMonsterAtProjectilePoint == 0 || !proj.SummonMonsterAtProjectilePointWithoutProjectile)
                    {
                        NewProjectile(npc.whoAmI, proj.Sign, Terraria.Projectile.GetNoneSource(), num62, num63, num55, num56, proj.ProjectileID, proj.Damage, proj.Knockback, Main.myPlayer, ai4, ai5, ai6, proj.Duration);
                    }
                    else if (proj.SummonMonsterAtProjectilePoint != 0)
                    {
                        LaunchProjectileSpawnNPC(proj.SummonMonsterAtProjectilePoint, num62, num63);
                    }
                }
            }
        }
    }
    #endregion

    #region 发射弹幕生成NPC方法
    public static void LaunchProjectileSpawnNPC(int npcID, float X, float Y)
    {
        var nPCById = TShock.Utils.GetNPCById(npcID);
        var num = (int) X >> 4;
        var num2 = (int) Y >> 4;
        if (nPCById != null && nPCById.type != 113 && nPCById.type != 0 && nPCById.type < NPCID.Count)
        {
            TSPlayer.Server.SpawnNPC(nPCById.type, nPCById.FullName, 1, num, num2, 1, 1);
        }
    }
    #endregion

    #region 清理弹幕方法
    public static int NewProjectile(int useIndex, string notes, IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = -1, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, int timeLeft = -1)
    {
        var num = Projectile.NewProjectile(spawnSource, X, Y, SpeedX, SpeedY, Type, Damage, KnockBack, Owner, ai0, ai1, ai2);
        if (timeLeft == 0)
        {
            Main.projectile[num].Kill();
        }
        else if (timeLeft > 0)
        {
            Main.projectile[num].timeLeft = timeLeft;
        }
        if (timeLeft != 0)
        {
            addPrjsOfUse(num, useIndex, Type, notes);
        }
        return num;
    }
    #endregion

    #region 伤害怪物方法
    public static void HurtMonster(List<MonsterStrikeGroup> Hmonster, NPC npc)
    {
        var lNPC = LNpcs![npc.whoAmI];
        if (lNPC == null || lNPC.Config == null)
        {
            return;
        }
        foreach (var item in Hmonster)
        {
            if (item.NPCID == 0 || item.NPCID == 488)
            {
                continue;
            }
            var CauseDamages = item.CauseDamage;
            CauseDamages += (int) (lNPC.getMarkers(item.NumberOfIndicatorsInjectedCausingDamageName) * item.DamageCoefficientCausedByInjectionOfIndicatorQuantity);
            if (CauseDamages == 0 && !item.DirectClear)
            {
                continue;
            }
            for (var i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] == null || !Main.npc[i].active || Main.npc[i].netID != item.NPCID || Main.npc[i].whoAmI == npc.whoAmI || (item.Range > 0 && !npc.WithinRange(Main.npc[i].Center, item.Range << 4)) || (item.Indicator != null && LNpcs[Main.npc[i].whoAmI] != null && !LNpcs[Main.npc[i].whoAmI].haveMarkers(item.Indicator, npc)) || (item.CheckSign != "" && LNpcs[Main.npc[i].whoAmI] != null && LNpcs[Main.npc[i].whoAmI].Config != null && LNpcs[Main.npc[i].whoAmI].Config!.Sign != item.CheckSign))
                {
                    continue;
                }
                if (item.DirectClear)
                {
                    Main.npc[i] = new NPC();
                    NetMessage.SendData(23, -1, -1, NetworkText.Empty, i, 0f, 0f, 0f, 0, 0, 0);
                }
                else if (item.DirectDamage)
                {
                    var obj = Main.npc[i];
                    obj.life -= CauseDamages;
                    if (Main.npc[i].life <= 0)
                    {
                        Main.npc[i].life = 1;
                    }
                    if (Main.npc[i].life > Main.npc[i].lifeMax)
                    {
                        Main.npc[i].life = Main.npc[i].lifeMax;
                    }
                    if (CauseDamages < 0)
                    {
                        Main.npc[i].HealEffect(Math.Abs(CauseDamages), true);
                    }
                    Main.npc[i].netUpdate = true;
                }
                else
                {
                    TSPlayer.Server.StrikeNPC(i, CauseDamages, 0f, 0);
                }
            }
        }
    }
    #endregion

    #region 设置怪物标识方法
    public static void SetMonsterMarkers(List<IndicatorModifyGroup> Hmonster, NPC npc, ref Random rd)
    {
        var lNPC = LNpcs![npc.whoAmI];
        if (lNPC == null || lNPC.Config == null)
        {
            return;
        }
        foreach (var item in Hmonster)
        {
            if (item.NPCID == 0 || item.NPCID == 488 || item.IndicatorModify == null || item.IndicatorModify.Count < 1)
            {
                continue;
            }
            for (var i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i] == null || !Main.npc[i].active || Main.npc[i].netID != item.NPCID || Main.npc[i].whoAmI == npc.whoAmI || (item.Range > 0 && !npc.WithinRange(Main.npc[i].Center, item.Range << 4)) || (item.Indicator != null && LNpcs[Main.npc[i].whoAmI] != null && !LNpcs[Main.npc[i].whoAmI].haveMarkers(item.Indicator, npc)) || (item.CheckSign != "" && LNpcs[Main.npc[i].whoAmI] != null && LNpcs[Main.npc[i].whoAmI].Config != null && LNpcs[Main.npc[i].whoAmI].Config!.Sign != item.CheckSign) || item.IndicatorModify == null || LNpcs[Main.npc[i].whoAmI] == null || LNpcs[Main.npc[i].whoAmI].Config == null)
                {
                    continue;
                }
                foreach (var item2 in item.IndicatorModify)
                {
                    LNpcs[Main.npc[i].whoAmI].setMarkers(item2.IndName, item2.IndStack, item2.Clear, item2.InjectionStackName, item2.InjectionStackRatio, item2.InjectionStackOperator, item2.RandomSmall, item2.RandomBig, ref rd, npc);
                }
            }
        }
    }
    #endregion

    #region 拉取玩家方法
    public static void PullTP(TSPlayer user, float x, float y, int r)
    {
        if (r == 0)
        {
            user.Teleport(x, y, 1);
            return;
        }
        var x2 = user.TPlayer.Center.X;
        var y2 = user.TPlayer.Center.Y;
        x2 -= x;
        y2 -= y;
        if (x2 != 0f || y2 != 0f)
        {
            var num = Math.Atan2(y2, x2) * 180.0 / Math.PI;
            x2 = (float) (r * Math.Cos(num * Math.PI / 180.0));
            y2 = (float) (r * Math.Sin(num * Math.PI / 180.0));
            x2 += x;
            y2 += y;
            user.Teleport(x2, y2, 1);
        }
    }
    #endregion

    #region 将指定玩家推开一定的距离方法
    public static void UserRepel(TSPlayer user, float x, float y, int r)
    {
        var x2 = user.TPlayer.Center.X;
        var y2 = user.TPlayer.Center.Y;
        x2 -= x;
        y2 -= y;
        if (x2 != 0f || y2 != 0f)
        {
            var num = Math.Atan2(y2, x2) * 180.0 / Math.PI;
            x2 = (float) (r * Math.Cos(num * Math.PI / 180.0));
            y2 = (float) (r * Math.Sin(num * Math.PI / 180.0));
            user.TPlayer.velocity = new Vector2(x2, y2);
            NetMessage.SendData(13, -1, -1, NetworkText.Empty, user.Index, 0f, 0f, 0f, 0, 0, 0);
        }
    }
    #endregion

    #region 执行两个整数的算术运算
    public static int intoperation(string operation, int a, int b)
    {
        var result = 0;
        if (operation == "" || operation == "+")
        {
            result = a + b;
        }
        else
        {
            switch (operation)
            {
                case "-":
                    result = a - b;
                    break;
                case "*":
                    result = a * b;
                    break;
                case "/":
                    result = a / b;
                    break;
                case "%":
                    result = a % b;
                    break;
            }
        }
        return result;
    }
    #endregion

    #region 检查怪物击杀条件是否满足
    public static bool NPCKillRequirement(Dictionary<int, long> Rmonster)
    {
        var result = false;
        if (Rmonster.Count > 0)
        {
            foreach (var item in Rmonster)
            {
                if (item.Key == 0 || item.Value == 0)
                {
                    continue;
                }
                var lNKC = getLNKC(item.Key);
                if (item.Value == 0)
                {
                    continue;
                }
                if (item.Value > 0)
                {
                    if (lNKC < item.Value)
                    {
                        result = true;
                        break;
                    }
                }
                else if (lNKC >= Math.Abs(item.Value))
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    #endregion

    #region 检查NPC的AI属性是否满足特定条件
    public static bool AIRequirement(Dictionary<string, float> Rmonster, NPC npc)
    {
        var result = false;
        if (Rmonster.Count > 0)
        {
            for (var i = 0; i < npc.ai.Count(); i++)
            {
                var key = i.ToString();
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value) && npc.ai[i] != value)
                {
                    result = true;
                    break;
                }
                key = "!" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value2) && npc.ai[i] == value2)
                {
                    result = true;
                    break;
                }
                key = ">" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value3) && npc.ai[i] <= value3)
                {
                    result = true;
                    break;
                }
                key = "<" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value4) && npc.ai[i] >= value4)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    #endregion

    #region 检查弹幕的AI属性是否满足特定条件。
    public static bool AIRequirementP(Dictionary<string, float> Rmonster, Projectile Projectiles)
    {
        var result = false;
        if (Rmonster.Count > 0)
        {
            for (var i = 0; i < Projectiles.ai.Count(); i++)
            {
                var key = i.ToString();
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value) && Projectiles.ai[i] != value)
                {
                    result = true;
                    break;
                }
                key = "!" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value2) && Projectiles.ai[i] == value2)
                {
                    result = true;
                    break;
                }
                key = ">" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value3) && Projectiles.ai[i] <= value3)
                {
                    result = true;
                    break;
                }
                key = "<" + i;
                if (Rmonster.ContainsKey(key) && Rmonster.TryGetValue(key, out var value4) && Projectiles.ai[i] >= value4)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
    #endregion

    #region 检查两个Vector2位置是否在指定的最大范围内 使用DistanceSquared方法避免计算平方根提高性能。
    public static bool WithinRange(Vector2 Center, Vector2 Target, float MaxRange)
    {
        return Vector2.DistanceSquared(Center, Target) <= MaxRange * MaxRange;
    }
    #endregion

    #region 检查由(x, y)坐标表示的位置与目标位置是否在指定的最大范围内。
    public static bool WithinRange(float x, float y, Vector2 target, float maxRange)
    {
        var pos = new Vector2(x, y);
        return Vector2.DistanceSquared(pos, target) <= maxRange * maxRange;
    }
    #endregion

    #region 检查目标位置与由(x, y)坐标表示的位置是否在指定的最大范围内。
    public static bool WithinRange(Vector2 target, float x, float y, float maxRange)
    {
        var pos = new Vector2(x, y);
        return Vector2.DistanceSquared(target, pos) <= maxRange * maxRange;
    }
    #endregion

    #region 检查两个点(x, y)和(x2, y2)之间是否在指定的最大范围内。
    public static bool WithinRange(float x, float y, float x2, float y2, float maxRange)
    {
        var pos1 = new Vector2(x, y);
        var pos2 = new Vector2(x2, y2);
        return Vector2.DistanceSquared(pos1, pos2) <= maxRange * maxRange;
    }
    #endregion

    #region 检查世界种子条件是否满足
    public static bool SeedRequirement(string[] Rmonster)
    {
        var result = false;
        var list = new List<string>();

        // 根据当前世界的特性填充list列表。
        if (Main.getGoodWorld)
        {
            list.Add("getGoodWorld");
        }

        if (Main.tenthAnniversaryWorld)
        {
            list.Add("tenthAnniversaryWorld");
        }

        if (Main.notTheBeesWorld)
        {
            list.Add("notTheBeesWorld");
        }
        if (Main.dontStarveWorld)
        {
            list.Add("dontStarveWorld");
        }
        if (Main.drunkWorld)
        {
            list.Add("drunkWorld");
        }
        if (Main.remixWorld)
        {
            list.Add("remixWorld");
        }
        if (Main.noTrapsWorld)
        {
            list.Add("noTrapsWorld");
        }
        if (Main.zenithWorld)
        {
            list.Add("zenithWorld");
        }

        foreach (var text in Rmonster)
        {
            if (text == "!getGoodWorld")
            {
                if (list.Contains("getGoodWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "getGoodWorld" && !list.Contains("getGoodWorld"))
            {
                result = true;
                break;
            }
            if (text == "!tenthAnniversaryWorld")
            {
                if (list.Contains("tenthAnniversaryWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "tenthAnniversaryWorld" && !list.Contains("tenthAnniversaryWorld"))
            {
                result = true;
                break;
            }
            if (text == "!notTheBeesWorld")
            {
                if (list.Contains("notTheBeesWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "notTheBeesWorld" && !list.Contains("notTheBeesWorld"))
            {
                result = true;
                break;
            }
            if (text == "!dontStarveWorld")
            {
                if (list.Contains("dontStarveWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "dontStarveWorld" && !list.Contains("dontStarveWorld"))
            {
                result = true;
                break;
            }
            if (text == "!drunkWorld")
            {
                if (list.Contains("drunkWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "drunkWorld" && !list.Contains("drunkWorld"))
            {
                result = true;
                break;
            }
            if (text == "!remixWorld")
            {
                if (list.Contains("remixWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "remixWorld" && !list.Contains("remixWorld"))
            {
                result = true;
                break;
            }
            if (text == "!noTrapsWorld")
            {
                if (list.Contains("noTrapsWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "noTrapsWorld" && !list.Contains("noTrapsWorld"))
            {
                result = true;
                break;
            }
            if (text == "!zenithWorld")
            {
                if (list.Contains("zenithWorld"))
                {
                    result = true;
                    break;
                }
            }
            else if (text == "zenithWorld" && !list.Contains("zenithWorld"))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion

    #region 检查玩家条件是否满足。
    public static bool PlayerRequirement(List<PlayerConditionGroup> Rmonster, NPC npc)
    {
        var Npc = npc;
        var result = false;
        foreach (var monster in Rmonster)
        {
            if (monster.InRange <= 0)
            {
                continue;
            }
            var num = 0;
            num = (monster.StartRange <= 0) ? TShock.Players.Count(p => p != null && p.Active && !p.Dead && p.TPlayer.statLife > 0 && (monster.Life == 0 || ((monster.Life > 0) ? (p.TPlayer.statLife >= monster.Life) : (p.TPlayer.statLife < Math.Abs(monster.Life)))) && Npc.WithinRange(p.TPlayer.Center, monster.InRange << 4)) : TShock.Players.Count(p => p != null && p.Active && !p.Dead && p.TPlayer.statLife > 0 && (monster.Life == 0 || ((monster.Life > 0) ? (p.TPlayer.statLife >= monster.Life) : (p.TPlayer.statLife < Math.Abs(monster.Life)))) && !Npc.WithinRange(p.TPlayer.Center, monster.StartRange << 4) && Npc.WithinRange(p.TPlayer.Center, monster.InRange << 4));
            if (monster.SuitNum == 0)
            {
                continue;
            }
            if (monster.SuitNum > 0)
            {
                if (num < monster.SuitNum)
                {
                    result = true;
                    break;
                }
            }
            else if (num >= Math.Abs(monster.SuitNum))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion

    #region 检查怪物条件是否满足。
    public static bool MonsterRequirement(List<MonsterConditionGroup> Rmonster, NPC npc)
    {
        var Npc = npc;
        var result = false;
        foreach (var monster in Rmonster)
        {
            var num = 0;
            num = (monster.Range <= 0) ? Main.npc.Count(p => p != null && p.active && (monster.NPCID == 0 || p.netID == monster.NPCID) && p.whoAmI != Npc.whoAmI && (monster.LifeRate == 0 || p.lifeMax < 1 || ((monster.LifeRate > 0) ? (p.life * 100 / p.lifeMax >= monster.LifeRate) : (p.life * 100 / p.lifeMax < Math.Abs(monster.LifeRate)))) && (monster.Indicator == null || (LNpcs![p.whoAmI] != null && LNpcs[p.whoAmI].haveMarkers(monster.Indicator, Npc))) && (monster.CheckSign == "" || (LNpcs![p.whoAmI] != null && LNpcs[p.whoAmI].Config != null && LNpcs[p.whoAmI].Config!.Sign == monster.CheckSign))) : Main.npc.Count(p => p != null && p.active && (monster.NPCID == 0 || p.netID == monster.NPCID) && p.whoAmI != Npc.whoAmI && Npc.WithinRange(p.Center, monster.Range << 4) && (monster.LifeRate == 0 || p.lifeMax < 1 || ((monster.LifeRate > 0) ? (p.life * 100 / p.lifeMax >= monster.LifeRate) : (p.life * 100 / p.lifeMax < Math.Abs(monster.LifeRate)))) && (monster.Indicator == null || (LNpcs![p.whoAmI] != null && LNpcs[p.whoAmI].haveMarkers(monster.Indicator, Npc))) && (monster.CheckSign == "" || (LNpcs != null && LNpcs[p.whoAmI].Config != null && LNpcs[p.whoAmI].Config!.Sign == monster.CheckSign)));
            if (monster.SuitNum == 0)
            {
                continue;
            }
            if (monster.SuitNum > 0)
            {
                if (num < monster.SuitNum)
                {
                    result = true;
                    break;
                }
            }
            else if (num >= Math.Abs(monster.SuitNum))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion

    #region 将字符串转换为浮点数，若转换失败则返回默认值。
    public static float StrToFloat(string FloatString, float DefaultFloat = 0f)
    {
        return FloatString != null && FloatString != "" ? float.TryParse(FloatString, out var result) ? result : DefaultFloat : DefaultFloat;
    }
    #endregion

    #region 检查弹幕条件是否满足。
    public static bool ProjectileRequirement(List<ProjectileConditionGroup> Rmonster, NPC npc)
    {
        var Npc = npc;
        var result = false;
        foreach (var rmonster in Rmonster)
        {
            var num = 0;
            num = (rmonster.Range <= 0) ? Main.projectile.Count(p => p != null && p.active && p.owner == Main.myPlayer && (rmonster.ProjectileID == 0 || p.type == rmonster.ProjectileID) && (!rmonster.FullProjectile || (LPrjs![p.whoAmI] != null && LPrjs[p.whoAmI].UseI == Npc.whoAmI)) && (rmonster.CheckSign == "" || (LPrjs![p.whoAmI] != null && LPrjs![p.whoAmI].Notes == rmonster.CheckSign))) : Main.projectile.Count(p => p != null && p.active && p.owner == Main.myPlayer && (rmonster.ProjectileID == 0 || p.type == rmonster.ProjectileID) && Npc.WithinRange(p.Center, rmonster.Range << 4) && (!rmonster.FullProjectile || (LPrjs![p.whoAmI] != null && LPrjs[p.whoAmI].UseI == Npc.whoAmI)) && (rmonster.CheckSign == "" || (LPrjs![p.whoAmI] != null && LPrjs[p.whoAmI].Notes == rmonster.CheckSign)));
            if (rmonster.SuitNum == 0)
            {
                continue;
            }
            if (rmonster.SuitNum > 0)
            {
                if (num < rmonster.SuitNum)
                {
                    result = true;
                    break;
                }
            }
            else if (num >= Math.Abs(rmonster.SuitNum))
            {
                result = true;
                break;
            }
        }
        return result;
    }
    #endregion

    #region 更新弹幕和属性
    public static void updataProjectile(List<ProjUpdateGroup> Projectiles, NPC npc, LNPC lnpc)
    {
        var list = new List<int>();
        foreach (var Proj in Projectiles)
        {
            if (Proj.ProjectileID <= 0)
            {
                continue;
            }
            var flag = false;
            lock (LPrjs!)
            {
                for (var i = 0; i < LPrjs.Length; i++)
                {
                    if (LPrjs[i] == null || LPrjs[i].Index < 0 || LPrjs[i].Type != Proj.ProjectileID || !(LPrjs[i].Notes == Proj.Sign) || LPrjs[i].UseI != npc.whoAmI)
                    {
                        continue;
                    }
                    var index = LPrjs[i].Index;
                    if (Main.projectile[index] == null || !Main.projectile[index].active || Main.projectile[index].type != Proj.ProjectileID || Main.projectile[index].owner != Main.myPlayer || AIRequirementP(Proj.AiConditions, Main.projectile[index]))
                    {
                        continue;
                    }
                    var x = Main.projectile[index].position.X;
                    var y = Main.projectile[index].position.Y;
                    if (!flag)
                    {
                        flag = true;
                        if (Proj.ProjectileXIndicatorName != "")
                        {
                            lnpc.setMarkers(Proj.ProjectileXIndicatorName, (int) x, reset: false);
                        }
                        if (Proj.ProjectileYIndicatorName != "")
                        {
                            lnpc.setMarkers(Proj.ProjectileYIndicatorName, (int) y, reset: false);
                        }
                    }
                    if (Proj.SummonMonsterAtProjectilePoint != 0)
                    {
                        LaunchProjectileSpawnNPC(Proj.SummonMonsterAtProjectilePoint, x, y);
                    }
                    var x2 = Main.projectile[index].velocity.X;
                    var y2 = Main.projectile[index].velocity.Y;
                    var damage = Main.projectile[index].damage;
                    var knockBack = Main.projectile[index].knockBack;
                    var num = x2;
                    var num2 = y2;
                    var num3 = damage;
                    var num4 = knockBack;
                    num3 += Proj.Damage;
                    num4 += Proj.Knockback;
                    if (Proj.LockRange > 0 || Proj.LockRange == -1)
                    {
                        var num5 = -2;
                        if (Proj.LockRange == -1)
                        {
                            num5 = -1;
                        }
                        else
                        {
                            var num6 = -1;
                            var num7 = -1f;
                            int? num8 = null;
                            int? num9 = null;
                            int? num10 = null;
                            for (var j = 0; j < 255; j++)
                            {
                                if (num5 == j || Main.player[j] == null || !Main.player[j].active || Main.player[j].dead || (Proj.AttackTargetOnly && j != npc.target))
                                {
                                    continue;
                                }
                                var num11 = Math.Abs(Main.player[j].Center.X - x + Math.Abs(Main.player[j].Center.Y - y));
                                if ((num7 == -1f || num11 < num7) && (!Proj.IncludeInHatred || !num9.HasValue || (Proj.ReverseHatredLock ? (Main.player[j].aggro < num9) : (Main.player[j].aggro > num9))) && (!Proj.LockLowLife || !num8.HasValue || (Proj.ReverseLifeLock ? (Main.player[j].statLife > num8) : (Main.player[j].statLife < num8))) && (!Proj.LockLowDefense || !num10.HasValue || (Proj.ReverseDefenseLock ? (Main.player[j].statDefense > num10) : (Main.player[j].statDefense < num10))))
                                {
                                    if (Proj.IncludeInHatred)
                                    {
                                        num9 = Main.player[j].aggro;
                                    }
                                    if (Proj.LockLowLife)
                                    {
                                        num8 = Main.player[j].statLife;
                                    }
                                    if (Proj.LockLowDefense)
                                    {
                                        num10 = Main.player[j].statDefense;
                                    }
                                    num7 = num11;
                                    num6 = j;
                                }
                            }
                            if (num6 != -1)
                            {
                                num5 = num6;
                            }
                        }
                        if (num5 != -2)
                        {
                            var x3 = npc.Center.X;
                            var y3 = npc.Center.Y;
                            var flag2 = false;
                            float num12;
                            float num13;
                            if (num5 == -1)
                            {
                                num12 = x3;
                                num13 = y3;
                            }
                            else
                            {
                                var val = Main.player[num5];
                                if (val == null)
                                {
                                    flag2 = true;
                                }
                                if (val!.dead || val.statLife < 1)
                                {
                                    flag2 = true;
                                }
                                if (Proj.LockRange > 350)
                                {
                                    Proj.LockRange = 350;
                                }
                                if (!WithinRange(x3, y3, val.Center, Proj.LockRange << 4))
                                {
                                    flag2 = true;
                                }
                                num12 = val.Center.X;
                                num13 = val.Center.Y;
                            }
                            if (!flag2)
                            {
                                var num14 = Proj.LockSpeed + (lnpc.getMarkers(Proj.IndicatorCountInjectLockSpeedName) * Proj.IndicatorCountInjectLockSpeedFactor);
                                var num15 = num12 - x;
                                var num16 = num13 - y;
                                if (num15 == 0f && num16 == 0f)
                                {
                                    num15 = 1f;
                                }
                                var num17 = Math.Atan2(num16, num15) * 180.0 / Math.PI;
                                num17 += (double) (Proj.AngleOffset + (lnpc.getMarkers(Proj.IndicatorCountInjectAngleName) * Proj.IndicatorCountInjectAngleFactor));
                                num = (float) ((double) num14 * Math.Cos(num17 * Math.PI / 180.0));
                                num2 = (float) ((double) num14 * Math.Sin(num17 * Math.PI / 180.0));
                                num += Proj.XSpeed + (lnpc.getMarkers(Proj.IndicatorCountInjectXSpeedName) * Proj.IndicatorCountInjectXSpeedFactor);
                                num2 += Proj.YSpeed + (lnpc.getMarkers(Proj.IndicatorCountInjectYSpeedName) * Proj.IndicatorCountInjectYSpeedFactor);
                            }
                        }
                    }
                    else
                    {
                        num = Proj.XSpeed + (lnpc.getMarkers(Proj.IndicatorCountInjectXSpeedName) * Proj.IndicatorCountInjectXSpeedFactor);
                        num2 = Proj.YSpeed + (lnpc.getMarkers(Proj.IndicatorCountInjectYSpeedName) * Proj.IndicatorCountInjectYSpeedFactor);
                        var num18 = (float) Math.Sqrt(Math.Pow(num, 2.0) + Math.Pow(num2, 2.0));
                        var num19 = Math.Atan2(num2, num) * 180.0 / Math.PI;
                        var num20 = Proj.AngleOffset + (lnpc.getMarkers(Proj.IndicatorCountInjectAngleName) * Proj.IndicatorCountInjectAngleFactor);
                        if (num20 != 0f)
                        {
                            num19 += (double) num20;
                            num = (float) ((double) num18 * Math.Cos(num19 * Math.PI / 180.0));
                            num2 = (float) ((double) num18 * Math.Sin(num19 * Math.PI / 180.0));
                        }
                    }
                    if (num3 != damage)
                    {
                        Main.projectile[index].damage = num3;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (num4 != knockBack)
                    {
                        Main.projectile[index].knockBack = num4;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (num != x2)
                    {
                        Main.projectile[index].velocity.X = num;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (num != x2)
                    {
                        Main.projectile[index].velocity.X = num;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (num2 != y2)
                    {
                        Main.projectile[index].velocity.Y = num2;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (Proj.SpeedInjectsIntoAi0)
                    {
                        var num21 = (float) Math.Atan2(num2, num);
                        num = Proj.XSpeedAfterAi0Injection;
                        num2 = Proj.YSpeedAfterAi0Injection;
                        Main.projectile[index].ai[0] = num21;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                    if (Proj.AiValues.Count > 0)
                    {
                        for (var k = 0; k < Main.projectile[index].ai.Count(); k++)
                        {
                            if (Proj.AiValues.ContainsKey(k) && Proj.AiValues.TryGetValue(k, out var value))
                            {
                                Main.projectile[index].ai[k] = value;
                                if (!list.Contains(index))
                                {
                                    list.Add(index);
                                }
                            }
                        }
                    }
                    if (Proj.IndicatorInjectAiValues.Count > 0)
                    {
                        for (var l = 0; l < Main.projectile[index].ai.Count(); l++)
                        {
                            if (Proj.IndicatorInjectAiValues.ContainsKey(l) && Proj.IndicatorInjectAiValues.TryGetValue(l, out var value2))
                            {
                                var array = value2.Split('*');
                                var result = 1f;
                                if (array.Length == 2 && array[1] != "")
                                {
                                    float.TryParse(array[1], out result);
                                }
                                Main.projectile[index].ai[l] = lnpc.getMarkers(value2) * result;
                                if (!list.Contains(index))
                                {
                                    list.Add(index);
                                }
                            }
                        }
                    }
                    if (Proj.Duration == 0)
                    {
                        Main.projectile[index].Kill();
                    }
                    else if (Proj.Duration > 0)
                    {
                        Main.projectile[index].timeLeft = Proj.Duration;
                    }
                    if (Proj.DestroyProjectile)
                    {
                        Main.projectile[index].active = false;
                        Main.projectile[index].type = 0;
                        if (!list.Contains(index))
                        {
                            list.Add(index);
                        }
                    }
                }
            }
        }
        foreach (var item in list)
        {
            TSPlayer.All.SendData((PacketTypes) 27, "", item, 0f, 0f, 0f, 0);
        }
    }
    #endregion

    #region 添加一个弹幕使用记录
    public static void addPrjsOfUse(int pid, int useIndex, int type, string notes)
    {
        lock (LPrjs!)
        {
            LPrjs[pid] = new LPrj(pid, useIndex, type, notes);
        }
    }
    #endregion
}