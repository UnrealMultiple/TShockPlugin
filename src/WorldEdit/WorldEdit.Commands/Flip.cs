using System.IO;
using TShockAPI;

namespace WorldEdit.Commands;

public class Flip : WECommand
{
	private readonly bool _flipX;

	private readonly bool _flipY;

	public Flip(TSPlayer plr, bool flipX, bool flipY)
		: base(0, 0, 0, 0, plr)
	{
		_flipX = flipX;
		_flipY = flipY;
	}

	public override void Execute()
	{
		string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		WorldSectionData worldSectionData = Tools.LoadWorldData(clipboardPath);
		int endX = _flipX ? -1 : worldSectionData.Width;
		int endY = _flipY ? -1 : worldSectionData.Height;
		int stepX = _flipX ? -1 : 1;
		int stepY = _flipY ? -1 : 1;
		using (BinaryWriter writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width, worldSectionData.Height))
		{
			for (int tileX = _flipX ? worldSectionData.Width - 1 : 0; tileX != endX; tileX += stepX)
			{
				for (int tileY = _flipY ? worldSectionData.Height - 1 : 0; tileY != endY; tileY += stepY)
				{
					switch (worldSectionData.Tiles[tileX, tileY].slope())
					{
					case 1:
						if (_flipX && _flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						}
						else if (_flipX)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						}
						else if (_flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						}
						break;
					case 2:
						if (_flipX && _flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						}
						else if (_flipX)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						}
						else if (_flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						}
						break;
					case 3:
						if (_flipX && _flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						}
						else if (_flipX)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)4);
						}
						else if (_flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						}
						break;
					case 4:
						if (_flipX && _flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)1);
						}
						else if (_flipX)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)3);
						}
						else if (_flipY)
						{
							worldSectionData.Tiles[tileX, tileY].slope((byte)2);
						}
						break;
					}
					writer.Write(worldSectionData.Tiles[tileX, tileY]);
				}
			}
		}
		plr.SendSuccessMessage("Flipped clipboard.");
	}
}
