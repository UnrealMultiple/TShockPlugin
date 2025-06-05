using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class ReplaceWall : WECommand
{
	private Expression expression;

	private int from;

	private int to;

	public ReplaceWall(int x, int y, int x2, int y2, TSPlayer plr, int from, int to, Expression expression)
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
				if (val.wall == from && Tools.CanSet(Tile: false, val, to, select, expression, magicWand, i, j, plr))
				{
					val.wall = (byte)to;
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Replaced walls. ({0})", num);
	}
}
