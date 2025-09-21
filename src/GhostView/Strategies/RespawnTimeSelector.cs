using TShockAPI;

namespace GhostView.Strategies;

public class RespawnTimeSelector
{
    private readonly int _normalSeconds = TShock.Config.Settings.RespawnSeconds;

    private readonly int _bossSeconds = TShock.Config.Settings.RespawnBossSeconds;

    public int GetRespawnSeconds(bool isBossAlive)
    {
        return isBossAlive ? this._bossSeconds : this._normalSeconds;
    }
}