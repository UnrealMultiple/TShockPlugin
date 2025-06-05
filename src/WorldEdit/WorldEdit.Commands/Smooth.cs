using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Smooth : WECommand
{
	private Expression expression;

	public Smooth(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.expression = expression ?? new TestExpression((ITile t) => true);
	}

	public override void Execute()
	{
		if (x < 1)
		{
			x = 1;
		}
		else if (x > Main.maxTilesX - 2)
		{
			x = Main.maxTilesX - 2;
		}
		if (y < 1)
		{
			y = 1;
		}
		else if (y > Main.maxTilesY - 2)
		{
			y = Main.maxTilesY - 2;
		}
		if (x2 < 1)
		{
			x2 = 1;
		}
		else if (x2 > Main.maxTilesX - 2)
		{
			x2 = Main.maxTilesX - 2;
		}
		if (y2 < 1)
		{
			y2 = 1;
		}
		else if (y2 > Main.maxTilesY - 2)
		{
			y2 = Main.maxTilesY - 2;
		}
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
				bool flag = Main.tile[i, j].active();
				bool flag2 = Main.tile[i, j].slope() == 0;
				bool flag3 = (magicWand.dontCheck ? Main.tile[i - 1, j].active() : magicWand.InSelection(i - 1, j));
				bool flag4 = (magicWand.dontCheck ? Main.tile[i + 1, j].active() : magicWand.InSelection(i + 1, j));
				bool flag5 = (magicWand.dontCheck ? Main.tile[i, j - 1].active() : magicWand.InSelection(i, j - 1));
				bool flag6 = (magicWand.dontCheck ? Main.tile[i, j + 1].active() : magicWand.InSelection(i, j + 1));
				if (flag && flag2 && expression.Evaluate(Main.tile[i, j]) && magicWand.InSelection(i, j))
				{
					if (flag3 && flag5 && !flag6 && !flag4)
					{
						Main.tile[i, j].slope((byte)3);
						num++;
					}
					else if (flag5 && flag4 && !flag6 && !flag3)
					{
						Main.tile[i, j].slope((byte)4);
						num++;
					}
					else if (flag4 && flag6 && !flag3 && !flag5)
					{
						Main.tile[i, j].slope((byte)2);
						num++;
					}
					else if (flag6 && flag3 && !flag5 && !flag4)
					{
						Main.tile[i, j].slope((byte)1);
						num++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Smoothed area. ({0})", num);
	}
}
