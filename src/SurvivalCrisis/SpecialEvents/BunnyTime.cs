using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class BunnyTime : SpecialEvent
	{
		private SurvivalCrisis Game;
		protected int bunnyType;
		protected string text;
		protected Color color;
		public BunnyTime()
		{
			bunnyType = NPCID.Bunny;
			text = Texts.SpecialEvents.BunnyTime;
			color = Texts.SpecialEvents.BunnyTimeColor;
		}

		public override void Reset()
		{
			base.Reset();
			Game = SurvivalCrisis.Instance;
			StartDelay = 0;
			TimeLeft = 60 * 60;
		}

		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.BCToAll(text, color);
			foreach (var player in Game.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.SetBuff(BuffID.Confused, 10 * 60);
					player.TSPlayer.SetBuff(BuffID.Featherfall, 60 * 60);
				}
			}
		}

		public override void Update()
		{
			foreach (var player in Game.Participants)
			{
				if (player?.TSPlayer != null && player.IsValid() && player.Identity != PlayerIdentity.Watcher && TimeLeft % 60 == player.Index)
				{
					if (Main.npc.Count(npc => npc.active && npc.type == bunnyType && npc.Distance(player.TPlayer.Center) < 16 * 100) < 20)
					{
						SurvivalCrisis.GameOperations.TrySpawnEnemies(player, 2, new[] { (bunnyType, 0.005) });
					}
				}
			}
		}
	} 
}
