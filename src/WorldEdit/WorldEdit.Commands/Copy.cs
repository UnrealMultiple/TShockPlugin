using TShockAPI;

namespace WorldEdit.Commands;

public class Copy : WECommand
{
	private string save;

	public Copy(int x, int y, int x2, int y2, TSPlayer plr, string save)
		: base(x, y, x2, y2, plr)
	{
		this.save = save;
	}

	public override void Execute()
	{
		string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		Tools.SaveWorldSection(x, y, x2, y2, save ?? clipboardPath);
		plr.SendSuccessMessage("Copied selection to {0}.", (save == null) ? "clipboard" : "schematic");
	}
}
