# History records grid operations

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: Cracker64 & Zaicon & Cai
- Source: None
- Record each player's operation on the graph.
- This allows you to use instructions to restore the graph.
- Or reproduce the player's operation.
- Avoid some situations where the game can't be played normally due to the intentional destruction of the player.

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

|grammar|another name|limit of authority|explain|
| -------------------------------- |:---:|:--------------:|:--------------------------------------:|
|/history [account name] [time] [range]|/history|history.get|Restore a user's behavior in time span and radius.|
|/prunehist [time]|/delete|history.prune|Delete the history over a time span.|
|/reenact [account name] [time] [range]|/reappearance|history.reenact|Recreate all the player's actions within the time span and radius.|
|/rollback [account name] [time] [range]|/backtracking|history.rollback|Restore all the actions of the player within the time span and radius.|
|/rollback [account name] [time] [range]|/cancel|history.rollback|Restore all the actions of the player within the time span and radius.|
|/hreset|/reset history|history.admin|Reset database table|

---
Instructions for attention
---
1.`time` format is `dd:hh:mm:ss`[Day/hour/minute/second]
  
2.`range` for your location `Radius lattice number` 

For example, reductionism `Two hours ago` in the current position `200 grids` the graph grid has been modified in the range: /rollback feather 2h 200.

## deploy

```json
暂无
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.