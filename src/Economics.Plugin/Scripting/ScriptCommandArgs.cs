using TShockAPI;

namespace Economics.Plugin.Scripting;

public sealed class ScriptCommandArgs(CommandArgs args)
{
    public string PlayerName { get; } = args.Player?.Name ?? "Console";
    public int PlayerIndex { get; } = args.Player?.Index ?? -1;
    public bool RealPlayer { get; } = args.TPlayer != null;
    public bool IsLoggedIn { get; } = args.Player?.IsLoggedIn ?? false;
    public List<string> Parameters { get; } = args.Parameters;
    public string Message { get; } = args.Message;
}
