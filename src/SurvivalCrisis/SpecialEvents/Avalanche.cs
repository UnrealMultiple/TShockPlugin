using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.SpecialEvents
{
	public class Avalanche : SpecialEvent
	{
		private const int escapeTime = 60 * 180;
		private int timer;
		public Avalanche()
		{

		}

		public override void Reset()
		{
			base.Reset();
			StartDelay = SurvivalCrisis.AvalancheTime - escapeTime;
			TimeLeft = SurvivalCrisis.TotalTime - SurvivalCrisis.AvalancheTime;
		}

		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.BCToAll(Texts.AvalancheComing, Color.Lavender);
		}

		public override void Update()
		{
			if (timer % 60 == 0)
			{
				if (timer < escapeTime)
				{
					foreach (var player in SurvivalCrisis.Instance.Players)
					{
						if (player.TSPlayer != null)
						{
							player.AddStatusMessage($"距离雪崩还有{(escapeTime - timer) / 60}s");
						}
					}
				}
			}
			if (timer == escapeTime)
			{
				SurvivalCrisis.Instance.BCToAll(Texts.AvalancheCome, Color.Lavender);
				foreach (var player in SurvivalCrisis.Instance.Players)
				{
					if (player != null && player.Identity != PlayerIdentity.Watcher && !SurvivalCrisis.Regions.Surface.InRange(player))
					{
						player.SendText("你被大雪活埋了...", Color.PaleVioletRed);
						player.TSPlayer.KillPlayer();
					}
				}
			}
			timer++;
		}

		public override void End(in bool gameEnd = false)
		{
			base.End(gameEnd);
		}
	}
}
