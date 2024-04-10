using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace LifemaxExtra
{
    [ApiVersion(2, 1)]
    public class LifemaxExtra : TerrariaPlugin
    {
        public override string Author => "佚名，肝帝熙恩添加自定义";
        public override string Description => "提升生命值上限";
        public override string Name => "LifemaxExtra";
        public override Version Version => new Version(1, 0, 0, 5);
        public static Configuration Config;
        private bool[] controlUseItemOld;
        private int[] itemUseTime;

        public LifemaxExtra(Main game) : base(game)
        {
            LoadConfig();
            this.controlUseItemOld = new bool[255];
            this.itemUseTime = new int[255];
        }

        private static void LoadConfig()
        {
            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);
        }

        private static void ReloadConfig(ReloadEventArgs args)
        {
            LoadConfig();
            args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(LifemaxExtra).Name);
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            ServerApi.Hooks.GameUpdate.Register(this, new HookHandler<EventArgs>(this.OnUpdate));
            PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= ReloadConfig;
                PlayerHooks.PlayerPostLogin -= OnPlayerPostLogin;
                ServerApi.Hooks.GameUpdate.Deregister(this, new HookHandler<EventArgs>(this.OnUpdate));
            }
            base.Dispose(disposing);
        }

        private void OnPlayerPostLogin(PlayerPostLoginEventArgs args)
        {
            foreach (TSPlayer tsplayer in TShock.Players)
            {
                if (tsplayer != null)
                {
                    // 检查生命上限并设置生命值
                    CheckAndSetPlayerHealth(tsplayer);
                }
            }
        }

        private static void CheckAndSetPlayerHealth(TSPlayer tsplayer)
        {
            int index = tsplayer.Index;
            Player tplayer = tsplayer.TPlayer;

            // 如果生命上限大于配置的最大值
            if (tplayer.statLifeMax > Config.LifeFruitMaxLife)
            {
                // 将生命值设置为配置的最大值
                tplayer.statLifeMax = Config.LifeFruitMaxLife;
                tsplayer.SendData(PacketTypes.PlayerHp, "", index);
            }
        }

        private void OnUpdate(EventArgs args)
        {
            foreach (TSPlayer tsplayer in TShock.Players)
            {

                if (!(tsplayer == null))
                {
                    int index = tsplayer.Index;
                    Player tplayer = tsplayer.TPlayer;
                    Item heldItem = tplayer.HeldItem;

                    if (!this.controlUseItemOld[index] && tsplayer.TPlayer.controlUseItem && this.itemUseTime[index] <= 0)
                    {
                        int useTime = heldItem.useTime; // 获取物品使用时间
                        int type = heldItem.type; // 获取物品类型

                        if (type != 29) // 如果物品不是 ID 为 29 的物品
                        {
                            if (type == 1291) // 如果物品是 ID 为 1291 的物品（Life Fruit）
                            {
                                if (tplayer.statLifeMax >= Config.LifeCrystalMaxLife) // 如果玩家的生命上限大于等于配置的最大水晶生命值
                                {
                                    if (tsplayer.TPlayer.statLifeMax < Config.LifeFruitMaxLife) // 如果玩家当前生命上限小于配置的最大果实生命值
                                    {
                                        tsplayer.TPlayer.inventory[tsplayer.TPlayer.selectedItem].stack--; // 减少玩家背包中选定物品的堆叠数量
                                        tsplayer.SendData(PacketTypes.PlayerSlot, "", index, (float)tplayer.selectedItem); // 更新客户端的选定物品槽位
                                        tplayer.statLifeMax += 5; // 增加玩家的生命上限
                                        tsplayer.SendData(PacketTypes.PlayerHp, "", index); // 更新客户端的生命值显示
                                    }
                                    else if (tsplayer.TPlayer.statLifeMax > Config.LifeFruitMaxLife) // 如果玩家当前生命上限大于配置的最大果实生命值
                                    {
                                        tplayer.statLifeMax = Config.LifeFruitMaxLife; // 将玩家的生命上限设置为配置的最大果实生命值
                                        tsplayer.SendData(PacketTypes.PlayerHp, "", index); // 更新客户端的生命值显示
                                    } 
                                }
                            }
                        }
                        else // 如果物品是 ID 为 29 的物品（Life Crystal）
                        {
                            if (tplayer.statLifeMax <= Config.LifeCrystalMaxLife) // 如果玩家的生命上限小于等于最大水晶生命值
                                {
                                    if (tsplayer.TPlayer.statLifeMax < Config.LifeCrystalMaxLife) // 如果玩家当前生命上限小于配置的最大水晶生命值
                                    {
                                        tsplayer.TPlayer.inventory[tplayer.selectedItem].stack--; // 减少玩家背包中选定物品的堆叠数量
                                        tsplayer.SendData(PacketTypes.PlayerSlot, "", index, (float)tplayer.selectedItem); // 更新客户端的选定物品槽位
                                        tplayer.statLifeMax += 20; // 增加玩家的生命上限
                                        tsplayer.SendData(PacketTypes.PlayerHp, "", index); // 更新客户端的生命值显示
                                    }
                                    else if (tsplayer.TPlayer.statLifeMax > Config.LifeFruitMaxLife) // 如果玩家当前生命上限大于配置的最大果生命值
                                    {
                                        tplayer.statLifeMax = Config.LifeFruitMaxLife; // 将玩家的生命上限设置为配置的最大生命果生命值
                                        tsplayer.SendData(PacketTypes.PlayerHp, "", index); // 更新客户端的生命值显示
                                }
                            }
                        }
                    }
                    this.controlUseItemOld[index] = tsplayer.TPlayer.controlUseItem;
                }
            }
        }
    }
}
