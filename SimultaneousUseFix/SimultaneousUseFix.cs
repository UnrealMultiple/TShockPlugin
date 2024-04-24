using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using System.Collections.Generic;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace fixbugpe
{
    [ApiVersion(2, 1)]
    public class SimultaneousUseFix : TerrariaPlugin
    {
        public override string Author => "熙恩，感谢恋恋";
        public override string Description => "解决卡双锤，卡星旋机枪之类的问题";
        public override string Name => "SimultaneousUseFix";
        public override Version Version => new Version(1, 0, 5);
        public static Configuration Config;
        public bool otherPluginExists = false;

        public SimultaneousUseFix(Main game) : base(game)
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
            args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(SimultaneousUseFix).Name);
        }


        public override void Initialize()
        {
            // 事件订阅代码
            GeneralHooks.ReloadEvent += ReloadConfig;
            ServerApi.Plugins.Get<Chireiden.TShock.Omni.Plugin>().Detections.SwapWhileUse += OnSwapWhileUse;
        }

        private void OnSwapWhileUse(int playerId, int slot)
        {
            TSPlayer player = TShock.Players[playerId];

            if (player != null && !player.HasPermission("SimultaneousUseFix"))
            {
                Item item = player.TPlayer.inventory[slot];

                if (item != null && Config.ExemptItemList.Contains(item.type))
                {
                    return;
                }

                if (Config.KickPlayerOnUse)
                {
                    TShock.Utils.Broadcast("玩家 " + player.Name + " 因为卡换格子bug被踢出", Color.Green);
                player.Kick("因为卡换格子bug被踢出");
            }

                if (Config.KillPlayerOnUse)
                {
                    TShock.Utils.Broadcast("玩家 " + player.Name + " 因为卡换格子bug被杀死", Color.Green);
                    player.KillPlayer();
                }

                if (Config.ApplyBuffOnUse)
                {
                    TShock.Utils.Broadcast("玩家 " + player.Name + " 因为卡换格子bug被上buff", Color.Green);
                    foreach (int buffType in Config.BuffTypes)
                    {
                        player.SetBuff(buffType, Config.Bufftime); // 持续1分钟，以秒为单位
                    }
                }

            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 移除事件订阅
                ServerApi.Plugins.Get<Chireiden.TShock.Omni.Plugin>().Detections.SwapWhileUse -= OnSwapWhileUse;
            }

            base.Dispose(disposing);
        }
    }
}


