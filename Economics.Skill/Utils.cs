using Economics.Skill.Enumerates;
using Economics.Skill.Model;
using Economics.Skill.Model.Options;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Economics.Skill;

public class Utils
{
    public static SkillContext VerifyBindSkill(TSPlayer Player, int index)
    {
        var context = Skill.Config.GetSkill(index) ?? throw new NullReferenceException($"技能序号{index} 不存在！");
        if (context.SkillSpark.SparkMethod.Contains(SkillSparkType.Take) && (Player.SelectedItem.netID == 0 || Player.SelectedItem.stack == 0))
            throw new Exception("这是一个主动技能，请手持一个有效武器!");
        if (!RPG.RPG.InLevel(Player.Name, context.LimitLevel))
            throw new Exception($"你当前等级无法购买此技能，限制等级:{string.Join(", ", context.LimitLevel)}");
        if (!Player.InProgress(context.LimitProgress))
            throw new Exception($"当前进度无法购买此技能，限制进度:{string.Join(", ", context.LimitProgress)}");
        var bind = Skill.PlayerSKillManager.QuerySkillByItem(Player.Name, Player.SelectedItem.netID);
        if (context.SkillUnique && Skill.PlayerSKillManager.HasSkill(Player.Name, index))
            throw new Exception("此技能是唯一的不能重复绑定!");
        if (context.SkillUniqueAll && Skill.PlayerSKillManager.HasSkill(index))
            throw new Exception("此技能全服唯一已经有其他人绑定了此技能!");
        if (bind.Count >= Skill.Config.SkillMaxCount)
            throw new Exception("技能已超过规定的最大绑定数量!");
        if (bind.Where(x => Convert.ToBoolean(x.Skill?.SkillSpark.SparkMethod.Contains(SkillSparkType.Take))).Count() >= Skill.Config.WeapoeBindMaxCount)
            throw new Exception("此武器已超过规定的最大绑定数量!");
        if (bind.Where(x => !Convert.ToBoolean(x.Skill?.SkillSpark.SparkMethod.Contains(SkillSparkType.Take))).Count() >= Skill.Config.PSkillMaxCount)
            throw new Exception("被动类型技能已超过最大绑定数量!");
        return context;
    }

    /// <summary>
    /// 通用技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public static void EmitGeneralSkill(TSPlayer Player, SkillContext skill)
    {
        if (!string.IsNullOrEmpty(skill.Broadcast))
            TShock.Utils.Broadcast(skill.Broadcast, Color.Wheat);
        Player.StrikeNpc(skill.StrikeNpc.Damage, skill.StrikeNpc.Range);
        Player.ExecRangeCommands(skill.ExecCommand.Range, skill.ExecCommand.Commands);
        Player.HealAllLife(skill.HealPlayerHPOption.Range, skill.HealPlayerHPOption.HP);
        Player.HealAllMana(skill.HealPlayerHPOption.Range, skill.HealPlayerHPOption.MP);
        Player.ClearProj(skill.ClearProjectile.Range);
        Player.CollectNPC(skill.PullNpc.Range, Skill.Config.BanPullNpcs, skill.PullNpc.X * 16, skill.PullNpc.Y * 16);
        foreach (var ply in Player.GetPlayerInRange(skill.BuffOption.Range))
            foreach (var buff in skill.BuffOption.Buffs)
                ply.SetBuff(buff.BuffId, buff.Time);
    }

