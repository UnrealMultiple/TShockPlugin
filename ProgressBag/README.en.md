# Progressbag progress gift package

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Whenever a progress is completed, you can receive a gift package

## Update log

```
v1.0.1.0
完善卸载函数

1. 区分世吞与克脑的进度判断
```

## instruction

|grammar|Authority|illustrate|
| --------------------- |: --------:|: ---------------------:|
|/Gift Package [Gift Pack Name]|bag.use|Receive a gift package|
|/Gift package collection all|bag.use|Receive all the gift packages|
|/Gift Pack List|bag.use|View gift package list|
|/Gift Pack Reset|bag.admin|Reset and receive gift package|

## For specific progress, please check the prOGRESSTYPE.CS file

## Configuration
> Configure file location: TSHOCK/Progress Pack.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love