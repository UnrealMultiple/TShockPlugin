using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class Inactive : WECommand
{
	private readonly Expression _expression;

	private readonly int _inactiveType;

	public Inactive(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int inacType, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_inactiveType = inacType;
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
		switch (_inactiveType)
		{
		case 0:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.active() && !tile.inActive() && select(tileX, tileY, plr) && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
					{
						tile.inActive(true);
						changedTileCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Made tiles inactive. ({0})", changedTileCount);
			break;
		}
		case 1:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.inActive() && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						tile.inActive(false);
						changedTileCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set\ttiles' inactive\tstatus off.\t({0})", changedTileCount);
			break;
		}
		case 2:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.active() && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						tile.inActive(!tile.inActive());
						changedTileCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Reversed tiles'\tinactive status. ({0})", changedTileCount);
			break;
		}
		}
	}
}
