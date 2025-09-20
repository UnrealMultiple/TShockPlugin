using TShockAPI;

namespace GhostView.Strategies;

public class RespawnTimeSelector
{
    private readonly double _normalSeconds = TShock.Config.Settings.RespawnSeconds;

    private readonly double _bossSeconds = TShock.Config.Settings.RespawnBossSeconds;

    public double GetRespawnSeconds(bool isBossAlive)
    {
        return isBossAlive ? this._bossSeconds : this._normalSeconds;
    }
}