using System.Text;
using TShockAPI;

namespace StatusTextManager.Utils;

internal static class Common
{
    public static ulong TickCount;
    public static IEnumerable<TSPlayer> PlayersOnline => from p in TShock.Players where p is { Active: true } select p;

    public static StringBuilder AcquirePlayerStringBuilder(this StringBuilder?[] sbs, TSPlayer player)
    {
        var id = player.Index;
        return (
            sbs[id] ??
            (sbs[id] = new StringBuilder())
        ).Clear();
    }

    public static void CountTick()
    {
        TickCount++;
    }

    public static void ClearTickCount()
    {
        TickCount = 0;
    }
}