# Ezperm convenience permissions

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Soybean Seed, Gan Di Xi En Optimization 1449
- Source: TShock Chinese official group
- One instruction helps Xiaobai to add missing permissions to the initial server, and can also add and delete permissions in batches.
- In fact, you can also directly /group addperm group name permission 1 permission 2 permission 3.

## Update log

```
v1.2.2
加了几个权限
v1.2.1
优化代码，完善卸载函数
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/inperms or/batch change permissions|inperms.admin|Batch change authority|

## deploy
> Configuration file location: tshock/ezperm.json
```json
{
   "Groups": [
    {
       "组名字": "default",
       "添加的权限": [
         "tshock.world.movenpc",
         "tshock.tp.pylon",
         "tshock.tp.demonconch",
         "tshock.tp.magicconch",
         "tshock.tp.tppotion",
         "tshock.tp.rod",
         "tshock.npc.startdd2",
         "tshock.tp.wormhole",
         "tshock.npc.summonboss",
         "tshock.npc.startinvasion",
         "tshock.world.time.usesundial" 
      ],
       "删除的权限": [
         "tshock.admin" 
      ]
    }
  ]
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.