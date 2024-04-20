using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using Terraria;

namespace EssentialsPlus.Extensions
{
	public static class TSPlayerExtensions
	{
		public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
		{
			if (!tsplayer.ContainsData(PlayerInfo.KEY))
				tsplayer.SetData(PlayerInfo.KEY, new PlayerInfo());
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
				return found;

			byte plrID;
			if (byte.TryParse(plr, out plrID) && plrID < Main.maxPlayers)
			{
				TSPlayer player = TShock.Players[plrID];
				if (player != null && player.Active)
				{
					return new List<TSPlayer> { player };
				}
			}

			string plrLower = plr.ToLower();
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null)
				{
					// Must be an EXACT match
					if (player.Name == plr)
						return new List<TSPlayer> { player };
					if (player.Name.ToLower().StartsWith(plrLower))
						found.Add(player);
				}
			}
			return found;
		}
	}
}
