# Bridgebuilder fast paving bridge

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: SOOFA, Liver Emperor Xien
- - 出处: [github](https://github.com/Soof4/BridgeBuilder) 
- Pay the way to players in the parallel direction, and stop
- It can customize bridge paving blocks, and the default configuration already has: platform, planting basin, team block
- Can be placed vertically
- Popular science: <> Inner must be filled, [] is selected and filled
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

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/Bridge [up/down] or/bridge [up/down]|bridebuilder.bridge|Quick Bridge Directive|

## Configuration
> Configuration file location: TSHOCK/Bridge configuration.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love