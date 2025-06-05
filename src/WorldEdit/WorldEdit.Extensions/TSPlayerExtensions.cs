using TShockAPI;

namespace WorldEdit.Extensions;

public static class TSPlayerExtensions
{
	public static PlayerInfo GetPlayerInfo(this TSPlayer tsplayer)
	{
		if (!tsplayer.ContainsData("WorldEdit_Data"))
		{
			tsplayer.SetData("WorldEdit_Data", new PlayerInfo());
		}
		return tsplayer.GetData<PlayerInfo>("WorldEdit_Data");
	}
}
