# ItemDeco 手持物品显示

- 作者: FrankV22
- 出处: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- 显示物品名称：当玩家切换持有的物品时，该物品的名称会以浮动消息的形式出现在玩家头顶，并在聊天中显示。此功能还支持显示伤害值，并可通过配置文件启用或禁用。
- 颜色自定义：浮动消息的颜色默认为白色（RGB 值为 255,255,255），可以自定义
- 兼容 [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## 更新日志

```
V1.0.0.1
重构代码
```

## 指令
```
暂无
```

## 配置
> 配置文件路径：tshock/ItemDecoration.json
```json

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

## 反馈
- 本插件优先找原仓库：[Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
