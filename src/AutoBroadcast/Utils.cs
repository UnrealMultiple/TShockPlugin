using TShockAPI;

namespace AutoBroadcast;

public static class Utils
{
    public static void BroadcastToGroups(string[] groups, string[] messages, byte[] colour)
    {
        foreach (var line in messages)
        {
            if (line.StartsWith(TShock.Config.Settings.CommandSpecifier) || line.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, line);
            }
            else
            {
                foreach (var player in TShock.Players)
                {
                    if (player != null && groups.Contains(player.Group.Name))
                    {
                        player.SendMessage(line, colour[0], colour[1], colour[2]);
                    }
                }
            }
        }
    }
    public static void BroadcastToAll(string[] messages, byte[] colour)
    {
        foreach (var line in messages)
        {
            if (line.StartsWith(TShock.Config.Settings.CommandSpecifier) || line.StartsWith(TShock.Config.Settings.CommandSilentSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, line);
            }
            else
            {
                TSPlayer.All.SendMessage(line, colour[0], colour[1], colour[2]);
            }
        }
    }
    public static void BroadcastToPlayer(TSPlayer plr, string[] messages, byte[] colour)
    {
        foreach (var line in messages)
        {
            if (line.StartsWith(TShock.Config.Settings.CommandSpecifier))
            {
                Commands.HandleCommand(TSPlayer.Server, line);
            }
            else
            {
                plr.SendMessage(line, colour[0], colour[1], colour[2]);
            }
        }
    }
}