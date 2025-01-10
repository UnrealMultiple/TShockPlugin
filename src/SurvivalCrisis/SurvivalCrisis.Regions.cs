using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;


namespace SurvivalCrisis
{
	public partial class SurvivalCrisis
	{
		public static class Regions
		{
			public static void Initialize(CrisisConfig config)
			{
				Hall = config.Hall;
				Lobby = config.Lobby;
				WaitingZone = config.WaitingZone;
				GamingZone = config.GamingZone;

				Islands = config.Islands;
				Surface = config.Surface;
				// Underground = config.Underground;
				Cave = config.Cave;
				Spheres = config.Spheres;
				Maze = config.Maze;
				CaveEx = config.CaveEx;
				Hell = config.Hell;
			}

			public static TileSection Hall
			{
				get;
				internal set;
			}
			public static TileSection Lobby
			{
				get;
				private set;
			}
			public static TileSection WaitingZone
			{
				get;
				private set;
			}
			public static TileSection GamingZone
			{
				get;
				private set;
			}


			public static TileSection Islands
			{
				get;
				private set;
			}
			public static TileSection Surface
			{
				get;
				private set;
			}
			//public static TileSection Underground
			//{
			//	get;
			//	private set;
			//}
			public static TileSection Cave
			{
				get;
				private set;
			}
			public static TileSection Spheres
			{
				get;
				private set;
			}
			public static TileSection Maze
			{
				get;
				private set;
			}
			public static TileSection CaveEx
			{
				get;
				private set;
			}
			public static TileSection Hell
			{
				get;
				private set;
			}


			public static void ShowTo(TSPlayer player)
			{
				player.SendMessage($"{nameof(Hall)}: {Hall}", Color.Blue);
				player.SendMessage($"{nameof(WaitingZone)}: {WaitingZone}", Color.Blue);
				player.SendMessage($"{nameof(GamingZone)}: {GamingZone}", Color.Blue);

				player.SendMessage($"{nameof(Islands)}: {Islands}", Color.Blue);
				player.SendMessage($"{nameof(Surface)}: {Surface}", Color.Blue);
				// player.SendMessage($"{nameof(Underground)}: {Underground}", Color.Blue);
				player.SendMessage($"{nameof(Cave)}: {Cave}", Color.Blue);
				player.SendMessage($"{nameof(Spheres)}: {Spheres}", Color.Blue);
				player.SendMessage($"{nameof(Maze)}: {Maze}", Color.Blue);
				player.SendMessage($"{nameof(CaveEx)}: {CaveEx}", Color.Blue);
				player.SendMessage($"{nameof(Hell)}: {Hell}", Color.Blue);
			}
		}
	}
}
