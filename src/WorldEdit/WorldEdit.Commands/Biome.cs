using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Biome : WECommand
{
	private string biome1;

	private string biome2;

	public Biome(int x, int y, int x2, int y2, TSPlayer plr, string biome1, string biome2)
		: base(x, y, x2, y2, plr)
	{
		this.biome1 = biome1;
		this.biome2 = biome2;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		int num = 0;
		if (biome1 != biome2)
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					if (select(i, j, plr) && WorldEdit.Biomes[biome1].ConvertTile(Main.tile[i, j], WorldEdit.Biomes[biome2]))
					{
						num++;
					}
				}
			}
			ResetSection();
		}
		plr.SendSuccessMessage("Converted biomes. ({0})", num);
	}
}
