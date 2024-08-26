# EndureBoost items for a long time after a certain number of buff.

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Gan Di Xi En
- Source: None
- When some items in the player's backpack reach a certain number, give them a designated buff.
- Enter the server refresh, command refresh, death refresh, and return to the city refresh.

## Update log

```
暂无
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/ebbuff，/ldbuff，/loadbuff|EndureBoost|Refresh the long-term buff status immediately.|

## deploy
> Configuration file path: tshock/EndureBoost.json
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

## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.