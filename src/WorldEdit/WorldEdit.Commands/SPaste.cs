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
		int clipboardMaxXOffset = worldSectionData.Width - 1;
		int clipboardMaxYOffset = worldSectionData.Height - 1;
		if ((alignment & 1) == 0)
		{
			x2 = x + clipboardMaxXOffset;
		}
		else
		{
			x2 = x;
			x -= clipboardMaxXOffset;
		}
		if ((alignment & 2) == 0)
		{
			y2 = y + clipboardMaxYOffset;
		}
		else
		{
			y2 = y;
			y -= clipboardMaxYOffset;
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
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				int clipboardX = tileX - x;
				int clipboardY = tileY - y;
				if (tileX < 0 || tileY < 0 || tileX >= Main.maxTilesX || tileY >= Main.maxTilesY || (expression != null && !expression.Evaluate(worldSectionData.Tiles[clipboardX, clipboardY])))
				{
					continue;
				}
				ITile pastedTile = (ITile)Main.tile[tileX, tileY].Clone();
				if (tiles)
				{
					pastedTile = worldSectionData.Tiles[clipboardX, clipboardY];
				}
				else
				{
					pastedTile.wall = worldSectionData.Tiles[clipboardX, clipboardY].wall;
					pastedTile.wallColor(worldSectionData.Tiles[clipboardX, clipboardY].wallColor());
					pastedTile.liquid = worldSectionData.Tiles[clipboardX, clipboardY].liquid;
					pastedTile.liquidType((int)worldSectionData.Tiles[clipboardX, clipboardY].liquidType());
					pastedTile.wire(worldSectionData.Tiles[clipboardX, clipboardY].wire());
					pastedTile.wire2(worldSectionData.Tiles[clipboardX, clipboardY].wire2());
					pastedTile.wire3(worldSectionData.Tiles[clipboardX, clipboardY].wire3());
					pastedTile.wire4(worldSectionData.Tiles[clipboardX, clipboardY].wire4());
					pastedTile.actuator(worldSectionData.Tiles[clipboardX, clipboardY].actuator());
					pastedTile.inActive(worldSectionData.Tiles[clipboardX, clipboardY].inActive());
				}
				if (emptyTiles || pastedTile.active() || pastedTile.wall != 0 || pastedTile.liquid != 0 || pastedTile.wire() || pastedTile.wire2() || pastedTile.wire3() || pastedTile.wire4())
				{
					if (!tilePaints)
					{
						pastedTile.color(Main.tile[tileX, tileY].color());
					}
					if (!walls)
					{
						pastedTile.wall = Main.tile[tileX, tileY].wall;
						pastedTile.wallColor(Main.tile[tileX, tileY].wallColor());
					}
					if (!wallPaints)
					{
						pastedTile.wallColor(Main.tile[tileX, tileY].wallColor());
					}
					if (!liquids)
					{
						pastedTile.liquid = Main.tile[tileX, tileY].liquid;
						pastedTile.liquidType((int)Main.tile[tileX, tileY].liquidType());
					}
					if (!wires)
					{
						pastedTile.wire(Main.tile[tileX, tileY].wire());
						pastedTile.wire2(Main.tile[tileX, tileY].wire2());
						pastedTile.wire3(Main.tile[tileX, tileY].wire3());
						pastedTile.wire4(Main.tile[tileX, tileY].wire4());
						pastedTile.actuator(Main.tile[tileX, tileY].actuator());
						pastedTile.inActive(Main.tile[tileX, tileY].inActive());
					}
					Main.tile[tileX, tileY] = pastedTile;
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, x, y, Tiles: false);
		ResetSection();
		plr.SendSuccessMessage("Pasted clipboard to selection.");
	}
}
