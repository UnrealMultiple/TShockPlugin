# DisableGodMod prevents players from being invincible

> Warning: This page is translated by MACHINE, which may lead to POOR QUALITY or INCORRECT INFORMATION, please read with CAUTION!


- Authors: GK, feather science
- Source: QQ group (232109072)
- The main purpose of plug-ins is to monitor and prevent players from using invincible or other cheating behaviors in the game.
- By monitoring the changes of players' health value, defense value, stealth state, evasion state and treatment effect,
- Kick out the offending player. At the same time, in order to avoid concurrency problems,
- Synchronize and lock the player data container.
- Analyze and judge the player's behavior, and when it is found that it is suspected of being invincible or illegally modifying the upper limit of health,
## Update log

```
- 给插件添加了一个权限名，  
- 避免它对服主、超管等用户组也会实施同样的检测惩罚，  
- 并注释掉了它原本可能的封禁手段，带来的玩家不满。  
```
## instruction

|grammar|limit of authority|explain|
| -------------- |:-----------------:|:------:|
|without|Invincible from inspection|Plug-ins do not detect it.|

## deploy

```json
暂无
```
## feedback
- Give priority to issued-> jointly maintained plug-in library: https://github.com/Controllerdestiny/TShockPlugin.
- Second priority: TShock official group: 816771079
- You can't see it with a high probability, but you can: domestic communities trhub.cn, bbstr.net, tr. monika.love.