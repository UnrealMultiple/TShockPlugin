using BedSet;
using LazyAPI;
using LazyAPI.Utility;
using TerrariaApi.Server;
using TShockAPI;
using Main = Terraria.Main;
using Version = System.Version;

namespace Plugin;

[ApiVersion(2, 1)]
public class Plugin : LazyPlugin
{
    public override string Author => "cmgy";
    public override string Description => GetString("");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override Version Version => new Version(1, 0, 0, 7);
    public Plugin(Main game) : base(game) { }

    public override void Initialize()
    {
        GetDataHandlers.PlayerSpawn.Register(this.OnSpawn);
        this.addCommands.Add(new("bed.spawn.set", this.CBed, "家", "shome"));
        base.Initialize();
    }

    private void CBed(CommandArgs args)
    {
        if (Config.Instance.SpawnOption.TryGetValue(args.Player.Name, out var spawnOption) && spawnOption != null)
        {
            spawnOption.X = args.Player.TileX;
            spawnOption.Y = args.Player.TileY;
        }
        else
        {
            Config.Instance.SpawnOption[args.Player.Name] = new()
            {
                X = args.Player.TileX,
                Y = args.Player.TileY
            };
        }
        args.Player.SendSuccessMessage(GetString("出生点位置设置成功!"));
        Config.Save();
    }

    private void OnSpawn(object? sender, GetDataHandlers.SpawnEventArgs e)
    {
        if (Config.Instance.SpawnOption.TryGetValue(e.Player.Name, out var spawnOption) && spawnOption != null)
        {
            TimingUtils.Delayed(60, () => e.Player.Teleport(spawnOption.X * 16, spawnOption.Y * 16));
        }

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.PlayerSpawn.UnRegister(this.OnSpawn);
        }
        base.Dispose(disposing);
    }


}