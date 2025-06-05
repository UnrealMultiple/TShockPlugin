using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Slope : WECommand
{
	private Expression expression;

	private byte slope;

	public Slope(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int slope, Expression expression)
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
		if (slope == 1)
		{
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					ITile val = Main.tile[i, j];
					if (val.active() && select(i, j, plr) && expression.Evaluate(val) && magicWand.InSelection(i, j))
					{
						val.halfBrick(true);
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
			for (int k = x; k <= x2; k++)
			{
				for (int l = y; l <= y2; l++)
				{
					ITile val2 = Main.tile[k, l];
					if (val2.active() && select(k, l, plr) && expression.Evaluate(val2))
					{
						if (val2.halfBrick())
						{
							val2.halfBrick(false);
						}
						val2.slope(slope);
						num++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Sloped tiles. ({0})", num);
	}
}
