using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using Terraria.GameContent.Creative;
using Google.Protobuf.WellKnownTypes;
using System.Configuration;
using TShockAPI.Hooks;

namespace InvincibilityPlugin
{
    [ApiVersion(2, 1)]
    public class InvincibilityPlugin : TerrariaPlugin
    {
        private Timer invincibleTimer;
        private float duration;
        public override string Author => "肝帝熙恩";
        public override string Description => "在命令中给予玩家一段时间的无敌状态。";
        public override string Name => "InvincibilityPlugin";
        public override Version Version => new Version(1, 0, 0, 4);
        public static Configuration Config;
        public InvincibilityPlugin(Main game) : base(game)
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
            args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(InvincibilityPlugin).Name);
        }
        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("限时god无敌", InvincibleCommand, "tgod", "限时god无敌"));
            Commands.ChatCommands.Add(new Command("限时无敌帧无敌", ActivateFrameInvincibility, "tframe", "限时无敌帧无敌"));
        }

        private void InvincibleCommand(CommandArgs args)
        {
            if (!args.Player.HasPermission("限时god无敌"))
            {
                args.Player.SendErrorMessage("你没有执行此命令的权限。");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /限时god无敌 或 tgod <持续时间秒数>");
                return;
            }

            if (!float.TryParse(args.Parameters[0], out duration) || duration <= 0)
            {
                args.Player.SendErrorMessage("无效的持续时间。请输入一个正数。");
                return;
            }

            TSPlayer player = TShock.Players[args.Player.Index];

            if (player == null || !player.Active)
            {
                args.Player.SendErrorMessage("玩家不在线。");
                return;
            }

            addGodMode(player);

            invincibleTimer = new Timer(RemoveInvincibility, player, (int)(duration * 1000), Timeout.Infinite);
        }



        private void RemoveInvincibility(object state)
        {
            TSPlayer player = (TSPlayer)state;
            delGodMode(player);
            invincibleTimer.Dispose();
        }

        private void addGodMode(TSPlayer player)
        {
            player.GodMode = !player.GodMode;
            CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, player.GodMode);
            if (Config.EnableInvincibleReminder)
            {
                player.SendSuccessMessage($"你将在 {duration} 秒内无敌.");
            }

            if (!string.IsNullOrEmpty(Config.CustomInvincibleReminderText))
            {
                player.SendSuccessMessage(Config.CustomInvincibleReminderText);
            }


            NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, null, player.Index, 1f);
        }

        private void delGodMode(TSPlayer player)
        {
            player.GodMode = !player.GodMode;
            CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, player.GodMode);
            player.SendSuccessMessage($"{Config.CustomInvincibleDisableText}");

            NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, null, player.Index, 1f);
        }

        private async void ActivateFrameInvincibility(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || !double.TryParse(args.Parameters[0], out double duration) || duration <= 0)
            {
                args.Player.SendSuccessMessage("用法: /限时无敌帧无敌或tframe <持续时间秒数>");
                return;
            }

            TSPlayer player = args.Player;

            if (Config.EnableFrameReminder)
            {
                args.Player.SendSuccessMessage($"你将在 {args.Parameters[0]} 秒内无敌.");
            }
            if (!string.IsNullOrEmpty(Config.CustomStartFrameText))
            {
                player.SendSuccessMessage(Config.CustomStartFrameText);
            }

            while (duration >= 1.33)
            {
                player.SendData(PacketTypes.PlayerDodge, "", player.Index, 2f, 0f, 0f, 0);
                duration -= 0.1;
                await Task.Delay(100);
            }

            if (!string.IsNullOrEmpty(Config.CustomEndFrameText))
            {
                player.SendSuccessMessage(Config.CustomEndFrameText);
            }
        }


    }
}
