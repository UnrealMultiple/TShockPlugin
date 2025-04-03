using TShockAPI;

namespace Platform;

public static class PlatformTool
{
    public static string GetPlatform(this TSPlayer plr)
    {
        return Platform.Platforms[plr.Index].ToString();
    }

}