# LifemaxExtra increases the maximum health.

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Anonymous, Gan Di Xi En, Shao Si Ming
- Source: TShock official group chat
- Customized items can be used to increase blood volume and break through the upper limit of blood volume.
- Due to the particularity of magic, the actual maximum increase can only reach 400.
- SSC only

## Update log

```
v1.0.0.7
完善卸载函数

1.修正使用物品判断
2.添加指令/hp 和 /mp
3.添加可自定义物品增加血量
```

## instruction

|grammar|limit of authority|explain|
| ----------------------- |:--------------:|:----------:|
|/hp enh [player name] [blood volume]|lifemaxextra.use|Increase player's blood volume|
|/hp set [player name] [blood volume]|lifemaxextra.use|Set the player's blood volume|
|/hp enh [blood volume]|lifemaxextra.use|Increase one's blood volume|
|/hp set [blood volume]|lifemaxextra.use|Set your own blood volume|
|/mp enh [player name] [blood volume]|lifemaxextra.use|Enhance the player's magic|
|/mp set [player name] [blood volume]|lifemaxextra.use|Set player magic|
|/mp enh [blood volume]|lifemaxextra.use|Improve one's magic|
|/mp set [blood volume]|lifemaxextra.use|Set one's own magic|

## deploy
> Configuration file location: tshock/LifemaxExtra.json
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

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.