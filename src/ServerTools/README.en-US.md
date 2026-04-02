# ServerTools

- **Author**: 少司命  
- **Source**: None  
- This plugin provides additional commands and features for easier server management.  


## Commands  

| Syntax                                                                                                                         |        Permission        |                  Description                  |
|--------------------------------------------------------------------------------------------------------------------------------|:------------------------:|:---------------------------------------------:|
| /clp [range]                                                                                                                   |      `tshock.clear`      |  Clear projectiles without removing summons.  |
| /退出 or /toolexit                                                                                                               | `servertool.query.exit`  |   Allow mobile players to kick themselves.    |
| /查花苞 or /scp                                                                                                                   | `servertool.query.wall`  | Locate Plantera's Bulbs and add them to Warp. |
| /移除花苞 or /rcp                                                                                                                  | `servertool.query.wall`  |      Remove Plantera's Bulbs from Warp.       |
| /自踢 or /selfkick                                                                                                               |  `servertool.user.kick`  |        Kick yourself from the server.         |
| /自杀 or /selfkill                                                                                                               |  `servertool.user.kill`  |                Kill yourself.                 |
| /ghost                                                                                                                         | `servertool.user.ghost`  |    Toggle ghost mode, use again to revert.    |
| /旅途难度 [difficulty] `master` `journey` `normal` `expert` or /journeydiff [difficulty mode] `master` `journey` `normal` `expert` | `servertool.set.journey` |       Set difficulty for Journey mode.        |
| /在线排行 or /onlinerank                                                                                                           | `servertool.user.online` |    Check the online players' leaderboard.     |
| /死亡排行 or /deadrank                                                                                                             |  `servertool.user.dead`  |         Check the death leaderboard.          |
| /oc [playerName] [cmd]                                                                                                            |  servertool.user.cmd     |    Execute commands for the specified player   |
| /kickcheater on | servertool.admin.cheater | Enable modified client detection and kick cheaters |
| /kickcheater off | servertool.admin.cheater | Disable modified client detection |

## Modified Client Detection

This plugin includes a modified client detection feature that can identify and handle players using modified clients to join the server.

### How It Works

The plugin identifies modified clients by detecting specific network packet signature values:
- Detects abnormal control packet coordinate values
- Uses byte array XOR operations to hide real detection values, preventing easy bypassing
- Automatically kicks detected cheaters and broadcasts warnings

### Configuration

```json5
{
  "KickCheater": true,  // Whether to kick detected cheaters
  "KickCheaterText": "Using modified client"  // Reason displayed when kicking
}
```

### Permissions

- `servertool.admin.cheater` - Allows using `/kickcheater` command to toggle detection

## REST API  

| Endpoint        |        Description        |
| --------------- | :-----------------------: |
| **/deathrank**  | Retrieve death rank data  |
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


| Field          |      Type       |                                          Description                                           |                         Possible Values                         |
|----------------|:---------------:|:----------------------------------------------------------------------------------------------:|:---------------------------------------------------------------:|
| `DeathLast` | `bool` | Players who log out while dead must wait until the death state ends to rejoin | `true` or `false` |
| `KickCheater` | `bool` | Whether to kick players using modified clients | `true` or `false` |
| `KickCheaterText` | `string` | Reason displayed when kicking cheaters | Any text, e.g., `"Using modified client is not allowed!"` |
| `SentryLimit` | `int32` | Limits the number of sentries a player can summon | Number, default `20` |
| `SummonLimit` | `int32` | Limits the number of summons a player can summon | Number, default `11` |
| `OnlySoftCoresAreAllowed` | `bool` | Only allows softcore players to join | `true` or `false` |
| `SetWorldMode` | `bool` | Whether to force set the world mode | `true` or `false` |
| `WorldMode` | `int32` | Specifies the world difficulty level | `0`=Journey, `1`=Normal, `2`=Expert, `3`=Master |
| `ChatLength` | `int32` | Restricts the maximum length of chat messages | Number, default `50` |
| `SetJourneyDifficult` | `bool` | Enables setting the difficulty for Journey mode | `true` or `false` |
| `JourneyDifficult` | `string` | Defines the difficulty level for Journey mode | `master`, `journey`, `normal`, `expert` |
| `BlockUnregisteredEntry` | `bool` | Prevents unregistered players from joining the server | `true` or `false` |
| `MonsterPickUpMoney` | `bool` | Prevents monsters from picking up coins dropped by players | `true` or `false` |
| `ClearDrop` | `bool` | Removes items dropped after a player's death | `true` or `false` |
| `PreventsDeath` | `bool` | Prevents players in a death state from joining the server | `true` or `false` |
| `DeadTimer` | `bool` | Enables a countdown timer for player deaths | `true` or `false` |
| `DeadFormat` | `string` | Format for the death countdown timer | Use `{0}` as time placeholder, e.g., `"You will respawn in {0} seconds!"` |
| `KeepOpenChest` | `bool` | Prevents players from opening multiple chests simultaneously | `true` or `false` |
| `KeepArmor` | `bool` | Prevents equipping duplicate accessories | `true` or `false` |
| `KeepArmor2` | `bool` | Prohibits having a seventh accessory slot before Hardmode | `true` or `false` |
| `BlockEntryStatement` | `string` | Message displayed to unregistered players trying to join | Any text, e.g., `"Unregistered players cannot join"` |
| `BlockEntryExecCommands` | `array<string>` | Commands executed at server startup when there are no registered players | Command array, e.g., `["/worldmode 2"]` |
| `EnableNpcProtect` | `bool` | Enables NPC protection feature | `true` or `false` |
| `NpcProtects` | `array<int>` | List of NPC IDs protected by the server | Array of NPC IDs |
| `MultipleFishingRodsAreProhibited` | `bool` | Prevents players from exploiting the multi-fishing line bug | `true` or `false` |
| `ForbiddenBuoys` | `array<int>` | List of Projectile IDs used to detect multi-fishing line issues | Array of Projectile IDs |


## Changelog

### v1.3.1.0
- Fixed byte array calculation logic for modified client detection
- Optimized detection value hiding using XOR operations

### v1.3.0.1
- Added NPC spawn rate modification

### v1.3.0.0
- Added modified client detection feature to detect and kick cheaters

### v1.2.0.0
- Added /readplayer command

### v1.1.8.7
- Removed decimals from death countdown message

### v1.1.8.6
- Prioritized English commands

### v1.1.8.5
- Fixed configuration hot-reload issues

### v1.1.7.9
- Fixed hook errors

### v1.1.7.8
- Improved accessory detection and handling

### v1.1.7.7
- Fixed summon and sentry detection issues

### v1.1.7.6
- Fixed default values and documentation

### v1.1.7.5
- Added summon limit configuration

### v1.1.7.4
- Improved unload function

### v1.1.7.0
- Fixed Demon Heart slot issues for mobile players

### v1.1.6.0
- Added NPC protection
- Added pre-Hardmode 7th accessory slot prevention

### v1.1.5.0
- Improved duplicate accessory handling

### v1.1.4.0
- Added accessory whitelist permission

### v1.1.3.0
- Added duplicate accessory prevention
- Added shorthand commands

### v1.1.2.0
- Enhanced /ai command with account ID lookup

### v1.1.1.0
- Fixed monster coin pickup prevention

### v1.1.0.0
- Added dual chest prevention

### v1.0.0.4
- Fixed REST API ban command issues

### v1.0.0.3
- Fixed death rank database errors


## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love