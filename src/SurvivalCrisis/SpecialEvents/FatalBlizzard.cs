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
	public class FatalBlizzard : SpecialEvent
	{
		public FatalBlizzard()
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
			Main.StartRain();
			Main.windPhysics = true;
			Main.windPhysicsStrength = 100;
			Main.windSpeedCurrent = 50;
			Main.windSpeedTarget = 100;
			NetMessage.SendData((int)PacketTypes.WorldInfo);
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.FatalBlizzard, Texts.SpecialEvents.FatalBlizzardColor);
			foreach (var player in SurvivalCrisis.Instance.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.SetBuff(BuffID.Weak, 3 * 60 * 60);
				}
			}
		}
		public override void Update()
		{
			if (TimeLeft % 90 == 0)
			{
				foreach (var player in SurvivalCrisis.Instance.Participants)
				{
					if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher && !player.HasBuff(BuffID.Campfire))
					{
						player.Life -= 5;
						Utils.SendCombatText("-5", Color.Azure, player.TPlayer.Center, player.Index);
					}
				}
			}
		}
	}
}
