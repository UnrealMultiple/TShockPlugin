# ItemDeco 手持物品显示

- Author: FrankV22
- Source: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- **Show Item Name**: Whenever a player switches the item they are holding, the name of the item will appear as a floating message above their head and in the chat. This feature also supports showing the item's damage value, which can be toggled in the config.此功能还支持显示伤害值，并可通过配置文件启用或禁用。
- **Color Customization**: The floating message color is by default white (RGB 255, 255, 255) but can be customized.
- **Compatible with** [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## Commands

```
暂无
```

## Configuration

> Configuration file path: tshock/ItemDeco/ItemDecoration.en-US.json

```json5
{
  "itemChat": {
    "itemColor": {
      "R": 255,
      "G": 255,
      "B": 255
    },
    "damageColor": {
      "R": 0,
      "G": 255,
      "B": 255
    },
    "showName": false,
    "showDamage": false
  },
  "itemText": {
    "damageText": "Damage",
    "defaultColor": {
      "R": 255,
      "G": 255,
      "B": 255
    },
    "rarityColors": {
      "-1": {
        "R": 169,
        "G": 169,
        "B": 169
      },
      "0": {
        "R": 255,
        "G": 255,
        "B": 255
      },
      "1": {
        "R": 0,
        "G": 128,
        "B": 0
      },
      "2": {
        "R": 0,
        "G": 112,
        "B": 221
      },
      "3": {
        "R": 128,
        "G": 0,
        "B": 128
      },
      "4": {
        "R": 255,
        "G": 128,
        "B": 0
      },
      "5": {
        "R": 255,
        "G": 0,
        "B": 0
      },
      "6": {
        "R": 255,
        "G": 215,
        "B": 0
      },
      "7": {
        "R": 255,
        "G": 105,
        "B": 180
      },
      "8": {
        "R": 255,
        "G": 215,
        "B": 0
      },
      "9": {
        "R": 0,
        "G": 255,
        "B": 255
      },
      "10": {
        "R": 255,
        "G": 105,
        "B": 180
      },
      "11": {
        "R": 75,
        "G": 0,
        "B": 130
      }
    },
    "showName": true,
    "showDamage": true
  }
}
```

## 更新日志

```
v1.0.0.1
完成西班牙语的i18n config配置，修复聊天不显示名字的问题
V1.0.0.0
重构代码
```

## Feedback

- The plugin’s primary repository: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- Preferred feedback through issues -> Shared plugin repository: https://github.com/UnrealMultiple/TShockPlugin
- Secondary: TShock official group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
