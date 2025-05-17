using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace MapTeleport;

[ApiVersion(2, 1)]
public class MapTeleport : TerrariaPlugin
{
    public MapTeleport(Main game) : base(game)
    {
        this.Order = 1;
    }
    public override Version Version => new Version(1, 0, 5);
    public override string Author => "Nova4334，肝帝熙恩汉化适配1449";
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!; public override string Description => GetString("允许玩家传送到地图上的选定位置.");

    public override void Initialize()
    {
        GetDataHandlers.ReadNetModule.Register(this.teleport);
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GetDataHandlers.ReadNetModule.UnRegister(this.teleport);
        }
        base.Dispose(disposing);
    }

    public const string ALLOWED = "maptp.noclip";

    public const string ALLOWEDSOLIDS = "maptp";

    private void teleport(object? unused, GetDataHandlers.ReadNetModuleEventArgs args)
    {
        if (args.Player.HasPermission(ALLOWED) || args.Player.HasPermission(ALLOWEDSOLIDS))
        {
            if (args.ModuleType == GetDataHandlers.NetModuleType.Ping)
            {
                using (var reader = new BinaryReader(args.Data))
                {
                    var pos = reader.ReadVector2();
                    if (!(pos.X == Tile.Type_Solid && pos.Y == Tile.Type_Solid) || args.Player.HasPermission(ALLOWEDSOLIDS))
                    {
                        args.Player.Teleport(pos.X * 16, pos.Y * 16); return;
                    }
                    else
                    {
                        args.Player.SendErrorMessage(GetString("您正在尝试传送到实心方块中。请在地图上选择一个不包含实心方块的地方，然后重试."));
                    }

                    return;
                }
            }
        }
    }
}