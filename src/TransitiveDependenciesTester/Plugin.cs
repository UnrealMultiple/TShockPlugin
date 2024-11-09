using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace TransitiveDependenciesTester;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override Version Version => new Version(7, 0);

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        TShockAPI.Commands.ChatCommands.Add(new Command("test_version", this.PrintVersion, "test_version"));
    }

    private void PrintVersion(CommandArgs args)
    {
        args.Player.SendInfoMessage(this.Version.ToString());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.PrintVersion);
        }
        base.Dispose(disposing);
    }
}