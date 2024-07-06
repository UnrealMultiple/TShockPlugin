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
        public override Version Version => new Version(1, 2, 2);
        public override string Description => "涡轮增压不蒸鸭";
        #endregion

        #region 注册与释放
        public AutoStoreItems(Main game) : base(game) { }
        public override void Initialize()
        {
            LoadConfig();
            GeneralHooks.ReloadEvent += (_) => LoadConfig();
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= (_) => LoadConfig();
                ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            }
            base.Dispose(disposing);
        }
        #endregion

        #region 配置重载读取与写入方法
        internal static Configuration Config = new();
        private static void LoadConfig()
        {
            Config = Configuration.Read();
            WriteName();
            Config.Write();
            TShock.Log.ConsoleInfo("[自动存储]重新加载配置完毕。");
        }
        #endregion

        #region 获取物品ID的中文名
        private static void WriteName()
        {
            foreach (var item in Config.Items)
            {
                var Names = new HashSet<string>(item.Name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries));
                foreach (var id in item.ID)
                {
                    string ItemName;
                    ItemName = (string)Lang.GetItemName(id);
                    if (!Names.Contains(ItemName))
                    {
                        Names.Add(ItemName);
                    }
                }
                item.Name = string.Join(", ", Names);
            }
        }
        #endregion

        #region 检测持有物品方法
        public static long Timer = 0L;
        private void OnGameUpdate(EventArgs args)
        {
            Timer++;

            foreach (var plr in TShock.Players.Where(plr => plr != null && plr.Active && plr.IsLoggedIn && Config.Enable))
            {
                if (!plr.HasPermission("tshock.ignore.itemstack") && Timer % 300 == 0)
                    TShock.Utils.Broadcast($"\n【自动储存】请通知管理给予免检堆栈权限：\n" +
                        $"/group addpern defualt [c/92C5EC:tshock.ignore.itemstack]\n", 255, 246, 158);

                if (Timer % Config.time == 0)
                {
                    foreach (var item in Config.HoldItems)
                    {
                        bool Stored = false;
                        for (int i = 0; i < plr.TPlayer.inventory.Length; i++)
                        {
                            var inv = plr.TPlayer.inventory[i];
                            if ((Config.Hand ? inv.type == plr.TPlayer.inventory[plr.TPlayer.selectedItem].type : inv.type == item) &&
                                (Config.bank1 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank.item, PlayerItemSlotID.Bank1_0, "存钱罐")) ||
                                 Config.bank2 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank2.item, PlayerItemSlotID.Bank2_0, "保险箱")) ||
                                 Config.bank3 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank3.item, PlayerItemSlotID.Bank3_0, "护卫熔炉")) ||
                                 Config.bank4 && !Stored && (Stored = AutoStoredItem(plr, plr.TPlayer.bank4.item, PlayerItemSlotID.Bank4_0, "虚空袋"))))
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region 自动储存物品方法
        public static bool AutoStoredItem(TSPlayer player, Item[] bankItems, int bankSlot, string bankName)
        {
            if (!player.IsLoggedIn || !Config.Enable) return false;

            Player plr = player.TPlayer;
            HashSet<int> itemID = new HashSet<int>(Config.Items.SelectMany(x => x.ID));

            foreach (var bank in bankItems)
            {
                for (int i = 0; i < plr.inventory.Length; i++)
                {
                    var inv = plr.inventory[i];
                    var items = Config.Items.FirstOrDefault(x => x.ID.Contains(inv.type));
                    if (items != null
                        && inv.stack >= items.Stack
                        && itemID.Contains(inv.type)
                        && inv.type == bank.type
                        && inv.type != plr.inventory[plr.selectedItem].type)
                    {
                        bank.stack += inv.stack;
                        inv.TurnToAir();
                        player.SendData(PacketTypes.PlayerSlot, null, player.Index, PlayerItemSlotID.Inventory0 + i);
                        player.SendData(PacketTypes.PlayerSlot, null, player.Index, bankSlot + Array.IndexOf(bankItems, bank));

                        if (Config.Mess)
                            player.SendMessage($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的{bankName} 当前数量: {bank.stack}", 255, 246, 158);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

    }
}