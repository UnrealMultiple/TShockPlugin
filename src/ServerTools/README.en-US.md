# ServerTools

- **Author**: 少司命
- **Source**: None
- This plugin provides additional commands and features for easier server management.

## Commands

| Syntax                                                                                                                                                                                                                                 |        Permission        |                        Description                       |
| -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | :----------------------: | :------------------------------------------------------: |
| /clp [range]                                                                                                                                                                       |      `tshock.clear`      |                       清理弹幕但不清理玩家召唤物                      |
| /退出 or /toolexit                                                                                                                                                                                                                       |  `servertool.query.exit` | Allow mobile players to kick themselves. |
| /查花苞 or /scp                                                                                                                                                                                                                           |  `servertool.query.wall` |                    查找地图上的花苞并添加到 Warp 里                   |
| /移除花苞 or /rcp                                                                                                                                                                                                                          |  `servertool.query.wall` |    Remove Plantera's Bulbs from Warp.    |
| /自踢 or /selfkick                                                                                                                                                                                                                       |  `servertool.user.kick`  |      Kick yourself from the server.      |
| /自杀 or /selfkill                                                                                                                                                                                                                       |  `servertool.user.kill`  |              Kill yourself.              |
| /ghost                                                                                                                                                                                                                                 |  `servertool.user.ghost` |  Toggle ghost mode, use again to revert. |
| /旅途难度 [difficulty] `master` `journey` `normal` `expert` or /journeydiff [difficulty mode] `master` `journey` `normal` `expert` | `servertool.set.journey` |     Set difficulty for Journey mode.     |
| /在线排行 or /onlinerank                                                                                                                                                                                                                   | `servertool.user.online` |  Check the online players' leaderboard.  |
| /死亡排行 or /deadrank                                                                                                                                                                                                                     |  `servertool.user.dead`  |                         查询玩家死亡排行                         |

## REST API

| 路径                  |        Description        |
| ------------------- | :-----------------------: |
| **/deathrank**      |  Retrieve death rank data |
| **/onlineDuration** | Retrieve online rank data |

## Configuration

> Configuration file path: `tshock/ServerTools.json`

```json5
{
  "死亡延续": true,  // When a player exits in a death state, they must wait for respawn when re-entering the server.
  "限制哨兵数量": 20,  // Limits the number of sentinels a player can summon.
  "限制召唤物数量": 11,  // Limits the number of summons a player can have.
  "仅允许软核进入": false,  // Allow only softcore players to join.
  "是否设置世界模式": true,  // If true, world mode will be set.
  "世界模式": 2,  // 0 for Journey, 1 for Normal, 2 for Expert, 3 for Master difficulty.
  "限制发言长度": 50,  // Limits the length of chat messages.
  "设置旅途模式难度": false,  // If true, the difficulty for Journey mode will be set.
  "旅途模式难度": "master",  // Set the difficulty for Journey mode (master, journey, normal, expert).
  "阻止未注册进入": false,  // Block unregistered players from entering the server.
  "禁止怪物捡钱": true,  // Prevent monsters from picking up money dropped by players.
  "清理掉落物": false,  // Clear dropped items after players die.
  "死亡倒计时": false,  // Enable or disable the death countdown.
  "阻止死亡角色进入": true,  // Block dead characters from entering the server; they must resolve death in single-player mode.
  "禁止双箱": true,  // Prevent opening two chests at the same time.
  "禁止双饰品": true,  // Prevent equipping duplicate accessories.
  "禁止肉前第七格饰品": true,  // Prevent players from having a seventh accessory slot before Hardmode.
  "死亡倒计时格式": "你还有{0}秒复活!",  // Format for the death countdown message (with remaining time).
  "未注册阻止语句": "未注册不能进入服务器",  // Message displayed when an unregistered user tries to join.
  "未注册启动服务器执行命令": [],  // Commands executed when the server starts and no users are registered.
  "开启NPC保护": false,  // Enable NPC protection.
  "NPC保护表": [  // List of NPCs to protect.
    17,
    18,
    19,
    20,
    38,
    105,
    106,
    107,
    108,
    160,
    123,
    124,
    142,
    207,
    208,
    227,
    228,
    229,
    353,
    354,
    376,
    441,
    453,
    550,
    579,
    588,
    589,
    633,
    663,
    678,
    679,
    680,
    681,
    682,
    683,
    684,
    685,
    686,
    687,
    375,
    442,
    443,
    539,
    444,
    445,
    446,
    447,
    448,
    605,
    627,
    601,
    613
  ], 
  "禁止多鱼线": true,  // Prevent multiple fishing lines bug.
  "浮漂列表": [ // List of fishing float IDs for multiple fishing lines detection.
    360,
    361,
    362,
    363,
    364,
    365,
    366,
    381,
    382,
    760,
    775,
    986,
    987,
    988,
    989,
    990,
    991,
    992,
    993
  ]
}
```

