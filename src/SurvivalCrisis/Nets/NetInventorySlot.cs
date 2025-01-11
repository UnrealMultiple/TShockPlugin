using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalCrisis.Nets
{
	using Item = Terraria.Item;
	public class NetInventorySlot
	{
		private readonly int? owner;

		public int? Slot { get; }

		public GamePlayer Owner => SurvivalCrisis.Instance.Players[(int)owner];
		public Item Item => Owner.TPlayer.inventory[(int)Slot];
		public int MaxStack => Item.maxStack;

		public int Type
		{
			get => Item.type;
			set => SetItemType(value);
		}
		public int Stack
		{
			get => Item.stack;
			set => SetItemStack(value);
		}
		public byte Prefix
		{
			get => Item.prefix;
			set => SetItemPrefix(value);
		}

		public bool IsAir
		{
			get => Item.IsAir;
		}

		public NetInventorySlot(int who, int slot)
		{
			owner = who;
			Slot = slot;
		}

		public void ToAir()
		{
			Type = 0;
		}
		public void SetDefaults(int type)
		{
			Item.SetDefaults(type);
			SendData();
		}

		#region Privates
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemPrefix(byte prefix)
		{
			Item.prefix = prefix;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemStack(int stack)
		{
			Item.stack = stack;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetItemType(int type)
		{
			Item.type = type;
			SendData();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SendData()
		{
			Owner.TSPlayer.SendData(PacketTypes.PlayerSlot, "", Owner.Index, (int)Slot);
		}
		#endregion
	}
}
