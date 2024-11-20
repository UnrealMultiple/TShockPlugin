namespace Lagrange.XocMat.Adapter.Enumerates;

public enum PostMessageType
{
    /// <summary>
    /// 执行方法
    /// </summary>
    Action,

    /// <summary>
    /// 服务器初始化完成
    /// </summary>
    GamePostInit,

    /// <summary>
    /// 玩家进入服务器
    /// </summary>
    PlayerJoin,

    /// <summary>
    /// 玩家离开服务器
    /// </summary>
    PlayerLeave,

    /// <summary>
    /// 玩家消息
    /// </summary>
    PlayerMessage,

    /// <summary>
    /// 玩家指令
    /// </summary>
    PlayerCommand,

    /// <summary>
    /// 心跳包
    /// </summary>
    HeartBeat,

    /// <summary>
    /// 连接Socket
    /// </summary>
    Connect
}

