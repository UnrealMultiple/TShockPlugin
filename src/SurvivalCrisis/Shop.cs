using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis
{
	public class Shop
	{
		public class ItemInfo
		{
			public int ID { get; }
			public int Stack { get; }
			public int Prefix { get; }
			public ItemInfo(int id, int stack = 1, int prefix = 0)
			{
				ID = id;
				Stack = stack;
				Prefix = prefix;
			}
			public override string ToString()
			{
				if (Prefix == 0)
				{
					return $"[i/s{Stack}:{ID}]";
				}
				return $"[i/p{Prefix}:{ID}]";
			}
		}
		public class Commodity
		{
			public int ID { get; }
			public ItemInfo[] Items { get; }
			public ItemInfo[] Prices { get; }
			public string Text { get; }
			public Commodity(int id)
			{
				ID = id;
				switch (id)
				{
					case 1:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.PsychoKnife, prefix: PrefixID.Broken)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.LunarOre, stack: 600),
								new ItemInfo(ItemID.GolemPetItem)
							};
						}
						break;
					case 2:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.DaoofPow)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.LifeFruit, stack: 4),
								new ItemInfo(ItemID.CrimsonHeart)
							};
						}
						break;
					case 3:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.Uzi)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.CactusChest, stack: 10),
								new ItemInfo(ItemID.MagicLantern)
							};
						}
						break;
					case 4:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.SniperRifle, prefix: PrefixID.Broken)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.Penguin, stack: 8),
								new ItemInfo(ItemID.PumpkingPetItem)
							};
						}
						break;
					case 5:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.ChlorophyteShotbow)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.SlimeChest, stack: 10),
								new ItemInfo(ItemID.WispinaBottle)
							};
						}
						break;
					case 6:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.ToxicFlask)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.Hellstone, stack: 150),
								new ItemInfo(ItemID.ShadowOrb)
							};
						}
						break;
					case 7:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.CursedFlames)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.Bone, stack: 100),
								new ItemInfo(ItemID.FairyBell)
							};
						}
						break;
					case 8:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.StaffofEarth)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.RainbowBrick, stack: 300),
								new ItemInfo(ItemID.ScalyTruffle)
							};
						}
						break;
					case 21:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.MoonlordBullet, stack: 30)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.MythrilBar, stack: 6),
							};
						}
						break;
					case 22:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.MoonlordArrow, stack: 20)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.OrichalcumBar, stack: 6),
							};
						}
						break;
					case 51:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.GoblinBattleStandard)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.TitaniumOre, stack: 30),
								// new ItemInfo(ItemID.QueenSlimeMountSaddle)
							};
						}
						break;
					case 52:
						{
							Items = new ItemInfo[]
							{
								new ItemInfo(ItemID.GoblinBattleStandard)
							};
							Prices = new ItemInfo[]
							{
								new ItemInfo(ItemID.AdamantiteOre, stack: 30),
								// new ItemInfo(ItemID.QueenSlimeMountSaddle)
							};
						}
						break;
				}
				Text =  string.Join("", (object[])Items) + ": " + string.Join("", (object[])Prices);
			}
			public override string ToString()
			{
				return Text;
			}
		}

		public Commodity[] Commodities
		{
			get;
			private set;
		}
		public PlayerIdentity TargetParty { get; }

		public Shop(PlayerIdentity targetParty)
		{
			TargetParty = targetParty;
		}
		public void Reset()
		{
			if (TargetParty == PlayerIdentity.Traitor)
			{
				Commodities = new Commodity[8];
				var weaponIDs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
				SurvivalCrisis.Rand.Shuffle(weaponIDs);
				Commodities[0] = new Commodity(weaponIDs[0]);
				Commodities[1] = new Commodity(weaponIDs[1]);
				Commodities[2] = new Commodity(weaponIDs[2]);
				Commodities[3] = new Commodity(weaponIDs[3]);
				Commodities[4] = new Commodity(21);
				Commodities[5] = new Commodity(22);
				Commodities[6] = new Commodity(51);
				Commodities[7] = new Commodity(52);
			}
		}
		public void DisplayTo(GamePlayer player)
		{
			var title = TargetParty == PlayerIdentity.Traitor ? "————背叛者商店————" : "————生存者商店————";
			player.SendText(title, Color.CadetBlue);
			for (int i = 0; i < Commodities.Length; i++)
			{
				player.SendText($"[{i}]" + Commodities[i], Color.DarkBlue);
			}
		}
		public void DisplayToByStatusText(GamePlayer player)
		{
			var title = TargetParty == PlayerIdentity.Traitor ? "————[c/f33e1f:背叛者商店]————" : "————[c/5f9ea0:生存者商店]————";
			player.AddStatusMessage(title);
			for (int i = 0; i < Commodities.Length; i++)
			{
				player.AddStatusMessage(Commodities[i].ToString());
			}
		}
		public void TryBuy(GamePlayer player, int index)
		{
			if (index < 0 || Commodities.Length <= index)
			{
				player.SendText("无效的序号", Color.YellowGreen);
			}
			else
			{
				var commodity = Commodities[index];
				var matched = 0;
				foreach(var item in commodity.Prices)
				{
					for (int i = 0; i < player.Inventory.Count; i++)
					{
						if (player.Inventory[i].Type == item.ID && player.Inventory[i].Stack >= item.Stack)
						{
							matched++;
						}
					}
				}
				if (matched == commodity.Prices.Length)
				{
					foreach (var item in commodity.Prices)
					{
						for (int i = 0; i < player.Inventory.Count; i++)
						{
							if (player.Inventory[i].Type == item.ID && player.Inventory[i].Stack >= item.Stack)
							{
								player.Inventory[i].Stack -= item.Stack;
							}
						}
					}
					foreach (var item in commodity.Items)
					{
						player.TSPlayer.GiveItem(item.ID, item.Stack, item.Prefix);
					}
					player.SendText("购买成功", Color.DarkGreen);
				}
				else
				{
					player.SendText("购买失败", Color.GreenYellow);
				}
			}
		}
		public void TryBuyFromPiggy(GamePlayer player)
		{
			for (int index = 0; index < Commodities.Length; index++)
			{
				TryBuyFromPiggy(player, index);
			}
		}
		public void TryBuyFromPiggy(GamePlayer player, int index)
		{
			if (index < 0 || Commodities.Length <= index)
			{
				player.SendText("无效的序号", Color.YellowGreen);
			}
			else
			{
				var commodity = Commodities[index];
				var matched = 0;
				foreach (var item in commodity.Prices)
				{
					for (int i = 0; i < player.Piggybank.Count; i++)
					{
						if (player.Piggybank[i].Type == item.ID && player.Piggybank[i].Stack >= item.Stack)
						{
							matched++;
						}
					}
				}
				if (matched == commodity.Prices.Length)
				{
					foreach (var item in commodity.Prices)
					{
						for (int i = 0; i < player.Piggybank.Count; i++)
						{
							if (player.Piggybank[i].Type == item.ID && player.Piggybank[i].Stack >= item.Stack)
							{
								player.Piggybank[i].Stack -= item.Stack;
							}
						}
					}
					foreach (var item in commodity.Items)
					{
						player.TSPlayer.GiveItem(item.ID, item.Stack, item.Prefix);
					}
					player.SendText("购买成功", Color.DarkGreen);
				}
				else
				{
					// player.SendText("购买失败", Color.GreenYellow);
				}
			}
		}
	}
}
