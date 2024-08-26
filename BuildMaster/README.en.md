# Buildmaster bean paste mini game · architecture master

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: [Bean_Paste] https://gitee.com/crafty/projects, Yu Xue
- Source: [bbstr] https://www.bbstr.net/r/117//117/
- This is a new mini -game, based on the MINIGAMESAPI framework, its gameplay is similar to a small game in MC (quickly build a big competition).
- Players will need to complete the corresponding theme of the subjects within a certain period of time. After the time, each player will be scored.
- The scoring will be ranked after the game. Come and use your imagination and make the building!

> [! Note]
> You need to install front plug -in: minigamesapi (this warehouse)

## Update log

```
1.0.2
完善卸载函数
1.0.1
补全修正卸载函数
1.0.0
记得开SSC，本插件运行在旅行模式服务器中（因为本插件使用到旅行的创造模式）  
设置好基本建筑套装（不用太复杂的,因为有创造模式 直接去里边拿东西），就可以设置评分套装了（插件内置默认 不用多余配置 只需要输入一条指令）  
玩家单独建筑区域 要计算好 加起来不要超过总区域的宽度（玩家建筑区域默认水平创建）不然是生成不了单独建筑区域的  
```

## instruction

|grammar|Authority|illustrate|
| ----------------------------- |: ----------:|: ---------------------:|
|/BM List|BM.USER|View room list|
|/BM Join room id|BM.USER|Join the room|
|/BM Leave|BM.USER|Leave the room|
|/BM Ready|BM.USER|Preparation/Unprepared|
|/bm vote theme|BM.USER|Voting theme|
|/Bma List|bm.admin|List all rooms|
|/Bma Create room name|bm.admin|Create a room|
|/bma remove room ID|bm.admin|Remove the designated room|
|/bma start room ID|bm.admin|Open the designated room|
|/bma stop room ID|bm.admin|Close the designated room|
|/bma SMP room ID number|bm.admin|Set the largest number of players in the room|
|/BMA SDP room ID number|bm.admin|Set the number of players in the room|
|/bma SWT room ID time|bm.admin|Setting waiting time (unit: second)|
|/BMA SGT room ID time|bm.admin|Set the game time (unit: second)|
|/bma sst room ID time|bm.admin|Set the scoring time (unit: second)|
|/BMA SP 1/2|bm.admin|Select point 1/2|
|/BMA SR room id theme|bm.admin|Set the game area of ​​the room|
|/bma addt room ID theme name|bm.admin|Add theme|
|/BMA SH room ID height|bm.admin|Set the high area high|
|/BMA SW Room ID width|bm.admin|Set the width of the community|
|/BMA SG room id interval|bm.admin|Set the community interval|
|/bma DP player name|bm.admin|Set the player's basic construction backpack|
|/BMA EP|bm.admin|Set the scoring set|
|/Bma RELOAD|bm.admin|Non -room file of heavy load configuration file|

## Configuration
> Configuration file location: TSHOCK/BUILDMASTER

> Config.json
```json    
{
   "UnlockAll": true,
   "Range": {},
   "BanItem": []
}
```
> default.json
```json
{
   "Name": "基础套",
   "ID": 2,
   "UnlockedBiomeTorches": 0,
   "HappyFunTorchTime": 0,
   "UsingBiomeTorches": 0,
   "QuestsCompleted": 0,
   "HideVisuals": [
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false
  ],
   "EyeColor": {
     "packedValue": 4283128425,
     "R": 105,
     "G": 90,
     "B": 75,
     "A": 255,
     "PackedValue": 4283128425
  },
   "SkinColor": {
     "packedValue": 4284120575,
     "R": 255,
     "G": 125,
     "B": 90,
     "A": 255,
     "PackedValue": 4284120575
  },
   "ShoeColor": {
     "packedValue": 4282149280,
     "R": 160,
     "G": 105,
     "B": 60,
     "A": 255,
     "PackedValue": 4282149280
  },
   "UnderShirtColor": {
     "packedValue": 4292326560,
     "R": 160,
     "G": 180,
     "B": 215,
     "A": 255,
     "PackedValue": 4292326560
  },
   "ShirtColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "HairColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "PantsColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "Hair": 0,
   "SkinVariant": 0,
   "ExtraSlots": 0,
   "SpawnY": -1,
   "SpawnX": -1,
   "Exists": true,
   "MaxMana": 20,
   "Mana": 20,
   "MaxHP": 100,
   "HP": 100,
   "HairDye": 0,
   "Items": []
}
```
> Eva.json
```json
{
   "Name": "评分套",
   "ID": 3,
   "UnlockedBiomeTorches": 0,
   "HappyFunTorchTime": 0,
   "UsingBiomeTorches": 0,
   "QuestsCompleted": 0,
   "HideVisuals": [
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false,
    false
  ],
   "EyeColor": {
     "packedValue": 4283128425,
     "R": 105,
     "G": 90,
     "B": 75,
     "A": 255,
     "PackedValue": 4283128425
  },
   "SkinColor": {
     "packedValue": 4284120575,
     "R": 255,
     "G": 125,
     "B": 90,
     "A": 255,
     "PackedValue": 4284120575
  },
   "ShoeColor": {
     "packedValue": 4282149280,
     "R": 160,
     "G": 105,
     "B": 60,
     "A": 255,
     "PackedValue": 4282149280
  },
   "UnderShirtColor": {
     "packedValue": 4292326560,
     "R": 160,
     "G": 180,
     "B": 215,
     "A": 255,
     "PackedValue": 4292326560
  },
   "ShirtColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "HairColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "PantsColor": {
     "packedValue": 4287407535,
     "R": 175,
     "G": 165,
     "B": 140,
     "A": 255,
     "PackedValue": 4287407535
  },
   "Hair": 0,
   "SkinVariant": 0,
   "ExtraSlots": 0,
   "SpawnY": -1,
   "SpawnX": -1,
   "Exists": true,
   "MaxMana": 20,
   "Mana": 20,
   "MaxHP": 100,
   "HP": 100,
   "HairDye": 0,
   "Items": []
}
```
> Rooms.json
```json
[]
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love