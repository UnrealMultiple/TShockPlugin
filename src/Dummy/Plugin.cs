using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TrProtocol.Packets;
using TShockAPI;

namespace Dummy;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 0, 3);

    public override string Author => "少司命";

    public override string Description => GetString("在你的服务器中放置假人！");

    internal static readonly DummyPlayer[] _players = new DummyPlayer[Main.maxNetPlayers];

    internal static int Port = 7777;

    public Plugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        ServerApi.Hooks.ServerLeave.Register(this, this.OnLeave);
        On.Terraria.Netplay.OpenPort += this.Netplay_OpenPort;
        this.addCommands.Add(new("dummy.client.use", CommandAdapter.Adapter, "dummy"));
        base.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnLeave);
            On.Terraria.Netplay.OpenPort -= this.Netplay_OpenPort;
        }
        base.Dispose(disposing);
    }

    private void Netplay_OpenPort(On.Terraria.Netplay.orig_OpenPort orig, int port)
    {
        orig(port);
        foreach (var dummy in Config.Instance.Dummys)
        {
            var ply = new DummyPlayer(new()
            {
                Hair = dummy.Hair,
                HairColor = dummy.HairColor,
                EyeColor = dummy.EyeColor,
                ShirtColor = dummy.ShirtColor,
                ShoeColor = dummy.ShoeColor,
                SkinColor = dummy.SkinColor,
                HairDye = dummy.HairDye,
                Name = dummy.Name,
                SkinVariant = dummy.SkinVariant,
                UnderShirtColor = dummy.UnderShirtColor,
                HideMisc = dummy.HideMisc,
            }, dummy.UUID);
            ply.GameLoop("127.0.0.1", port, TShock.Config.Settings.ServerPassword);
            if (!string.IsNullOrEmpty(dummy.Password))
            {
                ply.ChatText($"/login {dummy.Password}");
            }
            ply.On<LoadPlayer>(p => _players[p.PlayerSlot] = ply);
        }
        Port = port;

    }

    private void OnLeave(LeaveEventArgs args)
    {
        var ply = _players[args.Who];
        ply?.Close();

    }
}
