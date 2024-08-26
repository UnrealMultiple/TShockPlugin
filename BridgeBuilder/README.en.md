# BridgeBuilder quick paving bridge

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Soofa, Gan Di Xi 'en
- - 出处: [github](https://github.com/Soof4/BridgeBuilder) 
- Quickly pave the way in the parallel direction facing the player, and stop when touching the box.
- You can customize the paving box, and the default configuration already includes: platform, planting basin and team block.
- You can put it vertically.
- Popular science: < > is required, [] is optional.
## Update log

```
1.0.8
补全卸载函数
1.0.7
修复 地图边缘搭桥导致崩溃和竖直方向距离没有正常被限制（目前不知道为什么水平少两块，竖直少一块）

1.0.6
添加竖直方向

1.0.5
修改添加了墙壁逻辑，填墙壁id就好了
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/bridge [up/down] or/bridge [up/down]|bridgebuilder.bridge|Fast bridge laying instruction|

## deploy
> Configuration file location: tshock/ bridge paving configuration.json.
```json
{
   "允许快速铺路方块id": [//注意！是图格id，不是物品id，可以在wiki上搜到图格id
    19,
    380,
    427,
    435,
    436,
    437,
    438,
    439
  ],
   "一次性最长铺多少格": 256
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.