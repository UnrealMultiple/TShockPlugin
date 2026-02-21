using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;


namespace SurvivalCrisis.Nets
{
	public struct NetGroundItem
	{
		private bool noAutoUpdate;

		public int Index 
		{ 
			get;
		}
		public bool AutoUpdate
		{
			get => !noAutoUpdate;
			set => noAutoUpdate = !value;
		}

		public Item Item => Main.item[Index].inner;

        public bool Active
		{
			get => Item.active && Stack > 0 && ID > 0;
			set 
			{
				value &= Stack > 0 && ID > 0;
                if (!value)
                {
                    Item.TurnToAir();
                }
				if (AutoUpdate)
				{
					UpdateToClient();
				}
			}
		}
		public int ID
		{
			get => Item.type;
			set
			{
				Item.type = value;
				Active = value > 0;
				if (AutoUpdate)
				{
					UpdateToClient();
				}
			}
		}
		public int Stack
		{
			get => Item.stack;
			set
			{
				Item.stack = value;
                Active = value > 0;
				if (AutoUpdate)
				{
					UpdateToClient();
				}
			}
		}
		public byte Prefix
		{
			get => Item.prefix;
			set
			{
				Item.Prefix(value);
				if (AutoUpdate && Active)
				{
					UpdateToClient();
				}
			}
		}

		public NetGroundItem(int index)
		{
			Index = index;
			noAutoUpdate = false;
		}

		public void UpdateToClient(int clientID = -1)
		{
			NetMessage.SendData((int)PacketTypes.UpdateItemDrop, clientID, -1, null, Index);
		}
	}
}
