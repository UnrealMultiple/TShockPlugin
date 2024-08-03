using Terraria;
using Terraria.GameContent.Creative;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace InvincibilityPlugin
{
    [ApiVersion(2, 1)]
    public class InvincibilityPlugin : TerrariaPlugin
    {
        public override string Author => "肝帝熙恩";
        public override string Description => "在命令中给予玩家一段时间的无敌状态。";
        public override string Name => "InvincibilityPlugin";
        public override Version Version => new Version(1, 0, 6);
        public static Configuration Config;

        private readonly Dictionary<TSPlayer, float> invincibleDurations = new();
        private readonly Dictionary<TSPlayer, float> frameDurations = new();

        public long FrameCount;

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
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= ReloadConfig;
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("Invincibility.god", InvincibleCommand, "tgod", "限时god无敌"));
            Commands.ChatCommands.Add(new Command("Invincibility.frame", ActivateFrameInvincibility, "tframe", "限时无敌帧无敌"));
        }

        private void OnUpdate(EventArgs args)
        {
            FrameCount++;
            //每100毫秒检测一次
            if (FrameCount % 6 == 0)
            {
                foreach (var player in invincibleDurations.Keys.ToList())
                {
                    if (invincibleDurations[player] <= 100 || !player.Active)
                    {
                        delGodMode(player);
                    }
                    else
                    {
                        invincibleDurations[player] -= 100;
                    }
                }

                foreach (var player in frameDurations.Keys.ToList())
                {
                    if (frameDurations[player] >= 0 && player.Active)
                    {
                        player.SendData(PacketTypes.PlayerDodge, "", player.Index, 2f, 0f, 0f, 0);
                        frameDurations[player] -= 100;
                    }
                    else
                    {
                        player.SendSuccessMessage(Config.CustomEndFrameText);
                        frameDurations.Remove(player);
                    }
                }
            }
        }

        private void InvincibleCommand(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("用法: /限时god无敌或tgod <持续时间秒数>");
                return;
            }

            if (!float.TryParse(args.Parameters[0], out float duration) || duration <= 0)
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
            addGodMode(player, duration);
        }

        private void addGodMode(TSPlayer player, float duration)
        {
            player.GodMode = true;
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
            invincibleDurations[player] = duration * 1000;
        }

        private void delGodMode(TSPlayer player)
        {
            player.GodMode = false;
            CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>().SetEnabledState(player.Index, player.GodMode);
            player.SendSuccessMessage($"{Config.CustomInvincibleDisableText}");
            NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, null, player.Index, 1f);
            invincibleDurations.Remove(player);
        }

        private void ActivateFrameInvincibility(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || !int.TryParse(args.Parameters[0], out int duration) || duration <= 0)
            {
                args.Player.SendErrorMessage("用法: /限时无敌帧无敌或tframe <持续时间秒数>");
                return;
            }
            if (Config.EnableFrameReminder)
            {
                args.Player.SendSuccessMessage($"你将在 {args.Parameters[0]} 秒内无敌.");
            }
            if (!string.IsNullOrEmpty(Config.CustomStartFrameText))
            {
                args.Player.SendSuccessMessage(Config.CustomStartFrameText);
            }

            frameDurations[args.Player] = duration * 1000;
        }
    }
}
