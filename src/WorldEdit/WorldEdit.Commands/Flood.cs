using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Flood : WECommand
{
	private int liquid;

	public Flood(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int liquid)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.liquid = liquid;
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
				if ((!val.active() || !Main.tileSolid[val.type]) && magicWand.InSelection(i, j))
				{
					val.liquidType((int)(byte)liquid);
					val.liquid = byte.MaxValue;
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Flooded area. ({0})", num);
	}
}
