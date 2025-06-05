using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Inactive : WECommand
{
	private Expression expression;

	private int inactiveType;

	public Inactive(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int inacType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		inactiveType = inacType;
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
		switch (inactiveType)
		{
		case 0:
		{
			for (int m = x; m <= x2; m++)
			{
				for (int n = y; n <= y2; n++)
				{
					ITile val3 = Main.tile[m, n];
					if (val3.active() && !val3.inActive() && select(m, n, plr) && expression.Evaluate(val3) && magicWand.InSelection(m, n))
					{
						val3.inActive(true);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Made tiles inactive. ({0})", num);
			break;
		}
		case 1:
		{
			for (int k = x; k <= x2; k++)
			{
				for (int l = y; l <= y2; l++)
				{
					ITile val2 = Main.tile[k, l];
					if (val2.inActive() && select(k, l, plr) && expression.Evaluate(val2))
					{
						val2.inActive(false);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set\ttiles' inactive\tstatus off.\t({0})", num);
			break;
		}
		case 2:
		{
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					ITile val = Main.tile[i, j];
					if (val.active() && select(i, j, plr) && expression.Evaluate(val))
					{
						val.inActive(!val.inActive());
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Reversed tiles'\tinactive status. ({0})", num);
			break;
		}
		}
	}
}
