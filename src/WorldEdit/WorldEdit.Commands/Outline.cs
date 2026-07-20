using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Outline : WECommand
{
	private static readonly Point[] NeighborOffsets =
	{
		new(-1, -1), new(0, -1), new(1, -1), new(-1, 0),
		new(1, 0), new(-1, 1), new(0, 1), new(1, 1)
	};

	private readonly Expression _expression;
	private readonly int _tileType;
	private readonly int _color;
	private readonly bool _active;

	public Outline(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int tileType, int color, bool active, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_tileType = tileType;
		_color = color;
		_active = active;
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
		int outlinedTileCount = 0;
		List<Point> outlinePositions = new List<Point>();
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				ITile tile = Main.tile[tileX, tileY];
				if (!tile.active() || !_expression.Evaluate(tile) || !magicWand.InSelection(tileX, tileY))
				{
					continue;
				}

				foreach (Point offset in NeighborOffsets)
				{
					int neighborX = tileX + offset.X;
					int neighborY = tileY + offset.Y;
					if (!Main.tile[neighborX, neighborY].active())
					{
						outlinePositions.Add(new Point(neighborX, neighborY));
						outlinedTileCount++;
					}
				}
			}
		}

		foreach (Point position in outlinePositions)
		{
			SetTile(position.X, position.Y, _tileType);
			ITile tile = Main.tile[position.X, position.Y];
			tile.color((byte)_color);
			tile.inActive(!_active);
		}

		ResetSection();
		plr.SendSuccessMessage("Set outline. ({0})", outlinedTileCount);
	}
}
