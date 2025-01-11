using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Effects
{
	public class ChatInterference : Effect
	{
		public int Time { get; }
		public ChatInterference(int time) : base(Type.Global)
		{
			Time = time;
		}

		public override void Apply()
		{
			SurvivalCrisis.Instance.BCToAll(Texts.GlobalChatIsInterfered, Color.Khaki);
			SurvivalCrisis.GameOperations.InterferenceChat(Time);
		}

		public override void Update()
		{
			End();
		}
	}
}
