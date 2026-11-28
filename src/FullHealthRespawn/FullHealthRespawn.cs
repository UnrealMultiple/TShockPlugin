using System.Collections.Concurrent;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using static TShockAPI.GetDataHandlers;

namespace FullHealthRespawn;

/// <summary>
/// FullHealthRespawn - 复活满血插件
/// 仅在玩家死亡后复活时，自动恢复到当前最大生命值
/// 进服与魔镜/海螺等回城不再触发（v1.2.1 起按 Spawn 上下文过滤）
/// 对所有玩家开放，无需权限，无需配置
/// </summary>
[ApiVersion(2, 1)]
public class FullHealthRespawn : TerrariaPlugin
{
    public override string Name => System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!;
    public override string Author => "不是现在";
    public override string Description => GetString("复活满血");
    public override Version Version => new Version(1, 2, 1);

    // 使用线程安全的队列存储待处理的玩家
    private readonly ConcurrentQueue<TSPlayer> _restoreQueue = new();

    public FullHealthRespawn(Main game) : base(game) { }

    /// <summary>
    /// 插件初始化 - 订阅事件
    /// </summary>
    public override void Initialize()
    {
        PlayerSpawn += this.OnSpawn!;
        ServerApi.Hooks.GameUpdate.Register(this, this.OnUpdate);
    }

    /// <summary>
    /// 插件释放 - 取消事件订阅
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PlayerSpawn -= this.OnSpawn!;
            ServerApi.Hooks.GameUpdate.Deregister(this, this.OnUpdate);
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// 游戏更新事件 - 从队列中取出并执行恢复
    /// </summary>
    private void OnUpdate(EventArgs args)
    {
        while (_restoreQueue.TryDequeue(out var player))
        {
            RestoreHealth(player);
        }
    }

    /// <summary>
    /// 玩家死亡复活事件
    /// 说明：原版魔镜/记忆药水/海螺等回城与死亡复活共用同一个 Spawn 包，
    /// 仅上下文不同（回城为 RecallFromItem），因此这里按上下文过滤，只处理死亡复活。
    /// </summary>
    private void OnSpawn(object o, SpawnEventArgs args)
    {
        // 只处理死亡复活；回城（RecallFromItem）、进服出生（SpawningIntoWorld）不触发
        if (args.SpawnContext != PlayerSpawnContext.ReviveFromDeath)
            return;

        var plr = args.Player;

        if (plr == null || !plr.Active)
            return;

        // 加入队列，等待 GameUpdate 处理
        _restoreQueue.Enqueue(plr);
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
