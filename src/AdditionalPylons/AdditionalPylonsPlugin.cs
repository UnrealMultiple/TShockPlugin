using LazyAPI;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.NetModules;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Net;

namespace AdditionalPylons;

[ApiVersion(2, 1)]
public class AdditionalPylonsPlugin : LazyPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 1, 0);
    public override string Author => "Stealownz,肝帝熙恩优化1449";
    public override string Description => GetString("自定义各类晶塔数量上限，可配置跳过城镇 NPC 和群落检查");

    public AdditionalPylonsPlugin(Main game) : base(game) { }

    private const string permission_infiniteplace = "AdditionalPylons";
    // Track the actual item, including denied types, to handle direct pylon-to-pylon
    // switches without repeating limit messages on every player update.
    private readonly Dictionary<int, int> playersHoldingPylon = new();
    private readonly PylonConditionHooks conditionHooks = new();
    public bool IsDisposed { get; private set; }

    public override void Initialize()
    {
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        GetDataHandlers.PlaceTileEntity.Register(this.OnPlaceTileEntity, HandlerPriority.High);
        GetDataHandlers.SendTileRect.Register(this.OnSendTileRect, HandlerPriority.High);
        ServerApi.Hooks.ServerLeave.Register(this, this.OnServerLeave);
        // Install hooks before config load; the flag is checked at runtime,
        // so toggling NoTownEnvironment plus /reload takes effect immediately.
        try
        {
            this.conditionHooks.Install();
            Console.WriteLine($"[AdditionalPylons] v{this.Version} pylon condition hooks installed; " +
                "town NPC and biome teleport checks follow 晶塔无需城镇环境 (NoTownEnvironment).");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"[AdditionalPylons] v{this.Version} FAILED to install pylon condition hooks: {exception}");
            Console.Error.WriteLine("[AdditionalPylons] Placement limits keep working, but NoTownEnvironment will have no effect until this is fixed.");
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        if (this.IsDisposed) return;
        if (isDisposing)
        {
            this.conditionHooks.Dispose();
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            GetDataHandlers.PlaceTileEntity.UnRegister(this.OnPlaceTileEntity);
            GetDataHandlers.SendTileRect.UnRegister(this.OnSendTileRect);
            ServerApi.Hooks.ServerLeave.Deregister(this, this.OnServerLeave);
            foreach (var playerId in this.playersHoldingPylon.Keys.ToArray())
            {
                if (TShock.Players[playerId]?.Active == true)
                    this.SendPlayerPylonSystem(playerId, true);
            }
            this.playersHoldingPylon.Clear();
        }
        base.Dispose(isDisposing);
        this.IsDisposed = true;
    }

    private void OnServerLeave(LeaveEventArgs e) => this.playersHoldingPylon.Remove(e.Who);

    private void OnSendTileRect(object? sender, GetDataHandlers.SendTileRectEventArgs e)
    {
        if (this.IsDisposed || e.Handled || !e.Player.HasPermission(permission_infiniteplace)) return;
        if (e.Width != 3 || e.Length != 4) return;

        var savePosition = e.Data.Position;
        try
        {
            for (var x = 0; x < e.Width; x++)
            {
                for (var y = 0; y < e.Length; y++)
                {
                    if (new NetTile(e.Data).Type != Terraria.ID.TileID.TeleportationPylon) return;
                }
            }
        }
        finally
        {
            e.Data.Seek(savePosition, System.IO.SeekOrigin.Begin);
        }

        // Preserve upstream placement compatibility: TShock's STR implementation
        // calls PlaceEntityNet, which would otherwise reject a duplicate type.
        // This remains the upstream client-assisted placement mechanism, not an
        // authoritative server-side quota enforcement system.
        Main.PylonSystem._pylons.Clear();
    }

    private void OnPlayerUpdate(object? sender, GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (this.IsDisposed || e.Handled) return;
        var inventory = e.Player.TPlayer.inventory;
        if (e.SelectedItem < 0 || e.SelectedItem >= inventory.Length) return;

        var holdingItem = inventory[e.SelectedItem].type;
        var isHoldingPylon = e.Player.HasPermission(permission_infiniteplace)
            && PylonLimits.GetPylonTypeFromItemId(holdingItem) != TeleportPylonType.Count;
        var wasHoldingPylon = this.playersHoldingPylon.TryGetValue(e.PlayerId, out var previousItem);
        if (wasHoldingPylon && isHoldingPylon && previousItem == holdingItem) return;

        // Restoring the list must never depend on any quota, including when the
        // player switches directly to another pylon or loses the permission.
        if (wasHoldingPylon)
        {
            this.SendPlayerPylonSystem(e.PlayerId, true);
            this.playersHoldingPylon.Remove(e.PlayerId);
        }
        if (isHoldingPylon)
        {
            this.playersHoldingPylon[e.PlayerId] = holdingItem;
            this.SendPlayerPylonSystem(e.PlayerId, false, holdingItem);
        }
    }

    private void OnPlaceTileEntity(object? sender, GetDataHandlers.PlaceTileEntityEventArgs e)
    {
        if (this.IsDisposed || e.Handled || e.Type != 7) return;
        if (!e.Player.HasPermission(permission_infiniteplace))
        {
            TSPlayer.All.SendTileRect(e.X, e.Y, 3, 4);
            return;
        }

        Terraria.GameContent.Tile_Entities.TETeleportationPylon.Place(e.X, e.Y);
        Main.PylonSystem.Reset();
        TSPlayer.All.SendTileRect(e.X, e.Y, 3, 4);
        this.SendPlayerPylonSystem(e.Player.Index, true);
        this.playersHoldingPylon.Remove(e.Player.Index);
    }

    private void SendPlayerPylonSystem(int playerId, bool addPylons, int holdingItem = 0)
    {
        var type = PylonLimits.GetPylonTypeFromItemId(holdingItem);
        if (!addPylons)
        {
            if (type == TeleportPylonType.Count) return;
            var count = Main.PylonSystem.Pylons.Count(pylon => pylon.TypeOfPylon == type);
            if (count >= PylonLimits.GetLimit(Configuration.Instance, type))
            {
                TShock.Players[playerId].SendErrorMessage(GetString("当前手持晶塔数量已达到上限。"));
                return;
            }
        }

        foreach (var pylon in Main.PylonSystem.Pylons)
        {
            if (!PylonLimits.ShouldSendPylon(addPylons, type, pylon.TypeOfPylon)) continue;
            Terraria.Net.NetManager.Instance.SendToClient(
                NetTeleportPylonModule.SerializePylonWasAddedOrRemoved(pylon,
                    addPylons ? NetTeleportPylonModule.SubPacketType.PylonWasAdded
                              : NetTeleportPylonModule.SubPacketType.PylonWasRemoved),
                playerId);
        }
    }
}
