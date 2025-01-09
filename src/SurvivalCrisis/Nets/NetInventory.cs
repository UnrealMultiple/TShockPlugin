using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Nets
{
	public class NetInventory
	{
		private NetInventorySlot[] slots;
		public GamePlayer Player { get; }
		public int Count
		{
			get => slots.Length;
		}
		public NetInventory(GamePlayer player)
		{
			Player = player;
			slots = new NetInventorySlot[player.TPlayer.inventory.Length];
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i] = new NetInventorySlot(player.Index, i);
			}
		}
		public NetInventorySlot this[int slot] => slots[slot];
	}
}
