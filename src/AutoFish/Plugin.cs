using AutoFishR;
using LazyAPI;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Projectile = HookEvents.Terraria.Projectile;

namespace AutoFish;

[ApiVersion(2, 1)]
public partial class Plugin(Main game) : LazyPlugin(game)
{
    public const string AdminPermission = $"autofish.admin";
    public const string CommonPermission = $"autofish.common";
    public const string DenyPermissionPrefix = $"autofish.no.";

    /// <summary>DEBUG 模式开关。</summary>
    internal static bool DebugMode = false;

    /// <summary>玩家数据集合。</summary>
    internal static AFPlayerData PlayerData = new();

    /// <summary>插件名称。</summary>
    public override string Name => "自动钓鱼R";

    /// <summary>插件作者。</summary>
    public override string Author => "ksqeib 羽学 少司命";

    /// <summary>插件版本。</summary>
    public override Version Version => new(1, 4, 9, 1);

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

        var defaultAutoFish = Configuration.Instance.DefaultAutoFishEnabled && canFish;
        var defaultBuff = Configuration.Instance.DefaultBuffEnabled && canBuff;
        var defaultMulti = Configuration.Instance.DefaultMultiHookEnabled && canMulti;
        var defaultBlockMonster = Configuration.Instance.GlobalBlockMonsterCatch && Configuration.Instance.DefaultBlockMonsterCatch &&
                                  canBlockMonster;
        var defaultSkipAnimation = Configuration.Instance.GlobalSkipFishingAnimation && Configuration.Instance.DefaultSkipFishingAnimation &&
                                   canSkipAnimation;
        var defaultBlockQuestFish = Configuration.Instance.GlobalBlockQuestFish && Configuration.Instance.DefaultBlockQuestFish &&
                                    canBlockQuestFish;
        var defaultProtectBait = Configuration.Instance.GlobalProtectValuableBaitEnabled && Configuration.Instance.DefaultProtectValuableBaitEnabled &&
                                 canProtectBait;

        return new AFPlayerData.ItemData
        {
            Name = playerName,
            AutoFishEnabled = defaultAutoFish,
            BuffEnabled = defaultBuff,
            HookMaxNum = Configuration.Instance.GlobalMultiHookMaxNum,
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

        var allowPermission = $"autofish.{featureKey}";
        return player.HasPermission(allowPermission);
    }

    /// <summary>
    ///     插件初始化，注册事件和命令。
    /// </summary>
    public override void Initialize()
    {
        ServerApi.Hooks.GamePostInitialize.Register(this, this.OnGamePostInitialize);
        //容易出bug，还是OTAPI精准打击吧
        // ServerApi.Hooks.ProjectileAIUpdate.Register(this, ProjectAiUpdate);
        Projectile.AI_061_FishingBobber += this.OnAI_061_FishingBobber;
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
            return;
        }
    }

    /// <summary>
    ///     释放插件，注销事件与命令。
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.GamePostInitialize.Deregister(this, this.OnGamePostInitialize);
            // ServerApi.Hooks.ProjectileAIUpdate.Deregister(this, ProjectAiUpdate);
            Projectile.AI_061_FishingBobber -= this.OnAI_061_FishingBobber;
        }

        base.Dispose(disposing);
    }

}