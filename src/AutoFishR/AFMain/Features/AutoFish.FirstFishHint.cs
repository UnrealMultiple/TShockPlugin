using Terraria;
using TShockAPI;

namespace AutoFish.AFMain;

public partial class AutoFish
{
    /// <summary>
    ///     在玩家首次抛竿时提示可使用 /af 开启自动钓鱼（仅提示一次）。
    /// </summary>
    public void FirstFishHint(object? sender, GetDataHandlers.NewProjectileEventArgs args)
    {
        if (!Config.PluginEnabled) return;
        if (!Config.GlobalAutoFishFeatureEnabled) return;

        var player = args.Player;
        if (player == null || !player.Active || !player.IsLoggedIn) return;

        var projectile = Main.projectile[args.Index];
        if (!projectile.bobber) return;

        if (!HasFeaturePermission(player, "fish")) return;

        var playerData = PlayerData.GetOrCreatePlayerData(player.Name, CreateDefaultPlayerData);
        if (playerData.FirstFishHintShown) return;
        if (playerData.AutoFishEnabled) return;

        playerData.FirstFishHintShown = true;
        player.SendInfoMessage(Lang.T("firstFishHint"));
    }
}