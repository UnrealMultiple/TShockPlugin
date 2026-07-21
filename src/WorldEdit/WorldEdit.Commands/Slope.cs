using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Slope : WECommand
{
	private readonly Expression _expression;

	private byte _slope;

	public Slope(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int slope, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_slope = (byte)slope;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int changedTileCount = 0;
		if (_slope == 1)
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.active() && select(tileX, tileY, plr) && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
					{
						tile.halfBrick(true);
						changedTileCount++;
					}
				}
			}
		}
		else
		{
			if (_slope > 1)
			{
				_slope--;
			}
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.active() && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						if (tile.halfBrick())
						{
							tile.halfBrick(false);
						}
						tile.slope(_slope);
						changedTileCount++;
					}
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Sloped tiles. ({0})", changedTileCount);
	}
}
