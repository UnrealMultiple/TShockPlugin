# Dethdrop Death drop

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Dadou, Liver Emperor Xien Update and Optimization
- Source: TSHOCK Chinese official group
- Allow custom monsters when the monster dies.
- Random or custom, does not affect each other

## Update log

```
- v1.0.3 使用reload来重载配置，原先是杀一只怪就重载一次，感觉会爆
```

## instruction

```
暂无
```

## Configuration
> Configuration file location: TSHOCK/Death drop configuration table.json
```json
{
   "是否开启随机掉落": false,//随机掉落的总开关，必须设置这个为true能设置除了自定义以外的内容
   "完全随机掉落": false,//完全随机掉落，从1-5452里面选一个或多个物品
   "完全随机掉落排除物品ID": [],//不会选择这里面的物品
   "普通随机掉落物": [],//如果完全随机掉落为false，你可以在这里面自定义所有怪物一起的随机掉落物，随机掉落物从这里面选取
   "随机掉落概率": 100,//概率，同时影响完全随机掉落和普通随机掉落
   "随机掉落数量最小值": 1,//随机掉落数量最小值，同时影响完全随机掉落和普通随机掉落
   "随机掉落数量最大值": 1,//随机掉落数量最小值，同时影响完全随机掉落和普通随机掉落
   "是否开启自定义掉落": false,//自定义掉落，不受上面所有设置的影响，独立作用
   "自定义掉落设置": [
    {
       "生物id": 0,
       "完全随机掉落": false,
       "完全随机掉落排除物品ID": [],
       "普通随机掉落物": [],
       "随机掉落数量最小值": 1,
       "随机掉落数量最大值": 1,
       "掉落概率": 100
    }
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love