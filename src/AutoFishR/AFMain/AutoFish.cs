using AutoFish.Data;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Projectile = HookEvents.Terraria.Projectile;

namespace AutoFish.AFMain;

[ApiVersion(2, 1)]
public partial class AutoFish : TerrariaPlugin
{
    public const string PermissionPrefix = "autofish.";
    public const string AdminPermission = $"{PermissionPrefix}admin";
    public const string CommonPermission = $"{PermissionPrefix}common";
    public const string DenyPermissionPrefix = $"{PermissionPrefix}no.";

    /// <summary>DEBUG 模式开关。</summary>
    internal static bool DebugMode = false;

    /// <summary>全局配置实例。</summary>
    internal static Configuration Config = new();

    /// <summary>玩家数据集合。</summary>
    internal static AFPlayerData PlayerData = new();

    /// <summary>
    ///     创建插件实例。
    /// </summary>
    public AutoFish(Main game) : base(game)
    {
    }

    /// <summary>插件名称。</summary>
    public override string Name => "自动钓鱼R";

    /// <summary>插件作者。</summary>
    public override string Author => "ksqeib 羽学 少司命";

    /// <summary>插件版本。</summary>
    public override Version Version => new(1, 4, 9,1);

    /// <summary>插件描述。</summary>
    public override string Description => "青山常伴绿水，燕雀已是南飞";

    /// <summary>
    ///     默认玩家数据工厂，基于当前配置初始化。
    /// </summary>
    internal static AFPlayerData.ItemData CreateDefaultPlayerData(string playerName)
    {
        // Attempt to resolve current player to seed defaults from permissions
        var player = TShock.Players.FirstOrDefault(p => p != null && p.Active &&
                                                        p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));

        var canBuff = HasFeaturePermission(player, "buff");
        var canMulti = HasFeaturePermission(player, "multihook");
        var canFish = HasFeaturePermission(player, "fish");
        var canBlockMonster = HasFeaturePermission(player, "filter.monster");
        var canSkipAnimation = HasFeaturePermission(player, "skipanimation");
        var canBlockQuestFish = HasFeaturePermission(player, "filter.quest");
        var canProtectBait = HasFeaturePermission(player, "bait.protect");

        var defaultAutoFish = Config.DefaultAutoFishEnabled && canFish;
        var defaultBuff = Config.DefaultBuffEnabled && canBuff;
        var defaultMulti = Config.DefaultMultiHookEnabled && canMulti;
        var defaultBlockMonster = Config.GlobalBlockMonsterCatch && Config.DefaultBlockMonsterCatch &&
                                  canBlockMonster;
        var defaultSkipAnimation = Config.GlobalSkipFishingAnimation && Config.DefaultSkipFishingAnimation &&
                                   canSkipAnimation;
        var defaultBlockQuestFish = Config.GlobalBlockQuestFish && Config.DefaultBlockQuestFish &&
                                    canBlockQuestFish;
        var defaultProtectBait = Config.GlobalProtectValuableBaitEnabled && Config.DefaultProtectValuableBaitEnabled &&
                                 canProtectBait;

