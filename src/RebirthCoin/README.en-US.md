# RebirthCoin

- Authors: GK & 羽学
- Source: 无
- Description: This is a TShock server plugin mainly used for allowing players to quickly resurrect by consuming specific items.

## Update Log

```
v1.0.2
Adapted to .NET 6.0
Changed the resurrection coin item ID from int to int[] (array)
```

## Commands

| Syntax                             | Alias  |       Permission       |                   Description                   |
| -------------------------------- | :---: | :--------------: | :--------------------------------------: |
| No  | No |   RebirthCoin    |    Permission to use resurrection coins    |  
| /reload  | No |   tshock.cfg.reload    |    Reload the configuration file    |

## Configuration
> Configuration file location： tshock/复活币.json
```json
{
  "插件开关": true, // Enable or disable the plugin function
  "允许PVP复活": false, // Allow or disallow using resurrection coins in PvP mode
  "复活提醒": "{0} 被圣光笼罩，瞬间复活!!!", // The message displayed when a player resurrects
  "复活提醒的颜色": [
    255, // The color of the resurrection message, RGB values, higher numbers mean lighter colors, all 255 means white.
    215,
    0
  ],
  "复活币的物品ID": [
    3229 // Item IDs that can be used as resurrection coins, multiple items can be listed separated by commas.
  ]
}
```
## FeedBack
- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- China Terraria Forum: trhub.cn, bbstr.net, tr.monika.love