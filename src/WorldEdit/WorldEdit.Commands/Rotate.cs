using System.IO;
using TShockAPI;

namespace WorldEdit.Commands;

public class Rotate : WECommand
{
	private readonly int _degrees;

	public Rotate(TSPlayer plr, int degrees)
		: base(0, 0, 0, 0, plr)
	{
		_degrees = degrees;
	}

	public override void Execute()
	{
		string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		WorldSectionData worldSectionData = Tools.LoadWorldData(clipboardPath);
		BinaryWriter writer = null;
		switch ((_degrees / 90 % 4 + 4) % 4)
		{
		case 0:
		{
			writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height);
			for (int tileX = 0; tileX < worldSectionData.Width; tileX++)
			{
				for (int tileY = 0; tileY < worldSectionData.Height; tileY++)
				{
					writer.Write(worldSectionData.Tiles[tileX, tileY]);
				}
			}
			break;
		}
		case 1:
		{
			writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Height, worldSectionData.Width);
			for (int tileY = worldSectionData.Height - 1; tileY >= 0; tileY--)
			{
				for (int tileX = 0; tileX < worldSectionData.Width; tileX++)
				{
					switch (worldSectionData.Tiles[tileX, tileY].slope())
					{
					case 1:
						worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						break;
					case 2:
						worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						break;
					case 3:
						worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						break;
					case 4:
						worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						break;
					}
					writer.Write(worldSectionData.Tiles[tileX, tileY]);
				}
			}
			break;
		}
		case 2:
		{
			writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height);
			for (int tileX = worldSectionData.Width - 1; tileX >= 0; tileX--)
			{
				for (int tileY = worldSectionData.Height - 1; tileY >= 0; tileY--)
				{
					switch (worldSectionData.Tiles[tileX, tileY].slope())
					{
					case 1:
						worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						break;
					case 2:
						worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						break;
					case 3:
						worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						break;
					case 4:
						worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						break;
					}
					writer.Write(worldSectionData.Tiles[tileX, tileY]);
				}
			}
			break;
		}
		case 3:
		{
			writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Height, worldSectionData.Width);
			for (int tileY = 0; tileY < worldSectionData.Height; tileY++)
			{
				for (int tileX = worldSectionData.Width - 1; tileX >= 0; tileX--)
				{
					switch (worldSectionData.Tiles[tileX, tileY].slope())
					{
					case 1:
						worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						break;
					case 2:
						worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						break;
					case 3:
						worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						break;
					case 4:
						worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						break;
					}
					writer.Write(worldSectionData.Tiles[tileX, tileY]);
				}
			}
			break;
		}
		}
		writer?.Close();
		plr.SendSuccessMessage("Rotated clipboard {0} degrees.", _degrees);
	}
}
