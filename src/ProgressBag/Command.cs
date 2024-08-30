using TShockAPI;


namespace ProgressBag;

public static class ExecCommand
{
    public static bool HandleCommand(this TSPlayer player, string text)
    {
        player.tempGroup = new SuperAdminGroup();
        var code = Commands.HandleCommand(player, text);
        player.tempGroup = null;
        return code;
    }
}