using GhostView.Constants;
using TShockAPI;

namespace GhostView.Strategies;

public class RespawnTimeSelector
{
    private readonly double _normalSeconds = TShock.Config.Settings.RespawnSeconds > 0
        ? TShock.Config.Settings.RespawnSeconds
        : RespawnSettings.DefaultRespawnSeconds;

    private readonly double _bossSeconds = TShock.Config.Settings.RespawnBossSeconds > 0
        ? TShock.Config.Settings.RespawnBossSeconds
        : RespawnSettings.DefaultRespawnBossSeconds;

    public double GetRespawnSeconds(bool isBossAlive)
    {
        return isBossAlive ? this._bossSeconds : this._normalSeconds;
    }
}