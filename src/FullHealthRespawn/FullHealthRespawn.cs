using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace FullHealthRespawn;

/// <summary>
/// FullHealthRespawn - 复活满血插件
/// 玩家复活时自动恢复到当前最大生命值
/// 对所有玩家开放，无需权限，无需配置
/// </summary>
[ApiVersion(2, 1)]
public class FullHealthRespawn : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "MiMo";
    public override string Description => GetString("复活满血");
    public override Version Version => new Version(1, 1, 0);

    public FullHealthRespawn(Main game) : base(game) { }

    /// <summary>
    /// 插件初始化 - 订阅事件
    /// </summary>
    public override void Initialize()
    {
        PlayerSpawn += this.OnSpawn!;
        ServerApi.Hooks.ServerJoin.Register(this, this.OnJoin);
    }

    /// <summary>
    /// 插件释放 - 取消事件订阅
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PlayerSpawn -= this.OnSpawn!;
            ServerApi.Hooks.ServerJoin.Deregister(this, this.OnJoin);
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// 玩家加入服务器事件
    /// </summary>
    private void OnJoin(JoinEventArgs args)
    {
        var plr = TShock.Players[args.Who];
        if (plr == null)
            return;

        // 进服时恢复
        Task.Delay(500).ContinueWith(_ =>
        {
            RestoreHealth(plr);
        });
    }

    /// <summary>
    /// 玩家出生/复活事件
    /// </summary>
    private void OnSpawn(object o, SpawnEventArgs args)
    {
        var plr = args.Player;

        if (plr == null || !plr.Active)
            return;

        // 延迟执行，避免与 SSC 冲突
        Task.Delay(500).ContinueWith(_ =>
        {
            RestoreHealth(plr);
        });
    }

    /// <summary>
    /// 恢复玩家生命值到当前最大值
    /// </summary>
    private void RestoreHealth(TSPlayer player)
    {
        if (player == null || !player.Active || player.TPlayer == null)
            return;

        if (!player.ConnectionAlive)
            return;

        // 使用玩家当前的最大生命值（已经通过生命水晶等提升过的）
        int maxLife = player.TPlayer.statLifeMax2;

        // 如果当前血量小于最大值，则恢复
        if (player.TPlayer.statLife < maxLife)
        {
            player.TPlayer.statLife = maxLife;

            // 同步生命值到客户端
            player.SendData(PacketTypes.PlayerHp, "", player.Index);
        }
    }
}
