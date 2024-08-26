# Lifemaxextra to increase the maximum life value

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Anonymous, Liver Emperor Xien, Shao Life
- Source: TSHOCK official group chat
- Can customize the use of items to increase the blood volume and break the upper limit of the blood volume
- Due to the special nature of magic power, the maximum improvement can only reach 400
- Only SSC

## Update log

```
v1.0.0.7
完善卸载函数

1.修正使用物品判断
2.添加指令/hp 和 /mp
3.添加可自定义物品增加血量
```

## instruction

|grammar|Authority|illustrate|
| ----------------------- |: ----------------:|: -----------:|
|/HP ENH [Player Name] [Blood Volume]|lifemaxextra.use|Increase player blood volume|
|/hp set [Player name] [Blood volume]|lifemaxextra.use|Set player blood volume|
|/hp enh [blood volume]|lifemaxextra.use|Increase your own blood volume|
|/hp set [blood volume]|lifemaxextra.use|Set your own blood volume|
|/mp enh [Player name] [Blood volume]|lifemaxextra.use|Enhance the magic of the player|
|/mp set [Player name] [Blood volume]|lifemaxextra.use|Set up player magic|
|/mp enh [blood volume]|lifemaxextra.use|Improve your magic|
|/mp set [blood volume]|lifemaxextra.use|Set your own magic|

## Configuration
> Configuration file location: TSHOCK/LIFEMAXExtra.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love