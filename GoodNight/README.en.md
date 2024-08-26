# Goodnight curfew

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!

- Authors: Jonesn, feather science, Shao Siming
- Source: None
- It is forbidden to take clothes or summon monsters at the specified time every day (automatically lift the ban and summon monsters when the number of online people meets)
- This plug-in integrates the plug-in functions such as white list, curfew and no summoning monsters.
- According to the number of people online during the curfew, whether it is allowed to summon unbeaten or defeated monsters is judged.
- In case of meeting the number of people online and outside the curfew time:
- Compare the monster ID in 【 Prohibited Monster Table 】 through NPC death events, and automatically assign it to 【 Allowed Summon Table 】 through killing count.
- Convenient curfew time allows players to summon monsters, avoiding the situation that a single person pushes the progress of the server.
- When the number of people online does not meet the number of people required to turn off the ban:
- All online players can reach the designated Region territory through [Open Summoning Area] to summon the monsters in the Allowed Summoning Table.

## Update log

```
v2.7.3
修正一些广播格式
加入了清理《允许召唤表》的指令（/gn clear）

v2.7.2
修复检测到没有配置文件时，创建的配置没有参数
不会因为使用/reload重复写入或覆盖原来参数等问题

v2.7.1
优化了对《允许召唤表》播报细节的空检查

v2.7.0
加入了播报类型切换
(用于修复禁怪表含有自然刷新怪的情况导致广播刷屏问题)
【只播报BOSS或非BOSS】为true则只播报BOSS生成事件，反之只播报非BOSS
【关闭切换播报类型】为true则恢复默认，false则启动上面这个判断


v2.6.0
修改召唤区逻辑（不再关闭击杀计数）
通过击杀计数从《禁召表》获取ID添加到《可召表》
通过《可召表》的ID，允许《召唤区》内召唤。
添加了切换召唤区是否需要所有人判定:
启用则需所有在线人数到召唤区才能召唤出BOSS
或者有一人在召唤区，其他人在任意位置都可以召唤BOSS
可通过配置项自定义召唤区的region领地名

v2.5.0
优化了指令方法
加入了【允许召唤区】（用于切换2种逻辑的开关）
当开启功能时，则关闭原有击杀计数《允许召唤怪物表》功能
且所有在线玩家处于召唤区才能召唤怪物
否则需等宵禁时间过期或满足指定在线人数解禁
关闭后恢复原有宵禁逻辑
PS：需用TS自带的/Region指令创建名为“召唤区”的领地

v2.4.0
加入了根据击杀《禁止怪物生成表》计数，
写入《允许召唤怪物表》与其相关指令
计数要求则在满足在线人数或不在宵禁时间段
由玩家主动击杀存在《禁止怪物生成表》的怪物自动计入（无需手写）
加个配置项与指令，控制击杀什么怪物ID来重置《允许召唤怪物表》

v2.3.0
加入宵禁时间内可召唤已击败怪物
通过监听怪物死亡事件从禁止怪物表中
取值后比对赋值给“已击败进度限制”配置项实现
击败月总后自动清空“已击败进度限制”配置项

v2.2.1
修复移除内置配置项的“集合型”参数引起的指令覆盖参数问题
修复重启服务器覆盖配置参数的问题

v2.2.0
彻底修复Reload覆盖写入怪物ID问题
给弹幕更新方法补充了权限检查
加入了/gn 指令方法控制配置项

v2.1.1
清除无用代码，给断开玩家连接加入全检查

v2.1.0
修复玩家加入服务器拦截方法
加入在线人数判断禁止召唤怪物
将配置项加以描述，并把禁怪物表整理为全进度BOSS的NpcID
修复每次/Reload都会写入一次内置怪物ID问题

v2.0.0
加入了禁止召唤怪物逻辑
羽学适配了.net6.0并重构了大部分方法
```

## instruction

|grammar|another name|limit of authority|explain|
| ------------------------------ |:---:|:--------------:|:--------------------------------------:|
|/gn|/curfew|goodnight.admin|Check the curfew command menu.|
|/gn list|without|goodnight.admin|List all curfew tables|
|/reload|without|tshock.cfg.reload|Overloaded configuration file|
|/gn on|without|goodnight.admin|Turn the curfew function on or off.|
|/gn kick|without|goodnight.admin|Turn the disconnect function on or off.|
|/gn pos|without|goodnight.admin|Turn on or off the summoning area|
|/gn all|without|goodnight.admin|Everyone needs to be present to open or close the calling area.|
|/gn clear|without|goodnight.admin|Clean up the monster ID in the Allowed Summon Table.|
|/gn boss times|without|goodnight.admin|Set the number of times to join the Allow Call Table to kill.|
|/gn reset monster ID|without|goodnight.admin|Set the monster ID that resets the Allowed Summon Table.|
|/gn plr number of people|without|goodnight.admin|Set the online number of people who ignore the "No Monsters Table"|
|/gn add or del monster name|without|goodnight.admin|Add or remove the specified player to the disconnected whitelist.|
|/gn plr add or del player name|without|goodnight.admin|Add or delete the specified monsters in the list of prohibited monsters.|
|/gn time a & b 23:59|/gn time start & stop|goodnight.admin|Set the curfew opening and ending time|
|/region define Summoning Area|without|tshock.admin.region|Set the calling area with TS /Region command.|

## deploy
> Configuration file location: tshock/ curfew.json
```json
{
   "是否关闭宵禁": true,
   "宵禁时间设置(禁怪/断连)": {
     "Start": "00:00:00",
     "Stop": "23:59:59" 
  },
   "宵禁是否断连": false,
   "玩家进服拦截消息": "当前为宵禁时间，无法加入游戏。",
   "踢出玩家断连消息": "到点了，晚安",
   "断连白名单": [
     "羽学" 
  ],
   "关闭禁怪所需人数(设1为关闭)": 3,
   "是否开启召唤区": false,
   "只播报BOSS或非BOSS": true,
   "关闭切换播报类型": true,
   "召唤区的名字": "召唤区",
   "召唤区是否需要所有人": true,
   "计入'允许召唤表'的击杀次数": 2,
   "重置'允许召唤表'的怪物ID": 398,
   "允许召唤表(根据禁怪表ID自动写入)": [
    4
  ],
   "禁止怪物生成表(NpcID)": [
    4,
    13,
    14,
    15,
    35,
    36,
    50,
    113,
    114,
    125,
    126,
    127,
    128,
    129,
    130,
    131,
    134,
    135,
    136,
    222,
    245,
    246,
    247,
    248,
    249,
    262,
    266,
    370,
    396,
    397,
    398,
    400,
    439,
    440,
    422,
    493,
    507,
    517,
    636,
    657,
    668
  ]
}
```
## feedback
- Jointly maintained plug-in library: https:- domestic community trhub.cn or TShock official group, etc.