| Field          |       Type      |                                                   Description                                                  |                         Possible Values                         |
| -------------- | :-------------: | :------------------------------------------------------------------------------------------------------------: | :-------------------------------------------------------------: |
| `死亡延续`         |      `bool`     |         Players who log out while dead must wait until the death state ends to rejoin.         |                              Empty                              |
| `限制哨兵数量`       |     `int32`     |                       Limits the number of sentries a player can summon.                       |                              Empty                              |
| `限制召唤物数量`      |     `int32`     |                        Limits the number of summons a player can summon.                       |                              Empty                              |
| `仅允许软核进入`      |      `bool`     |                                                                                                                |                              Empty                              |
| `是否设置世界模式`     |      `bool`     |                      If set to `true`, the world mode will be configured.                      |                              Empty                              |
| `设置世界模式`       |     `int32`     |                                                       难度                                                       | `0` for Journey, `1` for Normal, `2` for Expert, `3` for Master |
| `限制发言长度`       |     `int32`     |                                                     发言长度限制                                                     |                              Empty                              |
| `设置旅途模式难度`     |      `bool`     |                  Enables setting the difficulty for Journey mode when `true`.                  |                              Empty                              |
| `旅途模式难度`       |     `string`    |                                                       难度                                                       |             `master`, `journey`, `normal`, `expert`             |
| `阻止未注册进入`      |      `bool`     |                     Prevents unregistered players from joining the server.                     |                              Empty                              |
| `禁止怪物捡钱`       |      `bool`     |                                                   玩家死亡后怪物无法捡钱                                                  |                              Empty                              |
| `清理掉落物`        |      `bool`     |                          Removes items dropped after a player's death.                         |                              Empty                              |
| `阻止死亡角色进入`     |      `bool`     | Prevents players in a death state from joining the server until revived in single-player mode. |                              Empty                              |
| `死亡倒计时`        |      `bool`     |                                                    是否开启死亡倒计时                                                   |                              Empty                              |
| `禁止双箱`         |      `bool`     |                                                   禁止同时打开两个箱子                                                   |                              Empty                              |
| `禁止双饰品`        |      `bool`     |                                                   禁止重复装备相同的饰品                                                  |                              Empty                              |
| `禁止肉前第七格饰品`    |      `bool`     |                                                  禁止肉前就有第七个饰品栏                                                  |                              Empty                              |
| `死亡倒计时格式`      |     `string`    |                              Format for the death countdown timer.                             |                       `{0}` remaining time                      |
| `未注册阻止语句`      |     `string`    |                                                   阻止未注册玩家提示语                                                   |                              Empty                              |
| `未注册启动服务器执行命令` | `array<string>` |            Commands executed at server startup when there are no registered players.           |                              Empty                              |
| `开启NPC保护`      |      `bool`     |                                     Enables NPC protection.                                    |                              Empty                              |
| `NPC保护表`       |   `array<int>`  |                                                     NPC保护表                                                     |                              Empty                              |
| `禁止多鱼线`        |      `bool`     |                  Prevents players from exploiting the multi-fishing line bug.                  |                              Empty                              |
| `浮漂列表`         |   `array<int>`  |                                                    检测多鱼线浮漂列表                                                   |                              Empty                              |

## 更新日志

```
v1.1.7.9
修复hook报错

v1.1.7.8
也许根本不需要判Main.projectile[e.Index].minion，以及非SSC情况下检测到多饰品不尝试为玩家摘下

v1.1.7.7
修复召唤物和哨兵检测问题，完成i18n英文

v1.1.7.6
修正默认值和文档，i18n预备

V1.1.7.5
添加配置限制召唤物数量

v1.1.7.4
完善卸载函数

V1.1.7.0
修复手游恶魔之心格子不正确问题
使用_timer字段加快清除速度

V1.1.6.0
添加NPC保护
添加禁止肉前第七格饰品栏

V1.1.5.0
给禁双饰品加入清理物品方法，避免重复刷控制台。
只检测Armor前10个格子

V1.1.4.0
只检查当前装备页与饰品栏
给禁双饰品加免检权限：servertool.armor.white
修复空饰品时的误判

V1.1.3.0
配置项加了【禁双饰品】
为兼容旧版添加2个别名命令
给/查花苞加了个字母命令：/scp
给/移除花苞加了个字母命令：/rcp

V1.1.2.0
给Tshock自带的/ai指令 加入了查询指定玩家的账户ID

V1.1.1.0
1.修复禁止怪物捡钱
2.移除了禁止快速放入箱子

V1.1.0.0
添加双箱限制

V1.0.0.4
修复使用Rest时无法使用ban指令的问题

V1.0.0.3
修复死亡排行添加数据库报错
修复死亡倒计时
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
