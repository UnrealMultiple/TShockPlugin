using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using Terraria.GameContent.Creative;
using Terraria.GameContent.NetModules;

namespace SurvivalCrisis
{
	using NetInventory = Nets.NetInventory;
	using NetPiggybank = Nets.NetPiggybank;
	public class GamePlayer
	{
		#region Fields
		private int timer;
		private PlayerIdentity identity;
		#endregion
		#region Properties
		public int Index { get; }
		public PlayerData Data { get; }
		public NetInventory Inventory { get; }
		public NetPiggybank Piggybank { get; }
		public Queue<Effect> Effects { get; }
		public bool IsGhost
		{
			get;
			set;
		}
		public PlayerIdentity Party
		{
			get;
			set;
		}
		public PlayerIdentity Identity
		{
			get
			{
				return identity;
			}
			set
			{
				identity = value;
				if (SurvivalCrisis.Instance.IsInGame)
				{
					switch (identity)
					{
						case PlayerIdentity.Traitor:
							SendText(Texts.YouAreTraitor, Color.Yellow);
							break;
						case PlayerIdentity.Survivor:
							SendText(Texts.YouAreSurvivor, Color.Yellow);
							break;
					}
				}
			}
		}
		public Player TPlayer
		{
			get => Main.player[Index];

		}
		public TSPlayer TSPlayer
		{
			get => TShock.Players[Index];
		}
		public string Name
		{
			get => TPlayer.name;
		}
		public string Prefix
		{
			get => SurvivalCrisis.Instance.Prefixs[Data.CurrentPrefixID];
		}
		public string Title
		{
			get => SurvivalCrisis.Instance.Titles[Data.CurrentTitleID];
		}
		public Item HeldItem
		{
			get => TPlayer.HeldItem;
		}
		public int Life
		{
			get
			{
				return TPlayer.statLife;
			}
			set
			{
				TPlayer.statLife = value;
				TSPlayer.SendData(PacketTypes.PlayerHp, string.Empty, Index);
			}
		}
		public int LifeMax
		{
			get
			{
				return TPlayer.statLifeMax2;
			}
			set
			{
				TPlayer.statLifeMax = value;
				TSPlayer.SendData(PacketTypes.PlayerHp, string.Empty, Index);
			}
		}
		public int ManaMax
		{
			get
			{
				return TPlayer.statManaMax2;
			}
			set
			{
				TPlayer.statManaMax = value;
				TSPlayer.SendData(PacketTypes.PlayerMana, string.Empty, Index);
			}
		}
		public int Team
		{
			get => TSPlayer.Team;
			set => TSPlayer.SetTeam(value);
		}
		public bool Pvp
		{
			get
			{
				return TPlayer.hostile;
			}
			set
			{
				TPlayer.hostile = value;
				TSPlayer.All.SendData(PacketTypes.TogglePvp, string.Empty, Index);
			}
		}
		public bool IsGuest
		{
			get => Data == null;
		}
		public string LastStatusMessage
		{
			get;
			private set;
		}
		public string StatusMessage
		{
			get;
			set;
		}

