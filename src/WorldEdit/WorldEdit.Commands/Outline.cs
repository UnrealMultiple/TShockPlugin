using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Outline : WECommand
{
	private Expression expression;

	private int tileType;

	private int color;

	private bool active;

	public Outline(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int tileType, int color, bool active, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		this.tileType = tileType;
		this.color = color;
		this.active = active;
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
		if (!CanUseCommand(x - 1, y - 1, x2 + 1, y2 + 1))
		{
			return;
		}
		Tools.PrepareUndo(x - 1, y - 1, x2 + 1, y2 + 1, plr);
		int num = 0;
		List<Point> list = new List<Point>();
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				ITile val = Main.tile[i, j];
				bool flag = val.active();
				bool flag2 = Main.tile[i - 1, j - 1].active();
				bool flag3 = Main.tile[i - 1, j + 1].active();
				bool flag4 = Main.tile[i - 1, j].active();
				bool flag5 = Main.tile[i + 1, j - 1].active();
				bool flag6 = Main.tile[i + 1, j + 1].active();
				bool flag7 = Main.tile[i + 1, j].active();
				bool flag8 = Main.tile[i, j - 1].active();
				bool flag9 = Main.tile[i, j + 1].active();
				if (flag && expression.Evaluate(Main.tile[i, j]) && magicWand.InSelection(i, j))
				{
					if (!flag2)
					{
						list.Add(new Point(i - 1, j - 1));
						num++;
					}
					if (!flag8)
					{
						list.Add(new Point(i, j - 1));
						num++;
					}
					if (!flag5)
					{
						list.Add(new Point(i + 1, j - 1));
						num++;
					}
					if (!flag4)
					{
						list.Add(new Point(i - 1, j));
						num++;
					}
					if (!flag7)
					{
						list.Add(new Point(i + 1, j));
						num++;
					}
					if (!flag3)
					{
						list.Add(new Point(i - 1, j + 1));
						num++;
					}
					if (!flag9)
					{
						list.Add(new Point(i, j + 1));
						num++;
					}
					if (!flag6)
					{
						list.Add(new Point(i + 1, j + 1));
						num++;
					}
				}
			}
		}
		foreach (Point item in list)
		{
			ITile val2 = Main.tile[item.X, item.Y];
			val2.color((byte)color);
			val2.inActive(!active);
			SetTile(item.X, item.Y, tileType);
		}
		ResetSection();
		plr.SendSuccessMessage("Set outline. ({0})", num);
	}
}
