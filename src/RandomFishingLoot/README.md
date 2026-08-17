# RandomFishingLoot 随机渔获

- 作者: 愚蠢
- 版本: 2.5.0
- 简介: 按当前进度阶段替换钓到的物品

## 功能

- **progression_items 模式**: 按 Boss/进度阶段解锁渔获表，当前阶段越高钓到的东西越好，同时叠加常驻基础池
- **random_npcs 模式**: 钓鱼时随机生成一只可用的生物（Boss、友好生物、城镇 NPC 可选），支持配置替换概率
- 配置文件自动生成: 首次启动自动生成 `tshock/RandomFishingLoot.json`，包含完整默认渔获表和说明
- 热重载: 修改配置后执行 `/fishrand reload` 即时生效
- 保留原版任务鱼（渔夫任务）和宝匣，默认不被自定义渔获表覆盖
- 加载时自动校验条目: 跳过非法物品 ID、被屏蔽的物品以及直接战斗武器

## 命令

| 语法 | 别名 | 权限 | 说明 |
|------|------|------|------|
| `/fishrand` | | 无 | 查看当前模式、阶段与状态 |
| `/fishrand reload` | `/fishrand r` | fishrand.admin | 热重载配置文件 |
| `/fishrand sample <数量>` | `/fishrand preview` | 无 | 预览当前阶段随机渔获（默认 8 个，最大 20） |
| `/fishrand stages` | | 无 | 列出所有阶段及解锁状态 |
| `/fishrand stage <阶段ID>` | | 无 | 查看指定阶段的具体物品 |

## 配置

> 配置文件位于 `tshock/RandomFishingLoot.json`，首次启动自动生成。

| 配置项 | 默认值 | 说明 |
|--------|--------|------|
| Enabled | true | 总开关 |
| Mode | progression_items | `progression_items` 或 `random_npcs` |
| AnnounceToPlayer | false | 钓上时是否发送渔获提示 |
| AllowQuestFish | true | 保留原版渔夫任务鱼 |
| AllowCrates | true | 保留原版宝匣 |
| BlockedItemIds | [] | 屏蔽的物品 ID（加载时跳过） |
| RandomNpc.ReplaceChancePercent | 100 | random_npcs 模式替换概率 |
| RandomNpc.IncludeBosses | true | 是否包含 Boss |
| RandomNpc.IncludeFriendlyNPCs | true | 是否包含友好生物 |
| RandomNpc.IncludeTownNPCs | false | 是否包含城镇 NPC |
| RandomNpc.BlockedNpcIds | [] | 屏蔽的 NPC ID |
| AlwaysAvailable | 基础材料/药水池 | 常驻基础池，任何阶段都会叠加 |
| Stages | 10 个进度阶段 | 按 Boss 进度解锁的渔获表 |

### 阶段解锁条件（Stages[].Unlock）

| 配置项 | 说明 |
|--------|------|
| HardMode | 是否必须处于困难模式（true/false/null） |
| GameMode | 游戏模式（0 经典 / 1 专家 / 2 大师 / 3 旅途） |
| Defeated | 需要击败的 Boss 列表 |
| NotDefeated | 需要未击败的 Boss 列表 |

Boss 关键字: `eye`、`evilboss`、`skeletron`、`queenbee`、`wof`、`anymech`、`twins`、`destroyer`、`skeletronprime`、`allmechs`、`plantera`、`golem`、`cultist`、`moonlord`

### 条目字段（LootEntry）

| 字段 | 说明 |
|------|------|
| Name | 显示名（留空自动取原版物品名） |
| ItemId | Terraria ItemID |
| Weight | 抽取权重 |
| MinStack / MaxStack | 每次钓上来的数量区间 |

## 更新日志

- **2.5.0**
  - 拆分代码架构，按职责分文件组织
  - 配置文件自动生成并包含完整默认渔获表
