using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace Autoclear
{
    [ApiVersion(2, 1)]
    public class Autoclear : TerrariaPlugin
    {
        public override string Author => "大豆子[Mute适配1447]，肝帝熙恩更新";
        public override string Description => "智能扫地机";
        public override string Name => "智能自动扫地";
        public override Version Version => new Version(1, 0, 3);
        public static Configuration Config;
        private bool _sweepScheduled = false;
        private DateTime _sweepScheduledAt;
        private long _updateCounter;

        public Autoclear(Main game) : base(game)
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            Config = Configuration.Read(Configuration.FilePath);
            Config.Write(Configuration.FilePath);
        }

        private static void ReloadConfig(ReloadEventArgs args)
        {
            LoadConfig();
            args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(Autoclear).Name);
        }

        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= ReloadConfig;
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void OnUpdate(EventArgs args)
        {
            _updateCounter++;

            if (_updateCounter % (60 * Config.detectionIntervalSeconds) == 0)
            {
                int totalItems2 = 0;
                for (int i = 0; i < Main.item.Length; i++)
                {
                    if (Main.item[i].active && !Config.NonSweepableItemIDs.Contains(Main.item[i].type))
                    {
                        totalItems2++;
                    }
                }

                if (totalItems2 >= Config.SmartSweepThreshold)
                {
                    if (!_sweepScheduled)
                    {
                        _sweepScheduled = true;
                        _sweepScheduledAt = DateTime.UtcNow.AddSeconds(Config.DelayedSweepTimeoutSeconds);
                        TSPlayer.All.SendSuccessMessage($"{Config.DelayedSweepCustomMessage}");
                    }
                }
                if (_sweepScheduled && DateTime.UtcNow >= _sweepScheduledAt)
                {
                    // 到达清扫时间，执行清扫任务
                    _sweepScheduled = false;
                    PerformSmartSweep();
                }
            }
        }

        private void PerformSmartSweep()
        {
            int totalItems = 0;
            int totalThrowable = 0;
            int totalSwinging = 0;
            int totalRegular = 0;
            int totalEquipment = 0;
            int totalVanity = 0;

            for (int i = 0; i < Main.item.Length; i++)
            {
                if (Main.item[i].active && !Config.NonSweepableItemIDs.Contains(Main.item[i].type))
                {
                    bool isThrowable = Main.item[i].damage > 0 && Main.item[i].maxStack > 1;
                    bool isSwinging = Main.item[i].damage > 0 && Main.item[i].maxStack == 1;
                    bool isRegular = Main.item[i].damage < 0 && Main.item[i].maxStack > 1;
                    bool isEquipment = Main.item[i].damage == 0 && Main.item[i].maxStack == 1;
                    bool isVanity = Main.item[i].damage < 0 && Main.item[i].maxStack == 1;

                    if ((Config.SweepThrowable && isThrowable) ||
                        (Config.SweepSwinging && isSwinging) ||
                        (Config.SweepRegular && isRegular) ||
                        (Config.SweepEquipment && isEquipment) ||
                        (Config.SweepVanity && isVanity))
                    {
                        Main.item[i].active = false;
                        TSPlayer.All.SendData(PacketTypes.ItemDrop, " ", i, 0f, 0f, 0f, 0);
                        totalItems++;

                        if (isThrowable) totalThrowable++;
                        if (isSwinging) totalSwinging++;
                        if (isRegular) totalRegular++;
                        if (isEquipment) totalEquipment++;
                        if (isVanity) totalVanity++;
                    }
                }
            }

            if (totalItems > 0)
            {
                if (!string.IsNullOrEmpty(Config.CustomMessage))
                {
                    TSPlayer.All.SendSuccessMessage($"{Config.CustomMessage}");
                }

                if (Config.SpecificMessage)
                {
                    TSPlayer.All.SendSuccessMessage($"智能扫地机已清扫：[c/FFFFFF:{totalItems}]种物品");
                    TSPlayer.All.SendSuccessMessage($"包含：【投掷武器[c/FFFFFF:{totalThrowable}]】-【挥动武器[c/FFFFFF:{totalSwinging}]】-【普通物品[c/FFFFFF:{totalRegular}]】-【装备[c/FFFFFF:{totalEquipment}]】-【时装[c/FFFFFF:{totalVanity}]】");
                }
            }
        }
    }
}