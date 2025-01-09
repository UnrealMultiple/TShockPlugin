using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.SpecialEvents
{
	public class PainOfRelief : SpecialEvent
	{
		public PainOfRelief()
		{

		}
		public override void Reset()
		{
			base.Reset();
			TimeLeft = 10 * 5 * 60;
		}
		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.PainOfRelief, Texts.SpecialEvents.PainOfReliefColor);
			foreach (var player in SurvivalCrisis.Instance.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.Heal(player.TPlayer.statLifeMax2);
				}
			}
		}
		public override void Update()
		{
			if (TimeLeft % (5 * 60) == 0)
			{
				foreach (var player in SurvivalCrisis.Instance.Participants)
				{
					if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
					{
						player.Life -= 20;
						Utils.SendCombatText("-20", Color.LightGreen, player.TPlayer.Center, player.Index);
					}
				}
			}
		}
	}
}
