using TShockAPI;

namespace WorldEdit.Commands;

public class Redo : WECommand
{
	private int accountID;

	private int steps;

	public Redo(TSPlayer plr, int accountID, int steps)
		: base(0, 0, 0, 0, plr)
	{
		this.accountID = accountID;
		this.steps = steps;
	}

	public override void Execute()
	{
		if (WorldEdit.Config.DisableUndoSystemForUnrealPlayers && (!plr.RealPlayer || accountID == 0))
		{
			plr.SendErrorMessage("Undo system is disabled for unreal players.");
			return;
		}
		int num = -1;
		while (++num < steps && Tools.Redo(accountID))
		{
		}
		if (num == 0)
		{
			plr.SendErrorMessage("Failed to redo any actions.");
			return;
		}
		plr.SendSuccessMessage("Redid {0}'s last {1}action{2}.", (accountID == 0) ? "ServerConsole" : TShock.UserAccounts.GetUserAccountByID(accountID).Name, (num == 1) ? "" : (num + " "), (num == 1) ? "" : "s");
	}
}