    public static bool HasItem(TSPlayer player, List<TermItem> terms)
    {
        foreach (var item in terms)
        {
            if (item.Inventory)
            {
                var inv = player.TPlayer.inventory.Where(x => x.netID == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                    return false;
            }
            if (item.Armory)
            {
                var inv = player.TPlayer.armor.Where(x => x.netID == item.netID);
                if (!inv.Any() || inv.Sum(x => x.stack) < item.Stack)
                    return false;
            }
            if (item.HeldItem)
            {
                if (player.SelectedItem.netID != item.netID)
                    return false;
            }
        }
        ConsumeItem(player, terms);
        return true;
    }

    private static void ConsumeItem(TSPlayer player, List<TermItem> terms)
    {
        foreach (var term in terms)
        {
            var stack = term.Stack;
            for (int j = 0; j < player.TPlayer.inventory.Length; j++)
            {
                var item = player.TPlayer.inventory[j];
                if (item.netID == term.netID && term.Consume)
                {
                    if (item.stack >= stack)
                    {
                        item.stack -= stack;
                        TSPlayer.All.SendData(PacketTypes.PlayerSlot, "", player.Index, j);
                    }
                    else
                    {
                        stack -= item.stack;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 圆弧技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="circles"></param>
    /// <param name="pos"></param>
    public static void SpawnPointsOnArcProj(TSPlayer Player, List<CircleProjectile> circles, Vector2 pos)
    {
        Task.Run(async () =>
        {
            foreach (var circle in circles)
            {
                if (circle.Enable)
                {
                    var posed = pos.GetArcPoints(circle.StartAngle, circle.EndAngle, circle.Radius, circle.Interval);
                    var reverse = circle.Reverse ? -1 : 1;
                    foreach (var vec in posed)
                    {
                        var angle = vec.AngleFrom(pos);
                        Vector2 vel;
                        if (!circle.FollowPlayer)
                            vel = vec + new Vector2(circle.X * 16, circle.Y * 16);
                        else
                            vel = Player.TPlayer.Center + Player.TPlayer.ItemOffSet() + new Vector2(circle.X * 16, circle.Y * 16);
                        var radiusvel = vec.RotatedBy(angle).ToLenOf(circle.Speed) * reverse;
                        int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                            //发射原无期
                            Player.TPlayer.GetProjectileSource_Item(Player.TPlayer.HeldItem),
                            //发射位置
                            vel,
                            radiusvel,
                            circle.ID,
                            circle.Damage,
                            circle.Knockback,
                            Player.Index,
                            circle.AI[0],
                            circle.AI[1],
                            circle.AI[2],
                            circle.TimeLeft);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                    }
                    await Task.Delay(circle.Dealy);
                }
            }
        });
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
        foreach (var proj in skill.Projectiles)
        {
            if (!proj.AutoDirection)
                vel = new Vector2(proj.SpeedX, proj.SpeedY);
            SpawnPointsOnArcProj(Player, proj.CircleProjectiles, pos);
            Task.Run(async () =>
            {
                foreach (var opt in proj.ProjectileCycle.ProjectileCycles)
                {
                    var _vel = vel.RotationAngle(proj.Angle).ToLenOf(proj.Speed);
                    var _pos = pos + new Vector2(proj.X * 16, proj.Y * 16);
                    for (int i = 0; i < opt.Count; i++)
                    {
                        #region 生成弹幕
                        int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                            //发射原无期
                            Player.TPlayer.GetProjectileSource_Item(Player.TPlayer.HeldItem),
                            //发射位置
                            _pos,
                            _vel,
                            proj.ID,
                            proj.Damage,
                            proj.Knockback,
                            Player.Index,
                            proj.AI[0],
                            proj.AI[1],
                            proj.AI[2],
                            proj.TimeLeft);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                        #endregion

                        #region 数值重置
                        _vel = _vel.RotationAngle(opt.GrowAngle).ToLenOf(proj.Speed);
                        if (opt.FollowPlayer)
                            _pos = Player.TPlayer.Center + Player.TPlayer.ItemOffSet() + new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                        else
                            _pos += new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                        #endregion
                        await Task.Delay(opt.Dealy);
                    }
                }
            });
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
    }

    public static void EmitSkill(GetDataHandlers.NewProjectileEventArgs e, SkillContext skill)
    {
        //原始发射位置
        var pos = e.Position;
        //原始角度速度参数
        var vel = e.Velocity;
        SpawnSkillProjectile(e.Player, skill, vel, pos);
    }
}
