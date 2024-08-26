# Economics.RPG Upgrade Career Plug-in

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Shao Siming
- Source: None
- RPG upgrade plug-in

> [! NOTE]
> Need to install the pre-plug-in: EconomicsAPI (this warehouse)

## Update log

```
V1.0.0.2
- 添加权限economics.rpg.chat，拥有此权限不会改变玩家聊天格式。

V1.0.0.1
- 增加显示信息
- 添加/level reset指令
- 添加自定义消息玩家组
- 添加RPG聊天渐变色
```

## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|/rank [professional name]|economics.rpg.rank|upgrade|
|/reset level|economics.rpg.reset|Resetting occupation|
|/level reset|economics.rpg.admin|reset|

## deploy
> Configuration file location: tshock/economics/rpg.json.
```json
{
   "RPG信息": {
     "战士": {
       "聊天前缀": "[战士]",
       "聊天颜色": [0, 238, 0],
       "聊天后缀": "",
       "聊天格式": "{0}{1}{2}: {3}",
       "升级广播": "恭喜玩家{0}升级至{1}!",
       "进度限制": [],
       "升级指令": [],
       "附加权限": [],
       "升级奖励": [],
       "升级消耗": 1000,
       "父等级": "萌新" 
    },
     "战士2": {
       "聊天前缀": "[战士2]",
       "聊天颜色": [0, 238, 0],
       "聊天后缀": "",
       "聊天格式": "{0}{1}{2}: {3}",
       "升级广播": "恭喜玩家{0}升级至{1}!",
       "进度限制": [],
       "升级指令": [],
       "附加权限": [],
       "升级奖励": [],
       "升级消耗": 1000,
       "父等级": "战士" 
    },
     "重置职业执行命令": [],
     "重置职业广播": "玩家{0}重新选择了职业",
     "重置后踢出": false,
     "默认等级": "萌新" 
  }
}
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.