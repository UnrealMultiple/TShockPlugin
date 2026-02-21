using TShockAPI;

namespace GhostView.Utils;

public static class RespawnConfigFixer
{
    private const int DefaultRespawnSeconds = 5;
    private const int DefaultRespawnBossSeconds = 20;
    public static void EnsureRespawnSettings()
    {
        var settings = TShock.Config.Settings;
        if (settings.RespawnSeconds <= 0)
        {
            settings.RespawnSeconds = DefaultRespawnSeconds;
        }
        if (settings.RespawnBossSeconds <= 0)
        {
            settings.RespawnBossSeconds = DefaultRespawnBossSeconds;
        }
        
    }
}