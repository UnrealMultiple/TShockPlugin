# pvper duel system

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: SOOFA, Yu Xue
- - 出处: [github](https://github.com/Soof4/PvPer/) 
+ This is a TSHOCK server plug -in mainly used for:
- The PVP combat function among players is implemented. The plug -in uses the SQLite database storage duel data.
- Limit and manage player behavior through the event hook monitoring the game event to ensure that the duel rules are followed.
- At the same time, the plug -in supports configuration file loading and overload.
- Note: Players will be directly killed if they get out of the scope of competition.
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

|grammar|Authority|illustrate|
| -------------- |: --------------------:|: -------:|
|/PVP ADD player name|pvper.use|Invite players to participate|
|/pvp yes|pvper.use|Consent|
|/PVP NO|pvper.use|Refuse to duel|
|/PVP List|pvper.use|Ranking|
|/PVP DATA|pvper.use|Record query|
|/pvp.wl|pvper.use|Check the Banning weapon table|
|/pvp.bl|pvper.use|Check the BUFF table|
|/pvp set 1 2 3 4|pvper.use|Synchronize the current position to the coordinate 1/2 is the player's location 3/4 is the border of the arena|
|/PVP Reset|pvper.admin|Reset the player's database table|
|/pvp.bw add <Weapon Name>|pvper.admin|Specifying weapons in the PVP state|
|/PVP.BB ADD <gain ID>|pvper.admin|Specify the specified gain in the PVP state|


## Configuration
> Configure file location: TSHOCK/Dueling System.json
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
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love