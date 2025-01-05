# RebirthCoin 复活币

- Authors: GK & 羽学
- 出处: GK的QQ群232109072
- Description: This is a TShock server plugin mainly used for allowing players to quickly resurrect by consuming specific items.

## Commands

| Syntax  |  别名 |                     Permission                    |              Description             |
| ------- | :-: | :-----------------------------------------------: | :----------------------------------: |
| No      |  No |                    RebirthCoin                    | Permission to use resurrection coins |
| /reload |  No | tshock.cfg.reload |     Reload the configuration file    |

## Configuration

> Configuration file location： tshock/复活币.json

```json5
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

## 更新日志

```
v1.0.2
适配.Net 6.0
将复活币的物品ID从整数改为数组（支持更多物品作为复活币）
将复活币权限名改为：RebirthCoin
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
