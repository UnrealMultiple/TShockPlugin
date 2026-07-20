using Terraria;
using TShockAPI;
using WorldEdit.Expressions;

namespace WorldEdit.Commands;

public class SetWire : WECommand
{
	private readonly Expression _expression;

	private readonly bool _state;

	private readonly int _wire;

	public SetWire(int x, int y, int x2, int y2, MagicWand magicWand, TSPlayer plr, int wire, bool state, Expression expression)
		: base(x, y, x2, y2, magicWand, plr)
	{
		_expression = expression ?? new TestExpression(_ => true);
		_state = state;
		_wire = wire;
	}

	public override void Execute()
	{
		if (!CanUseCommand())
		{
			return;
		}
		Tools.PrepareUndo(x, y, x2, y2, plr);
		int changedWireCount = 0;
		switch (_wire)
		{
		case 1:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.wire() != _state && select(tileX, tileY, plr) && _expression.Evaluate(tile) && magicWand.InSelection(tileX, tileY))
					{
						tile.wire(_state);
						changedWireCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire. ({0})", changedWireCount);
			break;
		}
		case 2:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.wire2() != _state && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						tile.wire2(_state);
						changedWireCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 2. ({0})", changedWireCount);
			break;
		}
		case 3:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.wire3() != _state && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						tile.wire3(_state);
						changedWireCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 3. ({0})", changedWireCount);
			break;
		}
		case 4:
		{
			for (int tileX = x; tileX <= x2; tileX++)
			{
				for (int tileY = y; tileY <= y2; tileY++)
				{
					ITile tile = Main.tile[tileX, tileY];
					if (tile.wire4() != _state && select(tileX, tileY, plr) && _expression.Evaluate(tile))
					{
						tile.wire4(_state);
						changedWireCount++;
					}
				}
			}
			ResetSection();
			plr.SendSuccessMessage("Set wire 4. ({0})", changedWireCount);
			break;
		}
		}
	}
}
