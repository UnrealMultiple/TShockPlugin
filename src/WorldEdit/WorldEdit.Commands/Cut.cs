using System.IO;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace WorldEdit.Commands;

public class Cut : WECommand
{
	public Cut(int x, int y, int x2, int y2, TSPlayer plr)
		: base(x, y, x2, y2, plr)
	{
	}

	public override void Execute()
	{
						foreach (string item in Directory.EnumerateFiles("worldedit", $"redo-{Main.worldID}-{plr.Account.ID}-*.dat"))
		{
			File.Delete(item);
		}
		if (WorldEdit.Database.GetSqlType() == SqlType.Mysql)
		{
			WorldEdit.Database.Query("INSERT IGNORE INTO WorldEdit VALUES (@0, -1, -1)", plr.Account.ID);
		}
		else
		{
			WorldEdit.Database.Query("INSERT OR IGNORE INTO WorldEdit VALUES (@0, 0, 0)", plr.Account.ID);
		}
		WorldEdit.Database.Query("UPDATE WorldEdit SET RedoLevel = -1 WHERE Account = @0", plr.Account.ID);
		WorldEdit.Database.Query("UPDATE WorldEdit SET UndoLevel = UndoLevel + 1 WHERE Account = @0", plr.Account.ID);
		int num = 0;
		using (QueryResult queryResult = WorldEdit.Database.QueryReader("SELECT UndoLevel FROM WorldEdit WHERE Account = @0", plr.Account.ID))
		{
			if (queryResult.Read())
			{
				num = queryResult.Get<int>("UndoLevel");
			}
		}
		string clipboardPath = Tools.GetClipboardPath(plr.Account.ID);
		string text = Path.Combine("worldedit", $"undo-{Main.worldID}-{plr.Account.ID}-{num}.dat");
		Tools.SaveWorldSection(x, y, x2, y2, text);
		Tools.ClearObjects(x, y, x2, y2);
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				Main.tile[i, j] = (ITile)new Tile();
			}
		}
		if (File.Exists(clipboardPath))
		{
			File.Delete(clipboardPath);
		}
		File.Copy(text, clipboardPath);
		ResetSection();
		plr.SendSuccessMessage("Cut selection. ({0})", (x2 - x + 1) * (y2 - y + 1));
	}
}
