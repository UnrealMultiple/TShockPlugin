# BridgeBuilder 快速铺桥

- 作者: Soofa，肝帝熙恩
- 出处: [github](https://github.com/Soof4/BridgeBuilder)
- 快速向玩家面向的平行方向铺路，碰到方块则停止
- 可自定义铺桥方块，默认配置内已有：平台，种植盆，团队块
- 可以竖直放了
- 科普：<>内是必填，[]内是选填
## 更新日志

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

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /bridge [up/down] 或 /桥来 [up/down]|  bridgebuilder.bridge  | 快速铺桥指令|

## 配置
> 配置文件位置：tshock/铺桥配置.json
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
## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/Controllerdestiny/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
