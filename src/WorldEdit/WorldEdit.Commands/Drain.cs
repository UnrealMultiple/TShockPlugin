using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Drain : WECommand
{
	public Drain(int x, int y, int x2, int y2, TSPlayer plr)
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
		int drainedTileCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.liquid != 0)
				{
					tile.liquid = 0;
					tile.liquidType(0);
					drainedTileCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Drained area. ({0})", drainedTileCount);
	}
}
