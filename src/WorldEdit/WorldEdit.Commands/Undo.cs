using TShockAPI;

namespace WorldEdit.Commands;

public class Undo : WECommand
{
	private int accountID;

	private int steps;

	public Undo(TSPlayer plr, int accountID, int steps)
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
		int completedStepCount = -1;
		while (++completedStepCount < steps && Tools.Undo(accountID))
		{
		}
		if (completedStepCount == 0)
		{
			plr.SendErrorMessage("Failed to undo any actions.");
			return;
		}
		plr.SendSuccessMessage("Undid {0}'s last {1}action{2}.", (accountID == 0) ? "ServerConsole" : TShock.UserAccounts.GetUserAccountByID(accountID).Name, (completedStepCount == 1) ? "" : (completedStepCount + " "), (completedStepCount == 1) ? "" : "s");
	}
}
