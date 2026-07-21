using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class FixSlopes : WECommand
{
	public FixSlopes(int x, int y, int x2, int y2, TSPlayer plr)
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
		int fixedSlopeCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				int slope = Main.tile[tileX, tileY].slope();
				if (((slope == 1 || slope == 2) && TileSolid(tileX, tileY - 1)) || ((slope == 3 || slope == 4) && TileSolid(tileX, tileY + 1)))
				{
					Main.tile[tileX, tileY].slope((byte)0);
					fixedSlopeCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Fixed slopes. ({0})", fixedSlopeCount);
	}
}
