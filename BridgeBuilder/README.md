# BridgeBuilder 快速铺桥

- 作者: Soofa，肝帝熙恩
- 出处: [github](https://github.com/Soof4/BridgeBuilder)
- 快速向玩家面向的平行方向铺路，碰到方块则停止
- 可自定义铺桥方块，默认配置内已有：平台，种植盆，团队块

## 更新日志

```
1.0.5
修改添加了墙壁逻辑，填墙壁id就好了
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /bridge 或 /桥来 |  bridgebuilder.bridge  | 快速铺桥指令|

## 配置

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
- 共同维护的插件库：https://github.com/THEXN/TShockPlugin/
- 国内社区trhub.cn 或 TShock官方群等
