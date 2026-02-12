using AutoFish.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    /// <summary>
    ///     处理多线钓鱼，派生额外的鱼线弹幕。
    /// </summary>
    public void AddMultiHook(TSPlayer player, Projectile oldHook, Vector2 pos)
    {
        if (!Config.GlobalMultiHookFeatureEnabled) return;

        // 从数据表中获取与玩家名字匹配的配置项
        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (!playerData.MultiHookEnabled) return;

        var hookCount = Main.projectile.Count(p => p.active && p.owner == oldHook.owner && p.bobber); // 浮漂计数
        //数量检测
        if (hookCount > Config.GlobalMultiHookMaxNum - 1) return;
        if (hookCount > playerData.HookMaxNum - 1) return;

        var guid = Guid.NewGuid().ToString();
        SpawnHook(player, oldHook, pos, guid);
    }
}