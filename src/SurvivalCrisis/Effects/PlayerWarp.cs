using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Effects
{
	public class PlayerWarp : Effect
	{
		public Vector2 Dest
		{
			get;
		}
		public int CountDown
		{
			get => Target.WarpingCountDown;
			set => Target.WarpingCountDown = value;
		}
		public PlayerWarp(GamePlayer target, int countDown, Vector2 dest) : base(Type.Player, target)
		{
			CountDown = countDown;
			Dest = dest;
		}

		public override void Apply()
		{
			
		}

		public override void Update()
		{
			if (SurvivalCrisis.Instance.TraitorEMPTime > 0)
			{
				Target.SendText("跃迁系统遭到干扰，本次跃迁中止", Color.Red);
				CountDown = 0;
				Abort();
				return;
			}
			if (CountDown == 0)
			{
				End();
				return;
			}
			if (CountDown % 60 == 0)
			{
				Target.AddStatusMessage($"{CountDown / 60}s后开始跃迁");
			}
			CountDown--;
		}

		public override void End()
		{
			Target.WarpingCount++;
			Target.TeleportTo(Dest);
			base.End();
		}
	}
}
