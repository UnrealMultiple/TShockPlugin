using Economics.Skill.Model;
using EconomicsAPI.Extensions;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Economics.Skill;

public class Utils
{
    public static void EmitGeneralSkill(TSPlayer Player, SkillContext skill)
    {
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
        foreach (var proj in skill.Projectiles)
        { 
            var vel = Player.TPlayer.ItemUseAngle()
        }
    }
}
