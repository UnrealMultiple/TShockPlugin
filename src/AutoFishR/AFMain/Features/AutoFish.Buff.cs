using AutoFish.Utils;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    /// <summary>
    ///     为满足条件的玩家在钓鱼时施加 Buff。
    /// </summary>
    public void BuffUpdate(object sender, GetDataHandlers.NewProjectileEventArgs args)
    {
        var player = args.Player;

        if (player == null) return;
        if (!player.Active) return;
        if (!player.IsLoggedIn) return;
        if (!Config.PluginEnabled) return;
        if (!Config.GlobalBuffFeatureEnabled) return;

        // 从数据表中获取与玩家名字匹配的配置项
        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (!playerData.BuffEnabled) return;

        //出现鱼钩摆动就给玩家施加buff
        if (!Tools.BobbersActive(args.Owner)) return;

        foreach (var buff in Config.BuffDurations)
            player.SetBuff(buff.Key, buff.Value);
    }
}