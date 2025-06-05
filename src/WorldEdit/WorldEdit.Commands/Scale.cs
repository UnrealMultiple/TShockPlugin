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
			List<ITile> list = new List<ITile>();
			for (int i = 0; i < worldSectionData.Width; i++)
			{
				for (int j = 0; j < worldSectionData.Height; j++)
				{
					for (int k = 0; k < _scale; k++)
					{
						writer.Write(worldSectionData.Tiles[i, j]);
					}
					list.Add(worldSectionData.Tiles[i, j]);
					if (j != worldSectionData.Height - 1)
					{
						continue;
					}
					for (int l = 0; l < _scale - 1; l++)
					{
						foreach (ITile item2 in list)
						{
							for (int m = 0; m < _scale; m++)
							{
								writer.Write(item2);
							}
						}
					}
					list.Clear();
				}
			}
		}
		else
		{
			int num = worldSectionData.Width % _scale;
			int num2 = worldSectionData.Height % _scale;
			int num3 = worldSectionData.Width / _scale;
			int num4 = worldSectionData.Height / _scale;
			int num5 = ((num == 0) ? num3 : (num3 + 1));
			int num6 = ((num2 == 0) ? num4 : (num4 + 1));
			ITile[,] array = new ITile[num5, num6];
			for (int n = 0; n < num5; n++)
			{
				for (int num7 = 0; num7 < num6; num7++)
				{
					List<ITile> list2 = new List<ITile>();
					for (int num8 = 0; num8 < _scale; num8++)
					{
						for (int num9 = 0; num9 < _scale; num9++)
						{
							ITile item;
							if (n * _scale + num8 >= worldSectionData.Width || num7 * _scale + num9 >= worldSectionData.Height)
							{
								ITile val = (ITile)new Tile();
								item = val;
							}
							else
							{
								item = worldSectionData.Tiles[n * _scale + num8, num7 * _scale + num9];
							}
							list2.Add(item);
						}
					}
					array[n, num7] = (from g in list2
						group g by g.type into g
						orderby g.Count() descending
						select g).SelectMany((IGrouping<ushort, ITile> g) => g).First();
				}
			}
			using BinaryWriter writer2 = WorldSectionData.WriteHeader(clipboardPath, 0, 0, num5, num6);
			for (int num10 = 0; num10 < num5; num10++)
			{
				for (int num11 = 0; num11 < num6; num11++)
				{
					writer2.Write(array[num10, num11]);
				}
			}
		}
		plr.SendSuccessMessage("Clipboard {0}creased by {1} times.", _addition ? "in" : "de", _scale);
	}
}
