using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Scale : WECommand
{
	private readonly bool _addition;

	private readonly int _scale;

	public Scale(TSPlayer plr, bool addition, int scale)
		: base(0, 0, 0, 0, plr)
	{
		_addition = addition;
		_scale = scale;
	}

	public override void Execute()
	{
						string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		WorldSectionData worldSectionData = Tools.LoadWorldData(clipboardPath);
		if (_addition)
		{
			using BinaryWriter writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, worldSectionData.Width * _scale, worldSectionData.Height * _scale);
			List<ITile> sourceRow = new List<ITile>();
			for (int tileX = 0; tileX < worldSectionData.Width; tileX++)
			{
				for (int tileY = 0; tileY < worldSectionData.Height; tileY++)
				{
					for (int horizontalCopy = 0; horizontalCopy < _scale; horizontalCopy++)
					{
						writer.Write(worldSectionData.Tiles[tileX, tileY]);
					}
					sourceRow.Add(worldSectionData.Tiles[tileX, tileY]);
					if (tileY != worldSectionData.Height - 1)
					{
						continue;
					}
					for (int verticalCopy = 0; verticalCopy < _scale - 1; verticalCopy++)
					{
						foreach (ITile tile in sourceRow)
						{
							for (int horizontalCopy = 0; horizontalCopy < _scale; horizontalCopy++)
							{
								writer.Write(tile);
							}
						}
					}
					sourceRow.Clear();
				}
			}
		}
		else
		{
			int widthRemainder = worldSectionData.Width % _scale;
			int heightRemainder = worldSectionData.Height % _scale;
			int wholeWidthGroups = worldSectionData.Width / _scale;
			int wholeHeightGroups = worldSectionData.Height / _scale;
			int targetWidth = widthRemainder == 0 ? wholeWidthGroups : wholeWidthGroups + 1;
			int targetHeight = heightRemainder == 0 ? wholeHeightGroups : wholeHeightGroups + 1;
			ITile[,] scaledTiles = new ITile[targetWidth, targetHeight];
			for (int targetX = 0; targetX < targetWidth; targetX++)
			{
				for (int targetY = 0; targetY < targetHeight; targetY++)
				{
					List<ITile> sourceTiles = new List<ITile>();
					for (int sourceXOffset = 0; sourceXOffset < _scale; sourceXOffset++)
					{
						for (int sourceYOffset = 0; sourceYOffset < _scale; sourceYOffset++)
						{
							ITile sourceTile;
							if (targetX * _scale + sourceXOffset >= worldSectionData.Width || targetY * _scale + sourceYOffset >= worldSectionData.Height)
							{
								sourceTile = new Tile();
							}
							else
							{
								sourceTile = worldSectionData.Tiles[targetX * _scale + sourceXOffset, targetY * _scale + sourceYOffset];
							}
							sourceTiles.Add(sourceTile);
						}
					}
					scaledTiles[targetX, targetY] = sourceTiles
						.GroupBy(tile => tile.type)
						.OrderByDescending(group => group.Count())
						.SelectMany(group => group)
						.First();
				}
			}
			using BinaryWriter writer = WorldSectionData.WriteHeader(clipboardPath, 0, 0, targetWidth, targetHeight);
			for (int tileX = 0; tileX < targetWidth; tileX++)
			{
				for (int tileY = 0; tileY < targetHeight; tileY++)
				{
					writer.Write(scaledTiles[tileX, tileY]);
				}
			}
		}
		plr.SendSuccessMessage("Clipboard {0}creased by {1} times.", _addition ? "in" : "de", _scale);
	}
}
