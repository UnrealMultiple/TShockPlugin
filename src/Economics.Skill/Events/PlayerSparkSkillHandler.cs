using Economics.Skill.DB;
using Economics.Skill.Enumerates;
using Economics.Core.Extensions;
using TShockAPI;

namespace Economics.Skill.Events;

public class PlayerSparkSkillHandler
{
    public delegate void EventCallBack<in TEventArgs>(TEventArgs args) where TEventArgs : System.EventArgs;

    public static bool IsSpark(TSPlayer Player, PlayerSKillManager.PlayerSkill skill, SkillSparkType skillSparkType)
    {
        var enable = false;
        if (skill.Skill!.SkillSpark.SparkMethod.Contains(skillSparkType))
        {
            foreach (var Spark in skill.Skill!.SkillSpark.SparkMethod)
            {
                enable = Spark switch
                {
                    SkillSparkType.HP => skill.Skill!.SkillSpark.MeetHP(Player),
                    SkillSparkType.MP => skill.Skill!.SkillSpark.MeetMP(Player),
                    SkillSparkType.Jump => Player.TPlayer.jump > 0,
                    SkillSparkType.Dash => Player.TPlayer.dashDelay == -1,
                    SkillSparkType.CD => skill.SkillCD <= 0,    
                    SkillSparkType.Armor => skill.Skill!.SkillSpark.HasItem(Player),
                    SkillSparkType.Death => Player.Dead,
                    SkillSparkType.Take => skillSparkType == SkillSparkType.Take && skill.BindItem == Player.SelectedItem.type,
                    SkillSparkType.Kill => skillSparkType == SkillSparkType.Kill,
                    SkillSparkType.Strike => skillSparkType == SkillSparkType.Strike,
                    SkillSparkType.Struck => skillSparkType == SkillSparkType.Struck,
                    SkillSparkType.Buff => skill.Skill.SkillSpark.BuffsCondition.All(i => Player.TPlayer.buffType.Contains(i)),
                    SkillSparkType.Skill => skill.Skill.SkillSpark.SkillCondition.All(i => Skill.PlayerSKillManager.HasSkill(Player.Name, i)),
                    SkillSparkType.Environment => Player.InProgress(skill.Skill.SkillSpark.EnvironmentCondition),
                    _ => false
                };
                if (!enable)
                {
                    return false;
                }
            }
        }
        return enable;
    }


    public static void Adapter(TSPlayer Player, SkillSparkType skillSparkType)
    {
        var skills = Skill.PlayerSKillManager.QuerySkill(Player.Name);
        foreach (var skill in skills)
        {
            if (skill.Skill != null && IsSpark(Player, skill, skillSparkType))
            {
                Utils.EmitSkill(Player, skill);
                skill.ResetCD();
            }

        }

    }

    public static void Adapter(GetDataHandlers.NewProjectileEventArgs e, SkillSparkType skillSparkType)
    {
        var skills = Skill.PlayerSKillManager.QuerySkill(e.Player.Name);
        foreach (var skill in skills)
        {
            if (skill.Skill != null && IsSpark(e.Player, skill, skillSparkType))
            {
                Utils.EmitSkill(e, skill);
                skill.ResetCD();
            }
        }
    }
}