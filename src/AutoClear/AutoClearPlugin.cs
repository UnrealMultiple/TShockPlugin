using System;
using System.Linq;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace AutoClear
{
    [ApiVersion(2, 1)]
    public sealed class AutoClearPlugin : TerrariaPlugin
    {
        private Command tshockClearCommand;
        private Command safeClearCommand;
        private Command reloadCommand;
        private AutoClearConfiguration configuration;
        private AutoClearScheduler scheduler;
        private bool starverProtectionActive;

        public AutoClearPlugin(Main game)
            : base(game)
        {
        }

        public override string Name => "AutoClear";
        public override string Author => "大豆子, Mute, 肝帝熙恩; TOFOUT & Clover";
        public override string Description => "Safely paces manual and automatic world-item cleanup.";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public override void Initialize()
        {
            configuration = AutoClearConfiguration.Load();
            scheduler = new AutoClearScheduler(
                configuration,
                IsStarverCleanupRunning);
            starverProtectionActive = HasStarverCleanupProtection();
            tshockClearCommand = FindTShockClearCommand();
            if (tshockClearCommand == null)
            {
                TShock.Log.ConsoleError(
                    "[AutoClear] Unable to locate TShock's built-in /clear command; use /safeclearitems instead.");
            }

            safeClearCommand = new Command(
                Permissions.clear,
                OnSafeClearItems,
                "safeclearitems",
                "sclearitems")
            {
                HelpText = "Safely clears world items in paced batches. Usage: /safeclearitems [radius]",
            };
            reloadCommand = new Command(
                Permissions.cfgreload,
                OnReloadConfiguration,
                "autoclearreload",
                "acreload")
            {
                HelpText = "Reloads AutoClear.json.",
            };

            Commands.ChatCommands.Add(safeClearCommand);
            Commands.ChatCommands.Add(reloadCommand);
            PlayerHooks.PlayerCommand += OnPlayerCommand;
            PlayerHooks.PrePlayerCommand += OnPrePlayerCommand;
            ServerApi.Hooks.GameUpdate.Register(this, OnGameUpdate);

            if (starverProtectionActive)
            {
                TShock.Log.ConsoleInfo(
                    "[AutoClear] Starver cleanup protection detected; /clear item remains delegated to Starver while automatic sweep stays available.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            ServerApi.Hooks.GameUpdate.Deregister(this, OnGameUpdate);
            PlayerHooks.PrePlayerCommand -= OnPrePlayerCommand;
            PlayerHooks.PlayerCommand -= OnPlayerCommand;
            if (safeClearCommand != null)
            {
                Commands.ChatCommands.Remove(safeClearCommand);
            }
            if (reloadCommand != null)
            {
                Commands.ChatCommands.Remove(reloadCommand);
            }

            AutoClearService.Reset();
            scheduler = null;
            configuration = null;
            reloadCommand = null;
            safeClearCommand = null;
            tshockClearCommand = null;
            base.Dispose(disposing);
        }

        private static Command FindTShockClearCommand()
        {
            return Commands.ChatCommands.FirstOrDefault(command =>
                string.Equals(command.Name, "clear", StringComparison.OrdinalIgnoreCase)
                && command.Permissions.Contains(Permissions.clear)
                && command.CommandDelegate?.Method.DeclaringType == typeof(Commands)
                && string.Equals(command.CommandDelegate.Method.Name, "Clear", StringComparison.Ordinal));
        }

        private void OnPrePlayerCommand(PrePlayerCommandEventArgs args)
        {
            if (HasStarverCleanupProtection()
                || !ReferenceEquals(args.Command, tshockClearCommand)
                || !AutoClearCommandRules.TryParseItemClearParameters(
                    args.Arguments.Parameters,
                    out int radiusTiles))
            {
                return;
            }

            args.Handled = true;
            StartCleanup(args.Arguments.Player, radiusTiles, args.Arguments.Silent);
        }

        private static void OnPlayerCommand(PlayerCommandEventArgs args)
        {
            if (!AutoClearService.IsRunning
                || !args.Player.HasPermission(Permissions.clear)
                || !string.Equals(args.CommandName, "clear", StringComparison.OrdinalIgnoreCase)
                || !AutoClearCommandRules.TryParseItemClearParameters(
                    args.Parameters,
                    out _))
            {
                return;
            }

            args.Handled = true;
            args.Player.SendWarningMessage(
                $"自动或安全物品清理正在进行，剩余 {AutoClearService.RemainingItems} 个物品，请稍后再试。");
        }

        private static void OnSafeClearItems(CommandArgs args)
        {
            if (!AutoClearCommandRules.TryParseSafeCommandParameters(
                args.Parameters,
                out int radiusTiles))
            {
                args.Player.SendErrorMessage("正确用法: /safeclearitems [半径]");
                return;
            }

            if (HasStarverCleanupProtection())
            {
                args.Player.SendWarningMessage("检测到 Starver 已内置相同保护，请使用 /clear item [半径]。");
                return;
            }

            StartCleanup(args.Player, radiusTiles, args.Silent);
        }

        private void OnReloadConfiguration(CommandArgs args)
        {
            if (args.Parameters.Count != 0)
            {
                args.Player.SendErrorMessage("正确用法: /autoclearreload");
                return;
            }

            configuration = AutoClearConfiguration.Load();
            scheduler.Reload(configuration);
            starverProtectionActive = HasStarverCleanupProtection();
            args.Player.SendSuccessMessage(
                $"AutoClear 配置已重载：自动清扫={(configuration.EnableAutomaticSweep ? "开启" : "关闭")}，" +
                $"阈值={configuration.SmartSweepThreshold}，检测间隔={configuration.DetectionIntervalSeconds}秒，" +
                $"延迟={configuration.DelayedSweepTimeoutSeconds}秒。");

            if (starverProtectionActive)
            {
                args.Player.SendInfoMessage("检测到 Starver 内置清理保护：/clear item 由 Starver 接管，自动清扫仍由本插件安全执行。");
            }
        }

        private static void StartCleanup(TSPlayer player, int radiusTiles, bool silent)
        {
            AutoClearStartResult result = AutoClearService.TryStart(
                player,
                radiusTiles,
                silent,
                out _);

            switch (result)
            {
                case AutoClearStartResult.Started:
                case AutoClearStartResult.NoMatchingItems:
                    return;

                case AutoClearStartResult.Busy:
                    player.SendWarningMessage(
                        $"已有世界物品清理任务正在进行，剩余 {AutoClearService.RemainingItems} 个物品，请稍后再试。");
                    return;

                case AutoClearStartResult.MainThreadRequired:
                    player.SendErrorMessage("世界物品安全清理只能在服务器主线程启动，本次请求已取消。");
                    return;

                default:
                    player.SendErrorMessage("无法启动世界物品安全清理任务。");
                    return;
            }
        }

        private static bool HasStarverCleanupProtection()
        {
            return ServerApi.Plugins.Any(container =>
                container.Initialized
                && container.Plugin?.GetType().Assembly.GetType(
                    "Starvers.WorldItemCleanupCoordinator",
                    throwOnError: false) != null);
        }

        private void OnGameUpdate(EventArgs args)
        {
            AutoClearService.Update();
            scheduler?.Update();
        }

        private static bool IsStarverCleanupRunning()
        {
            foreach (var container in ServerApi.Plugins)
            {
                if (!container.Initialized || container.Plugin == null)
                {
                    continue;
                }

                Type coordinatorType = container.Plugin.GetType().Assembly.GetType(
                    "Starvers.WorldItemCleanupCoordinator",
                    throwOnError: false);
                if (coordinatorType == null)
                {
                    continue;
                }

                object value = coordinatorType.GetProperty(
                    "IsRunning",
                    BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
                return value is bool isRunning && isRunning;
            }

            return false;
        }
    }
}
