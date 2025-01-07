using LazyAPI;
using System.Net;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Dummmy;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Name => "Dummmy";

    public override Version Version => new Version(1, 0, 0, 0);

    public override string Author => "少司命";

    public override string Description => "在你的服务器中放置假人！";

    internal static readonly DummmyPlayer[] _players = new DummmyPlayer[Main.maxNetPlayers];

    internal static int Port = 7777;

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        On.Terraria.Netplay.OpenPort += this.Netplay_OpenPort;
        Commands.ChatCommands.Add(new("dummy.client.use", CommandAdapter.Adapter, "dummmy"));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            On.Terraria.Netplay.OpenPort -= this.Netplay_OpenPort;
            Commands.ChatCommands.RemoveAll(c => c.CommandDelegate == CommandAdapter.Adapter);
        }
        base.Dispose(disposing);
    }

    private void Netplay_OpenPort(On.Terraria.Netplay.orig_OpenPort orig, int port)
    {
        foreach (var dummmy in Config.Instance.Dummmys)
        {
            var ply = new DummmyPlayer(dummmy);
            ply.GameLoop("127.0.0.1", port, TShock.Config.Settings.ServerPassword);
            _players[ply.PlayerSlot] = ply;
        }
        Port = port;
        orig(port);
    }

    private void OnLeave(LeaveEventArgs args)
    {
        var ply = _players[args.Who];
        ply?.Close();
        
    }
}
