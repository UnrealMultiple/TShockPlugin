using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace SurvivalCrisis.Nets
{
	public class NetGroundItemCollection
	{
		private NetGroundItem[] items;

		public int Count => items.Length;
		public NetGroundItem this[int index]
		{
			get => items[index];
		}

		public NetGroundItemCollection()
		{
			items = new NetGroundItem[Main.maxItems];
			for (int i = 0; i < items.Length; i++)
			{
				items[i] = new NetGroundItem(i);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < items.Length; i++)
			{
				items[i].Active = false;
			}
		}
		public void Clear(int itemID)
		{
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].ID == itemID)
				{
					items[i].Active = false;
				}
			}
		}
	}
}
