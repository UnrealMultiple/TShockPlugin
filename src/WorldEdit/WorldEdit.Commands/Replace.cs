using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Replace : WECommand
{
	private Expression expression;

	private int from;

	private int to;

	public Replace(int x, int y, int x2, int y2, TSPlayer plr, int from, int to, Expression expression)
		: base(x, y, x2, y2, plr)
	{
		this.from = from;
		this.to = to;
		this.expression = expression ?? new TestExpression((ITile t) => true);
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
				if (((from >= 0 && val.active() && from == val.type) || (from == -1 && !val.active()) || (from == -2 && val.liquid != 0 && val.liquidType() == 1) || (from == -3 && val.liquid != 0 && val.liquidType() == 2) || (from == -4 && val.liquid == 0 && val.liquidType() == 0)) && Tools.CanSet(Tile: true, val, to, select, expression, magicWand, i, j, plr))
				{
					SetTile(i, j, to);
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Replaced tiles. ({0})", num);
	}
}
