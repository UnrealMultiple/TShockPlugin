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

    public static void Adapter(TSPlayer player, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        lock (Skill.ILock)
        { 
            Console.WriteLine(skillSparkType.ToString() + "触发技能" + IsSpark(player, skillContext, skillSparkType));
        }
    }

    public static bool IsSpark(TSPlayer Player, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        var kill = Player.GetData<ManualResetEventSlim>("kill");
        var strike = Player.GetData<ManualResetEventSlim>("strike");
        if (kill == null || kill.IsSet)
        {
            Player.SetData<ManualResetEventSlim>("kill", new());
            kill = Player.GetData<ManualResetEventSlim>("kill");
        }

        if (strike == null || strike.IsSet)
        {
            Player.SetData<ManualResetEventSlim>("strike", new());
            strike = Player.GetData<ManualResetEventSlim>("strike");
        }

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
                    SkillSparkType.Take => Player.TPlayer.controlUseItem,
                    SkillSparkType.Kill => kill.Wait(TimeSpan.FromMilliseconds(100)),
                    SkillSparkType.Strike => strike.Wait(TimeSpan.FromMilliseconds(100)),
                    _ => false
                };
                if (enable == false)
                    break;
            }
        }
        return enable;
    }

    public static void Adapter(GetDataHandlers.NewProjectileEventArgs e, SkillContext skillContext, SkillSparkType skillSparkType)
    {
        lock (Skill.ILock)
        {
            Console.WriteLine(skillSparkType.ToString() + "触发技能" + IsSpark(e.Player, skillContext, skillSparkType));
        }
    }
}
