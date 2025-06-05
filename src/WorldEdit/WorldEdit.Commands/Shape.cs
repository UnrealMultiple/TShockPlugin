using System;
using System.Linq;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Shape : WECommand
{
	private Expression expression;

	private int shapeType;

	private int rotateType;

	private int flipType;

	private bool wall;

	private bool filled;

	private int materialType;

	public Shape(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int shapeType, int rotateType, int flipType, bool wall, bool filled, int materialType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr, minMaxPoints: false)
	{
		this.expression = expression ?? new TestExpression((ITile t) => true);
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
		int num = 0;
		switch (shapeType)
		{
		case 0:
		{
			WEPoint[] array17 = Tools.CreateLine(x, y, x2, y2);
			if (wall)
			{
				WEPoint[] array18 = array17;
				for (int num36 = 0; num36 < array18.Length; num36++)
				{
					WEPoint wEPoint13 = array18[num36];
					ITile val15 = Main.tile[(int)wEPoint13.X, (int)wEPoint13.Y];
					if (Tools.CanSet(Tile: false, Main.tile[(int)wEPoint13.X, (int)wEPoint13.Y], materialType, select, expression, magicWand, wEPoint13.X, wEPoint13.Y, plr))
					{
						Main.tile[(int)wEPoint13.X, (int)wEPoint13.Y].wall = (ushort)materialType;
						num++;
					}
				}
				break;
			}
			WEPoint[] array19 = array17;
			for (int num37 = 0; num37 < array19.Length; num37++)
			{
				WEPoint wEPoint14 = array19[num37];
				ITile val16 = Main.tile[(int)wEPoint14.X, (int)wEPoint14.Y];
				if (Tools.CanSet(Tile: true, Main.tile[(int)wEPoint14.X, (int)wEPoint14.Y], materialType, select, expression, magicWand, wEPoint14.X, wEPoint14.Y, plr))
				{
					SetTile(wEPoint14.X, wEPoint14.Y, materialType);
					num++;
				}
			}
			break;
		}
		case 1:
		{
			if (wall)
			{
				for (int num26 = x; num26 <= x2; num26++)
				{
					for (int num27 = y; num27 <= y2; num27++)
					{
						if (Tools.CanSet(Tile: false, Main.tile[num26, num27], materialType, select, expression, magicWand, num26, num27, plr) && (filled || WorldEdit.Selections["border"](num26, num27, plr)))
						{
							Main.tile[num26, num27].wall = (ushort)materialType;
							num++;
						}
					}
				}
				break;
			}
			for (int num28 = x; num28 <= x2; num28++)
			{
				for (int num29 = y; num29 <= y2; num29++)
				{
					if (Tools.CanSet(Tile: true, Main.tile[num28, num29], materialType, select, expression, magicWand, num28, num29, plr) && (filled || WorldEdit.Selections["border"](num28, num29, plr)))
					{
						SetTile(num28, num29, materialType);
						num++;
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
					for (int num30 = x; num30 <= x2; num30++)
					{
						for (int num31 = y; num31 <= y2; num31++)
						{
							if (Tools.CanSet(Tile: false, Main.tile[num30, num31], materialType, select, expression, magicWand, num30, num31, plr) && WorldEdit.Selections["ellipse"](num30, num31, plr))
							{
								Main.tile[num30, num31].wall = (ushort)materialType;
								num++;
							}
						}
					}
					break;
				}
				for (int num32 = x; num32 <= x2; num32++)
				{
					for (int num33 = y; num33 <= y2; num33++)
					{
						if (Tools.CanSet(Tile: true, Main.tile[num32, num33], materialType, select, expression, magicWand, num32, num33, plr) && WorldEdit.Selections["ellipse"](num32, num33, plr))
						{
							SetTile(num32, num33, materialType);
							num++;
						}
					}
				}
				break;
			}
			WEPoint[] array14 = Tools.CreateEllipseOutline(x, y, x2, y2);
			if (wall)
			{
				WEPoint[] array15 = array14;
				for (int num34 = 0; num34 < array15.Length; num34++)
				{
					WEPoint wEPoint11 = array15[num34];
					if (Tools.CanSet(Tile: false, Main.tile[(int)wEPoint11.X, (int)wEPoint11.Y], materialType, select, expression, magicWand, wEPoint11.X, wEPoint11.Y, plr))
					{
						Main.tile[(int)wEPoint11.X, (int)wEPoint11.Y].wall = (ushort)materialType;
						num++;
					}
				}
				break;
			}
			WEPoint[] array16 = array14;
			for (int num35 = 0; num35 < array16.Length; num35++)
			{
				WEPoint wEPoint12 = array16[num35];
				if (Tools.CanSet(Tile: true, Main.tile[(int)wEPoint12.X, (int)wEPoint12.Y], materialType, select, expression, magicWand, wEPoint12.X, wEPoint12.Y, plr))
				{
					SetTile(wEPoint12.X, wEPoint12.Y, materialType);
					num++;
				}
			}
			break;
		}
		case 3:
		case 4:
		{
			WEPoint[] array;
			WEPoint[] array2;
			WEPoint[] array3;
			if (shapeType == 3)
			{
				switch (rotateType)
				{
				default:
					return;
				case 0:
				{
					int num5 = x + (x2 - x) / 2;
					array = Tools.CreateLine(num5, y, x, y2).Concat(Tools.CreateLine(num5 + (x2 - x) % 2, y, x2, y2)).ToArray();
					array2 = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y2),
						new WEPoint((short)x2, (short)y2)
					};
					array3 = null;
					break;
				}
				case 1:
				{
					int num4 = x + (x2 - x) / 2;
					array = Tools.CreateLine(num4, y2, x, y).Concat(Tools.CreateLine(num4 + (x2 - x) % 2, y2, x2, y)).ToArray();
					array2 = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y),
						new WEPoint((short)x2, (short)y)
					};
					array3 = null;
					break;
				}
				case 2:
				{
					int num3 = y + (y2 - y) / 2;
					array = Tools.CreateLine(x, num3, x2, y).Concat(Tools.CreateLine(x, num3 + (y2 - y) % 2, x2, y2)).ToArray();
					array2 = new WEPoint[2]
					{
						new WEPoint((short)x2, (short)y),
						new WEPoint((short)x2, (short)y2)
					};
					array3 = null;
					break;
				}
				case 3:
				{
					int num2 = y + (y2 - y) / 2;
					array = Tools.CreateLine(x2, num2, x, y).Concat(Tools.CreateLine(x2, num2 + (x2 - x) % 2, x, y2)).ToArray();
					array2 = new WEPoint[2]
					{
						new WEPoint((short)x, (short)y),
						new WEPoint((short)x, (short)y2)
					};
					array3 = null;
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
						array = Tools.CreateLine(x, y2, x2, y);
						array2 = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y2),
							new WEPoint((short)x2, (short)y2)
						};
						array3 = new WEPoint[2]
						{
							new WEPoint((short)x2, (short)y),
							new WEPoint((short)x2, (short)y2)
						};
						break;
					case 1:
						array = Tools.CreateLine(x, y, x2, y2);
						array2 = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x2, (short)y)
						};
						array3 = new WEPoint[2]
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
						array = Tools.CreateLine(x, y, x2, y2);
						array2 = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x, (short)y2)
						};
						array3 = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y2),
							new WEPoint((short)x2, (short)y2)
						};
						break;
					case 1:
						array = Tools.CreateLine(x, y2, x2, y);
						array2 = new WEPoint[2]
						{
							new WEPoint((short)x, (short)y),
							new WEPoint((short)x, (short)y2)
						};
						array3 = new WEPoint[2]
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
						WEPoint[] array8 = array;
						for (int n = 0; n < array8.Length; n++)
						{
							WEPoint wEPoint5 = array8[n];
							for (int num9 = wEPoint5.Y; num9 <= y2; num9++)
							{
								ITile val5 = Main.tile[(int)wEPoint5.X, num9];
								if (Tools.CanSet(Tile: false, Main.tile[(int)wEPoint5.X, num9], materialType, select, expression, magicWand, wEPoint5.X, num9, plr))
								{
									Main.tile[(int)wEPoint5.X, num9].wall = (ushort)materialType;
									num++;
								}
							}
						}
						break;
					}
					WEPoint[] array9 = array;
					for (int num10 = 0; num10 < array9.Length; num10++)
					{
						WEPoint wEPoint6 = array9[num10];
						for (int num11 = wEPoint6.Y; num11 <= y2; num11++)
						{
							ITile val6 = Main.tile[(int)wEPoint6.X, num11];
							if (Tools.CanSet(Tile: true, Main.tile[(int)wEPoint6.X, num11], materialType, select, expression, magicWand, wEPoint6.X, num11, plr))
							{
								SetTile(wEPoint6.X, num11, materialType);
								num++;
							}
						}
					}
					break;
				}
				case 1:
				{
					if (wall)
					{
						WEPoint[] array6 = array;
						for (int l = 0; l < array6.Length; l++)
						{
							WEPoint wEPoint3 = array6[l];
							for (int num7 = wEPoint3.Y; num7 >= y; num7--)
							{
								ITile val3 = Main.tile[(int)wEPoint3.X, num7];
								if (Tools.CanSet(Tile: false, Main.tile[(int)wEPoint3.X, num7], materialType, select, expression, magicWand, wEPoint3.X, num7, plr))
								{
									Main.tile[(int)wEPoint3.X, num7].wall = (ushort)materialType;
									num++;
								}
							}
						}
						break;
					}
					WEPoint[] array7 = array;
					for (int m = 0; m < array7.Length; m++)
					{
						WEPoint wEPoint4 = array7[m];
						for (int num8 = wEPoint4.Y; num8 >= y; num8--)
						{
							ITile val4 = Main.tile[(int)wEPoint4.X, num8];
							if (Tools.CanSet(Tile: true, Main.tile[(int)wEPoint4.X, num8], materialType, select, expression, magicWand, wEPoint4.X, num8, plr))
							{
								SetTile(wEPoint4.X, num8, materialType);
								num++;
							}
						}
					}
					break;
				}
				case 2:
				{
					if (wall)
					{
						WEPoint[] array10 = array;
						for (int num12 = 0; num12 < array10.Length; num12++)
						{
							WEPoint wEPoint7 = array10[num12];
							for (int num13 = wEPoint7.X; num13 <= x2; num13++)
							{
								ITile val7 = Main.tile[num13, (int)wEPoint7.Y];
								if (Tools.CanSet(Tile: false, Main.tile[num13, (int)wEPoint7.Y], materialType, select, expression, magicWand, num13, wEPoint7.Y, plr))
								{
									Main.tile[num13, (int)wEPoint7.Y].wall = (ushort)materialType;
									num++;
								}
							}
						}
						break;
					}
					WEPoint[] array11 = array;
					for (int num14 = 0; num14 < array11.Length; num14++)
					{
						WEPoint wEPoint8 = array11[num14];
						for (int num15 = wEPoint8.X; num15 <= x2; num15++)
						{
							ITile val8 = Main.tile[num15, (int)wEPoint8.Y];
							if (Tools.CanSet(Tile: true, Main.tile[num15, (int)wEPoint8.Y], materialType, select, expression, magicWand, num15, wEPoint8.Y, plr))
							{
								SetTile(num15, wEPoint8.Y, materialType);
								num++;
							}
						}
					}
					break;
				}
				case 3:
				{
					if (wall)
					{
						WEPoint[] array4 = array;
						for (int i = 0; i < array4.Length; i++)
						{
							WEPoint wEPoint = array4[i];
							for (int num6 = wEPoint.X; num6 >= x; num6--)
							{
								ITile val = Main.tile[num6, (int)wEPoint.Y];
								if (Tools.CanSet(Tile: false, Main.tile[num6, (int)wEPoint.Y], materialType, select, expression, magicWand, num6, wEPoint.Y, plr))
								{
									Main.tile[num6, (int)wEPoint.Y].wall = (ushort)materialType;
									num++;
								}
							}
						}
						break;
					}
					WEPoint[] array5 = array;
					for (int j = 0; j < array5.Length; j++)
					{
						WEPoint wEPoint2 = array5[j];
						for (int k = wEPoint2.X; k >= x; k++)
						{
							ITile val2 = Main.tile[k, (int)wEPoint2.Y];
							if (Tools.CanSet(Tile: true, Main.tile[k, (int)wEPoint2.Y], materialType, select, expression, magicWand, k, wEPoint2.Y, plr))
							{
								SetTile(k, wEPoint2.Y, materialType);
								num++;
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
				WEPoint[] array12 = array;
				for (int num16 = 0; num16 < array12.Length; num16++)
				{
					WEPoint wEPoint9 = array12[num16];
					ITile val9 = Main.tile[(int)wEPoint9.X, (int)wEPoint9.Y];
					if (Tools.CanSet(Tile: false, Main.tile[(int)wEPoint9.X, (int)wEPoint9.Y], materialType, select, expression, magicWand, wEPoint9.X, wEPoint9.Y, plr))
					{
						Main.tile[(int)wEPoint9.X, (int)wEPoint9.Y].wall = (ushort)materialType;
						num++;
					}
				}
				for (int num17 = array2[0].X; num17 <= array2[1].X; num17++)
				{
					for (int num18 = array2[0].Y; num18 <= array2[1].Y; num18++)
					{
						ITile val10 = Main.tile[num17, num18];
						if (Tools.CanSet(Tile: true, Main.tile[num17, num18], materialType, select, expression, magicWand, num17, num18, plr))
						{
							Main.tile[num17, num18].wall = (ushort)materialType;
							num++;
						}
					}
				}
				if (array3 == null)
				{
					break;
				}
				for (int num19 = array3[0].X; num19 <= array3[1].X; num19++)
				{
					for (int num20 = array3[0].Y; num20 <= array3[1].Y; num20++)
					{
						ITile val11 = Main.tile[num19, num20];
						if (Tools.CanSet(Tile: true, Main.tile[num19, num20], materialType, select, expression, magicWand, num19, num20, plr))
						{
							Main.tile[num19, num20].wall = (ushort)materialType;
							num++;
						}
					}
				}
				break;
			}
			WEPoint[] array13 = array;
			for (int num21 = 0; num21 < array13.Length; num21++)
			{
				WEPoint wEPoint10 = array13[num21];
				ITile val12 = Main.tile[(int)wEPoint10.X, (int)wEPoint10.Y];
				if (Tools.CanSet(Tile: true, Main.tile[(int)wEPoint10.X, (int)wEPoint10.Y], materialType, select, expression, magicWand, wEPoint10.X, wEPoint10.Y, plr))
				{
					SetTile(wEPoint10.X, wEPoint10.Y, materialType);
					num++;
				}
			}
			for (int num22 = array2[0].X; num22 <= array2[1].X; num22++)
			{
				for (int num23 = array2[0].Y; num23 <= array2[1].Y; num23++)
				{
					ITile val13 = Main.tile[num22, num23];
					if (Tools.CanSet(Tile: true, Main.tile[num22, num23], materialType, select, expression, magicWand, num22, num23, plr))
					{
						SetTile(num22, num23, materialType);
						num++;
					}
				}
			}
			if (array3 == null)
			{
				break;
			}
			for (int num24 = array3[0].X; num24 <= array3[1].X; num24++)
			{
				for (int num25 = array3[0].Y; num25 <= array3[1].Y; num25++)
				{
					ITile val14 = Main.tile[num24, num25];
					if (Tools.CanSet(Tile: true, Main.tile[num24, num25], materialType, select, expression, magicWand, num24, num25, plr))
					{
						SetTile(num24, num25, materialType);
						num++;
					}
				}
			}
			break;
		}
		}
		ResetSection();
		plr.SendSuccessMessage("Set {0}{1} shape. ({2})", filled ? "filled " : "", wall ? "wall" : "tile", num);
	}
}
