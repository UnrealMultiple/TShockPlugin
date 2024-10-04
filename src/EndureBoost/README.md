# EndureBoost 拥有指定数量物品给指定buff

- 作者: 肝帝熙恩
- 出处: 本仓库
- 当玩家背包药水或某些物品到达一定数量后，给与指定buff
- 进入服务器刷新，指令刷新，死亡刷新，回城刷新

## 更新日志

```
v1.0.2
其他物品类型内支持多个buff，完成i18n
```

## 指令

| 语法           |        权限         |   说明   |
| -------------- | :-----------------: | :------: |
| /ebbuff 或 /ldbuff 或 /loadbuff 或 /刷新buff| EndureBoost| 立即刷新本插件buff状态|

## 配置
> 配置文件路径：tshock/EndureBoost.json
```json
{
  "猪猪储钱罐": false,// 示例
  "保险箱": false,
  "护卫熔炉": false,
  "虚空宝藏袋": true,
  "持续时间(s)": 3600,
  "药水": [
    {
      "药水id": [
        288,
        289
      ],//可以是一个数组，也就是里面可以放单个或者多个物品id
      "药水数量": 30
    },
    {
      "药水id": [
        290
      ],
      "药水数量": 200
    }
  ],
  "其他物品": [
    {
      "物品id": [
        2,
        3
      ],
      "物品数量": 3,
      "给buff的id": 87
    },
    {
      "物品id": [
        5
      ],
      "物品数量": 3,
      "给buff的id": 89
    }
  ]
}
```

## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
