# AutoFish

- Authors: 羽学 & 少司命
- Source: 无
- AutoFish is an automatic fishing plugin designed for Tshock servers.
- Extra Items: In addition to items that already exist in the environment, you can configure extra items to be caught through fishing.
- 可通过配置文件调整单次鱼钩数量上限，或消耗指定物品来换取插件使用时长。
- 配备完整的指令系统(有些功能没开启或没权限不会显示相关指令)
- Blood Moon Restriction: No NPCs can be fished during a Blood Moon. A relevant prompt will appear when using the /af command during a Blood Moon.
- (处于血月环境下使用/af指令会有相对于提示)

## Commands

| 语法                  |             别名            |                     Permission                    |                                  Description                                 |
| ------------------- | :-----------------------: | :-----------------------------------------------: | :--------------------------------------------------------------------------: |
| /af                 |         /autofish         |                      autofish                     | Command menu (check remaining automatic fishing duration) |
| /af on              |        /autofish on       |                      autofish                     |                    Enable automatic fishing for the player                   |
| /af off             |       /autofish off       |                      autofish                     |                   Disable automatic fishing for the player                   |
| /af list            |       /autofish list      |                      autofish                     |                   List items specified for consumption mode                  |
| /af loot            |       /autofish loot      |                      autofish                     |                            List extra catch items                            |
| /af buff            |       /autofish buff      |                      autofish                     |                       Toggle the player's fishing buff                       |
| /af more            |       /autofish more      |           autofish.admin          |                       Enable or disable multi-hook mode                      |
| /af duo <number>    |       /autofish duo       |           autofish.admin          |                  Set the number of hooks for multi-hook mode                 |
| /af + <item name>   |  /autofish + <item name>  |           autofish.admin          |                      Add an item to the extra catch list                     |
| /af - <item name>   |  /autofish - <item name>  |           autofish.admin          |                   Remove an item from the extra catch list                   |
| /af mod             |       /autofish mod       |           autofish.admin          |                      Enable or disable consumption mode                      |
| /af set <number>    |   /autofish set <number>  |           autofish.admin          |            Set the required quantity of items for consumption mode           |
| /af time <number>   |  /autofish time <minute>  |           autofish.admin          |               Set the duration of automatic fishing in minutes               |
| /af add <item name> | /autofish add <item name> |           autofish.admin          |                   Add a specified item for consumption mode                  |
| /af del <item name> | /autofish del <item name> |           autofish.admin          |                 Remove a specified item from consumption mode                |
| /reload             |             无             | tshock.cfg.reload |                         Reload the configuration file                        |

## Configuration

> Configuration file location： tshock/AutoFish.en-US.json

```json5
{
  "AdditionalCatches": [
    29,
    3093,
    4345
  ], // Extra catch items in addition to those that exist in the environment
  "Enable": true, // Global plugin switch
  "MultipleFishFloats": true, // Enable multi-hook mode for automatic fishing to increase efficiency
  "RandCatches": false, // Randomly fish out any item
  "MultipleFishFloatsLimit": 5, // Define the maximum number of hooks that can fish simultaneously
  "SetBuffs": {
    "80": 10, // 80 is the buff ID, 10 is the duration in frames (60 frames = 1 second)
    "122": 240
  },
  "ConsumeBait": false, // Consume items to exchange for automatic fishing usage duration
  "ConsumeBaitNum": 10, // Item quantity requirement in consumption mode (e.g., 6 of item A + 4 of item B = 10)
  "Time": 24, // Duration of automatic fishing granted to players in consumption mode, in minutes
  "ConsumeItem": [
    2002, // Specified items for consumption mode
    2675,
    2676,
    3191,
    3194
  ],
  "ForbidProjectiles": [
    623, // To solve the BUG where single summon creatures spawn more numbers under multi-hook mode
    625,
    626,
    627,
    628,
    831,
    832,
    833,
    834,
    835,
    963,
    970
  ]
}
```

## 更新日志

```
v1.3.5
准备更新TS 5.2.1,修正文档，初始配置内容更改

v1.3.4
补充v1.3.2遗漏的代码

v1.3.3
引用LazyAPI

v1.3.2
尝试修复鱼饵数量为1时崩服BUG

v1.3.1
修复了在多钩钓鱼模式下，单体召唤物衍生更多数量BUG
并加了一个配置项来禁止它们"禁止衍生弹幕"。
加入了随机物品配置项，开启时会随机钓到任意物品。

v1.3.0
修复了首选鱼饵不会消失,变成无限饵,或卡线程BUG
修改了额外渔获也能与其他环境已存在的物品一起上钩
重构了消耗模式的代码逻辑，优化性能，修复BUG。
加入了设置多钩数量的指令：/af duo 数量
加入了额外渔获表的相关指令
把/af buff指令改为玩家可用，用来切换自身BUFF

v1.2.0
修复了钓鱼不消耗鱼饵问题
修复了松露虫钓不了猪鲨的BUG
修复了鱼饵数量为1时线程卡住问题
加入了【消耗模式】配置项
加入了【钓鱼BUFF】配置项（上钩才会触发）
消耗模式为1.1.0版的扣除物品数量获取自动时长逻辑
完善了自动钓鱼的指令系统，并对其做了不同权限与模式下的内容显示

v1.1.0
成功完成Tshock版《自动钓鱼》，
加入了消耗鱼饵数量来换取自动钓鱼使用时长的逻辑
当身上有松露虫时，会自动钓上铁镐，并试图关闭玩家的自动钓鱼开关
玩家可通过/af on指令重新开启插件，并不会清空玩家的自动钓鱼时长

v1.0.0
试图制作Tshock版《自动钓鱼》，而失败的半成品：
服务器无法修改客户端玩家的操作，更没有相对数据包来处理上钩的状态。
尝试从AI[0]改为1来触发收线效果，但无法获取到实际的渔获。

```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
