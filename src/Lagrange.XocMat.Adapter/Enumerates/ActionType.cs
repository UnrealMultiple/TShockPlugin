

namespace Lagrange.XocMat.Adapter.Enumerates;

public enum ActionType
{
    /// <summary>
    /// 查询进度
    /// </summary>
    GameProgress,

    /// <summary>
    /// 生成地图
    /// </summary>
    WorldMap,

    /// <summary>
    /// 公共消息
    /// </summary>
    PluginMsg,

    /// <summary>
    /// 似有消息
    /// </summary>
    PrivateMsg,

    /// <summary>
    /// 执行指令
    /// </summary>
    Command,
    /// <summary>
    /// 在线排行
    /// </summary>
    OnlineRank,

    /// <summary>
    /// 死亡排行
    /// </summary>
    DeadRank,

    /// <summary>
    /// 背包查询
    /// </summary>
    Inventory,

    /// <summary>
    /// 服务器在线玩家查询
    /// </summary>
    ServerOnline,

    /// <summary>
    /// 注册账户
    /// </summary>
    RegisterAccount,

    /// <summary>
    /// 服务器重置
    /// </summary>
    ResetServer,

    /// <summary>
    /// 上传世界地图
    /// </summary>
    UpLoadWorld,

    /// <summary>
    /// 重启服务器
    /// </summary>
    ReStartServer,

    /// <summary>
    /// 服务器状态
    /// </summary>
    ServerStatus,

    /// <summary>
    /// 重置玩家密码
    /// </summary>
    ResetPassword,

    /// <summary>
    /// 连接状态
    /// </summary>
    ConnectStatus,

    /// <summary>
    /// 获取玩家账号信息
    /// </summary>
    Account,

    /// <summary>
    /// 玩家伤害BOSS记录
    /// </summary>
    PlayerStrikeBoss,

    /// <summary>
    /// 导出玩家存档
    /// </summary>
    ExportPlayer
}
