# PlayerSpeed

- Authors: 逆光奔跑 羽学
- Source: TShock QQ Group 816771079
- This is a Tshock server PlayerSpeed, mainly used for：
- Increase the speed and distance of player dashes and jumps.
- Automatically place players in a cooldown period after they have performed the specified number of actions as defined in the configuration file.

## Commands

| Syntax     | Alias  |    Permission     |               Description               |
|------------|:------:|:-----------------:|:---------------------------------------:|
| /vel on    |  None  |     vel.admin     |       Enable the plugin function        |
| /vel off   |  None  |     vel.admin     |       Disable the plugin function       |
| /vel set   | /vel s |     vel.admin     | Set global dash speed and cooldown time |
| /vel mess  |  None  |     vel.admin     |       Toggle announcement system        |
| /vel boss  |  None  |     vel.admin     |     Toggle automatic progress mode      |
| /vel del   |  None  |     vel.admin     |      Delete specified player data       |
| /vel reset |  None  |     vel.admin     |          Reset all player data          |
| /reload    |  None  | tshock.cfg.reload |        Reload configuration file        |

## Configuration
> Configuration file location： tshock/玩家速度.json
```json5
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