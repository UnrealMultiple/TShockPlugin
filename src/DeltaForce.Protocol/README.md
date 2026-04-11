# DeltaForce.Protocol - 三角洲行动通信协议

三角洲行动的通信协议库，用于特勤处服务器和游戏服务器之间的通信。

## 概述

本协议库定义了特勤处（Core）和游戏服务器（Game）之间的所有通信数据包格式和处理逻辑。

## 协议类型

### 数据包类型 (PacketType)

```csharp
public enum PacketType : byte
{
    // 系统
    Request, Response,
    
    // 连接
    Connect, Disconnect,
    
    // 游戏状态
    GameState, GameStateResponse,
    
    // 心跳
    Heartbeat, HeartbeatResponse,
    
    // 背包数据
    PlayerInventoryRequest, PlayerInventoryResponse,
    SaveInventoryRequest, SaveInventoryResponse,
    
    // 小队数据
    SquadDataRequest, SquadDataResponse,
    
    // 物品清单
    ItemListRequest, ItemListResponse,
    
    // 客户端标识
    ClientIdentity, ClientIdentityResponse,
    
    // 玩家位置
    PlayerPositionRequest, PlayerPositionResponse,
}
```

## 主要数据包

### 1. 游戏状态包 (GameStatePacket)

用于特勤处通知游戏服务器游戏开始或结束。

```csharp
public class GameStatePacket : IRequestPacket
{
    public PacketType PacketID => PacketType.GameState;
    public Guid RequestId { get; set; }
    public bool IsGameActive { get; set; }  // 游戏是否进行中
    public int GameMode { get; set; }        // 游戏模式
    public int GameTime { get; set; }        // 游戏时间
}
```

### 2. 物品列表包 (ItemListPacket)

游戏服务器请求特勤处的物品配置列表。

```csharp
public class ItemListRequestPacket : IRequestPacket
public class ItemListResponsePacket : IResponsePacket
{
    public ItemInfo[] Items { get; set; }
    public int TotalCount { get; set; }
}

public class ItemInfo
{
    public string Name { get; set; }    // 物品名称
    public int Type { get; set; }       // 物品类型ID
    public int Weight { get; set; }     // 物品权重
    public long Value { get; set; }     // 物品价值（哈夫币）
}
```

### 3. 小队数据包 (SquadDataPacket)

游戏服务器请求当前匹配的小队数据。

```csharp
public class SquadDataRequestPacket : IRequestPacket
public class SquadDataResponsePacket : IResponsePacket
{
    public SquadInfo[] Squads { get; set; }
}

public class SquadInfo
{
    public int SquadId { get; set; }           // 小队ID
    public SquadMemberInfo[] Members { get; set; }  // 小队成员
}

public class SquadMemberInfo
{
    public string PlayerName { get; set; }     // 玩家名称
    public int Team { get; set; }              // 队伍ID
}
```

### 4. 背包数据包 (PlayerInventoryPacket)

用于同步玩家背包数据。

```csharp
public class PlayerInventoryRequestPacket : IRequestPacket
{
    public string PlayerName { get; set; }
}

public class PlayerInventoryResponsePacket : IResponsePacket
{
    public string PlayerName { get; set; }
    public ItemData[] Items { get; set; }
}

public class ItemData
{
    public int Type { get; set; }      // 物品类型
    public int Stack { get; set; }     // 数量
    public byte Prefix { get; set; }   // 前缀
}
```

### 5. 保存背包包 (SaveInventoryPacket)

游戏服务器主动保存玩家背包数据到特勤处。

```csharp
public class SaveInventoryRequestPacket : IRequestPacket
{
    public string PlayerName { get; set; }
    public string Inventory { get; set; }      // 序列化的背包数据
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    // ... 其他玩家属性
}

public class SaveInventoryResponsePacket : IResponsePacket
{
    public string PlayerName { get; set; }
    public DateTime SavedAt { get; set; }
}
```

### 6. 心跳包 (HeartbeatPacket)

用于维持连接和检测断开。

```csharp
public class HeartbeatPacket : IRequestPacket
public class HeartbeatResponsePacket : IResponsePacket
```

### 7. 客户端身份包 (ClientIdentityPacket)

游戏服务器连接后发送身份验证。

```csharp
public class ClientIdentityPacket : INetPacket
{
    public Guid ClientId { get; set; }
    public string ClientName { get; set; }
}
```

## 序列化

协议使用二进制序列化，支持以下数据类型：

- 基础类型：byte, short, int, long, bool, float, double
- 字符串：UTF-8 编码
- 数组：支持任意类型的数组
- 复杂对象：支持嵌套对象

### 序列化特性

```csharp
[PacketType(PacketType.GameState)]  // 指定数据包类型
public class GameStatePacket : IRequestPacket
{
    public Guid RequestId { get; set; }  // 自动序列化
    public bool IsGameActive { get; set; }
}
```

## 处理器

### 请求处理器基类

```csharp
public abstract class RequestHandlerBase<TRequest, TResponse> 
    where TRequest : IRequestPacket
    where TResponse : IResponsePacket, new()
{
    public abstract TResponse Handle(TRequest request);
    
    protected TResponse CreateSuccessResponse(TRequest request, string message);
    protected TResponse CreateFailureResponse(TRequest request, string message);
}
```

### 处理器注册

```csharp
var processor = new PacketProcessor();
processor.RegisterHandler<GameStateHandler>();
processor.RegisterHandlersFromAssembly(Assembly.GetExecutingAssembly());
```

## 使用示例

### 发送请求

```csharp
var request = new ItemListRequestPacket();
var response = await client.RequestAsync<ItemListRequestPacket, ItemListResponsePacket>(request);
```

### 处理请求

```csharp
public class ItemListHandler : RequestHandlerBase<ItemListRequestPacket, ItemListResponsePacket>
{
    public override ItemListResponsePacket Handle(ItemListRequestPacket request)
    {
        var items = GetItemsFromConfig();
        
        var response = CreateSuccessResponse(request, "Success");
        response.Items = items;
        response.TotalCount = items.Length;
        return response;
    }
}
```

## 依赖

- .NET 9.0
- 无外部依赖

## 作者

少司命
