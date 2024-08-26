# BuildMaster red bean paste games and master architect

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Author: [bean _ paste] https://gitee.com/craft/projects, feather science
- Source: [bbstr] https://www.bbstr.net/r/117/
- This is a brand-new mini-game, which is developed based on MiniGamesAPI framework, and its gameplay is similar to the mini-game in MC (Quick Build Competition).
- Players will need to complete the construction of the corresponding theme within a certain period of time, and each player will score after the time is up.
- Scores will be ranked after the game. Come and use your imagination and get the building up!

> [! NOTE]
> Need to install the pre-plug-in: MiniGamesAPI (this warehouse)

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

|grammar|limit of authority|explain|
| ----------------------------- |:---------:|:--------------------:|
|/bm list|bm.user|View room list|
|/bm join room ID|bm.user|Join the room|
|/bm leave|bm.user|leave the room|
|/bm ready|bm.user|Ready/not ready|
|/bm vote theme|bm.user|Voting theme|
|/bma list|bm.admin|List all rooms|
|/bma create room name|bm.admin|Create a room|
|/bma remove room ID|bm.admin|Remove the specified room|
|/bma start room ID|bm.admin|Open the designated room|
|/bma stop room ID|bm.admin|Close the designated room|
|/bma smp room ID number of people|bm.admin|Set the maximum number of players in the room.|
|/bma sdp Room ID Number of people|bm.admin|Set the minimum number of players in the room|
|/bmawt room ID time|bm.admin|Set the waiting time (unit: seconds)|
|/bma sgt room ID time|bm.admin|Set the game time (unit: seconds)|
|/bma sst room ID time|bm.admin|Set the scoring time (unit: seconds)|
|/bma sp 1/2|bm.admin|Select point 1/2.|
|/bma sr room ID theme|bm.admin|Set the game area of the room.|
|/bma addt room ID theme name|bm.admin|Add a theme|
|/bma sh room ID height|bm.admin|Set the small area height|
|/bma sw room ID width|bm.admin|Set the small area width|
|/bma sg room ID interval|bm.admin|Set small area interval|
|/bma dp player name|bm.admin|Set the player's foundation to build a backpack.|
|/bma ep|bm.admin|Set the scoring package|
|/bma reload|bm.admin|Overloaded configuration file is not a room file.|

## deploy
> Configuration file location: tshock/BuildMaster

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
> eva.json
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
> rooms.json
```json
[]
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.