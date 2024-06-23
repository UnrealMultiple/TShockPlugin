using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.NetModules;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using TShockAPI.Net;

namespace AdditionalPylons
{
    [ApiVersion(2, 1)]
    public class AdditionalPylonsPlugin : TerrariaPlugin
    {


        #region Plugin Properties
        public override string Name => "[放置更多晶塔] AdditionalPylons";

        public override Version Version => new Version(1, 0, 0);

        public override string Author => "Stealownz，肝帝熙恩优化1449";

        public override string Description => "晶塔环境不受限制，数量可提升";


        public AdditionalPylonsPlugin(Main game) : base(game)
        {
            LoadConfig();
        }
        #endregion // Plugin Properties

        private const string permission_infiniteplace = "AdditionalPylons";

        private readonly HashSet<int> pylonItemIDList = new HashSet<int>() { 4875, 4876, 4916, 4917, 4918, 4919, 4920, 4921, 4951 };
        private readonly HashSet<int> playersHoldingPylon = new HashSet<int>();
        internal static Configuration Config;
        private static void LoadConfig()
        {

            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);

        }
        private static void ReloadConfig(ReloadEventArgs args)
        {
            LoadConfig();
            args.Player?.SendSuccessMessage("[{0}]重新加载配置完毕。", typeof(AdditionalPylonsPlugin).Name);
        }

        #region Plugin Overrides
        public override void Initialize()
        {
            GetDataHandlers.PlayerUpdate.Register(OnPlayerUpdate);
            GetDataHandlers.PlaceTileEntity.Register(OnPlaceTileEntity, HandlerPriority.High);
            GetDataHandlers.SendTileRect.Register(OnSendTileRect, HandlerPriority.High);
            GeneralHooks.ReloadEvent += ReloadConfig;
        }
        #endregion // Plugin overrides

        #region Plugin Hooks
        private void OnSendTileRect(object sender, GetDataHandlers.SendTileRectEventArgs e)
        {
            // 如果有更高优先级的插件需要处理，就尊重它们的决定...
            if (this.isDisposed || e.Handled)
                return;

            // 如果玩家没有权限，就没有必要检查数据
            if (!e.Player.HasPermission(permission_infiniteplace))
                return;

            // 最小的合理检查，这个STR很可能是晶塔
            if (e.Width != 3 || e.Length != 4)
                return;

            long savePosition = e.Data.Position;
            NetTile[,] tiles = new NetTile[e.Width, e.Length];

            for (int x = 0; x < e.Width; x++)
            {
                for (int y = 0; y < e.Length; y++)
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

        private void OnPlayerUpdate(object sender, TShockAPI.GetDataHandlers.PlayerUpdateEventArgs e)
        {
            if (this.isDisposed || e.Handled)
                return;

            if (!e.Player.HasPermission(permission_infiniteplace))
                return;

            int holdingItem = e.Player.TPlayer.inventory[e.SelectedItem].netID;
            bool alreadyHoldingPylon = playersHoldingPylon.Contains(e.PlayerId);
            bool isHoldingPylon = pylonItemIDList.Contains(holdingItem);

            if (alreadyHoldingPylon)
            {
                if (!isHoldingPylon)
                {
                    // 停止持有晶塔
                    playersHoldingPylon.Remove(e.PlayerId);

                    // 为玩家客户端重新加载晶塔系统
                    SendPlayerPylonSystem(e.PlayerId, true);
                }
            }
            else
            {
                if (isHoldingPylon)
                {
                    // 开始持有晶塔
                    playersHoldingPylon.Add(e.PlayerId);

                    // 清除玩家客户端的晶塔系统
                    SendPlayerPylonSystem(e.PlayerId, false);
                }
            }
        }

        private void OnPlaceTileEntity(object sender, TShockAPI.GetDataHandlers.PlaceTileEntityEventArgs e)
        {
            // 如果插件已被销毁或事件已被处理，则返回
            if (this.isDisposed || e.Handled)
                return;

            // 如果实体类型不是7（晶塔），则返回
            if (e.Type != 7)
                return;

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
            playersHoldingPylon.Remove(e.Player.Index);

            //e.Handled = true; // 这行代码被注释掉了，如果需要设置事件已处理，可以取消注释
        }
        #endregion // Plugin Hooks

        private void SendPlayerPylonSystem(int playerId, bool addPylons)
        {
            // 检测每种晶塔的数量是否达到指定值
            foreach (int pylonItemId in pylonItemIDList)
            {
                int count = Main.PylonSystem.Pylons.Count(pylon => pylon.TypeOfPylon == GetPylonTypeFromItemId(pylonItemId));

                // 根据晶塔类型进行不同的处理
                switch (pylonItemId)
                {
                    case 4875:
                        if (count >= Config.JungleTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("丛林晶塔数量已达到上限。");
                            return;
                        }
                        break;

                    case 4876:
                        if (count >= Config.ForestTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("森林晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4916:
                        if (count >= Config.HolyTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("神圣晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4917:
                        if (count >= Config.CaveTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("洞穴晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4918:
                        if (count >= Config.OceanTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("海洋晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4919:
                        if (count >= Config.DesertTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("沙漠晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4920:
                        if (count >= Config.SnowTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("雪原晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4921:
                        if (count >= Config.MushroomTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("蘑菇晶塔数量已达到上限。");
                            return;
                        }
                        break;
                    case 4951:
                        if (count >= Config.UniversalTowerLimit)
                        {
                            TShock.Players[playerId].SendErrorMessage("万能晶塔数量已达到上限。");
                            return;
                        }
                        break;
                }
            }

            // 如果未中断，继续发送晶塔信息
            foreach (TeleportPylonInfo pylon in Main.PylonSystem.Pylons)
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
            switch (pylonItemId)
            {
                case 4875: return TeleportPylonType.Jungle;
                case 4876: return TeleportPylonType.SurfacePurity;
                case 4916: return TeleportPylonType.Hallow;
                case 4917: return TeleportPylonType.Underground;
                case 4918: return TeleportPylonType.Beach;
                case 4919: return TeleportPylonType.Desert;
                case 4920: return TeleportPylonType.Snow;
                case 4921: return TeleportPylonType.GlowingMushroom;
                case 4951: return TeleportPylonType.Victory;
                default: return TeleportPylonType.Count; // 或者根据实际情况返回一个默认值
            }
        }

        #region [IDisposable Implementation]
        private bool isDisposed = false;

        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (this.IsDisposed)
                return;

            if (isDisposing)
            {
                GetDataHandlers.PlayerUpdate.UnRegister(OnPlayerUpdate);
                GetDataHandlers.PlaceTileEntity.UnRegister(OnPlaceTileEntity);
                GetDataHandlers.SendTileRect.UnRegister(OnSendTileRect);
            }

            base.Dispose(isDisposing);
            this.isDisposed = true;
        }
        #endregion // [IDisposable Implementation]
    }
}
