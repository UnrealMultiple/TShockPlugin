using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace SurvivalCrisis.MapGenerating
{
	public class ChestLevel
	{
		private struct ItemInfo
		{
			public int ID;
			public byte Prefix;
			public short MinStack;
			public short MaxStack;
			public ItemInfo(int id)
			{
				ID = id;
				Prefix = 0;
				MinStack = MaxStack = 1;
			}
			public ItemInfo(int id, int stack)
			{
				ID = id;
				Prefix = 0;
				MinStack = (short)stack;
				MaxStack = (short)stack;
			}
			public ItemInfo(int id, int minstack, int maxstack)
			{
				ID = id;
				Prefix = 0;
				MinStack = (short)minstack;
				MaxStack = (short)maxstack;
			}
			public Item ToItem()
			{
				var item = new Item();
				item.SetDefaults(ID);
				item.prefix = Prefix;
				item.stack = SurvivalCrisis.Rand.Next(MinStack, MaxStack + 1);
				return item;
			}
		}

		private ItemInfo[] Accessories;
		private ItemInfo[] Weapons;
		private ItemInfo[] Ammo;
		private ItemInfo[] Potions;
		private ItemInfo[] Ores;
		private ItemInfo[] Coins;
		private ItemInfo[] Lighters;
		private ItemInfo[] Healings;
		private ItemInfo[] Tools;
		private ItemInfo[] Mounts;
		private ItemInfo[] Sundries;

		/// <summary>
		/// 1: 仙人掌箱
		/// 2: 史莱姆箱
		/// 3: 大理石箱
		/// 4: 花岗岩箱
		/// 5: 玻璃箱(仅在空岛)
		/// 6: 血肉箱(仅在地狱)
		/// 7: 带锁蓝地牢箱(最为稀有)
		/// </summary>
		public int Level
		{
			get;
		}

		/// <summary>
		/// 1: 146
		/// 2: 193
		/// 3: 45
		/// 4: 385
		/// 5: 54(玻璃)
		/// 6: 195(血肉块)
		/// 7: 41(蓝地牢砖)
		/// </summary>
		public int IdentifierL
		{
			get;
		}
		/// <summary>
		/// 1: 202
		/// 2: 196
		/// 3: 177
		/// 4: 160
		/// 5: 315(珊瑚石块)
		/// 6: 194(骨块)
		/// 7: 541(回声块)
		/// </summary>
		public int IdentifierR
		{
			get;
		}

		public static readonly ChestLevel V1 = new ChestLevel(1);
		public static readonly ChestLevel V2 = new ChestLevel(2);
		public static readonly ChestLevel V3 = new ChestLevel(3);
		public static readonly ChestLevel V4 = new ChestLevel(4);
		public static readonly ChestLevel V5 = new ChestLevel(5);
		public static readonly ChestLevel V6 = new ChestLevel(6);
		public static readonly ChestLevel V7 = new ChestLevel(7);

		private ChestLevel(int level)
		{
			Level = level;
			switch(level)
			{
				case 1:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.NaturesGift),
							new ItemInfo(ItemID.Aglet),
							new ItemInfo(ItemID.BandofRegeneration),
							new ItemInfo(ItemID.BandofStarpower),
							new ItemInfo(ItemID.WaterWalkingBoots),
							new ItemInfo(ItemID.SharkToothNecklace),
							new ItemInfo(ItemID.PanicNecklace),
							new ItemInfo(ItemID.HoneyComb),
							new ItemInfo(ItemID.ExtendoGrip),
							new ItemInfo(ItemID.BrickLayer),
							new ItemInfo(ItemID.Toolbelt),
							new ItemInfo(ItemID.GreedyRing),
							new ItemInfo(ItemID.MagmaStone),
							new ItemInfo(ItemID.ObsidianSkullRose),
							new ItemInfo(ItemID.GoldHelmet),
							new ItemInfo(ItemID.GoldChainmail),
							new ItemInfo(ItemID.GoldGreaves),
							new ItemInfo(ItemID.SilverHelmet),
							new ItemInfo(ItemID.SilverChainmail),
							new ItemInfo(ItemID.SilverGreaves),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.AmberStaff),
							new ItemInfo(ItemID.RubyStaff),
							new ItemInfo(ItemID.DiamondStaff),
							new ItemInfo(ItemID.Vilethorn),
							new ItemInfo(ItemID.CrimsonRod),
							new ItemInfo(ItemID.RedPhaseblade),
							new ItemInfo(ItemID.BloodButcherer),
							new ItemInfo(ItemID.BoneSword),
							new ItemInfo(ItemID.Rally),
							new ItemInfo(ItemID.BallOHurt),
							new ItemInfo(ItemID.FruitcakeChakram),
							new ItemInfo(ItemID.TheUndertaker),
							new ItemInfo(ItemID.Musket),
							new ItemInfo(ItemID.Boomstick),
							new ItemInfo(ItemID.BabyBirdStaff),
							new ItemInfo(ItemID.BlandWhip),
							new ItemInfo(ItemID.MolotovCocktail, 15, 20),
							new ItemInfo(ItemID.ChainKnife),
							new ItemInfo(ItemID.Katana),
							new ItemInfo(ItemID.BladedGlove),
							new ItemInfo(ItemID.ThunderSpear),
							new ItemInfo(ItemID.ThunderStaff),
							new ItemInfo(ItemID.PlatinumHelmet),
							new ItemInfo(ItemID.PlatinumChainmail),
							new ItemInfo(ItemID.PlatinumGreaves),
							new ItemInfo(ItemID.TungstenHelmet),
							new ItemInfo(ItemID.TungstenChainmail),
							new ItemInfo(ItemID.TungstenGreaves),
							new ItemInfo(ItemID.FlinxStaff),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.IronOre, 15, 20),
							new ItemInfo(ItemID.GoldOre, 15, 20),
							new ItemInfo(ItemID.DemoniteOre, 8, 15),
							new ItemInfo(ItemID.CrimtaneOre, 8 , 15),
							new ItemInfo(ItemID.Obsidian, 15, 20),
							new ItemInfo(ItemID.Hellstone, 8, 10),
							new ItemInfo(ItemID.Meteorite, 8, 15),
							new ItemInfo(ItemID.CobaltOre, 3, 5),
							new ItemInfo(ItemID.PalladiumOre, 3, 5),
							new ItemInfo(ItemID.MythrilOre, 2, 4),
							new ItemInfo(ItemID.OrichalcumOre, 2, 4),
							new ItemInfo(ItemID.AdamantiteOre, 2, 3),
							new ItemInfo(ItemID.TitaniumOre, 2, 3),
							new ItemInfo(ItemID.LifeCrystal),
							new ItemInfo(ItemID.ManaCrystal)
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.IronskinPotion, 1, 2),
							new ItemInfo(ItemID.FeatherfallPotion, 1, 2),
							new ItemInfo(ItemID.RegenerationPotion, 1, 2),
							new ItemInfo(ItemID.MiningPotion, 1, 2),
							new ItemInfo(ItemID.SwiftnessPotion, 1, 2),
							new ItemInfo(ItemID.ShinePotion, 1, 2),
							new ItemInfo(ItemID.NightOwlPotion, 1, 2),
							new ItemInfo(ItemID.SpelunkerPotion, 1, 2),
							new ItemInfo(ItemID.BuilderPotion, 1, 2),
							new ItemInfo(ItemID.AppleJuice, 2, 3),
							new ItemInfo(ItemID.Sake, 2, 3),
							new ItemInfo(ItemID.BowlofSoup, 2, 3),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.SilverCoin, 20, 30)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.BonePickaxe),
							new ItemInfo(ItemID.FossilPickaxe)
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.Torch, 10, 15),
							new ItemInfo(ItemID.Glowstick, 5, 8)
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.FuzzyCarrot),
							new ItemInfo(ItemID.SlimySaddle)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.BorealWoodPlatform, 30, 50),
							new ItemInfo(ItemID.Bomb, 4, 6)
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.SilverBullet, 30, 40),
							new ItemInfo(ItemID.UnholyArrow, 10, 20),
							new ItemInfo(ItemID.JestersArrow, 5, 10)
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.HealingPotion, 1, 2),
							new ItemInfo(ItemID.ManaPotion, 2, 3)
						};
						break;
					}
				case 2:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.AnkletoftheWind),
							new ItemInfo(ItemID.IceSkates),
							new ItemInfo(ItemID.LavaCharm),
							new ItemInfo(ItemID.FlyingCarpet),
							new ItemInfo(ItemID.FleshKnuckles),
							new ItemInfo(ItemID.PutridScent),
							new ItemInfo(ItemID.ShinyRedBalloon),
							new ItemInfo(ItemID.BalloonPufferfish),
							new ItemInfo(ItemID.CloudinaBottle),
							new ItemInfo(ItemID.TsunamiInABottle),
							new ItemInfo(ItemID.BlizzardinaBottle),
							new ItemInfo(ItemID.FeralClaws),
							new ItemInfo(ItemID.ClimbingClaws),
							new ItemInfo(ItemID.CrossNecklace),
							new ItemInfo(ItemID.FrozenTurtleShell),
							new ItemInfo(ItemID.FartinaJar),
							new ItemInfo(ItemID.StingerNecklace),
							new ItemInfo(ItemID.SweetheartNecklace),
							new ItemInfo(ItemID.PygmyNecklace),
							new ItemInfo(ItemID.HiveBackpack),
							new ItemInfo(ItemID.BeeCloak),
							new ItemInfo(ItemID.MiningHelmet),
							new ItemInfo(ItemID.MiningShirt),
							new ItemInfo(ItemID.MiningPants),
							new ItemInfo(ItemID.FlinxFurCoat),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.FlintlockPistol),
							new ItemInfo(ItemID.BeeGun),
							new ItemInfo(ItemID.Starfury),
							new ItemInfo(ItemID.Muramasa),
							new ItemInfo(ItemID.AquaScepter),
							new ItemInfo(ItemID.EnchantedSword),
							new ItemInfo(ItemID.ThornChakram),
							new ItemInfo(ItemID.ThornWhip),
							new ItemInfo(ItemID.HornetStaff),
							new ItemInfo(ItemID.JungleYoyo),
							new ItemInfo(ItemID.Revolver),
							new ItemInfo(ItemID.BloodRainBow),
							new ItemInfo(ItemID.PainterPaintballGun),
							new ItemInfo(ItemID.Minishark),
							new ItemInfo(ItemID.BeeKeeper),
							new ItemInfo(ItemID.Code1),
							new ItemInfo(ItemID.ImpStaff),
							new ItemInfo(ItemID.NinjaHood),
							new ItemInfo(ItemID.NinjaShirt),
							new ItemInfo(ItemID.NinjaPants),
							new ItemInfo(ItemID.FossilHelm),
							new ItemInfo(ItemID.FossilShirt),
							new ItemInfo(ItemID.FossilPants),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.DemoniteOre, 10, 18),
							new ItemInfo(ItemID.CrimtaneOre, 10, 18),
							new ItemInfo(ItemID.Obsidian, 10, 25),
							new ItemInfo(ItemID.Hellstone, 10, 20),
							new ItemInfo(ItemID.Meteorite, 10, 20),
							new ItemInfo(ItemID.CobaltOre, 3, 5),
							new ItemInfo(ItemID.PalladiumOre, 3, 5),
							new ItemInfo(ItemID.MythrilOre, 3, 5),
							new ItemInfo(ItemID.PalladiumOre, 3, 5),
							new ItemInfo(ItemID.AdamantiteOre, 2, 4),
							new ItemInfo(ItemID.TitaniumOre, 2, 4),
							new ItemInfo(ItemID.LifeCrystal, 1, 2),
							new ItemInfo(ItemID.ManaCrystal, 2, 3),
							new ItemInfo(ItemID.BeeWax, 6, 8),
							new ItemInfo(ItemID.TissueSample, 6, 8),
							new ItemInfo(ItemID.ShadowScale, 6, 8)
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.IronskinPotion, 1, 2),
							new ItemInfo(ItemID.RegenerationPotion, 1, 2),
							new ItemInfo(ItemID.SwiftnessPotion, 1, 2),
							new ItemInfo(ItemID.ShinePotion, 1, 2),
							new ItemInfo(ItemID.NightOwlPotion, 1, 2),
							new ItemInfo(ItemID.SpelunkerPotion, 1, 2),
							new ItemInfo(ItemID.MiningPotion, 1, 2),
							new ItemInfo(ItemID.CalmingPotion, 1, 2),
							new ItemInfo(ItemID.HunterPotion, 1, 2),
							new ItemInfo(ItemID.BattlePotion, 1, 2),
							new ItemInfo(ItemID.ArcheryPotion, 1, 2),
							new ItemInfo(ItemID.BunnyStew, 2, 3),
							new ItemInfo(ItemID.Burger, 2, 3),
							new ItemInfo(ItemID.Escargot, 2, 3),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.GoldCoin, 1, 1),
							new ItemInfo(ItemID.SilverCoin, 60, 90),
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.DeathbringerPickaxe),
							new ItemInfo(ItemID.NightmarePickaxe)
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.WhiteTorch, 15, 20),
							new ItemInfo(ItemID.FairyGlowstick, 4, 6),
							new ItemInfo(ItemID.WhiteTorch, 15, 20),
							new ItemInfo(ItemID.FairyGlowstick, 4, 6),
							new ItemInfo(ItemID.WhiteTorch, 15, 20),
							new ItemInfo(ItemID.FairyGlowstick, 4, 6),
							new ItemInfo(ItemID.WhiteTorch, 15, 20),
							new ItemInfo(ItemID.FairyGlowstick, 4, 6),
							new ItemInfo(ItemID.ShadowOrb),
							new ItemInfo(ItemID.CrimsonHeart)
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.HoneyedGoggles),
							new ItemInfo(ItemID.AncientHorn),
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.WoodenCrateHard, 1, 3),
							new ItemInfo(ItemID.IronCrateHard, 1, 2),
							new ItemInfo(ItemID.Bomb, 3, 5),
							new ItemInfo(ItemID.Dynamite, 1, 2)
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.MeteorShot, 20, 30),
							new ItemInfo(ItemID.CursedBullet, 10, 15),
							new ItemInfo(ItemID.IchorBullet, 10, 15),
							new ItemInfo(ItemID.UnholyArrow, 15, 25),
							new ItemInfo(ItemID.JestersArrow, 10, 20),
							new ItemInfo(ItemID.HellfireArrow, 15, 25)
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.HealingPotion, 2, 3),
							new ItemInfo(ItemID.ManaPotion, 5, 6),
						};
						break;
					}
				case 3:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.FrogLeg),
							new ItemInfo(ItemID.CobaltShield),
							new ItemInfo(ItemID.SandstorminaBottle),
							new ItemInfo(ItemID.SunStone),
							new ItemInfo(ItemID.MoonStone),
							new ItemInfo(ItemID.BrainOfConfusion),
							new ItemInfo(ItemID.WormScarf),
							new ItemInfo(ItemID.MoonCharm),
							new ItemInfo(ItemID.PhilosophersStone),
							new ItemInfo(ItemID.WarriorEmblem),
							new ItemInfo(ItemID.RangerEmblem),
							new ItemInfo(ItemID.SorcererEmblem),
							new ItemInfo(ItemID.SummonerEmblem),
							new ItemInfo(ItemID.ManaCloak),
							new ItemInfo(ItemID.PowerGlove),
							new ItemInfo(ItemID.AmphibianBoots),
							new ItemInfo(ItemID.JellyfishDivingGear),
							new ItemInfo(ItemID.HerculesBeetle),
							new ItemInfo(ItemID.BoneGlove),
							new ItemInfo(ItemID.CreativeWings),
							new ItemInfo(ItemID.GladiatorHelmet),
							new ItemInfo(ItemID.GladiatorBreastplate),
							new ItemInfo(ItemID.GladiatorLeggings),
							new ItemInfo(ItemID.CrimsonHelmet),
							new ItemInfo(ItemID.CrimsonScalemail),
							new ItemInfo(ItemID.CrimsonGreaves),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.Handgun),
							new ItemInfo(ItemID.BeesKnees),
							new ItemInfo(ItemID.Sunfury),
							new ItemInfo(ItemID.MoltenFury),
							new ItemInfo(ItemID.DemonScythe),
							new ItemInfo(ItemID.SpaceGun),
							new ItemInfo(ItemID.MagicMissile),
							new ItemInfo(ItemID.VampireFrogStaff),
							new ItemInfo(ItemID.BoneWhip),
							new ItemInfo(ItemID.FlowerofFire),
							new ItemInfo(ItemID.QuadBarrelShotgun),
							new ItemInfo(ItemID.WaterBolt),
							new ItemInfo(ItemID.ZapinatorGray),
							new ItemInfo(ItemID.Valor),
							new ItemInfo(ItemID.Cascade),
							new ItemInfo(ItemID.FieryGreatsword),
							new ItemInfo(ItemID.Gi),
							new ItemInfo(ItemID.DiamondRobe),
							new ItemInfo(ItemID.RubyRobe),
							new ItemInfo(ItemID.WizardHat),
							new ItemInfo(ItemID.MagicHat),
							new ItemInfo(ItemID.GypsyRobe),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.Hellstone, 30, 45),
							new ItemInfo(ItemID.Meteorite, 30, 45),
							new ItemInfo(ItemID.CobaltOre, 7, 9),
							new ItemInfo(ItemID.PalladiumOre, 7, 9),
							new ItemInfo(ItemID.MythrilOre, 7, 9),
							new ItemInfo(ItemID.OrichalcumOre, 7, 9),
							new ItemInfo(ItemID.AdamantiteOre, 6, 7),
							new ItemInfo(ItemID.TitaniumOre, 6, 7),
							new ItemInfo(ItemID.LifeCrystal, 1, 2),
							new ItemInfo(ItemID.ManaCrystal, 3, 4),
							new ItemInfo(ItemID.LifeFruit, 2, 3),
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.EndurancePotion, 1, 2),
							new ItemInfo(ItemID.ManaRegenerationPotion, 1, 2),
							new ItemInfo(ItemID.TitanPotion, 1, 2),
							new ItemInfo(ItemID.InfernoPotion, 1, 2),
							new ItemInfo(ItemID.AmmoReservationPotion, 1, 2),
							new ItemInfo(ItemID.ObsidianSkinPotion, 1, 2),
							new ItemInfo(ItemID.CoffeeCup, 1, 2),
							new ItemInfo(ItemID.Milkshake, 1, 2),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.GoldCoin, 1, 2)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.CobaltPickaxe),
							new ItemInfo(ItemID.PalladiumPickaxe)
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.UltrabrightTorch, 20, 30),
							new ItemInfo(ItemID.SpelunkerGlowstick, 4, 6),
							new ItemInfo(ItemID.UltrabrightTorch, 20, 30),
							new ItemInfo(ItemID.SpelunkerGlowstick, 4, 6),
							new ItemInfo(ItemID.UltrabrightTorch, 20, 30),
							new ItemInfo(ItemID.SpelunkerGlowstick, 4, 6),
							new ItemInfo(ItemID.UltrabrightTorch, 20, 30),
							new ItemInfo(ItemID.SpelunkerGlowstick, 4, 6),
							new ItemInfo(ItemID.MagicLantern),
							new ItemInfo(ItemID.FairyBell)
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.BlessedApple),
							new ItemInfo(ItemID.ScalyTruffle)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.WoodenCrateHard, 2, 3),
							new ItemInfo(ItemID.IronCrateHard, 2, 3),
							new ItemInfo(ItemID.GoldenCrateHard, 1, 2),
							new ItemInfo(ItemID.Dynamite, 2, 3),
							new ItemInfo(ItemID.StickyDynamite, 2, 3),
							new ItemInfo(ItemID.BouncyDynamite, 2, 3),
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.ExplodingBullet, 30, 40),
							new ItemInfo(ItemID.GoldenBullet, 30, 40),
							new ItemInfo(ItemID.CrystalBullet, 20, 30),
							new ItemInfo(ItemID.IchorBullet, 20, 30),
							new ItemInfo(ItemID.CursedBullet, 20, 30),
							new ItemInfo(ItemID.HighVelocityBullet, 20, 30),
							new ItemInfo(ItemID.NanoBullet, 20, 30),
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.RestorationPotion, 4, 5),
							new ItemInfo(ItemID.GreaterHealingPotion, 3, 4),
							new ItemInfo(ItemID.GreaterManaPotion, 5, 8),
							new ItemInfo(ItemID.SuperManaPotion, 3, 5),
						};
						break;
					}
				case 4:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.AnkhCharm),
							new ItemInfo(ItemID.FrostsparkBoots),
							new ItemInfo(ItemID.DestroyerEmblem),
							new ItemInfo(ItemID.Tabi),
							new ItemInfo(ItemID.BlackBelt),
							new ItemInfo(ItemID.PaladinsShield),
							new ItemInfo(ItemID.MagicQuiver),
							new ItemInfo(ItemID.YoyoBag),
							new ItemInfo(ItemID.NecromanticScroll),
							new ItemInfo(ItemID.ArcaneFlower),
							new ItemInfo(ItemID.AvengerEmblem),
							new ItemInfo(ItemID.AngelWings),
							new ItemInfo(ItemID.DemonWings),
							new ItemInfo(ItemID.BeeHeadgear),
							new ItemInfo(ItemID.BeeBreastplate),
							new ItemInfo(ItemID.BeeGreaves),
							new ItemInfo(ItemID.JungleHat),
							new ItemInfo(ItemID.JungleShirt),
							new ItemInfo(ItemID.JunglePants)
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.HelFire),
							new ItemInfo(ItemID.Gatligator),
							new ItemInfo(ItemID.Flamelash),
							new ItemInfo(ItemID.SpiderStaff),
							new ItemInfo(ItemID.QueenSpiderStaff),
							new ItemInfo(ItemID.CoolWhip),
							new ItemInfo(ItemID.CrystalVileShard),
							new ItemInfo(ItemID.ClockworkAssaultRifle),
							new ItemInfo(ItemID.ChainGuillotines),
							new ItemInfo(ItemID.FlowerofFrost),
							new ItemInfo(ItemID.IceSickle),
							new ItemInfo(ItemID.Amarok),
							new ItemInfo(ItemID.ShadowFlameKnife),
							new ItemInfo(ItemID.ShadowFlameHexDoll),
							new ItemInfo(ItemID.ShadowFlameBow),
							new ItemInfo(ItemID.IceBow),
							new ItemInfo(ItemID.Marrow),
							new ItemInfo(ItemID.NimbusRod),
							new ItemInfo(ItemID.FireWhip),
							new ItemInfo(ItemID.PirateStaff),
							new ItemInfo(ItemID.SanguineStaff),
							new ItemInfo(ItemID.Bladetongue),
							new ItemInfo(ItemID.LaserRifle),
							new ItemInfo(ItemID.NecroHelmet),
							new ItemInfo(ItemID.NecroBreastplate),
							new ItemInfo(ItemID.NecroGreaves),
							new ItemInfo(ItemID.MoltenHelmet),
							new ItemInfo(ItemID.MoltenBreastplate),
							new ItemInfo(ItemID.MoltenGreaves),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.CobaltOre, 15, 20),
							new ItemInfo(ItemID.PalladiumOre, 15, 20),
							new ItemInfo(ItemID.MythrilOre, 15, 20),
							new ItemInfo(ItemID.OrichalcumOre, 15, 20),
							new ItemInfo(ItemID.AdamantiteOre, 15, 20),
							new ItemInfo(ItemID.TitaniumOre, 15, 20),
							new ItemInfo(ItemID.LifeCrystal, 2, 3),
							new ItemInfo(ItemID.ManaCrystal, 4, 5),
							new ItemInfo(ItemID.LifeFruit, 3, 4),
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.WrathPotion, 2, 3),
							new ItemInfo(ItemID.RagePotion, 2, 3),
							new ItemInfo(ItemID.LifeforcePotion, 2, 3),
							new ItemInfo(ItemID.SummoningPotion, 2, 3),
							new ItemInfo(ItemID.MagicPowerPotion, 3, 4),
							new ItemInfo(ItemID.SeafoodDinner, 3, 4),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.GoldCoin, 3, 4)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.MythrilPickaxe),
							new ItemInfo(ItemID.OrichalcumPickaxe),
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.WispinaBottle),
							new ItemInfo(ItemID.DD2PetGhost),
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.QueenSlimeMountSaddle),
							new ItemInfo(ItemID.ReindeerBells)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.WoodenCrateHard, 3, 5),
							new ItemInfo(ItemID.IronCrateHard, 3, 4),
							new ItemInfo(ItemID.GoldenCrateHard, 1, 2),
							new ItemInfo(ItemID.ThornHook),
							new ItemInfo(ItemID.IlluminantHook),
							new ItemInfo(ItemID.WormHook),
							new ItemInfo(ItemID.TendonHook),
							new ItemInfo(ItemID.QueenSlimeHook),
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.HolyArrow, 15, 20),
							new ItemInfo(ItemID.ChlorophyteArrow, 15, 20),
							new ItemInfo(ItemID.MoonlordArrow, 10, 15),
							new ItemInfo(ItemID.VenomBullet,40, 50),
							new ItemInfo(ItemID.IchorBullet, 40, 50),
							new ItemInfo(ItemID.CursedBullet, 40, 50),
							new ItemInfo(ItemID.CrystalBullet, 40, 50),
							new ItemInfo(ItemID.ChlorophyteBullet, 40, 50),
							new ItemInfo(ItemID.MoonlordBullet, 10, 20)
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.GreaterHealingPotion, 2, 3),
							new ItemInfo(ItemID.SuperHealingPotion, 1, 2),
							new ItemInfo(ItemID.GreaterManaPotion, 8, 10),
							new ItemInfo(ItemID.SuperManaPotion, 5, 8),
						};
						break;
					}
				case 5:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.FrozenWings),
							new ItemInfo(ItemID.HarpyWings),
							new ItemInfo(ItemID.FinWings),
							new ItemInfo(ItemID.FairyWings),
							new ItemInfo(ItemID.CelestialCuffs),
							new ItemInfo(ItemID.StarVeil),
							new ItemInfo(ItemID.GravityGlobe),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.FrozenWings),
							new ItemInfo(ItemID.HarpyWings),
							new ItemInfo(ItemID.FinWings),
							new ItemInfo(ItemID.FairyWings),
							new ItemInfo(ItemID.CelestialCuffs),
							new ItemInfo(ItemID.StarVeil),
							new ItemInfo(ItemID.GravityGlobe),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.CobaltOre, 10, 15),
							new ItemInfo(ItemID.PalladiumOre, 10, 15),
							new ItemInfo(ItemID.MythrilOre, 5, 10),
							new ItemInfo(ItemID.OrichalcumOre, 5, 10),
							new ItemInfo(ItemID.AdamantiteOre, 5, 10),
							new ItemInfo(ItemID.TitaniumOre, 5, 10),
							new ItemInfo(ItemID.LifeCrystal, 2, 3),
							new ItemInfo(ItemID.ManaCrystal, 3, 4),
							new ItemInfo(ItemID.Meteorite, 15, 25),
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.GravitationPotion, 1, 2),
							new ItemInfo(ItemID.CalmingPotion, 1, 2),
							new ItemInfo(ItemID.ManaRegenerationPotion, 1, 2),
							new ItemInfo(ItemID.MagicPowerPotion, 2, 3),
							new ItemInfo(ItemID.ObsidianSkinPotion, 1, 2),
							new ItemInfo(ItemID.SeafoodDinner, 2, 3),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.GoldCoin, 1, 2)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.CobaltPickaxe),
							new ItemInfo(ItemID.PalladiumPickaxe),
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.CursedTorch, 20, 30),
							new ItemInfo(ItemID.IchorTorch, 20, 30),
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.PirateShipMountItem),
							new ItemInfo(ItemID.WitchBroom)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.FallenStar, 3, 5)
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.HolyArrow, 10, 15),
							new ItemInfo(ItemID.JestersArrow, 10, 20),
							new ItemInfo(ItemID.MoonlordArrow, 5, 10),
							new ItemInfo(ItemID.MoonlordBullet, 10, 15)
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.GreaterManaPotion, 8, 10),
							new ItemInfo(ItemID.SuperManaPotion, 5, 8),
						};
						break;
					}
				case 6:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.FlameWings),
							new ItemInfo(ItemID.LavaWaders),
							new ItemInfo(ItemID.MoonLordLegs),
							new ItemInfo(ItemID.CobaltHat),
							new ItemInfo(ItemID.CobaltHelmet),
							new ItemInfo(ItemID.CobaltMask),
							new ItemInfo(ItemID.CobaltBreastplate),
							new ItemInfo(ItemID.CobaltLeggings),
							new ItemInfo(ItemID.PalladiumHelmet),
							new ItemInfo(ItemID.PalladiumMask),
							new ItemInfo(ItemID.PalladiumHeadgear),
							new ItemInfo(ItemID.PalladiumBreastplate),
							new ItemInfo(ItemID.PalladiumLeggings),
							new ItemInfo(ItemID.MythrilHat),
							new ItemInfo(ItemID.MythrilHelmet),
							new ItemInfo(ItemID.MythrilHood),
							new ItemInfo(ItemID.MythrilChainmail),
							new ItemInfo(ItemID.MythrilGreaves),
							new ItemInfo(ItemID.OrichalcumHeadgear),
							new ItemInfo(ItemID.OrichalcumHelmet),
							new ItemInfo(ItemID.OrichalcumMask),
							new ItemInfo(ItemID.OrichalcumBreastplate),
							new ItemInfo(ItemID.OrichalcumLeggings),
							new ItemInfo(ItemID.SpiderMask),
							new ItemInfo(ItemID.SpiderBreastplate),
							new ItemInfo(ItemID.SpiderGreaves),
							new ItemInfo(ItemID.ObsidianHelm),
							new ItemInfo(ItemID.ObsidianShirt),
							new ItemInfo(ItemID.ObsidianPants),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.FlameWings),
							new ItemInfo(ItemID.LavaWaders),
							new ItemInfo(ItemID.MoonLordLegs),
							new ItemInfo(ItemID.CobaltHat),
							new ItemInfo(ItemID.CobaltHelmet),
							new ItemInfo(ItemID.CobaltMask),
							new ItemInfo(ItemID.CobaltBreastplate),
							new ItemInfo(ItemID.CobaltLeggings),
							new ItemInfo(ItemID.PalladiumHelmet),
							new ItemInfo(ItemID.PalladiumMask),
							new ItemInfo(ItemID.PalladiumHeadgear),
							new ItemInfo(ItemID.PalladiumBreastplate),
							new ItemInfo(ItemID.PalladiumLeggings),
							new ItemInfo(ItemID.MythrilHat),
							new ItemInfo(ItemID.MythrilHelmet),
							new ItemInfo(ItemID.MythrilHood),
							new ItemInfo(ItemID.MythrilChainmail),
							new ItemInfo(ItemID.MythrilGreaves),
							new ItemInfo(ItemID.OrichalcumHeadgear),
							new ItemInfo(ItemID.OrichalcumHelmet),
							new ItemInfo(ItemID.OrichalcumMask),
							new ItemInfo(ItemID.OrichalcumBreastplate),
							new ItemInfo(ItemID.OrichalcumLeggings),
							new ItemInfo(ItemID.SpiderMask),
							new ItemInfo(ItemID.SpiderBreastplate),
							new ItemInfo(ItemID.SpiderGreaves),
							new ItemInfo(ItemID.ObsidianHelm),
							new ItemInfo(ItemID.ObsidianShirt),
							new ItemInfo(ItemID.ObsidianPants),
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.ManaCrystal, 3, 4),
							new ItemInfo(ItemID.LifeFruit, 2, 3),
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.FlaskofCursedFlames, 1, 2),
							new ItemInfo(ItemID.FlaskofFire, 1, 2),
							new ItemInfo(ItemID.FlaskofGold, 1, 2),
							new ItemInfo(ItemID.FlaskofIchor, 1, 2),
							new ItemInfo(ItemID.FlaskofNanites, 1, 2),
							new ItemInfo(ItemID.FlaskofParty, 1, 2),
							new ItemInfo(ItemID.FlaskofPoison, 1, 2),
							new ItemInfo(ItemID.FlaskofVenom, 1, 2),
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.GoldCoin, 3, 4)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.AdamantitePickaxe),
							new ItemInfo(ItemID.TitaniumPickaxe),
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.GolemPetItem),
							new ItemInfo(ItemID.PumpkingPetItem),
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.WallOfFleshGoatMountItem),
							new ItemInfo(ItemID.SpookyWoodMountItem)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.BoneKey),
							new ItemInfo(ItemID.BoneKey),
							new ItemInfo(ItemID.BoneKey),
							new ItemInfo(ItemID.BoneKey),
							new ItemInfo(ItemID.GoldenKey),
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.HellfireArrow, 30, 35),
							new ItemInfo(ItemID.ExplodingBullet, 30, 40),
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.SuperHealingPotion, 1, 2),
							new ItemInfo(ItemID.GreaterHealingPotion, 3, 4),
							new ItemInfo(ItemID.SuperManaPotion, 7, 9),
							new ItemInfo(ItemID.GreaterManaPotion, 8, 12),
						};
						break;
					}
				case 7:
					{
						Accessories = new ItemInfo[]
						{
							new ItemInfo(ItemID.LongRainbowTrailWings),
							new ItemInfo(ItemID.BundleofBalloons),
							new ItemInfo(ItemID.TerrasparkBoots),
							new ItemInfo(ItemID.MasterNinjaGear),
							new ItemInfo(ItemID.FireGauntlet),
							new ItemInfo(ItemID.CelestialShell),
							new ItemInfo(ItemID.ReconScope),
							new ItemInfo(ItemID.PapyrusScarab),
							new ItemInfo(ItemID.CelestialEmblem),
							new ItemInfo(ItemID.EmpressFlightBooster),
							new ItemInfo(ItemID.ShinyStone),
						};
						Weapons = new ItemInfo[]
						{
							new ItemInfo(ItemID.DeathSickle),
							new ItemInfo(ItemID.TerraBlade),
							new ItemInfo(ItemID.Kraken),
							new ItemInfo(ItemID.DaedalusStormbow),
							new ItemInfo(ItemID.DD2PhoenixBow),
							new ItemInfo(ItemID.Megashark),
							new ItemInfo(ItemID.VenusMagnum),
							new ItemInfo(ItemID.RainbowRod),
							new ItemInfo(ItemID.LeafBlower),
							new ItemInfo(ItemID.MagnetSphere),
							new ItemInfo(ItemID.ShadowbeamStaff),
							new ItemInfo(ItemID.RavenStaff),
							new ItemInfo(ItemID.StaffoftheFrostHydra),
							new ItemInfo(ItemID.StormTigerStaff),
							new ItemInfo(ItemID.SwordWhip),
							new ItemInfo(ItemID.ScytheWhip)
						};
						Ores = new ItemInfo[]
						{
							new ItemInfo(ItemID.HallowedHelmet),
							new ItemInfo(ItemID.HallowedHood),
							new ItemInfo(ItemID.HallowedHeadgear),
							new ItemInfo(ItemID.HallowedMask),
						};
						Potions = new ItemInfo[]
						{
							new ItemInfo(ItemID.HallowedPlateMail)
						};
						Coins = new ItemInfo[]
						{
							new ItemInfo(ItemID.HallowedGreaves)
						};
						Tools = new ItemInfo[]
						{
							new ItemInfo(ItemID.PickaxeAxe),
							new ItemInfo(ItemID.Drax),
						};
						Lighters = new ItemInfo[]
						{
							new ItemInfo(ItemID.SuspiciousLookingTentacle),
							new ItemInfo(ItemID.FairyQueenPetItem),
						};
						Mounts = new ItemInfo[]
						{
							new ItemInfo(ItemID.CosmicCarKey),
							new ItemInfo(ItemID.ShrimpyTruffle)
						};
						Sundries = new ItemInfo[]
						{
							new ItemInfo(ItemID.LongRainbowTrailWings),
							new ItemInfo(ItemID.BundleofBalloons),
							new ItemInfo(ItemID.TerrasparkBoots),
							new ItemInfo(ItemID.MasterNinjaGear),
							new ItemInfo(ItemID.FireGauntlet),
							new ItemInfo(ItemID.CelestialShell),
							new ItemInfo(ItemID.ReconScope),
							new ItemInfo(ItemID.PapyrusScarab),
							new ItemInfo(ItemID.CelestialEmblem),
							new ItemInfo(ItemID.EmpressFlightBooster),
							new ItemInfo(ItemID.ShinyStone),
						};
						Ammo = new ItemInfo[]
						{
							new ItemInfo(ItemID.DeathSickle),
							new ItemInfo(ItemID.TerraBlade),
							new ItemInfo(ItemID.Kraken),
							new ItemInfo(ItemID.DaedalusStormbow),
							new ItemInfo(ItemID.DD2PhoenixBow),
							new ItemInfo(ItemID.Megashark),
							new ItemInfo(ItemID.VenusMagnum),
							new ItemInfo(ItemID.RainbowRod),
							new ItemInfo(ItemID.LeafBlower),
							new ItemInfo(ItemID.MagnetSphere),
							new ItemInfo(ItemID.ShadowbeamStaff),
							new ItemInfo(ItemID.RavenStaff),
							new ItemInfo(ItemID.StaffoftheFrostHydra),
							new ItemInfo(ItemID.StormTigerStaff),
							new ItemInfo(ItemID.SwordWhip),
							new ItemInfo(ItemID.ScytheWhip)
						};
						Healings = new ItemInfo[]
						{
							new ItemInfo(ItemID.SuperHealingPotion, 3, 4),
							new ItemInfo(ItemID.SuperManaPotion, 12, 15)
						};
						break;
					}
				case 8:
					break;
				default:
					throw new Exception("无效等级");
			}
			if (level > 7)
			{
				level -= 7;
			}
			switch (level)
			{
				case 1:
					IdentifierL = 146;
					IdentifierR = 202;
					break;
				case 2:
					IdentifierL = 193;
					IdentifierR = 196;
					break;
				case 3:
					IdentifierL = 45;
					IdentifierR = 177;
					break;
				case 4:
					IdentifierL = 385;
					IdentifierR = 160;
					break;
				case 5:
					IdentifierL = 54;
					IdentifierR = 315;
					break;
				case 6:
					IdentifierL = 195;
					IdentifierR = 194;
					break;
				case 7:
					IdentifierL = 41;
					IdentifierR = 541;
					break;
			}
		}

		private List<ItemInfo[]> RandomItemTypes()
		{
			int level = Level;
			if (level > 7)//既然最高级的箱子现在是蓝箱，那同时出两件货的能力也就只属于蓝箱了
			{
				level -= 7;
			}
			var itemTypes = new List<ItemInfo[]>(11);
			if (level == 7)
			{
				itemTypes.Add(Accessories);
				itemTypes.Add(Weapons);
			}
			else
			{
				if (SurvivalCrisis.Rand.Next(2) == 1)
				{
					itemTypes.Add(Accessories);
				}
				else
				{
					itemTypes.Add(Weapons);
				}
			}
			int t = 1;
			if (SurvivalCrisis.Rand.NextDouble() < 0.05)
			{
				itemTypes.Add(Tools);
				t++;
			}
			if (SurvivalCrisis.Rand.NextDouble() < 0.05)
			{
				itemTypes.Add(Mounts);
				t++;
			}
			else if (level == 7)
			{
				if (SurvivalCrisis.Rand.NextDouble() < 0.5)
				{
					itemTypes.Add(Mounts);
					t++;
				}
			}
			var arr = new[] { Ammo, Potions, Ores, Coins, Lighters, Healings, Sundries };
			SurvivalCrisis.Rand.Shuffle(arr);
			int min = level == 7 ? 6 : 3;
			int max = level == 7 ? 7 : 5;
			int i = 0;
			while (t < min)
			{
				itemTypes.Add(arr[i++]);
				t++;
			}
			while(i < arr.Length && t < max)
			{
				if (SurvivalCrisis.Rand.NextDouble() < (level / 7.0 - (t - min) / 10.0))
				{
					itemTypes.Add(arr[i++]);
					t++;
				}
				else
				{
					break;
				}
			}
			return itemTypes;
		}

		public void AddChestItems(Chest chest)
		{
			var itemTypes = RandomItemTypes();
			foreach (var type in itemTypes)
			{
				var item = SurvivalCrisis.Rand.Next(type);
				chest.AddItemToShop(item.ToItem());
			}
		}

		public void PlaceChest(int x, int y)
		{
			int level = Level;
			if (level > 7)
			{
				level -= 7;
			}
			ushort chestType = level switch
			{
				1 => 43,
				2 => 35,
				3 => 52,
				4 => 51,
				5 => 48,
				6 => 44,
				7 => 41,
				_ => 0
			};
			chestType -= 1;
			Main.tile[x + 0, y - 1].liquid = 0;
			Main.tile[x + 1, y - 1].liquid = 0;
			Main.tile[x + 0, y - 0].liquid = 0;
			Main.tile[x + 1, y - 0].liquid = 0;
			Main.tile[x + 0, y - 1].active(false);
			Main.tile[x + 1, y - 1].active(false);
			Main.tile[x + 0, y - 0].active(false);
			Main.tile[x + 1, y - 0].active(false);
			int idx = WorldGen.PlaceChest(x, y, 21, style: chestType);
			try
			{
				AddChestItems(Main.chest[idx]);
			}
			catch (Exception e)
			{
				//TSPlayer.All.SendErrorMessage(e.ToString());
			}
		}

		public void Generate(in TileSection block)
		{
			for (int x = 0; x < block.Width - 1; x++)
			{
				for (int y = 1; y < block.Height; y++)
				{
					if (block[x, y].type == IdentifierL && block[x + 1, y].type == IdentifierR)
					{
						PlaceChest(x + block.X, y + block.Y);
					}
				}
			}
		}
	}
}
