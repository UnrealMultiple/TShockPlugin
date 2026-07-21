using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using TShockAPI;

namespace WorldEdit;

public sealed class WorldSectionData
{
	public struct DisplayItemData
	{
		public int X;

		public int Y;

		public NetItem Item;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Item);
		}

		public static DisplayItemData Read(BinaryReader reader)
		{
			DisplayItemData result = default(DisplayItemData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			result.Item = reader.ReadNetItem();
			return result;
		}
	}

	public struct DisplayItemsData
	{
		public int X;

		public int Y;

		public NetItem[] Items;

		public NetItem[] Dyes;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Items);
			writer.Write(Dyes);
		}

		public static DisplayItemsData Read(BinaryReader reader)
		{
			DisplayItemsData result = default(DisplayItemsData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			result.Items = reader.ReadNetItems();
			result.Dyes = reader.ReadNetItems();
			return result;
		}
	}

	public struct DisplayDollExtraData
	{
		public int X;

		public int Y;

		public NetItem[] Misc;

		public byte Pose;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Misc);
			writer.Write(Pose);
		}

		public static DisplayDollExtraData Read(BinaryReader reader)
		{
			return new DisplayDollExtraData
			{
				X = reader.ReadInt32(),
				Y = reader.ReadInt32(),
				Misc = reader.ReadNetItems(),
				Pose = reader.ReadByte()
			};
		}
	}

	public struct PositionData
	{
		public int X;

		public int Y;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
		}

		public static PositionData Read(BinaryReader reader)
		{
			PositionData result = default(PositionData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			return result;
		}
	}

	public struct LogicSensorData
	{
		public int X;

		public int Y;

        public TELogicSensor.LogicCheckType Type;

        public void Write(BinaryWriter writer)
		{
									writer.Write(X);
			writer.Write(Y);
			writer.Write((int)Type);
		}

		public static LogicSensorData Read(BinaryReader reader)
		{
						LogicSensorData result = default(LogicSensorData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			result.Type = (TELogicSensor.LogicCheckType)reader.ReadInt32();
			return result;
		}
	}

	public struct ChestData
	{
		public int X;

		public int Y;

		public NetItem[] Items;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Items);
		}

		public static ChestData Read(BinaryReader reader)
		{
			ChestData result = default(ChestData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			result.Items = reader.ReadNetItems();
			return result;
		}
	}

	public struct SignData
	{
		public int X;

		public int Y;

		public string Text;

		public void Write(BinaryWriter writer)
		{
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Text);
		}

		public static SignData Read(BinaryReader reader)
		{
			SignData result = default(SignData);
			result.X = reader.ReadInt32();
			result.Y = reader.ReadInt32();
			result.Text = reader.ReadString();
			return result;
		}
	}

	public IList<SignData> Signs;

	public IList<ChestData> Chests;

	public IList<DisplayItemData> ItemFrames;

	public IList<LogicSensorData> LogicSensors;

	public IList<PositionData> TrainingDummies;

	public IList<DisplayItemData> WeaponsRacks;

	public IList<PositionData> TeleportationPylons;

	public IList<DisplayItemsData> DisplayDolls;

	public IList<DisplayItemsData> HatRacks;

	public IList<DisplayItemData> FoodPlatters;

	public IList<DisplayItemData> DisplayJars;

	public IList<DisplayDollExtraData> DisplayDollExtras;

	public ITile[,] Tiles;

	public int Width;

	public int Height;

	public int X;

	public int Y;

	public WorldSectionData(int width, int height)
	{
		Width = width;
		Height = height;
		Signs = new List<SignData>();
		Chests = new List<ChestData>();
		ItemFrames = new List<DisplayItemData>();
		LogicSensors = new List<LogicSensorData>();
		TrainingDummies = new List<PositionData>();
		WeaponsRacks = new List<DisplayItemData>();
		TeleportationPylons = new List<PositionData>();
		DisplayDolls = new List<DisplayItemsData>();
		HatRacks = new List<DisplayItemsData>();
		FoodPlatters = new List<DisplayItemData>();
		DisplayJars = new List<DisplayItemData>();
		DisplayDollExtras = new List<DisplayDollExtraData>();
		Tiles = new ITile[width, height];
	}

	public void ProcessTile(ITile tile, int x, int y)
	{
																																		Tiles[x, y] = (ITile)new Tile();
		if (tile != null)
		{
			Tiles[x, y].CopyFrom(tile);
		}
		if (!tile.active())
		{
			return;
		}
		int worldX = x + X;
		int worldY = y + Y;
		switch (tile.type)
		{
		case 55:
		case 85:
		case 425:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int signIndex = Sign.ReadSign(worldX, worldY, false);
				if (signIndex != -1)
				{
					Signs.Add(new SignData
					{
						Text = Main.sign[signIndex].text,
						X = x,
						Y = y
					});
				}
			}
			break;
		case 395:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int itemFrameEntityId = TEItemFrame.Find(worldX, worldY);
				if (itemFrameEntityId != -1)
				{
					TEItemFrame itemFrame = (TEItemFrame)TileEntity.ByID[itemFrameEntityId];
					ItemFrames.Add(new DisplayItemData
					{
						Item = new NetItem(itemFrame.item),
						X = x,
						Y = y
					});
				}
			}
			break;
		case 21:
		case 88:
		case 467:
		{
			if (tile.frameX % 36 != 0 || tile.frameY % 36 != 0)
			{
				break;
			}
			int chestIndex = Chest.FindChest(worldX, worldY);
			if (chestIndex == -1)
			{
				break;
			}
			Chest chest = Main.chest[chestIndex];
			if (chest.item != null)
			{
				NetItem[] items = chest.item.Select(item => new NetItem(item)).ToArray();
				Chests.Add(new ChestData
				{
					Items = items,
					X = x,
					Y = y
				});
			}
			break;
		}
		case 423:
		{
			int logicSensorEntityId = TELogicSensor.Find(worldX, worldY);
			if (logicSensorEntityId != -1)
			{
				TELogicSensor logicSensor = (TELogicSensor)TileEntity.ByID[logicSensorEntityId];
				LogicSensors.Add(new LogicSensorData
				{
					Type = logicSensor.logicCheck,
					X = x,
					Y = y
				});
			}
			break;
		}
		case 378:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int trainingDummyEntityId = TETrainingDummy.Find(worldX, worldY);
				if (trainingDummyEntityId != -1)
				{
					TrainingDummies.Add(new PositionData
					{
						X = x,
						Y = y
					});
				}
			}
			break;
		case 471:
			if (tile.frameY == 0 && tile.frameX % 54 == 0)
			{
				int weaponRackEntityId = TEWeaponsRack.Find(worldX, worldY);
				if (weaponRackEntityId != -1)
				{
					TEWeaponsRack weaponRack = (TEWeaponsRack)TileEntity.ByID[weaponRackEntityId];
					WeaponsRacks.Add(new DisplayItemData
					{
						Item = new NetItem(weaponRack.item),
						X = x,
						Y = y
					});
				}
			}
			break;
		case 597:
			if (tile.frameY == 0 && tile.frameX % 54 == 0)
			{
				int pylonEntityId = TETeleportationPylon.Find(worldX, worldY);
				if (pylonEntityId != -1)
				{
					TeleportationPylons.Add(new PositionData
					{
						X = x,
						Y = y
					});
				}
			}
			break;
		case 470:
		{
			if (tile.frameY != 0 || tile.frameX % 36 != 0)
			{
				break;
			}
			int displayDollEntityId = TEDisplayDoll.Find(worldX, worldY);
			if (displayDollEntityId != -1)
			{
				TEDisplayDoll displayDoll = (TEDisplayDoll)TileEntity.ByID[displayDollEntityId];
				DisplayDolls.Add(new DisplayItemsData
				{
					Items = displayDoll._equip.Select(item => new NetItem(item)).ToArray(),
					Dyes = displayDoll._dyes.Select(item => new NetItem(item)).ToArray(),
					X = x,
					Y = y
				});
				DisplayDollExtras.Add(new DisplayDollExtraData
				{
					Misc = displayDoll._misc.Select(item => new NetItem(item)).ToArray(),
					Pose = displayDoll._pose,
					X = x,
					Y = y
				});
			}
			break;
		}
		case 475:
		{
			if (tile.frameY != 0 || tile.frameX % 54 != 0)
			{
				break;
			}
			int hatRackEntityId = TEHatRack.Find(worldX, worldY);
			if (hatRackEntityId != -1)
			{
				TEHatRack hatRack = (TEHatRack)TileEntity.ByID[hatRackEntityId];
				HatRacks.Add(new DisplayItemsData
				{
					Items = hatRack._items.Select(item => new NetItem(item)).ToArray(),
					Dyes = hatRack._dyes.Select(item => new NetItem(item)).ToArray(),
					X = x,
					Y = y
				});
			}
			break;
		}
		case 520:
		{
			int foodPlatterEntityId = TEFoodPlatter.Find(worldX, worldY);
			if (foodPlatterEntityId != -1)
			{
				TEFoodPlatter foodPlatter = (TEFoodPlatter)TileEntity.ByID[foodPlatterEntityId];
				FoodPlatters.Add(new DisplayItemData
				{
					Item = new NetItem(foodPlatter.item),
					X = x,
					Y = y
				});
			}
			break;
		}
		case 698:
		{
			if (TileEntity.ByPosition.TryGetValue(new Point16(worldX, worldY), out TileEntity entity) &&
				entity is TEDeadCellsDisplayJar displayJar)
			{
				DisplayJars.Add(new DisplayItemData
				{
					Item = new NetItem(displayJar.item),
					X = x,
					Y = y
				});
			}
			break;
		}
		}
	}

	private void WriteInner(BinaryWriter writer)
	{
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				writer.Write(Tiles[i, j]);
			}
		}
		writer.Write(Signs.Count);
		foreach (SignData sign in Signs)
		{
			sign.Write(writer);
		}
		writer.Write(Chests.Count);
		foreach (ChestData chest in Chests)
		{
			chest.Write(writer);
		}
		writer.Write(ItemFrames.Count);
		foreach (DisplayItemData itemFrame in ItemFrames)
		{
			itemFrame.Write(writer);
		}
		writer.Write(LogicSensors.Count);
		foreach (LogicSensorData logicSensor in LogicSensors)
		{
			logicSensor.Write(writer);
		}
		writer.Write(TrainingDummies.Count);
		foreach (PositionData trainingDummy in TrainingDummies)
		{
			trainingDummy.Write(writer);
		}
		writer.Write(WeaponsRacks.Count);
		foreach (DisplayItemData weaponsRack in WeaponsRacks)
		{
			weaponsRack.Write(writer);
		}
		writer.Write(TeleportationPylons.Count);
		foreach (PositionData teleportationPylon in TeleportationPylons)
		{
			teleportationPylon.Write(writer);
		}
		writer.Write(DisplayDolls.Count);
		foreach (DisplayItemsData displayDoll in DisplayDolls)
		{
			displayDoll.Write(writer);
		}
		writer.Write(HatRacks.Count);
		foreach (DisplayItemsData hatRack in HatRacks)
		{
			hatRack.Write(writer);
		}
		writer.Write(FoodPlatters.Count);
		foreach (DisplayItemData foodPlatter in FoodPlatters)
		{
			foodPlatter.Write(writer);
		}
		writer.Write(DisplayJars.Count);
		foreach (DisplayItemData displayJar in DisplayJars)
		{
			displayJar.Write(writer);
		}
		writer.Write(DisplayDollExtras.Count);
		foreach (DisplayDollExtraData displayDollExtra in DisplayDollExtras)
		{
			displayDollExtra.Write(writer);
		}
	}

	public void Write(Stream stream)
	{
		using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
		{
			WriteHeader(writer);
		}
		using BinaryWriter compressedWriter = new BinaryWriter(new BufferedStream(new GZipStream(stream, CompressionMode.Compress), 1048576));
		WriteInner(compressedWriter);
	}

	public void Write(string filePath)
	{
		using BinaryWriter writer = WriteHeader(filePath, X, Y, Width, Height);
		WriteInner(writer);
	}

	public void WriteHeader(BinaryWriter writer)
	{
		writer.Write(X);
		writer.Write(Y);
		writer.Write(Width);
		writer.Write(Height);
	}

	public static BinaryWriter WriteHeader(string filePath, int x, int y, int width, int height)
	{
		Stream stream = File.Open(filePath, FileMode.Create);
		using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
		{
			binaryWriter.Write(x);
			binaryWriter.Write(y);
			binaryWriter.Write(width);
			binaryWriter.Write(height);
		}
		return new BinaryWriter(new BufferedStream(new GZipStream(stream, CompressionMode.Compress), 1048576));
	}
}
