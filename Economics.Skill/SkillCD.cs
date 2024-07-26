
using System.Collections.Concurrent;
using Economics.Skill.Events;
using TShockAPI;

namespace Economics.Skill;

internal class SkillCD
{
    private readonly static ConcurrentDictionary<TSPlayer, int> God = new();

    private static long Count = 0;
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
        if (Count % 100 == 0)
            AIStyle.Remove();
        Count++;
    }

    public static void GodPlayer(TSPlayer player, int time)
    {
        if (God.TryGetValue(player, out var god))
        {
            God[player] += time;
        }
        else
        {
            God[player] = time;
        }
    }

    public static void SendGodPacket()
    {
        for (int i = 0; i < God.Count; i++)
        {
            var (player, time) = God.ElementAt(i);
            if (!player.Active || time <= 0)
                God.Remove(player, out var _);
            else
            { 
                player.SendData(PacketTypes.PlayerDodge, "", player.Index, 2f);
                God[player] -= 100;
            }
                
        }
    }
}