        return new AFPlayerData.ItemData
        {
            Name = playerName,
            AutoFishEnabled = defaultAutoFish,
            BuffEnabled = defaultBuff,
            HookMaxNum = Config.GlobalMultiHookMaxNum,
            MultiHookEnabled = defaultMulti,
            BlockMonsterCatch = defaultBlockMonster,
            SkipFishingAnimation = defaultSkipAnimation,
            BlockQuestFish = defaultBlockQuestFish,
            ProtectValuableBaitEnabled = defaultProtectBait,
            FirstFishHintShown = false
        };
    }

    /// <summary>统一的权限检查，支持 admin 全覆盖、common 通用以及显式负权限。</summary>
    internal static bool HasFeaturePermission(TSPlayer? player, string featureKey, bool allowCommon = true)
    {
        if (player == null) return false;

        if (player.HasPermission(AdminPermission)) return true;

        var denyPermission = $"{DenyPermissionPrefix}{featureKey}";
        if (player.HasPermission(denyPermission)) return false;

        if (allowCommon && player.HasPermission(CommonPermission)) return true;

        var allowPermission = $"{PermissionPrefix}{featureKey}";
        return player.HasPermission(allowPermission);
    }

    /// <summary>
    ///     插件初始化，注册事件和命令。
    /// </summary>
    public override void Initialize()
    {
        LoadConfig();
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Hooks.GamePostInitialize.Register(this, OnGamePostInitialize);
        //容易出bug，还是OTAPI精准打击吧
        // ServerApi.Hooks.ProjectileAIUpdate.Register(this, ProjectAiUpdate);
        Projectile.AI_061_FishingBobber += OnAI_061_FishingBobber;
        TShockAPI.Commands.ChatCommands.Add(new Command(new List<string> { "autofish", CommonPermission },
            Commands.Afs, "af", "autofish"));
        TShockAPI.Commands.ChatCommands.Add(new Command(AdminPermission, Commands.Afa, "afa", "autofishadmin"));
    }

    /// <summary>
    ///     服务器初始化后显示插件状态信息。
    /// </summary>
    private void OnGamePostInitialize(EventArgs args)
    {
        // 检查 SSC 是否开启
        if (!Main.ServerSideCharacter)
        {
            TShock.Log.ConsoleError("========================================");
            TShock.Log.ConsoleError("[AutoFishR] 严重错误：SSC 未开启！");
            TShock.Log.ConsoleError("[AutoFishR] 本插件需要 ServerSideCharacter 才能正常运行");
            TShock.Log.ConsoleError("[AutoFishR] 请在 tshock/sscconfig.json 中启用 SSC");
            TShock.Log.ConsoleError("[AutoFishR] 若将此错误截图到群里，请群友代开发者问候截图者");
            TShock.Log.ConsoleError("[AutoFishR] 插件已自动禁用");
            TShock.Log.ConsoleError("========================================");

            // 注销所有钩子和命令，禁用插件
            Dispose(true);
            return;
        }

        if (!Configuration.IsFirstInstall) return;

        TShock.Log.ConsoleInfo("========================================");
        TShock.Log.ConsoleInfo($"[AutoFishR] 插件已成功加载 v{Version}");
        TShock.Log.ConsoleInfo("[AutoFishR] 当前状态：正常运行");
        TShock.Log.ConsoleInfo("========================================");
        TShock.Log.ConsoleInfo("[AutoFishR] 遇到 BUG 或问题？");
        TShock.Log.ConsoleInfo("[AutoFishR] 1. 请先查看 README 文档");
        TShock.Log.ConsoleInfo("[AutoFishR] 2. 无法解决再联系开发者");
        TShock.Log.ConsoleInfo("[AutoFishR] GitHub: https://github.com/ksqeib/AutoFishR");
        TShock.Log.ConsoleInfo("[AutoFishR] Star 很重要，是支持开发者持续开发的动力，欢迎点个 Star！");
        TShock.Log.ConsoleInfo("[AutoFishR] 本插件为免费开源插件，如有任何付费购买行为，说明您被骗了。");
        TShock.Log.ConsoleInfo("[AutoFishR] 联系方式：QQ 2388990095 (ksqeib)");
        TShock.Log.ConsoleInfo("[AutoFishR] 警告：请勿在群内艾特开发者！");
        TShock.Log.ConsoleInfo("========================================");
    }

    /// <summary>
    ///     释放插件，注销事件与命令。
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Hooks.GamePostInitialize.Deregister(this, OnGamePostInitialize);
            // ServerApi.Hooks.ProjectileAIUpdate.Deregister(this, ProjectAiUpdate);
            Projectile.AI_061_FishingBobber -= OnAI_061_FishingBobber;
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Afs);
            TShockAPI.Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == Commands.Afa);
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///     处理 /reload 触发的配置重载。
    /// </summary>
    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player.SendInfoMessage(Lang.T("reload.done"));
    }

    /// <summary>
    ///     读取并落盘配置。
    /// </summary>
    private static void LoadConfig()
    {
        Config = Configuration.Read();
        Lang.Load(Config.Language);
    }

}