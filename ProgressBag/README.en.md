# ProgressBag progress package

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Siming
- Source: None
- You can get a gift package every time you complete a progress.

## Update log

```
v1.0.1.0
完善卸载函数

1. 区分世吞与克脑的进度判断
```

## instruction

|grammar|limit of authority|explain|
| --------------------- |:-------:|:----------------:|
|/Gift Package Collection [Gift Package Name]|bag.use|Receive a gift package|
|/gift package to receive all|bag.use|Collect all available gift packages|
|/gift package list|bag.use|View gift package list|
|/gift package reset|bag.admin|Reset the gift package|

## Please check the ProgressType.cs file for specific progress.

## deploy
> Configuration file location: tshock/ progress package. json
```json
{
   "礼包": [
    {
       "礼包名称": "无限制礼包",
       "进度限制": ["无限制"],
       "礼包奖励": [
        {
           "netID": 29,
           "prefix": 0,
           "stack": 5
        },
        {
           "netID": 22,
           "prefix": 0,
           "stack": 99
        }
      ],
       "执行命令": [],
       "已领取玩家": [],
       "可领取组": []
    },
    {
       "礼包名称": "闪退补偿礼包",
       "进度限制": [],
       "礼包奖励": [
        {
           "netID": 8,
           "prefix": 0,
           "stack": 5
        }
      ],
       "执行命令": [],
       "已领取玩家": [],
       "可领取组": []
    },
    {
       "礼包名称": "史莱姆王礼包",
       "进度限制": ["史莱姆王"],
       "礼包奖励": [
        {
           "netID": 2430,
           "prefix": 0,
           "stack": 1
        }
      ],
       "执行命令": [],
       "已领取玩家": [],
       "可领取组": []
    },
    {
       "礼包名称": "克苏鲁之眼礼包",
       "进度限制": ["克苏鲁之眼"],
       "礼包奖励": [
        {
           "netID": 3097,
           "prefix": 0,
           "stack": 1
        }
      ],
       "执行命令": [],
       "已领取玩家": [],
       "可领取组": []
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.