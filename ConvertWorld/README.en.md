# ConvertWorld defeats monsters and replaces world items.

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: onusai feather science
- - 出处: [tshock-bless-world](https://github.com/onusai/tshock-bless-world) 
- Defeat the specified monster and transform all the pictures and items in the box in the world.

## Update log
```
v1.0.0
修改BlessWorld插件，将bless指令与服务器初始化时自动转换功能移除     
加入了对击杀指定NPC实现转换设定      
加入了从《怪物ID》取值，通过/reload事件重载，自动赋名给《怪物名》    
```
  
## order
```
暂无
```

---
Configuration considerations
---
1.`World graph substitution table` is the graph ID,`Box item replacement table` it's the item ID
  
2.`Monster name` will be based on when you use /reload.`Monster ID` automatic replenishment
  
3.`Monster ID` you can write multiple monsters
  
4.`Beat all` you need to beat `Monster ID` after all the monsters in the room,
as long as there is in the world `Designated graph lattice` or `The contents of the box` killing any one will change,
not if there is no translatable id.`Prompt broadcast`.
  
## deploy
> Configuration file location: tshock/ Defeat monsters and replace world items. json
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
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.