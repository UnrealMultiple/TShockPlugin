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
        public override Version Version => new Version(1, 2, 1);
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
                            bool HandSwitch = Config.Hand ? plr.TPlayer.inventory[plr.TPlayer.selectedItem].type == item : inv.type == item ;

                            if (HandSwitch)
                            {
                                bool Stored = false;  //用来防止4个储存空间有同样物品刷堆叠用
                                if (Config.bank1 && !Stored)
                                    Stored = AutoStoredItem(plr);
                                if (Config.bank2 && !Stored)
                                    Stored = AutoStoredItem2(plr);
                                if (Config.bank3 && !Stored)
                                    Stored = AutoStoredItem3(plr);
                                if (Config.bank4 && !Stored)
                                    Stored = AutoStoredItem4(plr);
                                if (Stored) break;
                            }
                        }
                    }
                }
            });
        }
        #endregion

        #region 自动存储方法(存钱罐）
        public static bool AutoStoredItem(TSPlayer args)
        {
            if (!args.IsLoggedIn || !Config.Enable || !Config.bank1) return false;

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
                            && Array.Exists(item.ID, id => id == inv.type)
                            && inv.type == bank.type
                            && Array.Exists(item.ID, id => id == bank.type)
                            && Array.Exists(item.ID, id => id != plr.inventory[plr.selectedItem].type))
                        {

                            bank.stack += inv.stack;
                            inv.TurnToAir();
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Inventory0 + i);
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank1_0 + b);

                            if (Config.Mess)
                                args.SendMessage($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的存钱罐 当前数量: {bank.stack}", 255, 246, 158);
                            return true;
                        }

                        else if (Config.clear && bank.stack >= 9999)
                        {
                            bank.stack = 9999 - 1;
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank1_0 + b);
                            if (Config.Mess)
                                args.SendMessage($"【自动储存】检测到'[c/92C5EC:{bank.Name}]'已达到 [c/F7575C:{bank.stack}] 请整理你的存钱罐", 255, 246, 158);
                        }

                    }
                }
            }
            return false;
        }
        #endregion

        #region 自动存储方法（保险箱）
        public static bool AutoStoredItem2(TSPlayer args)
        {
            if (!args.IsLoggedIn || !Config.Enable || !Config.bank2) return false;

            Player plr = args.TPlayer;
            foreach (var item in Config.Items)
            {
                for (int b = 0; b < plr.bank2.item.Length; b++)
                {
                    var bank = plr.bank2.item[b];

                    for (int i = 0; i < plr.inventory.Length; i++)
                    {
                        var inv = plr.inventory[i];
                        if (inv.stack >= item.Stack
                           && Array.Exists(item.ID, id => id == inv.type)
                           && inv.type == bank.type
                           && Array.Exists(item.ID, id => id == bank.type)
                           && Array.Exists(item.ID, id => id != plr.inventory[plr.selectedItem].type))
                        {

                            bank.stack += inv.stack;
                            inv.TurnToAir();
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Inventory0 + i);
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank2_0 + b);

                            if (Config.Mess)
                                args.SendMessage($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的保险箱 当前数量: {bank.stack}", 255, 246, 158);
                            return true;
                        }

                        else if (Config.clear && bank.stack >= 9999)
                        {
                            bank.stack = 9999 - 1;
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank2_0 + b);
                            if (Config.Mess)
                                args.SendMessage($"【自动储存】检测到'[c/92C5EC:{bank.Name}]'已达到 [c/F7575C:{bank.stack}] 请整理你的保险箱", 255, 246, 158);
                        }

                    }
                }
            }
            return false;
        }
        #endregion

        #region 自动存储方法（护卫熔炉）
        public static bool AutoStoredItem3(TSPlayer args)
        {
            if (!args.IsLoggedIn || !Config.Enable || !Config.bank3) return false;

            Player plr = args.TPlayer;
            foreach (var item in Config.Items)
            {
                for (int b = 0; b < plr.bank3.item.Length; b++)
                {
                    var bank = plr.bank3.item[b];

                    for (int i = 0; i < plr.inventory.Length; i++)
                    {
                        var inv = plr.inventory[i];
                        if (inv.stack >= item.Stack
                            && Array.Exists(item.ID, id => id == inv.type)
                            && inv.type == bank.type
                            && Array.Exists(item.ID, id => id == bank.type)
                            && Array.Exists(item.ID, id => id != plr.inventory[plr.selectedItem].type))
                        {

                            bank.stack += inv.stack;
                            inv.TurnToAir();
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Inventory0 + i);
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank3_0 + b);

                            if (Config.Mess)
                                args.SendMessage($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的护卫熔炉 当前数量: {bank.stack}", 255, 246, 158);
                            return true;
                        }

                        else if (Config.clear && bank.stack >= 9999)
                        {
                            bank.stack = 9999 - 1;
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank3_0 + b);
                            if (Config.Mess)
                                args.SendMessage($"【自动储存】检测到'[c/92C5EC:{bank.Name}]'已达到 [c/F7575C:{bank.stack}] 请整理你的护卫熔炉", 255, 246, 158);
                        }

                    }
                }
            }
            return false;
        }
        #endregion

        #region 自动存储方法（虚空袋）
        public static bool AutoStoredItem4(TSPlayer args)
        {
            if (!args.IsLoggedIn || !Config.Enable || !Config.bank4) return false;

            Player plr = args.TPlayer;
            foreach (var item in Config.Items)
            {
                for (int b = 0; b < plr.bank4.item.Length; b++)
                {
                    var bank = plr.bank4.item[b];

                    for (int i = 0; i < plr.inventory.Length; i++)
                    {
                        var inv = plr.inventory[i];
                        if (inv.stack >= item.Stack
                           && Array.Exists(item.ID, id => id == inv.type)
                           && inv.type == bank.type
                           && Array.Exists(item.ID, id => id == bank.type)
                           && Array.Exists(item.ID, id => id != plr.inventory[plr.selectedItem].type))
                        {

                            bank.stack += inv.stack;
                            inv.TurnToAir();
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Inventory0 + i);
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank4_0 + b);

                            if (Config.Mess)
                                args.SendMessage($"【自动储存】已将'[c/92C5EC:{bank.Name}]'存入您的虚空袋 当前数量: {bank.stack}", 255, 246, 158);
                            return true;
                        }

                        else if (Config.clear && bank.stack >= 9999)
                        {
                            bank.stack = 9999 - 1;
                            args.SendData(PacketTypes.PlayerSlot, "", args.Index, PlayerItemSlotID.Bank4_0 + b);
                            if (Config.Mess)
                                args.SendMessage($"【自动储存】检测到'[c/92C5EC:{bank.Name}]'已达到 [c/F7575C:{bank.stack}] 请整理你的虚空袋", 255, 246, 158);
                        }

                    }
                }
            }
            return false;
        }
        #endregion
    }
}