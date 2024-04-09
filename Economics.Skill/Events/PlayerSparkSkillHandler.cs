
using Economics.Skill.Converter;
using Economics.Skill.Enumerates;
using Economics.Skill.Model;
using TShockAPI;
using Terraria;

namespace Economics.Skill.Events;

public class PlayerSparkSkillHandler
{
    public delegate void EventCallBack<in TEventArgs>(TEventArgs args) where TEventArgs : System.EventArgs;

    public static  List<SkillSparkType> SparkType = typeof(SkillSparkType)
            .GetFields()
            .Where(f => f.FieldType == typeof(SkillSparkType))
            .Select(f => (SkillSparkType) f.GetValue(-1)!)
            .ToList();

    public static void Adapter(TSPlayer player, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        bool enable = false;
        if (skillContext.SkillSpark.SparkMethod.Contains(skillSparkType))
        {
            foreach (var Spark in skillContext.SkillSpark.SparkMethod)
            {
                if (Spark == skillSparkType)
                    continue;
                enable = Spark switch
                {
                    SkillSparkType.HP => player.TPlayer.statLife <= skillContext.SkillSpark.HP,
                    SkillSparkType.MP => player.TPlayer.statMana <= skillContext.SkillSpark.MP,
                    SkillSparkType.CD => true,
                    SkillSparkType.Death => player.Dead,
                    SkillSparkType.Take => player.TPlayer.controlUseItem,
                    SkillSparkType.Kill => skillSparkType == SkillSparkType.Kill,
                    SkillSparkType.Strike => skillSparkType == SkillSparkType.Strike,
                    _ => false
                };
                if (enable == false)
                    break;
            }
            Console.WriteLine(enable);
        }
    }

    public async static void Adapter(GetDataHandlers.NewProjectileEventArgs e, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        bool enable = false;
        if (skillContext.SkillSpark.SparkMethod.Contains(skillSparkType))
        {
            foreach (var Spark in skillContext.SkillSpark.SparkMethod)
            {
                if (Spark == skillSparkType)
                    continue;
                enable = Spark switch
                {
                    SkillSparkType.HP => e.Player.TPlayer.statLife <= skillContext.SkillSpark.HP,
                    SkillSparkType.MP => e.Player.TPlayer.statMana <= skillContext.SkillSpark.MP,
                    SkillSparkType.CD => true,
                    SkillSparkType.Death => e.Player.Dead,
                    SkillSparkType.Take => true,
                    SkillSparkType.Kill => await Skill.ESPlayers[e.Player.Index].IsKillNpc(TimeSpan.FromMilliseconds(10)),
                    SkillSparkType.Strike => skillSparkType == SkillSparkType.Strike,
                    _ => false
                } ;
                if (enable == false)
                    break;
            }
            if(enable)
            Console.WriteLine(enable);
        }
    }
}
