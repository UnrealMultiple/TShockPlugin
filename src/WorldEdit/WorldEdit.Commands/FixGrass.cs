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
		int fixedGrassCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ushort tileType = Main.tile[tileX, tileY].type;
				if (grassTiles.Contains(tileType) && TileSolid(tileX - 1, tileY - 1) && TileSolid(tileX - 1, tileY) && TileSolid(tileX - 1, tileY + 1) && TileSolid(tileX, tileY - 1) && TileSolid(tileX, tileY + 1) && TileSolid(tileX + 1, tileY) && TileSolid(tileX + 1, tileY) && TileSolid(tileX + 1, tileY + 1))
				{
					Main.tile[tileX, tileY].type = (ushort)((tileType == 60 || tileType == 70) ? 59u : 0u);
					fixedGrassCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Fixed grass. ({0})", fixedGrassCount);
	}
}
