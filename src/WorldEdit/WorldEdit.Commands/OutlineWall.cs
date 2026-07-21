using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class OutlineWall : WECommand
{
	private static readonly Point[] NeighborOffsets =
	{
		new(-1, -1), new(0, -1), new(1, -1), new(-1, 0),
		new(1, 0), new(-1, 1), new(0, 1), new(1, 1)
	};

	private readonly Expression _expression;
	private readonly int _wallType;
	private readonly int _color;

	public OutlineWall(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int wallType, int color, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_wallType = wallType;
		_color = color;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		x = Math.Clamp(x, 1, Main.maxTilesX - 2);
		y = Math.Clamp(y, 1, Main.maxTilesY - 2);
		x2 = Math.Clamp(x2, 1, Main.maxTilesX - 2);
		y2 = Math.Clamp(y2, 1, Main.maxTilesY - 2);
		if (!CanUseCommand(x - 1, y - 1, x2 + 1, y2 + 1))
		{
			return;
		}

		Tools.PrepareUndo(x - 1, y - 1, x2 + 1, y2 + 1, plr);
		int outlinedWallCount = 0;
		List<Point> outlinePositions = new List<Point>();
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (tile.wall == 0 || !_expression.Evaluate(tile) || !magicWand.InSelection(tileX, tileY))
				{
					continue;
				}

				foreach (Point offset in NeighborOffsets)
				{
					int neighborX = tileX + offset.X;
					int neighborY = tileY + offset.Y;
					if (Main.tile[neighborX, neighborY].wall == 0)
					{
						outlinePositions.Add(new Point(neighborX, neighborY));
						outlinedWallCount++;
					}
				}
			}
		}

		foreach (Point position in outlinePositions)
		{
			ITile tile = Main.tile[position.X, position.Y];
			tile.wallColor((byte)_color);
			tile.wall = (ushort)_wallType;
		}

		ResetSection();
		plr.SendSuccessMessage("Set wall outline. ({0})", outlinedWallCount);
	}
}
