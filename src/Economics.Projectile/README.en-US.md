# Economics.Projectile Custom pop-ups

- 作者: 少司命
- Source: None
- Customizable projectile to make your weapon more cool!

> [!NOTE]
> requires pre-installed plugins: EconomicsAPI, Economics.RPG (in this repository).

## Commands

None

## Configuration

> configuration file location: tshock/Economics/Projectile.json

```json5
{
  "弹幕触发": {
    "274": {
      "使用物品时触发": true, //注意下面的布尔值，严格分类不要弄混了
      "来自于召唤物": false,
      "本身就是召唤物": false,
      "召唤物攻击间隔": 15,
      "弹幕数据": [
        {
          "弹幕ID": 132,
          "弹幕击退动态跟随": false,
          "弹幕伤害动态跟随": false,
          "弹幕伤害": 80.0,
          "弹幕击退": 10.0,
          "弹幕射速": 15.0,
          "限制等级": []
        }
      ],
      "备注": ""
    }
  },
  "物品触发 ": {
    "1327": {
      "弹幕数据": [
        {
          "弹幕ID": 132,
          "弹幕击退动态跟随": false,
          "弹幕伤害动态跟随": false,
          "弹幕伤害": 80.0,
          "弹幕击退": 10.0,
          "弹幕射速": 15.0,
          "限制等级": []
        }
      ],
      "物品使用弹药": false,
      "备注": ""
    }
  }
}
```

## 更新日志

```
无
```

## FeedBack

- Github Issue -> TShockPlugin Repo: https://github.com/UnrealMultiple/TShockPlugin
- 国内社区 trhub.cn 或 TShock 官方群等
