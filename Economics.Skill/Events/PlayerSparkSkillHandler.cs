using Economics.Skill.DB;
using Economics.Skill.Enumerates;
using TShockAPI;

namespace Economics.Skill.Events;

public class PlayerSparkSkillHandler
{
    public delegate void EventCallBack<in TEventArgs>(TEventArgs args) where TEventArgs : System.EventArgs;

    public static bool IsSpark(TSPlayer Player, PlayerSKillManager.PlayerSkill skill, SkillSparkType skillSparkType)
    {
        bool enable = false;
        if (skill.Skill!.SkillSpark.SparkMethod.Contains(skillSparkType))
        {
            foreach (var Spark in skill.Skill!.SkillSpark.SparkMethod)
            {
                enable = Spark switch
                {
                    SkillSparkType.HP => Player.TPlayer.statLife <= skill.Skill!.SkillSpark.HP,
                    SkillSparkType.MP => Player.TPlayer.statMana <= skill.Skill!.SkillSpark.MP,
                    SkillSparkType.Dash => Player.TPlayer.dashDelay == -1,
                    SkillSparkType.CD => skill.SkillCD <= 0,
                    SkillSparkType.Death => Player.Dead,
                    SkillSparkType.Take => skillSparkType == SkillSparkType.Take && skill.BindItem == Player.SelectedItem.netID,
                    SkillSparkType.Kill => skillSparkType == SkillSparkType.Kill,
                    SkillSparkType.Strike => skillSparkType == SkillSparkType.Strike,
                    _ => false
                };
                if (!enable)
                    return false;
            }
        }
        enable = Utils.HasItem(Player, skill.Skill.TermItem);
        return enable;
    }


    public static void Adapter(TSPlayer Player, SkillSparkType skillSparkType)
    {
        var skills = Skill.PlayerSKillManager.QuerySkill(Player.Name);
        foreach (var skill in skills)
        {
            if (skill.Skill != null && IsSpark(Player, skill, skillSparkType))
            {
                Utils.EmitSkill(Player, skill.Skill);
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
                Utils.EmitSkill(e, skill.Skill);
                skill.ResetCD();
            }
        }
    }
}
