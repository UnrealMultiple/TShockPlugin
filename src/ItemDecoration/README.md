# ItemDeco 手持物品显示

- 作者: FrankV22
- 出处: [Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- 显示物品名称：当玩家切换持有的物品时，该物品的名称会以浮动消息的形式出现在玩家头顶，并在聊天中显示。此功能还支持显示伤害值，并可通过配置文件启用或禁用。
- 颜色自定义：浮动消息的颜色默认为白色（RGB 值为 255,255,255），可以自定义
- 兼容 [Floating-MessageChat](https://github.com/itsFrankV22/FloatingText-Chat)

## 更新日志

```
暂无
```

## 指令
```
暂无
```

## 配置
> 配置文件路径：tshock/ItemDeco/ItemChatConfig.json
```json
{
  "CONFIGURATION": {    
    "ShowItem": true, // 是否显示物品名称
    "ShowDamage": true, // 是否显示物品的伤害值
    "ItemColor": {  // 物品名称的颜色
      "R": 255,
      "G": 255,
      "B": 255
    },
    "DamageColor": {  // 伤害值的颜色
      "R": 0,
      "G": 255,
      "B": 255
    }
  }
}

```
> 配置文件路径：tshock/ItemDeco/ItemTextConfig.json
```json
{
  "ShowName": true, // 是否显示物品名称
  "ShowDamage": true, // 是否显示物品的伤害值
  "DamageText": "Damage", // 显示伤害值时的前缀文本
  "DefaultColor": { // 默认颜色
    "r": 255, // 红色分量
    "g": 255, // 绿色分量
    "b": 255  // 蓝色分量
  },
  "RarityColors": { // 稀有度对应的颜色
    "-1": { // 灰色（169, 169, 169）
      "r": 169,
      "g": 169,
      "b": 169
    },
    "0": { // 白色（255, 255, 255）
      "r": 255,
      "g": 255,
      "b": 255
    },
    "1": { // 绿色（0, 128, 0）
      "r": 0,
      "g": 128,
      "b": 0
    },
    "2": { // 蓝色（0, 112, 221）
      "r": 0,
      "g": 112,
      "b": 221
    },
    "3": { // 紫色（128, 0, 128）
      "r": 128,
      "g": 0,
      "b": 128
    },
    "4": { // 橙色（255, 128, 0）
      "r": 255,
      "g": 128,
      "b": 0
    },
    "5": { // 红色（255, 0, 0）
      "r": 255,
      "g": 0,
      "b": 0
    },
    "6": { // 金色（255, 215, 0）
      "r": 255,
      "g": 215,
      "b": 0
    },
    "7": { // 粉色（255, 105, 180）
      "r": 255,
      "g": 105,
      "b": 180
    },
    "8": { // 金色（255, 215, 0）
      "r": 255,
      "g": 215,
      "b": 0
    },
    "9": { // 青色（0, 255, 255）
      "r": 0,
      "g": 255,
      "b": 255
    },
    "10": { // 粉色（255, 105, 180）
      "r": 255,
      "g": 105,
      "b": 180
    },
    "11": { // 深紫色（75, 0, 130）
      "r": 75,
      "g": 0,
      "b": 130
    }
  }
}
```

## 反馈
- 本插件优先找原仓库：[Github](https://github.com/itsFrankV22/ItemsDeco-Plugin)
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
