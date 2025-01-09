using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class NightMare : SpecialEvent
	{
		public override void Reset()
		{
			base.Reset();
			TimeLeft = 1;
		}
		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.NightMare, Texts.SpecialEvents.NightMareColor);
			foreach (var player in SurvivalCrisis.Instance.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.SetBuff(BuffID.Obstructed, 30 * 60);
					player.TSPlayer.SetBuff(BuffID.ShadowDodge, 4 * 60 * 60);
				}
			}
		}
		public override void Update()
		{

		}
	}
}
