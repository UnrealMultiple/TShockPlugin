# Veinminer chain mining

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Megghy|Yspoof|MaXthegreat99|Liver Emperor Heen
- - 出处: [github](https://github.com/Maxthegreat99/TSHockVeinMiner) 
- Chain mining, literal meaning
- You can dig a bunch of ores in the chain and explode the specified items
  
> [! Important]
> Enable chain mining requires permissions `veinminer` 
> Authorized command:`/group addperm default veinminer`((`default` for the default group, you can also replace it with the group you need)
## Update log

```
v1.6.0.4
完善卸载函数

v1.6.0.3
添加配置，当矿石上方有指定方块时，不会触发连锁挖矿（避免刷矿）
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/VM|veinminer|Switching chain mining instruction|
|/vm [Any parameter]|veinminer|Switching chain mining prompt message instruction|

## Configuration
> Configuration file location: TSHOCK/VEINMINER.JSON
```json
{
   "启用": true,
   "广播": true,
   "放入背包": true,
   "矿石类型": [
    7,
    166,
    6,
    167,
    9,
    168,
    8,
    169,
    37,
    22,
    204,
    56,
    58,
    107,
    221,
    108,
    222,
    111,
    223,
    211,
    408,
    123,
    224,
    404,
    178,
    63,
    64,
    65,
    66,
    67,
    68
  ],
   "兑换规则": [
    {
       "仅给予物品": false,
       "最小尺寸": 0,
       "类型": 0,
       "物品": null
    }
  ]
}
```
### Exemplary example
```json
{
   "启用": true,
   "广播": true,
   "放入背包": true,
   "矿石类型": [
    7,
    166,
    6,
    167,
    9,
    168,
    8,
    169,
    37,
    22,
    204,
    56,
    58,
    107,
    221,
    108,
    222,
    111,
    223,
    211,
    408,
    123,
    224,
    404,
    178,
    63,
    64,
    65,
    66,
    67,
    68
  ],
   "兑换规则": [
    {
       "仅给予物品": true,
       "最小尺寸": 10,
       "类型": 168,
       "物品": {
         "666": 1,
         "669": 1
      }
    },
    {
       "仅给予物品": true,
       "最小尺寸": 10,
       "类型": 8,
       "物品": {
         "662": 5,
         "219": 1
      }
    }
  ]
}
```


## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love