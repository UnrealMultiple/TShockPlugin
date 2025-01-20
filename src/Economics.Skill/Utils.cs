using Economics.Skill.Enumerates;
using Economics.Skill.Internal;
using Economics.Skill.JSInterpreter;
using Economics.Skill.Model;
using Economics.Skill.Model.Options.Projectile;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

public class Utils
{
    public static SkillContext VerifyBindSkill(TSPlayer Player, int index)
    {
        var context = Skill.Config.GetSkill(index) ?? throw new NullReferenceException(GetString($"技能序号{index} 不存在！"));
        if (context.SkillSpark.SparkMethod.Contains(SkillSparkType.Take) && (Player.SelectedItem.netID == 0 || Player.SelectedItem.stack == 0))
        {
            throw new Exception(GetString("绑定此技能需要手持一个武器!"));
        }
        if (context.Hidden)
        {
            throw new Exception(GetString("此技能无法被购买!"));
        }
        if (!RPG.RPG.InLevel(Player.Name, context.LimitLevel))
        {
            throw new Exception(GetString($"你当前等级无法购买此技能，限制等级:{string.Join(", ", context.LimitLevel)}"));
        }

        if (!Player.InProgress(context.LimitProgress))
        {
            throw new Exception(GetString($"当前进度无法购买此技能，限制进度:{string.Join(", ", context.LimitProgress)}"));
        }

        var bind = Skill.PlayerSKillManager.QuerySkillByItem(Player.Name, Player.SelectedItem.netID).Where(s => s.Skill != null && s.Skill.Hidden);
        return context.SkillUnique && Skill.PlayerSKillManager.HasSkill(Player.Name, index)
            ? throw new Exception(GetString("此技能是唯一的不能重复绑定!"))
            : context.SkillUniqueAll && Skill.PlayerSKillManager.HasSkill(index)
            ? throw new Exception(GetString("此技能全服唯一已经有其他人绑定了此技能!"))
            : bind.Count() >= Skill.Config.SkillMaxCount
            ? throw new Exception(GetString("技能已超过规定的最大绑定数量!"))
            : bind.Where(x => x.Skill != null && x.Skill.SkillSpark.SparkMethod.Contains(SkillSparkType.Take)).Count() >= Skill.Config.WeapoeBindMaxCount
            ? throw new Exception(GetString("此武器已超过规定的最大绑定数量!"))
            : context;
    }

    /// <summary>
    /// 通用技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public static void EmitGeneralSkill(TSPlayer Player, SkillContext skill)
    {
        skill.EmitGeneralSkill(Player);
    }


    internal static void CycleAdapr(TSPlayer ply, Vector2 vel, Vector2 pos, ProjectileOption option, NPC? lockNpc = null)
    {
        foreach (var opt in option.ProjectileCycle.ProjectileCycles)
        {
            var _vel = vel;
            #region 锁定敌怪
            if (option.LockNpcOption.Enable && option.LockNpcOption.Lock && lockNpc != null)
            {
                pos.Distance(lockNpc.Center);
                _vel = (pos.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY) * lockNpc.velocity.Length()).ToLenOf(option.Speed);
            }
            if(!option.AutoDirection)
            {
                _vel = vel.RotationAngle(option.StartAngle).ToLenOf(option.Speed);
            }
            #endregion

            var _pos = pos + new Vector2(option.X * 16, option.Y * 16);
            var oldpos = _pos;
            var cpos = _pos.GetPointsOnCircle(opt.Radius * 16, option.StartAngle, opt.GrowAngle, opt.Count);

            foreach (var i in Enumerable.Range(0, opt.Count))
            {
                JobjManager.Add(() =>
                {
                    if (opt.NewPos)
                    {
                        _vel = oldpos.DirectionTo(cpos[i]).SafeNormalize(-Vector2.UnitY).ToLenOf(option.Speed);
                        _pos = cpos[i];
                    }
                    //判断锁定敌怪
                    if (option.LockNpcOption.Enable && option.LockNpcOption.Lock && lockNpc != null)
                    {
                        _vel = _pos.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY).ToLenOf(option.Speed);
                    }
                    #region 生成弹幕
                    var guid = Guid.NewGuid().ToString();
                    var index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                        //发射原无期
                        ply.TPlayer.GetProjectileSource_Item(ply.TPlayer.HeldItem),
                        //发射位置
                        _pos,
                        _vel * (opt.Reverse ? -1 : 1),
                        option.ID,
                        option.Damage,
                        option.Knockback,
                        ply.Index,
                        option.AI[0],
                        option.AI[1],
                        option.AI[2],
                        option.TimeLeft,
                        guid);
                    TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                    #endregion
                    AIStyle.Set(Main.projectile[index], option.AISytle, guid);
                    #region 数值重置

                    if (!opt.NewPos)
                    {
                        _vel = _vel.RotationAngle(opt.GrowAngle).ToLenOf(option.Speed);
                    }

                    if (opt.FollowPlayer)
                    {
                        _pos = ply.TPlayer.Center + ply.TPlayer.ItemOffSet() + new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                    }
                    else
                    {
                        _pos += new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                    }

                    #endregion
                }).AddMilliSeconds(i * opt.Dealy);
            }
        }
    }


    /// <summary>
    /// 技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    /// <param name="vel"></param>
    /// <param name="pos"></param>
    public static void SpawnSkillProjectile(TSPlayer Player, SkillContext skill, Vector2 vel, Vector2 pos)
    {
        EmitGeneralSkill(Player, skill);
        foreach (var i in Enumerable.Range(0, skill.Projectiles.Count))
        {
            var proj = skill.Projectiles[i];
            JobjManager.Add(() =>
            {
                if (!proj.AutoDirection)
                {
                    vel = new Vector2(proj.SpeedX, proj.SpeedY);
                }

                NPC? lockNpc = null;
                if (proj.LockNpcOption.Enable)
                {
                    if (proj.LockNpcOption.LockCenter)
                    {
                        lockNpc = proj.LockNpcOption.LockMinHp ? Player.TPlayer.GetNpcInRangeByHp(proj.LockNpcOption.Range) : Player.TPlayer.GetNpcInRangeByDis(proj.LockNpcOption.Range);
                        if (lockNpc != null)
                        {
                            pos = lockNpc.Center;
                        }
                    }
                }
                CycleAdapr(Player, vel, pos, proj, lockNpc);
            }).AddMilliSeconds(proj.Dealy * i);
        }
    }
    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public static void EmitSkill(TSPlayer Player, SkillContext skill)
    {
        //原始发射位置
        var pos = Player.TPlayer.Center + Player.TPlayer.ItemOffSet();
        //原始角度速度参数
        var vel = Player.TPlayer.ItemOffSet();
        SpawnSkillProjectile(Player, skill, vel, pos);
        Interpreter.ExecuteScript(skill, Player, pos, vel);
    }

    public static void EmitSkill(GetDataHandlers.NewProjectileEventArgs e, SkillContext skill)
    {
        //原始发射位置
        var pos = e.Position;
        //原始角度速度参数
        var vel = e.Velocity;
        SpawnSkillProjectile(e.Player, skill, vel, pos);
        Interpreter.ExecuteScript(skill, e.Player, pos, vel);
    }
}