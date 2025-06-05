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
		BinaryWriter binaryWriter = null;
		switch ((_degrees / 90 % 4 + 4) % 4)
		{
		case 0:
		{
			binaryWriter = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height);
			for (int k = 0; k < worldSectionData.Width; k++)
			{
				for (int l = 0; l < worldSectionData.Height; l++)
				{
					binaryWriter.Write(worldSectionData.Tiles[k, l]);
				}
			}
			break;
		}
		case 1:
		{
			binaryWriter = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Height, worldSectionData.Width);
			for (int num2 = worldSectionData.Height - 1; num2 >= 0; num2--)
			{
				for (int j = 0; j < worldSectionData.Width; j++)
				{
					switch (worldSectionData.Tiles[j, num2].slope())
					{
					case 1:
						worldSectionData.Tiles[j, num2].slope((byte)3);
						break;
					case 2:
						worldSectionData.Tiles[j, num2].slope((byte)1);
						break;
					case 3:
						worldSectionData.Tiles[j, num2].slope((byte)4);
						break;
					case 4:
						worldSectionData.Tiles[j, num2].slope((byte)2);
						break;
					}
					binaryWriter.Write(worldSectionData.Tiles[j, num2]);
				}
			}
			break;
		}
		case 2:
		{
			binaryWriter = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height);
			for (int num3 = worldSectionData.Width - 1; num3 >= 0; num3--)
			{
				for (int num4 = worldSectionData.Height - 1; num4 >= 0; num4--)
				{
					switch (worldSectionData.Tiles[num3, num4].slope())
					{
					case 1:
						worldSectionData.Tiles[num3, num4].slope((byte)4);
						break;
					case 2:
						worldSectionData.Tiles[num3, num4].slope((byte)3);
						break;
					case 3:
						worldSectionData.Tiles[num3, num4].slope((byte)2);
						break;
					case 4:
						worldSectionData.Tiles[num3, num4].slope((byte)1);
						break;
					}
					binaryWriter.Write(worldSectionData.Tiles[num3, num4]);
				}
			}
			break;
		}
		case 3:
		{
			binaryWriter = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Height, worldSectionData.Width);
			for (int i = 0; i < worldSectionData.Height; i++)
			{
				for (int num = worldSectionData.Width - 1; num >= 0; num--)
				{
					switch (worldSectionData.Tiles[num, i].slope())
					{
					case 1:
						worldSectionData.Tiles[num, i].slope((byte)2);
						break;
					case 2:
						worldSectionData.Tiles[num, i].slope((byte)4);
						break;
					case 3:
						worldSectionData.Tiles[num, i].slope((byte)1);
						break;
					case 4:
						worldSectionData.Tiles[num, i].slope((byte)3);
						break;
					}
					binaryWriter.Write(worldSectionData.Tiles[num, i]);
				}
			}
			break;
		}
		}
		binaryWriter?.Close();
		plr.SendSuccessMessage("Rotated clipboard {0} degrees.", _degrees);
	}
}
