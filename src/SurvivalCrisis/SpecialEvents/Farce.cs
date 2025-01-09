using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SurvivalCrisis.SpecialEvents
{
	public class Farce : SpecialEvent
	{
		private static SurvivalCrisis Game => SurvivalCrisis.Instance;
		public Farce()
		{
		}
		public override void Reset()
		{
			base.Reset();
			TimeLeft = 3 * 60 * 60;
		}
		public override void Start()
		{
			base.Start();
			Game.IsFarce = true;
			foreach (var player in Game.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					var name = player.TPlayer.name;
					var hair = player.TPlayer.hair;
					var hairColor = player.TPlayer.hairColor;
					var skinColor = player.TPlayer.skinColor;
					var skinVariant = player.TPlayer.skinVariant;
					var pantsColor = player.TPlayer.pantsColor;
					var shirtColor = player.TPlayer.shirtColor;
					var shoeColor = player.TPlayer.shoeColor;
					var shoe = player.TPlayer.shoe;


					player.TPlayer.name = "█████";
					player.TPlayer.hair = 0;
					player.TPlayer.hairColor = new Color(215, 90, 55);
					player.TPlayer.skinColor = new Color(255, 125, 90);
					player.TPlayer.skinVariant = 0;
					player.TPlayer.pantsColor = new Color(255, 230, 175);
					player.TPlayer.shirtColor = new Color(175, 165, 140);
					player.TPlayer.shoeColor = new Color(160, 105, 60);
					player.TPlayer.shoe = 0;

					NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, player.Index, null, player.Index);

					player.TPlayer.name = name;
					player.TPlayer.hair = hair;
					player.TPlayer.hairColor = hairColor;
					player.TPlayer.skinColor = skinColor;
					player.TPlayer.skinVariant = skinVariant;
					player.TPlayer.pantsColor = pantsColor;
					player.TPlayer.shirtColor = shirtColor;
					player.TPlayer.shoeColor = shoeColor;
					player.TPlayer.shoe = shoe;

					NetMessage.SendData((int)PacketTypes.PlayerActive, -1, player.Index, null, player.Index, false.GetHashCode());
				}
			}
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.Farce, Texts.SpecialEvents.FarceColor);
		}
		public override void Update()
		{

		}
		public override void End(in bool gameEnd = false)
		{
			if (!gameEnd)
			{
				Game.IsFarce = false;
				foreach (var player in Game.Participants)
				{
					if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
					{
						player.HideName();
						NetMessage.SendData((int)PacketTypes.PlayerActive, -1, player.Index, null, player.Index, true.GetHashCode());
					}
				}
			}
			base.End(gameEnd);
		}
	}
}
