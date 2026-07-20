using System;
using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Shape : WECommand
{
	private readonly Expression expression;
	private readonly int shapeType;
	private readonly int rotateType;
	private readonly int flipType;
	private readonly bool wall;
	private readonly bool filled;
	private readonly int materialType;

	public Shape(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int shapeType, int rotateType, int flipType, bool wall, bool filled, int materialType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr, minMaxPoints: false)
	{
		this.expression = expression ?? new TestExpression(_ => true);
		this.shapeType = shapeType;
		this.rotateType = rotateType;
		this.flipType = flipType;
		this.wall = wall;
		this.filled = filled;
		this.materialType = materialType;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		if (shapeType != 0)
		{
			Position(force: true);
			Tools.PrepareUndo(x, y, x2, y2, plr);
		}
		else
		{
			Tools.PrepareUndo(Math.Min(x, x2), Math.Min(y, y2), Math.Max(x, x2), Math.Max(y, y2), plr);
		}
		int changedTileCount = 0;
		switch (shapeType)
		{
		case 0:
		{
			WEPoint[] linePoints = Tools.CreateLine(x, y, x2, y2);
			if (wall)
			{
				for (int pointIndex = 0; pointIndex < linePoints.Length; pointIndex++)
				{
					WEPoint point = linePoints[pointIndex];
					if (Tools.CanSet(Tile: false, Main.tile[(int)point.X, (int)point.Y], materialType, select, expression, magicWand, point.X, point.Y, plr))
					{
						Main.tile[(int)point.X, (int)point.Y].wall = (ushort)materialType;
						changedTileCount++;
					}
				}
				break;
			}
			for (int pointIndex = 0; pointIndex < linePoints.Length; pointIndex++)
			{
				WEPoint point = linePoints[pointIndex];
				if (Tools.CanSet(Tile: true, Main.tile[(int)point.X, (int)point.Y], materialType, select, expression, magicWand, point.X, point.Y, plr))
				{
					SetTile(point.X, point.Y, materialType);
					changedTileCount++;
				}
			}
			break;
		}
		case 1:
		{
			if (wall)
			{
				for (int tileX = x; tileX <= x2; tileX++)
				{
					for (int tileY = y; tileY <= y2; tileY++)
					{
						if (Tools.CanSet(Tile: false, Main.tile[tileX, tileY], materialType, select, expression, magicWand, tileX, tileY, plr) && (filled || WorldEdit.Selections["border"](tileX, tileY, plr)))
						{
							Main.tile[tileX, tileY].wall = (ushort)materialType;
							changedTileCount++;
						}
					}
				}
				break;
			}
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					if (Tools.CanSet(Tile: true, Main.tile[tileX, tileY], materialType, select, expression, magicWand, tileX, tileY, plr) && (filled || WorldEdit.Selections["border"](tileX, tileY, plr)))
					{
						SetTile(tileX, tileY, materialType);
						changedTileCount++;
					}
				}
			}
			break;
		}
		case 2:
		{
			if (filled)
			{
				if (wall)
				{
					for (int tileX = x; tileX <= x2; tileX++)
					{
						for (int tileY = y; tileY <= y2; tileY++)
						{
							if (Tools.CanSet(Tile: false, Main.tile[tileX, tileY], materialType, select, expression, magicWand, tileX, tileY, plr) && WorldEdit.Selections["ellipse"](tileX, tileY, plr))
							{
								Main.tile[tileX, tileY].wall = (ushort)materialType;
								changedTileCount++;
							}
						}
					}
					break;
				}
				for (int tileX = x; tileX <= x2; tileX++)
				{
					for (int tileY = y; tileY <= y2; tileY++)
					{
						if (Tools.CanSet(Tile: true, Main.tile[tileX, tileY], materialType, select, expression, magicWand, tileX, tileY, plr) && WorldEdit.Selections["ellipse"](tileX, tileY, plr))
						{
							SetTile(tileX, tileY, materialType);
							changedTileCount++;
						}
					}
				}
				break;
			}
			WEPoint[] ellipseOutline = Tools.CreateEllipseOutline(x, y, x2, y2);
			if (wall)
			{
				WEPoint[] outlinePoints = ellipseOutline;
				for (int pointIndex = 0; pointIndex < outlinePoints.Length; pointIndex++)
				{
					WEPoint point = outlinePoints[pointIndex];
					if (Tools.CanSet(Tile: false, Main.tile[(int)point.X, (int)point.Y], materialType, select, expression, magicWand, point.X, point.Y, plr))
					{
						Main.tile[(int)point.X, (int)point.Y].wall = (ushort)materialType;
						changedTileCount++;
					}
				}
				break;
			}
			WEPoint[] outlineTilePoints = ellipseOutline;
			for (int pointIndex = 0; pointIndex < outlineTilePoints.Length; pointIndex++)
			{
				WEPoint point = outlineTilePoints[pointIndex];
				if (Tools.CanSet(Tile: true, Main.tile[(int)point.X, (int)point.Y], materialType, select, expression, magicWand, point.X, point.Y, plr))
				{
					SetTile(point.X, point.Y, materialType);
					changedTileCount++;
				}
			}
			break;
		}
		case 3:
		case 4:
		{
			WEPoint[] outlinePoints;
			WEPoint[] firstEdgePoints;
			WEPoint[] secondEdgePoints;
			if (shapeType == 3)
			{
				switch (rotateType)
				{
				default:
					return;
				case 0:
				{
					int centerX = x + (x2 - x) / 2;
					outlinePoints = Tools.CreateLine(centerX, y, x, y2).Concat(Tools.CreateLine(centerX + (x2 - x) % 2, y, x2, y2)).ToArray();
					firstEdgePoints = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y2),
						new WEPoint((short)x2, (short)y2)
					};
					secondEdgePoints = null;
					break;
				}
				case 1:
				{
					int centerX = x + (x2 - x) / 2;
					outlinePoints = Tools.CreateLine(centerX, y2, x, y).Concat(Tools.CreateLine(centerX + (x2 - x) % 2, y2, x2, y)).ToArray();
					firstEdgePoints = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y),
						new WEPoint((short)x2, (short)y)
					};
					secondEdgePoints = null;
					break;
				}
				case 2:
				{
					int centerY = y + (y2 - y) / 2;
					outlinePoints = Tools.CreateLine(x, centerY, x2, y).Concat(Tools.CreateLine(x, centerY + (y2 - y) % 2, x2, y2)).ToArray();
					firstEdgePoints = new WEPoint[2]
					{
						new WEPoint((short)x2, (short)y),
						new WEPoint((short)x2, (short)y2)
					};
					secondEdgePoints = null;
					break;
				}
				case 3:
				{
					int centerY = y + (y2 - y) / 2;
					outlinePoints = Tools.CreateLine(x2, centerY, x, y).Concat(Tools.CreateLine(x2, centerY + (x2 - x) % 2, x, y2)).ToArray();
					firstEdgePoints = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y),
						new WEPoint((short)x, (short)y2)
					};
					secondEdgePoints = null;
					break;
				}
				}
			}
			else
			{
				switch (flipType)
				{
				default:
					return;
				case 0:
					switch (rotateType)
					{
					default:
						return;
					case 0:
						outlinePoints = Tools.CreateLine(x, y2, x2, y);
						firstEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y2),
							new WEPoint((short)x2, (short)y2)
						};
						secondEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x2, (short)y),
							new WEPoint((short)x2, (short)y2)
						};
						break;
					case 1:
						outlinePoints = Tools.CreateLine(x, y, x2, y2);
						firstEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x2, (short)y)
						};
						secondEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x2, (short)y),
							new WEPoint((short)x2, (short)y2)
						};
						break;
					}
					break;
				case 1:
					switch (rotateType)
					{
					default:
						return;
					case 0:
						outlinePoints = Tools.CreateLine(x, y, x2, y2);
						firstEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x, (short)y2)
						};
						secondEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y2),
							new WEPoint((short)x2, (short)y2)
						};
						break;
					case 1:
						outlinePoints = Tools.CreateLine(x, y2, x2, y);
						firstEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x, (short)y2)
						};
						secondEdgePoints = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x2, (short)y)
						};
						break;
					}
					break;
				}
			}
			if (filled)
			{
				switch (rotateType)
				{
				default:
					return;
				case 0:
				{
					if (wall)
					{
						WEPoint[] fillBoundaryPoints = outlinePoints;
						for (int pointIndex = 0; pointIndex < fillBoundaryPoints.Length; pointIndex++)
						{
							WEPoint point = fillBoundaryPoints[pointIndex];
							for (int tileY = point.Y; tileY <= y2; tileY++)
							{
								ITile tile = Main.tile[(int)point.X, tileY];
								if (Tools.CanSet(Tile: false, tile, materialType, select, expression, magicWand, point.X, tileY, plr))
								{
									tile.wall = (ushort)materialType;
									changedTileCount++;
								}
							}
						}
						break;
					}
					WEPoint[] fillTileBoundaryPoints = outlinePoints;
					for (int pointIndex = 0; pointIndex < fillTileBoundaryPoints.Length; pointIndex++)
					{
						WEPoint point = fillTileBoundaryPoints[pointIndex];
						for (int tileY = point.Y; tileY <= y2; tileY++)
						{
							ITile tile = Main.tile[(int)point.X, tileY];
							if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, point.X, tileY, plr))
							{
								SetTile(point.X, tileY, materialType);
								changedTileCount++;
							}
						}
					}
					break;
				}
				case 1:
				{
					if (wall)
					{
						WEPoint[] fillBoundaryPoints = outlinePoints;
						for (int pointIndex = 0; pointIndex < fillBoundaryPoints.Length; pointIndex++)
						{
							WEPoint point = fillBoundaryPoints[pointIndex];
							for (int tileY = point.Y; tileY >= y; tileY--)
							{
								ITile tile = Main.tile[(int)point.X, tileY];
								if (Tools.CanSet(Tile: false, tile, materialType, select, expression, magicWand, point.X, tileY, plr))
								{
									tile.wall = (ushort)materialType;
									changedTileCount++;
								}
							}
						}
						break;
					}
					WEPoint[] fillTileBoundaryPoints = outlinePoints;
					for (int pointIndex = 0; pointIndex < fillTileBoundaryPoints.Length; pointIndex++)
					{
						WEPoint point = fillTileBoundaryPoints[pointIndex];
						for (int tileY = point.Y; tileY >= y; tileY--)
						{
							ITile tile = Main.tile[(int)point.X, tileY];
							if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, point.X, tileY, plr))
							{
								SetTile(point.X, tileY, materialType);
								changedTileCount++;
							}
						}
					}
					break;
				}
				case 2:
				{
					if (wall)
					{
						WEPoint[] fillBoundaryPoints = outlinePoints;
						for (int pointIndex = 0; pointIndex < fillBoundaryPoints.Length; pointIndex++)
						{
							WEPoint point = fillBoundaryPoints[pointIndex];
							for (int tileX = point.X; tileX <= x2; tileX++)
							{
								ITile tile = Main.tile[tileX, (int)point.Y];
								if (Tools.CanSet(Tile: false, tile, materialType, select, expression, magicWand, tileX, point.Y, plr))
								{
									tile.wall = (ushort)materialType;
									changedTileCount++;
								}
							}
						}
						break;
					}
					WEPoint[] fillTileBoundaryPoints = outlinePoints;
					for (int pointIndex = 0; pointIndex < fillTileBoundaryPoints.Length; pointIndex++)
					{
						WEPoint point = fillTileBoundaryPoints[pointIndex];
						for (int tileX = point.X; tileX <= x2; tileX++)
						{
							ITile tile = Main.tile[tileX, (int)point.Y];
							if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, point.Y, plr))
							{
								SetTile(tileX, point.Y, materialType);
								changedTileCount++;
							}
						}
					}
					break;
				}
				case 3:
				{
					if (wall)
					{
						WEPoint[] fillBoundaryPoints = outlinePoints;
						for (int pointIndex = 0; pointIndex < fillBoundaryPoints.Length; pointIndex++)
						{
							WEPoint point = fillBoundaryPoints[pointIndex];
							for (int tileX = point.X; tileX >= x; tileX--)
							{
								ITile tile = Main.tile[tileX, (int)point.Y];
								if (Tools.CanSet(Tile: false, tile, materialType, select, expression, magicWand, tileX, point.Y, plr))
								{
									tile.wall = (ushort)materialType;
									changedTileCount++;
								}
							}
						}
						break;
					}
					WEPoint[] fillTileBoundaryPoints = outlinePoints;
					for (int pointIndex = 0; pointIndex < fillTileBoundaryPoints.Length; pointIndex++)
					{
						WEPoint point = fillTileBoundaryPoints[pointIndex];
						for (int tileX = point.X; tileX >= x; tileX++)
						{
							ITile tile = Main.tile[tileX, (int)point.Y];
							if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, point.Y, plr))
							{
								SetTile(tileX, point.Y, materialType);
								changedTileCount++;
							}
						}
					}
					break;
				}
				}
				break;
			}
			if (wall)
			{
				WEPoint[] wallOutlinePoints = outlinePoints;
				for (int pointIndex = 0; pointIndex < wallOutlinePoints.Length; pointIndex++)
				{
					WEPoint point = wallOutlinePoints[pointIndex];
					ITile tile = Main.tile[(int)point.X, (int)point.Y];
					if (Tools.CanSet(Tile: false, tile, materialType, select, expression, magicWand, point.X, point.Y, plr))
					{
						tile.wall = (ushort)materialType;
						changedTileCount++;
					}
				}
				for (int tileX = firstEdgePoints[0].X; tileX <= firstEdgePoints[1].X; tileX++)
				{
					for (int tileY = firstEdgePoints[0].Y; tileY <= firstEdgePoints[1].Y; tileY++)
					{
						ITile tile = Main.tile[tileX, tileY];
						if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, tileY, plr))
						{
							tile.wall = (ushort)materialType;
							changedTileCount++;
						}
					}
				}
				if (secondEdgePoints == null)
				{
					break;
				}
				for (int tileX = secondEdgePoints[0].X; tileX <= secondEdgePoints[1].X; tileX++)
				{
					for (int tileY = secondEdgePoints[0].Y; tileY <= secondEdgePoints[1].Y; tileY++)
					{
						ITile tile = Main.tile[tileX, tileY];
						if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, tileY, plr))
						{
							tile.wall = (ushort)materialType;
							changedTileCount++;
						}
					}
				}
				break;
			}
			WEPoint[] tileOutlinePoints = outlinePoints;
			for (int pointIndex = 0; pointIndex < tileOutlinePoints.Length; pointIndex++)
			{
				WEPoint point = tileOutlinePoints[pointIndex];
				ITile tile = Main.tile[(int)point.X, (int)point.Y];
				if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, point.X, point.Y, plr))
				{
					SetTile(point.X, point.Y, materialType);
					changedTileCount++;
				}
			}
			for (int tileX = firstEdgePoints[0].X; tileX <= firstEdgePoints[1].X; tileX++)
			{
				for (int tileY = firstEdgePoints[0].Y; tileY <= firstEdgePoints[1].Y; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, tileY, plr))
					{
						SetTile(tileX, tileY, materialType);
						changedTileCount++;
					}
				}
			}
			if (secondEdgePoints == null)
			{
				break;
			}
			for (int tileX = secondEdgePoints[0].X; tileX <= secondEdgePoints[1].X; tileX++)
			{
				for (int tileY = secondEdgePoints[0].Y; tileY <= secondEdgePoints[1].Y; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (Tools.CanSet(Tile: true, tile, materialType, select, expression, magicWand, tileX, tileY, plr))
					{
						SetTile(tileX, tileY, materialType);
						changedTileCount++;
					}
				}
			}
			break;
		}
		}
		ResetSection();
		plr.SendSuccessMessage("Set {0}{1} shape. ({2})", filled ? "filled " : "", wall ? "wall" : "tile", changedTileCount);
	}
}
