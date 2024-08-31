using Terraria;
using TShockAPI;

namespace EssentialsPlus.Extensions;

public static class TSPlayerExtensions
{
    public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
    {
        if (!tsplayer.ContainsData(PlayerInfo.KEY))
        {
            tsplayer.SetData(PlayerInfo.KEY, new PlayerInfo());
        }

        return tsplayer.GetData<PlayerInfo>(PlayerInfo.KEY);
    }

    /// <summary>
    /// Finds a TSPlayer based on name or ID
    /// </summary>
    /// <param name="plr">Player name or ID</param>
    /// <returns>A list of matching players</returns>
    public static List<TSPlayer> FindPlayers(this TSPlayer[] tsplayer, string plr)
    {
        var found = new List<TSPlayer>();
        // Avoid errors caused by null search
        if (plr == null)
        {
            return found;
        }

        if (byte.TryParse(plr, out var plrID) && plrID < Main.maxPlayers)
        {
            var player = TShock.Players[plrID];
            if (player != null && player.Active)
            {
                return new List<TSPlayer> { player };
            }
        }

        var plrLower = plr.ToLower();
        foreach (var player in TShock.Players)
        {
            if (player != null)
            {
                // Must be an EXACT match
                if (player.Name == plr)
                {
                    return new List<TSPlayer> { player };
                }

                if (player.Name.ToLower().StartsWith(plrLower))
                {
                    found.Add(player);
                }
            }
        }
        return found;
    }
}