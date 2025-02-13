using Economics.Skill.Enumerates;
using Economics.Skill.Internal;
using Economics.Skill.JSInterpreter;
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
        if (!context.LimitSkill.All(i => Skill.PlayerSKillManager.HasSkill(Player.Name, i)))
        {
            throw new Exception(GetString($"你当前等级无法购买此技能，限制必须购买技能:{string.Join(", ", context.LimitSkill)}"));
        }
        var bind = Skill.PlayerSKillManager.QuerySkillByItem(Player.Name, Player.SelectedItem.netID).Where(s => s.Skill != null && !s.Skill.Hidden);
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
    /// 技能触发器
    /// </summary>
    /// <param name="Player"></param>
    /// <param name="skill"></param>
    /// <param name="vel"></param>
    /// <param name="pos"></param>
    public static void SpawnSkillProjectile(TSPlayer Player, SkillContext skill, Vector2 vel, Vector2 pos, int Damage)
    {
        var playerskill = new PlayerSkill(skill.LoopEvent, Player);
        foreach (var i in Enumerable.Range(0, skill.LoopEvent.LoopCount + 1))
        {
            JobjManager.Delayed(skill.LoopEvent.Interval * i, (args) =>
            {
                if (args is PlayerSkill con)
                {
                    con.Update(i, vel);
                }
            }, playerskill);
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
        SpawnSkillProjectile(Player, skill, vel, pos, Player.SelectedItem.damage);
        Interpreter.ExecuteScript(skill, Player, pos, vel);
    }

    public static void EmitSkill(GetDataHandlers.NewProjectileEventArgs e, SkillContext skill)
    {
        //原始发射位置
        var pos = e.Position;
        //原始角度速度参数
        var vel = e.Velocity;
        SpawnSkillProjectile(e.Player, skill, vel, pos, e.Damage);
        Interpreter.ExecuteScript(skill, e.Player, pos, vel);
    }
}