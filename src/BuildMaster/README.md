# BuildMaster 豆沙小游戏·建筑大师
- 作者: [Bean_Paste]https://gitee.com/Crafty/projects,羽学  
- 出处: [bbstr]https://www.bbstr.net/r/117/  
- 这是一款全新的小游戏，基于MiniGamesAPI框架开发，它的玩法类似于MC中的小游戏(速建大比拼)。  
- 玩家将需要在一定时间内完成相对应主题的建筑，时间到后将由每位玩家进行评分，  
- 得分会在游戏结束后进行一个排名。快来发挥你的想象力，把建筑搞起来！  

> [!NOTE]  
> 需要安装前置插件：MiniGamesAPI (本仓库) 



## 指令

| 语法                 |    权限    |      说明      |
|--------------------|:--------:|:------------:|
| /bm list           | bm.user  |    查看房间列表    |
| /bm join 房间ID      | bm.user  |     加入房间     |
| /bm leave          | bm.user  |     离开房间     |
| /bm ready          | bm.user  |    准备/未准备    |
| /bm vote 主题        | bm.user  |     投票主题     |
| /bma list          | bm.admin |    列出所有房间    |
| /bma create 房间名    | bm.admin |     创建房间     |
| /bma remove 房间ID   | bm.admin |    移除指定房间    |
| /bma start 房间ID    | bm.admin |    开启指定房间    |
| /bma stop 房间ID     | bm.admin |    关闭指定房间    |
| /bma smp 房间ID 人数   | bm.admin |  设置房间最大玩家数   |
| /bma sdp 房间ID 人数   | bm.admin |  设置房间最小玩家数   |
| /bma swt 房间ID 时间   | bm.admin | 设置等待时间(单位：秒) |
| /bma sgt 房间ID 时间   | bm.admin | 设置游戏时间(单位：秒) |
| /bma sst 房间ID 时间   | bm.admin | 设置评分时间(单位：秒) |
| /bma sp 1/2        | bm.admin |    选取点1/2    |
| /bma sr 房间ID 主题    | bm.admin |  设置房间的游戏区域   |
| /bma addt 房间ID 主题名 | bm.admin |     添加主题     |
| /bma sh 房间ID 高度    | bm.admin |    设置小区域高    |
| /bma sw 房间ID 宽度    | bm.admin |    设置小区域宽    |
| /bma sg 房间ID 间隔    | bm.admin |   设置小区域间隔    |
| /bma dp 玩家名        | bm.admin | 设置玩家的基础建造背包  |
| /bma ep            | bm.admin |    设置评分套装    |
| /bma reload        | bm.admin | 重载配置文件非房间文件  |

## 配置
> 配置文件位置：tshock/BuildMaster

> Config.json
```json    
{
  "UnlockAll": true,
  "Range": {},
  "BanItem": []
}
```
> default.json
```json5
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
```json5
[]
```

## 更新日志

```
1.0.7
添加 GetString
1.0.5
修正不正确的GetString
1.0.3
i18n预定（这个好多，不想写啊）
1.0.2
完善卸载函数
1.0.1
补全修正卸载函数
1.0.0
记得开SSC，本插件运行在旅行模式服务器中（因为本插件使用到旅行的创造模式）  
设置好基本建筑套装（不用太复杂的,因为有创造模式 直接去里边拿东西），就可以设置评分套装了（插件内置默认 不用多余配置 只需要输入一条指令）  
玩家单独建筑区域 要计算好 加起来不要超过总区域的宽度（玩家建筑区域默认水平创建）不然是生成不了单独建筑区域的  
```


## 反馈
- 优先发issued -> 共同维护的插件库：https://github.com/UnrealMultiple/TShockPlugin
- 次优先：TShock官方群：816771079
- 大概率看不到但是也可以：国内社区trhub.cn ，bbstr.net , tr.monika.love
