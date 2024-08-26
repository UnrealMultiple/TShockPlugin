# Economics.npc plugin custom monster reward

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Si Ming
- Source: None
- Configure NPC exclusive reward

> [! Note]
> You need to install front plug -in: Economicsapi (this warehouse)

## Update log

```
无
```

## instruction

none

## Configuration
> Configuration file location: TSHOCK/ECONOMICS/NPC.JSON
```json
{
   "开启提示": true,
   "提示内容": "你因击杀{0},获得额外奖励{1}{2}个",
   "额外奖励列表": [
    {
       "怪物ID": 390,
       "怪物名称": "猪鲨",
       "奖励货币": 100000,
       "按输出瓜分": true // false 时每个人发10000奖励
    }
  ],
   "转换率更改": {
     "50": 1.3 //id 和 转换率
  }
}
```

## feedback

- Co -maintained plug -in library: https://github.com/Controllerdestiny/tshockplugin
- The official group of Trhub.cn or Tshock in the domestic community