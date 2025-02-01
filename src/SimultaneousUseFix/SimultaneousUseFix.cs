using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace fixbugpe;

[ApiVersion(2, 1)]
public class SimultaneousUseFix : TerrariaPlugin
{
    public override string Author => "熙恩，感谢恋恋";
    public override string Description => GetString("解决卡双锤，卡星旋机枪之类的问题");
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override Version Version => new Version(1, 0, 9);
    public static Configuration Config = null!;
    public bool otherPluginExists = false;

    public SimultaneousUseFix(Main game) : base(game)
    {
    }

    private static void LoadConfig()
    {
        Config = Configuration.Read(Configuration.FilePath);
        Config.Write(Configuration.FilePath);
    }

    private static void ReloadConfig(ReloadEventArgs args)
    {
        LoadConfig();
        args.Player?.SendSuccessMessage(GetString("[{0}] 重新加载配置完毕。"), nameof(SimultaneousUseFix));
    }


    public override void Initialize()
    {
        GeneralHooks.ReloadEvent += ReloadConfig;
        ServerApi.Plugins.Get<Chireiden.TShock.Omni.Plugin>()!.Detections.SwapWhileUse += this.OnSwapWhileUse;
        LoadConfig();
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            GeneralHooks.ReloadEvent -= ReloadConfig;
            ServerApi.Plugins.Get<Chireiden.TShock.Omni.Plugin>()!.Detections.SwapWhileUse -= this.OnSwapWhileUse;
        }

        base.Dispose(disposing);
    }
    private void OnSwapWhileUse(int playerId, int slot)
    {
        var player = TShock.Players[playerId];

        if (player != null && !player.HasPermission("SimultaneousUseFix"))
        {
            var item = player.TPlayer.inventory[slot];

            if (item != null && Config.ExemptItemList.Contains(item.type))
            {
                return;
            }

            if (Config.KickPlayerOnUse)
            {
                TShock.Utils.Broadcast(GetString($"玩家 {player.Name} 因为卡换格子bug被踢出"), Color.Green);
                player.Kick(GetString("因为卡换格子bug被踢出"));
            }

            if (Config.KillPlayerOnUse)
            {
                TShock.Utils.Broadcast(GetString($"玩家 {player.Name} 因为卡换格子bug被杀死"), Color.Green);
                player.KillPlayer();
            }

            if (Config.ApplyBuffOnUse)
            {
                TShock.Utils.Broadcast(GetString($"玩家 {player.Name} 因为卡换格子bug被上buff"), Color.Green);
                foreach (var buffType in Config.BuffTypes)
                {
                    player.SetBuff(buffType, Config.Bufftime);
                }
            }

        }
    }
}