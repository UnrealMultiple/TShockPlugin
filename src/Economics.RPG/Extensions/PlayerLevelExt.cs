using Economics.RPG.Model;
using TShockAPI;

namespace Economics.RPG.Extensions;

public static class PlayerLevelExt
{
    public static Level? GetLevel(this TSPlayer player)
    {
        return RPG.PlayerLevelManager.GetLevel(player.Name);
    }

    public static bool InLevel(this TSPlayer player, IEnumerable<string> levels)
    {
        var playerLevel = player.GetLevel();
        if (!levels.Any())
        {
            return true;
        }

        if (playerLevel == null)
        {
            return false;
        }

        if (levels.Contains(playerLevel.Name))
        {
            return true;
        }

        foreach (var level in levels)
        {
            if (playerLevel.AllParentLevels.Any(x => x.Name == level))
            {
                return true;
            }
        }
        return false;
    }
}