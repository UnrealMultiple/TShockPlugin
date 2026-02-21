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
    #region Plugin Properties
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 8);

    public override string Author => "Stealownz,肝帝熙恩优化1449";

    public override string Description => GetString("自定义晶塔可放置的数量，至少为一个，且所有晶塔都无视环境");


    public AdditionalPylonsPlugin(Main game) : base(game)
    {
    }
    #endregion // Plugin Properties

    private const string permission_infiniteplace = "AdditionalPylons";

    private readonly HashSet<int> pylonItemIDList = new HashSet<int>() { 4875, 4876, 4916, 4917, 4918, 4919, 4920, 4921, 4951 };
    private readonly HashSet<int> playersHoldingPylon = new HashSet<int>();


    #region Plugin Overrides
    public override void Initialize()
    {
        GetDataHandlers.PlayerUpdate.Register(this.OnPlayerUpdate);
        GetDataHandlers.PlaceTileEntity.Register(this.OnPlaceTileEntity, HandlerPriority.High);
        GetDataHandlers.SendTileRect.Register(this.OnSendTileRect, HandlerPriority.High);
    }
    #endregion
    #region [IDisposable Implementation]

    public bool IsDisposed { get; private set; } = false;

    protected override void Dispose(bool isDisposing)
    {
        if (this.IsDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            GetDataHandlers.PlayerUpdate.UnRegister(this.OnPlayerUpdate);
            GetDataHandlers.PlaceTileEntity.UnRegister(this.OnPlaceTileEntity);
            GetDataHandlers.SendTileRect.UnRegister(this.OnSendTileRect);
        }

        base.Dispose(isDisposing);
        this.IsDisposed = true;
    }
    #endregion // [IDisposable Implementation]

    #region Plugin Hooks
    private void OnSendTileRect(object? sender, GetDataHandlers.SendTileRectEventArgs e)
    {
        // 如果有更高优先级的插件需要处理，就尊重它们的决定...
        if (this.IsDisposed || e.Handled)
        {
            return;
        }

        // 如果玩家没有权限，就没有必要检查数据
        if (!e.Player.HasPermission(permission_infiniteplace))
        {
            return;
        }

        // 最小的合理检查，这个STR很可能是晶塔
        if (e.Width != 3 || e.Length != 4)
        {
            return;
        }

        var savePosition = e.Data.Position;
        var tiles = new NetTile[e.Width, e.Length];

        for (var x = 0; x < e.Width; x++)
        {
            for (var y = 0; y < e.Length; y++)
            {
                tiles[x, y] = new NetTile(e.Data);
                if (tiles[x, y].Type != Terraria.ID.TileID.TeleportationPylon)
                {
                    e.Data.Seek(savePosition, System.IO.SeekOrigin.Begin);
                    return;
                }
            }
        }

        // 重置数据回到原始位置
        e.Data.Seek(savePosition, System.IO.SeekOrigin.Begin);

        // 简单地清除主系统的晶塔网络，以欺骗服务器 >:DD
        // 这样做之所以有效，是因为当晶塔系统被放置时，它无论如何都会被刷新。
        // 这个部分是必需的，因为TShock重新实现了带有反弹器的STR，
        // 然后调用PlaceEntityNet，由于在Main.PylonSystem内部已经包含了这种类型的晶塔，所以它会拒绝这个晶塔。
        Main.PylonSystem._pylons.Clear();
    }

    private void OnPlayerUpdate(object? sender, TShockAPI.GetDataHandlers.PlayerUpdateEventArgs e)
    {
        if (this.IsDisposed || e.Handled)
        {
            return;
        }

        if (!e.Player.HasPermission(permission_infiniteplace))
        {
            return;
        }

        var holdingItem = e.Player.TPlayer.inventory[e.SelectedItem].type;
        var alreadyHoldingPylon = this.playersHoldingPylon.Contains(e.PlayerId);
        var isHoldingPylon = this.pylonItemIDList.Contains(holdingItem);

        if (alreadyHoldingPylon)
        {
            if (!isHoldingPylon)
            {
                // 停止持有晶塔
                this.playersHoldingPylon.Remove(e.PlayerId);

                // 为玩家客户端重新加载晶塔系统
                this.SendPlayerPylonSystem(e.PlayerId, true);
            }
        }
        else
        {
            if (isHoldingPylon)
            {
                // 开始持有晶塔
                this.playersHoldingPylon.Add(e.PlayerId);

                // 清除玩家客户端的晶塔系统
                this.SendPlayerPylonSystem(e.PlayerId, false);
            }
        }
    }

    private void OnPlaceTileEntity(object? sender, TShockAPI.GetDataHandlers.PlaceTileEntityEventArgs e)
    {
        // 如果插件已被销毁或事件已被处理，则返回
        if (this.IsDisposed || e.Handled)
        {
            return;
        }

        // 如果实体类型不是7（晶塔），则返回
        if (e.Type != 7)
        {
            return;
        }

        // 如果玩家没有无限放置晶塔的权限，则向所有客户端发送STR以更新非无限晶塔玩家的第一个晶塔放置
        if (!e.Player.HasPermission(permission_infiniteplace))
        {
            TShockAPI.TSPlayer.All.SendTileRect(e.X, e.Y, 3, 4);
            return;
        }

        // 在指定位置放置晶塔
        Terraria.GameContent.Tile_Entities.TETeleportationPylon.Place(e.X, e.Y);

        // 这是为了更新服务器上的晶塔列表。
        // 注意：重置将向所有玩家广播更改。
        Main.PylonSystem.Reset();

        // 由于其他客户端不知道这个晶塔，所以在手动执行TETeleportationPylon.Place()之后发送STR
        TShockAPI.TSPlayer.All.SendTileRect(e.X, e.Y, 3, 4);

        // 从持有晶塔的玩家列表中移除当前玩家
        this.playersHoldingPylon.Remove(e.Player.Index);

        //e.Handled = true; // 这行代码被注释掉了，如果需要设置事件已处理，可以取消注释
    }
    #endregion // Plugin Hooks

    private void SendPlayerPylonSystem(int playerId, bool addPylons)
    {
        // 检测每种晶塔的数量是否达到指定值
        foreach (var pylonItemId in this.pylonItemIDList)
        {
            var count = Main.PylonSystem.Pylons.Count(pylon => pylon.TypeOfPylon == this.GetPylonTypeFromItemId(pylonItemId));

            // 根据晶塔类型进行不同的处理
            switch (pylonItemId)
            {
                case 4875:
                    if (count >= Configuration.Instance.MaxJunglePylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("丛林晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4876:
                    if (count >= Configuration.Instance.MaxForestPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("森林晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4916:
                    if (count >= Configuration.Instance.MaxHallowPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("神圣晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4917:
                    if (count >= Configuration.Instance.MaxCavernPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("洞穴晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4918:
                    if (count >= Configuration.Instance.MaxOceanPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("海洋晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4919:
                    if (count >= Configuration.Instance.MaxDesertPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("沙漠晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4920:
                    if (count >= Configuration.Instance.MaxSnowPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("雪原晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4921:
                    if (count >= Configuration.Instance.MaxMushroomPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("蘑菇晶塔数量已达到上限。"));
                        return;
                    }
                    break;

                case 4951:
                    if (count >= Configuration.Instance.MaxUniversalPylons)
                    {
                        TShock.Players[playerId].SendErrorMessage(GetString("万能晶塔数量已达到上限。"));
                        return;
                    }
                    break;
            }
        }

        // 如果未中断，继续发送晶塔信息
        foreach (var pylon in Main.PylonSystem.Pylons)
        {
            Terraria.Net.NetManager.Instance.SendToClient(
                NetTeleportPylonModule.SerializePylonWasAddedOrRemoved(
                    pylon,
                    addPylons ? NetTeleportPylonModule.SubPacketType.PylonWasAdded : NetTeleportPylonModule.SubPacketType.PylonWasRemoved
                ),
                playerId
            );
        }
    }

    // 根据晶塔物品ID获取对应的晶塔类型
    private TeleportPylonType GetPylonTypeFromItemId(int pylonItemId)
    {
        return pylonItemId switch
        {
            4875 => TeleportPylonType.Jungle,
            4876 => TeleportPylonType.SurfacePurity,
            4916 => TeleportPylonType.Hallow,
            4917 => TeleportPylonType.Underground,
            4918 => TeleportPylonType.Beach,
            4919 => TeleportPylonType.Desert,
            4920 => TeleportPylonType.Snow,
            4921 => TeleportPylonType.GlowingMushroom,
            4951 => TeleportPylonType.Victory,
            _ => TeleportPylonType.Count,// 或者根据实际情况返回一个默认值
        };
    }

}