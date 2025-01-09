using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class LongLongDark : SpecialEvent
	{
		public override void Reset()
		{
			base.Reset();
			TimeLeft = 120 * 60;
		}
		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.IsLongLongDark = true;
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.LongLongDark, Texts.SpecialEvents.LongLongDarkColor);
			foreach (var player in SurvivalCrisis.Instance.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.SetBuff(BuffID.Slow, 120 * 60);
					player.TSPlayer.SetBuff(BuffID.Darkness, 120 * 60);
				}
			}
		}
		public override void Update()
		{

		}
		public override void End(in bool gameEnd = false)
		{
			if (!gameEnd)
			{
				SurvivalCrisis.Instance.IsLongLongDark = false;
			}
			base.End(gameEnd);
		}
	}
}
