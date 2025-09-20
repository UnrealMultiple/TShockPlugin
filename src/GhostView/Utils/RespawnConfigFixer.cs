using TShockAPI;

namespace GhostView.Utils;

public static class RespawnConfigFixer
{
    private const int DefaultRespawnSeconds = 5;
    private const int DefaultRespawnBossSeconds = 20;
    private static readonly string SavePath = Path.Combine(TShock.SavePath, "config.json");
    public static void EnsureRespawnSettings()
    {
        try
        {
            if (TShock.Config?.Settings == null)
            {
                return;
            }

            var settings = TShock.Config.Settings;

            if (settings.RespawnSeconds <= 0)
            {
                settings.RespawnSeconds = DefaultRespawnSeconds;
            }

            if (settings.RespawnBossSeconds <= 0)
            {
                settings.RespawnBossSeconds = DefaultRespawnBossSeconds;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SavePath) ?? string.Empty);
            TShock.Config.Write(SavePath);
        }
        catch (Exception ex)
        {
            TShock.Log.ConsoleError(ex.Message);
        }
    }
}