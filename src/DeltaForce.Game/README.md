# DeltaForce.Game - 三角洲行动游戏服务器

> ⚠️ **重要说明：此插件必须配合 [维度(RealmNexus)](https://github.com/UnrealMultiple/RealmNexus) 使用否则无法传送玩家！**

三角洲行动的游戏玩法插件，负责实际的游戏逻辑判定，包括死亡掉落、出生点传送、撤离点系统和箱子物品分配。

## 功能特性

### 1. 出生点系统
- 游戏开始时为每个小队分配随机出生点
- 支持配置文件中定义多个出生点
- 可为特定小队设置专属出生点
- 在出生点范围内随机偏移传送

### 2. 死亡掉落系统
- 玩家死亡时掉落所有装备
- 死亡后变为幽灵状态
- 装备掉落在死亡位置

### 3. 撤离点系统
- 配置文件中可定义多个撤离点
- 玩家需在撤离点范围内停留指定时间
- 撤离成功后保存装备并返回特勤处
- 撤离倒计时提示

### 4. 箱子物品分配
- 游戏开始时自动重置所有箱子
- 根据物品权重随机分配物品
- 每个箱子随机放置 1-6 个物品

### 5. 游戏时间管理
- 游戏倒计时提示（5分钟、3分钟、1分钟、30秒、最后10秒）
- 游戏结束后未撤离玩家丢失装备

### 6. 玩家主动离开
- 支持玩家使用指令主动离开游戏
- 存活玩家离开需丢弃所有装备
- 死亡玩家可直接离开

## 指令列表

| 指令 | 权限 | 说明 |
|------|------|------|
| `/df leave` | deltaforce.game | 主动离开游戏返回特勤处 |
| `/df time` | deltaforce.game | 查看剩余游戏时间 |
| `/df evac` | deltaforce.game | 查看撤离点列表 |

## 配置文件

配置文件路径：`tshock/DeltaGame.json`

```json
{
  "socket_server": {
    "address": "127.0.0.1",
    "port": 7778
  },
  "core_server": {
    "address": "127.0.0.1",
    "port": 7777
  },
  "match_minute": 10,
  "ready_second": 30,
  "spawn_points": [
    {
      "name": "北部出生点",
      "x": 100,
      "y": 200,
      "team_id": 1
    },
    {
      "name": "南部出生点",
      "x": 500,
      "y": 300,
      "team_id": 2
    },
    {
      "name": "随机出生点",
      "x": 300,
      "y": 400
    }
  ],
  "spawn_range": 40,
  "evacuation_points": [
    {
      "name": "主撤离点",
      "x": 800,
      "y": 600,
      "radius": 5,
      "is_active": true
    },
    {
      "name": "备用撤离点",
      "x": 200,
      "y": 700,
      "radius": 4,
      "is_active": true
    }
  ],
  "evacuation_time_seconds": 10
}
```

### 配置项说明

- `socket_server`: 通信服务器配置，用于与特勤处通信
- `core_server`: 特勤处服务器地址，玩家撤离后将被传送回此服务器
- `match_minute`: 游戏时长（分钟）
- `ready_second`: 准备时间（秒）
- `spawn_points`: 出生点配置
  - `name`: 出生点名称
  - `x`, `y`: 出生点坐标
  - `team_id`: 专属小队ID（可选，null表示通用出生点）
- `spawn_range`: 出生点随机偏移范围
- `evacuation_points`: 撤离点配置
  - `name`: 撤离点名称
  - `x`, `y`: 撤离点坐标
  - `radius`: 撤离点有效范围（格）
  - `is_active`: 是否激活
- `evacuation_time_seconds`: 撤离所需时间（秒）

## 游戏流程

1. 特勤处匹配玩家，通过协议通知游戏服务器开始游戏
2. 游戏服务器接收游戏开始信号
3. 分配物品到地图中的所有箱子
4. 为每个小队分配出生点并传送玩家
5. 玩家在游戏中搜刮装备
6. 玩家到达撤离点，停留指定时间后成功撤离
7. 撤离成功的玩家保存装备并返回特勤处
8. 游戏时间结束，未撤离的玩家丢失装备并返回特勤处

## 通信协议

游戏服务器通过 TCP 协议与特勤处通信，主要数据包：

- `GameStatePacket`: 游戏状态同步
- `ItemListRequest/Response`: 物品列表请求
- `SquadDataRequest/Response`: 小队数据请求
- `PlayerInventoryRequest/Response`: 玩家背包数据请求
- `SaveInventoryRequest/Response`: 保存背包数据

## 依赖

- TShock 5.x
- LazyAPI (同项目)
- DeltaForce.Protocol (同项目)
- SSC (服务器端角色)

## 权限节点

- `deltaforce.game` - 使用游戏指令

## 作者

少司命
