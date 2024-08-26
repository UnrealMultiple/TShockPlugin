# EZPERM convenience authority

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Dadou, Liver Emperor Heen Optimized 1449
- Source: TSHOCK Chinese official group
- One instruction helps Xiaobai to add missing permissions to the initial server, and you can also add the permission in batches
- In fact, you can also directly/group addperm group name permissions 1 permissions 2 permissions 3 3 3

## Update log

```
v1.2.2
加了几个权限
v1.2.1
优化代码，完善卸载函数
```

## instruction

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/inperms or /batch modification permissions|inperms.admin|Batch modification|

## Configuration
> Configuration file location: TSHOCK/EZPERM.JSON
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love