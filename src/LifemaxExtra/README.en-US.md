# LifemaxExtra

- Authors: 佚名，肝帝熙恩，少司命
- Source: TShock Official Group Chat
- 可自定义使用物品提高血量，突破血量上限
- Due to the special nature of mana, the actual maximum increase can only be up to 400.
- Only usable when SSC (Forced Survival Mode) is enabled.

## Commands

| Syntax                                                                                                                                 |            Permission            |        Description       |
| -------------------------------------------------------------------------------------------------------------------------------------- | :------------------------------: | :----------------------: |
| /hp enh [player name] [health] | lifemaxextra.use | Increase player's health |
| /hp set [player name] [health] | lifemaxextra.use |          设置玩家血量          |
| /hp enh [health]                                                                   | lifemaxextra.use |    Increase own health   |
| /hp set [health]                                                                   | lifemaxextra.use |          设置自身血量          |
| /mp enh [player name] [mana]   | lifemaxextra.use |  Increase player's mana  |
| /mp set [player name] [mana]   | lifemaxextra.use |     Set player's mana    |
| /mp enh [mana]                                                                     | lifemaxextra.use |     Increase own mana    |
| /mp set [mana]                                                                     | lifemaxextra.use |       Set own mana       |

## Config

> Configuration file location：tshock/LifemaxExtra.en-US.json

```json5
{
  "最大生命值": 1000,
  "最大法力值": 400,
  "提高血量物品": {
    "29": {
      "最大提升至": 600, // 使用此物品最大可提升到多少血量或魔力
      "提升数值": 20 //每次使用提升多少血量或者魔力
    },
    "1291": {
      "最大提升至": 100,
      "提升数值": 5
    }
  },
  "提高法力物品": {
    "109": {
      "最大提升至": 400,
      "提升数值": 20
    }
  }
}
```

## 更新日志

```
v1.0.0.8
SSC下指令无效提示，使用本插件自动更改ts自己config的maxmp和maxhp

v1.0.0.7
完善卸载函数

1.修正使用物品判断
2.添加指令/hp 和 /mp
3.添加可自定义物品增加血量
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- TShock QQ Group: 816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
