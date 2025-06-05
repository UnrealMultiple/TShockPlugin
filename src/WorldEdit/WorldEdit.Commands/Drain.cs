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
		int num = 0;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				ITile val = Main.tile[i, j];
				if (val.liquid != 0)
				{
					val.liquid = 0;
					val.liquidType(0);
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Drained area. ({0})", num);
	}
}
