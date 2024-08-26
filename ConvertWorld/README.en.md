# Convertworld defeats monsters and replace world items

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: Onusai Yu Xue
- - 出处: [TSHOCK-PLESS-Workd](https://github.com/onusai/tshock-bless-world) 
- Defeat the specified monster conversion of all graphics and the items in the box in the world

## Update log
```
v1.0.0
修改BlessWorld插件，将bless指令与服务器初始化时自动转换功能移除     
加入了对击杀指定NPC实现转换设定      
加入了从《怪物ID》取值，通过/reload事件重载，自动赋名给《怪物名》    
```
  
## Order
```
暂无
```

---
Configuration Note
---
1. 1.`World graphite replacement table` it's a graphic ID,`Box item replacement table` is item ID
  
2.`Monster name` it will be based on your use/regoad `Monster ID` automatic supplement
  
3..`Monster ID` you can write multiple monsters
  
4. 4..`Defeat everything` you need to defeat `Monster ID` after all the monsters inside,
as long as the world exists `Specified graphm` or `Items in the box` if you kill any intention 1, you will be converted,
if there is no convertible ID, it will not `Prompt broadcast` essence
  
## Configuration
> Configuration file location: TSHOCK/defeat monsters replace world items .json
```json
{
   "使用说明":"击败指定NPC将世界所有指定图格与箱子内物品对比1:1转换",
   "插件开关": true,
   "击败所有": false,
   "击杀转换表": [
    {
       "怪物名": "血肉墙",
       "怪物ID": [
        113
      ],
       "世界图格替换表": {
         "7": 58,
         "166": 58,
         "6": 107,
         "167": 221,
         "9": 108,
         "168": 222,
         "8": 111,
         "169": 223
      },
       "箱子物品替换表": {
         "9": 621,
         "188": 499,
         "189": 500,
         "964": 534,
         "848": 857
      }
    },
    {
       "怪物名": "世纪之花",
       "怪物ID": [
        262
      ],
       "世界图格替换表": {
         "12": 236,
         "22": 211,
         "204": 211
      },
       "箱子物品替换表": {
         "953": 976,
         "975": 976,
         "29": 1291
      }
    }
  ]
}
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love