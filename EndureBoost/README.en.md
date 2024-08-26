# EndureBoost items for a certain number of buffs after a certain amount

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Liver Emperor Xi En
- Source: None
- When the player's backpack reaches a certain number of items, give the specified buff
- Enter the server refresh, instruction refresh, death refresh, return to the city refresh

## Update log

```
暂无
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/ebbuff,/ldbuff,/loadbuff|Endureboost|Refresh the long -term buff state now|

## Configuration
> Configuration file path: tshock/endureBoost.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love