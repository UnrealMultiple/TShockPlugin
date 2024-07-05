using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;


namespace Plugin
{
    [ApiVersion(2, 1)]
    public class AutoStoreItems : TerrariaPlugin
    {

        #region 插件信息
        public override string Name => "自动存储";
        public override string Author => "羽学";
        public override Version Version => new Version(1, 0, 0);
        public override string Description => "涡轮增压不蒸鸭";
        #endregion

        #region 注册与释放
        public AutoStoreItems(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private static void LoadConfig()
        {
            Config = Configuration.Read();
            Config.Write();
            TShock.Log.ConsoleInfo("[自动存储]重新加载配置完毕。");
        }
        #endregion

        #region 判断持有物品方法
        public static long Timer = 0L;
        private void OnGameUpdate(EventArgs args)
        {
            Timer++;
            TShock.Players.Where(plr => plr != null && plr.Active).ToList().ForEach(plr =>
            {
                if (!plr.IsLoggedIn || !Config.Enable) return;
                else if (Timer % Config.time == 0)
                {
                    foreach (var item in Config.HoldItems)
                    {
                        for (int i = 0; i < plr.TPlayer.inventory.Length; i++)
                        {
                            var inv = plr.TPlayer.inventory[i];

                            if (inv.type == item) 
                            { 
                                AutoStoreItem(plr);
                            }
                        }
                    }
                }

            });
        }
        #endregion

        #region 自动存储方法
        public static void AutoStoreItem(TSPlayer args)
        {
            if (!args.IsLoggedIn || !Config.Enable) return;

            Player plr = args.TPlayer;
            foreach (var item in Config.Items)
            {
                for (int b = 0; b < plr.bank.item.Length; b++)
                {
                    var bank = plr.bank.item[b];
                    for (int i = 0; i < plr.inventory.Length; i++)
                    {
                        var inv = plr.inventory[i];
                        if (inv.stack >= item.Stack
                            && inv.type == item.ID
                            && inv.type == bank.type
                            && bank.type == item.ID)
                        {
                            bank.stack += inv.stack;
                            inv.TurnToAir();
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Inventory0 + i);
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank1_0 + b);
                        }
                    }
                }
            }
        }
        #endregion
    }
}