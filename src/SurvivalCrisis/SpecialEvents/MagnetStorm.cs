using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class MagnetStorm : SpecialEvent
	{
		public MagnetStorm()
		{

		}
		public override void Reset()
		{
			base.Reset();
			TimeLeft = 10 * 60;
		}
		public override void Start()
		{
			base.Start();
			SurvivalCrisis.Instance.IsMagnetStorm = true;
			SurvivalCrisis.Instance.BCToAll(Texts.SpecialEvents.MagneticStorm, Texts.SpecialEvents.MagneticStormColor);
			foreach (var player in SurvivalCrisis.Instance.Participants)
			{
				if (player.TSPlayer != null && player.Identity != PlayerIdentity.Watcher)
				{
					player.TSPlayer.SetBuff(BuffID.Electrified, 10 * 60);
				}
			}
		}
		public override void Update()
		{

		}
		public override void End(in bool gameEnd = false)
		{
			if(!gameEnd)
			{
				SurvivalCrisis.Instance.IsMagnetStorm = false;
			}
			base.End(gameEnd);
		}
	}
}
