using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class PaintWall : WECommand
{
	private readonly int _color;

	private readonly Expression _expression;

	public PaintWall(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int color, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_color = color;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int paintedWallCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.wall > 0 && tile.wallColor() != _color && select(tileX, tileY, plr) && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
				{
					tile.wallColor((byte)_color);
					paintedWallCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Painted walls. ({0})", paintedWallCount);
	}
}
