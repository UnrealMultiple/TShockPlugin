using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Paint : WECommand
{
	private int color;

	private Expression expression;

	public Paint(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int color, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.color = color;
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
				if (val.active() && val.color() != color && select(i, j, plr) && expression.Evaluate(val) && magicWand.InSelection(i, j))
				{
					val.color((byte)color);
					num++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Painted tiles. ({0})", num);
	}
}
