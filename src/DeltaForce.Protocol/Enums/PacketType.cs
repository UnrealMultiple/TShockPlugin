namespace DeltaForce.Protocol.Enums;

public enum PacketType : byte
{
    None = 0,
    Unknown = 1,

    // 系统
    Request,
    Response,

    // 连接
    Connect,
    Disconnect,

    // 登录
    LoginRequest,
    LoginResponse,

    GameState,
    GameStateResponse,

    // 心跳
    Heartbeat,
    HeartbeatResponse,

    // 背包数据
    PlayerInventoryRequest,
    PlayerInventoryResponse,

    // 保存背包数据
    SaveInventoryRequest,
    SaveInventoryResponse,

    // 小队数据
    SquadDataRequest,
    SquadDataResponse,

    // 物品清单
    ItemListRequest,
    ItemListResponse,

    // 客户端标识
    ClientIdentity,
    ClientIdentityResponse,

    PlayerPositionRequest,
    PlayerPositionResponse,
}
