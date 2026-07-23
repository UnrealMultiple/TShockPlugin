# WorldEdit 世界编辑

- 作者: Nyx Studios、Anzhelika，肝帝熙恩适配 Terraria 1.4.5.6
- 出处: [radishes/WorldEdit](https://github.com/radishes/WorldEdit)
- 适用版本: TShock 6.1.0 / Terraria 1.4.5.6

用于批量编辑世界的管理插件。支持选区、方块与墙体编辑、复制粘贴、撤销重做、原理图以及条件表达式，并可正确复制箱子、物品框、物品瓶和展示人偶等实体数据。

## 基本用法

先使用 `//p1` 和 `//p2` 设置选区的两个端点，再执行编辑指令。不填写坐标时，输入指令后挖掘或敲击目标方块即可选点。

```text
//p1
//p2
//set stone
```

大部分编辑指令支持在末尾添加 `=> 条件表达式`，只处理满足条件的方块。

## 指令

### 选区

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//p1 [X Y]` | `worldedit.selection.point` | 设置选区第一点；省略坐标后通过挖掘或敲击选点 |
| `//p2 [X Y]` | `worldedit.selection.point` | 设置选区第二点；省略坐标后通过挖掘或敲击选点 |
| `//all` | `worldedit.selection.all` | 选中整个世界 |
| `//near <半径>` | `worldedit.selection.near` | 以玩家为中心创建选区 |
| `//region [区域名]` | `worldedit.selection.region` | 选中 TShock 区域；省略名称后通过敲击选择 |
| `//select <类型>` | `worldedit.selection.selecttype` | 设置选区类型；使用 `//select help` 查看类型 |
| `//resize <方向> <数量>` | `worldedit.selection.resize` | 从指定方向扩大或缩小选区；方向可组合 |
| `//shift <方向> <数量>` | `worldedit.selection.shift` | 按指定方向平移选区；方向可组合 |

选区类型包括 `normal`、`border`、`outline`、`ellipse`、`checkers`、`altcheckers` 和 `random`。

`//resize` 与 `//shift` 的方向：`u` 上、`d` 下、`l` 左、`r` 右；可组合使用，例如 `//resize rd 10` 向右和向下各扩展 10 格。

### 区域编辑

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//set <方块 ID/英文名> [=> 条件]` | `worldedit.region.set` | 设置选区内的方块 |
| `//replace <原方块> <新方块> [=> 条件]` | `worldedit.region.replace` | 替换方块 |
| `//fill <方块 ID/英文名> [=> 条件]` | `worldedit.region.fill` | 填充封闭区域 |
| `//setwall <墙 ID/英文名> [=> 条件]` | `worldedit.region.setwall` | 设置墙体 |
| `//replacewall <原墙> <新墙> [=> 条件]` | `worldedit.region.replacewall` | 替换墙体 |
| `//fillwall <墙 ID/英文名> [=> 条件]` | `worldedit.region.fillwall` | 填充封闭区域的墙体 |
| `//paint <颜色英文名/ID> [=> 条件]` | `worldedit.region.paint` | 为方块上色；颜色 ID 范围 `0`–`31` |
| `//paintwall <颜色英文名/ID> [=> 条件]` | `worldedit.region.paintwall` | 为墙体上色；颜色 ID 范围 `0`–`31` |
| `//setwire <1-4> <on/off> [=> 条件]` | `worldedit.region.setwire` | 设置指定颜色的电线 |
| `//actuator <on/off> [=> 条件]` | `worldedit.region.actuator` | 设置执行器 |
| `//inactive <on/off/reverse> [=> 条件]` | `worldedit.selection.inactive` | 设置或反转方块虚化状态 |
| `//slope <类型> [=> 条件]` | `worldedit.region.slope` | 设置方块坡度 |
| `//delslope [类型] [=> 条件]` | `worldedit.region.delslope` | 删除指定坡度的方块 |
| `//smooth [=> 条件]` | `worldedit.region.smooth` | 平滑选区地形 |
| `//setgrass <生态> [=> 条件]` | `worldedit.region.setgrass` | 转换草地生态 |
| `//biome <原生态> <新生态>` | `worldedit.region.biome` | 转换选区生态群落 |
| `//move <向右距离> <向下距离> [=> 条件]` | `worldedit.region.move` | 移动选区内容 |
| `//outline <方块 ID/英文名> <颜色英文名/ID> <active/nactive> [=> 条件]` | `worldedit.selection.outline` | 创建方块轮廓；`active` 正常、`nactive`/`na` 虚化 |
| `//outlinewall <墙 ID/英文名> <颜色英文名/ID> [=> 条件]` | `worldedit.selection.outlinewall` | 创建墙体轮廓 |
| `//shape[wall][fill] <形状> [旋转] [翻转] <方块/墙> [=> 条件]` | `worldedit.region.shape` | 绘制直线、矩形、椭圆或三角形；三角形可指定方向 |
| `//text <文本>` | `worldedit.selection.text` | 在选区中绘制文字；使用 `\n` 换行 |

方块和墙体名称仅支持英文。名称会先精确匹配；没有精确命中时按前缀匹配。若提示 `More than one tile matched!`（如 `wood`），请使用完整英文名称或数字 ID，例如普通木块为 `30`。

`//paint`、`//paintwall`、`//outline` 与 `//outlinewall` 的颜色支持英文油漆名或颜色 ID；中文油漆名不支持，颜色 ID 不是油漆物品 ID。

| ID / 颜色 | ID / 颜色 | ID / 颜色 | ID / 颜色 |
| --- | --- | --- | --- |
| `0` 无漆 | `1` 红 | `2` 橙 | `3` 黄 |
| `4` 黄绿 | `5` 绿 | `6` 蓝绿 | `7` 青 |
| `8` 天蓝 | `9` 蓝 | `10` 紫 | `11` 紫罗兰 |
| `12` 粉 | `13` 深红 | `14` 深橙 | `15` 深黄 |
| `16` 深黄绿 | `17` 深绿 | `18` 深蓝绿 | `19` 深青 |
| `20` 深天蓝 | `21` 深蓝 | `22` 深紫 | `23` 深紫罗兰 |
| `24` 深粉 | `25` 黑 | `26` 白 | `27` 灰 |
| `28` 棕 | `29` 暗影 | `30` 负片 | `31` 夜光 |

`//shape` 的等腰三角形使用 `u/d/l/r`（上/下/左/右）指定旋转方向；直角三角形使用 `u/d` 指定旋转、`l/r` 指定翻转方向。

坡度类型可使用 `none`、`t`、`tr`、`tl`、`br`、`bl`；草地生态支持 `crimson`、`corruption`、`hallow`、`jungle`、`mushroom`、`normal`、`forest`、`desert`、`sand` 和 `hell`。

### 剪贴板与历史

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//copy` | `worldedit.clipboard.copy` | 将选区复制到剪贴板 |
| `//cut` | `worldedit.clipboard.cut` | 剪切选区 |
| `//paste [对齐] [-f] [=> 条件]` | `worldedit.clipboard.paste` | 粘贴剪贴板内容 |
| `//spaste [对齐] [过滤参数] [=> 条件]` | `worldedit.clipboard.spaste` | 选择性粘贴剪贴板内容 |
| `//flip <x/y/xy>` | `worldedit.clipboard.flip` | 翻转剪贴板：`x` 左右翻转、`y` 上下翻转 |
| `//rotate <角度>` | `worldedit.clipboard.rotate` | 旋转剪贴板；角度必须是 90 的倍数 |
| `//scale <+/-> <数量>` | `worldedit.clipboard.scale` | 放大或缩小剪贴板 |
| `//undo [次数] [账号]` | `worldedit.history.undo` | 撤销编辑 |
| `//redo [次数] [账号]` | `worldedit.history.redo` | 重做编辑 |

粘贴对齐参数可组合使用 `l`、`r`、`t`、`b`。`//spaste` 可使用 `-t`、`-tp`、`-et`、`-w`、`-wp`、`-wi`、`-l` 排除方块、方块涂漆、空方块、墙、墙体涂漆、电线或液体。

### 原理图

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//schematic list [页码]` | `worldedit.schematic` | 查看原理图列表 |
| `//schematic load <名称>` | `worldedit.schematic` | 将原理图载入剪贴板 |
| `//schematic save [-f] <名称>` | `worldedit.schematic.save` | 保存当前剪贴板为原理图 |
| `//schematic copysave [-f] <名称>` | `worldedit.schematic.save` | 复制选区并直接保存为原理图 |
| `//schematic paste <名称> [对齐] [-f] [=> 条件]` | `worldedit.schematic.paste` | 直接粘贴原理图 |
| `//schematic delete -confirm <名称>` | `worldedit.schematic.delete` | 删除原理图 |

覆盖已有原理图还需要 `worldedit.schematic.overwrite` 权限。

### 工具与维护

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//activate <sign/chest/itemframe/sensor/dummy/all>` | `worldedit.utils.activate` | 激活选区内的标牌、箱子、物品框、传感器或假人 |
| `//drain` | `worldedit.utils.drain` | 排空选区液体 |
| `//flood <water/lava/honey>` | `worldedit.utils.flood` | 用指定液体灌满选区 |
| `//magicwand [X Y] => 条件` | `worldedit.magic.wand` | 从起点选择连续且满足条件的方块 |
| `//fixghosts` | `worldedit.utils.fixghosts` | 修复幽灵方块 |
| `//fixgrass` | `worldedit.utils.fixgrass` | 修复草方块状态 |
| `//fixhalves` | `worldedit.utils.fixhalves` | 修复半砖状态 |
| `//fixslopes` | `worldedit.utils.fixslopes` | 修复坡度状态 |
| `//killempty <signs/chests/all>` | `worldedit.utils.killempty` | 清理空标牌或空箱子 |
| `//mow` | `worldedit.utils.mow` | 清理选区内的植物 |
| `//size <clipboard> [玩家]` | `worldedit.utils.size` | 查看剪贴板尺寸 |
| `//size <schematic> <名称>` | `worldedit.utils.size` | 查看原理图尺寸 |

### 管理

| 指令 | 权限 | 说明 |
| --- | --- | --- |
| `//worldedit <选项> [值]` | `worldedit.admin` | 查看或修改 WorldEdit 运行配置 |

可用选项为 `wand`、`undocount`、`undodisable` 和 `schematic`。拥有 `worldedit.usage.otheraccounts` 权限的玩家可以操作其他账号的撤销记录或剪贴板信息。

## 配置

> 配置文件位置：`{服务器目录}/worldedit/config.json`

```json
{
  "MagicWandTileLimit": 10000,
  "MaxUndoCount": 50,
  "DisableUndoSystemForUnrealPlayers": false,
  "StartSchematicNamesWithCreatorUserID": false,
  "SchematicFolderPath": "schematics"
}
```

`MagicWandTileLimit` 限制魔法棒单次最多选择的方块数；`MaxUndoCount` 控制每个账号保留的撤销记录数；`DisableUndoSystemForUnrealPlayers` 可关闭未登录玩家的撤销记录；`StartSchematicNamesWithCreatorUserID` 会在原理图文件名前加入创建者账号 ID；`SchematicFolderPath` 指定原理图目录。

## 更新日志

```
v2026.07.20
- 适配 TShock 6.1.0 与 Terraria 1.4.5.6
- 完善 WorldEdit 指令、权限和配置文档
```

## 反馈

- 优先提交 Issue：[UnrealMultiple/TShockPlugin](https://github.com/UnrealMultiple/TShockPlugin/issues)
- TShock 官方群：816771079
- 国内社区：trhub.cn、bbstr.net、tr.monika.love
1111
