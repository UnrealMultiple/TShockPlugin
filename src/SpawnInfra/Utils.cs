using TShockAPI;

namespace SpawnInfra;

internal class Utils
{
    public static int GetUnixTimestamp => (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    public static bool NeedInGame(TSPlayer plr)
    {
        if (!plr.RealPlayer)
        {
            plr.SendErrorMessage(GetString("请进入游戏后再操作！"));
        }
        return !plr.RealPlayer;
    }
}