using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

internal class SlopeDelete : WECommand
{
	private Expression expression;

	private byte slope;

	public SlopeDelete(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int slope, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.slope = (byte)slope;
		this.expression = expression ?? new TestExpression((ITile t) => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int removedSlopeCount = 0;
		if (slope == byte.MaxValue)
		{
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					ITile val = Main.tile[i, j];
					if (val.active() && select(i, j, plr) && expression.Evaluate(val) && magicWand.InSelection(i, j))
					{
						val.slope((byte)0);
						val.halfBrick(false);
						removedSlopeCount++;
					}
				}
			}
		}
		else if (slope == 1)
		{
			for (int k = x; k <= x2; k++)
			{
				for (int l = y; l <= y2; l++)
				{
					ITile tile = Main.tile[k, l];
					if (tile.active() && select(k, l, plr) && expression.Evaluate(tile) && tile.slope() == 0 && tile.halfBrick())
					{
						tile.slope((byte)0);
						tile.halfBrick(false);
						removedSlopeCount++;
					}
				}
			}
		}
		else
		{
			if (slope > 1)
			{
				slope--;
			}
			for (int m = x; m <= x2; m++)
			{
				for (int n = y; n <= y2; n++)
				{
					ITile tile = Main.tile[m, n];
					if (tile.active() && select(m, n, plr) && expression.Evaluate(tile) && tile.slope() == slope)
					{
						tile.slope((byte)0);
						removedSlopeCount++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Removed slopes. ({0})", removedSlopeCount);
	}
}
