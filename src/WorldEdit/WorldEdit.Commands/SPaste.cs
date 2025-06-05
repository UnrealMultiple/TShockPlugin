using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SPaste : WECommand
{
	private readonly int alignment;

	private readonly Expression expression;

	private readonly bool tiles;

	private readonly bool tilePaints;

	private readonly bool emptyTiles;

	private readonly bool walls;

	private readonly bool wallPaints;

	private readonly bool wires;

	private readonly bool liquids;

	public SPaste(int x, int y, TSPlayer plr, int alignment, Expression expression, bool tiles, bool tilePaints, bool emptyTiles, bool walls, bool wallPaints, bool wires, bool liquids)
		: base(x, y, int.MaxValue, int.MaxValue, plr)
	{
		this.alignment = alignment;
		this.expression = expression;
		this.tiles = tiles;
		this.tilePaints = tilePaints;
		this.emptyTiles = emptyTiles;
		this.walls = walls;
		this.wallPaints = wallPaints;
		this.wires = wires;
		this.liquids = liquids;
	}

	public override void Execute()
	{
						string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		WorldSectionData worldSectionData = Tools.LoadWorldData(clipboardPath);
		int num = worldSectionData.Width - 1;
		int num2 = worldSectionData.Height - 1;
		if ((alignment & 1) == 0)
		{
			x2 = x + num;
		}
		else
		{
			x2 = x;
			x -= num;
		}
		if ((alignment & 2) == 0)
		{
			y2 = y + num2;
		}
		else
		{
			y2 = y;
			y -= num2;
		}
		if (!CanUseCommand())
		{
			return;
		}
		if (x < 0)
		{
			x = 0;
		}
		if (x2 < 0)
		{
			x2 = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		if (y2 < 0)
		{
			y2 = 0;
		}
		if (x >= Main.maxTilesX)
		{
			x = Main.maxTilesX - 1;
		}
		if (x2 >= Main.maxTilesX)
		{
			x2 = Main.maxTilesX - 1;
		}
		if (y >= Main.maxTilesY)
		{
			y = Main.maxTilesY - 1;
		}
		if (y2 >= Main.maxTilesY)
		{
			y2 = Main.maxTilesY - 1;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				int num3 = i - x;
				int num4 = j - y;
				if (i < 0 || j < 0 || i >= Main.maxTilesX || j >= Main.maxTilesY || (expression != null && !expression.Evaluate(worldSectionData.Tiles[num3, num4])))
				{
					continue;
				}
				ITile val = (ITile)Main.tile[i, j].Clone();
				if (tiles)
				{
					val = worldSectionData.Tiles[num3, num4];
				}
				else
				{
					val.wall = worldSectionData.Tiles[num3, num4].wall;
					val.wallColor(worldSectionData.Tiles[num3, num4].wallColor());
					val.liquid = worldSectionData.Tiles[num3, num4].liquid;
					val.liquidType((int)worldSectionData.Tiles[num3, num4].liquidType());
					val.wire(worldSectionData.Tiles[num3, num4].wire());
					val.wire2(worldSectionData.Tiles[num3, num4].wire2());
					val.wire3(worldSectionData.Tiles[num3, num4].wire3());
					val.wire4(worldSectionData.Tiles[num3, num4].wire4());
					val.actuator(worldSectionData.Tiles[num3, num4].actuator());
					val.inActive(worldSectionData.Tiles[num3, num4].inActive());
				}
				if (emptyTiles || val.active() || val.wall != 0 || val.liquid != 0 || val.wire() || val.wire2() || val.wire3() || val.wire4())
				{
					if (!tilePaints)
					{
						val.color(Main.tile[i, j].color());
					}
					if (!walls)
					{
						val.wall = Main.tile[i, j].wall;
						val.wallColor(Main.tile[i, j].wallColor());
					}
					if (!wallPaints)
					{
						val.wallColor(Main.tile[i, j].wallColor());
					}
					if (!liquids)
					{
						val.liquid = Main.tile[i, j].liquid;
						val.liquidType((int)Main.tile[i, j].liquidType());
					}
					if (!wires)
					{
						val.wire(Main.tile[i, j].wire());
						val.wire2(Main.tile[i, j].wire2());
						val.wire3(Main.tile[i, j].wire3());
						val.wire4(Main.tile[i, j].wire4());
						val.actuator(Main.tile[i, j].actuator());
						val.inActive(Main.tile[i, j].inActive());
					}
					Main.tile[i, j] = val;
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, x, y, Tiles: false);
		ResetSection();
		plr.SendSuccessMessage("Pasted clipboard to selection.");
	}
}
