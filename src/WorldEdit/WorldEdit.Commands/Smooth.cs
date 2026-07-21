using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Smooth : WECommand
{
	private readonly Expression _expression;

	public Smooth(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_expression = expression ?? new TestExpression(_ => true);
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
		int smoothedTileCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				bool leftSelected = magicWand.dontCheck ? Main.tile[tileX - 1, tileY].active() : magicWand.InSelection(tileX - 1, tileY);
				bool rightSelected = magicWand.dontCheck ? Main.tile[tileX + 1, tileY].active() : magicWand.InSelection(tileX + 1, tileY);
				bool topSelected = magicWand.dontCheck ? Main.tile[tileX, tileY - 1].active() : magicWand.InSelection(tileX, tileY - 1);
				bool bottomSelected = magicWand.dontCheck ? Main.tile[tileX, tileY + 1].active() : magicWand.InSelection(tileX, tileY + 1);
				if (tile.active() && tile.slope() == 0 && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
				{
					if (leftSelected && topSelected && !bottomSelected && !rightSelected)
					{
						tile.slope((byte)3);
						smoothedTileCount++;
					}
					else if (topSelected && rightSelected && !bottomSelected && !leftSelected)
					{
						tile.slope((byte)4);
						smoothedTileCount++;
					}
					else if (rightSelected && bottomSelected && !leftSelected && !topSelected)
					{
						tile.slope((byte)2);
						smoothedTileCount++;
					}
					else if (bottomSelected && leftSelected && !topSelected && !rightSelected)
					{
						tile.slope((byte)1);
						smoothedTileCount++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Smoothed area. ({0})", smoothedTileCount);
	}
}
