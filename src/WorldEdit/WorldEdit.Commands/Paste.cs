using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Paste : WECommand
{
	private readonly int alignment;

	private readonly Expression expression;

	private readonly bool mode_MainBlocks;

	private readonly string path;

	private readonly bool prepareUndo;

	public Paste(int x, int y, TSPlayer plr, string path, int alignment, Expression expression, bool mode_MainBlocks, bool prepareUndo)
		: base(x, y, int.MaxValue, int.MaxValue, plr)
	{
		this.alignment = alignment;
		this.expression = expression;
		this.mode_MainBlocks = mode_MainBlocks;
		this.path = path;
		this.prepareUndo = prepareUndo;
	}

	public override void Execute()
	{
		WorldSectionData worldSectionData = Tools.LoadWorldData(path);
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
		if (!CanUseCommand())
		{
			return;
		}
		if (prepareUndo)
		{
			Tools.PrepareUndo(x, y, x2, y2, plr);
		}
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				int clipboardX = tileX - x;
				int clipboardY = tileY - y;
				if (tileX >= 0 && tileY >= 0 && tileX < Main.maxTilesX && tileY < Main.maxTilesY && (expression == null || expression.Evaluate(mode_MainBlocks ? Main.tile[tileX, tileY] : worldSectionData.Tiles[clipboardX, clipboardY])))
				{
					Main.tile[tileX, tileY] = worldSectionData.Tiles[clipboardX, clipboardY];
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, x, y, Tiles: false);
		ResetSection();
		plr.SendSuccessMessage("Pasted clipboard to selection.");
	}
}
