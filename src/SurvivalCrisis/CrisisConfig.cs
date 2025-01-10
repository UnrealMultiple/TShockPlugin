using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using TShockAPI;
using JObject = Newtonsoft.Json.Linq.JObject;

namespace SurvivalCrisis
{
	public class CrisisConfig
	{
		public struct Point4
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public static implicit operator TileSection(in Point4 value)
			{
				return new TileSection(value.Left, value.Top, value.Right - value.Left, value.Bottom - value.Top);
			}

			public override string ToString()
			{
				var X = Left;
				var Y = Top;
				var Width = Right - Left;
				var Height = Bottom - Top;
				return $"({X}, {Y}) Width({Width}) Height({Height})";
			}
		}
		public Point4 Hall
		{
			get;
			set;
		}
		public Point4 Lobby
		{
			get;
			set;
		}
		public Point4 WaitingZone
		{
			get;
			set;
		}
		public Point4 GamingZone
		{
			get;
			set;
		}

		public Point4 Islands
		{
			get;
			set;
		}
		public Point4 Surface
		{
			get;
			set;
		}
		public Point4 Underground
		{
			get;
			set;
		}
		public Point4 Cave
		{
			get;
			set;
		}
		public Point4 Spheres
		{
			get;
			set;
		}
		public Point4 Maze
		{
			get;
			set;
		}
		public Point4 CaveEx
		{
			get;
			set;
		}
		public Point4 Hell
		{
			get;
			set;
		}

		public Point[] SpheresLarge
		{
			get;
			set;
		}

		public static CrisisConfig LoadFile(string path)
		{
			var text = File.ReadAllText(path);
			return JsonConvert.DeserializeObject<CrisisConfig>(text);
		}
		public void Save(string path)
		{
			var text = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(path, text);
		}


		public void ShowTo(TSPlayer player)
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
