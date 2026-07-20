using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class FixHalves : WECommand
{
	public FixHalves(int x, int y, int x2, int y2, TSPlayer plr)
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
		int fixedHalfBlockCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.halfBrick() && TileSolid(tileX, tileY - 1))
				{
					tile.halfBrick(false);
					fixedHalfBlockCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Fixed half blocks. ({0})", fixedHalfBlockCount);
	}
}
