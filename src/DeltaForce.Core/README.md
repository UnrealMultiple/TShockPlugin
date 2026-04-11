# DeltaForce.Core - 三角洲行动特勤处

> ⚠️ **重要说明：此插件必须配合 [维度(RealmNexus)](https://github.com/UnrealMultiple/RealmNexus) 使用否则无法传送玩家！**

三角洲行动的核心插件，负责玩家匹配、装备管理、交易行和哈夫币系统。

## 功能特性

### 1. 小队匹配系统
- 自动将玩家分配到小队（每队最多3人）
- 匹配倒计时提示（60秒、30秒、最后10秒）
- 匹配成功后自动传送到游戏服务器

### 2. 背包数据管理
- 自动保存玩家背包数据到数据库
- 支持从游戏服务器同步背包数据
- 玩家进入服务器时自动恢复背包

### 3. 哈夫币系统
- 新玩家初始获得 1000 哈夫币
- 支持玩家间转账
- 数据持久化到数据库

### 4. 交易行系统
- 玩家可以上架物品出售
- 支持搜索和浏览物品
- 自动处理交易和货币转移
- 支持直接出售物品给系统

## 指令列表

### 匹配指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/squad` | deltaforce.squad | 加入匹配队列 |

### 交易行指令
| 指令 | 权限 | 说明 |
|------|------|------|
| `/market balance` | deltaforce.market | 查看哈夫币余额 |
| `/market list [页码]` | deltaforce.market | 查看上架物品列表 |
| `/market sell <价格> [数量]` | deltaforce.market | 上架手持物品到交易行 |
| `/market sellnpc [数量]` | deltaforce.market | 直接出售手持物品给系统 |
| `/market buy <ID>` | deltaforce.market | 购买物品 |
| `/market cancel <ID>` | deltaforce.market | 取消自己上架的物品 |
| `/market my` | deltaforce.market | 查看自己上架的物品 |
| `/market search <关键词>` | deltaforce.market | 搜索物品 |
| `/market pay <玩家> <金额>` | deltaforce.market | 转账哈夫币给其他玩家 |

## 配置文件

配置文件路径：`tshock/DeltaCore.json`

```json
{
  "socket_server": {
    "address": "0.0.0.0",
    "port": 7778
  },
  "game_server": {
    "address": "127.0.0.1",
    "port": 7777
  },
  "match_seconds": 60,
  "items": [
    {
      "name": "金币",
      "type": 73,
      "weight": 20,
      "value": 10000
    }
  ]
}
```

### 配置项说明

- `socket_server`: 通信服务器配置，用于与游戏服务器通信
- `game_server`: 游戏服务器地址，匹配成功后玩家将被传送到此服务器
- `match_seconds`: 匹配等待时间（秒）
- `items`: 物品配置列表
  - `name`: 物品名称
  - `type`: 物品类型ID
  - `weight`: 物品权重（用于游戏内箱子分配）
  - `value`: 物品价值（哈夫币）

## 数据库表

插件会自动创建以下数据库表：

- `deltaforce_inventory`: 玩家背包数据
- `deltaforce_currency`: 玩家哈夫币数据
- `deltaforce_marketplace`: 交易行物品数据

## 依赖

- TShock 5.x
- LazyAPI (同项目)
- DeltaForce.Protocol (同项目)

## 权限节点

- `deltaforce.squad` - 使用匹配指令
- `deltaforce.market` - 使用交易行指令

## 作者

少司命
