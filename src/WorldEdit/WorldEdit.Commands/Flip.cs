using System.IO;
using TShockAPI;

namespace WorldEdit.Commands;

public class Flip : WECommand
{
	private readonly bool flipX;

	private readonly bool flipY;

	public Flip(TSPlayer plr, bool flipX, bool flipY)
		: base(0, 0, 0, 0, plr)
	{
		this.flipX = flipX;
		this.flipY = flipY;
	}

	public override void Execute()
	{
		string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		WorldSectionData worldSectionData = Tools.LoadWorldData(clipboardPath);
		int num = (flipX ? (-1) : worldSectionData.Width);
		int num2 = (flipY ? (-1) : worldSectionData.Height);
		int num3 = ((!flipX) ? 1 : (-1));
		int num4 = ((!flipY) ? 1 : (-1));
		using (BinaryWriter writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height))
		{
			for (int i = (flipX ? (worldSectionData.Width - 1) : 0); i != num; i += num3)
			{
				for (int j = (flipY ? (worldSectionData.Height - 1) : 0); j != num2; j += num4)
				{
					switch (worldSectionData.Tiles[i, j].slope())
					{
					case 1:
						if (flipX && flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)4);
						}
						else if (flipX)
						{
							worldSectionData.Tiles[i, j].slope((byte)2);
						}
						else if (flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)3);
						}
						break;
					case 2:
						if (flipX && flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)3);
						}
						else if (flipX)
						{
							worldSectionData.Tiles[i, j].slope((byte)1);
						}
						else if (flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)4);
						}
						break;
					case 3:
						if (flipX && flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)2);
						}
						else if (flipX)
						{
							worldSectionData.Tiles[i, j].slope((byte)4);
						}
						else if (flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)1);
						}
						break;
					case 4:
						if (flipX && flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)1);
						}
						else if (flipX)
						{
							worldSectionData.Tiles[i, j].slope((byte)3);
						}
						else if (flipY)
						{
							worldSectionData.Tiles[i, j].slope((byte)2);
						}
						break;
					}
					writer.Write(worldSectionData.Tiles[i, j]);
				}
			}
		}
		plr.SendSuccessMessage("Flipped clipboard.");
	}
}
