using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Biome : WECommand
{
	private readonly string sourceBiome;

	private readonly string targetBiome;

	public Biome(int x, int y, int x2, int y2, TSPlayer plr, string sourceBiome, string targetBiome)
		: base(x, y, x2, y2, plr)
	{
		this.sourceBiome = sourceBiome;
		this.targetBiome = targetBiome;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		int convertedTileCount = 0;
		if (sourceBiome != targetBiome)
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if (select(i, j, plr) && WorldEdit.Biomes[sourceBiome].ConvertTile(Main.tile[i, j], WorldEdit.Biomes[targetBiome]))
					{
						convertedTileCount++;
					}
				}
			}
			ResetSection();
		}
		plr.SendSuccessMessage("Converted biomes. ({0})", convertedTileCount);
	}
}
