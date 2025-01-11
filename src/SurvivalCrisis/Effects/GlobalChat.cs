using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Effects
{
	public class GlobalChat : Effect
	{
		public GlobalChat() : base(Type.Global)
		{

		}

		public override void Apply()
		{
			SurvivalCrisis.Instance.EnabledGlobalChat = true;
		}

		public override void Update()
		{
			End();
		}
	}
}
