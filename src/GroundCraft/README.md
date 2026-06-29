# GroundCraft 地上合成

- 作者: 愚蠢
- 出处: 通用 TShock 插件，按 `UnrealMultiple/TShockPlugin` 仓库格式整理
- 插件名/关键词: GroundCraft、DropCraft、Ground Item Craft、地上合成、掉落物合成
- 功能: 玩家把掉落物丢在同一片位置，插件按 JSON 自定义配方自动合成新物品；不会读取或触发原版 Terraria 合成表

## 指令

| 语法 | 别名 | 权限 | 说明 |
|---|---|---|---|
| `/groundcraft` | `/gc` | `tshock.canchat` | 查看插件状态 |
| `/gcrecipes [页码/搜索]` | `/gcr` | `tshock.canchat` | 查看已启用配方 |
| `/gcenv` | 无 | `tshock.canchat` | 查看当前位置层级、生物群系、液体标签 |
| `/gcreload` | 无 | `groundcraft.admin` | 热重载配置和配方 JSON |
| `/gcaudit` | 无 | `groundcraft.admin` | 查看配方审核结果和运行统计 |

## 配置

配置目录：`tshock/GroundCraft/`

- `config.json`: 扫描间隔、合成半径、通知、残影清理、安全规则、权限
- `recipes.json`: 自定义地上合成表

`/gcreload` 会热重载两个 JSON 文件，不需要重启服务器。

```json
{
  "enabled": true,
  "scanIntervalTicks": 45,
  "requiredStableScans": 2,
  "clusterRadiusTiles": 3,
  "stillVelocitySquared": 0.02,
  "stationSearchRadiusTiles": 7,
  "environmentSearchRadiusTiles": 8,
  "biomePlayerProbeRadiusTiles": 40,
  "maxCraftsPerClusterPerScan": 1,
  "notifyRadiusTiles": 24,
  "notifyPlayers": true,
  "notifyConsumedItems": true,
  "clearClientGhostItems": true,
  "allowSingleIngredientTypeRecipes": false,
  "allowCoinRecipes": false,
  "allowInputOutputSameItem": false,
  "playerPermission": "tshock.canchat",
  "adminPermission": "groundcraft.admin"
}
```

## 配方 JSON

`requiredTiles` 使用 Terraria 图格 ID；空数组表示任意地点。`conditions` 可同时限制层级、生物群系、附近液体和 Boss 进度。

```json
{
  "version": 1,
  "recipes": [
    {
      "id": "cloud_in_sky",
      "enabled": true,
      "output": { "id": 751, "stack": 8, "name": "Cloud" },
      "ingredients": [
        { "id": 1518, "stack": 1, "name": "Feather" },
        { "id": 126, "stack": 1, "name": "Bottled Water" }
      ],
      "requiredTiles": [],
      "conditions": {
        "layers": [ "Sky" ],
        "biomes": [],
        "liquids": [],
        "anyLiquids": [],
        "bossProgress": {}
      }
    }
  ]
}
```

支持的常用条件：

- `layers`: `Sky`, `Surface`, `Underground`, `Cavern`, `Underworld`
- `biomes`: `Forest`, `Snow`, `Desert`, `Jungle`, `Corruption`, `Crimson`, `Hallow`, `Mushroom`, `Graveyard`, `Ocean`, `Shimmer`, `Dungeon`, `Hive`, `Temple`
- `liquids` / `anyLiquids`: `Water`, `Lava`, `Honey`, `Shimmer`
- `bossProgress.hardmode`: `true` 或 `false`
- `bossProgress.allDowned` / `anyDowned` / `noneDowned`: `KingSlime`, `EyeOfCthulhu`, `EaterOfWorldsOrBrain`, `QueenBee`, `Deerclops`, `Skeletron`, `WallOfFlesh`, `QueenSlime`, `MechBossAny`, `Destroyer`, `Twins`, `SkeletronPrime`, `MechBossAll`, `Plantera`, `Golem`, `DukeFishron`, `EmpressOfLight`, `LunaticCultist`, `MoonLord`

多数条件也支持中文别名，例如 `天空`、`地表`、`雪原`、`微光`、`克苏鲁之眼`、`肉山`、`世纪之花`。

## 安全规则

- 默认至少需要两类材料。
- 默认拒绝钱币配方。
- 默认拒绝材料和产物相同的循环配方。
- 同材料、同工作站、同条件但产物不同的配方会被拒绝，避免歧义。
- 合成时会清除被消耗的掉落物并同步消失包；若客户端仍短暂显示旧材料，会提示玩家那是残影，实际已不存在。

## 更新日志

### v1.0.0

- 初版：JSON 配方、JSON 条件、地形/环境判断、Boss 进度判断、热重载、配方审核、旧掉落物残影清理。

## 反馈

- 优先发 issue 到共同维护插件库：https://github.com/UnrealMultiple/TShockPlugin
