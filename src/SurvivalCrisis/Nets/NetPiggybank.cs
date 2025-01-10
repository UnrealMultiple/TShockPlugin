using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Nets
{
	public class NetPiggybank
	{
		private NetPiggySlot[] slots;
		public GamePlayer Player { get; }
		public int Count
		{
			get => slots.Length;
		}
		public NetPiggySlot this[int slot] => slots[slot];
		public NetPiggybank(GamePlayer player)
		{
			Player = player;
			slots = new NetPiggySlot[player.TPlayer.bank.item.Length];
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = new NetPiggySlot(player.Index, i);
			}
		}
		public void Clear()
		{
			for (int i = 0; i < Count; i++)
			{
				this[i].ToAir();
			}
		}
		
	}
}
