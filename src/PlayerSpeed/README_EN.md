# PlayerSpeed

- Authors: 逆光奔跑 羽学
- Source: TShock QQ Group 816771079
- This is a Tshock server PlayerSpeed, mainly used for：
- Increase the speed and distance of player dashes and jumps.
- Automatically place players in a cooldown period after they have performed the specified number of actions as defined in the configuration file.

## Update Log

```
v1.2.2
- Added automatic progress mode, which automatically sets speed and cooling related values based on defeating any boss (toggle with command: /vel boss).
- Remember to reset defeat records using the command: /vel reset when using automatic progress mode.
- Admins with vel.admin permission will not enter the cooldown period (trigger intervals are still subject to prevent server lag).
- Players with vel.use permission can use /vel to check the current speed status.
- Added a configuration item for dividing the Y-axis descent multiplier (used to optimize the feel when assisting mount sprints).
- Added logic for Y-axis ascending and descending acceleration:
  - Holding down left upper or left lower while pressing space will accelerate ascent and descent.
  - Conversely, only pressing left or right, then pressing space directly accelerates forward movement.
- The /vel s command now includes new parameter attributes:
  - Height: h

v1.2.1
- Restructured infinite dash logic to avoid performance issues:
  - Enters cooldown time after satisfying the 'number of uses' specified in the configuration.
  - Set an interval time (milliseconds) for each action.
- Removed the "last jump" related broadcast.
- Removed stop time (ut) attribute.
- The /vel s command now includes new parameter attributes:
  - Interval: r
  - Number of uses: c

v1.2.0
- Added infinite dash mechanism:
  - Continuous dashing by double-tapping sprint with shield-like accessories.
  - Constant jumping when equipped with specified items can refresh the infinite dash interval.
  - Automatically enters cooldown when the dash interval exceeds the set milliseconds.
- The /vel s command now includes new parameter attributes:
  - Stop time: ut
  - Add jump item: add
  - Remove jump item: del

v1.1.0
- Modified according to the requirements of server owner "Sentry" from the QQ group.
- Added sprint judgment logic and cooldown mechanisms.
- Restructured most of the code, command methods, and trigger logic.
- Player usage permission: vel.use
- Administrator permission: vel.admin
- /vel set command format:
  - /vel s sd 40 t 10

v1.0.0
- Initially decompiled from Niguangbenpao's version.

v1.2.1
Refactored the infinite dash logic to avoid performance issues:
- Enters cooldown after reaching the configured 'count' of actions.
- Added interval time (in milliseconds) between each action.
- Removed "last jump" related announcements.
- Removed stop time (ut) attribute.
- Added new property parameters to the `/vel s` command: `interval:r`, `count:c`.

v1.2.0
Infinite Sprint Mechanism Added:
When using shield-type accessories, double-tap to sprint continuously.
Equipping specified items allows continuous jumping, which can refresh the infinite sprint interval.
If the sprint interval exceeds the milliseconds defined as Stop Infinite Sprint, it will automatically enter a cooldown period.

New Attributes Added to /vel s Command:
Stop Time (ut)
Add Jump Item (add)
Remove Jump Item (del)

v1.1.0
Modified based on customization requirements by the server owner "Sentinel" from the group.
Added dash logic and cooldown mechanism.
Refactored most of the code, command methods, and trigger logic.
Player permission: vel.use
Admin permission: vel.admin
Command format for /vel set:
/vel s sd 40 t 10

v1.0.0
Decompiled from Niguang Benpao's version.
```

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| /vel on                          | None  | vel.admin    | Enable the plugin function           |
| /vel off                         | None  | vel.admin    | Disable the plugin function          |
| /vel set                         | /vel s| vel.admin    | Set global dash speed and cooldown time |
| /vel mess                        | None  | vel.admin    | Toggle announcement system            |
| /vel boss                        | None  | vel.admin    | Toggle automatic progress mode            |
| /vel del                         | None  | vel.admin    | Delete specified player data          |
| /vel reset                       | None  | vel.admin    | Reset all player data                 |
| /reload                          | None  | tshock.cfg.reload | Reload configuration file |

## Configuration
> Configuration file location： tshock/玩家速度.json
```json
{
  "Enabled": true,
  "Uses": 5,
  "Interval": 2000.0,
  "CoolDown": 25,
  "Speed": 20.0,
  "Height": 5.0,
  "Broadcast": true,
  "Sprint": true,
  "SprintMultiplier": 1.5,
  "Jump": true,
  "JumpDivideMultiplier": 5.0,
  "JumpBoostItems": [
    5107,
    4989
  ],
  "AutoProgress": true,
  "BossProgressList": [
    {
      "Name": "Eye of Cthulhu, King Slime",
      "Defeated": true,
      "SetSpeed": 20.0,
      "SetHeight": 2.5,
      "Uses": 1,
      "CoolDownTime": 60,
      "NPCIds": [
        4,
        50
      ]
    },
    {
      "Name": "Eater of Worlds, Brain of Cthulhu",
      "Defeated": false,
      "SetSpeed": 25.0,
      "SetHeight": 5.0,
      "Uses": 2,
      "CoolDownTime": 45,
      "NPCIds": [
        13,
        266
      ]
    },
    {
      "Name": "Wall of Flesh",
      "Defeated": false,
      "SetSpeed": 30.0,
      "SetHeight": 10.0,
      "Uses": 3,
      "CoolDownTime": 30,
      "NPCIds": [
        113
      ]
    },
    {
      "Name": "Golem, Destroyer, Skeletron Prime, Plantera",
      "Defeated": false,
      "SetSpeed": 40.0,
      "SetHeight": 15.0,
      "Uses": 4,
      "CoolDownTime": 15,
      "NPCIds": [
        125,
        126,
        127,
        134
      ]
    }
  ]
}
```
## FeedBack
- Github Issue -> TShockPlayerSpeed Repo: https://github.com/UnrealMultiple/TShockPlayerSpeed
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love