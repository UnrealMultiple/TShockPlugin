using Economics.Skill.Enumerates;
using Economics.Skill.Model;
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
        if (bind.Where(x => x.Skill != null && x.Skill.SkillSpark.SparkMethod.Contains(SkillSparkType.Take)).Count() >= Skill.Config.WeapoeBindMaxCount)
            throw new Exception("此武器已超过规定的最大绑定数量!");
        if (bind.Where(x => x.Skill != null && x.Skill.SkillSpark.SparkMethod.Contains(SkillSparkType.Take)).Count() >= Skill.Config.PSkillMaxCount)
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
        Player.StrikeNpc(skill.StrikeNpc.Damage, skill.StrikeNpc.Range, Skill.Config.BanStrikeNpcs);
        Player.ExecRangeCommands(skill.ExecCommand.Range, skill.ExecCommand.Commands);
        Player.HealAllLife(skill.HealPlayerHPOption.Range, skill.HealPlayerHPOption.HP);
        Player.HealAllMana(skill.HealPlayerHPOption.Range, skill.HealPlayerHPOption.MP);
        Player.ClearProj(skill.ClearProjectile.Range);
        Player.CollectNPC(skill.PullNpc.Range, Skill.Config.BanPullNpcs, skill.PullNpc.X * 16, skill.PullNpc.Y * 16);
        if (skill.PlayerTp.Enable)
            Player.Teleport(Player.X + skill.PlayerTp.X * 16 * (skill.PlayerTp.Incline ? Player.TPlayer.direction : 1), Player.Y + skill.PlayerTp.Y * 16); 
        if (skill.PlayerGod.Enable)
            SkillCD.GodPlayer(Player, skill.PlayerGod.Time);
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
    /// 技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    /// <param name="vel"></param>
    /// <param name="pos"></param>
    public static void SpawnSkillProjectile(TSPlayer Player, SkillContext skill, Vector2 vel, Vector2 pos)
    {
        EmitGeneralSkill(Player, skill);
        Task.Run(async () =>
        {
            foreach (var proj in skill.Projectiles)
            {
                if (!proj.AutoDirection)
                    vel = new Vector2(proj.SpeedX, proj.SpeedY);
                NPC? lockNpc = null;
                if (proj.LockNpcOption.Enable)
                {
                    if (proj.LockNpcOption.LockCenter)
                    {
                        lockNpc = proj.LockNpcOption.LockMinHp ? Player.TPlayer.GetNpcInRangeByHp(proj.LockNpcOption.Range) : Player.TPlayer.GetNpcInRangeByDis(proj.LockNpcOption.Range);
                        if (lockNpc != null)
                            pos = lockNpc.Center;
                    }
                }

                foreach (var opt in proj.ProjectileCycle.ProjectileCycles)
                {
                    Vector2 _vel;
                    #region 锁定敌怪
                    if (proj.LockNpcOption.Enable && proj.LockNpcOption.Lock && lockNpc != null)
                    {
                        pos.Distance(lockNpc.Center);
                        _vel = (pos.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY) * lockNpc.velocity.Length()).ToLenOf(proj.Speed);
                    }
                    else
                    {
                        _vel = vel.RotationAngle(proj.StartAngle).ToLenOf(proj.Speed);
                    }
                    #endregion

                    var _pos = pos + new Vector2(proj.X * 16, proj.Y * 16);
                    var oldpos = _pos;
                    var cpos = _pos.GetPointsOnCircle(opt.Radius * 16, proj.StartAngle, opt.GrowAngle, opt.Count);

                    for (int i = 0; i < opt.Count; i++)
                    {

                        if (opt.NewPos)
                        {
                            _vel = oldpos.DirectionTo(cpos[i]).SafeNormalize(-Vector2.UnitY).ToLenOf(proj.Speed);
                            _pos = cpos[i];
                        }
                        //判断锁定敌怪
                        if (proj.LockNpcOption.Enable && proj.LockNpcOption.Lock && lockNpc != null)
                        {
                            _vel = (_pos.DirectionTo(lockNpc.Center).SafeNormalize(-Vector2.UnitY)).ToLenOf(proj.Speed);
                        }
                        #region 生成弹幕
                        int index = EconomicsAPI.Utils.SpawnProjectile.NewProjectile(
                            //发射原无期
                            Player.TPlayer.GetProjectileSource_Item(Player.TPlayer.HeldItem),
                            //发射位置
                            _pos,
                            _vel * (opt.Reverse ? -1 : 1),
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
                        AIStyle.Set(Main.projectile[index], proj.AISytle);
                        #region 数值重置

                        if (!opt.NewPos)
                            _vel = _vel.RotationAngle(opt.GrowAngle).ToLenOf(proj.Speed);
                        if (opt.FollowPlayer)
                            _pos = Player.TPlayer.Center + Player.TPlayer.ItemOffSet() + new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                        else
                            _pos += new Vector2(opt.GrowX * 16, opt.GrowY * 16);
                        
                        #endregion
                        await Task.Delay(opt.Dealy);
                    }
                }
                await Task.Delay(proj.Dealy);
            }
        });
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
