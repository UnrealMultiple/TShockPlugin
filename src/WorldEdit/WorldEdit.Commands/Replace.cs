using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Replace : WECommand
{
	private readonly Expression _expression;

	private readonly int _sourceTileType;

	private readonly int _targetTileType;

	public Replace(int x, int y, int x2, int y2, TSPlayer plr, int from, int to, Expression expression)
		: base(x, y, x2, y2, plr)
	{
		_sourceTileType = from;
		_targetTileType = to;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int replacedTileCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (((_sourceTileType >= 0 && tile.active() && _sourceTileType == tile.type) || (_sourceTileType == -1 && !tile.active()) || (_sourceTileType == -2 && tile.liquid != 0 && tile.liquidType() == 1) || (_sourceTileType == -3 && tile.liquid != 0 && tile.liquidType() == 2) || (_sourceTileType == -4 && tile.liquid == 0 && tile.liquidType() == 0)) && Tools.CanSet(Tile: true, tile, _targetTileType, select, _expression, magicWand, tileX, tileY, plr))
				{
					SetTile(tileX, tileY, _targetTileType);
					replacedTileCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Replaced tiles. ({0})", replacedTileCount);
	}
}
