using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SetWire : WECommand
{
	private Expression expression;

	private bool state;

	private int wire;

	public SetWire(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int wire, bool state, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.expression = expression ?? new TestExpression((ITile t) => true);
		this.state = state;
		this.wire = wire;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int num = 0;
		switch (wire)
		{
		case 1:
		{
			for (int m = x; m <= x2; m++)
			{
				for (int n = y; n <= y2; n++)
				{
					ITile val3 = Main.tile[m, n];
					if (val3.wire() != state && select(m, n, plr) && expression.Evaluate(val3) && magicWand.InSelection(m, n))
					{
						val3.wire(state);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire. ({0})", num);
			break;
		}
		case 2:
		{
			for (int k = x; k <= x2; k++)
			{
				for (int l = y; l <= y2; l++)
				{
					ITile val2 = Main.tile[k, l];
					if (val2.wire2() != state && select(k, l, plr) && expression.Evaluate(val2))
					{
						val2.wire2(state);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 2. ({0})", num);
			break;
		}
		case 3:
		{
			for (int num2 = x; num2 <= x2; num2++)
			{
				for (int num3 = y; num3 <= y2; num3++)
				{
					ITile val4 = Main.tile[num2, num3];
					if (val4.wire3() != state && select(num2, num3, plr) && expression.Evaluate(val4))
					{
						val4.wire3(state);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 3. ({0})", num);
			break;
		}
		case 4:
		{
			for (int i = x; i <= x2; i++)
			{
				for (int j = y; j <= y2; j++)
				{
					ITile val = Main.tile[i, j];
					if (val.wire4() != state && select(i, j, plr) && expression.Evaluate(val))
					{
						val.wire4(state);
						num++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 4. ({0})", num);
			break;
		}
		}
	}
}
