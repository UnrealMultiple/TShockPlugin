# AutoStoreItems

- Authors: 羽学 cmgy雱
- Source: None
- Automatically store specified items into storage space based on configured item IDs.
- (Supports automatic money storage, thanks to cmgy雱’s coin stacking algorithm)

## 命令

| 语法                |         别名        |                         权限                        |                        说明                        |
| ----------------- | :---------------: | :-----------------------------------------------: | :----------------------------------------------: |
| /ast              |        /自存        |           AutoStore.use           |                       指令菜单                       |
| /ast auto         |      /自存 auto     |           AutoStore.use           |                    开启或关闭自动识别模式                   |
| /ast hand         |      /自存 hand     |           AutoStore.use           |                    开启或关闭手持识别模式                   |
| /ast armor        |     /自存 armor     |           AutoStore.use           |                    开启或关闭装备识别模式                   |
| /ast list         |      /自存 list     |           AutoStore.use           |                    列出自己的自存桶物品名                   |
| /ast clear        |     /自存 clear     |           AutoStore.use           |                     清空自己的自存桶表                    |
| /ast bank         |      /自存 auto     |           AutoStore.use           |                 将物品放入存钱罐时自动添加自存表                 |
| /ast mess         |      /自存 mess     |           AutoStore.use           |                     开启或关闭自存消息                    |
| /ast add 或 del id | /自存 add 或 del 物品名 |           AutoStore.use           |                   添加或移除自己的自存物品                   |
| /ast pm           |       /自存 pm      |          AutoStore.admin          | 开启或关闭性能模式(不为堆叠达到单格上限物品进行分堆累积) |
| /astreset         |       /重置自存       |          AutoStore.admin          |                  清空玩家数据表（重置服务器用）                 |
| /reload           |         无         | tshock.cfg.reload |                      重载配置文件                      |

---

## Configuration Notes

1. `自动` `手持` `装备` 当开启3种模式`任意一个`时其他2个模式会`默认关闭`，不论哪种模式都需要`玩家移动和攻击`才会触发储存

2.`性能模式`不会对`单格达到9999`或者本身为`1`堆叠上限的物品，进行空槽分堆累积，如果`服务器人少`的情况下可以考虑`关闭`给玩家最好的自存体验

5.`Existing BUG` collected items will be ` Unfavorite` (referring to the `Void Bag` potions having the risk of `stacking into the chest`).

## Config

> Configuration file location：tshock/AutoStoreItems.en-US.json

```json
{
  "插件开关": true,
  "性能模式": true,
  "存钱罐": true,
  "保险箱": true,
  "虚空袋": true,
  "护卫熔炉": true,
  "触发存储的物品ID": [
    87,
    346,
    3213,
    3813,
    4076,
    4131,
    5098,
    5325
  ],
  "装备饰品的物品ID": [
    88,
    410,
    411,
    489,
    490,
    491,
    855,
    935,
    1301,
    2220,
    2998,
    3034,
    3035,
    3061,
    3068,
    4008,
    4056,
    4989,
    5098,
    5107,
    5126
  ]
}
```

## 更新日志

```
{
  "Enable": true, // Enables equipment holding detection and main inventory holding detection features.
  "Optimize": true, // Optimization settings, this may include setting the storage speed (Main Inventory) to the recommended value of 120, as well as money storage speed settings, etc.
  "Piggiy": true, // Enables the piggy bank (money storage) feature.
  "Safe": true, // Enables the safe storage feature.
  "Forge": true, // Enables the Defender's Forge feature.
  "VoidVault": true, // Enables the Void Bag feature.
  "StoragePrompt": true, // Shows a prompt message when items are stored.
  "HoldItemToActivate": false, // Whether a specific item needs to be held to activate the storage function; if disabled, having one of the specified items in the inventory will suffice to activate it.
  "TargetItems": [ // List of item IDs that need to be detected for holding. When the player holds these items, it can trigger the auto-storage function.
    87,
    346,
    3213,
    3813,
    4076,
    4131,
    5098, // Note: Here ID `5098` seems to replace the original `5325`, possibly an update or correction.
    5325 // It appears both IDs are included now. Please verify if this is intentional.
  ],
  "LoadoutItem": [ // List of item IDs for armor and accessories. Equipping these items will trigger auto-storage when the player moves or attacks.
    88,
    410,
    411,
    489,
    490,
    491,
    855,
    935,
    1301,
    2220,
    2998,
    3034,
    3035,
    3061,
    3068,
    4008,
    4056,
    4989,
    5098,
    5107,
    5126
  ]
}
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
