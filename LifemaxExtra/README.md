# LifemaxExtra 提升生命值上限

- 作者: 佚名，肝帝熙恩，少司命
- 出处: TShock 官方群聊
- 可自定义使用物品提高血量，突破血量上限
- 因魔力的特殊性实际最大提升只能到 400
- 仅限 SSC

## 更新日志

```
1.修正使用物品判断
2.添加指令/hp 和 /mp
3.添加可自定义物品增加血量
```

## 指令

| 语法                    |       权限       |     说明     |
| ----------------------- | :--------------: | :----------: |
| /hp enh [玩家名] [血量] | lifemaxextra.use | 提升玩家血量 |
| /hp set [玩家名] [血量] | lifemaxextra.use | 设置玩家血量 |
| /hp enh [血量]          | lifemaxextra.use | 提升自身血量 |
| /hp set [血量]          | lifemaxextra.use | 设置自身血量 |
| /mp enh [玩家名] [血量] | lifemaxextra.use | 提升玩家魔力 |
| /mp set [玩家名] [血量] | lifemaxextra.use | 设置玩家魔力 |
| /mp enh [血量]          | lifemaxextra.use | 提升自身魔力 |
| /mp set [血量]          | lifemaxextra.use | 设置自身魔力 |

## 配置
    配置文件位置：tshock/LifemaxExtra.json
```json
{
  "最大生命值": 1000,
  "最大法力值": 1000,
  "提高血量物品": {
    "29": {
      // 物品ID
      "最大提升至": 600, // 使用此物品最大可提升到多少血量或魔力
      "提升数值": 20 //每次使用提升多少血量或者魔力
    },
    "1291": {
      "最大提升至": 700,
      "提升数值": 5
    }
  },
  "提高法力物品": {
    "109": {
      "最大提升至": 700,
      "提升数值": 20
    }
  }
}
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
