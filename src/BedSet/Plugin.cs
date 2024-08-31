using BedSet;
using TerrariaApi.Server;
using TShockAPI;
using Main = Terraria.Main;
using Version = System.Version;

namespace Plugin;

[ApiVersion(2, 1)]
public class Plugin : TerrariaPlugin
{
    public override string Author => "cmgy";
    public override string Description => "";
    public override string Name => "BedSet 床设置";
    public override Version Version => new(1, 0, 0, 1);

    private Config Config { get; set; } = new();
    public Plugin(Main game) : base(game) { }

    public override void Initialize()
    {
        this.Config = Config.Read();
        GetDataHandlers.PlayerSpawn.Register(this.OnSpawn);
        Commands.ChatCommands.Add(new("bed.spawn.set", this.CBed, "家", "shome"));
    }

    private void CBed(CommandArgs args)
    {
        if (this.Config.SpawnOption.TryGetValue(args.Player.Name, out var spawnOption) && spawnOption != null)
        {
            spawnOption.X = args.Player.TileX;
            spawnOption.Y = args.Player.TileY;
        }
        else
        {
            this.Config.SpawnOption[args.Player.Name] = new()
            {
                X = args.Player.TileX,
                Y = args.Player.TileY
            };
        }
        args.Player.SendSuccessMessage("出生点位置设置成功!");
        this.Config.Write();
    }

    private void OnSpawn(object? sender, GetDataHandlers.SpawnEventArgs e)
    {
        if (this.Config.SpawnOption.TryGetValue(e.Player.Name, out var spawnOption) && spawnOption != null)
        {
            Task.Run(async () =>
            {
                await Task.Delay(100);
                e.Player.Teleport(spawnOption.X * 16, spawnOption.Y * 16);
            });
        }

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.CBed);
            GetDataHandlers.PlayerSpawn.UnRegister(this.OnSpawn);
        }
        base.Dispose(disposing);
    }


}