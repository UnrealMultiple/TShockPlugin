using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SetWall : WECommand
{
	private readonly Expression _expression;

	private readonly int _wallType;

	public SetWall(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int wallType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_expression = expression ?? new TestExpression(_ => true);
		_wallType = wallType;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int changedWallCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				if (Tools.CanSet(Tile: false, Main.tile[tileX, tileY], _wallType, select, _expression, magicWand, tileX, tileY, plr))
				{
					Main.tile[tileX, tileY].wall = (ushort)_wallType;
					changedWallCount++;
				}
			}
		}
		ResetSection();
		string wallDescription = _wallType == 0 ? "air" : "wall " + _wallType;
		plr.SendSuccessMessage("Set walls to {0}. ({1})", wallDescription, changedWallCount);
	}
}
