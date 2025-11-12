using TShockAPI;

namespace GhostView.Strategies;

public class RespawnTimeSelector
{
    private static int NormalSeconds => TShock.Config.Settings.RespawnSeconds;

    private static int BossSeconds => TShock.Config.Settings.RespawnBossSeconds;

    public static int GetRespawnSeconds(bool isBossAlive)
    {
        return isBossAlive ? BossSeconds : NormalSeconds;
    }
}