using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace RandReSpawn;

[ApiVersion(2, 1)]
public class MainPlugin : TerrariaPlugin
{
    private Random rand = new();
    public override string Name => "RandRespawn";
    public override string Author => "1413,肝帝熙恩适配1449";
    public override string Description => "随机出生点，任何回到出生点的操作都会被随机传送";
    public override Version Version => new Version(1, 0, 2);

    private readonly List<TSPlayer> RandSpawns = new List<TSPlayer>();
    public MainPlugin(Main game) : base(game)
    {

    }
    public override void Initialize()
    {
        this.rand = new Random();
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
        GetDataHandlers.PlayerSpawn.Register(this.OnSpawn, HandlerPriority.Lowest);
    }

    private void OnUpdate(EventArgs args)
    {
        for (var i = this.RandSpawns.Count - 1; i >= 0; i--)
        {
            var ply = this.RandSpawns[i];
            var (x, y) = this.Search();
            ply.Teleport(x * 16, y * 16);
            this.RandSpawns.RemoveAt(i);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.PlayerSpawn -= this.OnSpawn;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
        }
        base.Dispose(disposing);
    }

    private void OnSpawn(object? sender, GetDataHandlers.SpawnEventArgs args)
    {
        this.RandSpawns.Add(args.Player);
    }

    private (int x, int y) Search()
    {
        int x, y;
        do
        {
            x = this.rand.Next(0, Main.maxTilesX - 2);
            y = this.rand.Next(0, Main.maxTilesY - 2);
        }
        while (!Avaiable(x, y));
        return (x, y);
    }
    private static bool Avaiable(int x, int y)
    {
        return
        !Main.tile[x + 0, y + 0].active() && !Main.tile[x + 1, y + 0].active() &&
        !Main.tile[x + 0, y + 1].active() && !Main.tile[x + 1, y + 1].active() &&
        !Main.tile[x + 0, y + 2].active() && !Main.tile[x + 1, y + 2].active() &&
        Main.tile[x + 0, y + 0].liquid == 0 && Main.tile[x + 1, y + 0].liquid == 0 &&
        Main.tile[x + 0, y + 1].liquid == 0 && Main.tile[x + 1, y + 1].liquid == 0 &&
        Main.tile[x + 0, y + 2].liquid == 0 && Main.tile[x + 1, y + 2].liquid == 0 &&
        Main.tile[x + 0, y + 3].active() && Main.tile[x + 1, y + 3].active();
    }
}