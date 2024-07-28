using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace BP
{
    [ApiVersion(2, 1)]
    public class BackPlugin : TerrariaPlugin
    {
        private readonly Dictionary<int, DateTime> cooldowns = new Dictionary<int, DateTime>();

        public override string Author => "Megghy,熙恩改";
        public override string Description => "允许玩家传送回死亡地点";
        public override string Name => "BackPlugin";
        public override Version Version => new Version(1, 0, 0, 4);
        public static Configuration Config;
        public BackPlugin(Main game) : base(game)
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
            args.Player?.SendSuccessMessage("[{0}] 重新加载配置完毕。", typeof(BackPlugin).Name);
        }
        public override void Initialize()
        {
            GeneralHooks.ReloadEvent += ReloadConfig;
            ServerApi.Hooks.ServerLeave.Register(this, ResetPos);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnPlayerJoin);
            GetDataHandlers.KillMe += OnDead;

            Commands.ChatCommands.Add(new Command("back", Back, "back")
            {
                HelpText = "返回最后一次死亡的位置",
                AllowServer = false
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                GeneralHooks.ReloadEvent -= ReloadConfig;
                ServerApi.Hooks.ServerLeave.Deregister(this, ResetPos);
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnPlayerJoin);
                GetDataHandlers.KillMe -= OnDead;
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Back);
            }

            base.Dispose(disposing);
        }

        private void ResetPos(LeaveEventArgs args)
        {
            var list = TSPlayer.FindByNameOrID(Main.player[args.Who].name);
            if (list.Count > 0)
                list[0].RemoveData("DeadPoint");
        }

        private void Back(CommandArgs args)
        {
            var data = args.Player.GetData<Point>("DeadPoint");

            if (args.Player.TPlayer.dead)
            {
                args.Player.SendErrorMessage("你尚未复活，无法传送回死亡地点.");
            }
            else if (data != Point.Zero)
            {
                if (!CanUseCommand(args.Player))
                {
                    var remainingCooldown = GetRemainingCooldown(args.Player);
                    args.Player.SendErrorMessage($"你还需要等待 {remainingCooldown.TotalSeconds:F} 秒才能再次使用此命令.");
                    return;
                }

                try
                {
                    args.Player.Teleport(data.X, data.Y, 1);
                    args.Player.SendSuccessMessage($"已传送至死亡地点 [c/8DF9D8:<{data.X / 16} - {data.Y / 16}>].");

                    SetCooldown(args.Player);
                }
                catch (Exception ex)
                {
                    TShock.Log.Error($"BackPlugin: 传送玩家 {args.Player.Name} 时发生错误: {ex}");
                }
            }
            else
            {
                args.Player.SendErrorMessage("你还未死亡过");
            }
        }

        private void OnDead(object o, GetDataHandlers.KillMeEventArgs args)
        {
            args.Player.SetData("DeadPoint", new Point((int)args.Player.X, (int)args.Player.Y));
        }

        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            var list = TSPlayer.FindByNameOrID(Main.player[args.Who].name);
            if (list.Count > 0)
                list[0].SetData("DeadPoint", Point.Zero);
        }

        private bool CanUseCommand(TSPlayer player)
        {
            if (cooldowns.ContainsKey(player.Index))
            {
                var cooldownEnd = cooldowns[player.Index];
                if (DateTime.Now < cooldownEnd)
                {
                    return false;
                }
            }

            return true;
        }

        private TimeSpan GetRemainingCooldown(TSPlayer player)
        {
            if (cooldowns.ContainsKey(player.Index))
            {
                var cooldownEnd = cooldowns[player.Index];
                var remainingTime = cooldownEnd - DateTime.Now;
                return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
            }

            return TimeSpan.Zero;
        }

        private void SetCooldown(TSPlayer player)
        {
            var cooldownDuration = TimeSpan.FromSeconds(Config.BackCooldown);
            var cooldownEnd = DateTime.Now.Add(cooldownDuration);

            if (cooldowns.ContainsKey(player.Index))
            {
                cooldowns[player.Index] = cooldownEnd;
            }
            else
            {
                cooldowns.Add(player.Index, cooldownEnd);
            }
        }
    }
}

