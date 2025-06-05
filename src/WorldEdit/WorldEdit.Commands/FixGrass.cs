using System.Linq;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class FixGrass : WECommand
{
	private static ushort[] grassTiles = new ushort[8] { 23, 199, 2, 109, 60, 70, 477, 492 };

	public FixGrass(int x, int y, int x2, int y2, TSPlayer plr)
		: base(x, y, x2, y2, plr)
	{
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int num = 0;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				ushort type = Main.tile[i, j].type;
				if (grassTiles.Contains(type) && TileSolid(i - 1, j - 1) && TileSolid(i - 1, j) && TileSolid(i - 1, j + 1) && TileSolid(i, j - 1) && TileSolid(i, j + 1) && TileSolid(i + 1, j) && TileSolid(i + 1, j) && TileSolid(i + 1, j + 1))
				{
					Main.tile[i, j].type = (ushort)((type == 60 || type == 70) ? 59u : 0u);
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Fixed grass. ({0})", num);
	}
}
