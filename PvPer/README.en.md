# Pvper duel system

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Soofa, feather science
- - 出处: [github](https://github.com/Soof4/PvPer/) 
+This is a Tshock server plug-in mainly used for:
- The PvP combat function between players is realized, and the plug-in uses SQLite database to store duel data.
- Monitor game events through event hooks, restrict and manage players' behaviors, and ensure that duel rules are followed.
- At the same time, the plug-in supports configuration file loading and overloading.
- Note: if the player leaves the competition range, he will be killed directly and will be judged to lose.
## Update log

```
1.1.3
完善卸载函数（也许）
1.1.2
优化了/pvp help 翻页
加入了新指令与武器/BUFF的相关检测机制
配置文件更新了【启用检查】、【是否检查第7个饰品栏】、【禁武器表】、【禁BUFF表】（这2个表会自动根据指令写入）

1.1.1
配置文件中加入了【拉取范围】、是否【拉回竞技场】配置项  
拉取范围为0时，传送冲出竞技场的玩家到竞技场中心点  
拉取范围为负数时，拉取玩家到中心点与对应的玩家冲出相对方向  
拉取范围为正数时，拉取玩家到中心点与对应的玩家冲出相反方向  

1.1.0
将[决斗重置]命令放进了/pvp help
加入了死亡嘲讽与连胜统计播报
配置文件加入了离场扣血(关闭死亡惩罚时默认开启)

1.0.3
新增了子命令可快速设置玩家与竞技场的界限（自动写入Config并保存更新）
配置文件加了个【离开竞技场杀死玩家】的开关选项并进行了汉化
修复了离开竞技场判定死亡的设定。

1.0.2
羽学汉化并修改了所有指令与其回馈信息，  
并加入了个新指令权限用于重置玩家数据表
```
## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/pvp add player name|pvper.use|Invite players to a duel|
|/pvp yes|pvper.use|Agree to a duel|
|/pvp no|pvper.use|Refuse a duel|
|/pvp list|pvper.use|ranking list|
|/pvp data|pvper.use|Record inquiry|
|/pvp.WL|pvper.use|Seal up the list of prohibited weapons|
|/pvp.BL|pvper.use|Seal up the forbidden BUFF table|
|/pvp set 1 2 3 4|pvper.use|Synchronizing the current position to coordinate 1/2 is the player's position and 3/4 is the arena boundary.|
|/pvp reset|pvper.admin|Reset the player's database table|
|/pvp.BW add < weapon name >|pvper.admin|Prohibit designated weapons in PVP state|
|/pvp.BB add < gain ID >|pvper.admin|Specified gain in banned PVP state|


## deploy
> Configuration file location: tshock/ dueling system. json
```json
{
   "插件权限名": "pvper.use / pvper.admin",
   "竞技场边界说明": "/pvp set 3 4 要比玩家传送坐标高或低3格设置",
   "竞技场边界说明2": "拉取范围：会从玩家冲出竞技场方向,拉回到竞技场中心的指定反向位置（当为负数则是正向位置）,关闭杀死玩家选项后默认开启扣血",
   "启用检查": true,
   "是否检查第7个饰品栏": false,
   "拉回竞技场": true,
   "拉取范围": -20,
   "离开竞技场杀死玩家": false,
   "离场扣血": 20,
   "邀请者传送坐标.X": 0,
   "邀请者传送坐标.Y": 0,
   "受邀者传送坐标.X": 0,
   "受邀者传送坐标.Y": 0,
   "竞技场左上角坐标.X": 0,
   "竞技场左上角坐标.Y": 0,
   "竞技场右下角坐标.X": 0,
   "竞技场右下角坐标.Y": 0,
   "禁武器表": [],
   "禁BUFF表": []
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.