using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Set : WECommand
{
	private readonly Expression _expression;

	private readonly int _tileType;

	public Set(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int tileType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_tileType = tileType;
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
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				if (Tools.CanSet(Tile: true, Main.tile[tileX, tileY], _tileType, select, _expression, magicWand, tileX, tileY, plr))
				{
					SetTile(tileX, tileY, _tileType);
					changedTileCount++;
				}
			}
		}
		ResetSection();
		plr.SendSuccessMessage("Set tiles. ({0})", changedTileCount);
	}
}
