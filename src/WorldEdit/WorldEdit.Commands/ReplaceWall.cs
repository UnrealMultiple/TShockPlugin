using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class ReplaceWall : WECommand
{
	private readonly Expression _expression;

	private readonly int _sourceWallType;

	private readonly int _targetWallType;

	public ReplaceWall(int x, int y, int x2, int y2, TSPlayer plr, int from, int to, Expression expression)
		: base(x, y, x2, y2, plr)
	{
		_sourceWallType = from;
		_targetWallType = to;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int replacedWallCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.wall == _sourceWallType && Tools.CanSet(Tile: false, tile, _targetWallType, select, _expression, magicWand, tileX, tileY, plr))
				{
					tile.wall = (byte)_targetWallType;
					replacedWallCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Replaced walls. ({0})", replacedWallCount);
	}
}
