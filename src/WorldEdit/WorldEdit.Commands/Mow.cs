using System.Linq;
using Terraria;
using TShockAPI;

namespace WorldEdit.Commands;

public class Mow : WECommand
{
	private static ushort[] mowedTiles = new ushort[17]
	{
		227, 3, 73, 24, 110, 113, 61, 74, 71, 201,
		32, 352, 69, 52, 205, 115, 62
	};

	public Mow(int x, int y, int x2, int y2, TSPlayer plr)
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
				if (mowedTiles.Contains(val.type))
				{
					val.active(false);
					val.type = 0;
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Mowed grass, thorns, and vines. ({0})", num);
	}
}
