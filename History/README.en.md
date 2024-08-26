# History record graphic operation

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Author: Cracker64 & Zaicon & Cai
- Source: None
- Record each player's operation of the graphit
- This allows you to use instructions to restore the graphic
- Or reproduce the player's operation
- Avoid some cases from the intention of the player's intentional damage that the game cannot be performed normally

## Update log

```
v1.0.3
修复了后台单独使用/history会报错的问题，现在无法在后台单独使用/history指令

v1.0.2
修改了初始化的优先级

v1.0.1
加了hreset指令
```

## instruction

|grammar|Alias|Authority|illustrate|
| -------------------------------- |: ---:|: ----------------:|: ----------------------------------------:|
|/history [Account Name] [Time] [Scope]|/history|hestory.get|Restore a user's behavior in time span and radius|
|/prunehist [Time]|/delete|hestory.prune|Delete a historical record of a time span|
|/Reenact [Account Name] [Time] [Scope]|/Reproduce|hestory.reenAct|Create all the actions of the player in time span and radius|
|/rollback [Account Name] [Time] [range]|Trace|hestory.rollback|Restore all the movements of the player in time span and radius|
|/rollback [Account Name] [Time] [range]|/Revoke|hestory.rollback|Restore all the movements of the player in time span and radius|
|/hreset|/Reset history|hestory.admin|Reset the database table|

---
Note
---
1. 1.`time` format `dd:hh:mm:ss`[天/hour/minute/second]
  
2.`scope` where you are `Diameter number` 

For example, restore feathers `2 hours ago` in the current position `200 square meters` modified the graphic within the range:/rollback Yu Xue 2H 200

## Configuration

```json
暂无
```
## feedback
- Priority to ISSUED-> Commonly maintained plug -in library: https://github.com/Controllerdestiny/tshockPlugin
- Two priority: TSHOCK official group: 816771079
- It may not be seen in a high probability, but it can also be: domestic community trhub.cn, bbstr.net, tr.monika.love