using AutoFish.Utils;
using Terraria;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    /// <summary>
    ///     处理多线钓鱼，派生额外的鱼线弹幕。
    /// </summary>
    public void ProjectNew(object sender, GetDataHandlers.NewProjectileEventArgs args)
    {
        var player = args.Player;
        var guid = Guid.NewGuid().ToString();
        var hookCount = Main.projectile.Count(p => p.active && p.owner == args.Owner && p.bobber); // 浮漂计数

        if (player == null) return;
        if (!player.Active) return;
        if (!player.IsLoggedIn) return;
        if (!Config.PluginEnabled) return;
        if (!Config.GlobalMultiHookFeatureEnabled) return;
        if (hookCount > Config.GlobalMultiHookMaxNum - 1) return;

        // 从数据表中获取与玩家名字匹配的配置项
        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (!playerData.MultiHookEnabled) return;

        // 正常状态下与消耗模式下启用多线钓鱼
        if (Config.GlobalConsumptionModeEnabled && !playerData.ConsumptionEnabled) return;
        // 检查是否上钩
        if (!Tools.BobbersActive(args.Owner)) return;

        var index = SpawnProjectile.NewProjectile(Main.projectile[args.Index].GetProjectileSource_FromThis(),
            args.Position, args.Velocity, args.Type, args.Damage, args.Knockback, args.Owner, 0, 0, 0, -1, guid);
        player.SendData(PacketTypes.ProjectileNew, "", index);
    }
}