		public int KillingCount
		{
			get;
			set;
		}
		/// <summary>
		/// 当前游戏的
		/// </summary>
		public int KilledCount
		{
			get;
			set;
		}
		/// <summary>
		/// 当前游戏的
		/// </summary>
		public int DamageCaused
		{
			get;
			set;
		}
		public int ChestsOpened
		{
			get;
			set;
		}
		public int SurvivedFrames
		{
			get;
			set;
		}
		public double PerformanceScore
		{
			get;
			private set;
		}
		public bool CanVote
		{
			get;
			set;
		}
		public int VotedCount
		{
			get;
			set;
		}
		public int WarpingCountDown
		{
			get;
			set;
		}
		public int WarpingCount
		{
			get;
			set;
		}
		public int ChatCount
		{
			get;
			set;
		}
		public int GoldenKeyFound
		{
			get;
			set;
		}
		#endregion
		#region Ctor
		public GamePlayer(int index, PlayerData data)
		{
			Index = index;
			Data = data;
			Inventory = new NetInventory(this);
			Piggybank = new NetPiggybank(this);
			Effects = new Queue<Effect>();
			identity = PlayerIdentity.Watcher;
		}
		#endregion
		#region Methods
		public bool Equipped(int itemType)
		{
			return TPlayer.armor.Count(item => item.active && item.type == itemType) > 0;
		}
		public void OnHurt(GetDataHandlers.PlayerDamageEventArgs args)
		{
			if (args.PVP)
			{
				var source = SurvivalCrisis.Instance.Players[args.PlayerDeathReason._sourcePlayerIndex];
				if (source.Identity == PlayerIdentity.Traitor)
				{
					source.DamageCaused += args.Damage;
				}
			}
		}
		public void OnStrikeNpc(NpcStrikeEventArgs args)
		{
			if (Identity == PlayerIdentity.Watcher)
			{
				args.Handled = true;
				return;
			}
			var index = args.Npc.realLife > 0 ? args.Npc.realLife : args.Npc.whoAmI;
			if (Identity == PlayerIdentity.Survivor)
			{
				if (index == SurvivalCrisis.Instance.FinalBossIndex)
				{
					DamageCaused += (int)Main.CalculateDamageNPCsTake(args.Damage, args.Npc.defense);
				}
			}
		}
		public void OnKill(GetDataHandlers.KillMeEventArgs args)
		{
			// var reason = PlayerDeathReason.LegacyEmpty();
			KilledCount++;
			if (args.Pvp)
			{
				var killer = SurvivalCrisis.Instance.Players[args.PlayerDeathReason._sourcePlayerIndex];
				killer.KillingCount++;
				if (killer.Party == PlayerIdentity.Survivor && Party == PlayerIdentity.Traitor)
				{
					if (!SurvivalCrisis.Instance.IsFinalBattleTime && !killer.HasPrefix(-2))
					{
						killer.AddPrefix(-2);
					}
				}
				else if (killer.Party == PlayerIdentity.Traitor && SurvivalCrisis.Instance.GameTime <= SurvivalCrisis.NightTime + 2 * 60 * 60)
				{
					if (!killer.HasPrefix(-7))
					{
						killer.AddPrefix(-7);
					}
				}
			}
			if (!args.Pvp && !SurvivalCrisis.Instance.IsFinalBattleTime && SurvivalCrisis.Instance.TraitorEMPTime == 0)
			{
				var pos = SurvivalCrisis.Regions.Hall.Center;
				TSPlayer.Spawn(pos.X, pos.Y, PlayerSpawnContext.ReviveFromDeath);
				SurvivalCrisis.Instance.BCToAll(Index + "号失去了意识...", Color.Yellow);
				args.Handled = true;
				return;
			}
			DropItems();
			ToGhost();
			SurvivedFrames = SurvivalCrisis.Instance.GameTime;
			switch (Party)
			{
				case PlayerIdentity.Survivor:
					Data.SurvivorDatas.KillingCount += KillingCount;
					Data.SurvivorDatas.KilledCount += KilledCount;
					Data.SurvivorDatas.DamageCaused += DamageCaused;
					Data.SurvivorDatas.MaxSurvivalFrames = Math.Max(Data.SurvivorDatas.MaxSurvivalFrames, SurvivalCrisis.Instance.GameTime);
					break;
				case PlayerIdentity.Traitor:
					Data.TraitorDatas.KillingCount += KillingCount;
					Data.TraitorDatas.KilledCount += KilledCount;
					Data.TraitorDatas.DamageCaused += DamageCaused;
					Data.TraitorDatas.MaxSurvivalFrames = Math.Max(Data.TraitorDatas.MaxSurvivalFrames, SurvivalCrisis.Instance.GameTime);
					break;
			}
			identity = PlayerIdentity.Watcher;
			// NetMessage.SendPlayerDeath(Index, reason, short.MaxValue, 1, false, -1, Index);
			SurvivalCrisis.Instance.BCToAll(Texts.SomeoneKilled, Color.Yellow);
			TSPlayer.Spawn(PlayerSpawnContext.ReviveFromDeath, 0);
		}
		public void OnGameEnd()
		{
			ToActive();
			Identity = PlayerIdentity.Watcher;
			Team = 0;
			ClearEffects();
			if (ChatCount == 0)
			{
				if (!HasPrefix(-1))
				{
					AddPrefix(-1);
				}
			}
			if (KillingCount >= 4)
			{
				if (!HasPrefix(-3))
				{
					AddPrefix(-3);
				}
			}
			if (ChestsOpened >= 75)
			{
				if (!HasPrefix(-4))
				{
					AddPrefix(-4);
				}
			}
			if (WarpingCount == 0 && KilledCount == 0)
			{
				if (!HasPrefix(-5))
				{
					AddPrefix(-5);
				}
			}
			if (GoldenKeyFound >= 2)
			{
				if (!HasPrefix(-8))
				{
					AddPrefix(-8);
				}
			}
		}
		public void ToGhost()
		{
			if (TSPlayer == null)
			{
				return;
			}
			TSPlayer.RespawnTimer = int.MaxValue;
			IsGhost = true;
			NetMessage.SendData(14, -1, Index, null, Index, false.GetHashCode());
			TSPlayer.GodMode = true;
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(Index, true);

			if (false)
			{
				NetMessage.SendData(4, -1, Index, null, Index, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(13, -1, Index, null, Index, 0f, 0f, 0f, 0, 0, 0);
			}
		}
		public void SetGodmode(bool value)
		{
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(Index, value);
		}
		public void ToActive()
		{
			TPlayer.active = true;
			IsGhost = false;
			var power = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			power.SetEnabledState(Index, false);
			TSPlayer.GodMode = false;
			NetMessage.SendData(14, -1, Index, null, Index, true.GetHashCode());
			if (true)
			{
				NetMessage.SendData(4, -1, Index, null, Index, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(13, -1, Index, null, Index, 0f, 0f, 0f, 0, 0, 0);
			}
		}
		public void Update()
		{
			if (TSPlayer == null)
			{
				return;
			}
			timer++;
			if (timer % 60 == 0 && StatusMessage != null)
			{
				SendStatusMessage(StatusMessage);
				LastStatusMessage = StatusMessage;
				StatusMessage = null;
			}
			if (!SurvivalCrisis.Instance.IsInGame)
			{
				return;
			}
			if (timer % 60 == 0)
			{
				if (!SurvivalCrisis.Regions.GamingZone.InRange(this))
				{
					TeleportTo(SurvivalCrisis.Regions.Hall.Center);
				}
				if (Equipped(ItemID.DiscountCard))
				{
					SurvivalCrisis.Instance.TraitorShop.DisplayToByStatusText(this);
				}
				if (identity == PlayerIdentity.Traitor)
				{
					if (TPlayer.FindBuffIndex(BuffID.Battle) == -1)
					{
						TSPlayer.SetBuff(BuffID.Battle, 60 * 3000);
					}
					if (Equipped(ItemID.SpectreGoggles))
					{
						var players = SurvivalCrisis.Instance.Players.Where(p => p?.Identity == PlayerIdentity.Survivor);
						foreach (var player in players)
						{
							var dist = player.TPlayer.Distance(TPlayer.Center) / 16;
							var msg = $@"{player.Index}号:  {(int)dist}格";
							if (SurvivalCrisis.Instance.IsMagnetStorm)
							{
								msg = $@"烫烫烫:  锟斤拷";
							}
							var textPos = player.TPlayer.Center - TPlayer.Center;
							textPos.Normalize();
							textPos *= 16 * 20;
							Utils.SendCombatText(msg, Color.MediumPurple, TPlayer.Center + textPos, Index);
						}
					}
				}
				if (SurvivalCrisis.Instance.CurrentTask != null)
				{
					AddStatusMessage("当前任务: " + SurvivalCrisis.Instance.CurrentTask.Name);
					AddStatusMessage(SurvivalCrisis.Instance.CurrentTask.CurrentProcess());
				}
			}
			if (SurvivalCrisis.Instance.IsInGame && identity == PlayerIdentity.Watcher)
			{
				if (!IsGhost)
				{
					ToGhost();
				}
			}
			if (IsGhost)
			{
				if (Pvp)
				{
					Pvp = false;
				}
				if (Team != 0)
				{
					Team = 0;
				}
				TSPlayer.SetBuff(BuffID.Invisibility, short.MaxValue);
				if (TPlayer.active)
				{
					ToGhost();
				}
			}
			else
			{
				if (!TPlayer.active)
				{
					ToActive();
				}
			}
		}
		public void TeleportTo(Point pos)
		{
			TeleportTo(new Vector2(pos.X, pos.Y) * 16);
		}
		public void TeleportTo(Vector2 pos)
		{
			TSPlayer.Teleport(pos.X, pos.Y);
		}
		public void DropItems()
		{
			var player = TPlayer;
			var box = new Vector2(player.width, player.height);
			var pos = player.Center;
			for (int i = 0; i < player.inventory.Length - 1; i++)
			{
				var item = player.inventory[i];
				var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
				TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
				item.netDefaults(0);
			}
			for (int i = 0; i < player.miscEquips.Length; i++)
			{
				var item = player.miscEquips[i];
				var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
				TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
				item.netDefaults(0);
			}
			for (int i = 0; i < 3; i++)
			{
				var item = player.armor[i];
				var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
				TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
				item.netDefaults(0);
			}
			for (int i = 5; i < 13; i++)
			{
				var item = player.armor[i];
				var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
				TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
				item.netDefaults(0);
			}
			for (int i = 15; i < player.armor.Length; i++)
			{
				var item = player.armor[i];
				var idx = Item.NewItem(new EntitySource_DebugCommand(), pos, box, item.type, item.stack, false, item.prefix);
				TSPlayer.All.SendData(PacketTypes.ItemDrop, "", idx);
				item.netDefaults(0);
			}
			for (int i = 0; i < 59; i++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, i, TPlayer.inventory[i].prefix);
			}
			for (int j = 0; j < TPlayer.armor.Length; j++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 59 + j, TPlayer.armor[j].prefix);
			}
			for (int k = 0; k < TPlayer.dye.Length; k++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 79 + k, TPlayer.dye[k].prefix);
			}
			for (int l = 0; l < TPlayer.miscEquips.Length; l++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 89 + l, TPlayer.miscEquips[l].prefix);
			}
			for (int m = 0; m < TPlayer.miscDyes.Length; m++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 94 + m, TPlayer.miscDyes[m].prefix);
			}
		}
		public void Reset()
		{
			DamageCaused = 0;
			KilledCount = 0;
			KillingCount = 0;
			PerformanceScore = 0;
			ChestsOpened = 0;
			SurvivedFrames = 0;
			VotedCount = 0;
			WarpingCountDown = 0;
			WarpingCount = 0;
			ChatCount = 0;
			GoldenKeyFound = 0;
			SetGodmode(false);
			ResetInventory();
		}
		public void ResetInventory()
		{
			Piggybank.Clear();
			var inventory = TPlayer.inventory;
			for (int i = 0; i < inventory.Length; i++)
			{
				inventory[i].TurnToAir();
			}
			for (int i = 0; i < TPlayer.armor.Length; i++)
			{
				TPlayer.armor[i].TurnToAir();
			}
			for (int i = 0; i < TPlayer.miscEquips.Length; i++)
			{
				TPlayer.miscEquips[i].TurnToAir();
			}
			if (Identity != PlayerIdentity.Watcher)
			{
				int t = 0;
				inventory[t++].SetDefaults(ItemID.MagicMirror);
				inventory[t++].SetDefaults(ItemID.PlatinumBroadsword);
				inventory[t++].SetDefaults(ItemID.ReaverShark);
				inventory[t++].SetDefaults(ItemID.SawtoothShark);
				inventory[t++].SetDefaults(ItemID.TungstenBow);
				inventory[t++].SetDefaults(ItemID.SlimeStaff);
				inventory[t++].SetDefaults(ItemID.SpectreBoots);
				inventory[t++].SetDefaults(ItemID.MoneyTrough);
				inventory[t++].SetDefaults(ItemID.AncientChisel);
				inventory[t++].SetDefaults(ItemID.LuckyHorseshoe);
				inventory[t++].SetDefaults(ItemID.EoCShield);
				inventory[t++].SetDefaults(ItemID.FamiliarWig);
				inventory[t++].SetDefaults(ItemID.FamiliarShirt);
				inventory[t++].SetDefaults(ItemID.FamiliarPants);
				inventory[t++].SetDefaults(ItemID.BatHook);
				inventory[t++].SetDefaults(ItemID.WeatherRadio);
				inventory[t].SetDefaults(ItemID.MusketBall);
				inventory[t].stack = 150;
				t++;
				inventory[t].SetDefaults(ItemID.WoodenArrow);
				inventory[t].stack = 100;
				t++;
				inventory[t].SetDefaults(ItemID.Wood);
				inventory[t].stack = 100;
				t++;
				inventory[t].SetDefaults(ItemID.Torch);
				inventory[t].stack = 50;
				t++;
				inventory[t].SetDefaults(ItemID.Bomb);
				inventory[t].stack = 20;
				t++;
				inventory[t].SetDefaults(ItemID.LesserHealingPotion);
				inventory[t].stack = 5;
				t++;
				inventory[t].SetDefaults(ItemID.NightOwlPotion);
				inventory[t].stack = 2;
				t++;
				inventory[t].SetDefaults(ItemID.ShinePotion);
				inventory[t].stack = 2;
				t++;
				inventory[t].SetDefaults(ItemID.SpelunkerPotion);
				inventory[t].stack = 2;
				t++;
				inventory[t].SetDefaults(ItemID.MiningPotion);
				inventory[t].stack = 2;
				t++;
				inventory[t].SetDefaults(ItemID.GravitationPotion);
				inventory[t].stack = 1;
				t++;
				inventory[t].SetDefaults(ItemID.GoldCoin);
				inventory[t].stack = 2;
				t++;

				if (Identity == PlayerIdentity.Traitor)
				{
					inventory[t++].SetDefaults(ItemID.PhoenixBlaster);
					inventory[t++].SetDefaults(ItemID.SpectreGoggles);
					inventory[t++].SetDefaults(ItemID.DiscountCard);
					inventory[t].SetDefaults(ItemID.ChlorophyteBullet);
					inventory[t].stack = 70;
					t++;
					inventory[t].SetDefaults(ItemID.WrathPotion);
					inventory[t].stack = 3;
					t++;
					inventory[t].SetDefaults(ItemID.RagePotion);
					inventory[t].stack = 3;
					t++;
				}
			}
			for (int i = 0; i < 59; i++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, i, TPlayer.inventory[i].prefix);
			}
			for (int j = 0; j < TPlayer.armor.Length; j++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 59 + j, TPlayer.armor[j].prefix);
			}
			for (int k = 0; k < TPlayer.dye.Length; k++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 79 + k, TPlayer.dye[k].prefix);
			}
			for (int l = 0; l < TPlayer.miscEquips.Length; l++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 89 + l, TPlayer.miscEquips[l].prefix);
			}
			for (int m = 0; m < TPlayer.miscDyes.Length; m++)
			{
				TSPlayer.SendData(PacketTypes.PlayerSlot, "", Index, 94 + m, TPlayer.miscDyes[m].prefix);
			}

			LifeMax = 200;
			ManaMax = 40;
		}
		public void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0, float number3 = 0, float number4 = 0, int number5 = 0)
		{
			TSPlayer.SendData(msgType, text, number, number2, number3, number4, number5);
		}
		public void AddPerformanceScore(double score, string reason)
		{
			PerformanceScore += score;
			SendText($"表现分+{score}({reason})", Color.YellowGreen);
		}
		public bool HasBuff(int buffID)
		{
			return TPlayer.FindBuffIndex(buffID) != -1;
		}
		public bool IsValid()
		{
			return TSPlayer != null && TSPlayer.Account.ID == Data.UserID;
		}
		public void HideName()
		{
			// var name = Name;
			// TPlayer.name = Index + "号";
			// NetMessage.TrySendData((int)PacketTypes.PlayerInfo, -1, Index, null, Index);
			// TPlayer.name = name;


			var name = TPlayer.name;
			var hair = TPlayer.hair;
			var hairColor = TPlayer.hairColor;
			var skinColor = TPlayer.skinColor;
			var skinVariant = TPlayer.skinVariant;
			var pantsColor = TPlayer.pantsColor;
			var shirtColor = TPlayer.shirtColor;
			var shoeColor = TPlayer.shoeColor;
			var shoe = TPlayer.shoe;


			TPlayer.name = Index + "号";
			TPlayer.hair = 0;
			TPlayer.hairColor = new Color(215, 90, 55);
			TPlayer.skinColor = new Color(255, 125, 90);
			TPlayer.skinVariant = 0;
			TPlayer.pantsColor = new Color(255, 230, 175);
			TPlayer.shirtColor = new Color(175, 165, 140);
			TPlayer.shoeColor = new Color(160, 105, 60);
			TPlayer.shoe = 0;

			NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, Index, null, Index);

			TPlayer.name = name;
			TPlayer.hair = hair;
			TPlayer.hairColor = hairColor;
			TPlayer.skinColor = skinColor;
			TPlayer.skinVariant = skinVariant;
			TPlayer.pantsColor = pantsColor;
			TPlayer.shirtColor = shirtColor;
			TPlayer.shoeColor = shoeColor;
			TPlayer.shoe = shoe;
		}

		public void ToNextTitleID()
		{
			int t = Data.UnlockedTitles.FindIndex(id => id == Data.CurrentTitleID);
			t++;
			t %= Data.UnlockedTitles.Count;
			Data.CurrentTitleID = Data.UnlockedTitles[t];
			if (Data.CurrentTitleID != 0)
			{
				SendText($"你的称号已更改为 {Title}", Color.White);
			}
		}
		public void ToNextPrefixID()
		{
			int t = Data.UnlockedPrefixs.FindIndex(id => id == Data.CurrentPrefixID);
			t++;
			t %= Data.UnlockedPrefixs.Count;
			Data.CurrentPrefixID = Data.UnlockedPrefixs[t];
			if (Data.CurrentPrefixID != 0)
			{
				SendText($"你的前缀已更改为 {Prefix}", Color.White);
			}
		}
		public bool HasPrefix(int prefixID)
		{
			return Data.UnlockedPrefixs.FindIndex(id => id == prefixID) > 0;
		}
		public bool HasTitle(int titleID)
		{
			return Data.UnlockedTitles.FindIndex(id => id == titleID) > 0;
		}
		public void AddPrefix(int prefixID)
		{
			Data.UnlockedPrefixs.Add(prefixID);
			Data.UnlockedPrefixs.Sort();
			SendText($"你已获得前缀 {SurvivalCrisis.Instance.Prefixs[prefixID]}", Color.White);
		}
		public void AddTitle(int titleID)
		{
			Data.UnlockedTitles.Add(titleID);
			Data.UnlockedTitles.Sort();
			SendText($"你已获得称号 {SurvivalCrisis.Instance.Titles[titleID]}", Color.White);
		}
		public void BuyRandomPrefix(int cost)
		{
			if (Data.Coins < cost)
			{
				SendText($"你的可兑换积分不足(至少需要{cost}分)", Color.YellowGreen);
				return;
			}
			if (Data.UnlockedPrefixs.Count(id => id > 0) == SurvivalCrisis.Instance.Prefixs.Count - 8 - 1) // 特殊前缀和空前缀id为非正
			{
				SendText("你已解锁所有前缀", Color.DarkGreen);
				return;
			}
			int t = 0;
			int[] prefixesCanGet = new int[SurvivalCrisis.Instance.Prefixs.Count - 8 - 1 - Data.UnlockedPrefixs.Count(id => id > 0)];
			for(int i = 1;i<SurvivalCrisis.Instance.Prefixs.Count - 8 - 1;i++)
			{
				if(!HasPrefix(i))
				{
					prefixesCanGet[t++] = i;
				}
			}
			AddPrefix(SurvivalCrisis.Rand.Next(prefixesCanGet));
			Data.Coins -= cost;
		}
		public void BuyRandomTitle(int cost)
		{
			if (Data.Coins < cost)
			{
				SendText($"你的可兑换积分不足(至少需要{cost}分)", Color.YellowGreen);
				return;
			}
			if (Data.UnlockedTitles.Count(id => id > 0) == SurvivalCrisis.Instance.Titles.Count - 1) // 特殊称号和空称号id为非正
			{
				SendText("你已解锁所有称号", Color.DarkGreen);
				return;
			}
			int t = 0;
			int[] titlesCanGet = new int[SurvivalCrisis.Instance.Titles.Count - 8 - 1 - Data.UnlockedTitles.Count(id => id > 0)];
			for (int i = 1; i < SurvivalCrisis.Instance.Titles.Count - 1; i++)
			{
				if (!HasTitle(i))
				{
					titlesCanGet[t++] = i;
				}
			}
			AddTitle(SurvivalCrisis.Rand.Next(titlesCanGet));
			Data.Coins -= cost;
		}
		#region Effects
		public void AddEffect(Effect effect)
		{
			effect.Apply();
			Effects.Enqueue(effect);
		}
		public void UpdateEffects()
		{
			int count = Effects.Count;
			while (count-- > 0)
			{
				var effect = Effects.Dequeue();
				effect.Update();
				if (!effect.IsEnd)
				{
					Effects.Enqueue(effect);
				}
			}
		}
		public void RemoveEffect(Effect effect)
		{
			int count = Effects.Count;
			while (count-- > 0)
			{
				var e = Effects.Dequeue();
				if (e == effect)
				{
					e.Abort();
					break;
				}
				Effects.Enqueue(e);
			}
		}
		public void ClearEffects()
		{
			foreach (var effect in Effects)
			{
				effect.Abort();
			}
			Effects.Clear();
		}
		#endregion
		#region Sends
		public void AddStatusMessage(string msg, Color color)
		{
			var hex = color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
			AddStatusMessage("[c/" + hex + ":" + msg + "]");
		}
		public void AddStatusMessage(string msg)
		{
			StatusMessage += msg + "\n";
		}
		public void SendStatusMessage(string msg)
		{
			TSPlayer.SendData(PacketTypes.Status, "\n\n\\nn\n\n\n\n\n\n\n" + msg);
		}
		public void SendText(string msg, Color color)
		{
			TSPlayer.SendMessage(msg, color);
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return Name;
		}
		#endregion
		#endregion
		#region Statics
		public static GamePlayer Guest(int index) => new GamePlayer(index, null);
		#endregion
	}
}
