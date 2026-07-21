using System;
using Terraria;
using TShockAPI;
using WorldEdit.Expressions;
using WorldEdit.Extensions;

namespace WorldEdit.Commands;

public class Move : WECommand
{
	private readonly int _down;

	private readonly int _right;

	private readonly Expression _expression;

	public Move(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int down, int right, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_down = down;
		_right = right;
		_expression = expression ?? new TestExpression(_ => true);
	}

	public override void Execute()
	{
		int destinationX = x + _right;
		int destinationY = y + _down;
		if (destinationX < 0)
		{
			destinationX = 0;
		}
		if (destinationY < 0)
		{
			destinationY = 0;
		}
		if (destinationX >= Main.maxTilesX - Math.Abs(x - x2))
		{
			destinationX = Main.maxTilesX - Math.Abs(x - x2) - 1;
		}
		if (destinationY >= Main.maxTilesY - Math.Abs(y - y2))
		{
			destinationY = Main.maxTilesY - Math.Abs(y - y2) - 1;
		}
		int destinationX2 = destinationX + Math.Abs(x - x2);
		int destinationY2 = destinationY + Math.Abs(y - y2);
		int affectedX = Math.Min(x, Math.Min(destinationX, destinationX2));
		int affectedY = Math.Min(y, Math.Min(destinationY, destinationY2));
		int affectedX2 = Math.Max(x2, Math.Max(destinationX, destinationX2));
		int affectedY2 = Math.Max(y2, Math.Max(destinationY, destinationY2));
		if (!CanUseCommand(affectedX, affectedY, affectedX2, affectedY2))
		{
			return;
		}
		Tools.PrepareUndo(affectedX, affectedY, affectedX2, affectedY2, plr);
		WorldSectionData worldSectionData = Tools.SaveWorldSection(x, y, x2, y2);
		int movedTileCount = 0;
		for (int tileX = x; tileX <= x2; tileX++)
		{
			for (int tileY = y; tileY <= y2; tileY++)
			{
				if (magicWand.InSelection(tileX, tileY) && _expression.Evaluate(Main.tile[tileX, tileY]))
				{
					Main.tile[tileX, tileY] = new Tile();
					movedTileCount++;
				}
			}
		}
		for (int tileX = destinationX; tileX <= destinationX2; tileX++)
		{
			for (int tileY = destinationY; tileY <= destinationY2; tileY++)
			{
				int sourceX = tileX - destinationX;
				int sourceY = tileY - destinationY;
				if (tileX >= 0 && tileY >= 0 && tileX < Main.maxTilesX && tileY < Main.maxTilesY && magicWand.InSelection(tileX - _right, tileY - _down) && _expression.Evaluate(worldSectionData.Tiles[sourceX, sourceY]))
				{
					Main.tile[tileX, tileY] = worldSectionData.Tiles[sourceX, sourceY];
				}
			}
		}
		Tools.LoadWorldSection(worldSectionData, destinationX, destinationY, Tiles: false);
		ResetSection();
		PlayerInfo playerInfo = plr.GetPlayerInfo();
		playerInfo.X = destinationX;
		playerInfo.Y = destinationY;
		playerInfo.X2 = destinationX2;
		playerInfo.Y2 = destinationY2;
		plr.SendInfoMessage("Moved tiles ({0}).", movedTileCount);
	}
}
