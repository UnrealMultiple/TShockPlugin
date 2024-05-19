using Economics.Skill.Enumerates;
using Economics.Skill.Model;
using TShockAPI;

namespace Economics.Skill.Events;

public class PlayerSparkSkillHandler
{
    public delegate void EventCallBack<in TEventArgs>(TEventArgs args) where TEventArgs : System.EventArgs;

    public static List<SkillSparkType> SparkType = typeof(SkillSparkType)
            .GetFields()
            .Where(f => f.FieldType == typeof(SkillSparkType))
            .Select(f => (SkillSparkType)f.GetValue(-1)!)
            .ToList();
    public static bool IsSpark(TSPlayer Player, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        bool enable = false;
        if (skillContext.SkillSpark.SparkMethod.Contains(skillSparkType))
        {
            foreach (var Spark in skillContext.SkillSpark.SparkMethod)
            {
                if (Spark == skillSparkType)
                {
                    enable = true;
                    continue;
                }
                enable = Spark switch
                {
                    SkillSparkType.HP => Player.TPlayer.statLife <= skillContext.SkillSpark.HP,
                    SkillSparkType.MP => Player.TPlayer.statMana <= skillContext.SkillSpark.MP,
                    SkillSparkType.CD => true,
                    SkillSparkType.Death => Player.Dead,
                    SkillSparkType.Take => true,
                    SkillSparkType.Kill => true,
                    SkillSparkType.Strike => true,
                    _ => false
                };
                if (enable == false)
                    break;
            }
        }
        return enable;
    }


    public static void Adapter(TSPlayer Player, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        if (IsSpark(Player, skillContext, skillSparkType))
            Utils.EmitSkill(Player, skillContext);
    }

    public static void Adapter(GetDataHandlers.NewProjectileEventArgs e, SkillContext skillContext, SkillSparkType skillSparkType)
    {
       
    }
}
