using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace SurvivalCrisis.SpecialEvents
{
	public class BunnyRevenge : BunnyTime
	{
		public BunnyRevenge()
		{
			bunnyType = NPCID.ExplosiveBunny;
			text = Texts.SpecialEvents.BunnyRevenge;
			color = Texts.SpecialEvents.BunnyRevengeColor;
		}
	}
}
