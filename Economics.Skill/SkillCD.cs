

using Economics.Skill.Events;

namespace Economics.Skill;

internal class SkillCD
{
    public static void Updata()
    {
        foreach (var player in Skill.PlayerSKillManager.PlayerSkills)
        {
            if (player.Player != null && player.Player.Active)
            {
                if (player.SkillCD <= 0 && player.Skill != null && player.Skill.SkillSpark.SparkMethod.Contains(Enumerates.SkillSparkType.CD))
                {
                    PlayerSparkSkillHandler.Adapter(player.Player, Enumerates.SkillSparkType.CD);
                }
                player.SkillCD -= 100;
            }
        }
    }
}
