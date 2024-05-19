using Economics.Skill.Model;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.Skill;

public class Utils
{
    public static void EmitGeneralSkill(TSPlayer Player, SkillContext skill)
    {
        if (!string.IsNullOrEmpty(skill.Broadcast))
            TShock.Utils.Broadcast(skill.Broadcast, Color.Wheat);
        Player.StrikeNpc(skill.StrikeNpc.Damage, skill.StrikeNpc.Range);
        Player.ExecRangeCommands(skill.ExecCommand.Range, skill.ExecCommand.Commands);
        Player.HealAllLife(skill.HealPlayerHPOption.Range, skill.HealPlayerHPOption.HP);
        Player.ClearProj(skill.ClearProjectile.Range);
        Player.CollectNPC(skill.PullNpc.Range);
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    public static void EmitSkill(TSPlayer Player, SkillContext skill)
    {
        EmitGeneralSkill(Player, skill);
        //原始发射位置
        var pos = Player.TPlayer.Center + Player.TPlayer.ItemOffSet();
        //原始角度速度参数
        var vel = Player.TPlayer.ItemOffSet();
        Task.Run(() =>
        {
            foreach (var proj in skill.Projectiles)
            {
                foreach (var opt in proj.ProjectileCycle.ProjectileCycles)
                {
                    var _vel = vel.RotationAngle(proj.Angle).ToLenOf(proj.Speed);
                    var _pos = pos + new Vector2(proj.X, proj.Y);
                    for (int i = 0; i < opt.Count; i++)
                    {
                        #region 生成弹幕
                        int index = EconomicsAPI.Utils.Projectile.NewProjectile(
                            //发射原无期
                           Player.TPlayer.GetProjectileSource_Item(Player.TPlayer.HeldItem),
                           //发射位置
                           _pos,
                           _vel,
                           proj.ID,
                           proj.Damage, 
                           proj.Knockback, 
                           Player.Index);
                        TSPlayer.All.SendData(PacketTypes.ProjectileNew, "", index);
                        #endregion

                        #region 数值重置
                        _vel = _vel.RotationAngle(opt.GrowAngle).ToLenOf(proj.Speed);
                        if (opt.FollowPlayer)
                            _pos = Player.TPlayer.Center + Player.TPlayer.ItemOffSet() + new Vector2(opt.GrowX, opt.GrowY);
                        else
                            _pos += new Vector2(opt.GrowX, opt.GrowY);
                        #endregion
                        Task.Delay(opt.Dealy).Wait();
                    }
                }
            }
        });
    }
}
