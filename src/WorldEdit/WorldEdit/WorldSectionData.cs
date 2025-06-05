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
		int num = x + X;
		int num2 = y + Y;
		switch (tile.type)
		{
		case 55:
		case 85:
		case 425:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int num12 = Sign.ReadSign(num, num2, false);
				if (num12 != -1)
				{
					Signs.Add(new SignData
					{
						Text = Main.sign[num12].text,
						X = x,
						Y = y
					});
				}
			}
			break;
		case 395:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int num9 = TEItemFrame.Find(num, num2);
				if (num9 != -1)
				{
					TEItemFrame val5 = (TEItemFrame)TileEntity.ByID[num9];
					ItemFrames.Add(new DisplayItemData
					{
						Item = new NetItem(val5.item.netID, val5.item.stack, val5.item.prefix),
						X = x,
						Y = y
					});
				}
			}
			break;
		case 21:
		case 88:
		{
			if (tile.frameX % 36 != 0 || tile.frameY != 0)
			{
				break;
			}
			int num11 = Chest.FindChest(num, num2);
			if (num11 == -1)
			{
				break;
			}
			Chest val7 = Main.chest[num11];
			if (val7.item != null)
			{
				NetItem[] items = val7.item.Select((Item item) => new NetItem(item.netID, item.stack, item.prefix)).ToArray();
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
			int num7 = TELogicSensor.Find(num, num2);
			if (num7 != -1)
			{
				TELogicSensor val4 = (TELogicSensor)TileEntity.ByID[num7];
				LogicSensors.Add(new LogicSensorData
				{
					Type = val4.logicCheck,
					X = x,
					Y = y
				});
			}
			break;
		}
		case 378:
			if (tile.frameX % 36 == 0 && tile.frameY == 0)
			{
				int num8 = TETrainingDummy.Find(num, num2);
				if (num8 != -1)
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
				int num10 = TEWeaponsRack.Find(num, num2);
				if (num10 != -1)
				{
					TEWeaponsRack val6 = (TEWeaponsRack)TileEntity.ByID[num10];
					WeaponsRacks.Add(new DisplayItemData
					{
						Item = new NetItem(val6.item.netID, val6.item.stack, val6.item.prefix),
						X = x,
						Y = y
					});
				}
			}
			break;
		case 597:
			if (tile.frameY == 0 && tile.frameX % 54 == 0)
			{
				int num6 = TETeleportationPylon.Find(num, num2);
				if (num6 != -1)
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
			int num4 = TEDisplayDoll.Find(num, num2);
			if (num4 != -1)
			{
				TEDisplayDoll val2 = (TEDisplayDoll)TileEntity.ByID[num4];
				DisplayDolls.Add(new DisplayItemsData
				{
					Items = val2._items.Select((Item i) => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
					Dyes = val2._dyes.Select((Item i) => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
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
			int num5 = TEHatRack.Find(num, num2);
			if (num5 != -1)
			{
				TEHatRack val3 = (TEHatRack)TileEntity.ByID[num5];
				HatRacks.Add(new DisplayItemsData
				{
					Items = val3._items.Select((Item i) => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
					Dyes = val3._dyes.Select((Item i) => new NetItem(i.netID, i.stack, i.prefix)).ToArray(),
					X = x,
					Y = y
				});
			}
			break;
		}
		case 520:
		{
			int num3 = TEFoodPlatter.Find(num, num2);
			if (num3 != -1)
			{
				TEFoodPlatter val = (TEFoodPlatter)TileEntity.ByID[num3];
				FoodPlatters.Add(new DisplayItemData
				{
					Item = new NetItem(val.item.netID, val.item.stack, val.item.prefix),
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
	}

	public void Write(Stream stream)
	{
		using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
		{
			WriteHeader(writer);
		}
		using BinaryWriter writer2 = new BinaryWriter(new BufferedStream(new GZipStream(stream, CompressionMode.Compress), 1048576));
		WriteInner(writer2);
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
