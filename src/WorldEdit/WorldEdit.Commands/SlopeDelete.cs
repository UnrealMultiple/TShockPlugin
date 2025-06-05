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
		int num = 0;
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
						num++;
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
					ITile val2 = Main.tile[k, l];
					if (val2.active() && select(k, l, plr) && expression.Evaluate(val2) && val2.slope() == 0 && val2.halfBrick())
					{
						val2.slope((byte)0);
						val2.halfBrick(false);
						num++;
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
					ITile val3 = Main.tile[m, n];
					if (val3.active() && select(m, n, plr) && expression.Evaluate(val3) && val3.slope() == slope)
					{
						val3.slope((byte)0);
						num++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Removed slopes. ({0})", num);
	}
}
